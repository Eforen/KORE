#!/bin/bash

# KORE RISC-V Compiler - Linux Setup Script
# This script helps set up the development environment on Linux

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

print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}  KORE RISC-V Compiler Setup${NC}"
    echo -e "${BLUE}================================${NC}"
    echo ""
}

# Detect Linux distribution
detect_distro() {
    if [ -f /etc/os-release ]; then
        . /etc/os-release
        DISTRO=$ID
        VERSION=$VERSION_ID
    else
        DISTRO="unknown"
    fi
}

# Install .NET SDK based on distribution
install_dotnet() {
    print_status "Installing .NET SDK..."
    
    case $DISTRO in
        "ubuntu"|"debian")
            print_status "Detected Ubuntu/Debian system"
            
            # Add Microsoft package repository
            wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
            sudo dpkg -i packages-microsoft-prod.deb
            rm packages-microsoft-prod.deb
            
            # Update package list and install .NET SDK
            sudo apt-get update
            sudo apt-get install -y dotnet-sdk-6.0
            ;;
            
        "fedora")
            print_status "Detected Fedora system"
            # Fedora has .NET SDK 8.0 and 9.0 available in the official repos
            sudo dnf install -y dotnet-sdk-8.0
            ;;
            
        "centos"|"rhel")
            print_status "Detected CentOS/RHEL system"
            
            # Add Microsoft package repository
            sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
            
            # Install .NET SDK
            sudo yum install -y dotnet-sdk-6.0
            ;;
            
        "arch"|"manjaro")
            print_status "Detected Arch Linux system"
            sudo pacman -S --noconfirm dotnet-sdk
            ;;
            
        *)
            print_warning "Unknown distribution: $DISTRO"
            print_status "Please install .NET SDK manually from:"
            print_status "https://docs.microsoft.com/en-us/dotnet/core/install/linux"
            return 1
            ;;
    esac
}

# Check if .NET SDK is installed and working
check_dotnet() {
    if command -v dotnet &> /dev/null; then
        DOTNET_VERSION=$(dotnet --version)
        print_success ".NET SDK is installed (version: $DOTNET_VERSION)"
        
        # Check if version is compatible (5.0 or higher recommended)
        MAJOR_VERSION=$(echo $DOTNET_VERSION | cut -d. -f1)
        if [ "$MAJOR_VERSION" -ge 5 ]; then
            print_success ".NET SDK version is compatible"
        else
            print_warning ".NET SDK version might be too old. Consider upgrading to 6.0 or higher."
        fi
        return 0
    else
        print_error ".NET SDK is not installed or not in PATH"
        return 1
    fi
}

# Make scripts executable
setup_scripts() {
    print_status "Making build scripts executable..."
    chmod +x build.sh
    chmod +x test.sh
    chmod +x clean.sh
    chmod +x setup.sh
    print_success "Scripts are now executable"
}

# Main setup process
main() {
    print_header
    
    detect_distro
    print_status "Detected system: $DISTRO $VERSION"
    
    # Check if .NET SDK is already installed
    if check_dotnet; then
        print_status ".NET SDK is already installed and working"
    else
        print_status ".NET SDK not found. Installing..."
        
        if install_dotnet; then
            print_success ".NET SDK installation completed"
            
            # Verify installation
            if check_dotnet; then
                print_success ".NET SDK is working correctly"
            else
                print_error ".NET SDK installation failed or is not working"
                exit 1
            fi
        else
            print_error "Failed to install .NET SDK automatically"
            print_status "Please install .NET SDK manually and run this script again"
            exit 1
        fi
    fi
    
    # Setup build scripts
    setup_scripts
    
    # Test the build
    print_status "Testing the build system..."
    if ./build.sh > /dev/null 2>&1; then
        print_success "Build system is working correctly"
    else
        print_warning "Build test failed. You may need to install additional dependencies."
        print_status "Try running: ./build.sh"
    fi
    
    print_success "Setup completed successfully!"
    echo ""
    print_status "Available commands:"
    echo "  ./build.sh [Debug|Release]  - Build the solution"
    echo "  ./test.sh [Debug|Release]   - Run all tests"
    echo "  ./clean.sh                  - Clean build artifacts"
    echo ""
    print_status "To get started, run: ./build.sh"
}

# Run main function
main "$@" 