//#define FULLTEST
using Kore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Kore.RiscISA;
using Kore.RiscISA.Instruction;

namespace KoreTests
{
    public class RiscISATests
    {
        public Random rand = new Random();
        // from_opcode
        [TestCase("opcode", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_01111111u, // Shifted
            0b00000000_00000000_00000000_01111111u //  Unshifted
        })]
        // from_rd
        [TestCase("rd", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_00011111u, // Shifted
            0b00000000_00000000_00001111_10000000u //  Unshifted
        })]
        // from_imm_4_0
        [TestCase("imm_4_0", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_00011111u, // Shifted
            0b00000000_00000000_00001111_10000000u //  Unshifted
        })]
        // from_func3
        [TestCase("func3", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_00000111u, // Shifted
            0b00000000_00000000_01110000_00000000u //  Unshifted
        })]
        // from_rs1
        [TestCase("rs1", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_00011111u, // Shifted
            0b00000000_00001111_10000000_00000000u //  Unshifted
        })]
        // from_rs2
        [TestCase("rs2", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_00011111u, // Shifted
            0b00000001_11110000_00000000_00000000u //  Unshifted
        })]
        // from_func7
        [TestCase("func7", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_01111111u, // Shifted
            0b11111110_00000000_00000000_00000000u //  Unshifted
        })]
        // from_imm_11_0
        [TestCase("imm_11_0", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00001111_11111111u, // Shifted
            0b11111111_11110000_00000000_00000000u //  Unshifted
        })]
        // from_imm_11_5
        [TestCase("imm_11_5", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_01111111u, // Shifted
            0b11111110_00000000_00000000_00000000u //  Unshifted
        })]
        // from_imm_12_10_5
        [TestCase("imm_12_10_5", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00000000_00000000_01111111u, // Shifted
            0b11111110_00000000_00000000_00000000u //  Unshifted
        })]
        // from_imm_31_12
        [TestCase("imm_31_12", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00001111_11111111_11111111u, // Shifted
            0b11111111_11111111_11110000_00000000u //  Unshifted
        })]
        // from_imm_20_10_1_11_19_12
        [TestCase("imm_20_10_1_11_19_12", new ulong[]{
            0b11111111_11111111_11111111_11111111u, // Src
            0b00000000_00001111_11111111_11111111u, // Shifted
            0b11111111_11111111_11110000_00000000u //  Unshifted
        })]
        public void testConversions(string name, ulong[] data)
        {
            ulong testa = data[0]; // Src
            ulong testb = data[1]; // Shifted
            ulong testc = data[2]; // Unshifted
            ulong testd = data[2]; // Unshifted
            Assert.AreEqual(testb, to(name, testa, true));
            Assert.AreEqual(testc, to(name, testa, false));
            Assert.AreEqual(testd, from(name, testb));

            int finalTest = rand.Next(0x0, (int) testb);
            ulong finalConvert = from(name, (ulong)finalTest);
            ulong finalResult = to(name, finalConvert, true);
            Assert.AreEqual(finalTest, finalResult);
        }

        protected ulong to(string name, ulong data, bool shift)
        {
            switch (name)
            {
                case "opcode":
                    return Transcoder.to_opcode(data, shift);
                case "rd":
                    return Transcoder.to_rd(data, shift);
                case "imm_4_0":
                    return Transcoder.to_imm_4_0(data, shift);
                case "func3":
                    return Transcoder.to_func3(data, shift);
                case "rs1":
                    return Transcoder.to_rs1(data, shift);
                case "rs2":
                    return Transcoder.to_rs2(data, shift);
                case "func7":
                    return Transcoder.to_func7(data, shift);
                case "imm_11_0":
                    return Transcoder.to_imm_11_0(data, shift);
                case "imm_11_5":
                    return Transcoder.to_imm_11_5(data, shift);
                case "imm_12_10_5":
                    return Transcoder.to_imm_12_10_5(data, shift);
                case "imm_31_12":
                    return Transcoder.to_imm_31_12(data, shift);
                case "imm_20_10_1_11_19_12":
                    return Transcoder.to_imm_20_10_1_11_19_12(data, shift);
                default:
                    return 0;
            }
        }

        protected ulong from(string name, ulong data)
        {
            switch (name)
            {
                case "opcode":
                    return Transcoder.from_opcode((byte)data);
                case "rd":
                    return Transcoder.from_rd((byte) data);
                case "imm_4_0":
                    return Transcoder.from_imm_4_0((byte) data);
                case "func3":
                    return Transcoder.from_func3((byte) data);
                case "rs1":
                    return Transcoder.from_rs1((byte) data);
                case "rs2":
                    return Transcoder.from_rs2((byte) data);
                case "func7":
                    return Transcoder.from_func7((byte) data);
                case "imm_11_0":
                    return Transcoder.from_imm_11_0((uint) data);
                case "imm_11_5":
                    return Transcoder.from_imm_11_5((byte) data);
                case "imm_12_10_5":
                    return Transcoder.from_imm_12_10_5((byte) data);
                case "imm_31_12":
                    return Transcoder.from_imm_31_12((uint) data);
                case "imm_20_10_1_11_19_12":
                    return Transcoder.from_imm_20_10_1_11_19_12((uint) data);
                default:
                    return 0;
            }
        }

        [Test]
        public void Instruction_I_Type_Struct_Precomp()
        {
            //1c: ffc62883  lw     a7,-4(a2)
            //10: 0006a803  lw     a6,0(a3)

            // .  addi x29, x0, 5   // Add 5 and 0, and store the value to x29.
            // .  addi x30, x0, 37  // Add 37 and 0, and store the value to x30.
            //                                00    01    02    03  
            byte[] addi01_bin = new byte[] { 0x93, 0x0E, 0x50, 0x00 };
            byte[] addi02_bin = new byte[] { 0x13, 0x0F, 0x50, 0x02 };
            ulong addi01 = Kore.Utility.Misc.toDWords(addi01_bin)[0]; // 0b0000_0000_0000_0 101_00 00_0 000_00 10_1001
            ulong addi02 = Kore.Utility.Misc.toDWords(addi02_bin)[0]; // 0b0000_0010_0101_0 000_00 00_1 111_00 01_0011

            //              0b0000000000000
            Assert.AreEqual(0b00000000_01010000_00001110_10010011, addi01);
            Assert.AreEqual(0b00000010_01010000_00001111_00010011, addi02);

            Kore.RiscISA.Instruction.IType inst = new Kore.RiscISA.Instruction.IType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(Register.x0, inst.rd);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(0, inst.imm);

            inst.Decode(addi01);
            Assert.AreEqual(addi01, inst.Encode());

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_ADDI, inst.opcode);
            Assert.AreEqual(Register.x29, inst.rd);
            Assert.AreEqual(0b000, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(0b00000000_0101, inst.imm);

            inst.Decode(addi02);
            Assert.AreEqual(addi02, inst.Encode());

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_ADDI, inst.opcode);
            Assert.AreEqual(Register.x30, inst.rd);
            Assert.AreEqual(0b000, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(0b00000010_0101, inst.imm);

        }

        [Test]
        public void Instruction_I_Type_Struct_FullRange()
        {
            Kore.RiscISA.Instruction.IType inst = new Kore.RiscISA.Instruction.IType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rd);
            Assert.AreEqual(Register.x0, inst.rs1);

            Random rand = new Random();
            bool wasRun = false;
#if FULLTEST
            for (Register rd = 0; rd <= Register.x31; rd++)
            {
                for (Register rs1 = 0; rs1 <= Register.x31; rs1++)
                {
#else

            Register rd = (Register)rand.Next(0, 31);
            Register rs1 = (Register)rand.Next(0, 31);
            // byte func3 = (byte)rand.Next(0, 0b111);
#endif
            for (byte func3 = 0; func3 <= 0b111; func3++)
            {
                for (int i = -2048; i < 0b0000_0111_1111_1111; i++)
                {
                    if (wasRun == false) wasRun = true;
                    // Register rs1 = (Register)rand.Next(0, 31);
                    // Register rs2 = (Register)rand.Next(0, 31);
                    // byte func3 = (byte)rand.Next(0, 0b111);

                    inst.opcode = (OPCODE)0b0100011;
                    inst.rd = rd;
                    inst.rs1 = rs1;
                    inst.func3 = func3;
                    inst.imm = i;

                    Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                    Assert.AreEqual(i, inst.imm);
                    Assert.AreEqual(func3, inst.func3);
                    Assert.AreEqual(rd, inst.rd);
                    Assert.AreEqual(rs1, inst.rs1);

                    ulong code = inst.Encode();

                    Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                    Assert.AreEqual(i, inst.imm);
                    Assert.AreEqual(func3, inst.func3);
                    Assert.AreEqual(rd, inst.rd);
                    Assert.AreEqual(rs1, inst.rs1);

                    inst.opcode = Kore.RiscISA.Instruction.OPCODE.unknown00;
                    inst.rd = 0;
                    inst.rs1 = 0;
                    inst.func3 = 0;
                    inst.imm = 0;

                    Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
                    Assert.AreEqual(0x00, inst.imm);
                    Assert.AreEqual(0, inst.func3);
                    Assert.AreEqual(Register.x0, inst.rd);
                    Assert.AreEqual(Register.x0, inst.rs1);

                    inst.Decode(code);

                    Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                    Assert.AreEqual(i, inst.imm);
                    Assert.AreEqual(func3, inst.func3);
                    Assert.AreEqual(rd, inst.rd);
                    Assert.AreEqual(rs1, inst.rs1);
                }
            }
#if FULLTEST
                }
            }
#endif

            if (wasRun == false) Assert.Fail("No tests run for some reason.");
        }

        [Test]
        public void Instruction_R_Type_Struct_Precomp()
        {
            //  add x31, x30, x29 // x31 should contain 42 (0x2a).
            //                             00    01    02    03  
            byte[] add_bin = new byte[] { 0xB3, 0x0F, 0xDF, 0x01 };
            ulong add01 = Kore.Utility.Misc.toDWords(add_bin)[0];

            //              0b0000000000000
            Assert.AreEqual(0b00000001_11011111_00001111_10110011, add01);

            Kore.RiscISA.Instruction.RType inst = new Kore.RiscISA.Instruction.RType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(Register.x0, inst.rd);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(Register.x0, inst.rs2);

            inst.Decode(add01);
            Assert.AreEqual(add01, inst.Encode());

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_ADD, inst.opcode);
            Assert.AreEqual(Register.x31, inst.rd);
            Assert.AreEqual(0b000, inst.func3);
            Assert.AreEqual(Register.x30, inst.rs1);
            Assert.AreEqual(Register.x29, inst.rs2);
        }

        [Test]
        public void Instruction_R_Type_Struct_FullRange()
        {
            
            Kore.RiscISA.Instruction.RType inst = new Kore.RiscISA.Instruction.RType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rd);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(Register.x0, inst.rs2);

            Random rand = new Random();
            bool wasRun = false;
            for (Register rd = 0; rd <= Register.x31; rd++)
            {
                for (Register rs1 = 0; rs1 <= Register.x31; rs1++)
                {
                    for (Register rs2 = 0; rs2 <= Register.x31; rs2++)
                    {
                        for (byte func3 = 0; func3 <= 0b111; func3++)
                        {
                            if (wasRun == false) wasRun = true;
                            // Register rs1 = (Register)rand.Next(0, 31);
                            // Register rs2 = (Register)rand.Next(0, 31);
                            // byte func3 = (byte)rand.Next(0, 0b111);

                            inst.opcode = OPCODE.B32_ADD;
                            inst.rd = rd;
                            inst.rs1 = rs1;
                            inst.rs2 = rs2;
                            inst.func3 = func3;

                            Assert.AreEqual(OPCODE.B32_ADD, inst.opcode);
                            Assert.AreEqual(func3, inst.func3);
                            Assert.AreEqual(rd, inst.rd);
                            Assert.AreEqual(rs1, inst.rs1);
                            Assert.AreEqual(rs2, inst.rs2);

                            ulong code = inst.Encode();

                            Assert.AreEqual(OPCODE.B32_ADD, inst.opcode);
                            Assert.AreEqual(func3, inst.func3);
                            Assert.AreEqual(rd, inst.rd);
                            Assert.AreEqual(rs1, inst.rs1);
                            Assert.AreEqual(rs2, inst.rs2);

                            inst.opcode = Kore.RiscISA.Instruction.OPCODE.unknown00;
                            inst.rd = 0;
                            inst.rs1 = 0;
                            inst.rs2 = 0;
                            inst.func3 = 0;

                            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
                            Assert.AreEqual(0, inst.func3);
                            Assert.AreEqual(Register.x0, inst.rd);
                            Assert.AreEqual(Register.x0, inst.rs1);
                            Assert.AreEqual(Register.x0, inst.rs2);

                            inst.Decode(code);

                            Assert.AreEqual(OPCODE.B32_ADD, inst.opcode);
                            Assert.AreEqual(func3, inst.func3);
                            Assert.AreEqual(rd, inst.rd);
                            Assert.AreEqual(rs1, inst.rs1);
                            Assert.AreEqual(rs2, inst.rs2);
                        }
                    }
                }
            }

            if (wasRun == false) Assert.Fail("No tests run for some reason.");
        }

        [Test]
        public void Instruction_B_Type_Struct_Precomp()
        {
            // 08: 0x00b76463  bltu     a4,a1,0x08  (+0x08)
            // 20: 0x01185a63  bge      a6,a7,0x34  (+0x14)
            // 30: 0xfe0796e3  bne      a5,x0,0x1c  (-0x14)
            ulong bltu = 0x00b76463;
            ulong bge = 0x01185a63;
            ulong bne = 0xfe0796e3;

            Kore.RiscISA.Instruction.BType inst = new Kore.RiscISA.Instruction.BType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(Register.x0, inst.rs2);

            inst.Decode(bltu);

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_BRANCH, inst.opcode);
            Assert.AreEqual(8, inst.imm);
            Assert.AreEqual(0b110, inst.func3);
            Assert.AreEqual(Register.a4, inst.rs1);
            Assert.AreEqual(Register.a1, inst.rs2);

            Assert.AreEqual(bltu, inst.Encode());

            inst.Decode(bge);

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_BRANCH, inst.opcode);
            Assert.AreEqual(0x14, inst.imm);
            Assert.AreEqual(0b101, inst.func3);
            Assert.AreEqual(Register.a6, inst.rs1);
            Assert.AreEqual(Register.a7, inst.rs2);

            Assert.AreEqual(bge, inst.Encode());

            inst.Decode(bne);

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_BRANCH, inst.opcode);
            Assert.AreEqual(-0x14, inst.imm);
            Assert.AreEqual(0b001, inst.func3);
            Assert.AreEqual(Register.a5, inst.rs1);
            Assert.AreEqual(Register.x0, inst.rs2);

            Assert.AreEqual(bne, inst.Encode());
        }

        [Test, Ignore("Needs Code")]
        public void Instruction_B_Type_Struct_FullRange()
        {
        }

        [Test, Ignore("Needs Example")]
        public void Instruction_U_Type_Struct_Precomp()
        {
            // lui
        }

        [Test]
        public void Instruction_U_Type_Struct_FullRange()
        {
            //The lower 12 bits are all not used by the imm so we will be testing multiples of 4,096
            
            Kore.RiscISA.Instruction.UType inst = new Kore.RiscISA.Instruction.UType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(Register.x0, inst.rd);

            Random rand = new Random();
            bool wasRun = false;
#if FULLTEST
            for (Register rd = 0; rd <= Register.x31; rd++)
            {
#else
                Register rd = (Register)rand.Next(0, 31);
#endif
                bool secondHalf = false;
                for (int ii = 0; ii <= 0b00000000_00001111_11111111_11111111; ii++) // By 2 because the imm[0] is not used
                {
                    int i = ii << 12;
                    if (wasRun == false) wasRun = true;
                    if (i < 0 && secondHalf)
                    {
                        // Has looped over onto itself
                        // So drop out and do all 1s test
                        break;
                    }
                    else if(i > 0 && secondHalf == false)
                    {
                        secondHalf = true;
                    }
                    // Register rs1 = (Register)rand.Next(0, 31);
                    // Register rs2 = (Register)rand.Next(0, 31);
                    // byte func3 = (byte)rand.Next(0, 0b111);

                    inst.opcode = (OPCODE)0b0100011;
                    inst.rd = rd;
                    inst.imm = i;

                    Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                    Assert.AreEqual(i, inst.imm);
                    Assert.AreEqual(rd, inst.rd);

                    ulong code = inst.Encode();

                    Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                    Assert.AreEqual(i, inst.imm);
                    Assert.AreEqual(rd, inst.rd);

                    inst.opcode = Kore.RiscISA.Instruction.OPCODE.unknown00;
                    inst.rd = 0;
                    inst.imm = 0;

                    Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
                    Assert.AreEqual(0x00, inst.imm);
                    Assert.AreEqual(Register.x0, inst.rd);

                    inst.Decode(code);

                    Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                    Assert.AreEqual(rd, inst.rd);
                    Assert.AreEqual(i, inst.imm);
                }

                // Just run it all one more time cause I am too lazy to do the math

//                 int last = (int)0b01111111_11111111_11110000_00000000u;
//                 inst.opcode = (OPCODE)0b0100011;
//                 inst.rd = rd;
//                 inst.imm = last;
// 
//                 Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
//                 Assert.AreEqual(last, inst.imm);
//                 Assert.AreEqual(rd, inst.rd);
// 
//                 ulong code2 = inst.Encode();
// 
//                 Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
//                 Assert.AreEqual(last, inst.imm);
//                 Assert.AreEqual(rd, inst.rd);
// 
//                 inst.opcode = Kore.RiscISA.Instruction.OPCODE.unknown00;
//                 inst.rd = 0;
//                 inst.imm = 0;
// 
//                 Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
//                 Assert.AreEqual(0x00, inst.imm);
//                 Assert.AreEqual(Register.x0, inst.rd);
// 
//                 inst.Decode(code2);
// 
//                 Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
//                 Assert.AreEqual(rd, inst.rd);
//                 Assert.AreEqual(last, inst.imm);
#if FULLTEST
            }
#endif

            if (wasRun == false) Assert.Fail("No tests run for some reason.");

        }

        [Test, Ignore("Needs Example")]
        public void Instruction_J_Type_Struct_Precomp()
        {
        }

        [Test]
        public void Instruction_J_Type_Struct_FullRange()
        {
            Kore.RiscISA.Instruction.JType inst = new Kore.RiscISA.Instruction.JType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(Register.x0, inst.rd);

            Random rand = new Random();
            bool wasRun = false;
#if FULLTEST
            for (Register rd = 0; rd <= Register.x31; rd++)
            {
#else

                Register rd = (Register)rand.Next(0, 31);
#endif
                for (byte func3 = 0; func3 <= 0b111; func3++)
                {
                    for (int i = -2048; i < 0b0000_0111_1111_1111; i+=2) // By 2 because the imm[0] is not used
                    {
                        if (wasRun == false) wasRun = true;
                        // Register rs1 = (Register)rand.Next(0, 31);
                        // Register rs2 = (Register)rand.Next(0, 31);
                        // byte func3 = (byte)rand.Next(0, 0b111);

                        inst.opcode = (OPCODE)0b0100011;
                        inst.rd = rd;
                        inst.imm = i;

                        Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                        Assert.AreEqual(i, inst.imm);
                        Assert.AreEqual(rd, inst.rd);

                        ulong code = inst.Encode();

                        Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                        Assert.AreEqual(i, inst.imm);
                        Assert.AreEqual(rd, inst.rd);

                        inst.opcode = Kore.RiscISA.Instruction.OPCODE.unknown00;
                        inst.rd = 0;
                        inst.imm = 0;

                        Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
                        Assert.AreEqual(0x00, inst.imm);
                        Assert.AreEqual(Register.x0, inst.rd);

                        inst.Decode(code);

                        Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                        Assert.AreEqual(rd, inst.rd);
                        Assert.AreEqual(i, inst.imm);
                    }
                }
#if FULLTEST
            }
#endif

            if (wasRun == false) Assert.Fail("No tests run for some reason.");
        }

        [Test]
        public void Instruction_S_Type_Struct_Precomp()
        {
            //24: 01162023  sw     a7,0(a2) 0b0000000_10001_01100_010_00000_0100011
            //3c: 0107a023  sw     a6,0(a5) 0b0000000_10000_01111_010_00000_0100011
            ulong sw1 = 0b0000000_10001_01100_010_00000_0100011;
            ulong sw2 = 0b0000000_10000_01111_010_00000_0100011;

            Kore.RiscISA.Instruction.SType inst = new Kore.RiscISA.Instruction.SType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(Register.x0, inst.rs2);

            inst.Decode(sw1);

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_STORE, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(0b010, inst.func3);
            Assert.AreEqual(Register.a2, inst.rs1);
            Assert.AreEqual(Register.a7, inst.rs2);

            Assert.AreEqual(sw1, inst.Encode());

            inst.Decode(sw2);

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.B32_STORE, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(0b010, inst.func3);
            Assert.AreEqual(Register.a5, inst.rs1);
            Assert.AreEqual(Register.a6, inst.rs2);

            Assert.AreEqual(sw2, inst.Encode());
        }

        [Test]
        public void Instruction_S_Type_Struct_FullRange()
        {
            Kore.RiscISA.Instruction.SType inst = new Kore.RiscISA.Instruction.SType();

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
            Assert.AreEqual(0x00, inst.imm);
            Assert.AreEqual(0, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(Register.x0, inst.rs2);

            Random rand = new Random();
            bool wasRun = false;
#if FULLTEST
            for (Register rs1 = 0; rs1 <= Register.x31; rs1++)
            {
                for (Register rs2 = 0; rs2 <= Register.x31; rs2++)
                {
#else

                    Register rs1 = (Register)rand.Next(0, 31);
                    Register rs2 = (Register)rand.Next(0, 31);
                    // byte func3 = (byte)rand.Next(0, 0b111);
#endif
                    for (byte func3 = 0; func3 <= 0b111; func3++)
                    {
                        for (int i = -2048; i < 0b0000_0111_1111_1111; i++)
                        {
                            if (wasRun == false) wasRun = true;
                            // Register rs1 = (Register)rand.Next(0, 31);
                            // Register rs2 = (Register)rand.Next(0, 31);
                            // byte func3 = (byte)rand.Next(0, 0b111);

                            inst.opcode = (OPCODE)0b0100011;
                            inst.rs1 = rs1;
                            inst.rs2 = rs2;
                            inst.func3 = func3;
                            inst.imm = i;

                            Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                            Assert.AreEqual(i, inst.imm);
                            Assert.AreEqual(func3, inst.func3);
                            Assert.AreEqual(rs1, inst.rs1);
                            Assert.AreEqual(rs2, inst.rs2);

                            ulong code = inst.Encode();

                            Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                            Assert.AreEqual(i, inst.imm);
                            Assert.AreEqual(func3, inst.func3);
                            Assert.AreEqual(rs1, inst.rs1);
                            Assert.AreEqual(rs2, inst.rs2);

                            inst.opcode = Kore.RiscISA.Instruction.OPCODE.unknown00;
                            inst.rs1 = 0;
                            inst.rs2 = 0;
                            inst.func3 = 0;
                            inst.imm = 0;

                            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.unknown00, inst.opcode);
                            Assert.AreEqual(0x00, inst.imm);
                            Assert.AreEqual(0, inst.func3);
                            Assert.AreEqual(Register.x0, inst.rs1);
                            Assert.AreEqual(Register.x0, inst.rs2);

                            inst.Decode(code);

                            Assert.AreEqual((OPCODE)0b0100011, inst.opcode);
                            Assert.AreEqual(i, inst.imm);
                            Assert.AreEqual(func3, inst.func3);
                            Assert.AreEqual(rs1, inst.rs1);
                            Assert.AreEqual(rs2, inst.rs2);
                        }
                    }
#if FULLTEST
                }
            }
#endif

            if (wasRun == false) Assert.Fail("No tests run for some reason.");
        }
    }
}
