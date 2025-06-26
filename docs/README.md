# KORE Documentation Build System

This directory contains the Sphinx documentation for the KORE project, along with a containerized build system that eliminates the need for local Sphinx installation.

## Quick Start

To build the documentation using containers:

```bash
cd docs
make local
```

To build and serve the documentation locally for preview:

```bash
cd docs
make local-serve
```

This will automatically detect whether you have Docker or Podman installed and use the appropriate container runtime.

## Available Targets

### Building Documentation

| Target | Description |
|--------|-------------|
| `make local` | Build docs using Docker or Podman (auto-detect) |
| `make local-docker` | Build docs using Docker specifically |
| `make local-podman` | Build docs using Podman specifically |
| `make build-image` | Build the custom Sphinx container image only |

### Serving Documentation

| Target | Description |
|--------|-------------|
| `make serve` | Serve docs using Python HTTP server (requires build) |
| `make serve-container` | Serve docs using containerized Nginx (requires build) |
| `make local-serve` | Build and serve docs using Python HTTP server |
| `make local-serve-container` | Build and serve docs using containerized Nginx |

### Maintenance

| Target | Description |
|--------|-------------|
| `make local-clean` | Clean the build directory |
| `make local-clean-all` | Clean build directory and remove container image |
| `make local-help` | Show help for container-based targets |

## Requirements

You need either Docker or Podman installed:

- **Docker**: https://docs.docker.com/get-docker/
- **Podman**: https://podman.io/getting-started/installation

For serving without containers, you also need:
- **Python 3**: For the built-in HTTP server

### On Fedora/RHEL Systems

Install Podman (recommended):
```bash
sudo dnf install podman
```

## Serving Documentation Locally

### Option 1: Python HTTP Server (Recommended)

```bash
# Build and serve in one command
make local-serve

# Or serve existing build
make serve
```

This uses Python's built-in HTTP server and is the simplest option. The documentation will be available at `http://localhost:8000`.

### Option 2: Containerized Nginx Server

```bash
# Build and serve using containers
make local-serve-container

# Or serve existing build with containers
make serve-container
```

This uses a containerized Nginx server, which provides a production-like environment.

### Custom Port

You can specify a custom port using the `SERVE_PORT` environment variable:

```bash
# Serve on port 9000 instead of default 8000
SERVE_PORT=9000 make serve
```

## Container Build System

The documentation build uses a custom Docker image based on `sphinxdoc/sphinx:latest` with additional dependencies:

- `sphinx-rtd-theme` - Read the Docs theme
- `sphinx-tabs` - Tabbed content support
- Custom lexers for KORE-specific syntax highlighting

### Custom Image Details

The build system:
1. Builds a custom image (`kore-sphinx:latest`) with all required dependencies
2. Copies custom lexer files and source documentation
3. Runs Sphinx build inside the container
4. Outputs HTML documentation to `build/html/`

### SELinux Support

On SELinux-enabled systems (Fedora/RHEL), the volume mounts automatically include the `:Z` flag for proper container access to host files.

## Output

After a successful build:
- HTML documentation is available in `build/html/`
- Open `build/html/index.html` in your browser to view the documentation
- Or use one of the serve targets to host it locally

## Traditional Build (Alternative)

If you prefer to install Sphinx locally:

```bash
# Install dependencies (Python/pip required)
pip install sphinx sphinx-rtd-theme sphinx-tabs

# Build documentation
make html

# Serve using Python (from build/html directory)
cd build/html && python3 -m http.server 8000
```

## Troubleshooting

### Permission Errors
If you encounter permission errors, ensure your container runtime has proper access:
- **Docker**: Add your user to the `docker` group or run with `sudo`
- **Podman**: Usually works without additional permissions

### Missing Container Runtime
If neither Docker nor Podman is found:
```
Error: Neither Docker nor Podman found. Please install one of them:
  - Docker: https://docs.docker.com/get-docker/
  - Podman: https://podman.io/getting-started/installation
```

Install either Docker or Podman following the provided links.

### Port Already in Use
If you get a "port already in use" error:
```bash
# Use a different port
SERVE_PORT=9000 make serve
```

Or find and stop the process using the port:
```bash
# Find process using port 8000
lsof -i :8000

# Kill the process (replace PID with actual process ID)
kill <PID>
```

### Build Warnings
Some warnings during build are normal and don't prevent successful documentation generation. The warnings typically relate to:
- RST formatting issues
- Missing files in toctrees
- Title underline lengths

## Development Notes

- The `Dockerfile.sphinx` contains the custom image definition
- Custom lexers (`.py` files) are automatically copied into the container
- The `conf.py` handles missing custom lexers gracefully with try/except blocks
- Extensions are loaded conditionally to avoid errors if custom lexers are unavailable
- Serving targets automatically build documentation if it doesn't exist 