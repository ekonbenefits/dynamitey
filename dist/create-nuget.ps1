try{
  
  $solname = "Dynamitey" 
  $testname = "Tests"
  @projectname = $solname
  $projecttype ="csproj"

  #Build PCL and .NET version from one project using msbuild script
  C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe build.proj /p:solname=$solname
  
  if (!$?){
     throw $error[0].Exception
  }
  
  #Download and configure Nunit test runner
  ..\.nuget\nuget.exe install NUnit.Runners -Version 2.6.2 -o ..\packages
  Copy-Item nunit-console.exe.config ..\packages\NUnit.Runners.2.6.2\tools\
  
  if (!$?){
     throw $error[0].Exception
  }
  
  #Test .net 40
  Echo "Testing Net40"
  ..\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe /framework:net-4.0 /noxml /nodots /labels /stoponerror /exclude=Performance ..\$testname\bin\Release.net40\$testname.dll

  if (!$?){
     throw $error[0].Exception
  }
  
  #Test portable
  Echo "Testing Portable"
  ..\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe /framework:net-4.5 /noxml /nodots /labels /stoponerror /exclude=Performance ..\$testname\bin\Release\$testname.dll
 
  if (!$?){
     throw $error[0].Exception
  }
  
 
}catch{
  Echo "Build Failed"
  exit
}

#if successful create nuget package
..\.nuget\nuget.exe pack ..\$projectname\$projectname.$projecttype -Properties Configuration=Release -Symbols
Echo "Nuget Success"