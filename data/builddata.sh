#!/bin/bash

# Change directory to the parent directory of this script regardless of where it is called from
cd "$(dirname "$(readlink -f "$0")")/.."

# Build the hello.s file
./BuildRiscWithDocker.sh data/tests/elf/hello.s
