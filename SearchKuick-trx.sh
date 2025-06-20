#!/bin/bash

# Simple script to search TRX file for specific test patterns
# Usage: ./SearchKuick-trx.sh "search_term" [--summary]
# Input: TestResults/Kuick.trx
# Output: Filtered test results

set -e

SEARCH_TERM="$1"
GENERATE_SUMMARY=""

if [ "$2" = "--summary" ]; then
    GENERATE_SUMMARY="true"
fi

TRX_FILE="TestResults/Kuick.trx"

if [ -z "$SEARCH_TERM" ]; then
    echo "Usage: $0 \"search_term\" [--summary]"
    echo "Example: $0 \"rdcycle\""
    echo "Example: $0 \"csrr\" --summary"
    exit 1
fi

if [ ! -f "$TRX_FILE" ]; then
    echo "Error: TRX file not found at $TRX_FILE"
    echo "Please run ./GenerateKuick-trx.sh first"
    exit 1
fi

echo "Searching for tests containing: '$SEARCH_TERM'"
echo "========================================"

# Count total matching tests
TOTAL_MATCHES=$(grep -c "testName=\"[^\"]*$SEARCH_TERM[^\"]*\"" "$TRX_FILE" || echo "0")

if [ "$TOTAL_MATCHES" -eq 0 ]; then
    echo "No tests found matching '$SEARCH_TERM'"
    exit 0
fi

# Count passed and failed tests
PASSED_MATCHES=$(grep "testName=\"[^\"]*$SEARCH_TERM[^\"]*\".*outcome=\"Passed\"" "$TRX_FILE" | wc -l || echo "0")
FAILED_MATCHES=$(grep "testName=\"[^\"]*$SEARCH_TERM[^\"]*\".*outcome=\"Failed\"" "$TRX_FILE" | wc -l || echo "0")

echo "Found $TOTAL_MATCHES tests matching '$SEARCH_TERM'"
echo "✅ Passed: $PASSED_MATCHES"
echo "❌ Failed: $FAILED_MATCHES"
echo ""

# Show failed test details if any
if [ "$FAILED_MATCHES" -gt 0 ]; then
    echo "Failed Test Details:"
    echo "==================="
    
    # Extract failed test info with error messages
    grep "testName=\"[^\"]*$SEARCH_TERM[^\"]*\".*outcome=\"Failed\"" "$TRX_FILE" | while IFS= read -r line; do
        TEST_NAME=$(echo "$line" | sed 's/.*testName="//;s/".*$//')
        # Clean up test name
        CLEAN_NAME=$(echo "$TEST_NAME" | sed 's/.*\.\([^.]*\)(\(.*\))/\1(\2)/')
        echo "❌ $CLEAN_NAME"
        
        # Try to find corresponding error message in the TRX file
        # This is a simplified approach - the TRX format can be complex
        TEST_ID=$(echo "$line" | sed 's/.*testId="//;s/".*$//')
        if [ -n "$TEST_ID" ]; then
            ERROR_INFO=$(grep -A 10 "testId=\"$TEST_ID\"" "$TRX_FILE" | grep -E "(Message|StackTrace)" | head -1 | sed 's/<[^>]*>//g' | sed 's/^[[:space:]]*//' || echo "")
            if [ -n "$ERROR_INFO" ]; then
                echo "   Error: $ERROR_INFO"
            fi
        fi
        echo ""
    done
fi

# Show passed test summary
if [ "$PASSED_MATCHES" -gt 0 ]; then
    echo "Passed Tests:"
    echo "============="
    grep "testName=\"[^\"]*$SEARCH_TERM[^\"]*\".*outcome=\"Passed\"" "$TRX_FILE" | while IFS= read -r line; do
        TEST_NAME=$(echo "$line" | sed 's/.*testName="//;s/".*$//')
        CLEAN_NAME=$(echo "$TEST_NAME" | sed 's/.*\.\([^.]*\)(\(.*\))/\1(\2)/')
        echo "✅ $CLEAN_NAME"
    done
fi

# Generate summary if requested
if [ "$GENERATE_SUMMARY" = "true" ]; then
    echo ""
    echo "Generating full test summary..."
    ./GenerateSummary.sh
fi 