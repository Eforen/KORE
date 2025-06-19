using NUnit.Framework;
using System;
using System.IO;
using Kore.Utility.ELF;

namespace Kore.Utility.Tests
{
    [TestFixture]
    public class ElfTests
    {
        // Valid ELF64 header bytes for testing
        private static readonly byte[] ValidElf64Header = new byte[] {
            0x7F, 0x45, 0x4C, 0x46, // ELF magic number
            0x02,                   // 64-bit
            0x01,                   // Little endian
            0x01,                   // Version 1
            0x00,                   // OSABI = 0
            0x00,                   // ABI version = 0
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Padding (7 bytes)
            0x02, 0x00,             // ET_EXEC (executable)
            0xF3, 0x00,             // RISC-V machine type (0x00F3)
            0x01, 0x00, 0x00, 0x00, // Version 1
            0x78, 0x56, 0x34, 0x12, 0x00, 0x00, 0x00, 0x00, // Entry point
            0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Program header offset
            0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Section header offset
            0x00, 0x00, 0x00, 0x00, // Flags
            0x40, 0x00,             // ELF header size (64 bytes)
            0x38, 0x00,             // Program header entry size
            0x02, 0x00,             // Number of program headers
            0x40, 0x00,             // Section header entry size
            0x03, 0x00,             // Number of section headers
            0x02, 0x00              // Section header string table index
        };

        [TestFixture]
        public class ElfTypesTests
        {
            [Test]
            public void Elf_Half_Constructor_ShouldSetValue()
            {
                // Arrange
                ushort expectedValue = 0x1234;
                
                // Act
                var elfHalf = new Elf_Half(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elfHalf.Value);
            }

            [Test]
            public void Elf_Half_ImplicitConversion_FromUshort_ShouldWork()
            {
                // Arrange
                ushort value = 0x5678;
                
                // Act
                Elf_Half elfHalf = value;
                
                // Assert
                Assert.AreEqual(value, elfHalf.Value);
            }

            [Test]
            public void Elf_Half_ImplicitConversion_ToUshort_ShouldWork()
            {
                // Arrange
                var elfHalf = new Elf_Half(0x9ABC);
                
                // Act
                ushort value = elfHalf;
                
                // Assert
                Assert.AreEqual(0x9ABC, value);
            }

            [Test]
            public void Elf_Word_Constructor_ShouldSetValue()
            {
                // Arrange
                uint expectedValue = 0x12345678;
                
                // Act
                var elfWord = new Elf_Word(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elfWord.Value);
            }

            [Test]
            public void Elf32_Addr_Constructor_ShouldSetValue()
            {
                // Arrange
                uint expectedValue = 0xDEADBEEF;
                
                // Act
                var elf32Addr = new Elf32_Addr(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elf32Addr.Value);
            }

            [Test]
            public void Elf64_Addr_Constructor_ShouldSetValue()
            {
                // Arrange
                ulong expectedValue = 0xDEADBEEFCAFEBABE;
                
                // Act
                var elf64Addr = new Elf64_Addr(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elf64Addr.Value);
            }

            [Test]
            public void Elf32_Off_Constructor_ShouldSetValue()
            {
                // Arrange
                uint expectedValue = 0x1000;
                
                // Act
                var elf32Off = new Elf32_Off(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elf32Off.Value);
            }

            [Test]
            public void Elf64_Off_Constructor_ShouldSetValue()
            {
                // Arrange
                ulong expectedValue = 0x2000;
                
                // Act
                var elf64Off = new Elf64_Off(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elf64Off.Value);
            }

            [Test]
            public void Elf64_Xword_Constructor_ShouldSetValue()
            {
                // Arrange
                ulong expectedValue = 0xFEDCBA9876543210;
                
                // Act
                var elf64Xword = new Elf64_Xword(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elf64Xword.Value);
            }

            [Test]
            public void Elf64_Sxword_Constructor_ShouldSetValue()
            {
                // Arrange
                long expectedValue = -0x123456789ABCDEF0;
                
                // Act
                var elf64Sxword = new Elf64_Sxword(expectedValue);
                
                // Assert
                Assert.AreEqual(expectedValue, elf64Sxword.Value);
            }
        }

        [TestFixture]
        public class EIdentTests
        {
            [Test]
            public void e_ident_FromBinaryReader_ValidELF64_ShouldParseCorrectly()
            {
                // Arrange
                using var stream = new MemoryStream(ValidElf64Header);
                using var reader = new BinaryReader(stream);
                
                // Act
                var ident = e_ident.FromBinaryReader(reader);
                
                // Assert
                Assert.AreEqual(0x7F, ident.EI_MAG0);
                Assert.AreEqual(0x45, ident.EI_MAG1);
                Assert.AreEqual(0x4C, ident.EI_MAG2);
                Assert.AreEqual(0x46, ident.EI_MAG3);
                Assert.AreEqual((byte)ELFClass.ELFCLASS64, ident.EI_CLASS);
                Assert.AreEqual((byte)ELFFormat.ELFDATA2LSB, ident.EI_DATA);
                Assert.AreEqual((byte)1, ident.EI_VERSION);
                Assert.AreEqual(0, ident.EI_OSABI);
                Assert.AreEqual(0, ident.EI_ABIVERSION);
                Assert.AreEqual(7, ident.EI_PAD.Length);
            }

            [Test]
            public void e_ident_FromBytes_ValidELF64_ShouldParseCorrectly()
            {
                // Arrange
                byte[] identBytes = new byte[16];
                Array.Copy(ValidElf64Header, 0, identBytes, 0, 16);
                
                // Act
                var ident = e_ident.FromBytes(identBytes);
                
                // Assert
                Assert.AreEqual(0x7F, ident.EI_MAG0);
                Assert.AreEqual(0x45, ident.EI_MAG1);
                Assert.AreEqual(0x4C, ident.EI_MAG2);
                Assert.AreEqual(0x46, ident.EI_MAG3);
                Assert.AreEqual((byte)ELFClass.ELFCLASS64, ident.EI_CLASS);
                Assert.AreEqual((byte)ELFFormat.ELFDATA2LSB, ident.EI_DATA);
            }

            [Test]
            public void e_ident_FromBinaryReader_InvalidMagic_ShouldThrowException()
            {
                // Arrange
                byte[] invalidHeader = new byte[ValidElf64Header.Length];
                Array.Copy(ValidElf64Header, invalidHeader, ValidElf64Header.Length);
                invalidHeader[0] = 0x00; // Invalid magic number
                
                using var stream = new MemoryStream(invalidHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                var ex = Assert.Throws<Exception>(() => e_ident.FromBinaryReader(reader));
                Assert.AreEqual("Not an ELF file", ex.Message);
            }

            [Test]
            public void e_ident_FromBinaryReader_Invalid32Bit_ShouldThrowException()
            {
                // Arrange
                byte[] invalidHeader = new byte[ValidElf64Header.Length];
                Array.Copy(ValidElf64Header, invalidHeader, ValidElf64Header.Length);
                invalidHeader[4] = (byte)ELFClass.ELFCLASS32; // 32-bit instead of 64-bit
                
                using var stream = new MemoryStream(invalidHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                var ex = Assert.Throws<Exception>(() => e_ident.FromBinaryReader(reader));
                Assert.AreEqual("Not a 64-bit ELF file", ex.Message);
            }

            [Test]
            public void e_ident_FromBinaryReader_BigEndian_ShouldThrowException()
            {
                // Arrange
                byte[] invalidHeader = new byte[ValidElf64Header.Length];
                Array.Copy(ValidElf64Header, invalidHeader, ValidElf64Header.Length);
                invalidHeader[5] = (byte)ELFFormat.ELFDATA2MSB; // Big endian instead of little endian
                
                using var stream = new MemoryStream(invalidHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                var ex = Assert.Throws<Exception>(() => e_ident.FromBinaryReader(reader));
                Assert.AreEqual("Not a little-endian ELF file", ex.Message);
            }

            [Test]
            public void e_ident_FromBinaryReader_InvalidVersion_ShouldThrowException()
            {
                // Arrange
                byte[] invalidHeader = new byte[ValidElf64Header.Length];
                Array.Copy(ValidElf64Header, invalidHeader, ValidElf64Header.Length);
                // Set version to 2 (invalid)
                invalidHeader[6] = 0x02;
                
                using var stream = new MemoryStream(invalidHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                var ex = Assert.Throws<Exception>(() => e_ident.FromBinaryReader(reader));
                Assert.AreEqual("Not an ELF version 1 file", ex.Message);
            }

            [Test]
            public void e_ident_FromBinaryReader_InvalidOSABI_ShouldThrowException()
            {
                // Arrange
                byte[] invalidHeader = new byte[ValidElf64Header.Length];
                Array.Copy(ValidElf64Header, invalidHeader, ValidElf64Header.Length);
                invalidHeader[7] = 0x01; // Non-zero OSABI
                
                using var stream = new MemoryStream(invalidHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                var ex = Assert.Throws<Exception>(() => e_ident.FromBinaryReader(reader));
                Assert.AreEqual("Not an ELF version 0 OSABI file", ex.Message);
            }

            [Test]
            public void e_ident_FromBinaryReader_InvalidABIVersion_ShouldThrowException()
            {
                // Arrange
                byte[] invalidHeader = new byte[ValidElf64Header.Length];
                Array.Copy(ValidElf64Header, invalidHeader, ValidElf64Header.Length);
                invalidHeader[8] = 0x01; // Non-zero ABI version
                
                using var stream = new MemoryStream(invalidHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                var ex = Assert.Throws<Exception>(() => e_ident.FromBinaryReader(reader));
                Assert.AreEqual("Not an ELF version 0 ABI file", ex.Message);
            }
        }

        [TestFixture]
        public class ELFFileTests
        {
            [Test]
            public void ELF_FILE_FromBinaryReader_ValidHeader_ShouldParseCorrectly()
            {
                // Arrange
                using var stream = new MemoryStream(ValidElf64Header);
                using var reader = new BinaryReader(stream);
                
                // Act
                var elfFile = ELF_FILE.FromBinaryReader(reader);
                
                // Assert
                Assert.IsNotNull(elfFile);
                Assert.IsNotNull(elfFile.e_ident);
                Assert.AreEqual((ushort)ELFType.ET_EXEC, elfFile.e_type);
                Assert.AreEqual(0x00F3, elfFile.e_machine); // RISC-V
                Assert.AreEqual(1u, elfFile.e_version);
                Assert.AreEqual(0x0000000012345678ul, elfFile.e_entry);
                Assert.AreEqual(0x0000000000000040ul, elfFile.e_phoff);
                Assert.AreEqual(0x0000000000000080ul, elfFile.e_shoff);
                Assert.AreEqual(0u, elfFile.e_flags);
                Assert.AreEqual(64, elfFile.e_ehsize);
                Assert.AreEqual(56, elfFile.e_phentsize);
                Assert.AreEqual(2, elfFile.e_phnum);
                Assert.AreEqual(64, elfFile.e_shentsize);
                Assert.AreEqual(3, elfFile.e_shnum);
                Assert.AreEqual(2, elfFile.e_shstrndx);
            }

            [Test]
            public void ELF_FILE_FromBinaryReader_WithRealWorldValues_ShouldParseCorrectly()
            {
                // Arrange - Create a more realistic ELF header
                var realisticHeader = new byte[] {
                    0x7F, 0x45, 0x4C, 0x46, // ELF magic
                    0x02,                   // 64-bit
                    0x01,                   // Little endian
                    0x01,                   // Version 1
                    0x00,                   // OSABI = 0
                    0x00,                   // ABI version = 0
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Padding (7 bytes)
                    0x02, 0x00,             // ET_EXEC
                    0xF3, 0x00,             // RISC-V
                    0x01, 0x00, 0x00, 0x00, // Version 1
                    0x00, 0x10, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, // Entry: 0x401000
                    0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // PH offset: 64
                    0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // SH offset: 8192
                    0x00, 0x00, 0x00, 0x00, // Flags
                    0x40, 0x00,             // ELF header size
                    0x38, 0x00,             // PH entry size
                    0x05, 0x00,             // PH count
                    0x40, 0x00,             // SH entry size
                    0x1A, 0x00,             // SH count
                    0x19, 0x00              // SH string table index
                };
                
                using var stream = new MemoryStream(realisticHeader);
                using var reader = new BinaryReader(stream);
                
                // Act
                var elfFile = ELF_FILE.FromBinaryReader(reader);
                
                // Assert
                Assert.AreEqual(0x0000000000401000ul, elfFile.e_entry);
                Assert.AreEqual(5, elfFile.e_phnum);
                Assert.AreEqual(26, elfFile.e_shnum);
                Assert.AreEqual(25, elfFile.e_shstrndx);
            }

            [Test]
            public void ELF_FILE_FromBinaryReader_InsufficientData_ShouldThrowException()
            {
                // Arrange - Header too short
                byte[] shortHeader = new byte[32]; // Only 32 bytes instead of required 64
                Array.Copy(ValidElf64Header, shortHeader, 32);
                
                using var stream = new MemoryStream(shortHeader);
                using var reader = new BinaryReader(stream);
                
                // Act & Assert
                Assert.Throws<EndOfStreamException>(() => ELF_FILE.FromBinaryReader(reader));
            }
        }

        [TestFixture]
        public class ELFEnumTests
        {
            [Test]
            public void ELFClass_Values_ShouldBeCorrect()
            {
                Assert.AreEqual(0, (byte)ELFClass.ELFCLASSNONE);
                Assert.AreEqual(1, (byte)ELFClass.ELFCLASS32);
                Assert.AreEqual(2, (byte)ELFClass.ELFCLASS64);
            }

            [Test]
            public void ELFFormat_Values_ShouldBeCorrect()
            {
                Assert.AreEqual(0, (byte)ELFFormat.ELFDATANONE);
                Assert.AreEqual(1, (byte)ELFFormat.ELFDATA2LSB);
                Assert.AreEqual(2, (byte)ELFFormat.ELFDATA2MSB);
            }

            [Test]
            public void ELFType_Values_ShouldBeCorrect()
            {
                Assert.AreEqual(0, (ushort)ELFType.ET_NONE);
                Assert.AreEqual(1, (ushort)ELFType.ET_REL);
                Assert.AreEqual(2, (ushort)ELFType.ET_EXEC);
                Assert.AreEqual(3, (ushort)ELFType.ET_DYN);
                Assert.AreEqual(4, (ushort)ELFType.ET_CORE);
                Assert.AreEqual(0xFF00, (ushort)ELFType.ET_LOPROC);
                Assert.AreEqual(0xFFFF, (ushort)ELFType.ET_HIPROC);
            }
        }

        [TestFixture]
        public class BinaryReaderConstructorTests
        {
            [Test]
            public void Elf_Half_FromBinaryReader_ShouldReadCorrectly()
            {
                // Arrange
                var bytes = new byte[] { 0x34, 0x12 }; // Little endian 0x1234
                using var stream = new MemoryStream(bytes);
                using var reader = new BinaryReader(stream);
                
                // Act
                var elfHalf = new Elf_Half(reader);
                
                // Assert
                Assert.AreEqual(0x1234, elfHalf.Value);
            }

            [Test]
            public void Elf_Word_FromBinaryReader_ShouldReadCorrectly()
            {
                // Arrange
                var bytes = new byte[] { 0x78, 0x56, 0x34, 0x12 }; // Little endian 0x12345678
                using var stream = new MemoryStream(bytes);
                using var reader = new BinaryReader(stream);
                
                // Act
                var elfWord = new Elf_Word(reader);
                
                // Assert
                Assert.AreEqual(0x12345678u, elfWord.Value);
            }

            [Test]
            public void Elf64_Addr_FromBinaryReader_ShouldReadCorrectly()
            {
                // Arrange
                var bytes = new byte[] { 0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01 };
                using var stream = new MemoryStream(bytes);
                using var reader = new BinaryReader(stream);
                
                // Act
                var elf64Addr = new Elf64_Addr(reader);
                
                // Assert
                Assert.AreEqual(0x0123456789ABCDEFul, elf64Addr.Value);
            }
        }
    }
} 