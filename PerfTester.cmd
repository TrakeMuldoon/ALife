@echo off
setlocal
cd /d "%~dp0"

for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format 'yyMMdd_HHmm'"') do set TIMESTAMP=%%I
set LOGFILE=PerfTest_%TIMESTAMP%.log

echo Running RunPerfTest -^> %LOGFILE%
dotnet test Core\ALife.Tests\ALife.Tests.csproj -c Release -v quiet --filter "TestCategory=Performance" --logger "console;verbosity=detailed" > "%LOGFILE%" 2>&1

echo Done.
endlocal
