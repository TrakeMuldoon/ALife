#!/usr/bin/env bash
cd "$(dirname "$0")"

TIMESTAMP=$(date +%y%m%d_%H%M)
LOGFILE="PerfTest_${TIMESTAMP}.log"

echo "Running RunPerfTest -> $LOGFILE"
dotnet test Core/ALife.Tests/ALife.Tests.csproj -c Release -v quiet --filter "TestCategory=Performance" --logger "console;verbosity=detailed" > "$LOGFILE" 2>&1

echo "Done."
