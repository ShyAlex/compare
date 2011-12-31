@echo off
set nunit=..\3rdParty\NUnit\nunit-console-x86.exe
set opencover=..\3rdParty\OpenCover\OpenCover.Console.exe
set config=debug
%opencover% -register:user -target:%nunit% -targetargs:""/noshadow" "/xml=ShyAlex.Compare.Tests.xml" "..\Compare\ShyAlex.Compare.Tests\bin\%config%\ShyAlex.Compare.Tests.dll"" -output:ShyAlex.Compare.Coverage.xml -filter:+[ShyAlex.Compare]*
exit /b %errorlevel%