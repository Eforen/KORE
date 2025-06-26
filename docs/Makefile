# Minimal makefile for Sphinx documentation
#

# You can set these variables from the command line, and also
# from the environment for the first two.
SPHINXOPTS    ?=
SPHINXBUILD   ?= sphinx-build
SOURCEDIR     = source
BUILDDIR      = build
SERVE_PORT    ?= 8000

# Container runtime detection
CONTAINER_CMD := $(shell command -v podman 2> /dev/null)
ifndef CONTAINER_CMD
    CONTAINER_CMD := $(shell command -v docker 2> /dev/null)
endif

# Put it first so that "make" without argument is like "make help".
help:
	@$(SPHINXBUILD) -M help "$(SOURCEDIR)" "$(BUILDDIR)" $(SPHINXOPTS) $(O)

# Build the custom Sphinx image with required dependencies
build-image:
	@echo "Building custom Sphinx image with dependencies..."
ifndef CONTAINER_CMD
	@echo "Error: Neither Docker nor Podman found. Please install one of them:"
	@echo "  - Docker: https://docs.docker.com/get-docker/"
	@echo "  - Podman: https://podman.io/getting-started/installation"
	@exit 1
endif
	$(CONTAINER_CMD) build -f Dockerfile.sphinx -t kore-sphinx:latest .
	@echo "Custom Sphinx image built successfully!"

# Docker/Podman-based local build target
local: build-image
	@echo "Building Sphinx documentation using containers..."
	@echo "Using container runtime: $(CONTAINER_CMD)"
	$(CONTAINER_CMD) run --rm \
		-v "$(shell pwd)":/docs:Z \
		-w /docs \
		kore-sphinx:latest \
		sphinx-build -b html source build/html
	@echo "Documentation built successfully!"
	@echo "Open build/html/index.html in your browser to view the documentation."

# Alternative local build using Docker specifically
local-docker: 
	@echo "Building custom Sphinx image with Docker..."
	@command -v docker >/dev/null 2>&1 || { echo "Error: Docker not found. Please install Docker: https://docs.docker.com/get-docker/"; exit 1; }
	docker build -f Dockerfile.sphinx -t kore-sphinx:latest .
	@echo "Building Sphinx documentation using Docker..."
	docker run --rm \
		-v "$(shell pwd)":/docs:Z \
		-w /docs \
		kore-sphinx:latest \
		sphinx-build -b html source build/html
	@echo "Documentation built successfully!"
	@echo "Open build/html/index.html in your browser to view the documentation."

# Alternative local build using Podman specifically  
local-podman:
	@echo "Building custom Sphinx image with Podman..."
	@command -v podman >/dev/null 2>&1 || { echo "Error: Podman not found. Please install Podman: https://podman.io/getting-started/installation"; exit 1; }
	podman build -f Dockerfile.sphinx -t kore-sphinx:latest .
	@echo "Building Sphinx documentation using Podman..."
	podman run --rm \
		-v "$(shell pwd)":/docs:Z \
		-w /docs \
		kore-sphinx:latest \
		sphinx-build -b html source build/html
	@echo "Documentation built successfully!"
	@echo "Open build/html/index.html in your browser to view the documentation."

# Serve documentation locally using Python's built-in HTTP server
serve:
	@echo "Checking if documentation exists..."
	@if [ ! -d "build/html" ]; then \
		echo "Documentation not found. Building first..."; \
		$(MAKE) local; \
	fi
	@echo "Starting HTTP server on port $(SERVE_PORT)..."
	@echo "Documentation will be available at: http://localhost:$(SERVE_PORT)"
	@echo "Press Ctrl+C to stop the server"
	@cd build/html && python3 -m http.server $(SERVE_PORT)

# Serve documentation using containerized HTTP server
serve-container:
	@echo "Checking if documentation exists..."
	@if [ ! -d "build/html" ]; then \
		echo "Documentation not found. Building first..."; \
		$(MAKE) local; \
	fi
ifndef CONTAINER_CMD
	@echo "Error: Neither Docker nor Podman found. Please install one of them:"
	@echo "  - Docker: https://docs.docker.com/get-docker/"
	@echo "  - Podman: https://podman.io/getting-started/installation"
	@exit 1
endif
	@echo "Starting containerized HTTP server on port $(SERVE_PORT)..."
	@echo "Documentation will be available at: http://localhost:$(SERVE_PORT)"
	@echo "Press Ctrl+C to stop the server"
	$(CONTAINER_CMD) run --rm -it \
		-p $(SERVE_PORT):8000 \
		-v "$(shell pwd)/build/html":/usr/share/nginx/html:Z,ro \
		docker.io/nginx:alpine

# Build and serve in one command
local-serve: local serve

# Build and serve using containers in one command
local-serve-container: local serve-container

# Docker-based clean target
local-clean:
	@echo "Cleaning build directory..."
	rm -rf build/
	@echo "Build directory cleaned."

# Clean both build directory and container image
local-clean-all: local-clean
	@echo "Removing custom Sphinx container image..."
ifndef CONTAINER_CMD
	@echo "No container runtime found, skipping image cleanup."
else
	-$(CONTAINER_CMD) rmi kore-sphinx:latest 2>/dev/null || true
	@echo "Container image cleanup complete."
endif

# Show container build help
local-help:
	@echo "Container-based Sphinx documentation build targets:"
	@echo ""
	@echo "Building:"
	@echo "  make local            - Build docs using Docker or Podman (auto-detect)"
	@echo "  make local-docker     - Build docs using Docker specifically"
	@echo "  make local-podman     - Build docs using Podman specifically"
	@echo "  make build-image      - Build the custom Sphinx container image"
	@echo ""
	@echo "Serving:"
	@echo "  make serve            - Serve docs using Python HTTP server (port $(SERVE_PORT))"
	@echo "  make serve-container  - Serve docs using containerized Nginx (port $(SERVE_PORT))"
	@echo "  make local-serve      - Build and serve docs using Python HTTP server"
	@echo "  make local-serve-container - Build and serve docs using containerized Nginx"
	@echo ""
	@echo "Cleaning:"
	@echo "  make local-clean      - Clean the build directory"
	@echo "  make local-clean-all  - Clean build directory and container image"
	@echo "  make local-help       - Show this help message"
	@echo ""
	@echo "Environment Variables:"
	@echo "  SERVE_PORT=8000       - Port for serving documentation (default: 8000)"
	@echo ""
	@echo "Requirements:"
	@echo "  - Docker: https://docs.docker.com/get-docker/"
	@echo "  - OR Podman: https://podman.io/getting-started/installation"
	@echo "  - Python 3 (for non-container serving)"
	@echo ""
	@echo "On Fedora/RHEL systems, install Podman with:"
	@echo "  sudo dnf install podman"
	@echo ""
	@echo "Note: On SELinux systems (Fedora/RHEL), volume mounts use :Z flag"
	@echo "for proper container access to host files."

.PHONY: help Makefile local local-docker local-podman build-image local-clean local-clean-all local-help serve serve-container local-serve local-serve-container

# Catch-all target: route all unknown targets to Sphinx using the new
# "make mode" option.  $(O) is meant as a shortcut for $(SPHINXOPTS).
%: Makefile
	@$(SPHINXBUILD) -M $@ "$(SOURCEDIR)" "$(BUILDDIR)" $(SPHINXOPTS) $(O)
