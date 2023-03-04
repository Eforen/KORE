using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using Kore.RiscISA;
using Kore.RiscMeta;

namespace KoreTests
{
    class RegisterFileTests
    {
        private Kore.RegisterFile reg;
        [SetUp]
        public void Setup()
        {
            reg = new Kore.RegisterFile();
        }

        [Test]
        public void Instantiate()
        {
            Kore.RegisterFile regTest = new Kore.RegisterFile();
            Assert.AreEqual(typeof(Kore.RegisterFile), regTest.GetType());
        }

        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Register.x1, new ulong[] { 0, (ulong)0x29928a0bac818116u, (ulong)0x952d2a4c42c3b899u })]
        [TestCase(Register.x1, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x1, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x1, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x1, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x1, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.x10, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.x10, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x10, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x10, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x10, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x10, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.x20, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.x20, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x20, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x20, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x20, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x20, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.x30, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.x30, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x30, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x30, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x30, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x30, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.x15, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.x15, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x15, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x15, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x15, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x15, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.x25, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.x25, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x25, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x25, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x25, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x25, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.x31, new ulong[] { 0, (ulong)0x29928a0bac818116u, (ulong)0x952d2a4c42c3b899u })]
        [TestCase(Register.x31, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.x31, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x31, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.x31, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.x31, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Register.PC, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.PC, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Register.PC, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.PC, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.PC, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Register.PC, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
        public void TestRegister08Bytes(Register r, ulong[] data)// startExpectation, ulong target1, ulong target2)
        {
            ulong startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, reg.getR(r));
            reg.setR(r, target1);
            Assert.AreEqual(target1, reg.getR(r));
            reg.setR(r, target2);
            Assert.AreEqual(target2, reg.getR(r));
        }

        [TestCase(Register.x0, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Register.zero, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase((Register)0x0, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Register.x0, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Register.zero, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase((Register)0x0, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
        public void TestRegisterZero(Register r, ulong[] data)// startExpectation, ulong target1, ulong target2)
        {
            ulong startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, reg.getR(r));
            reg.setR(r, target1);
            Assert.AreEqual(0, reg.getR(r));
            reg.setR(r, target2);
            Assert.AreEqual(0, reg.getR(r));
        }
    }
}
