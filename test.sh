#!/bin/bash

# KORE RISC-V Compiler - Linux Test Script
# This script runs all tests in the KORE solution on Linux using dotnet CLI

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    print_error "dotnet CLI is not installed. Please install .NET SDK first."
    print_status "Visit: https://docs.microsoft.com/en-us/dotnet/core/install/linux"
    exit 1
fi

# Configuration (default to Debug, can be overridden)
CONFIGURATION=${1:-Debug}
VERBOSITY=${2:-normal}

print_status "Running tests in $CONFIGURATION configuration"
print_status "Using .NET SDK version: $(dotnet --version)"

# Navigate to source directory
cd "$(dirname "$0")/src"

# Build first to ensure everything is up to date
print_status "Building solution before running tests..."
dotnet build KorePlatform.sln --configuration $CONFIGURATION --verbosity quiet

if [ $? -ne 0 ]; then
    print_error "Build failed! Cannot run tests."
    exit 1
fi

# Run tests with detailed output
print_status "Running all tests..."
print_status "Test projects found:"

# List test projects
find . -name "*.Tests.csproj" -o -name "*Tests.csproj" | while read proj; do
    echo "  - $(basename "$(dirname "$proj")")"
done

echo ""

# Run tests for the entire solution
dotnet test KorePlatform.sln \
    --configuration $CONFIGURATION \
    --no-build \
    --verbosity $VERBOSITY \
    --logger "console;verbosity=detailed" \
    --collect:"XPlat Code Coverage"

TEST_RESULT=$?

if [ $TEST_RESULT -eq 0 ]; then
    print_success "All tests passed!"
else
    print_error "Some tests failed!"
    exit $TEST_RESULT
fi

# Optional: Show test result summary
print_status "Test execution completed."
print_status "For detailed test results, check the TestResults directory." 