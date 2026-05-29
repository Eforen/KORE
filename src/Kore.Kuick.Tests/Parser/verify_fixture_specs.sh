#!/usr/bin/env bash
# Drift detector only: compares Kore.Kuick.AstDump output to each sibling .spec after human review.
# Does not validate ISA correctness — see AstFixtures/REVIEW_PROCESS.md.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
CONFIGURATION="${CONFIGURATION:-Debug}"
DUMP_DLL="$ROOT/src/Kore.Kuick.AstDump/bin/$CONFIGURATION/net10.0/Kore.Kuick.AstDump.dll"
FIXTURES="$ROOT/src/Kore.Kuick.Tests/Parser/AstFixtures"
normalize() { python3 -c "import sys; t=sys.stdin.read(); print(t.replace('\r\n','\n').replace('\r','\n').rstrip())"; }
if [[ ! -f "$DUMP_DLL" ]]; then
  echo "Build AstDump first: dotnet build src/Kore.Kuick.AstDump/Kore.Kuick.AstDump.csproj -c ${CONFIGURATION}" >&2
  exit 1
fi
failed=0
for s in "$FIXTURES"/*.S; do
  base=$(basename "$s" .S)
  spec="$FIXTURES/$base.spec"
  [[ -f "$spec" ]] || continue
  got=$(dotnet "$DUMP_DLL" "$s" 2>/dev/null | normalize)
  exp=$(normalize <"$spec")
  if [[ "$got" != "$exp" ]]; then
    echo "MISMATCH: $base.spec" >&2
    failed=1
  else
    echo "OK $base"
  fi
done
exit "$failed"
