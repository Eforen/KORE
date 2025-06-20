#!/bin/bash

# KORE RISC-V Compiler - Kuick Test Script (Structured Output)
# This script builds and runs tests for the Kuick components and outputs results in JSON/YAML format

# Configuration (default to Debug, can be overridden)
CONFIGURATION=${1:-Debug}
OUTPUT_FORMAT=${2:-json}  # json or yaml

# Initialize variables
TIMESTAMP=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
START_TIME=$(date +%s)
TEMP_DIR=$(mktemp -d)
BUILD_LOG="$TEMP_DIR/build.log"
TEST_LOG="$TEMP_DIR/test.log"

# Cleanup function
cleanup() {
    rm -rf "$TEMP_DIR"
}
trap cleanup EXIT

# Function to check dependencies
check_dependencies() {
    local missing_deps=()
    
    if ! command -v jq &> /dev/null; then
        missing_deps+=("jq")
    fi
    
    if ! command -v dotnet &> /dev/null; then
        missing_deps+=("dotnet")
    fi
    
    if [ "$OUTPUT_FORMAT" = "yaml" ] && ! command -v yq &> /dev/null; then
        missing_deps+=("yq (for YAML output)")
    fi
    
    if [ ${#missing_deps[@]} -ne 0 ]; then
        local error_json='{"error": "Missing dependencies", "missing": [], "timestamp": "'$TIMESTAMP'"}'
        for dep in "${missing_deps[@]}"; do
            error_json=$(echo "$error_json" | jq --arg dep "$dep" '.missing += [$dep]')
        done
        echo "$error_json"
        exit 1
    fi
}

# Function to build component
build_component() {
    local component="$1"
    local project_path="$2"
    local start_time=$(date +%s)
    
    echo "Building $component..." >&2
    
    if dotnet build "$project_path" --configuration "$CONFIGURATION" --verbosity minimal >> "$BUILD_LOG" 2>&1; then
        local end_time=$(date +%s)
        local duration=$((end_time - start_time))
        echo '{"component": "'$component'", "status": "success", "duration": "'${duration}s'"}'
    else
        local end_time=$(date +%s)
        local duration=$((end_time - start_time))
        local error_msg=$(tail -20 "$BUILD_LOG" | tr '\n' ' ' | tr '"' "'")
        echo '{"component": "'$component'", "status": "failed", "duration": "'${duration}s'", "error": "'"$error_msg"'"}'
        return 1
    fi
}

# Function to parse test output
parse_test_output() {
    local test_output="$1"
    
    # Extract summary line (e.g., "Total tests: 139. Passed: 71. Failed: 68. Skipped: 0.")
    local summary_line=$(echo "$test_output" | grep -E "Total tests:|Passed:|Failed:|Skipped:" | tail -1)
    
    local total=0
    local passed=0
    local failed=0
    local skipped=0
    
    if [ -n "$summary_line" ]; then
        total=$(echo "$summary_line" | grep -oE "Total tests: [0-9]+" | grep -oE "[0-9]+" || echo "0")
        passed=$(echo "$summary_line" | grep -oE "Passed: [0-9]+" | grep -oE "[0-9]+" || echo "0")
        failed=$(echo "$summary_line" | grep -oE "Failed: [0-9]+" | grep -oE "[0-9]+" || echo "0")
        skipped=$(echo "$summary_line" | grep -oE "Skipped: [0-9]+" | grep -oE "[0-9]+" || echo "0")
    fi
    
    # Extract individual test results
    local test_cases='[]'
    
    # Look for test case results in the output
    while IFS= read -r line; do
        if [[ "$line" =~ ^[[:space:]]*PseudoInstructions\(.*\) ]]; then
            local test_name=$(echo "$line" | sed -E 's/^[[:space:]]*([^[:space:]]+).*/\1/')
            local status="Unknown"
            
            # Look for status indicators
            if echo "$test_output" | grep -A5 -B5 "$test_name" | grep -q "Passed"; then
                status="Passed"
            elif echo "$test_output" | grep -A5 -B5 "$test_name" | grep -q "Failed"; then
                status="Failed"
            elif echo "$test_output" | grep -A5 -B5 "$test_name" | grep -q "Skipped"; then
                status="Skipped"
            fi
            
            local test_case='{"name": "'"$test_name"'", "status": "'"$status"'"}'
            test_cases=$(echo "$test_cases" | jq ". += [$test_case]")
        fi
    done <<< "$test_output"
    
    echo '{"total": '$total', "passed": '$passed', "failed": '$failed', "skipped": '$skipped', "test_cases": '"$test_cases"'}'
}

# Main execution
check_dependencies

echo "Running Kuick tests..." >&2

# Navigate to source directory
cd "$(dirname "$0")/src"

# Get dotnet version
DOTNET_VERSION=$(dotnet --version)

# Build components
echo "Building components..." >&2
builds='[]'

for component_info in "Kore.Utility:Kore.Utility/Kore.Utility.csproj" \
                     "Kore.AST:Kore.AST/Kore.AST.csproj" \
                     "Kore.RiscMeta:Kore.RiscMeta/Kore.RiscMeta.csproj" \
                     "Kore.Kuick:Kore.Kuick/Kore.Kuick.csproj"; do
    IFS=':' read -r component project <<< "$component_info"
    
    if build_result=$(build_component "$component" "$project"); then
        builds=$(echo "$builds" | jq ". += [$build_result]")
    else
        builds=$(echo "$builds" | jq ". += [$build_result]")
        # Exit on build failure
        build_failed=true
        break
    fi
done

# Run tests if builds succeeded
test_start=$(date +%s)
overall_status="success"
test_results='{"total": 0, "passed": 0, "failed": 0, "skipped": 0, "test_cases": []}'

if [ -z "$build_failed" ]; then
    echo "Running tests..." >&2
    
    if test_output=$(dotnet test Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --configuration "$CONFIGURATION" --verbosity normal 2>&1); then
        if echo "$test_output" | grep -q "Failed.*[1-9]"; then
            overall_status="failed"
        fi
    else
        overall_status="failed"
    fi
    
    # Parse test results
    test_results=$(parse_test_output "$test_output")
    echo "$test_output" > "$TEST_LOG"
else
    overall_status="failed"
fi

test_end=$(date +%s)
test_duration=$((test_end - test_start))

# Calculate total duration
END_TIME=$(date +%s)
TOTAL_DURATION=$((END_TIME - START_TIME))

# Build final JSON result
result=$(jq -n \
    --arg timestamp "$TIMESTAMP" \
    --arg configuration "$CONFIGURATION" \
    --arg dotnet_version "$DOTNET_VERSION" \
    --argjson builds "$builds" \
    --argjson tests "$test_results" \
    --arg test_duration "${test_duration}s" \
    --arg overall_status "$overall_status" \
    --arg duration "${TOTAL_DURATION}s" \
    '{
        timestamp: $timestamp,
        configuration: $configuration,
        dotnet_version: $dotnet_version,
        builds: $builds,
        tests: ($tests + {duration: $test_duration}),
        overall_status: $overall_status,
        duration: $duration,
        artifacts_location: ("Kore.Kuick.Tests/bin/" + $configuration)
    }')

# Output in requested format
if [ "$OUTPUT_FORMAT" = "yaml" ]; then
    if command -v yq &> /dev/null; then
        echo "$result" | yq eval -P
    else
        echo "$result" | jq -r '
            def format_value(v): 
                if type == "string" then v
                elif type == "number" then v | tostring
                elif type == "boolean" then v | tostring
                elif type == "array" then "[" + (map(format_value(.)) | join(", ")) + "]"
                elif type == "object" then "{" + (to_entries | map("\(.key): \(format_value(.value))") | join(", ")) + "}"
                else v | tostring end;
            to_entries | map("\(.key): \(format_value(.value))") | .[]'
    fi
else
    echo "$result" | jq .
fi

# Exit with appropriate code
if [ "$overall_status" = "success" ]; then
    exit 0
else
    exit 1
fi 