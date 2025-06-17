#!/bin/bash

# KORE RISC-V Compiler - Linux Clean Script
# This script cleans all build artifacts and temporary files

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

print_status "Cleaning KORE RISC-V Compiler build artifacts..."

# Navigate to source directory
cd "$(dirname "$0")/src"

# Clean using dotnet CLI
if command -v dotnet &> /dev/null; then
    print_status "Cleaning solution using dotnet CLI..."
    dotnet clean KorePlatform.sln --verbosity quiet
else
    print_warning "dotnet CLI not found, performing manual cleanup..."
fi

# Manual cleanup of common build artifacts
print_status "Removing build directories..."

# Remove bin and obj directories
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true

# Remove test results
print_status "Removing test results..."
find . -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true

# Remove NuGet package cache (local)
print_status "Removing local package cache..."
find . -name "packages" -type d -exec rm -rf {} + 2>/dev/null || true

# Remove Visual Studio specific files
print_status "Removing IDE specific files..."
find . -name "*.user" -type f -delete 2>/dev/null || true
find . -name "*.suo" -type f -delete 2>/dev/null || true
find . -name ".vs" -type d -exec rm -rf {} + 2>/dev/null || true

# Remove temporary files
print_status "Removing temporary files..."
find . -name "*.tmp" -type f -delete 2>/dev/null || true
find . -name "*.temp" -type f -delete 2>/dev/null || true
find . -name "*~" -type f -delete 2>/dev/null || true

# Remove coverage reports
print_status "Removing coverage reports..."
find . -name "coverage.*.xml" -type f -delete 2>/dev/null || true
find . -name "*.coverage" -type f -delete 2>/dev/null || true

print_success "Clean completed successfully!"
print_status "All build artifacts and temporary files have been removed." 