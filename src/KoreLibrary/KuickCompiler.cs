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
            public static readonly string ImmidiateValue = "[0-9a-f]+";
            public static readonly string OffsetRegister = "(?<ori>" + ImmidiateValue + ")\\(?<orr>" + Register+"\\)";
            public static readonly string DataRegReg = "(?<rr1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rr2>" + Register + ")";
            public static readonly string DataRegRegReg = "(?<rrr1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rrr2>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rrr3>" + Register + ")";
            public static readonly string DataRegImm = "(?<rir>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rii>" + ImmidiateValue + ")";
            public static readonly string DataRegRegImm = "(?<rri1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rri2>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + "(?<rrii>" + ImmidiateValue + ")";
            public static readonly string DataRegOffsetReg = "(?<ror1>" + Register + ")" + SpaceAnyOrNone + "," + SpaceAnyOrNone + OffsetRegister;
            public static readonly string DataCombined = DataRegReg + "|" + DataRegRegReg + "|" + DataRegImm + "|" + DataRegRegImm + "|" + DataRegOffsetReg;
            public static readonly string CommentOptional = "(?<comment>#.+)?(\n|$)";
            public static readonly string OpName = "(?<op>[a-z.]+)";
            public static readonly string expr = SpaceAnyOrNone+ OpName + SpaceOneOrMore+"(?<data>"+ DataCombined + ")"+SpaceAnyOrNone+CommentOptional;
            public static readonly Regex regex = new Regex(expr);
            public static class g
            {
                public static readonly int op = regex.GroupNumberFromName("op");
                public static readonly int rrr1 = regex.GroupNumberFromName("rrr1");
                public static readonly int rrr2 = regex.GroupNumberFromName("rrr2");
                public static readonly int rrr3 = regex.GroupNumberFromName("rrr3");
            }
            public static Match test(string text)
            {
                return regex.Match(text);
            }
        }

        private static RType r = new RType();
        private static IType i = new IType();
        private static SType s = new SType();
        private static BType b = new BType();
        private static UType u = new UType();
        private static JType j = new JType();
        public static uint compile(string asm)
        {
            Match match = Regexpression.test(asm.ToLower());
            if(match.Success == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could not understand the input of `" + asm + "` as a line of Risc-V ASM"));
            RiscISA.Instruction.TYPE_OF_INST instStruct = RiscISA.Instruction.TYPE_OF_INST.add;
            string op = match.Groups[Regexpression.g.op].Value;
            //Register reg = Register.x0;
            uint output = 0;
            if (Enum.TryParse(op, out instStruct)){
                switch ((RiscISA.Instruction.INST_TYPE)(byte)instStruct)
                {
                    case INST_TYPE.RType:
                        r.opcode = OPCODE.B32_OP;
                        r.func7 = 0b0000000;
                        if (Enum.TryParse(match.Groups[Regexpression.g.rrr1].Value, out r.rd) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rd` of `" + match.Groups[Regexpression.g.rrr1].Value + "` in the partern `OP rd, rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (Enum.TryParse(match.Groups[Regexpression.g.rrr2].Value, out r.rs1) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs1` of `" + match.Groups[Regexpression.g.rrr2].Value + "` in the partern `OP rd, rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
                        if (Enum.TryParse(match.Groups[Regexpression.g.rrr3].Value, out r.rs2) == false) throw new Exception("External Compiler Panic", new Exception("Kuick Compiler could understand `rs2` of `" + match.Groups[Regexpression.g.rrr3].Value + "` in the partern `OP rd, rs1, rs2` with the input `" + asm + "` as a line of Risc-V ASM"));
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
                        break;
                    case INST_TYPE.SType:
                        break;
                    case INST_TYPE.BType:
                        break;
                    case INST_TYPE.UType:
                        break;
                    case INST_TYPE.JType:
                        break;
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
