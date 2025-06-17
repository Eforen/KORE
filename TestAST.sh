#!/bin/bash

# KORE RISC-V Compiler - AST Test Script
# This script builds and runs tests for the AST components

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

# Display dotnet version
print_status "Using .NET SDK version: $(dotnet --version)"

# Configuration (default to Debug, can be overridden)
CONFIGURATION=${1:-Debug}
print_status "Testing AST components in $CONFIGURATION configuration"

# Navigate to source directory
cd "$(dirname "$0")/src"

# Build dependencies first
print_status "Building AST dependencies..."

# Build Kore.Utility (if AST depends on it)
print_status "Building Kore.Utility..."
dotnet build Kore.Utility/Kore.Utility.csproj --configuration $CONFIGURATION --verbosity minimal

# Build Kore.AST
print_status "Building Kore.AST..."
dotnet build Kore.AST/Kore.AST.csproj --configuration $CONFIGURATION --verbosity minimal

# Build and run Kore.AST.Test
print_status "Building and running Kore.AST.Test..."
dotnet test Kore.AST.Test/Kore.AST.Tests.csproj --configuration $CONFIGURATION --verbosity normal

if [ $? -eq 0 ]; then
    print_success "AST tests completed successfully!"
else
    print_error "AST tests failed!"
    exit 1
fi

print_status "AST test artifacts location: Kore.AST.Test/bin/$CONFIGURATION" 