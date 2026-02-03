@echo off
WHERE npm
IF %ERRORLEVEL% NEQ 0 (
  ECHO Node.js is required to build and run this project. Please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE.
  exit /b 1
  pause
)

set SUPPORT_API_PORT=5004

netstat /o /a /n /p TCP | find /i "listening" | find ":%SUPPORT_API_PORT%" >nul 2>nul && (
  echo Port %SUPPORT_API_PORT% is open. Seems the SupportApi service already started.
) || (
  echo Starting the SupportApi service, port %SUPPORT_API_PORT%.
  start cmd.exe @cmd /k supportapi.cmd
)

echo Building the sample
call npm i

set CHECK_COUNT=0

:checksupportapiport
netstat /o /a /n /p TCP | find /i "listening" | find ":%SUPPORT_API_PORT%" >nul 2>nul && (
  echo Starting the sample web server.
) || (
set /A CHECK_COUNT=CHECK_COUNT+1
echo Waiting for the SupportApi service port %SUPPORT_API_PORT% to be ready
TIMEOUT /T 2
if %CHECK_COUNT% LSS 10 (
  goto checksupportapiport
) else (
  echo Port %SUPPORT_API_PORT% is Not open. The SupportApi service is Not ready.
  exit /b 1
)
)

call npm start