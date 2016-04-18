nuget.exe restore "..\FullyTypedExample.sln"
"C:\Program Files (x86)\MSBuild\12.0\bin\MSBuild.exe" "..\FullyTypedExample.WebApi.SelfHosted\FullyTypedExample.WebApi.SelfHosted.proj" /v:minimal
"..\FullyTypedExample.WebApi.SelfHosted\bin\Debug\FullyTypedExample.WebApi.SelfHosted.exe" --swagger "swagger.json"