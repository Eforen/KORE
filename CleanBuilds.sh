#!/bin/bash

# KORE RISC-V Compiler - Clean Builds Script
# This script cleans all build artifacts from KORE projects

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

# Configuration (default to clean all, can be overridden)
CONFIGURATION=${1:-""}
if [ -z "$CONFIGURATION" ]; then
    print_status "Cleaning all configurations (Debug and Release)"
else
    print_status "Cleaning $CONFIGURATION configuration"
fi

# Navigate to source directory
cd "$(dirname "$0")/src"

# Clean all projects
print_status "Cleaning all KORE projects..."

# Clean each project individually
projects=(
    "Kore.Utility/Kore.Utility.csproj"
    "Kore.AST/Kore.AST.csproj"
    "Kore.RiscMeta/Kore.RiscMeta.csproj"
    "Kore.Kuick/Kore.Kuick.csproj"
    "Kore.Kuick.Tests/Kore.Kuick.Tests.csproj"
    "Kore.AST.Test/Kore.AST.Test.csproj"
    "KoreLibrary/KoreLibrary.csproj"
    "KoreTests/KoreTests.csproj"
)

for project in "${projects[@]}"; do
    if [ -f "$project" ]; then
        project_name=$(basename "$project" .csproj)
        print_status "Cleaning $project_name..."
        
        if [ -z "$CONFIGURATION" ]; then
            # Clean both Debug and Release
            dotnet clean "$project" --configuration Debug --verbosity quiet 2>/dev/null || true
            dotnet clean "$project" --configuration Release --verbosity quiet 2>/dev/null || true
        else
            # Clean specific configuration
            dotnet clean "$project" --configuration "$CONFIGURATION" --verbosity quiet 2>/dev/null || true
        fi
    else
        print_warning "Project file not found: $project"
    fi
done

# Also remove bin and obj directories manually for thorough cleanup
print_status "Removing bin and obj directories..."
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true

print_success "Build cleanup completed successfully!"
print_status "All build artifacts have been removed." 