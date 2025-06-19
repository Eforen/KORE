using System;
using System.IO;
using NUnit.Framework;
using Kore.Utility.ELF;

namespace Kore.Utility.ELF.Tests
{
    [TestFixture]
    public class ElfFileParserTests
    {
        // Placeholder for ElfFileBytes (5256 bytes from hello.elf)
        // Generate using elf_to_csharp.py and copy the array here
        private static readonly byte[] ElfFileBytes = TestData.HelloElf;

        [Test]
        public void TestElfIdentParsing()
        {
            using (var stream = new MemoryStream(ElfFileBytes))
            using (var reader = new BinaryReader(stream))
            {
                var ident = e_ident.FromBinaryReader(reader);

                Assert.AreEqual(0x7F, ident.EI_MAG0, "EI_MAG0 mismatch");
                Assert.AreEqual(0x45, ident.EI_MAG1, "EI_MAG1 mismatch");
                Assert.AreEqual(0x4C, ident.EI_MAG2, "EI_MAG2 mismatch");
                Assert.AreEqual(0x46, ident.EI_MAG3, "EI_MAG3 mismatch");
                Assert.AreEqual((byte)ELFClass.ELFCLASS64, ident.EI_CLASS, "EI_CLASS mismatch");
                Assert.AreEqual((byte)ELFFormat.ELFDATA2LSB, ident.EI_DATA, "EI_DATA mismatch");
                Assert.AreEqual(0x01, ident.EI_VERSION, "EI_VERSION mismatch");
                Assert.AreEqual(0x00, ident.EI_OSABI, "EI_OSABI mismatch");
                Assert.AreEqual(0x00, ident.EI_ABIVERSION, "EI_ABIVERSION mismatch");
                CollectionAssert.AreEqual(new byte[7], ident.EI_PAD, "EI_PAD mismatch");
            }
        }

        [Test]
        public void TestElfHeaderParsing()
        {
            using (var stream = new MemoryStream(ElfFileBytes))
            using (var reader = new BinaryReader(stream))
            {
                var elf = ELF_FILE.FromBinaryReader(reader);

                Assert.AreEqual((ushort)ELFType.ET_EXEC, elf.e_type, "e_type mismatch");
                Assert.AreEqual(0x00F3, elf.e_machine, "e_machine mismatch");
                Assert.AreEqual(0x00000001U, elf.e_version, "e_version mismatch");
                Assert.AreEqual(0x80000000UL, elf.e_entry, "e_entry mismatch");
                Assert.AreEqual(0x0000000000000040UL, elf.e_phoff, "e_phoff mismatch");
                Assert.AreEqual(0x00000000000012C8UL, elf.e_shoff, "e_shoff mismatch");
                Assert.AreEqual(0x00000005U, elf.e_flags, "e_flags mismatch");
                Assert.AreEqual(0x0040, elf.e_ehsize, "e_ehsize mismatch");
                Assert.AreEqual(0x0038, elf.e_phentsize, "e_phentsize mismatch");
                Assert.AreEqual(0x0003, elf.e_phnum, "e_phnum mismatch");
                Assert.AreEqual(0x0040, elf.e_shentsize, "e_shentsize mismatch");
                Assert.AreEqual(0x0007, elf.e_shnum, "e_shnum mismatch");
                Assert.AreEqual(0x0006, elf.e_shstrndx, "e_shstrndx mismatch");
            }
        }

        [Test]
        public void TestDataSectionContent()
        {
            using (var stream = new MemoryStream(ElfFileBytes))
            using (var reader = new BinaryReader(stream))
            {
                // Seek to .data section at offset 0x1026 (4134)
                stream.Seek(4134, SeekOrigin.Begin);
                byte[] data = reader.ReadBytes(6);
                CollectionAssert.AreEqual(
                    new byte[] { 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x0A },
                    data,
                    "Data section should contain 'hello\\n'"
                );
            }
        }

        [Test]
        public void TestInvalidElfMagic()
        {
            byte[] invalidElf = new byte[] { 0x00, 0x45, 0x4C, 0x46, 0x02, 0x01, 0x01, 0x00, 0x00 };
            using (var stream = new MemoryStream(invalidElf))
            using (var reader = new BinaryReader(stream))
            {
                Assert.Throws<Exception>(
                    () => e_ident.FromBinaryReader(reader),
                    "Expected exception for invalid ELF magic"
                );
            }
        }

        [Test]
        public void TestNon64BitElf()
        {
            byte[] non64BitElf = new byte[] { 0x7F, 0x45, 0x4C, 0x46, 0x01, 0x01, 0x01, 0x00, 0x00 };
            using (var stream = new MemoryStream(non64BitElf))
            using (var reader = new BinaryReader(stream))
            {
                Assert.Throws<Exception>(
                    () => e_ident.FromBinaryReader(reader),
                    "Expected exception for non-64-bit ELF"
                );
            }
        }

        [Test]
        public void TestNonLittleEndianElf()
        {
            byte[] nonLittleEndianElf = new byte[] { 0x7F, 0x45, 0x4C, 0x46, 0x02, 0x02, 0x01, 0x00, 0x00 };
            using (var stream = new MemoryStream(nonLittleEndianElf))
            using (var reader = new BinaryReader(stream))
            {
                Assert.Throws<Exception>(
                    () => e_ident.FromBinaryReader(reader),
                    "Expected exception for non-little-endian ELF"
                );
            }
        }

        [Test]
        public void TestNonSystemVElf()
        {
            byte[] nonSystemVElf = new byte[] { 0x7F, 0x45, 0x4C, 0x46, 0x02, 0x01, 0x01, 0x01, 0x00 };
            using (var stream = new MemoryStream(nonSystemVElf))
            using (var reader = new BinaryReader(stream))
            {
                Assert.Throws<Exception>(
                    () => e_ident.FromBinaryReader(reader),
                    "Expected exception for non-System V OSABI"
                );
            }
        }

        [Test]
        public void TestNonZeroABIVersion()
        {
            byte[] nonZeroABIVersion = new byte[] { 0x7F, 0x45, 0x4C, 0x46, 0x02, 0x01, 0x01, 0x00, 0x01 };
            using (var stream = new MemoryStream(nonZeroABIVersion))
            using (var reader = new BinaryReader(stream))
            {
                Assert.Throws<Exception>(
                    () => e_ident.FromBinaryReader(reader),
                    "Expected exception for non-zero ABI version"
                );
            }
        }

        [Test]
        public void TestElfFileWrite()
        {
            using (var stream = new MemoryStream(ElfFileBytes))
            using (var reader = new BinaryReader(stream))
            {
                var elf = ELF_FILE.FromBinaryReader(reader);
                CollectionAssert.AreEqual(ElfFileBytes, elf.ToBytes(), "ELF file write mismatch");
            }
        }
    }
}