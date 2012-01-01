@echo off
cd /D %~dp0
set msbuild=%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
if [%1]==[] (set config=Debug) else (set config=%1)
%msbuild% ..\Compare\Compare.sln /t:Rebuild /p:Configuration=%config%;Platform="Any CPU"
exit /b %%ERRORLEVEL%%