call "c:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
call env.cmd
msbuild ..\Backend\Progress.Navireo\Progress.Navireo.csproj /t:Publish /p:Configuration=Release /p:PublishDir=%deploy_dir%\NavireoApi /p:Platform=x64
pause