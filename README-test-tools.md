# Kuick Test Tools

This directory contains scripts for running and analyzing Kuick parser and lexer tests.

## Scripts Overview

### 🔧 `GenerateKuick-trx.sh`
Runs all Kuick tests and generates TRX results with markdown summary.

**Usage:**
```bash
./GenerateKuick-trx.sh
```

**Output:**
- `TestResults/Kuick.trx` - Full test results in TRX format
- `TestResults/KUICK.md` - Markdown summary with pass/fail statistics

### 🔍 `SearchKuick-trx.sh`
Search for specific test patterns in the TRX results.

**Usage:**
```bash
./SearchKuick-trx.sh "search_term" [--summary]
```

**Examples:**
```bash
# Search for CSR-related tests
./SearchKuick-trx.sh "csrr"

# Search for rdcycle tests with full summary
./SearchKuick-trx.sh "rdcycle" --summary

# Search for floating-point tests
./SearchKuick-trx.sh "f[0-9]"
```

### 📊 `GenerateSummary.sh`
Generate markdown summary from existing TRX file.

**Usage:**
```bash
./GenerateSummary.sh
```

**Output:**
- `TestResults/KUICK.md` - Markdown summary with emoji status indicators

## Test Results Format

The markdown summary includes:

### Overall Statistics
| Metric | Count | Status |
|--------|-------|--------|
| Total Tests | 879 | ℹ️ |
| Executed | 855 | ▶️ |
| Passed | 772 | ✅ |
| Failed | 83 | ❌ |

### Status Indicators
- ✅ **Passed** - Test completed successfully
- ❌ **Failed** - Test failed with errors  
- ℹ️ **Info** - General information
- ▶️ **Executed** - Tests that were run

## Common Test Patterns

### CSR (Control Status Register) Tests
```bash
# All CSR-related tests
./SearchKuick-trx.sh "csr"

# Specific CSR instructions
./SearchKuick-trx.sh "csrr"
./SearchKuick-trx.sh "csrw"
./SearchKuick-trx.sh "rdcycle"
```

### Pseudo Instructions
```bash
# All pseudo instructions
./SearchKuick-trx.sh "PseudoInstructions"

# Specific pseudo instructions
./SearchKuick-trx.sh "nop"
./SearchKuick-trx.sh "neg"
./SearchKuick-trx.sh "beqz"
```

### Floating Point Tests
```bash
# All floating point registers
./SearchKuick-trx.sh "f[0-9]"

# Floating point operations
./SearchKuick-trx.sh "flw"
./SearchKuick-trx.sh "fld"
./SearchKuick-trx.sh "fsw"
```

## Workflow

1. **Run tests:** `./GenerateKuick-trx.sh`
2. **Check specific failures:** `./SearchKuick-trx.sh "failing_pattern"`
3. **View full summary:** Open `TestResults/KUICK.md`

## Files Generated

- `TestResults/Kuick.trx` - Full XML test results
- `TestResults/KUICK.md` - Human-readable markdown summary

## Dependencies

- .NET SDK (for running tests)
- `grep`, `sed` (for text processing)
- `bash` (shell environment)

---
*Generated for the Kuick RISC-V Assembler project* 