call "c:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
call env.cmd
msbuild ..\Backend\Progress.Api\Progress.Api.csproj /t:Publish /p:Configuration=Release /p:PublishDir=%deploy_dir%\Backend 

rem /p:Platform=x64
pause