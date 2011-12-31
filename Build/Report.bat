@echo off
set reportgen=..\3rdParty\ReportGenerator\bin\ReportGenerator.exe
%reportgen% ShyAlex.Compare.Coverage.xml Reports
exit /b %errorlevel%