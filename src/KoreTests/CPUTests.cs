using NUnit.Framework;
using System;
using System.Linq;

namespace KoreTests
{
    public class CPUTests
    {
//         private Kore.CPU cpu;
//         [SetUp]
//         public void Setup()
//         {
//             cpu = new Kore.CPU();
//         }
// 
//         [Test]
//         public void Instantiation()
//         {
//             Assert.IsInstanceOf<Kore.CPU>(cpu);
//         }
// 
//         // Uses Arrays because of a quark of NUnit TestCase
//         [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x29928a0bac818116u, (ulong)0x952d2a4c42c3b899u })]
//         [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
//         [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
//         [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
//         [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
//         [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
// 
//         [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
//         [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
//         [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
//         [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
//         [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
//         [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
// 
//         [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
//         [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
//         [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
//         [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
//         [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
//         [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
// 
//         [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
//         [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
//         [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
//         [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
//         [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
//         [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
// 
//         [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
//         [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
//         [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
//         [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
//         [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
//         [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
// 
//         [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
//         [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
//         [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
//         [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
//         [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
//         [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
//         public void TestRegister08Bytes(Kore.CPU.Register r, ulong[] data)// startExpectation, ulong target1, ulong target2)
//         {
//             ulong startExpectation = data[0], target1 = data[1], target2 = data[2];
//             Assert.AreEqual(startExpectation, cpu.getR(r));
//             cpu.setR(r, target1);
//             Assert.AreEqual(target1, cpu.getR(r));
//             cpu.setR(r, target2);
//             Assert.AreEqual(target2, cpu.getR(r));
//         }

        /*
        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.A, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.A, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.A, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.A, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.A, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.A, new uint[] { 0, 0x204f36cf, 0x742c59eb })]

        [TestCase(Kore.CPU.Register.B, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.B, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.B, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.B, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.B, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.B, new uint[] { 0, 0x204f36cf, 0x742c59eb })]

        [TestCase(Kore.CPU.Register.BP, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.BP, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.BP, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.BP, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.BP, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.BP, new uint[] { 0, 0x204f36cf, 0x742c59eb })]

        [TestCase(Kore.CPU.Register.C, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.C, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.C, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.C, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.C, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.C, new uint[] { 0, 0x204f36cf, 0x742c59eb })]

        [TestCase(Kore.CPU.Register.D, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.D, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.D, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.D, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.D, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.D, new uint[] { 0, 0x204f36cf, 0x742c59eb })]

        [TestCase(Kore.CPU.Register.IP, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.IP, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.IP, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.IP, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.IP, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.IP, new uint[] { 0, 0x204f36cf, 0x742c59eb })]

        [TestCase(Kore.CPU.Register.SP, new uint[] { 0, 0x29928a0b, 0x952d2a4c })]
        [TestCase(Kore.CPU.Register.SP, new uint[] { 0, 0xec8525bd, 0x374814b7 })]
        [TestCase(Kore.CPU.Register.SP, new uint[] { 0, 0x1811dc85, 0xff43757f })]
        [TestCase(Kore.CPU.Register.SP, new uint[] { 0, 0x842843ad, 0xba9d3e0c })]
        [TestCase(Kore.CPU.Register.SP, new uint[] { 0, 0xea2b62db, 0xf03ed337 })]
        [TestCase(Kore.CPU.Register.SP, new uint[] { 0, 0x204f36cf, 0x742c59eb })]
        public void TestRegister04Bytes(Kore.CPU.Register r, uint[] data)
        {
            uint startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, cpu.getEX(r));
            cpu.setEX(r, target1);
            Assert.AreEqual(target1, cpu.getEX(r));
            cpu.setEX(r, target2);
            Assert.AreEqual(target2, cpu.getEX(r));
        }

        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.A, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.A, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.A, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.A, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.A, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.A, new ushort[] { 0, 0x204f, 0x742c })]

        [TestCase(Kore.CPU.Register.B, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.B, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.B, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.B, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.B, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.B, new ushort[] { 0, 0x204f, 0x742c })]

        [TestCase(Kore.CPU.Register.BP, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.BP, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.BP, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.BP, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.BP, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.BP, new ushort[] { 0, 0x204f, 0x742c })]

        [TestCase(Kore.CPU.Register.C, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.C, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.C, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.C, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.C, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.C, new ushort[] { 0, 0x204f, 0x742c })]

        [TestCase(Kore.CPU.Register.D, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.D, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.D, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.D, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.D, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.D, new ushort[] { 0, 0x204f, 0x742c })]

        [TestCase(Kore.CPU.Register.IP, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.IP, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.IP, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.IP, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.IP, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.IP, new ushort[] { 0, 0x204f, 0x742c })]

        [TestCase(Kore.CPU.Register.SP, new ushort[] { 0, 0x2992, 0x952d })]
        [TestCase(Kore.CPU.Register.SP, new ushort[] { 0, 0xec85, 0x3748 })]
        [TestCase(Kore.CPU.Register.SP, new ushort[] { 0, 0x1811, 0xff43 })]
        [TestCase(Kore.CPU.Register.SP, new ushort[] { 0, 0x8428, 0xba9d })]
        [TestCase(Kore.CPU.Register.SP, new ushort[] { 0, 0xea2b, 0xf03e })]
        [TestCase(Kore.CPU.Register.SP, new ushort[] { 0, 0x204f, 0x742c })]
        public void TestRegister02Bytes(Kore.CPU.Register r, ushort[] data)
        {
            ushort startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, cpu.getX(r));
            cpu.setX(r, target1);
            Assert.AreEqual(target1, cpu.getX(r));
            cpu.setX(r, target2);
            Assert.AreEqual(target2, cpu.getX(r));
        }

        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x20, 0x74 })]

        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x20, 0x74 })]

        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x20, 0x74 })]

        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x20, 0x74 })]

        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x20, 0x74 })]

        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x20, 0x74 })]

        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x29, 0x95 })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0xec, 0x37 })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x18, 0xff })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x84, 0xba })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0xea, 0xf0 })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x20, 0x74 })]
        public void TestRegisterHighByte(Kore.CPU.Register r, byte[] data)
        {
            byte startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, cpu.getH(r));
            cpu.setH(r, target1);
            Assert.AreEqual(target1, cpu.getH(r));
            cpu.setH(r, target2);
            Assert.AreEqual(target2, cpu.getH(r));
        }

        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.A, new byte[] { 0, 0x4f, 0x2c })]

        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.B, new byte[] { 0, 0x4f, 0x2c })]

        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.BP, new byte[] { 0, 0x4f, 0x2c })]

        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.C, new byte[] { 0, 0x4f, 0x2c })]

        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.D, new byte[] { 0, 0x4f, 0x2c })]

        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.IP, new byte[] { 0, 0x4f, 0x2c })]

        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x92, 0x2d })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x85, 0x48 })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x11, 0x43 })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x28, 0x9d })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x2b, 0x3e })]
        [TestCase(Kore.CPU.Register.SP, new byte[] { 0, 0x4f, 0x2c })]
        public void TestRegisterLowByte(Kore.CPU.Register r, byte[] data)
        {
            byte startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, cpu.getL(r));
            cpu.setL(r, target1);
            Assert.AreEqual(target1, cpu.getL(r));
            cpu.setL(r, target2);
            Assert.AreEqual(target2, cpu.getL(r));
        }

        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]

        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]

        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]

        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]

        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]

        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]

        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x29928a0bac818116 })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0xec8525bd287984a6 })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x1811dc85da0d217e })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x842843ad1907db58 })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0xea2b62db634c485f })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x204f36cfdc6aff52 })]
        public void TestRegisterZeroing(Kore.CPU.Register r, ulong[] data)
        {
            ulong startExpectation = (ulong)data[0], tLong = (ulong)data[1];

            ulong target = startExpectation;

            Assert.AreEqual(target, cpu.getRX(r));

            // Reset and check zeroing out EX
            target = 0;
            cpu.setRX(r, tLong);
            target = tLong;
            Assert.AreEqual(target, cpu.getRX(r)); // Insure state is as expected
            cpu.setEX(r, 0);
            target = target & 0xFF_FF_FF_FF_00_00_00_00u;
            Assert.AreEqual(target, cpu.getRX(r), "E"+Enum.GetName(typeof(Kore.CPU.Register),r)+ "X Register zero out not working correctly");

            // Reset and check zeroing out X
            target = 0;
            cpu.setRX(r, tLong);
            target = tLong;
            Assert.AreEqual(target, cpu.getRX(r)); // Insure state is as expected
            cpu.setX(r, 0);
            target = target & 0xFF_FF_FF_FF_FF_FF_00_00u;
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register zero out not working correctly");

            // Reset and check zeroing out H
            target = 0;
            cpu.setRX(r, tLong);
            target = tLong;
            Assert.AreEqual(target, cpu.getRX(r)); // Insure state is as expected
            cpu.setH(r, 0);
            target = target & 0xFF_FF_FF_FF_FF_FF_00_FFu;
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "H Register zero out not working correctly");

            // Reset and check zeroing out L
            target = 0;
            cpu.setRX(r, tLong);
            target = tLong;
            Assert.AreEqual(target, cpu.getRX(r)); // Insure state is as expected
            cpu.setL(r, 0);
            target = target & 0xFF_FF_FF_FF_FF_FF_FF_00u;
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "L Register zero out not working correctly");
        }


        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.A, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]

        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.B, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]

        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.BP, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]

        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.C, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]

        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.D, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]

        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.IP, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]

        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x952d2a4c42c3b899u, 0x29928a0bu, 0x2992u, 0x95u, 0x92u })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x374814b7cc672d12u, 0xec8525bdu, 0xec85u, 0x37u, 0x85u })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0xff43757f49b222f1u, 0x1811dc85u, 0x1811u, 0xffu, 0x11u })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0xba9d3e0c4cd68b66u, 0x842843adu, 0x8428u, 0xbau, 0x28u })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0xf03ed33799359b5du, 0xea2b62dbu, 0xea2bu, 0xf0u, 0x2bu })]
        [TestCase(Kore.CPU.Register.SP, new ulong[] { 0, (ulong)0x742c59eb74b11a06u, 0x204f36cfu, 0x204fu, 0x74u, 0x4fu })]
        public void TestRegisterConversion(Kore.CPU.Register r, ulong[] data)
        {
            ulong startExpectation = (ulong) data[0], tLong = (ulong) data[1];
            uint tInt = (uint) data[2];
            ushort tShort = (ushort) data[3];
            byte tHigh = (byte) data[4];
            byte tLow = (byte) data[5];

            ulong target = startExpectation;

            Assert.AreEqual(target, cpu.getRX(r));

            cpu.setRX(r, tLong);
            target = tLong;
            Assert.AreEqual(tLong, cpu.getRX(r), "R" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register not working correctly");

            cpu.setEX(r, tInt);
            target = target & 0xFF_FF_FF_FF_00_00_00_00u | tInt;
            Assert.AreEqual(target, cpu.getRX(r), "E" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register not working correctly");

            cpu.setX(r, tShort);
            target = target & 0xFF_FF_FF_FF_FF_FF_00_00u | tShort;
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register not working correctly");

            cpu.setH(r, tHigh);
            target = target & 0xFF_FF_FF_FF_FF_FF_00_FFu | (ushort)((ushort)(tHigh) << 8);
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "H Register not working correctly");

            cpu.setL(r, tLow);
            target = target & 0xFF_FF_FF_FF_FF_FF_FF_00u | tLow;
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "L Register not working correctly");

            //Back up again just in case
            cpu.setL(r, tLow);
            // Don't change target because should stay the same
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "L Register not working correctly");

            cpu.setH(r, tHigh);
            target = target & 0xFF_FF_FF_FF_FF_FF_00_FFu | (ulong)(tHigh << 8);
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "H Register not working correctly");

            cpu.setX(r, tShort);
            target = target & 0xFF_FF_FF_FF_FF_FF_00_00u | tShort;
            Assert.AreEqual(target, cpu.getRX(r), "" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register not working correctly");

            cpu.setEX(r, tInt);
            target = target & 0xFF_FF_FF_FF_00_00_00_00u | tInt;
            Assert.AreEqual(target, cpu.getRX(r), "E" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register not working correctly");

            cpu.setRX(r, tLong);
            target = tLong;
            Assert.AreEqual(target, cpu.getRX(r), "R" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register not working correctly");

            cpu.setRX(r, 0);
            target = 0;
            Assert.AreEqual(target, cpu.getRX(r), "R" + Enum.GetName(typeof(Kore.CPU.Register), r) + "X Register zero out not working correctly");
        }

        */
    }
}