using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    class RiscCPU
    {
        private Kore.CPU cpu;
        [SetUp]
        public void Setup()
        {
            cpu = new Kore.CPU();
        }

        [Test]
        public void Instantiate()
        {
            Kore.CPU cpuTest = new Kore.CPU();
            Assert.AreEqual(typeof(Kore.CPU), cpuTest.GetType());
        }

        [Test]
        public void StackPointerAtInstantiate()
        {
            ulong defaultSize = 1024 * 1024 * 128; // 128MB
            Assert.AreEqual(cpu.MemorySize, defaultSize, "Default size not 128MB");
            Assert.AreEqual(defaultSize - 1, cpu.getR(Kore.CPU.Register.sp), "Stack Pointer not set to Default size");

            int seed = 234682399;
            Random r = new Random(seed);
            for (int i = 0; i < 20; i++)
            {
                ulong amountOfRam = (ulong) r.Next(1024, 1024 * 1024 * 256); // Instantiate with anywhere from 1024B to 256MB of ram
                Kore.CPU cpuTest = new Kore.CPU(amountOfRam);

                Assert.AreEqual(cpuTest.MemorySize, amountOfRam, "memory size not correctly");
                Assert.AreEqual(amountOfRam - 1, cpuTest.getR(Kore.CPU.Register.sp), "Stack Pointer not set to max ram address");
            }
        }

        // Uses Arrays because of a quark of NUnit TestCase
        [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x29928a0bac818116u, (ulong)0x952d2a4c42c3b899u })]
        [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Kore.CPU.Register.x1, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Kore.CPU.Register.x10, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Kore.CPU.Register.x20, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Kore.CPU.Register.x30, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Kore.CPU.Register.x15, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]

        [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase(Kore.CPU.Register.x25, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
        public void TestRegister08Bytes(Kore.CPU.Register r, ulong[] data)// startExpectation, ulong target1, ulong target2)
        {
            ulong startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, cpu.getR(r));
            cpu.setR(r, target1);
            Assert.AreEqual(target1, cpu.getR(r));
            cpu.setR(r, target2);
            Assert.AreEqual(target2, cpu.getR(r));
        }

        [TestCase(Kore.CPU.Register.x0, new ulong[] { 0, (ulong)0x29928a0bac818116, (ulong)0x952d2a4c42c3b899 })]
        [TestCase(Kore.CPU.Register.zero, new ulong[] { 0, (ulong)0xec8525bd287984a6, (ulong)0x374814b7cc672d12 })]
        [TestCase((Kore.CPU.Register) 0xff, new ulong[] { 0, (ulong)0x1811dc85da0d217e, (ulong)0xff43757f49b222f1 })]
        [TestCase(Kore.CPU.Register.x0, new ulong[] { 0, (ulong)0x842843ad1907db58, (ulong)0xba9d3e0c4cd68b66 })]
        [TestCase(Kore.CPU.Register.zero, new ulong[] { 0, (ulong)0xea2b62db634c485f, (ulong)0xf03ed33799359b5d })]
        [TestCase((Kore.CPU.Register) 0xff, new ulong[] { 0, (ulong)0x204f36cfdc6aff52, (ulong)0x742c59eb74b11a06 })]
        public void TestRegisterZero(Kore.CPU.Register r, ulong[] data)// startExpectation, ulong target1, ulong target2)
        {
            ulong startExpectation = data[0], target1 = data[1], target2 = data[2];
            Assert.AreEqual(startExpectation, cpu.getR(r));
            cpu.setR(r, target1);
            Assert.AreEqual(0, cpu.getR(r));
            cpu.setR(r, target2);
            Assert.AreEqual(0, cpu.getR(r));
        }

        [Test, Ignore("Not made yet")]
        public void bin_add_addi()
        {
            // add_addi_bin contains the following instructions:
            // main:
            // .  addi x29, x0, 5   // Add 5 and 0, and store the value to x29.
            // .  addi x30, x0, 37  // Add 37 and 0, and store the value to x30.
            // .  add x31, x30, x29 // x31 should contain 42 (0x2a).
            //                                 00    01    02    03    04    05    06    07    08    09    10    11
            byte[] add_addi_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00, 0x13, 0x0F, 0x50, 0x02, 0xB3, 0x0F, 0xDF, 0x01 };

            // Run the above code

            // Get value of register x29 it should contain 0x05 (5)
            // Get value of register x30 it should contain 0x25 (37)
            // Get value of register x31 it should contain 0x2a (42)
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_R_Type()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_I_Type()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_S_Type()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_B_Type()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_U_Type()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_J_Type()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_I_immediate()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_S_immediate()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_B_immediate()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_U_immediate()
        {
        }

        [Test, Ignore("Not made yet")]
        public void Instruction_J_immediate()
        {
        }
    }
}
