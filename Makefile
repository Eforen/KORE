# KORE RISC-V Compiler - Makefile
# Provides convenient targets for building, testing, and cleaning

.PHONY: all build test clean setup help debug release

# Default configuration
CONFIGURATION ?= Debug
VERBOSITY ?= minimal

# Default target
all: build

# Build the solution
build:
	@echo "Building KORE RISC-V Compiler ($(CONFIGURATION))..."
	@cd src && dotnet build KorePlatform.sln --configuration $(CONFIGURATION) --verbosity $(VERBOSITY)

# Build in Debug configuration
debug:
	@$(MAKE) build CONFIGURATION=Debug

# Build in Release configuration
release:
	@$(MAKE) build CONFIGURATION=Release

# Run all tests
test: build
	@echo "Running tests ($(CONFIGURATION))..."
	@cd src && dotnet test KorePlatform.sln --configuration $(CONFIGURATION) --no-build --verbosity normal

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	@cd src && dotnet clean KorePlatform.sln --verbosity quiet
	@find src -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
	@find src -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true
	@find src -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true

# Setup development environment
setup:
	@echo "Setting up development environment..."
	@chmod +x setup.sh build.sh test.sh clean.sh
	@./setup.sh

# Restore NuGet packages
restore:
	@echo "Restoring NuGet packages..."
	@cd src && dotnet restore KorePlatform.sln --verbosity quiet

# Check .NET SDK installation
check:
	@echo "Checking .NET SDK installation..."
	@dotnet --version || (echo "ERROR: .NET SDK not found. Run 'make setup' to install." && exit 1)

# Show help
help:
	@echo "KORE RISC-V Compiler - Available targets:"
	@echo ""
	@echo "  make build      - Build the solution (Debug configuration)"
	@echo "  make debug      - Build in Debug configuration"
	@echo "  make release    - Build in Release configuration"
	@echo "  make test       - Run all tests"
	@echo "  make clean      - Clean build artifacts"
	@echo "  make setup      - Setup development environment"
	@echo "  make restore    - Restore NuGet packages"
	@echo "  make check      - Check .NET SDK installation"
	@echo "  make help       - Show this help message"
	@echo ""
	@echo "Configuration can be overridden:"
	@echo "  make build CONFIGURATION=Release"
	@echo "  make test CONFIGURATION=Release" 