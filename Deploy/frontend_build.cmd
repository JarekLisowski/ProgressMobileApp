call "c:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
call env.cmd

rmdir /s /q "Frontend"
mkdir "Frontend"
cd ..\ProgressApp

ng build --configuration=production --output-path=%deploy_dir%\Frontend
rem  /p:PublishDir=%deploy_dir%\Backend /p:Platform=x64
pause