using System;
using System.IO;
using System.Collections.Generic;

namespace Kore.Utility.ELF {
    public enum ELFClass : byte {
        ELFCLASSNONE = 0,
        ELFCLASS32 = 1,
        ELFCLASS64 = 2
    }

    public enum ELFFormat : byte {
        ELFDATANONE = 0,
        ELFDATA2LSB = 1,
        ELFDATA2MSB = 2
    }

    public enum ELFType : ushort {
        ET_NONE = 0,
        ET_REL = 1,
        ET_EXEC = 2,
        ET_DYN = 3,
        ET_CORE = 4,
        ET_LOPROC = 0xff00,
        ET_HIPROC = 0xffff
    }

/*
http://www.skyfree.org/linux/references/ELF_Format.pdf
https://interrupt.memfault.com/blog/elf-format-differences
*/

    public struct Elf_Half {
        public ushort Value;
        public Elf_Half(ushort value) {
            Value = value;
        }
        public Elf_Half(BinaryReader reader) {
            Value = reader.ReadUInt16();
        }
        public static implicit operator Elf_Half(ushort value) {
            return new Elf_Half(value);
        }
        public static implicit operator ushort(Elf_Half value) {
            return value.Value;
        }
    }

    public struct Elf_Word {
        public uint Value;
        public Elf_Word(uint value) {
            Value = value;
        }
        public Elf_Word(BinaryReader reader) {
            Value = reader.ReadUInt32();
        }
        public static implicit operator Elf_Word(uint value) {
            return new Elf_Word(value);
        }
        public static implicit operator uint(Elf_Word value) {
            return value.Value;
        }
    }

    public struct Elf32_Addr {
        public uint Value;
        public Elf32_Addr(uint value) {
            Value = value;
        }
        public Elf32_Addr(BinaryReader reader) {
            Value = reader.ReadUInt32();
        }
        public static implicit operator Elf32_Addr(uint value) {
            return new Elf32_Addr(value);
        }
        public static implicit operator uint(Elf32_Addr value) {
            return value.Value;
        }
    }

    public struct Elf64_Addr {
        public ulong Value;
        public Elf64_Addr(ulong value) {
            Value = value;
        }
        public Elf64_Addr(BinaryReader reader) {
            Value = reader.ReadUInt64();
        }
        public static implicit operator Elf64_Addr(ulong value) {
            return new Elf64_Addr(value);
        }
        public static implicit operator ulong(Elf64_Addr value) {
            return value.Value;
        }
    }

    public struct Elf32_Off {
        public uint Value;
        public Elf32_Off(uint value) {
            Value = value;
        }
        public Elf32_Off(BinaryReader reader) {
            Value = reader.ReadUInt32();
        }
        public static implicit operator Elf32_Off(uint value) {
            return new Elf32_Off(value);
        }
        public static implicit operator uint(Elf32_Off value) {
            return value.Value;
        }
    }

    public struct Elf64_Off {
        public ulong Value;
        public Elf64_Off(ulong value) {
            Value = value;
        }
        public Elf64_Off(BinaryReader reader) {
            Value = reader.ReadUInt64();
        }
        public static implicit operator Elf64_Off(ulong value) {
            return new Elf64_Off(value);
        }
        public static implicit operator ulong(Elf64_Off value) {
            return value.Value;
        }
    }

    public struct Elf64_Xword {
        public ulong Value;
        public Elf64_Xword(ulong value) {
            Value = value;
        }
        public Elf64_Xword(BinaryReader reader) {
            Value = reader.ReadUInt64();
        }
        public static implicit operator Elf64_Xword(ulong value) {
            return new Elf64_Xword(value);
        }
        public static implicit operator ulong(Elf64_Xword value) {
            return value.Value;
        }
    }

    public struct Elf64_Sxword {
        public long Value;
        public Elf64_Sxword(long value) {
            Value = value;
        }
        public Elf64_Sxword(BinaryReader reader) {
            Value = reader.ReadInt64();
        }
        public static implicit operator Elf64_Sxword(long value) {
            return new Elf64_Sxword(value);
        }
        public static implicit operator long(Elf64_Sxword value) {
            return value.Value;
        }
    }

    /// <summary>
    /// Program Header Entry (64-bit)
    /// <seealso cref="https://upload.wikimedia.org/wikipedia/commons/e/e4/ELF_Executable_and_Linkable_Format_diagram_by_Ange_Albertini.png"/>
    /// </summary>
    public class Elf64_Phdr {
        public uint p_type;      // Segment type
        public uint p_flags;     // Segment flags
        public ulong p_offset;   // Segment file offset
        public ulong p_vaddr;    // Segment virtual address
        public ulong p_paddr;    // Segment physical address
        public ulong p_filesz;   // Segment size in file
        public ulong p_memsz;    // Segment size in memory
        public ulong p_align;    // Segment alignment

        public static Elf64_Phdr FromBinaryReader(BinaryReader reader) {
            var phdr = new Elf64_Phdr();
            phdr.p_type = reader.ReadUInt32();
            phdr.p_flags = reader.ReadUInt32();
            phdr.p_offset = reader.ReadUInt64();
            phdr.p_vaddr = reader.ReadUInt64();
            phdr.p_paddr = reader.ReadUInt64();
            phdr.p_filesz = reader.ReadUInt64();
            phdr.p_memsz = reader.ReadUInt64();
            phdr.p_align = reader.ReadUInt64();
            return phdr;
        }

        public byte[] ToBytes() {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(p_type));
            result.AddRange(BitConverter.GetBytes(p_flags));
            result.AddRange(BitConverter.GetBytes(p_offset));
            result.AddRange(BitConverter.GetBytes(p_vaddr));
            result.AddRange(BitConverter.GetBytes(p_paddr));
            result.AddRange(BitConverter.GetBytes(p_filesz));
            result.AddRange(BitConverter.GetBytes(p_memsz));
            result.AddRange(BitConverter.GetBytes(p_align));
            return result.ToArray();
        }
    }

    /// <summary>
    /// Section Header Entry (64-bit)
    /// </summary>
    public class Elf64_Shdr {
        public uint sh_name;      // Section name (string table index)
        public uint sh_type;      // Section type
        public ulong sh_flags;    // Section flags
        public ulong sh_addr;     // Section virtual addr at execution
        public ulong sh_offset;   // Section file offset
        public ulong sh_size;     // Section size in bytes
        public uint sh_link;      // Link to another section
        public uint sh_info;      // Additional section information
        public ulong sh_addralign; // Section alignment
        public ulong sh_entsize;  // Entry size if section holds table

        public static Elf64_Shdr FromBinaryReader(BinaryReader reader) {
            var shdr = new Elf64_Shdr();
            shdr.sh_name = reader.ReadUInt32();
            shdr.sh_type = reader.ReadUInt32();
            shdr.sh_flags = reader.ReadUInt64();
            shdr.sh_addr = reader.ReadUInt64();
            shdr.sh_offset = reader.ReadUInt64();
            shdr.sh_size = reader.ReadUInt64();
            shdr.sh_link = reader.ReadUInt32();
            shdr.sh_info = reader.ReadUInt32();
            shdr.sh_addralign = reader.ReadUInt64();
            shdr.sh_entsize = reader.ReadUInt64();
            return shdr;
        }

        public byte[] ToBytes() {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(sh_name));
            result.AddRange(BitConverter.GetBytes(sh_type));
            result.AddRange(BitConverter.GetBytes(sh_flags));
            result.AddRange(BitConverter.GetBytes(sh_addr));
            result.AddRange(BitConverter.GetBytes(sh_offset));
            result.AddRange(BitConverter.GetBytes(sh_size));
            result.AddRange(BitConverter.GetBytes(sh_link));
            result.AddRange(BitConverter.GetBytes(sh_info));
            result.AddRange(BitConverter.GetBytes(sh_addralign));
            result.AddRange(BitConverter.GetBytes(sh_entsize));
            return result.ToArray();
        }
    }

    /// <summary>
    /// ELF File Structure
    /// </summary>
    public class ELF_FILE{
        /// <summary>
        /// 16 bytes 7f 45 4c 46 01 01 01 00 00 00 00 00 00 00 00 00
        /// </summary>
        public e_ident e_ident;
        /// <summary>
        /// 2 bytes 02 00 for ET_EXEC (Executable file) No other types are supported atm
        /// </summary>
        public ushort e_type;
        /// <summary>
        /// 2 bytes we only support RISC-V atm so must be 0xf3
        /// </summary>
        public ushort e_machine;
        /// <summary>
        /// 4 bytes 01 00 00 00 for version 1 should never change
        /// </summary>
        public uint e_version;
        /// <summary>
        /// 4 bytes entry point of the program in memory
        /// </summary>
        public ulong e_entry;
        /// <summary>
        /// 4 bytes offset of the program header table
        /// </summary>
        public ulong e_phoff;
        /// <summary>
        /// 4 bytes offset of the section header table
        /// </summary>
        public ulong e_shoff;
        /// <summary>
        /// 4 bytes flags usually 0
        /// </summary>
        public uint e_flags;
        /// <summary>
        /// 2 bytes size of this header
        /// </summary>
        public ushort e_ehsize;
        /// <summary>
        /// 2 bytes size of a program header table entry
        /// </summary>
        public ushort e_phentsize;
        /// <summary>
        /// 2 bytes number of entries in the program header table
        /// </summary>
        public ushort e_phnum;
        /// <summary>
        /// 2 bytes size of a single section header table entry
        /// </summary>
        public ushort e_shentsize;
        /// <summary>
        /// 2 bytes number of entries in the section header table
        /// </summary>
        public ushort e_shnum;
        /// <summary>
        /// 2 bytes section header table index of the entry associated with the section name string table
        /// </summary>
        public ushort e_shstrndx;

        /// <summary>
        /// Program Headers
        /// </summary>
        public List<Elf64_Phdr> ProgramHeaders { get; set; } = new List<Elf64_Phdr>();

        /// <summary>
        /// Section Headers
        /// </summary>
        public List<Elf64_Shdr> SectionHeaders { get; set; } = new List<Elf64_Shdr>();

        /// <summary>
        /// Section Data - maps section index to section content
        /// </summary>
        public Dictionary<int, byte[]> SectionData { get; set; } = new Dictionary<int, byte[]>();

        /// <summary>
        /// Raw file data between sections and headers for exact reconstruction
        /// </summary>
        public Dictionary<ulong, byte[]> RawData { get; set; } = new Dictionary<ulong, byte[]>();
        
        public static ELF_FILE FromBinaryReader(System.IO.BinaryReader reader) {
            ELF_FILE file = new ELF_FILE();
            
            // Read the ELF header fields
            file.e_ident = e_ident.FromBinaryReader(reader);
            file.e_type = reader.ReadUInt16();
            file.e_machine = reader.ReadUInt16();
            file.e_version = reader.ReadUInt32();
            file.e_entry = reader.ReadUInt64();
            file.e_phoff = reader.ReadUInt64();
            file.e_shoff = reader.ReadUInt64();
            file.e_flags = reader.ReadUInt32();
            file.e_ehsize = reader.ReadUInt16();
            file.e_phentsize = reader.ReadUInt16();
            file.e_phnum = reader.ReadUInt16();
            file.e_shentsize = reader.ReadUInt16();
            file.e_shnum = reader.ReadUInt16();
            file.e_shstrndx = reader.ReadUInt16();

            // Store current position (should be end of ELF header)
            long currentPos = reader.BaseStream.Position;
            long streamLength = reader.BaseStream.Length;

            // Read Program Headers - only if they exist within the stream
            if (file.e_phoff > 0 && file.e_phnum > 0) {
                long phTableEnd = (long)file.e_phoff + (file.e_phnum * file.e_phentsize);
                if (phTableEnd <= streamLength) {
                    reader.BaseStream.Seek((long)file.e_phoff, SeekOrigin.Begin);
                    for (int i = 0; i < file.e_phnum; i++) {
                        file.ProgramHeaders.Add(Elf64_Phdr.FromBinaryReader(reader));
                    }
                }
            }

            // Read Section Headers - only if they exist within the stream
            if (file.e_shoff > 0 && file.e_shnum > 0) {
                long shTableEnd = (long)file.e_shoff + (file.e_shnum * file.e_shentsize);
                if (shTableEnd <= streamLength) {
                    reader.BaseStream.Seek((long)file.e_shoff, SeekOrigin.Begin);
                    for (int i = 0; i < file.e_shnum; i++) {
                        file.SectionHeaders.Add(Elf64_Shdr.FromBinaryReader(reader));
                    }
                }
            }

            // Read Section Data - only if section headers were successfully read
            for (int i = 0; i < file.SectionHeaders.Count; i++) {
                var shdr = file.SectionHeaders[i];
                if (shdr.sh_offset > 0 && shdr.sh_size > 0) {
                    long sectionEnd = (long)shdr.sh_offset + (long)shdr.sh_size;
                    if (sectionEnd <= streamLength) {
                        reader.BaseStream.Seek((long)shdr.sh_offset, SeekOrigin.Begin);
                        file.SectionData[i] = reader.ReadBytes((int)shdr.sh_size);
                    }
                }
            }

            // Read any remaining data to ensure exact reconstruction
            file.ReadRemainingData(reader);

            return file;
        }

        private void ReadRemainingData(BinaryReader reader) {
            // Read the entire file to capture any data not explicitly covered by sections
            long fileLength = reader.BaseStream.Length;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] entireFile = reader.ReadBytes((int)fileLength);

            // Store data between ELF header and first program header
            if (e_phoff > e_ehsize && e_phoff <= (ulong)fileLength) {
                var paddingSize = (int)(e_phoff - e_ehsize);
                if (e_ehsize + paddingSize <= fileLength) {
                    var padding = new byte[paddingSize];
                    Array.Copy(entireFile, e_ehsize, padding, 0, paddingSize);
                    RawData[e_ehsize] = padding;
                }
            }

            // Store data between program headers and sections
            ulong phTableEnd = e_phoff + (ulong)(e_phnum * e_phentsize);
            var offsets = new List<ulong>();
            
            // Collect all offsets that are within file bounds
            if (e_shoff > 0 && e_shoff < (ulong)fileLength) offsets.Add(e_shoff);
            foreach (var shdr in SectionHeaders) {
                if (shdr.sh_offset > 0 && shdr.sh_offset < (ulong)fileLength) offsets.Add(shdr.sh_offset);
            }
            offsets.Sort();

            // Fill gaps between known structures
            ulong lastOffset = phTableEnd;
            foreach (var offset in offsets) {
                if (offset > lastOffset && lastOffset < (ulong)fileLength) {
                    var gapSize = (int)(offset - lastOffset);
                    // Ensure we don't read beyond file boundaries
                    if ((int)lastOffset + gapSize <= fileLength) {
                        var gapData = new byte[gapSize];
                        Array.Copy(entireFile, (int)lastOffset, gapData, 0, gapSize);
                        RawData[lastOffset] = gapData;
                    }
                }
                
                // Update lastOffset based on what we know about this structure
                if (offset == e_shoff) {
                    lastOffset = offset + (ulong)(e_shnum * e_shentsize);
                } else {
                    // Find the section header for this offset
                    for (int i = 0; i < SectionHeaders.Count; i++) {
                        if (SectionHeaders[i].sh_offset == offset) {
                            lastOffset = offset + SectionHeaders[i].sh_size;
                            break;
                        }
                    }
                }
            }

            // Store any trailing data
            if (lastOffset < (ulong)fileLength) {
                var trailingSize = (int)((ulong)fileLength - lastOffset);
                if ((int)lastOffset + trailingSize <= fileLength) {
                    var trailingData = new byte[trailingSize];
                    Array.Copy(entireFile, (int)lastOffset, trailingData, 0, trailingSize);
                    RawData[lastOffset] = trailingData;
                }
            }
        }

        public byte[] ToBytes() {
            var result = new List<byte>();

            // ELF Header
            result.AddRange(e_ident.ToBytes());
            result.AddRange(BitConverter.GetBytes(e_type));
            result.AddRange(BitConverter.GetBytes(e_machine));
            result.AddRange(BitConverter.GetBytes(e_version));
            result.AddRange(BitConverter.GetBytes(e_entry));
            result.AddRange(BitConverter.GetBytes(e_phoff));
            result.AddRange(BitConverter.GetBytes(e_shoff));
            result.AddRange(BitConverter.GetBytes(e_flags));
            result.AddRange(BitConverter.GetBytes(e_ehsize));
            result.AddRange(BitConverter.GetBytes(e_phentsize));
            result.AddRange(BitConverter.GetBytes(e_phnum));
            result.AddRange(BitConverter.GetBytes(e_shentsize));
            result.AddRange(BitConverter.GetBytes(e_shnum));
            result.AddRange(BitConverter.GetBytes(e_shstrndx));

            // Add padding between header and program headers if needed
            while (result.Count < (int)e_phoff) {
                if (RawData.ContainsKey((ulong)result.Count)) {
                    result.AddRange(RawData[(ulong)result.Count]);
                    break;
                } else {
                    result.Add(0);
                }
            }

            // Program Headers
            foreach (var phdr in ProgramHeaders) {
                result.AddRange(phdr.ToBytes());
            }

            // Build a sorted list of all data that needs to be written
            var dataBlocks = new List<(ulong offset, byte[] data)>();
            
            // Add section data
            for (int i = 0; i < SectionHeaders.Count; i++) {
                var shdr = SectionHeaders[i];
                if (shdr.sh_offset > 0 && SectionData.ContainsKey(i)) {
                    dataBlocks.Add((shdr.sh_offset, SectionData[i]));
                }
            }

            // Add section headers
            dataBlocks.Add((e_shoff, GetSectionHeadersBytes()));

            // Add raw data blocks
            foreach (var kvp in RawData) {
                dataBlocks.Add((kvp.Key, kvp.Value));
            }

            // Sort by offset
            dataBlocks.Sort((a, b) => a.offset.CompareTo(b.offset));

            // Write all data blocks in order
            ulong currentOffset = (ulong)result.Count;
            foreach (var (offset, data) in dataBlocks) {
                // Add padding if needed
                while (currentOffset < offset) {
                    result.Add(0);
                    currentOffset++;
                }

                // Add the data
                result.AddRange(data);
                currentOffset += (ulong)data.Length;
            }

            return result.ToArray();
        }

        private byte[] GetSectionHeadersBytes() {
            var result = new List<byte>();
            foreach (var shdr in SectionHeaders) {
                result.AddRange(shdr.ToBytes());
            }
            return result.ToArray();
        }
    }

    public class e_ident {
        public byte EI_MAG0;
        public byte EI_MAG1;
        public byte EI_MAG2;
        public byte EI_MAG3;
        public byte EI_CLASS;
        public byte EI_DATA;
        public byte EI_VERSION;
        public byte EI_OSABI;
        public byte EI_ABIVERSION;
        public byte[] EI_PAD = new byte[7];

        public static e_ident FromBinaryReader(System.IO.BinaryReader reader) {
            e_ident ident = new e_ident();
            ident.EI_MAG0 = reader.ReadByte();
            ident.EI_MAG1 = reader.ReadByte();
            ident.EI_MAG2 = reader.ReadByte();
            ident.EI_MAG3 = reader.ReadByte();
            if (ident.EI_MAG0 != 0x7f || ident.EI_MAG1 != 0x45 || ident.EI_MAG2 != 0x4c || ident.EI_MAG3 != 0x46) {
                throw new Exception("Not an ELF file");
            }
            ident.EI_CLASS = reader.ReadByte();
            if (ident.EI_CLASS != (byte)ELFClass.ELFCLASS64) {
                throw new Exception("Not a 64-bit ELF file");
            }
            ident.EI_DATA = reader.ReadByte();
            if (ident.EI_DATA != 1) {
                throw new Exception("Not a little-endian ELF file");
            }
            ident.EI_VERSION = reader.ReadByte();
            if (ident.EI_VERSION != 1) {
                throw new Exception("Not an ELF version 1 file");
            }
            ident.EI_OSABI = reader.ReadByte();
            if(ident.EI_OSABI != 0) {
                throw new Exception("Not an ELF version 0 OSABI file");
            }
            ident.EI_ABIVERSION = reader.ReadByte();
            if(ident.EI_ABIVERSION != 0) {
                throw new Exception("Not an ELF version 0 ABI file");
            }
            for(int i = 0; i < 7; i++) {
                ident.EI_PAD[i] = reader.ReadByte();
            }
            return ident;
        }

        public static e_ident FromBytes(byte[] bytes) {
            e_ident ident = new e_ident();
            ident.EI_MAG0 = bytes[0];
            ident.EI_MAG1 = bytes[1];
            ident.EI_MAG2 = bytes[2];
            ident.EI_MAG3 = bytes[3];
            ident.EI_CLASS = bytes[4];
            ident.EI_DATA = bytes[5];
            ident.EI_VERSION = bytes[6];
            ident.EI_OSABI = bytes[7];
            ident.EI_ABIVERSION = bytes[8];
            for(int i = 0; i < 7; i++) {
                ident.EI_PAD[i] = bytes[9 + i];
            }
            return ident;
        }

        public byte[] ToBytes() {
            return new byte[] {
                EI_MAG0, EI_MAG1, EI_MAG2, EI_MAG3, EI_CLASS, EI_DATA, EI_VERSION, EI_OSABI, EI_ABIVERSION,
                EI_PAD[0], EI_PAD[1], EI_PAD[2], EI_PAD[3], EI_PAD[4], EI_PAD[5], EI_PAD[6]
            };
        }
    }

}