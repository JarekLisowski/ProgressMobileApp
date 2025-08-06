call "c:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
call env.cmd
msbuild ..\Backend\Progress.Navireo\Progress.Navireo.csproj /t:Publish /p:Configuration=Debug /p:PublishDir=%deploy_dir%\NavireoApi 

pause