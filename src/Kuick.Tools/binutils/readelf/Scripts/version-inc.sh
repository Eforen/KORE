#!/usr/bin/env bash
# Usage: version-inc.sh <suffix> <major|minor|patch>
# Bumps semantic version in the Version/ directory for the given suffix and resets compile to 0.
set -euo pipefail

SUFFIX="${1:?suffix required (e.g. readelf)}"
KIND="${2:?kind required: major, minor, or patch}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

case "$SUFFIX" in
  readelf)
    VERSION_DIR="$PROJECT_DIR/Version"
    ;;
  *)
    echo "Unknown version suffix: $SUFFIX" >&2
    exit 1
    ;;
esac

read_num() {
  local f="$1"
  local d
  d="$(tr -d '[:space:]' < "$f" 2>/dev/null || true)"
  if [[ -z "$d" ]] || ! [[ "$d" =~ ^[0-9]+$ ]]; then
    echo 0
  else
    echo "$d"
  fi
}

write_num() {
  printf '%s\n' "$2" > "$1"
}

major="$(read_num "$VERSION_DIR/major.txt")"
minor="$(read_num "$VERSION_DIR/minor.txt")"
patch="$(read_num "$VERSION_DIR/patch.txt")"

case "$KIND" in
  major)
    major=$((major + 1))
    minor=0
    patch=0
    ;;
  minor)
    minor=$((minor + 1))
    patch=0
    ;;
  patch)
    patch=$((patch + 1))
    ;;
  *)
    echo "Unknown kind: $KIND (use major, minor, or patch)" >&2
    exit 1
    ;;
esac

write_num "$VERSION_DIR/major.txt" "$major"
write_num "$VERSION_DIR/minor.txt" "$minor"
write_num "$VERSION_DIR/patch.txt" "$patch"
write_num "$VERSION_DIR/compile.txt" 0

echo "Version for '$SUFFIX' set to $major.$minor.$patch.0 (compile reset to 0)."
