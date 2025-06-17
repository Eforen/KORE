KORE (Kerbo Operations Runtime Engine)
========

KORE (Kerbo Operations Runtime Engine) is designed to be an entire Rocketry Architecture Platform this the goal is to make a platform that will not only avoid lockin (Only being able to be in KSP) but also provide a way for users to play with and/or learn any level of computing from highlevel programing to vm code to to direct assembly or even binary mechine code if desired.

Features
--------

- RISC-V Assembly Parser and Compiler
- CPU Emulation/Simulation
- Comprehensive Test Suite
- Cross-platform support (Windows, Linux)

Planned Features
--------

- Archecture Simulation/Emulation
- Testability
- Mechine Language
- VM Code Intermediate Language
- High Level C Type Language with strict type checking
- Compatability with kOS Kerbal Script

## Building on Linux

### Prerequisites

- .NET SDK 6.0 or higher
- Git

### Quick Setup

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd KORE
   ```

2. Run the setup script (installs .NET SDK if needed):
   ```bash
   chmod +x setup.sh
   ./setup.sh
   ```

3. Build the project:
   ```bash
   ./build.sh
   ```

### Manual Setup

If you prefer to install dependencies manually:

1. Install .NET SDK 6.0 or higher from [Microsoft's official site](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

2. Make scripts executable:
   ```bash
   chmod +x build.sh test.sh clean.sh
   ```

### Build Commands

#### Using Shell Scripts

- **Build the solution:**
  ```bash
  ./build.sh [Debug|Release]
  ```

- **Run all tests:**
  ```bash
  ./test.sh [Debug|Release]
  ```

- **Clean build artifacts:**
  ```bash
  ./clean.sh
  ```

#### Using Make

- **Build the solution:**
  ```bash
  make build
  make debug    # Debug configuration
  make release  # Release configuration
  ```

- **Run tests:**
  ```bash
  make test
  ```

- **Clean:**
  ```bash
  make clean
  ```

- **Setup environment:**
  ```bash
  make setup
  ```

- **Show all available targets:**
  ```bash
  make help
  ```

## Building on Windows

Use the existing Visual Studio solution or run:
```cmd
build.CMD
```

Installation
------------

TBA

Contribute
----------

- Issue Tracker: github.com/$project/$project/issues
- Source Code: github.com/$project/$project

Support
-------

If you are having issues, please let us know.
You can submit an issue,
or you can find me on the kOS Discord as `Eforen`.

License
-------

The project is licensed under the GPL-3.0 License.