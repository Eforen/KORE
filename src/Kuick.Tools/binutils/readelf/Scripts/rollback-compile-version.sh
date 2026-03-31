#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
VERSION_DIR="$PROJECT_DIR/Version"
BACKUP_FILE="$PROJECT_DIR/Version/.compile.backup"

if [[ -f "$BACKUP_FILE" ]]; then
  backup="$(tr -d "[:space:]" < "$BACKUP_FILE")"
  echo "$backup" > "$VERSION_DIR/compile.txt"
  rm -f "$BACKUP_FILE"
fi
