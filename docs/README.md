# KORE Documentation Build System

This directory contains the Sphinx documentation for the KORE project, along with a **containerized** build (`Dockerfile.sphinx`, image `kore-sphinx:latest`) so you get a consistent environment without installing Sphinx on the host.

## Quick Start

From the **repository root** (requires **Docker** or **Podman**):

```bash
make build-docs
```

This is equivalent to:

```bash
cd docs && make local
```

Output is written to `docs/build/html/`. Open `docs/build/html/index.html` in a browser to preview.

Pass options through to Sphinx (for example warnings as errors):

```bash
make build-docs SPHINXOPTS=-W
```

### CI vs local preview

- **GitHub Actions** (`.github/workflows/gh-pages.yml`) uses [`sphinx-action`](https://github.com/ammaraskar/sphinx-action) to run `sphinx-build` in a container on the runner. It does **not** use `Dockerfile.sphinx` directly, but the result is standard Sphinx HTML from `docs/source/`.
- **Local preview** via `make build-docs` / `make local` uses this repo’s **`Dockerfile.sphinx`**, which extends `sphinxdoc/sphinx` and installs `sphinx-rtd-theme` and `sphinx-tabs` — matching how this tree is set up for reproducible local builds.

### Without Docker

If you cannot run containers, install dependencies from `requirements.txt` into a virtualenv and run Sphinx manually, for example:

```bash
cd docs
python3 -m venv .venv
. .venv/bin/activate
pip install -r requirements.txt
sphinx-build -b html source build/html
```

(Older pins in `requirements.txt` may require an older Python; see package notes.)

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
| `make build-docs` (from repo root) | Same as `cd docs && make local` — build HTML via Docker/Podman + `Dockerfile.sphinx` |
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

The documentation build uses a custom Docker image based on `sphinxdoc/sphinx:latest` with additional dependencies:

- `sphinx-rtd-theme` - Read the Docs theme
- `sphinx-tabs` - Tabbed content support

## Container Build System

The containerized build system provides several advantages:

1. Builds a custom image (`kore-sphinx:latest`) with all required dependencies
2. Eliminates the need for local Python environment management
3. Ensures consistent builds across different development environments
4. Works on Linux, macOS, and Windows (with Docker Desktop)

### Prerequisites

- Docker or Podman installed and running
- Git (for cloning the repository)

### Quick Commands

```bash
# Build documentation using auto-detected container runtime
make local

# Build using Docker specifically
make local-docker

# Build using Podman specifically
make local-podman

# Build only the container image
make build-image
```

### Manual Container Build

If you prefer to build manually:

```bash
# Build the image
docker build -f Dockerfile.sphinx -t kore-sphinx:latest .

# Run the build
docker run --rm -v "$(pwd)":/docs -w /docs kore-sphinx:latest sphinx-build -b html source build/html
```

### Alternative: Local Sphinx Installation

If you prefer not to use containers:

```bash
pip install sphinx sphinx-rtd-theme sphinx-tabs
cd docs
sphinx-build -b html source build/html
```

## Development Workflow

1. Edit documentation in `source/`
2. Build using `make local` (or `make build-docs` from repo root)
3. Preview in `build/html/index.html`
4. Commit changes

## Troubleshooting

### Container Runtime Issues

If you get permission errors with Podman on Linux:

```bash
# Add your user to the docker group (Docker)
sudo usermod -aG docker $USER

# Or configure Podman for rootless operation
```

### SELinux (Fedora/RHEL)

Volume mounts use the `:Z` flag for SELinux compatibility. If you still have issues:

```bash
# Check SELinux status
getenforce

# Temporarily set to permissive for testing
sudo setenforce 0
```

### Build Failures

1. Check that all files in `source/` are valid RST
2. Verify `conf.py` configuration
3. Check container logs for detailed error messages

## File Structure

```
docs/
├── Dockerfile.sphinx    # Custom Sphinx container definition
├── Makefile            # Build automation
├── requirements.txt    # Python dependencies (non-container / legacy)
├── source/             # Documentation source files
│   ├── conf.py        # Sphinx configuration
│   └── index.rst      # Main documentation index
└── build/              # Generated output (gitignored)
    └── html/          # HTML output
```

## Contributing to Documentation

1. Follow RST formatting guidelines
2. Test builds locally using `make local`
3. Ensure all links work
4. Update `index.rst` when adding new pages

## Additional Resources

- [Sphinx Documentation](https://www.sphinx-doc.org/)
- [reStructuredText Primer](https://www.sphinx-doc.org/en/master/usage/restructuredtext/basics.html)
- [Read the Docs Theme](https://sphinx-rtd-theme.readthedocs.io/)
