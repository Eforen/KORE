#!/usr/bin/env bash
# Regenerate Golden/*.txt from TestData/readelf_fixture.s using riscv32-unknown-elf-as, ld.lld -shared, and the built readelf.
# Requires: riscv32-unknown-elf-as, ld.lld (LLVM). Run: src/Kuick.Tools.Readelf.Tests/scripts/regenerate-readelf-golden.sh
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TESTS_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
REPO_ROOT="$(cd "$TESTS_DIR/../.." && pwd)"
ASM="$TESTS_DIR/TestData/readelf_fixture.s"
GOLD="$TESTS_DIR/Golden"
OBJ="$(mktemp /tmp/kore-readelf-golden.XXXXXX.o)"
SO="$(mktemp /tmp/kore-readelf-golden.XXXXXX.so)"
cleanup() { rm -f "$OBJ" "$SO"; }
trap cleanup EXIT

CONFIG="${1:-Debug}"
RF="$REPO_ROOT/bin/Kuick.Tools/bin/$CONFIG/net8.0/riscv32-kuick-elf-readelf"
if [[ ! -x "$RF" ]]; then
  echo "Build readelf first (expected at $RF)" >&2
  exit 1
fi

if ! command -v ld.lld >/dev/null 2>&1; then
  echo "ld.lld not found on PATH (install LLVM lld)" >&2
  exit 1
fi

riscv32-unknown-elf-as -march=rv32i -mabi=ilp32 -o "$OBJ" "$ASM"
ld.lld -shared -m elf32lriscv -o "$SO" "$OBJ"
mkdir -p "$GOLD"

run_rf() {
  local out=$1
  shift
  "$RF" "$@" "$SO" > "$out"
}

run_rf "$GOLD/view_h.txt" -h
run_rf "$GOLD/view_l.txt" -l
run_rf "$GOLD/view_flag_S_sections.txt" -S
run_rf "$GOLD/view_flag_s_syms.txt" -s
run_rf "$GOLD/view_r.txt" -r
run_rf "$GOLD/view_d.txt" -d
run_rf "$GOLD/view_V.txt" -V
run_rf "$GOLD/view_flag_A_arch.txt" -A
run_rf "$GOLD/view_I.txt" -I
run_rf "$GOLD/view_gotcontents.txt" --got-contents
run_rf "$GOLD/view_flag_S_includeempty.txt" -S --include-empty
run_rf "$GOLD/view_h_verbose.txt" -h --verbose
run_rf "$GOLD/view_hS.txt" -hS
run_rf "$GOLD/view_flag_a_all.txt" -a
run_rf "$GOLD/view_default.txt"

diff -q <("$RF" --all "$OBJ") <("$RF" -a "$OBJ") >/dev/null || { echo "--all vs -a mismatch" >&2; exit 1; }
echo "Wrote golden files under $GOLD"
