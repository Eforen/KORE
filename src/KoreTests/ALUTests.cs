using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    class ALUTests
    {
        private Kore.MainBus bus;
        private Kore.CPU cpu;
        private Kore.RamController ram;
        private Kore.ALU64 alu;

        [SetUp]
        public void Setup()
        {
            bus = new Kore.MainBus();
            cpu = new Kore.CPU(bus);
            ram = new Kore.RamController(bus, cpu.MemorySize);
            alu = cpu.alu;
        }

        [Test]
        public void Instantiate()
        {
            Assert.AreEqual(typeof(Kore.ALU64), cpu.alu.GetType());
            Assert.AreSame(alu, cpu.alu);
        }

        //                             XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX      XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX
        [TestCase("1", new ulong[] { 0b00000000_00000000_00000000_00010000UL, 0b11111111_11111111_11111111_11110000UL, 5, 4 * 8 })]
        [TestCase("2", new ulong[] { 0b00000000_00000000_00000000_00111111UL, 0b11111111_11111111_11111111_11111111UL, 5, 4 * 8 })]
        [TestCase("3", new ulong[] { 0b00000000_00000000_00000000_00001111UL, 0b00000000_00000000_00000000_00001111UL, 5, 4 * 8 })]
        [TestCase("4", new ulong[] { 0b00000000_00000000_00000000_00000000UL, 0b00000000_00000000_00000000_00000000UL, 5, 4 * 8 })]

        //                             XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX      XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX
        [TestCase("5", new ulong[] { 0b11111111_11111111_11111111_11111111UL, 0b11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111UL, 4 * 8, 8 * 8 })]
        [TestCase("6", new ulong[] { 0b10000000_00000000_00000000_00000000UL, 0b11111111_11111111_11111111_11111111_10000000_00000000_00000000_00000000UL, 4 * 8, 8 * 8 })]
        [TestCase("7", new ulong[] { 0b10101110_11101110_10110010_10111111UL, 0b11111111_11111111_11111111_11111111_10101110_11101110_10110010_10111111UL, 4 * 8, 8 * 8 })]
        [TestCase("8", new ulong[] { 0b00100101_01010100_01011101_10101111UL, 0b00000000_00000000_00000000_00000000_00100101_01010100_01011101_10101111UL, 4 * 8, 8 * 8 })]
        [TestCase("9", new ulong[] { 0b01111111_11111111_11111111_11111111UL, 0b00000000_00000000_00000000_00000000_01111111_11111111_11111111_11111111UL, 4 * 8, 8 * 8 })]
        public void SignExtend(string desc, ulong[] arr)
        {
            ulong from = (ulong)arr[0], to = (ulong)arr[1];
            byte fromBits = (byte)arr[2], toBits = (byte)arr[3];

            ulong result = cpu.alu.signExtend(from, fromBits, toBits);
            Console.WriteLine("F: " + Convert.ToString((long)from, 2).PadLeft(64, '0'));
            Console.WriteLine("T: " + Convert.ToString((long)to, 2).PadLeft(64, '0'));
            Console.WriteLine("M: " + Convert.ToString((long)alu.getDataMask(fromBits, false), 2).PadLeft(64, '0'));
            Console.WriteLine("R: " + Convert.ToString((long)result, 2).PadLeft(64, '0'));
            Console.WriteLine("   " + Convert.ToString((long)alu.getDataMask(toBits, false), 2).PadLeft(64, '0'));
            Assert.AreEqual(to, result);
        }

        [TestCase(0, 0b0UL)]
        [TestCase(1, 0b01UL)]
        [TestCase(3, 0b0100UL)]
        [TestCase(10, 0b01000000000UL)]
        public void BitMask(byte bits, ulong target)
        {
            Assert.AreEqual(target, cpu.alu.getBitMask(bits));
        }
    }
}
