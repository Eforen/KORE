#!/bin/bash

# KORE RISC-V Compiler - Linux Build Script
# This script builds the entire KORE solution on Linux using dotnet CLI

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
print_status "Building in $CONFIGURATION configuration"

# Navigate to source directory
cd "$(dirname "$0")/src"

# Clean previous builds
print_status "Cleaning previous builds..."
dotnet clean KorePlatform.sln --configuration $CONFIGURATION --verbosity quiet

# Restore NuGet packages
print_status "Restoring NuGet packages..."
dotnet restore KorePlatform.sln --verbosity quiet

# Build the solution
print_status "Building solution..."
dotnet build KorePlatform.sln --configuration $CONFIGURATION --no-restore --verbosity minimal

if [ $? -eq 0 ]; then
    print_success "Build completed successfully!"
    print_status "Built artifacts are in the respective bin/$CONFIGURATION directories"
else
    print_error "Build failed!"
    exit 1
fi

# Optional: Display build output locations
print_status "Build output locations:"
find . -name "bin" -type d | while read dir; do
    if [ -d "$dir/$CONFIGURATION" ]; then
        echo "  - $dir/$CONFIGURATION"
    fi
done 