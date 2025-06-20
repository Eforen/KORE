#!/bin/bash

# Simple test script using dotnet's native logging formats
# Usage: ./TestKuick-native.sh [format] [filter]
# Formats: json, trx, xml, console
# Example: ./TestKuick-native.sh json "PseudoInstructions"

set -e

FORMAT=${1:-console}
FILTER=${2:-}
OUTPUT_DIR="TestResults"
TIMESTAMP=$(date -u +"%Y%m%dT%H%M%SZ")

# Ensure output directory exists
mkdir -p "$OUTPUT_DIR"

echo "Running Kuick tests with format: $FORMAT"
echo "Filter: ${FILTER:-'(all tests)'}"
echo "Timestamp: $TIMESTAMP"

# Always generate TRX file at workspace root regardless of format
TRX_FILE="$(pwd)/$OUTPUT_DIR/Kuick.trx"

# Change to source directory
cd src

# Clean previous test results
rm -rf */TestResults/* 2>/dev/null || true

# Build the test project
echo "Building test project..."
dotnet build Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --configuration Release --verbosity quiet

# Prepare test command arguments
TEST_ARGS="Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --configuration Release --verbosity minimal"

# Add filter if specified
if [ -n "$FILTER" ]; then
    TEST_ARGS="$TEST_ARGS --filter \"$FILTER\""
fi

# Add TRX logger
TEST_ARGS="$TEST_ARGS --logger \"trx;LogFileName=$TRX_FILE\""

# Add additional logger based on format
case "$FORMAT" in
    "json")
        # Use console logger with structured output (closest to JSON)
        TEST_ARGS="$TEST_ARGS --logger console;verbosity=normal"
        OUTPUT_FILE="../$OUTPUT_DIR/test-results-$TIMESTAMP.log"
        ;;
    "trx")
        # TRX already added above, no additional logger needed
        OUTPUT_FILE="$TRX_FILE"
        ;;
    "xml")
        # Use junit format for XML
        OUTPUT_FILE="../$OUTPUT_DIR/test-results-$TIMESTAMP.xml"
        TEST_ARGS="$TEST_ARGS --logger \"junit;LogFilePath=$OUTPUT_FILE\""
        ;;
    "console"|*)
        TEST_ARGS="$TEST_ARGS --logger console;verbosity=detailed"
        OUTPUT_FILE="../$OUTPUT_DIR/test-results-$TIMESTAMP.log"
        ;;
esac

echo "Running tests..."
START_TIME=$(date +%s)

if [ "$FORMAT" = "console" ] || [ "$FORMAT" = "json" ]; then
    # Capture output for console/json formats
    eval "dotnet test $TEST_ARGS" 2>&1 | tee "$OUTPUT_FILE"
    TEST_EXIT_CODE=${PIPESTATUS[0]}
else
    # Let dotnet test write directly to file for XML formats
    eval "dotnet test $TEST_ARGS"
    TEST_EXIT_CODE=$?
fi

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))

echo ""
echo "Test run completed in ${DURATION}s"
echo "Exit code: $TEST_EXIT_CODE"
echo "Results saved to: $OUTPUT_FILE"
echo "TRX file saved to: $TRX_FILE"

# Show a summary of the output file
if [ -f "$OUTPUT_FILE" ]; then
    echo ""
    echo "Output file info:"
    echo "  Size: $(wc -c < "$OUTPUT_FILE") bytes"
    echo "  Lines: $(wc -l < "$OUTPUT_FILE") lines"
    
    case "$FORMAT" in
        "trx"|"xml")
            echo "  Format: XML"
            ;;
        *)
            echo "  Format: Text"
            ;;
    esac
fi

exit $TEST_EXIT_CODE 