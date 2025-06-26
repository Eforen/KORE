#!/bin/bash

# Simple script to generate TRX file for all AST tests
# Usage: ./GenerateAst-trx.sh
# Output: TestResults/AST.trx

set -e

# Get the directory where this script is located (repo root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$SCRIPT_DIR"

# CD into the repo root to ensure we're in the correct location
cd "$REPO_ROOT"

echo "Working from repo root: $REPO_ROOT"

# Ensure output directory exists
mkdir -p TestResults

echo "Generating Ast test results..."

# Change to source directory
cd src

# Build and run all tests, output TRX to workspace root
dotnet test Kore.AST.Test/Kore.AST.Tests.csproj --configuration Release --verbosity minimal --logger "trx;LogFileName=$(pwd)/../TestResults/AST.trx" | true

# Return to workspace root
cd "$REPO_ROOT"

echo "TRX file generated at TestResults/AST.trx"

# Remove absolute paths from TRX file to make it generic
echo "Removing absolute paths from TRX file..."
if [[ -f "TestResults/AST.trx" ]]; then
    # Replace all instances of the repo root path and its subdirectories with relative paths (case-insensitive)
    # This handles cases like /full/path/to/repo/src/ -> src/
    sed -i "s|$REPO_ROOT/||gI" "TestResults/AST.trx"
    # Also handle case where repo root appears without trailing slash (case-insensitive)
    sed -i "s|$REPO_ROOT||gI" "TestResults/AST.trx"
    
    echo "Absolute paths removed from TRX file"
else
    echo "Warning: TRX file not found at TestResults/AST.trx"
fi

# Generate markdown summary
echo "Generating markdown summary..."
./GenerateSummary.sh Ast

# Strip GUIDs and timestamps from TRX file and convert to XML
echo "Stripping GUIDs and timestamps from TRX file..."
if [[ -f "TestResults/AST.trx" ]]; then
    
    # Strip GUIDs and timestamps, convert to XML with sorted test results
    ./strip-xml-guids.sh --sort "TestResults/AST.trx" "TestResults/Ast.xml"
    
    # Remove the original TRX file since we now have the clean XML version
    rm "TestResults/AST.trx"
    
    echo "Clean XML file generated at TestResults/Ast.xml"
else
    echo "Warning: TRX file not found, skipping GUID/timestamp stripping"
fi

echo "Done!" 