namespace Kuick.Elf.Models;

/// <summary>
/// One ELF program header entry (PT_*); fields use 64-bit width after normalization from ELF32.
/// </summary>
public sealed class ProgramHeader
{
    public uint Type { get; init; }
    public uint Flags { get; init; }
    public ulong Offset { get; init; }
    public ulong VirtualAddress { get; init; }
    public ulong PhysicalAddress { get; init; }
    public ulong FileSize { get; init; }
    public ulong MemorySize { get; init; }
    public ulong Align { get; init; }
}
