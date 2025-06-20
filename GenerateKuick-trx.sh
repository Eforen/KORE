#!/bin/bash

# Simple script to generate TRX file for all Kuick tests
# Usage: ./GenerateKuick-trx.sh
# Output: TestResults/Kuick.trx

set -e

# Get the directory where this script is located (repo root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$SCRIPT_DIR"

# CD into the repo root to ensure we're in the correct location
cd "$REPO_ROOT"

echo "Working from repo root: $REPO_ROOT"

# Ensure output directory exists
mkdir -p TestResults

echo "Generating Kuick test results..."

# Change to source directory
cd src

# Build and run all tests, output TRX to workspace root
dotnet test Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --configuration Release --verbosity minimal --logger "trx;LogFileName=$(pwd)/../TestResults/Kuick.trx" | true

# Return to workspace root
cd "$REPO_ROOT"

echo "TRX file generated at TestResults/Kuick.trx"

# Remove absolute paths from TRX file to make it generic
echo "Removing absolute paths from TRX file..."
if [[ -f "TestResults/Kuick.trx" ]]; then
    # Replace all instances of the repo root path and its subdirectories with relative paths (case-insensitive)
    # This handles cases like /full/path/to/repo/src/ -> src/
    sed -i "s|$REPO_ROOT/||gI" "TestResults/Kuick.trx"
    # Also handle case where repo root appears without trailing slash (case-insensitive)
    sed -i "s|$REPO_ROOT||gI" "TestResults/Kuick.trx"
    
    echo "Absolute paths removed from TRX file"
else
    echo "Warning: TRX file not found at TestResults/Kuick.trx"
fi

# Generate markdown summary
echo "Generating markdown summary..."
./GenerateSummary.sh 

# Strip GUIDs and timestamps from TRX file and convert to XML
echo "Stripping GUIDs and timestamps from TRX file..."
if [[ -f "TestResults/Kuick.trx" ]]; then
    
    # Strip GUIDs and timestamps, convert to XML with sorted test results
    ./strip-xml-guids.sh --sort "TestResults/Kuick.trx" "TestResults/Kuick.xml"
    
    # Remove the original TRX file since we now have the clean XML version
    rm "TestResults/Kuick.trx"
    
    echo "Clean XML file generated at TestResults/Kuick.xml"
else
    echo "Warning: TRX file not found, skipping GUID/timestamp stripping"
fi

echo "Done!" 