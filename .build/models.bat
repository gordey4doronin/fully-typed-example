nuget.exe restore "..\FullyTypedExample.sln"
"..\packages\AutoRest.0.15.0\tools\AutoRest.exe" -Input "swagger.json" -CodeGenerator NodeJS
move "Generated\models\index.d.ts" "..\FullyTypedExample.HtmlApp\models.d.ts"
::rmdir "Generated" /s /q