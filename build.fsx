#!/bin/sh
#if bin_sh
  # Doing this because arguments can't be used with /usr/bin/env on linux, just mac
  exec fsharpi --define:mono_posix --exec $0 $*
#endif
#if FSharp_MakeFile

(*
 * Single File Crossplatform FSharp Makefile Bootstrapper
 * Apache licensed - Copyright 2014 Jay Tuley <jay+code@tuley.name>
 * v 2.0 https://gist.github.com/jbtule/11181987
 *
 * How to use:
 *  On Windows `fsi --exec build.fsx <buildtarget>
 *    *Note:* if you have trouble first run "%vs120comntools%\vsvars32.bat" or use the "Developer Command Prompt for VS201X"
 *                                                           or install https://github.com/Iristyle/Posh-VsVars#posh-vsvars
 *
 *  On Mac Or Linux `./build.fsx <buildtarget>`
 *    *Note:* But if you have trouble then use `sh build.fsx <buildtarget>`
 *
 *)

#I "packages/FAKE/tools"
#r "FakeLib.dll"

open Fake

let sln = "./Dynamitey.sln"

let commonBuild target =
    let buildMode = getBuildParamOrDefault "buildMode" "Release"
    let setParams defaults =
            { defaults with
                Verbosity = Some(Quiet)
                Targets = [target]
                Properties =
                    [
                        "Optimize", "True"
                        "DebugSymbols", "True"
                        "Configuration", buildMode
                    ]
             }
    build setParams sln
          |> DoNothing

Target "Restore" (fun () ->
    trace " --- Restore Packages --- "

    //because nuget doesn't know how to find msbuild15 on linux 
    let restoreProj = fun args ->
                   directExec (fun info ->
                       info.FileName <- "msbuild"
                       info.Arguments <- "/t:restore " + args) |> ignore

    sln |> restoreProj
 
)

Target "Clean" (fun () ->
    trace " --- Cleaning stuff --- "
    commonBuild "Clean"
)

Target "Build" (fun () ->
    trace " --- Building the libs --- "
    commonBuild "Build"
)

Target "Test" (fun () ->
    trace " --- Test the libs --- "
    let testDir = "./Tests/bin/Release/net462/"
    !! (testDir + "Tests.dll")
               |> NUnit (fun p ->
                         { p with
                               ToolPath = "./packages/NUnit.Runners/2.6.2/tools"
                               //DisableShadowCopy = true;
                               ExcludeCategory = "Performance"
                               OutputFile = testDir + "TestResults.xml" })
     
    let appveyor = environVarOrNone "APPVEYOR_JOB_ID"
    match appveyor with
        | Some(jobid) -> 
            use webClient = new System.Net.WebClient()
            webClient.UploadFile(sprintf "https://ci.appveyor.com/api/testresults/nunit/%s" jobid, testDir + "TestResults.xml") |> ignore
        | None -> ()
    
    
)

"Restore"
  ==> "Build"
  ==> "Test"

RunTargetOrDefault "Test"


#else

open System
open System.IO
open System.Diagnostics

(* helper functions *)
#if mono_posix
#r "Mono.Posix.dll"
open Mono.Unix.Native
let applyExecutionPermissionUnix path =
    let _,stat = Syscall.lstat(path)
    Syscall.chmod(path, FilePermissions.S_IXUSR ||| stat.st_mode) |> ignore
#else
let applyExecutionPermissionUnix path = ()
#endif

let doesNotExist path =
    path |> Path.GetFullPath |> File.Exists |> not

let execAt (workingDir:string) (exePath:string) (args:string seq) =
    let processStart (psi:ProcessStartInfo) =
        let ps = Process.Start(psi)
        ps.WaitForExit ()
        ps.ExitCode
    let fullExePath = exePath |> Path.GetFullPath
    applyExecutionPermissionUnix fullExePath
    let exitCode = ProcessStartInfo(
                        fullExePath,
                        args |> String.concat " ",
                        WorkingDirectory = (workingDir |> Path.GetFullPath),
                        UseShellExecute = false)
                   |> processStart
    if exitCode <> 0 then
        exit exitCode
    ()

let exec = execAt Environment.CurrentDirectory

let downloadNugetTo path =
    let fullPath = path |> Path.GetFullPath;
    if doesNotExist fullPath then
        printf "Downloading NuGet..."
        use webClient = new System.Net.WebClient()
        fullPath |> Path.GetDirectoryName |> Directory.CreateDirectory |> ignore
        webClient.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", path |> Path.GetFullPath)
        printfn "Done."

let passedArgs = fsi.CommandLineArgs.[1..] |> Array.toList

(* execution script customize below *)

let makeFsx = fsi.CommandLineArgs.[0]

let nugetExe = ".nuget/NuGet.exe"
let fakeExe = "packages/FAKE/tools/FAKE.exe"

downloadNugetTo nugetExe

if doesNotExist fakeExe then
    exec nugetExe ["install"; "fake"; "-OutputDirectory packages"; "-ExcludeVersion"]

exec fakeExe ([makeFsx; "-d:FSharp_MakeFile"] @ passedArgs)

#endif