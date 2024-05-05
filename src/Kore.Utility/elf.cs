using System;
using System.IO;

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
    /// ELF Header
    /// <seealso cref="https://upload.wikimedia.org/wikipedia/commons/e/e4/ELF_Executable_and_Linkable_Format_diagram_by_Ange_Albertini.png"/>
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
        public static ELF_FILE FromBinaryReader(System.IO.BinaryReader reader) {
            ELF_FILE file = new ELF_FILE();
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
            return file;
        }
    }

    public class e_ident {
        public byte EI_MAG0;
        public byte EI_MAG1;
        public byte EI_MAG2;
        public byte EI_MAG3;
        public byte EI_CLASS;
        public byte EI_DATA;
        public uint EI_VERSION;
        public byte EI_OSABI;
        public byte EI_ABIVERSION;
        public byte[] EI_PAD = new byte[4];

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
            ident.EI_VERSION = reader.ReadUInt32();
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
            for(int i = 0; i < 4; i++) {
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
            ident.EI_VERSION = BitConverter.ToUInt32(bytes, 6);
            ident.EI_OSABI = bytes[7];
            ident.EI_ABIVERSION = bytes[8];
            for(int i = 0; i < 4; i++) {
                ident.EI_PAD[i] = bytes[9 + i];
            }
            return ident;
        }
    }

}