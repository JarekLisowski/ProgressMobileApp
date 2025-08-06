call env.cmd
net use \\192.168.1.37\Deploy
xcopy /E /I /Y "Backend" "%target%\backend"
pause