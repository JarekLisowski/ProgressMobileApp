call env.cmd
rmdir /s /q "%target%\Frontend"
mkdir "%target%\Frontend"
xcopy /E /I "Frontend" "%target%\Frontend"
pause