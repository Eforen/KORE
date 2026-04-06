# KORE RISC-V Compiler - Makefile
# Provides convenient targets for building, testing, and cleaning

.PHONY: all build build-solution build-docs test clean setup help debug release \
	build-tools build-tools-binutils build-tools-binutils-readelf \
	version-inc-readelf-major version-inc-readelf-minor version-inc-readelf-patch \
	ast-dump

# Default configuration
CONFIGURATION ?= Debug
VERBOSITY ?= minimal

# Default target
all: build

# Sphinx HTML documentation — same as `cd docs && make local` (Dockerfile.sphinx / kore-sphinx image)
build-docs:
	@$(MAKE) -C docs local SPHINXOPTS="$(SPHINXOPTS)"
	@echo "Documentation built: docs/build/html/index.html"

# Full solution build (entire KorePlatform.sln)
build-solution:
	@echo "Building KORE RISC-V Compiler ($(CONFIGURATION))..."
	@cd src && dotnet build KorePlatform.sln --configuration $(CONFIGURATION) --verbosity $(VERBOSITY)

# Build all component targets (utility, AST, Kuick, tools)
build: build-utility build-ast build-kuick build-tools
	@echo "All build-* component targets completed ($(CONFIGURATION))."

# Build all tools under build-tools-* (e.g. binutils subtree)
build-tools: build-tools-binutils
	@echo "All build-tools-* targets completed ($(CONFIGURATION))."

# Build all tools under build-tools-binutils-* (readelf, etc.)
build-tools-binutils: build-tools-binutils-readelf
	@echo "All build-tools-binutils-* targets completed ($(CONFIGURATION))."

# Kuick readelf (KORE binutils) — uses conditional compile bump script
build-tools-binutils-readelf:
	@echo "Building readelf ($(CONFIGURATION))..."
	@./src/Kuick.Tools/binutils/readelf/build-readelf.sh -c $(CONFIGURATION) --verbosity $(VERBOSITY)

# Bump semantic version for readelf tool (resets compile to 0 per rules)
version-inc-readelf-major:
	@./src/Kuick.Tools/binutils/readelf/Scripts/version-inc.sh readelf major

version-inc-readelf-minor:
	@./src/Kuick.Tools/binutils/readelf/Scripts/version-inc.sh readelf minor

version-inc-readelf-patch:
	@./src/Kuick.Tools/binutils/readelf/Scripts/version-inc.sh readelf patch

# Build in Debug configuration
debug:
	@$(MAKE) build CONFIGURATION=Debug

# Build in Release configuration
release:
	@$(MAKE) build CONFIGURATION=Release

.PHONY: build-utility
# Build Utility components
build-utility:
	@echo "Building Utility components ($(CONFIGURATION))..."
	@./BuildUtility.sh $(CONFIGURATION)

.PHONY: build-ast
# Build AST components
build-ast:
	@echo "Building AST components ($(CONFIGURATION))..."
	@./BuildAST.sh $(CONFIGURATION)

.PHONY: build-kuick
# Build Kuick components
build-kuick:
	@echo "Building Kuick components ($(CONFIGURATION))..."
	@./BuildKuick.sh $(CONFIGURATION)

# Run all tests (requires full solution build)
test: build-solution
	@echo "Running tests ($(CONFIGURATION))..."
	@cd src && dotnet test KorePlatform.sln --configuration $(CONFIGURATION) --no-build --verbosity normal

.PHONY: test-utility
# Test Utility components
test-utility:
	@echo "Testing Utility components ($(CONFIGURATION))..."
	@./TestUtility.sh $(CONFIGURATION)

.PHONY: test-ast
# Test AST components
test-ast:
	@echo "Testing AST components ($(CONFIGURATION))..."
	@./TestAST.sh $(CONFIGURATION)

.PHONY: test-kuick
# Test Kuick components
test-kuick:
	@echo "Testing Kuick components ($(CONFIGURATION))..."
	@./TestKuick.sh $(CONFIGURATION)

# Print AstNode.getDebugText() for a single assembly file (FILE= path from repo root or absolute)
ast-dump:
	@test -n "$(FILE)" || (echo "Usage: make ast-dump FILE=src/path/to/file.S"; exit 1)
	@dotnet run --project src/Kore.Kuick.AstDump/Kore.Kuick.AstDump.csproj --configuration $(CONFIGURATION) -- "$(FILE)"

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	@cd src && dotnet clean KorePlatform.sln --verbosity quiet
	@find src -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
	@find src -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true
	@find src -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true

.PHONY: clean-builds
# Clean all builds (including additional cleanup)
clean-builds:
	@echo "Performing comprehensive build cleanup..."
	@./CleanBuilds.sh

# Setup development environment
setup:
	@echo "Setting up development environment..."
	@chmod +x setup.sh build.sh test.sh clean.sh BuildUtility.sh TestUtility.sh BuildAST.sh TestAST.sh BuildKuick.sh TestKuick.sh CleanBuilds.sh \
		src/Kuick.Tools/binutils/readelf/build-readelf.sh src/Kuick.Tools/binutils/readelf/Scripts/*.sh
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
	@echo "  make build              - Run all build-* component targets (utility, AST, Kuick, tools)"
	@echo "  make build-solution     - Build entire KorePlatform.sln"
	@echo "  make build-docs         - Build Sphinx HTML via Docker/Podman (docs/Makefile local)"
	@echo "  make debug         - Build in Debug configuration"
	@echo "  make release       - Build in Release configuration"
	@echo "  make build-utility - Build Utility components only"
	@echo "  make build-ast     - Build AST components only"
	@echo "  make build-kuick        - Build Kuick components only"
	@echo "  make build-tools        - Build all build-tools-* targets"
	@echo "  make build-tools-binutils - Build all build-tools-binutils-* targets"
	@echo "  make build-tools-binutils-readelf - Build readelf via src/Kuick.Tools/binutils/readelf/build-readelf.sh"
	@echo "  make version-inc-readelf-major|minor|patch - Bump readelf Version/ and reset compile"
	@echo "  make test          - Run all tests"
	@echo "  make test-utility  - Run Utility tests only"
	@echo "  make test-ast      - Run AST tests only"
	@echo "  make test-kuick    - Run Kuick tests only"
	@echo "  make ast-dump FILE=src/path/to/file.S - Print AST getDebugText() for one .S file"
	@echo "  make clean         - Clean build artifacts"
	@echo "  make clean-builds  - Comprehensive build cleanup"
	@echo "  make setup         - Setup development environment"
	@echo "  make restore       - Restore NuGet packages"
	@echo "  make check         - Check .NET SDK installation"
	@echo "  make help          - Show this help message"
	@echo ""
	@echo "Configuration can be overridden:"
	@echo "  make build CONFIGURATION=Release"
	@echo "  make build-tools-binutils-readelf CONFIGURATION=Release"
	@echo "  make test CONFIGURATION=Release"
	@echo "  make build-utility CONFIGURATION=Release"
	@echo "  make build-ast CONFIGURATION=Release"
	@echo "  make build-kuick CONFIGURATION=Release"
	@echo "  make test-utility CONFIGURATION=Release"
	@echo "  make test-ast CONFIGURATION=Release"
	@echo "  make test-kuick CONFIGURATION=Release" 