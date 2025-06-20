#!/bin/bash

# Script to strip GUIDs, timestamps, durations, and computer names from XML test result files to reduce git repository entropy
# Usage: ./strip-xml-guids.sh [--sort] [input.xml] [output.xml]

set -euo pipefail

# Function to show usage
show_usage() {
    echo "Usage: $0 [--sort] <input.xml> <output.xml>"
    echo "  This script removes all GUID attributes, timestamps, durations, and computer names from XML test result files"
    echo ""
    echo "Options:"
    echo "  --sort    Sort UnitTestResult elements by testName for deterministic ordering"
    echo ""
    echo "Examples:"
    echo "  $0 TestResults/Kuick.trx TestResults/Kuick.xml"
    echo "  $0 --sort TestResults/Kuick.trx TestResults/Kuick.xml"
    echo "  $0 TestResults/results.xml TestResults/results-clean.xml"
}

# Parse arguments
SORT_TESTS=false
while [[ $# -gt 0 ]]; do
    case $1 in
        --sort)
            SORT_TESTS=true
            shift
            ;;
        -h|--help)
            show_usage
            exit 0
            ;;
        *)
            break
            ;;
    esac
done

# Check remaining arguments
if [ $# -ne 2 ]; then
    show_usage
    exit 1
fi

INPUT_FILE="$1"
OUTPUT_FILE="$2"

# Check if input file exists
if [ ! -f "$INPUT_FILE" ]; then
    echo "Error: Input file '$INPUT_FILE' does not exist."
    exit 1
fi

echo "Processing XML file: $INPUT_FILE"
echo "Output will be saved to: $OUTPUT_FILE"
if [ "$SORT_TESTS" = true ]; then
    echo "Will sort UnitTestResult elements by testName using XMLStarlet"
fi

# Process the file with sed to remove all GUID attributes and timestamps
sed -E '
    # Remove id attribute from TestRun (both namespaced and non-namespaced)
    s/(<(ns0:)?TestRun[^>]*)\s+id="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"([^>]*>)/\1\3/g
    
    # Remove id attribute from TestSettings (both namespaced and non-namespaced)
    s/(<(ns0:)?TestSettings[^>]*)\s+id="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"([^>]*>)/\1\3/g
    
    # Remove id attribute from UnitTest elements
    s/(<(ns0:)?UnitTest[^>]*)\s+id="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"([^>]*>)/\1\3/g
    
    # Remove id attribute from Execution elements
    s/(<(ns0:)?Execution[^>]*)\s+id="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"([^>]*>)/\1\3/g
    
    # Remove id attribute from TestList elements
    s/(<(ns0:)?TestList[^>]*)\s+id="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"([^>]*>)/\1\3/g
    
    # Remove executionId attribute
    s/\s+executionId="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"//g
    
    # Remove testId attribute
    s/\s+testId="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"//g
    
    # Remove testType attribute
    s/\s+testType="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"//g
    
    # Remove testListId attribute
    s/\s+testListId="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"//g
    
    # Remove relativeResultsDirectory attribute with GUID values
    s/\s+relativeResultsDirectory="[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}"//g
    
    # Remove startTime attributes
    s/\s+startTime="[^"]*"//g
    
    # Remove endTime attributes
    s/\s+endTime="[^"]*"//g
    
    # Remove computerName attributes
    s/\s+computerName="[^"]*"//g
    
    # Remove duration attributes
    s/\s+duration="[^"]*"//g
    
    # Remove computer name and timestamp from TestRun name attribute (keep just @)
    s/(name=")@[^"]*"/\1@"/g
    
    # Remove computer name and timestamp from runDeploymentRoot attribute
    s/(runDeploymentRoot=")[^"]*"/\1testrun"/g
' "$INPUT_FILE" > "$OUTPUT_FILE"

# Sort UnitTestResult elements if requested using XMLStarlet
if [ "$SORT_TESTS" = true ]; then
    echo "Sorting UnitTestResult elements by testName attribute using XMLStarlet..."
    
    TEMP_SORTED="${OUTPUT_FILE}.tmp.sorted"
    TEMP_XSLT="${OUTPUT_FILE}.tmp.xslt"
    
    # Create XSLT for sorting UnitTestResult elements
    cat > "$TEMP_XSLT" << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:mstest="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
    
    <xsl:output method="xml" indent="yes" encoding="UTF-8"/>
    
    <!-- Copy all elements by default -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>
    
    <!-- Sort UnitTestResult elements within Results -->
    <xsl:template match="mstest:Results">
        <xsl:copy>
            <xsl:apply-templates select="@*"/>
            <xsl:text>
    </xsl:text>
            <xsl:for-each select="mstest:UnitTestResult">
                <xsl:sort select="@testName"/>
                <xsl:copy-of select="."/>
                <xsl:text>
    </xsl:text>
            </xsl:for-each>
        </xsl:copy>
    </xsl:template>
    
</xsl:stylesheet>
EOF
    
    # Apply XSLT transformation
    if xmlstarlet tr "$TEMP_XSLT" "$OUTPUT_FILE" > "$TEMP_SORTED" 2>/dev/null && [ -s "$TEMP_SORTED" ]; then
        mv "$TEMP_SORTED" "$OUTPUT_FILE"
        echo "XMLStarlet sorting completed successfully."
    else
        echo "Warning: XMLStarlet transformation failed, retaining original order."
        rm -f "$TEMP_SORTED"
    fi
    
    # Clean up temporary XSLT file
    rm -f "$TEMP_XSLT"
fi

echo "Successfully processed XML file!"
echo "Removed all GUID attributes, timestamps, durations, and computer names:"
echo "  - TestRun id attribute removed"
echo "  - TestSettings id attribute removed" 
echo "  - UnitTest id attributes removed"
echo "  - Execution id attributes removed"
echo "  - TestList id attributes removed"
echo "  - executionId attributes removed"
echo "  - testId attributes removed"
echo "  - testType attributes removed"
echo "  - testListId attributes removed"
echo "  - relativeResultsDirectory GUID attributes removed"
echo "  - startTime attributes removed"
echo "  - endTime attributes removed"
echo "  - computerName attributes removed"
echo "  - duration attributes removed"
echo "  - TestRun name sanitized"
echo "  - runDeploymentRoot sanitized"
if [ "$SORT_TESTS" = true ]; then
    echo "  - UnitTestResult elements sorted by testName using XMLStarlet"
fi

# Show file size comparison and test count
ORIGINAL_SIZE=$(wc -c < "$INPUT_FILE")
NEW_SIZE=$(wc -c < "$OUTPUT_FILE")
TEST_COUNT=$(grep -c '<UnitTestResult' "$OUTPUT_FILE")
echo "File size reduced from $ORIGINAL_SIZE to $NEW_SIZE bytes"
echo "Test count: $TEST_COUNT"

echo "Done!" 