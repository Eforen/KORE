#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
VERSION_DIR="$PROJECT_DIR/Version"
BACKUP_FILE="$PROJECT_DIR/Version/.compile.backup"

if [[ ! -f "$VERSION_DIR/compile.txt" ]]; then
  echo "0" > "$VERSION_DIR/compile.txt"
fi

current="$(tr -d "[:space:]" < "$VERSION_DIR/compile.txt")"
if [[ -z "$current" ]]; then current=0; fi
if ! [[ "$current" =~ ^[0-9]+$ ]]; then
  echo "Invalid compile version: $current" >&2
  exit 1
fi

echo "$current" > "$BACKUP_FILE"
echo "$((current + 1))" > "$VERSION_DIR/compile.txt"
