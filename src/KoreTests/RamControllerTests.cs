using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    class RamControllerTests
    {
        private Kore.MainBus bus;
        private Kore.CPU cpu;
        private Kore.RamController ram;
        [SetUp]
        public void Setup()
        {
            bus = new Kore.MainBus();
            cpu = new Kore.CPU(bus);
            ram = new Kore.RamController(bus, cpu.MemorySize);
        }

        [Test]
        public void Instantiate()
        {
            Assert.AreEqual(typeof(Kore.RamController), ram.GetType());
        }

        [Test, Ignore("Not Coded")]
        public void StackPointerAtInstantiate()
        {
            //If Correct command is on the bus at clockRise then place the answer on the bus
            //Or store the answer in the ram from the bus on rise if that is the command
        }

        [Test]
        public void SetGetMemoryMethods()
        {
            ushort MEMORY_SIZE = 1024;
            ushort MEMORY_TEST_SIZE = 32;
            bus = new Kore.MainBus();
            cpu = new Kore.CPU(bus, MEMORY_SIZE);
            ram = new Kore.RamController(bus, cpu.MemorySize);

            ram.store(add_addi_bin, 0, (ulong)add_addi_bin.Length, 0);

            Random rand = new Random();

            for (uint c = 0; c < MEMORY_TEST_SIZE; c++)
            {
                uint offset = (uint) rand.Next(0, MEMORY_SIZE- MEMORY_TEST_SIZE);
                byte[] signature = new byte[MEMORY_TEST_SIZE];
                rand.NextBytes(signature);

                for (uint i = 0; i < MEMORY_TEST_SIZE; i++)
                {
                    ram.setByte(offset + i, signature[i]);
                }

                for (uint i = 0; i < MEMORY_TEST_SIZE; i++)
                {
                    Assert.AreEqual(signature[i], ram.getByte(offset + i));
                }
            }
            
        }

        [Test]
        public void SetMemoryAddressPastOutOfRange()
        {
            Assert.Throws<IndexOutOfRangeException>(() => { ram.setByte(cpu.MemorySize, 0xff); });
            Random rand = new Random();
            for (int i = 0; i < 32; i++)
            {
                Assert.Throws<IndexOutOfRangeException>(() => { ram.setByte(cpu.MemorySize, 0xff); });
            }
        }

        readonly byte[] add_addi_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00, 0x13, 0x0F, 0x50, 0x02, 0xB3, 0x0F, 0xDF, 0x01 };

        [Test]
        public void StoreMethod()
        {
            bus = new Kore.MainBus();
            cpu = new Kore.CPU(bus, 16);
            ram = new Kore.RamController(bus, cpu.MemorySize);

            byte offset = 3;
            ram.store(add_addi_bin, 0, (ulong)add_addi_bin.Length, offset);

            for (uint i = 0; i < 16; i++)
            {
                if (i < offset) Assert.AreEqual(0x0, ram.getByte(i));
                else if (i < offset + add_addi_bin.Length) Assert.AreEqual(add_addi_bin[i - offset], ram.getByte(i));
                else Assert.AreEqual(0x0, ram.getByte(i));
            }

            for (uint i = 0; i < 16; i++)
            {
                ram.setByte(i, 0xFF);
            }

            for (uint i = 0; i < 16; i++)
            {
                Assert.AreEqual(0xFF, ram.getByte(i));
            }

            ram.store(add_addi_bin, 0, (ulong)add_addi_bin.Length, offset);

            for (uint i = 0; i < 16; i++)
            {
                if (i < offset) Assert.AreEqual(0xFF, ram.getByte(i));
                else if (i < offset + add_addi_bin.Length) Assert.AreEqual(add_addi_bin[i - offset], ram.getByte(i));
                else Assert.AreEqual(0xFF, ram.getByte(i));
            }
        }
    }
}
