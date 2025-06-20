#!/bin/bash

# Script to generate a markdown summary from Kuick TRX test results
# Usage: ./GenerateSummary.sh
# Input: TestResults/Kuick.trx or TestResults/Kuick.trx.backup
# Output: TestResults/KUICK.md

set -e

TRX_FILE="TestResults/Kuick.trx"
BACKUP_TRX_FILE="TestResults/Kuick.trx.backup"
XML_FILE="TestResults/Kuick.xml"
OUTPUT_FILE="TestResults/KUICK.md"

# Check for TRX file, fall back to backup, then XML if neither TRX exists
if [ -f "$TRX_FILE" ]; then
    SOURCE_FILE="$TRX_FILE"
elif [ -f "$BACKUP_TRX_FILE" ]; then
    SOURCE_FILE="$BACKUP_TRX_FILE"
elif [ -f "$XML_FILE" ]; then
    SOURCE_FILE="$XML_FILE"
else
    echo "Error: No test results file found at $TRX_FILE, $BACKUP_TRX_FILE, or $XML_FILE"
    echo "Please run ./GenerateKuick-trx.sh first"
    exit 1
fi

echo "Generating markdown summary from $SOURCE_FILE..."

# Create output directory if it doesn't exist
mkdir -p TestResults

# Parse TRX file and generate markdown
cat > "$OUTPUT_FILE" << 'EOF'
# Kuick Test Results Summary

## Overall Statistics

EOF

# Extract overall statistics
TOTAL_TESTS=$(grep -o 'total="[0-9]*"' "$SOURCE_FILE" | head -1 | sed 's/total="//;s/"//')
EXECUTED_TESTS=$(grep -o 'executed="[0-9]*"' "$SOURCE_FILE" | head -1 | sed 's/executed="//;s/"//')
PASSED_TESTS=$(grep -o 'passed="[0-9]*"' "$SOURCE_FILE" | head -1 | sed 's/passed="//;s/"//')
FAILED_TESTS=$(grep -o 'failed="[0-9]*"' "$SOURCE_FILE" | head -1 | sed 's/failed="//;s/"//')

echo "| Metric | Count | Status |" >> "$OUTPUT_FILE"
echo "|--------|-------|--------|" >> "$OUTPUT_FILE"
echo "| Total Tests | $TOTAL_TESTS | ℹ️ |" >> "$OUTPUT_FILE"
echo "| Executed | $EXECUTED_TESTS | ▶️ |" >> "$OUTPUT_FILE"
echo "| Passed | $PASSED_TESTS | ✅ |" >> "$OUTPUT_FILE"
echo "| Failed | $FAILED_TESTS | ❌ |" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"

# Add test class breakdown
echo "## Test Method Breakdown" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"
echo "| Status | Details | Test Method |" >> "$OUTPUT_FILE"
echo "|--------|---------|-------------|" >> "$OUTPUT_FILE"

# Create temporary files for processing
TEMP_CLASSES=$(mktemp)
TEMP_SORTED=$(mktemp)

# Extract test results and group by class
grep -o '<UnitTestResult.*testName="[^"]*".*outcome="[^"]*"' "$SOURCE_FILE" | while IFS= read -r line; do
    TEST_NAME=$(echo "$line" | sed 's/.*testName="//;s/".*$//' | sed 's/&quot;/"/g;s/&amp;/\&/g;s/&lt;/</g;s/&gt;/>/g')
    OUTCOME=$(echo "$line" | sed 's/.*outcome="//;s/".*$//')
    
    # Extract method name (everything before the first parenthesis)
    METHOD_NAME=$(echo "$TEST_NAME" | sed 's/(.*//')
    
    echo "$METHOD_NAME|$OUTCOME"
done > "$TEMP_CLASSES"

# Sort by class name and count occurrences
sort "$TEMP_CLASSES" | uniq -c | sort -k3 > "$TEMP_SORTED"

# Process sorted results
while read count class_outcome; do
    METHOD=$(echo "$class_outcome" | cut -d'|' -f1)
    OUTCOME=$(echo "$class_outcome" | cut -d'|' -f2)
    
    if [ "$OUTCOME" = "Passed" ]; then
        EMOJI="✅"
        STATUS="$count passed"
    else
        EMOJI="❌" 
        STATUS="$count failed"
    fi
    
    echo "| $EMOJI | $STATUS | \`$METHOD\` |" >> "$OUTPUT_FILE"
done < "$TEMP_SORTED"

# Clean up temporary files
rm -f "$TEMP_CLASSES" "$TEMP_SORTED"

# Add detailed test results
echo "## Detailed Test Results" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"
echo "| Status | Error Details | Test Name |" >> "$OUTPUT_FILE"
echo "|--------|---------------|-----------|" >> "$OUTPUT_FILE"

# Create temporary file for test results
TEMP_TESTS=$(mktemp)

# Extract all test results with details
grep -o '<UnitTestResult.*testName="[^"]*".*outcome="[^"]*"' "$SOURCE_FILE" | while IFS= read -r line; do
    TEST_NAME=$(echo "$line" | sed 's/.*testName="//;s/".*$//' | sed 's/&quot;/"/g;s/&amp;/\&/g;s/&lt;/</g;s/&gt;/>/g')
    OUTCOME=$(echo "$line" | sed 's/.*outcome="//;s/".*$//')
    
    # Clean up test name (remove class prefix and parameters)
    CLEAN_NAME=$(echo "$TEST_NAME" | sed 's/.*\.\([^.]*\)(\(.*\))/\1(\2)/')
    
    if [ "$OUTCOME" = "Passed" ]; then
        EMOJI="✅"
        ERROR_DETAILS="N/A"
    else
        EMOJI="❌"
        ERROR_DETAILS="Test failed - see full TRX for details"
    fi
    
    echo "$CLEAN_NAME|$EMOJI|$ERROR_DETAILS"
done > "$TEMP_TESTS"

# Sort by test name and output to markdown
sort "$TEMP_TESTS" | while IFS='|' read -r clean_name emoji error_details; do
    echo "| $emoji | $error_details | \`$clean_name\` |" >> "$OUTPUT_FILE"
done

# Clean up temporary file
rm -f "$TEMP_TESTS"

echo "" >> "$OUTPUT_FILE"
echo "---" >> "$OUTPUT_FILE"
echo "*Generated on $(date)*" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"
echo "📁 **Full results**: [Kuick.xml](./Kuick.xml)" >> "$OUTPUT_FILE"

echo "✅ Markdown summary generated at $OUTPUT_FILE" 