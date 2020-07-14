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
        public void Instruction_I_Type_Struct()
        {
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

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.ADDI, inst.opcode);
            Assert.AreEqual(Register.x29, inst.rd);
            Assert.AreEqual(0b000, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(0b00000000_0101, inst.imm);

            inst.Decode(addi02);
            Assert.AreEqual(addi02, inst.Encode());

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.ADDI, inst.opcode);
            Assert.AreEqual(Register.x30, inst.rd);
            Assert.AreEqual(0b000, inst.func3);
            Assert.AreEqual(Register.x0, inst.rs1);
            Assert.AreEqual(0b00000010_0101, inst.imm);

        }

        [Test]
        public void Instruction_R_Type_Struct()
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

            Assert.AreEqual(Kore.RiscISA.Instruction.OPCODE.ADD, inst.opcode);
            Assert.AreEqual(Register.x31, inst.rd);
            Assert.AreEqual(0b000, inst.func3);
            Assert.AreEqual(Register.x30, inst.rs1);
            Assert.AreEqual(Register.x29, inst.rs2);
        }

    }

}
