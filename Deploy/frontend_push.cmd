call env.cmd
rmdir /s /q "%target%\Frontend"
mkdir "%target%\Frontend"
xcopy /E /I /Y "Frontend" "%target%\Frontend"
pause