@echo off
set nunit=..\3rdParty\NUnit\nunit-console-x86.exe
set opencover=..\3rdParty\OpenCover\OpenCover.Console.exe
if [%1]==[] (set config=Debug) else (set config=%1)
%opencover% -register:user -target:%nunit% -targetargs:""/noshadow" "/xml=ShyAlex.Compare.Tests.xml" "..\Compare\ShyAlex.Compare.Tests\bin\%config%\ShyAlex.Compare.Tests.dll"" -output:ShyAlex.Compare.Coverage.xml -filter:+[ShyAlex.Compare]*
exit /b %errorlevel%