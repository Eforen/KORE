﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Kore.RiscISA;

namespace KoreTests
{
    class RiscCPUTests
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
            Assert.AreEqual(defaultSize - 1, cpu.registers.getR(Register.sp), "Stack Pointer not set to Default size");

            int seed = 234682399;
            Random r = new Random(seed);
            for (int i = 0; i < 20; i++)
            {
                ulong amountOfRam = (ulong)r.Next(1024, 1024 * 1024 * 256); // Instantiate with anywhere from 1024B to 256MB of ram
                Kore.CPU cpuTest = new Kore.CPU(bus, amountOfRam);

                Assert.AreEqual(cpuTest.MemorySize, amountOfRam, "memory size not correctly");
                Assert.AreEqual(amountOfRam - 1, cpuTest.registers.getR(Register.sp), "Stack Pointer not set to max ram address");
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

            Assert.AreEqual(Kore.CPU.Cycle.Off, cpu.state);
            cpu.turnOn();
            Assert.AreEqual(Kore.CPU.Cycle.Init, cpu.state);
            bus.tick();
            for (int i = 1; i <= 100; i++)
            {
                Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Read, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Exec, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Write, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.MovPC, cpu.state);
                bus.tick();
                Assert.AreEqual(i * 4, cpu.registers.getR(Register.PC));
            }
        }

        [Test]
        public void bin_add_addi()
        {
            // Run the add_addi_bin code
            ram.store(add_addi_bin, 0, (ulong)add_addi_bin.Length, 0);

            Assert.AreEqual(Kore.CPU.Cycle.Off, cpu.state);
            cpu.turnOn();
            Assert.AreEqual(Kore.CPU.Cycle.Init, cpu.state);
            bus.tick();
            for (int i = 1; i <= 3; i++)
            {
                Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Read, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Exec, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.Write, cpu.state);
                bus.tick();
                Assert.AreEqual(Kore.CPU.Cycle.MovPC, cpu.state);
                bus.tick();
                Assert.AreEqual(i * 4, cpu.registers.getR(Register.PC));
            }

            // Get value of register x29 it should contain 0x05 (5)
            Assert.AreEqual(0x05, cpu.registers.getR(Register.x29));
            // Get value of register x30 it should contain 0x25 (37)
            Assert.AreEqual(0x25, cpu.registers.getR(Register.x30));
            // Get value of register x31 it should contain 0x2a (42)
            Assert.AreEqual(0x2a, cpu.registers.getR(Register.x31));
        }

        [Test]
        public void Instruction_R_Type_CPU()
        {
            //  add x31, x30, x29 // x31 should contain 42 (0x2a).
            //                             00    01    02    03  
            byte[] add_bin = new byte[] { 0xB3, 0x0F, 0xDF, 0x01 };
            ulong add01 = Kore.Utility.Misc.toDWords(add_bin)[0];

            // Set Registers
            cpu.registers.setR(Register.x29, 5);
            cpu.registers.setR(Register.x30, 37);

            //              0b0000000000000
            Assert.AreEqual(0b00000001_11011111_00001111_10110011, add01);

            // Place program in CPU Ram
            ram.store(add_bin, 0, (ulong)add_bin.Length, 0);

            Assert.AreEqual(Kore.CPU.Cycle.Off, cpu.state);
            cpu.turnOn();
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
            bus.tick();
            // Should now have encoded instruction
            Assert.AreEqual(add01, cpu.currentInstruction);

            Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
            bus.tick();
            // Should now have decoded instruction

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.ADD, cpu.currentRType.opcode);
            Assert.AreEqual(Register.x31, cpu.currentRType.rd);
            Assert.AreEqual(0b000, cpu.currentRType.func3);
            Assert.AreEqual(Register.x30, cpu.currentRType.rs1);
            Assert.AreEqual(Register.x29, cpu.currentRType.rs2);

            Kore.RiscISA.Instruction.RType hold = cpu.currentRType;

            bus.tick();
            bus.tick();
            bus.tick();
            bus.tick();
            Assert.AreEqual(37+5, cpu.registers.getR(Register.x31));
            Assert.AreEqual(0x4, cpu.registers.getR(Register.PC));
        }

        [Test]
        public void Instruction_I_Type_CPU()
        {
            // .  addi x29, x0, 5   // Add 5 and 0, and store the value to x29.
            // .  addi x30, x0, 37  // Add 37 and 0, and store the value to x30.
            //                              00    01    02    03    04    05    06    07  
            byte[] addi_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00, 0x13, 0x0F, 0x50, 0x02 };
            byte[] addi01_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00 };
            byte[] addi02_bin = new byte[] { 0x13, 0x0F, 0x50, 0x02 };

            ulong addi01 = Kore.Utility.Misc.toDWords(addi01_bin)[0]; // 0b0000_0000_0000_0 101_00 00_0 000_00 10_1001
            ulong addi02 = Kore.Utility.Misc.toDWords(addi02_bin)[0]; // 0b0000_0010_0101_0 000_00 00_1 111_00 01_0011

            //              0b0000000000000
            Assert.AreEqual(0b00000000_01010000_00001110_10010011, addi01);
            Assert.AreEqual(0b00000010_01010000_00001111_00010011, addi02);

            // Place program in CPU Ram
            ram.store(addi_bin, 0, (ulong)addi_bin.Length, 0);

            Assert.AreEqual(Kore.CPU.Cycle.Off, cpu.state);
            cpu.turnOn();
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
            bus.tick();
            // Should now have encoded instruction
            Assert.AreEqual(addi01, cpu.currentInstruction);

            Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
            bus.tick();
            // Should now have decoded instruction

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.ADDI, cpu.currentIType.opcode);
            Assert.AreEqual(Register.x29, cpu.currentIType.rd);
            Assert.AreEqual(0b000, cpu.currentIType.func3);
            Assert.AreEqual(Register.x0, cpu.currentIType.rs1);
            Assert.AreEqual(0b00000000_0101, cpu.currentIType.imm);

            Kore.RiscISA.Instruction.IType hold = cpu.currentIType;

            bus.tick();
            bus.tick();
            bus.tick();
            bus.tick();
            Assert.AreEqual(5, cpu.registers.getR(Register.x29));
            Assert.AreEqual(0x4, cpu.registers.getR(Register.PC));
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
            bus.tick();
            // Should now have encoded instruction
            Assert.AreEqual(addi02, cpu.currentInstruction);

            Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
            bus.tick();
            // Should now have decoded instruction

            Assert.AreSame(hold, cpu.currentIType);

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.ADDI, cpu.currentIType.opcode);
            Assert.AreEqual(Register.x30, cpu.currentIType.rd);
            Assert.AreEqual(0b000, cpu.currentIType.func3);
            Assert.AreEqual(Register.x0, cpu.currentIType.rs1);
            Assert.AreEqual(0b00000010_0101, cpu.currentIType.imm);

            bus.tick();
            bus.tick();
            bus.tick();
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
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
    class RiscV64iInstructions
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
        public void addi()
        {
            // .  addi x29, x0, 5   // Add 5 and 0, and store the value to x29.
            // .  addi x30, x0, 37  // Add 37 and 0, and store the value to x30.
            //                              00    01    02    03    04    05    06    07  
            byte[] addi_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00, 0x13, 0x0F, 0x50, 0x02 };

            ram.store(addi_bin, 0, (ulong)addi_bin.Length, 0);

            Assert.AreEqual(Kore.CPU.Cycle.Off, cpu.state);
            cpu.turnOn();
            Assert.AreEqual(Kore.CPU.Cycle.Init, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Read, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Exec, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Write, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.MovPC, cpu.state);
            bus.tick();
            Assert.AreEqual(5, cpu.registers.getR(Register.x29));
            Assert.AreEqual(0x4, cpu.registers.getR(Register.PC));
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Read, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Exec, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Write, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.MovPC, cpu.state);
            bus.tick();
            Assert.AreEqual(5, cpu.registers.getR(Register.x29));
            Assert.AreEqual(37, cpu.registers.getR(Register.x30));
            Assert.AreEqual(0x8, cpu.registers.getR(Register.PC));
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
        }

    }
}