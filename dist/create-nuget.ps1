try{
  C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe build.proj
  
  if (!$?){
     throw $error[0].Exception
  }
  
  ..\.nuget\nuget.exe install NUnit.Runners -Version 2.6.2 -o ..\packages
  Copy-Item nunit-console.exe.config ..\packages\NUnit.Runners.2.6.2\tools\
  if (!$?){
     throw $error[0].Exception
  }
  
  Echo "Testing Portable"
  ..\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe /framework:net-4.5 /noxml /nodots /labels /stoponerror /exclude=Performance ..\Tests\bin\Release\Tests.dll
 
  if (!$?){
     throw $error[0].Exception
  }
  
  Echo "Testing Net40"
  ..\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe /framework:net-4.0 /noxml /nodots /labels /stoponerror /exclude=Performance ..\Tests\bin\Release.net40\Tests.dll

  if (!$?){
     throw $error[0].Exception
  }
}catch{
  Echo "Build Failed"
  exit
}

..\.nuget\nuget.exe pack ..\Dynamitey\Dynamitey.csproj -Properties Configuration=Release -Symbols
Echo "Nuget Success"