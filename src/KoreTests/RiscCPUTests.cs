using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Kore.RiscISA;
using Kore.RiscISA.Instruction;

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

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_ADD, cpu.currentRType.opcode);
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

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_ADDI, cpu.currentIType.opcode);
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

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_ADDI, cpu.currentIType.opcode);
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
// 
//         [Test, Ignore("Not made yet")]
//         public void Instruction_I_immediate()
//         {
//         }
// 
//         [Test, Ignore("Not made yet")]
//         public void Instruction_S_immediate()
//         {
//         }
// 
//         [Test, Ignore("Not made yet")]
//         public void Instruction_B_immediate()
//         {
//         }
// 
//         [Test, Ignore("Not made yet")]
//         public void Instruction_U_immediate()
//         {
//         }
// 
//         [Test, Ignore("Not made yet")]
//         public void Instruction_J_immediate()
//         {
//         }
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

        [TestCase(0x00ul, "addi a3,a0,4", 0x00450693UL, INST_TYPE.IType, new ulong[] { (byte)Register.a0, 5 }, new ulong[] { (byte)Register.a3, 5 + 4 })]
        [TestCase(0x00ul, "addi a4,x0,1", 0x00100713UL, INST_TYPE.IType, new ulong[] { }, new ulong[] { (byte)Register.a4, 1 })]
        [TestCase(0x00ul, "08:1 0x00b76463  bltu     a4,a1,0x10  (+8)", 0x00b76463UL, INST_TYPE.BType, new ulong[] { (byte)Register.a4, 5, (byte)Register.a1, 9 }, new ulong[] { (byte)Register.PC, 0x08 })] // if (rs1 < rs2) pc += sext(offset) {Offset from 0 not 8}
        [TestCase(0x00ul, "08:2 0x00b76463  bltu     a4,a1,0x10  (+8)", 0x00b76463UL, INST_TYPE.BType, new ulong[] { (byte)Register.a4, 5, (byte)Register.a1, 5 }, new ulong[] { (byte)Register.PC, 0x04 })] // if (rs1 < rs2) pc += sext(offset) {Offset from 0 not 8}
        [TestCase(0x00ul, "08:3 0x00b76463  bltu     a4,a1,0x10  (+8)", 0x00b76463UL, INST_TYPE.BType, new ulong[] { (byte)Register.a4, 5, (byte)Register.a1, 4 }, new ulong[] { (byte)Register.PC, 0x04 })] // if (rs1 < rs2) pc += sext(offset) {Offset from 0 not 8}
        [TestCase(0x20ul, "20:1 0x01185a63  bge      a6,a7,0x34  (+0x14)", 0x01185a63UL, INST_TYPE.BType, new ulong[] { (byte)Register.a6, 5, (byte)Register.a7, 4 }, new ulong[] { (byte)Register.PC, 0x34 })]
        [TestCase(0x20ul, "20:2 0x01185a63  bge      a6,a7,0x34  (+0x14)", 0x01185a63UL, INST_TYPE.BType, new ulong[] { (byte)Register.a6, 5, (byte)Register.a7, 5 }, new ulong[] { (byte)Register.PC, 0x34 })]
        [TestCase(0x20ul, "20:3 0x01185a63  bge      a6,a7,0x34  (+0x14)", 0x01185a63UL, INST_TYPE.BType, new ulong[] { (byte)Register.a6, 5, (byte)Register.a7, 6 }, new ulong[] { (byte)Register.PC, 0x24 })]
        [TestCase(0x20ul, "20:3 0x01185a63  bge      a6,a7,0x34  (+0x14)", 0x01185a63UL, INST_TYPE.BType, new ulong[] { (byte)Register.a6, 5, (byte)Register.a7, 0xFFFF_FFF0 }, new ulong[] { (byte)Register.PC, 0x24 })]
        [TestCase(0x30ul, "30:1 0xfe0796e3  bne      a5,x0,0x1c  (-0x14)", 0xfe0796e3UL, INST_TYPE.BType, new ulong[] { (byte)Register.a5, 0, (byte)Register.x0, 0 }, new ulong[] { (byte)Register.PC, 0x34 })]
        [TestCase(0x30ul, "30:2 0xfe0796e3  bne      a5,x0,0x1c  (-0x14)", 0xfe0796e3UL, INST_TYPE.BType, new ulong[] { (byte)Register.a5, 5, (byte)Register.x0, 0 }, new ulong[] { (byte)Register.PC, 0x1c })]
        [TestCase(0x00ul, "00:1 0x02050463  beqz     a0,0x28", 0x02050463UL, INST_TYPE.BType, new ulong[] { (byte)Register.a0, 5, (byte)Register.x0, 0 }, new ulong[] { (byte)Register.PC, 0x04 })]
        [TestCase(0x00ul, "00:2 0x02050463  beqz     a0,0x28", 0x02050463UL, INST_TYPE.BType, new ulong[] { (byte)Register.a0, 0, (byte)Register.x0, 0 }, new ulong[] { (byte)Register.PC, 0x28 })]
        //0c: 00008067  jalr   x0,x1,0
        //48: fc1ff06f  jal    x0,8     J-Type
        //34: 00279793  slli   a5,a5,0x2
        //14: 00068613  addi   a2,a3,0
        //18: 00070793  addi   a5,a4,0
        //28: fff78793  addi   a5,a5,-1
        //2c: ffc60613  addi   a2,a2,-4
        //40: 00170713  addi   a4,a4,1
        //44: 00468693  addi   a3,a3,4
        //38: 00f507b3  add    a5,a0,a5
        public void instructionTest(
            ulong startPC, 
            string inst, 
            ulong code, 
            INST_TYPE instT, 
            ulong[] registerPrep, 
            ulong[] expectation
        ){
            byte[] code_bin = Kore.Utility.Misc.fromDWord(code);
            ram.store(code_bin, 0, (ulong)code_bin.Length, startPC);

//             for (int i = 0; i < ramPrepAddr.Length; i++)
//             {
//                 ram.store(ramPrepData[i], 0, (uint)ramPrepData[i].Length, ramPrepAddr[i]);
//                 for (uint m = 0; m < ramPrepData[m].Length; m++)
//                 {
//                     Assert.AreEqual(ramPrepData[i][m], ram.getByte(ramPrepAddr[i] + m));
//                 }
//             }

            for (int i = 0; i < registerPrep.Length / 2; i++)
            {
                Register reg = (Register)registerPrep[i * 2];
                cpu.registers.setR((Register)reg, registerPrep[i * 2 + 1]);
                Assert.AreEqual(registerPrep[i * 2 + 1], cpu.registers.getR((Register)reg));
            }

            cpu.registers.setR(Register.PC, startPC);
            Assert.AreEqual(startPC, cpu.registers.getR(Register.PC));

            Assert.AreEqual(Kore.CPU.Cycle.Off, cpu.state);
            cpu.turnOn();
            Assert.AreEqual(Kore.CPU.Cycle.Init, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Fetch, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Decode, cpu.state);
            bus.tick();
            Assert.AreEqual(instT, cpu.currentInstructionType);
            Assert.AreEqual(Kore.CPU.Cycle.Read, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Exec, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.Write, cpu.state);
            bus.tick();
            Assert.AreEqual(Kore.CPU.Cycle.MovPC, cpu.state);
            bus.tick();

            for (int i = 0; i < expectation.Length / 2; i++)
            {
                Register reg = (Register)expectation[i * 2];
                Assert.AreEqual(expectation[i * 2 + 1], cpu.registers.getR((Register)reg), "Register "+reg+" is not set to the correct value");
            }

            //             for (int i = 0; i < expectationRam.Length / 2; i++)
            //             {
            //                 Register reg = (Register)expectationRam[i * 2];
            //                 Assert.AreEqual(expectationRam[i * 2 + 1], cpu.registers.getR((Register)reg));
            //             }
        }

        [Test]
        public void lw()
        {

            //[TestCase(0x00ul, "00:2 0xffc62883  lw       a7,-4(a2)", 0xffc62883UL, INST_TYPE.IType, new ulong[] { (byte)Register.a7, 4*6, (byte)Register.a2, 4*12 }, new ulong[] { (byte)Register.PC, 0x04 }, new ulong[] { 0xffff_ffff }, new byte[][] { { 0 } }, new ulong[] { })]

            //10: 0006a803  lw     a6,0(a3)
            ram.store(new byte[] { 0xff, 0xff, 0xff, 0xff }, 0, 4, 48); // 4 * 12 = 48
            instructionTest(0x00ul, "", 0x0006a803, INST_TYPE.IType,
                new ulong[] { (byte)Register.a6, 4 * 6, (byte)Register.a3, 48 }, // 4 * 12 = 48
                new ulong[] { (byte)Register.PC, 0x04, (byte)Register.a6, 0xffffffff_fffffffful });

            this.Setup();
            //1c: ffc62883  lw     a7,-4(a2)
            ram.store(new byte[] { 0xff, 0xff, 0xff, 0xff }, 0, 4, 48 - 4); // 4 * 12 = 48
            instructionTest(0x00ul, "", 0b111111111100_01100_010_10001_0000011UL, INST_TYPE.IType,
                new ulong[] { (byte)Register.a7, 4 * 6, (byte)Register.a2, 48 }, // 4 * 12 = 48
                new ulong[] { (byte)Register.PC, 0x04, (byte)Register.a7, 0xffffffff_fffffffful });

            this.Setup();
            //10: 0006a803  lw     a6,0(a3)
            ram.store(new byte[] { 0xFF, 0xAA, 0x77, 0x33 }, 0, 4, 4 * 16);
            instructionTest(0x00ul, "", 0x0006a803UL, INST_TYPE.IType,
                new ulong[] { (byte)Register.a6, 4 * 7, (byte)Register.a3, 4 * 16 },
                new ulong[] { (byte)Register.PC, 0x04, (byte)Register.a6, 0x3377aaff });
        }

        [Test]
        public void sw()
        {
            // Stores the four least-significant bytes in register rs2 to memory at address rs1 + sign-extend(offset)
            // sw rs2,offset(rs1)
            //24: 01162023  sw     a7,0(a2)
            instructionTest(0x04ul, "", 0x7FFF_FFFF_0116_2023UL, INST_TYPE.SType,
                new ulong[] { (byte)Register.a7, 0x23201601, (byte)Register.a2, 4 * 12 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 1));
            Assert.AreEqual(0x01, ram.getByte(4 * 12 + 0));
            Assert.AreEqual(0x16, ram.getByte(4 * 12 + 1));
            Assert.AreEqual(0x20, ram.getByte(4 * 12 + 2));
            Assert.AreEqual(0x23, ram.getByte(4 * 12 + 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 5));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 6));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 7));

            this.Setup();
            //3c: 0107a023  sw     a6,0(a5)
            instructionTest(0x04ul, "", 0x7FFF_FFFF_0107_A023UL, INST_TYPE.SType,
                new ulong[] { (byte)Register.a6, 0x0107A023, (byte)Register.a5, 4 * 16 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 1));
            Assert.AreEqual(0x23, ram.getByte(4 * 16 + 0));
            Assert.AreEqual(0xA0, ram.getByte(4 * 16 + 1));
            Assert.AreEqual(0x07, ram.getByte(4 * 16 + 2));
            Assert.AreEqual(0x01, ram.getByte(4 * 16 + 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 5));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 6));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 7));

            this.Setup();
            //04: ????????  sw     t1,f0(a3)
            instructionTest(0x04ul, "", 
                new SType() { opcode = OPCODE.B32_STORE_S, func3=(byte)FUNC3_MEMORY.WORD, rs2 = Register.t1, imm = 0xf0, rs1=Register.a3}.Encode(),
                INST_TYPE.SType,
                new ulong[] { (byte)Register.t1, 0xA4F25174, (byte)Register.a3, 4 * 16 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 4 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 3 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 2 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 - 1 + 0xf0));
            Assert.AreEqual(0x74, ram.getByte(4 * 16 + 0 + 0xf0));
            Assert.AreEqual(0x51, ram.getByte(4 * 16 + 1 + 0xf0));
            Assert.AreEqual(0xF2, ram.getByte(4 * 16 + 2 + 0xf0));
            Assert.AreEqual(0xA4, ram.getByte(4 * 16 + 3 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 4 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 5 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 6 + 0xf0));
            Assert.AreEqual(0x00, ram.getByte(4 * 16 + 7 + 0xf0));
        }

        [Test, Ignore("test not coded")]
        public void sh()
        {
            //TODO: change this to an instruction built from S Type
            // Stores the two least-significant bytes in register rs2 to memory at address rs1 + sign-extend(offset)
            // sh rs2,offset(rs1)

            //24: 01162023  sw     a7,0(a2)
            instructionTest(0x04ul, "", 0x7FFF_FFFF_0116_2023UL, INST_TYPE.SType,
                new ulong[] { (byte)Register.a7, 4 * 6, (byte)Register.a2, 4 * 12 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 1));
            Assert.AreEqual(0x23, ram.getByte(4 * 12 + 0));
            Assert.AreEqual(0x20, ram.getByte(4 * 12 + 1));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 5));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 6));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 7));

            //3c: 0107a023  sw     a6,0(a5)
            instructionTest(0x04ul, "", 0x7FFF_FFFF_0107_A023UL, INST_TYPE.SType,
                new ulong[] { (byte)Register.a6, 4 * 7, (byte)Register.a2, 4 * 16 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 1));
            Assert.AreEqual(0x23, ram.getByte(4 * 12 + 0));
            Assert.AreEqual(0xA0, ram.getByte(4 * 12 + 1));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 5));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 6));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 7));
        }

        [Test, Ignore("test not coded")]
        public void sd()
        {
            //TODO: change this to an instruction built from S Type
            // Stores the all 8 bytes in register rs2 to memory at address rs1 + sign-extend(offset)
            // sd rs2,offset(rs1)

            //24: 01162023  sw     a7,0(a2)
            instructionTest(0x04ul, "", 0x7FDD_AA99_0116_2023UL, INST_TYPE.SType,
                new ulong[] { (byte)Register.a7, 4 * 6, (byte)Register.a2, 4 * 12 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 1));
            Assert.AreEqual(0x23, ram.getByte(4 * 12 + 0));
            Assert.AreEqual(0x20, ram.getByte(4 * 12 + 1));
            Assert.AreEqual(0x16, ram.getByte(4 * 12 + 2));
            Assert.AreEqual(0x01, ram.getByte(4 * 12 + 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 5));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 6));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 7));

            //3c: 0107a023  sw     a6,0(a5)
            instructionTest(0x04ul, "", 0x7FFF_FFFF_0107_A023UL, INST_TYPE.SType,
                new ulong[] { (byte)Register.a6, 4 * 7, (byte)Register.a2, 4 * 16 },
                new ulong[] { (byte)Register.PC, 0x08 });

            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 2));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 - 1));
            Assert.AreEqual(0x23, ram.getByte(4 * 12 + 0));
            Assert.AreEqual(0xA0, ram.getByte(4 * 12 + 1));
            Assert.AreEqual(0x07, ram.getByte(4 * 12 + 2));
            Assert.AreEqual(0x01, ram.getByte(4 * 12 + 3));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 4));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 5));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 6));
            Assert.AreEqual(0x00, ram.getByte(4 * 12 + 7));
        }

        [Test]
        public void nop()
        {
            // Expands to `addi x0, x0, 0`
            IType inst = new IType();
            inst.opcode = OPCODE.B32_ADDI;
            inst.func3 = 0b111;
            inst.rs1 = Register.x0;
            inst.rd = Register.x0;
            inst.imm = 0;

            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                uint r = (uint) rand.Next(0, 200);
                r *= 4;

                this.Setup();
                instructionTest(r, "addi x0,x0,0", inst.Encode(), INST_TYPE.IType, new ulong[] { }, new ulong[] {
                    (byte)Register.x0, 0,
                    (byte)Register.x1, 0,
                    (byte)Register.x2, cpu.MemorySize - 1,
                    (byte)Register.x3, 0,
                    (byte)Register.x4, 0,
                    (byte)Register.x5, 0,
                    (byte)Register.x6, 0,
                    (byte)Register.x7, 0,
                    (byte)Register.x8, 0,
                    (byte)Register.x9, 0,
                    (byte)Register.x10, 0,
                    (byte)Register.x11, 0,
                    (byte)Register.x12, 0,
                    (byte)Register.x13, 0,
                    (byte)Register.x14, 0,
                    (byte)Register.x15, 0,
                    (byte)Register.x16, 0,
                    (byte)Register.x17, 0,
                    (byte)Register.x18, 0,
                    (byte)Register.x19, 0,
                    (byte)Register.x20, 0,
                    (byte)Register.x21, 0,
                    (byte)Register.x22, 0,
                    (byte)Register.x23, 0,
                    (byte)Register.x24, 0,
                    (byte)Register.x25, 0,
                    (byte)Register.x26, 0,
                    (byte)Register.x27, 0,
                    (byte)Register.x28, 0,
                    (byte)Register.x29, 0,
                    (byte)Register.x30, 0,
                    (byte)Register.x31, 0,
                    (byte)Register.PC, (r + 0x4U)
                });
            }
        }

        [Test]
        public void bgeu()
        {
            // No reference so uses BType
            BType inst = new BType();
            inst.opcode = OPCODE.B32_BRANCH;
            inst.func3 = 0b111;
            inst.rs1 = Register.t0;
            inst.rs2 = Register.t1;
            inst.imm = 0x08;

            // >=
            instructionTest(0x08ul, "", inst.Encode(), INST_TYPE.BType, new ulong[] { (byte)Register.t0, 0, (byte)Register.t1, 0 }, new ulong[] { (byte)Register.PC, 0x10 });

            this.Setup();
            instructionTest(0x08ul, "", inst.Encode(), INST_TYPE.BType, new ulong[] { (byte)Register.t0, 5, (byte)Register.t1, 6 }, new ulong[] { (byte)Register.PC, 0x0C });

            this.Setup();
            instructionTest(0x08ul, "", inst.Encode(), INST_TYPE.BType, new ulong[] { (byte)Register.t0, 5, (byte)Register.t1, 5 }, new ulong[] { (byte)Register.PC, 0x10 });

            this.Setup();
            instructionTest(0x08ul, "", inst.Encode(), INST_TYPE.BType, new ulong[] { (byte)Register.t0, 5, (byte)Register.t1, 4 }, new ulong[] { (byte)Register.PC, 0x10 });

            this.Setup();
            instructionTest(0x08ul, "", inst.Encode(), INST_TYPE.BType, new ulong[] { (byte)Register.t0, 5, (byte)Register.t1, 0xFFFF_FF00 }, new ulong[] { (byte)Register.PC, 0x0C });

            this.Setup();
            instructionTest(0x08ul, "", inst.Encode(), INST_TYPE.BType, new ulong[] { (byte)Register.t0, 0xFFFF_FF00, (byte)Register.t1, 5 }, new ulong[] { (byte)Register.PC, 0x10 });
        }
    }
}
