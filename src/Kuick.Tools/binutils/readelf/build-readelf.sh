#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$PROJECT_DIR/../../../.." && pwd)"
INCREMENT_SCRIPT="$PROJECT_DIR/Scripts/increment-compile-version.sh"
ROLLBACK_SCRIPT="$PROJECT_DIR/Scripts/rollback-compile-version.sh"
STATE_DIR="$REPO_ROOT/bin/Kuick.Tools/.build-state"
HASH_FILE="$STATE_DIR/readelf-input.sha256"

compute_source_hash() {
  (
    cd "$REPO_ROOT"
    {
      rg --files src/Kuick.Tools/binutils/readelf src/Kuick.Elf \
        -g '!**/bin/**' \
        -g '!**/obj/**' \
        -g '!src/Kuick.Tools/binutils/readelf/Version/compile.txt' \
        -g '!src/Kuick.Tools/binutils/readelf/Version/.compile.backup' \
        | sort \
        | while IFS= read -r file; do
            sha256sum "$file"
          done
    } | sha256sum | awk '{print $1}'
  )
}

cleanup_on_fail() {
  "$ROLLBACK_SCRIPT"
}

dotnet build "$PROJECT_DIR/kuick-readelf.csproj" "$@"
current_hash="$(compute_source_hash)"
previous_hash=""

if [[ -f "$HASH_FILE" ]]; then
  previous_hash="$(tr -d '[:space:]' < "$HASH_FILE")"
fi

if [[ "$current_hash" == "$previous_hash" ]]; then
  echo "No source changes detected. Compile version unchanged."
  exit 0
fi

trap cleanup_on_fail ERR
"$INCREMENT_SCRIPT"
dotnet build "$PROJECT_DIR/kuick-readelf.csproj" "$@"
rm -f "$PROJECT_DIR/Version/.compile.backup"

mkdir -p "$STATE_DIR"
echo "$current_hash" > "$HASH_FILE"
echo "Source changes detected. Compile version incremented and rebuild completed."
