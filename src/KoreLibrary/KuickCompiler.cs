using Kore.RiscISA;
using Kore.RiscISA.Instruction;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kore
{
    public static class KuickCompiler {
        // This regex is built with static read only strings because its basicly free because of how the C# compiler handels it and even if not for that it would only be done once.
        public static class Regexpression
        {
            public static readonly string RegisterPC = "pc";
            public static readonly string RegisterX = "x[0123]{0,1}\\d";
            public static readonly string RegisterZ = "zero";
            public static readonly string RegisterR = "ra";
            public static readonly string RegisterG = "gp";
            public static readonly string RegisterF = "fp";
            public static readonly string RegisterT = "t[p0123456]";
            public static readonly string RegisterS = "s(\\d|10|11|p)";
            public static readonly string RegisterA = "a[01]{0,1}\\d";
            public static readonly string Register = "(" + RegisterX + "|"
                                                              + RegisterZ + "|"
                                                              + RegisterR + "|"
                                                              + RegisterG + "|"
                                                              + RegisterF + "|"
                                                              + RegisterT + "|"
                                                              + RegisterS + "|"
                                                              + RegisterA + ")";
            public static readonly string SpaceSingle = "\\s";
            public static readonly string SpaceAnyOrNone = SpaceSingle+"*";
            public static readonly string SpaceOneOrMore = SpaceSingle + "+";
            public static readonly string ImmidiateValue = "\\-{0,1}[0-9a-f]+|\\-{0,1}0x[0-9a-f]+";
            public static readonly string DataRegReg = "(?<rr1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rr2>" + Register + ")";
            public static readonly string DataRegRegReg = "(?<rrr1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rrr2>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rrr3>" + Register + ")";
            public static readonly string DataRegImm = "(?<rir>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rii>" + ImmidiateValue + ")";
            public static readonly string DataRegRegImm = "(?<rri_rd>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rri_r1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rri_imm>" + ImmidiateValue + ")";
            public static readonly string DataRegOffsetReg = "(?<ror_r2>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<ror_imm>" + ImmidiateValue + ")\\((?<ror_r1>" + Register + ")\\)";
            public static readonly string DataOffsetReg = "(?<or_imm>" + ImmidiateValue + ")\\((?<or_rs1>" + Register + ")\\)";
            public static readonly string DataCombined = DataRegReg + "|" + DataRegRegReg + "|" + DataRegImm + "|" + DataRegRegImm + "|" + DataRegOffsetReg;
            public static readonly string CommentOptional = "(" + SpaceAnyOrNone + "#" + SpaceAnyOrNone + "(?<comment>.+){0,1})?(\n|$)";
            public static readonly string OpName = "(?<op>[a-z.]+)";
            public static readonly string expr = SpaceAnyOrNone+ OpName + "("+ SpaceOneOrMore + "(?<data>"+ DataCombined + ")){0,1}" + SpaceAnyOrNone + CommentOptional;
            public static readonly Regex regex = new Regex(expr);
            public static class g
            {
                public static readonly int op = regex.GroupNumberFromName("op");
                public static readonly int rrr_rd = regex.GroupNumberFromName("rrr1");
                public static readonly int rrr_r1 = regex.GroupNumberFromName("rrr2");
                public static readonly int rrr_r2 = regex.GroupNumberFromName("rrr3");
                /// <summary>
                /// OP rs, rs, imm<br/>
                /// OP __, rs, imm<br/>
                /// op rri_rd, rri_r1, rri_imm 
                /// </summary>
                public static readonly int rri_rd = regex.GroupNumberFromName("rri_rd");
                /// <summary>
                /// OP rs, rs, imm<br/>
                /// OP rs, __, imm<br/>
                /// op rri_rd, rri_r1, rri_imm 
                /// </summary>
                public static readonly int rri_r1 = regex.GroupNumberFromName("rri_r1");
                /// <summary>
                /// OP rs, rs, imm<br/>
                /// OP rs, rs, ___<br/>
                /// op rri_rd, rri_r1, rri_imm 
                /// </summary>
                public static readonly int rri_imm = regex.GroupNumberFromName("rri_imm");
                public static readonly int ror_r1 = regex.GroupNumberFromName("ror_r1");
                public static readonly int ror_r2 = regex.GroupNumberFromName("ror_r2");
                public static readonly int ror_imm = regex.GroupNumberFromName("ror_imm");
                public static readonly int rir = regex.GroupNumberFromName("rir");
                public static readonly int rii = regex.GroupNumberFromName("rii");
                public static readonly int rr1 = regex.GroupNumberFromName("rr1");
                public static readonly int rr2 = regex.GroupNumberFromName("rr2");
            }
            public static Match test(string text)
            {
                // throw new Exception("Test Expression: "+expr); // For debugging expression
                return regex.Match(text);
            }
        }

        enum instructionSyntax
        {
            /// <summary>
            /// OP rd, rs1
            /// </summary>
            rr,
            /// <summary>
            /// OP rd, rs1, rs2
            /// </summary>
            rrr,
            /// <summary>
            ///  OP r2, imm
            /// </summary>
            ri,
            /// <summary>
            /// OP rs, rs1, imm
            /// </summary>
            rri,
            /// <summary>
            /// OP rs2, imm(rs1)
            /// </summary>
            ror,
            /// <summary>
            /// OP imm(rs1)
            /// </summary>
            or
        }

        private static RType r = new RType();
        private static IType i = new IType();
        private static SType s = new SType();
        private static BType b = new BType();
        private static UType u = new UType();
        private static JType j = new JType();
        public static uint assembleSingleLine(ulong opStartByte, string asm)
        {
            Match match = Regexpression.test(asm.ToLower());
            if(match.Success == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could not understand the input of `" + asm + "` as a line of Risc-V ASM"));
            RiscISA.Instruction.TYPE_OF_INST instStruct = RiscISA.Instruction.TYPE_OF_INST.add;
            string op = match.Groups[Regexpression.g.op].Value;
            //Register reg = Register.x0;
            uint output = 0;
            bool needs_rd = true;
            bool needs_rs1 = true;
            bool needs_rs2 = true;
            bool needs_imm = true;
            instructionSyntax syntax = instructionSyntax.or;
            if (Enum.TryParse(op, out instStruct)){
                switch ((RiscISA.Instruction.INST_TYPE)(byte)instStruct)
                {
                    case INST_TYPE.RType:
                        syntax = instructionSyntax.rrr;
                        r.opcode = OPCODE.B32_OP;
                        r.func7 = 0b0000000;
                        if (Enum.TryParse(match.Groups[Regexpression.g.rrr_rd].Value, out r.rd) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rd` of `" + match.Groups[Regexpression.g.rrr_rd].Value + "` in the RType partern `OP rd, rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (Enum.TryParse(match.Groups[Regexpression.g.rrr_r1].Value, out r.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.rrr_r1].Value + "` in the RType partern `OP rd, rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (Enum.TryParse(match.Groups[Regexpression.g.rrr_r2].Value, out r.rs2) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs2` of `" + match.Groups[Regexpression.g.rrr_r2].Value + "` in the RType partern `OP rd, rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                        switch (op)
                        {
                            case "add": // add rd, rs1, rs2
                                r.func3 = 0b000;
                                break;
                            case "sub": // sub rd, rs1, rs2 
                                r.func3 = 0b000;
                                r.func7 = 0b0100000;
                                break;
                            case "sll": // sll rd, rs1, rs2
                                r.func3 = 0b001;
                                break;
                            case "slt": // slt rd, rs1, rs2
                                r.func3 = 0b010;
                                break;
                            case "sltu": // sltu rd, rs1, rs2
                                r.func3 = 0b011;
                                break;
                            case "xor": // xor rd, rs1, rs2
                                r.func3 = 0b100;
                                break;
                            case "srl": // srl rd, rs1, rs2
                                r.func3 = 0b101;
                                break;
                            case "sra": // sra rd, rs1, rs2
                                r.func3 = 0b101;
                                r.func7 = 0b0100000;
                                break;
                            case "or": // or rd, rs1, rs2
                                r.func3 = 0b110;
                                break;
                            case "and": // and rd, rs1, rs2
                                r.func3 = 0b111;
                                break;
                            default:
                                break;
                        }
                        output = (uint)r.Encode();
                        break;
                    case INST_TYPE.IType:
                        syntax = instructionSyntax.rri;
                        i.opcode = 0;
                        needs_rd = true;
                        needs_rs1 = true;
                        needs_imm = true;
                        switch (op)
                        {
                            case "jalr":
                                syntax = instructionSyntax.ror;
                                i.opcode = (OPCODE)0b11_001_11;
                                break;
                            case "lb":
                            case "lh":
                            case "lw":
                            case "ld":
                            case "lbu":
                            case "lhu":
                            case "lwu":
                            case "ldu":
                                syntax = instructionSyntax.ror;
                                i.opcode = (OPCODE)0b00_000_11;
                                break;
                            case "addiw": // RV64I Only
                            case "slliw": // RV64I Only
                            case "srliw": // RV64I Only
                            case "sraiw": // RV64I Only
                            case "sltiw": // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
                            case "sltiuw": // I don't think this instruction exists in the RiscV ISA // Would be RV64I Only because would do nothing on RV32
                            case "xoriw": // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
                            case "oriw": // I don't think this instruction exists in the RiscV ISA   // Would be RV64I Only because would do nothing on RV32
                            case "andiw": // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
                                i.opcode = (OPCODE)0b00_110_11;
                                break;
                            case "addi":
                            case "slti":
                            case "sltiu":
                            case "xori":
                            case "ori":
                            case "andi":
                            case "slli":
                            case "srli":
                            case "srai":
                                i.opcode = (OPCODE)0b00_100_11;
                                break;
                            case "fence":
                            case "fence.i":
                                i.opcode = (OPCODE)0b00_011_11;
                                break;
                            case "ecall":
                            case "ebrake":
                                i.opcode = (OPCODE)0b11_100_11;
                                break;
                            case "csrrw":
                            case "csrrs":
                            case "csrrc":
                            case "csrrwi":
                            case "csrrsi":
                            case "csrrci":
                                i.opcode = (OPCODE)0b11_100_11;
                                break;
                            default:
                                break;
                        }
                        switch (op)
                        {
                            case "jalr":
                                i.func3 = 0b000;
                                break;
                            case "lb":
                                i.func3 = (byte)FUNC3_MEMORY.BYTE;
                                break;
                            case "lh":
                                i.func3 = (byte)FUNC3_MEMORY.HALFWORD;
                                break;
                            case "lw":
                                i.func3 = (byte)FUNC3_MEMORY.WORD;
                                break;
                            case "ld":
                                i.func3 = (byte)FUNC3_MEMORY.DOUBLEWORD;
                                break;
                            case "lbu":
                                i.func3 = (byte)FUNC3_MEMORY.UNSIGNED_BYTE;
                                break;
                            case "lhu":
                                i.func3 = (byte)FUNC3_MEMORY.UNSIGNED_HALFWORD;
                                break;
                            case "lwu":
                                i.func3 = (byte)FUNC3_MEMORY.UNSIGNED_WORD;
                                break;
                            case "ldu":
                                i.func3 = (byte)FUNC3_MEMORY.UNSIGNED_DOUBLEWORD;
                                break;
                            case "addi":
                            case "addiw": // RV64I Only
                                i.func3 = (byte)FUNC3_ALU.ADD;
                                break;
                            case "sltiw": // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
                            case "slti":
                                i.func3 = (byte)FUNC3_ALU.SLT;
                                break;
                            case "sltiuw": // I don't think this instruction exists in the RiscV ISA // Would be RV64I Only because would do nothing on RV32
                            case "sltiu":
                                i.func3 = (byte)FUNC3_ALU.SLTU;
                                break;
                            case "xori":
                            case "xoriw": // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
                                i.func3 = (byte)FUNC3_ALU.XOR;
                                break;
                            case "ori":
                            case "oriw": // I don't think this instruction exists in the RiscV ISA   // Would be RV64I Only because would do nothing on RV32
                                i.func3 = (byte)FUNC3_ALU.OR;
                                break;
                            case "andi":
                            case "andiw": // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
                                i.func3 = (byte)FUNC3_ALU.AND;
                                break;
                            case "slli":
                            case "slliw": // RV64I Only
                                i.func3 = (byte)FUNC3_ALU.SLL;
                                i.imm = 0b0000000_11111 & i.imm; //Truncate IMM if its out of range
                                break;
                            case "srli":
                            case "srliw": // RV64I Only
                                i.func3 = (byte)FUNC3_ALU.SR;
                                i.imm = 0b0000000_11111 & i.imm; //Truncate IMM if its out of range
                                break;
                            case "srai":
                            case "sraiw": // RV64I Only
                                i.func3 = (byte)FUNC3_ALU.SR;
                                i.imm = 0b0000000_11111 & i.imm; //Truncate IMM if its out of range
                                i.imm = 0b0100000_00000 | i.imm; // Add bit that denotes this being and sra not an srl
                                break;
                            case "fence":
                            case "fence.i":
                                i.func3 = 0b000;
                                break;
                            case "ecall":
                                i.func3 = 0b000;
                                i.rd = Register.zero;
                                i.rs1 = Register.zero;
                                i.imm = 0;
                                needs_rd = false;
                                needs_rs1 = false;
                                needs_imm = false;
                                break;
                            case "ebrake":
                                i.func3 = 0b000;
                                i.rd = Register.zero;
                                i.rs1 = Register.zero;
                                needs_rd = false;
                                needs_rs1 = false;
                                needs_imm = false;
                                i.imm = 1;
                                break;
                            case "csrrw":
                            case "csrrs":
                            case "csrrc":
                            case "csrrwi":
                            case "csrrsi":
                            case "csrrci":
                                i.func3 = 0b000;
                                break;
                            default:
                                break;
                        }
                        if (syntax == instructionSyntax.rri && needs_rd && Enum.TryParse(match.Groups[Regexpression.g.rri_rd].Value, out i.rd) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rd` of `" + match.Groups[Regexpression.g.rrr_rd].Value + "` in the IType partern `OP rd, rs1, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.rri && needs_rs1 && Enum.TryParse(match.Groups[Regexpression.g.rri_r1].Value, out i.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.rrr_r1].Value + "` in the IType partern `OP rd, rs1, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.rri && needs_imm && Int32.TryParse(match.Groups[Regexpression.g.rri_imm].Value, out i.imm) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `imm` of `" + match.Groups[Regexpression.g.rrr_r1].Value + "` in the IType partern `OP rd, rs1, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.ror && needs_rd && Enum.TryParse(match.Groups[Regexpression.g.ror_r2].Value, out i.rd) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs2` of `" + match.Groups[Regexpression.g.ror_r2].Value + "` in the IType partern `OP rs2, imm(rs1)` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.ror && needs_rs1 && Enum.TryParse(match.Groups[Regexpression.g.ror_r1].Value, out i.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.ror_r1].Value + "` in the IType partern `OP rs2, imm(rs1)` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.ror && needs_imm && Int32.TryParse(match.Groups[Regexpression.g.ror_imm].Value, out i.imm) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `imm` of `" + match.Groups[Regexpression.g.ror_imm].Value + "` in the IType partern `OP rs2, imm(rs1)` with the input `" + asm + "` as a line of Risc-V ASM"));
                        output = (uint)i.Encode();
                        break;
                    case INST_TYPE.SType:
                        syntax = instructionSyntax.ror;
                        needs_rs1 = true;
                        needs_rs2 = true;
                        needs_imm = true;

                        s.opcode = 0;

                        switch (op)
                        {
                            case "sb":
                            case "sh":
                            case "sw":
                            case "sd":
                            // case "sbu": // May make sense but is not in the SPEC
                            // case "shu": // May make sense but is not in the SPEC
                            // case "swu": // May make sense but is not in the SPEC
                            // case "sdu": // May make sense but is not in the SPEC
                                s.opcode = (OPCODE)0b01_000_11;
                                break;
                            default:
                                break;
                        }
                        switch (op)
                        {
                            case "sb":
                                s.func3 = (byte)FUNC3_MEMORY.BYTE;
                                break;
                            case "sh":
                                s.func3 = (byte)FUNC3_MEMORY.HALFWORD;
                                break;
                            case "sw":
                                s.func3 = (byte)FUNC3_MEMORY.WORD;
                                break;
                            case "sd":
                                s.func3 = (byte)FUNC3_MEMORY.DOUBLEWORD;
                                break;
                            // May make sense but is not in the SPEC
                            //                             case "lbu":
                            //                                 s.func3 = (byte)FUNC3_MEMORY.UNSIGNED_BYTE;
                            //                                 break;
                            //                             case "lhu":
                            //                                 s.func3 = (byte)FUNC3_MEMORY.UNSIGNED_HALFWORD;
                            //                                 break;
                            //                             case "lwu":
                            //                                 s.func3 = (byte)FUNC3_MEMORY.UNSIGNED_WORD;
                            //                                 break;
                            //                             case "ldu":
                            //                                 s.func3 = (byte)FUNC3_MEMORY.UNSIGNED_DOUBLEWORD;
                            //                                 break;
                            default:
                                break;
                        }

                        if (syntax == instructionSyntax.ror && needs_rs1 && Enum.TryParse(match.Groups[Regexpression.g.ror_r1].Value, out s.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.ror_r1].Value + "` in the IType partern `OP rs2, imm(rs1)` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.ror && needs_rs2 && Enum.TryParse(match.Groups[Regexpression.g.ror_r2].Value, out s.rs2) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs2` of `" + match.Groups[Regexpression.g.ror_r2].Value + "` in the IType partern `OP rs2, imm(rs1)` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (syntax == instructionSyntax.ror && needs_imm && Int64.TryParse(match.Groups[Regexpression.g.ror_imm].Value, out s.imm) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `imm` of `" + match.Groups[Regexpression.g.ror_imm].Value + "` in the IType partern `OP rs2, imm(rs1)` with the input `" + asm + "` as a line of Risc-V ASM"));
                        output = (uint)s.Encode();
                        break;
                    case INST_TYPE.BType:
                        syntax = instructionSyntax.rri;
                        b.opcode = OPCODE.B32_BRANCH;
                        if (Enum.TryParse(match.Groups[Regexpression.g.rri_rd].Value, out b.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.rri_rd].Value + "` in the BType partern `OP rs1, rs2, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (Enum.TryParse(match.Groups[Regexpression.g.rri_r1].Value, out b.rs2) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs2` of `" + match.Groups[Regexpression.g.rri_r1].Value + "` in the BType partern `OP rs1, rs2, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        ulong immTemp = 0; // TODO: This should probably be moved out to a global temp so that it does not cause memory alloc.
                        if (ulong.TryParse(match.Groups[Regexpression.g.rri_imm].Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.CurrentInfo, out immTemp) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `imm` of `" + match.Groups[Regexpression.g.rri_imm].Value + "` in the BType partern `OP rs1, rs2, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        //b.imm = (short)((immTemp - opStartByte) / 2); // This is automatic from the corrections to the BType encoding that has been corrected
                        b.imm = (short)(immTemp - opStartByte);

                        switch (op)
                        {
                            case "beq":
                                b.func3 = (byte) FUNC3_LOGICAL.EQ;
                                break;
                            case "bne":
                                b.func3 = (byte)FUNC3_LOGICAL.NE;
                                break;
                            case "blt":
                                b.func3 = (byte)FUNC3_LOGICAL.LT;
                                break;
                            case "bge":
                                b.func3 = (byte)FUNC3_LOGICAL.GE;
                                break;
                            case "bltu":
                                b.func3 = (byte)FUNC3_LOGICAL.LTU;
                                break;
                            case "bgeu":
                                b.func3 = (byte)FUNC3_LOGICAL.GEU;
                                break;
                            default:
                                throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `op` of `" + op + "` in the BType partern `OP rs1, rs2, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        }

                        output = (uint)b.Encode();
                        break;
                    case INST_TYPE.UType:
                        syntax = instructionSyntax.ri;
                        u.opcode = 0;
                        needs_rd = true;
                        needs_imm = true;

                        switch (op)
                        {
                            case "auipc":
                                u.opcode = (OPCODE)0b00_101_11;
                                break;
                            default:
                                break;
                        }

                        if (syntax == instructionSyntax.ri && needs_rd && Enum.TryParse(match.Groups[Regexpression.g.rir].Value, out u.rd) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rd` of `" + match.Groups[Regexpression.g.rir].Value + "` in the IType partern `OP rd, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        try
                        {
                            if (syntax == instructionSyntax.ri && needs_imm) u.imm = Convert.ToInt32(match.Groups[Regexpression.g.rii].Value, 16);
                        }
                        catch (Exception)
                        {
                            throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `imm` of `" + match.Groups[Regexpression.g.rii].Value + "` in the IType partern `OP rd, imm` with the input `" + asm + "` as a line of Risc-V ASM"));
                        }
                        output = (uint)u.Encode();
                        break;
                    case INST_TYPE.JType:
                        break;
                    case INST_TYPE.PType:
                        switch (op)
                        {
                            case "mv":
                                if (syntax == instructionSyntax.ror && needs_rs1 && Enum.TryParse(match.Groups[Regexpression.g.rr1].Value, out s.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.rr1].Value + "` in the partern `OP rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                                if (syntax == instructionSyntax.ror && needs_rs2 && Enum.TryParse(match.Groups[Regexpression.g.rr2].Value, out s.rs2) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs2` of `" + match.Groups[Regexpression.g.rr2].Value + "` in the partern `OP rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                                return assembleSingleLine(opStartByte, "addi "+ match.Groups[Regexpression.g.rr1].Value+", "+ match.Groups[Regexpression.g.rr2].Value + ", 0");
                            default:
                                throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could not translate pseudoinstruction `op` of `" + op + "` in the line input `" + asm + "` as a line of Risc-V ASM"));
                        }
                    case INST_TYPE.PRType:
                        try
                        {
                            return assembleSingleLine(opStartByte, Transcoder.PSEDO_REPLACEMENTS[op]);
                        } catch(Exception e)
                        {
                            //Console.WriteLine("External Compiler Panic");
                            //Console.WriteLine(e);
                            throw new Exception("External Compiler Panic", new Exception("Kuick Compiler should never reach this line of code. It is likely that the transcoder could not find the Psedocode replacement. The input was `" + asm + "`"));
                        }
                    case INST_TYPE.Unkwn:
                    default:
                        throw new Exception("External Compiler Panic", new Exception("Kuick Compiler should never reach this line of code. The input was `" + asm + "`"));
                }
            } else throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could not find the OP `" + op + "` in the input of `" + asm + "`"));
            return output;
        }
        public static uint[] compile(string[] asm)
        {
            uint[] output = new uint[asm.Length * 4];
            return output;
        }
        public static uint[] compileToStructs(string[] asm)
        {
            uint[] output = new uint[asm.Length * 4];
            return output;
        }
    }
}
