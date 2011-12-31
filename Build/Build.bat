@echo off
cd /D %~dp0
set msbuild=%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
set config=Debug
%msbuild% ..\Compare\Compare.sln /t:Rebuild /p:Configuration=%config%;Platform="Any CPU"
exit /b %%ERRORLEVEL%%