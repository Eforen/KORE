#!/bin/bash

# KORE RISC-V Compiler - Kuick Test Script (JSON Output)
# This script builds and runs tests for the Kuick components and outputs results in JSON format

set -e  # Exit on any error

# Configuration (default to Debug, can be overridden)
CONFIGURATION=${1:-Debug}
OUTPUT_FORMAT=${2:-json}  # json or yaml

# Initialize test results
TIMESTAMP=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
START_TIME=$(date +%s)
TEST_RESULTS='{"timestamp":"'$TIMESTAMP'","configuration":"'$CONFIGURATION'","dotnet_version":"","builds":[],"tests":{"total":0,"passed":0,"failed":0,"skipped":0,"duration":"","test_cases":[]},"overall_status":"","duration":"","errors":[]}'

# Function to add error to results
add_error() {
    local error_msg="$1"
    local component="$2"
    TEST_RESULTS=$(echo "$TEST_RESULTS" | jq --arg msg "$error_msg" --arg comp "$component" '.errors += [{"component": $comp, "message": $msg, "timestamp": (now | strftime("%Y-%m-%dT%H:%M:%SZ"))}]')
}

# Function to update build status
update_build_status() {
    local component="$1"
    local status="$2"
    local duration="$3"
    TEST_RESULTS=$(echo "$TEST_RESULTS" | jq --arg comp "$component" --arg stat "$status" --arg dur "$duration" '.builds += [{"component": $comp, "status": $stat, "duration": $dur}]')
}

# Function to check if jq is available
if ! command -v jq &> /dev/null; then
    echo '{"error": "jq is not installed. Please install jq to use JSON output format.", "timestamp": "'$TIMESTAMP'"}' 
    exit 1
fi

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo '{"error": "dotnet CLI is not installed. Please install .NET SDK first.", "timestamp": "'$TIMESTAMP'", "help": "Visit: https://docs.microsoft.com/en-us/dotnet/core/install/linux"}'
    exit 1
fi

# Get and store dotnet version
DOTNET_VERSION=$(dotnet --version)
TEST_RESULTS=$(echo "$TEST_RESULTS" | jq --arg version "$DOTNET_VERSION" '.dotnet_version = $version')

# Navigate to source directory
cd "$(dirname "$0")/src"

# Function to build component and capture result
build_component() {
    local component="$1"
    local project_path="$2"
    
    local build_start=$(date +%s)
    local build_output
    local build_status
    
    if build_output=$(dotnet build "$project_path" --configuration "$CONFIGURATION" --verbosity minimal 2>&1); then
        build_status="success"
    else
        build_status="failed"
        add_error "$build_output" "$component"
    fi
    
    local build_end=$(date +%s)
    local build_duration=$((build_end - build_start))
    
    update_build_status "$component" "$build_status" "${build_duration}s"
    
    if [ "$build_status" = "failed" ]; then
        return 1
    fi
    return 0
}

# Build all components
build_component "Kore.Utility" "Kore.Utility/Kore.Utility.csproj" || exit 1
build_component "Kore.AST" "Kore.AST/Kore.AST.csproj" || exit 1
build_component "Kore.RiscMeta" "Kore.RiscMeta/Kore.RiscMeta.csproj" || exit 1
build_component "Kore.Kuick" "Kore.Kuick/Kore.Kuick.csproj" || exit 1

# Run tests and capture detailed results
test_start=$(date +%s)
test_output_file=$(mktemp)

# Run tests with JSON logger
if dotnet test Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --configuration "$CONFIGURATION" --logger "json;LogFilePath=$test_output_file" --verbosity minimal > /dev/null 2>&1; then
    overall_status="success"
else
    overall_status="failed"
fi

test_end=$(date +%s)
test_duration=$((test_end - test_start))

# Parse test results if the JSON file exists and is valid
if [ -f "$test_output_file" ] && [ -s "$test_output_file" ]; then
    # Read the JSON test results
    test_json_content=$(cat "$test_output_file")
    
    # Extract test statistics from the JSON
    if echo "$test_json_content" | jq empty 2>/dev/null; then
        # Parse individual test results
        test_cases=$(echo "$test_json_content" | jq '[.[] | select(.MessageLevel == "Informational" and .Message.TestResult?) | {
            name: .Message.TestResult.TestCase.FullyQualifiedName,
            outcome: .Message.TestResult.Outcome,
            duration: .Message.TestResult.Duration,
            error_message: (.Message.TestResult.ErrorMessage // null),
            display_name: (.Message.TestResult.TestCase.DisplayName // .Message.TestResult.TestCase.FullyQualifiedName)
        }]')
        
        # Count test results
        total_tests=$(echo "$test_cases" | jq 'length')
        passed_tests=$(echo "$test_cases" | jq '[.[] | select(.outcome == "Passed")] | length')
        failed_tests=$(echo "$test_cases" | jq '[.[] | select(.outcome == "Failed")] | length')
        skipped_tests=$(echo "$test_cases" | jq '[.[] | select(.outcome == "Skipped")] | length')
        
        # Update test results
        TEST_RESULTS=$(echo "$TEST_RESULTS" | jq --argjson total "$total_tests" --argjson passed "$passed_tests" --argjson failed "$failed_tests" --argjson skipped "$skipped_tests" --arg duration "${test_duration}s" --argjson cases "$test_cases" '
            .tests.total = $total |
            .tests.passed = $passed |
            .tests.failed = $failed |
            .tests.skipped = $skipped |
            .tests.duration = $duration |
            .tests.test_cases = $cases
        ')
    fi
fi

# Clean up temp file
rm -f "$test_output_file"

# Calculate total duration
END_TIME=$(date +%s)
TOTAL_DURATION=$((END_TIME - START_TIME))

# Finalize results
TEST_RESULTS=$(echo "$TEST_RESULTS" | jq --arg status "$overall_status" --arg duration "${TOTAL_DURATION}s" '
    .overall_status = $status |
    .duration = $duration
')

# Output results based on format
if [ "$OUTPUT_FORMAT" = "yaml" ]; then
    if command -v yq &> /dev/null; then
        echo "$TEST_RESULTS" | yq eval -P
    else
        echo "$TEST_RESULTS" | jq -r 'to_entries | map("\(.key): \(.value)") | .[]'
    fi
else
    # Default to JSON
    echo "$TEST_RESULTS" | jq .
fi

# Exit with appropriate code
if [ "$overall_status" = "success" ]; then
    exit 0
else
    exit 1
fi 