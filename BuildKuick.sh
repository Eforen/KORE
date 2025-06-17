#!/bin/bash

# KORE RISC-V Compiler - Kuick Build Script
# This script builds the Kuick components and their dependencies

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
print_status "Building Kuick components in $CONFIGURATION configuration"

# Navigate to source directory
cd "$(dirname "$0")/src"

# Restore NuGet packages for Kuick components
print_status "Restoring NuGet packages..."
dotnet restore Kore.Utility/Kore.Utility.csproj --verbosity quiet
dotnet restore Kore.AST/Kore.AST.csproj --verbosity quiet
dotnet restore Kore.RiscMeta/Kore.RiscMeta.csproj --verbosity quiet
dotnet restore Kore.Kuick/Kore.Kuick.csproj --verbosity quiet
dotnet restore Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --verbosity quiet

# Build dependencies first
print_status "Building Kuick dependencies..."

# Build Kore.Utility
print_status "Building Kore.Utility..."
dotnet build Kore.Utility/Kore.Utility.csproj --configuration $CONFIGURATION --no-restore --verbosity minimal

# Build Kore.AST
print_status "Building Kore.AST..."
dotnet build Kore.AST/Kore.AST.csproj --configuration $CONFIGURATION --no-restore --verbosity minimal

# Build Kore.RiscMeta
print_status "Building Kore.RiscMeta..."
dotnet build Kore.RiscMeta/Kore.RiscMeta.csproj --configuration $CONFIGURATION --no-restore --verbosity minimal

# Build Kore.Kuick
print_status "Building Kore.Kuick..."
dotnet build Kore.Kuick/Kore.Kuick.csproj --configuration $CONFIGURATION --no-restore --verbosity minimal

# Build Kore.Kuick.Tests
print_status "Building Kore.Kuick.Tests..."
dotnet build Kore.Kuick.Tests/Kore.Kuick.Tests.csproj --configuration $CONFIGURATION --no-restore --verbosity minimal

if [ $? -eq 0 ]; then
    print_success "Kuick components build completed successfully!"
    print_status "Built artifacts are in the respective bin/$CONFIGURATION directories"
else
    print_error "Kuick components build failed!"
    exit 1
fi

# Display build output locations
print_status "Build output locations:"
echo "  - Kore.Utility/bin/$CONFIGURATION"
echo "  - Kore.AST/bin/$CONFIGURATION"
echo "  - Kore.RiscMeta/bin/$CONFIGURATION"
echo "  - Kore.Kuick/bin/$CONFIGURATION"
echo "  - Kore.Kuick.Tests/bin/$CONFIGURATION" 