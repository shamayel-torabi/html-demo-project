@echo off
set PROJ_TYPE=%1
if '%PROJ_TYPE%' NEQ 'AspNetCore' (
  if '%PROJ_TYPE%' NEQ 'WebForms' (
    set PROJ_TYPE=AspNetCore
  )
)
title DsPdfViewer SupportApi service

if defined VSDEVCMD goto build
for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property installationPath`) do (
  if exist "%%i\Common7\Tools\VsDevCmd.bat" (
    SET VSDEVCMD="%%i\Common7\Tools\VsDevCmd.bat"
    goto build
  )
)
if not defined VSDEVCMD (
  echo WARNING: Cannot find VsDevCmd.bat. You may need to set VSDEVCMD variable to path to VsDevCmd.bat and run this batch again.
)

:build
call %VSDEVCMD%
if not exist "SupportApiService" (
 call tar -xf WebApi.zip
)
cd WebApi
call msbuild %PROJ_TYPE%.sln /t:Restore /p:Configuration=Release
call msbuild %PROJ_TYPE%.sln  /p:Configuration=Release
cd %PROJ_TYPE%
echo Running the SupportApi service, press Ctrl+C to stop...
dotnet run
