using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    class RiscCPU
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
            Assert.AreEqual(typeof(Kore.CPU), cpu.GetType());
        }

        [Test]
        public void StackPointerAtInstantiate()
        {
            ulong defaultSize = 1024 * 1024 * 128; // 128MB
            Assert.AreEqual(cpu.MemorySize, defaultSize, "Default size not 128MB");
            Assert.AreEqual(defaultSize - 1, cpu.registers.getR(Kore.RegisterFile.Register.sp), "Stack Pointer not set to Default size");

            int seed = 234682399;
            Random r = new Random(seed);
            for (int i = 0; i < 20; i++)
            {
                ulong amountOfRam = (ulong) r.Next(1024, 1024 * 1024 * 256); // Instantiate with anywhere from 1024B to 256MB of ram
                Kore.CPU cpuTest = new Kore.CPU(bus, amountOfRam);

                Assert.AreEqual(cpuTest.MemorySize, amountOfRam, "memory size not correctly");
                Assert.AreEqual(amountOfRam - 1, cpuTest.registers.getR(Kore.RegisterFile.Register.sp), "Stack Pointer not set to max ram address");
            }
        }

        // add_addi_bin contains the following instructions:
        // main:
        // .  addi x29, x0, 5   // Add 5 and 0, and store the value to x29.
        // .  addi x30, x0, 37  // Add 37 and 0, and store the value to x30.
        // .  add x31, x30, x29 // x31 should contain 42 (0x2a).
        //                                           00    01    02    03    04    05    06    07    08    09    10    11
        readonly byte[] add_addi_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00, 0x13, 0x0F, 0x50, 0x02, 0xB3, 0x0F, 0xDF, 0x01 };

        [Test]
        public void VonNeumannLoop()
        {
            //TODO: Add interrupt Capabilities here somewhere
            //The processor should complete a single step of the loop in a clock pulse
            ram.store(add_addi_bin, 0, (ulong)add_addi_bin.Length, 0);

            // Step 1: Fetch Instruction
            // Step 2: Decode Instruction
            // Step 3: Read Source Operands [This step may include interaction with the Main Bus]
            // Step 3.up: put command and address on the bus
            // Step 3.down: pull the data off the bus and clear it
            // Step 4: Execute
            // Step 5: Write Destination Operand
            // Step 6: Move PC to next position +4 or jump location

            Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Off);
            cpu.turnOn();
            Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Init);
            bus.tick();
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Fetch);
                bus.tick();
                Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Decode);
                bus.tick();
                Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Read);
                bus.tick();
                Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Exec);
                bus.tick();
                Assert.AreEqual(cpu.state, Kore.CPU.Cycle.Write);
                bus.tick();
                Assert.AreEqual(cpu.state, Kore.CPU.Cycle.MovPC);
                bus.tick();
                cpu.registers.getR(Kore.RegisterFile.Register.PC);
            }
        }

        [Test, Ignore("Not made yet")]
        public void bin_add_addi()
        {
            // Run the add_addi_bin code

            // Get value of register x29 it should contain 0x05 (5)
            Assert.AreEqual(0x05, cpu.registers.getR(Kore.RegisterFile.Register.x29));
            // Get value of register x30 it should contain 0x25 (37)
            Assert.AreEqual(0x25, cpu.registers.getR(Kore.RegisterFile.Register.x30));
            // Get value of register x31 it should contain 0x2a (42)
            Assert.AreEqual(0x2a, cpu.registers.getR(Kore.RegisterFile.Register.x31));
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
