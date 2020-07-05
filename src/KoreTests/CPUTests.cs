using NUnit.Framework;

namespace KoreTests
{
    public class CPUTests
    {
        private Kore.CPU cpu;
        [SetUp]
        public void Setup()
        {
            cpu = new Kore.CPU();
        }

        [Test]
        public void Instantiation()
        {
            Assert.IsInstanceOf<Kore.CPU>(cpu);
        }

        [TestCase(Kore.CPU.Register.A, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.A, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.A, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.A, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.A, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.A, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]

        [TestCase(Kore.CPU.Register.B, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.B, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.B, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.B, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.B, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.B, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]

        [TestCase(Kore.CPU.Register.BP, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]

        [TestCase(Kore.CPU.Register.C, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.C, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.C, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.C, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.C, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.C, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]

        [TestCase(Kore.CPU.Register.D, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.D, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.D, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.D, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.D, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.D, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]

        [TestCase(Kore.CPU.Register.IP, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]

        [TestCase(Kore.CPU.Register.SP, 0, 0x29928a0bac818116, 0x952d2a4c42c3b899)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xec8525bd287984a6, 0x374814b7cc672d12)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x1811dc85da0d217e, 0xff43757f49b222f1)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x842843ad1907db58, 0xba9d3e0c4cd68b66)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xea2b62db634c485f, 0xf03ed33799359b5d)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x204f36cfdc6aff52, 0x742c59eb74b11a06)]
        public void TestRegister08Bytes(Kore.CPU.Register r, ulong startExpectation, ulong target1, ulong target2)
        {
            Assert.AreEqual(startExpectation, cpu.getRX(r));
            cpu.setRX(r, target1);
            Assert.AreEqual(target1, cpu.getRX(r));
            cpu.setRX(r, target2);
            Assert.AreEqual(target2, cpu.getRX(r));
        }

        [TestCase(Kore.CPU.Register.A, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.A, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.A, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.A, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.A, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.A, 0, 0x204f36cf, 0x742c59eb)]

        [TestCase(Kore.CPU.Register.B, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.B, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.B, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.B, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.B, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.B, 0, 0x204f36cf, 0x742c59eb)]

        [TestCase(Kore.CPU.Register.BP, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x204f36cf, 0x742c59eb)]

        [TestCase(Kore.CPU.Register.C, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.C, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.C, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.C, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.C, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.C, 0, 0x204f36cf, 0x742c59eb)]

        [TestCase(Kore.CPU.Register.D, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.D, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.D, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.D, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.D, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.D, 0, 0x204f36cf, 0x742c59eb)]

        [TestCase(Kore.CPU.Register.IP, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x204f36cf, 0x742c59eb)]

        [TestCase(Kore.CPU.Register.SP, 0, 0x29928a0b, 0x952d2a4c)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xec8525bd, 0x374814b7)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x1811dc85, 0xff43757f)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x842843ad, 0xba9d3e0c)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xea2b62db, 0xf03ed337)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x204f36cf, 0x742c59eb)]
        public void TestRegister04Bytes(Kore.CPU.Register r, uint startExpectation, uint target1, uint target2)
        {
            Assert.AreEqual(startExpectation, cpu.getEX(r));
            cpu.setEX(r, target1);
            Assert.AreEqual(target1, cpu.getEX(r));
            cpu.setEX(r, target2);
            Assert.AreEqual(target2, cpu.getEX(r));
        }

        [TestCase(Kore.CPU.Register.A, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.A, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.A, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.A, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.A, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.A, 0, 0x204f, 0x742c)]

        [TestCase(Kore.CPU.Register.B, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.B, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.B, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.B, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.B, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.B, 0, 0x204f, 0x742c)]

        [TestCase(Kore.CPU.Register.BP, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x204f, 0x742c)]

        [TestCase(Kore.CPU.Register.C, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.C, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.C, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.C, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.C, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.C, 0, 0x204f, 0x742c)]

        [TestCase(Kore.CPU.Register.D, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.D, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.D, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.D, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.D, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.D, 0, 0x204f, 0x742c)]

        [TestCase(Kore.CPU.Register.IP, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x204f, 0x742c)]

        [TestCase(Kore.CPU.Register.SP, 0, 0x2992, 0x952d)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xec85, 0x3748)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x1811, 0xff43)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x8428, 0xba9d)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xea2b, 0xf03e)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x204f, 0x742c)]
        public void TestRegister02Bytes(Kore.CPU.Register r, ushort startExpectation, ushort target1, ushort target2)
        {
            Assert.AreEqual(startExpectation, cpu.getX(r));
            cpu.setX(r, target1);
            Assert.AreEqual(target1, cpu.getX(r));
            cpu.setX(r, target2);
            Assert.AreEqual(target2, cpu.getX(r));
        }

        [TestCase(Kore.CPU.Register.A, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.A, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.A, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.A, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.A, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.A, 0, 0x20, 0x74)]

        [TestCase(Kore.CPU.Register.B, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.B, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.B, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.B, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.B, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.B, 0, 0x20, 0x74)]

        [TestCase(Kore.CPU.Register.BP, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.BP, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x20, 0x74)]

        [TestCase(Kore.CPU.Register.C, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.C, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.C, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.C, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.C, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.C, 0, 0x20, 0x74)]

        [TestCase(Kore.CPU.Register.D, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.D, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.D, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.D, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.D, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.D, 0, 0x20, 0x74)]

        [TestCase(Kore.CPU.Register.IP, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.IP, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x20, 0x74)]

        [TestCase(Kore.CPU.Register.SP, 0, 0x29, 0x95)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xec, 0x37)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x18, 0xff)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x84, 0xba)]
        [TestCase(Kore.CPU.Register.SP, 0, 0xea, 0xf0)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x20, 0x74)]
        public void TestRegisterHighByte(Kore.CPU.Register r, byte startExpectation, byte target1, byte target2)
        {
            Assert.AreEqual(startExpectation, cpu.getH(r));
            cpu.setH(r, target1);
            Assert.AreEqual(target1, cpu.getH(r));
            cpu.setH(r, target2);
            Assert.AreEqual(target2, cpu.getH(r));
        }

        [TestCase(Kore.CPU.Register.A, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.A, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.A, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.A, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.A, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.A, 0, 0x4f, 0x2c)]

        [TestCase(Kore.CPU.Register.B, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.B, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.B, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.B, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.B, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.B, 0, 0x4f, 0x2c)]

        [TestCase(Kore.CPU.Register.BP, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.BP, 0, 0x4f, 0x2c)]

        [TestCase(Kore.CPU.Register.C, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.C, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.C, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.C, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.C, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.C, 0, 0x4f, 0x2c)]

        [TestCase(Kore.CPU.Register.D, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.D, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.D, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.D, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.D, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.D, 0, 0x4f, 0x2c)]

        [TestCase(Kore.CPU.Register.IP, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.IP, 0, 0x4f, 0x2c)]

        [TestCase(Kore.CPU.Register.SP, 0, 0x92, 0x2d)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x85, 0x48)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x11, 0x43)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x28, 0x9d)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x2b, 0x3e)]
        [TestCase(Kore.CPU.Register.SP, 0, 0x4f, 0x2c)]
        public void TestRegisterLowByte(Kore.CPU.Register r, byte startExpectation, byte target1, byte target2)
        {
            Assert.AreEqual(startExpectation, cpu.getL(r));
            cpu.setL(r, target1);
            Assert.AreEqual(target1, cpu.getL(r));
            cpu.setL(r, target2);
            Assert.AreEqual(target2, cpu.getL(r));
        }
    }
}