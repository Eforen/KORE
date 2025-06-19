#!/bin/bash

# Check if an input file was provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <path-to-assembly-file>"
    echo "Example: $0 data/tests/elf/hello.s"
    exit 1
fi

# Input assembly file (e.g., data/tests/elf/hello.s)
INPUT_FILE="$1"

# Validate that the input file exists
if [ ! -f "$INPUT_FILE" ]; then
    echo "Error: Input file '$INPUT_FILE' does not exist"
    exit 1
fi

# Determine the output ELF file path (e.g., data/tests/elf/hello.elf)
OUTPUT_FILE="${INPUT_FILE%.s}.elf"

# Ensure the output directory exists
OUTPUT_DIR=$(dirname "$OUTPUT_FILE")
mkdir -p "$OUTPUT_DIR"

# Docker image for RISC-V toolchain
DOCKER_IMAGE="fiveembeddev/riscv_gnu_toolchain_dev_env:latest"

# Pull the Docker image if not already present
echo "Pulling Docker image $DOCKER_IMAGE..."
docker pull "$DOCKER_IMAGE"

# Run the Docker container to compile the assembly file
echo "Compiling $INPUT_FILE to $OUTPUT_FILE..."
docker run --rm \
    -v "$(pwd):/workspace" \
    -w /workspace \
    "$DOCKER_IMAGE" \
    riscv64-unknown-elf-gcc -nostdlib -o "$OUTPUT_FILE" "$INPUT_FILE"

# Check if the compilation was successful
if [ $? -eq 0 ]; then
    echo "Successfully created $OUTPUT_FILE"
else
    echo "Error: Compilation failed"
    exit 1
fi