using Kore;
using Kore.RiscISA.Instruction;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace KoreTests
{
    public class KuickParserTests
    {
        private static Random rand = new Random();


        [Test]
        public void simpleDevTest()
        {
            KuickParser parser = new KuickParser();

            string page = "42";

            object r = parser.parse(page);

            Console.WriteLine(r.ToString());
            Assert.AreEqual("[PAGE|[NUMBER_INT|42]]", (r.ToString()));
        }

        [Test]
        public void RVDDT_Stand00_crt0_Test()
        {
            KuickParser parser = new KuickParser();

            string page = File.ReadAllText("rvddt.stand00.crt0.S");

            object r = parser.parse(page);

            Console.WriteLine(r.ToString());
            Assert.AreEqual("[PAGE|[NUMBER_INT|42]]", (r.ToString()));
        }

        [Test]
        public void numberTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = "  " + target + "  ";
                string page2 = "" + target;

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r2.ToString()));
            }
        }

        [Test]
        public void stringTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = "  \"" + target + "\"  ";
                string page2 = "\"    " + target + "  \"";

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.STRING, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual("" + target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.STRING, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual("    " + target + "  ", ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[STRING|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[STRING|    " + target + "  ]]", (r2.ToString()));
            }
        }

        [Test]
        public void commentDoubleSlashTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = " //asdghjjheu \n " + target + " // " + target + " ";
                string page2 = "" + target + " //asgfjsdkahn";

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r2.ToString()));
            }
        }

        [Test]
        public void commentHashTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = " #asdghjjheu \n " + target + " # " + target + " ";
                string page2 = "" + target + " #asgfjsdkahn";

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r2.ToString()));
            }
        }

        [Test]
        public void commentSlashStarTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = " /*asdghjjheu*/ \n " + target + " /* \n" + target + "\n*/ ";
                string page2 = "" + target + " /*\n\n\n\n\n\nasgfjsdkahn*/";

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r2.ToString()));
            }
        }

        [TestCase("text", KuickParser.StatementType.DIRECTIVE_TEXT)]
        [TestCase("data", KuickParser.StatementType.DIRECTIVE_DATA)]
        [TestCase("bss", KuickParser.StatementType.DIRECTIVE_BSS)]
        public void SimpleDirectiveTest(string directive, KuickTokenizer.Token type)
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int padding = rand.Next();
                string page1 = " ." + directive + " \n " + padding + " /* \n." + directive + "\n" + padding + "\n*/ ";
                string page2 = "" + padding + " \n\n\n\n\n\n." + directive + "";
                string page3 = "." + directive + "";

                KuickParser.ParseData r1 = parser.parse(page1);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);*/

                KuickParser.ParseData r2 = parser.parse(page2);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);*/
                KuickParser.ParseData r3 = parser.parse(page3);

                /* TestActionAttribute debugging lines
                Console.WriteLine("r1: " + page1 + "\n|\n" + r1.ToString());
                Console.WriteLine("r2: " + page2 + "\n|\n" + r2.ToString());
                */

                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[DIRECTIVE_" + directive.ToUpper() + "],[NUMBER_INT|" + padding + "]]]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[NUMBER_INT|" + padding + "],[DIRECTIVE_" + directive.ToUpper() + "]]]]", (r2.ToString()));
                Assert.AreEqual("[PAGE|[DIRECTIVE_" + directive.ToUpper() + "]]", (r3.ToString()));
            }
        }

        [TestCase("rvc")]
        [TestCase("norvc")]
        [TestCase("relax")]
        [TestCase("norelax")]
        [TestCase("pic")]
        [TestCase("nopic")]
        [TestCase("push")]
        [TestCase("pop")]
        public void OptionDirectiveTest(string option)
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int padding = rand.Next();
                string page1 = " .option " + option + " \n " + padding + " /* \n.option " + option + "\n" + padding + "\n*/ ";
                string page2 = "" + padding + " \n\n\n\n\n\n.option " + option + "";
                string page3 = ".option " + option + "";

                KuickParser.ParseData r1 = parser.parse(page1);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);*/

                KuickParser.ParseData r2 = parser.parse(page2);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);*/
                KuickParser.ParseData r3 = parser.parse(page3);

                /* TestActionAttribute debugging lines
                Console.WriteLine("r1: " + page1 + "\n|\n" + r1.ToString());
                Console.WriteLine("r2: " + page2 + "\n|\n" + r2.ToString());
                */

                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[DIRECTIVE_OPTION|" + option + "],[NUMBER_INT|" + padding + "]]]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[NUMBER_INT|" + padding + "],[DIRECTIVE_OPTION|" + option + "]]]]", (r2.ToString()));
                Assert.AreEqual("[PAGE|[DIRECTIVE_OPTION|" + option + "]]", (r3.ToString()));
            }
        }


        [TestCase("ajdgnf")]
        [TestCase("sdgere")]
        [TestCase("asdg4er")]
        [TestCase("ASDgfagf4")]
        [TestCase("_fnkejhb3")]
        [TestCase("_3")]
        [TestCase("asklfj_23")]
        [TestCase("pop")]
        public void GlobalDirectiveTest(string sym)
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int padding = rand.Next();
                string page1 = " .global " + sym + " \n " + padding + " /* \n.global " + sym + "\n" + padding + "\n*/ ";
                string page2 = "" + padding + " \n\n\n\n\n\n.global " + sym + "";
                string page3 = ".global " + sym + "";

                KuickParser.ParseData r1 = parser.parse(page1);
                KuickParser.ParseData r2 = parser.parse(page2);
                KuickParser.ParseData r3 = parser.parse(page3);

                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[DIRECTIVE_GLOBAL|" + sym + "],[NUMBER_INT|" + padding + "]]]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[NUMBER_INT|" + padding + "],[DIRECTIVE_GLOBAL|" + sym + "]]]]", (r2.ToString()));
                Assert.AreEqual("[PAGE|[DIRECTIVE_GLOBAL|" + sym + "]]", (r3.ToString()));
            }
        }

        // Shifts RV32I
        [TestCase("SLL <rd>, <rs1>, <rs2>", "[OP_R|[OP|SLL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Left Logical
        [TestCase("SLLI <rd>, <rs1>, <shamt>", "[OP_I|[OP|SLLI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]", true)] // Shift Left Logical Immediate
        [TestCase("SRL <rd>, <rs1>, <rs2>", "[OP_R|[OP|SRL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Right Logical
        [TestCase("SRLI <rd>, <rs1>, <shamt>", "[OP_I|[OP|SRLI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Shift Right Logical Immediate
        [TestCase("SRA <rd>, <rs1>, <rs2>", "[OP_R|[OP|SRA]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Right Arithmetic
        [TestCase("SRAI <rd>, <rs1>, <shamt>", "[OP_I|[OP|SRAI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Shift Right Arithmetic Immediate
        // Shifts RV64I
        [TestCase("SLLW <rd>, <rs1>, <rs2>", "[OP_R|[OP|SLLW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Left Logical
        [TestCase("SLLIW <rd>, <rs1>, <shamt>", "[OP_I|[OP|SLLIW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Shift Left Logical Immediate
        [TestCase("SRLW <rd>, <rs1>, <rs2>", "[OP_R|[OP|SRLW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Right Logical
        [TestCase("SRLIW <rd>, <rs1>, <shamt>", "[OP_I|[OP|SRLIW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Shift Right Logical Immediate
        [TestCase("SRAW <rd>, <rs1>, <rs2>", "[OP_R|[OP|SRAW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Right Arithmetic
        [TestCase("SRAIW <rd>, <rs1>, <shamt>", "[OP_I|[OP|SRAIW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Shift Right Arithmetic Immediate
        // // Arithmetic RV32I
        [TestCase("ADD <rd>, <rs1>, <rs2>", "[OP_R|[OP|ADD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Add
        [TestCase("ADDI <rd>, <rs1>, <shamt>", "[OP_I|[OP|ADDI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Add Immediate
        [TestCase("SUB <rd>, <rs1>, <rs2>", "[OP_R|[OP|SUB]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Subtract
        [TestCase("LUI <rd>, <imm[31:12] << 12>", "[OP_U|[OP|LUI]|[REGISTER|<rd>]|[NUMBER_HEX|<imm[31:12] << 12>]]")] // [RV32I][RV64I] Load Upper Immediate
        [TestCase("AUIPC <rd>, <imm[31:12] << 12>", "[OP_U|[OP|AUIPC]|[REGISTER|<rd>]|[NUMBER_HEX|<imm[31:12] << 12>]]")] // [RV32I][RV64I] Add Upper Immediate to PC and then assigns result to rd
        // // Arithmetic RV64I
        [TestCase("ADDW <rd>, <rs1>, <rs2>", "[OP_R|[OP|ADDW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Add
        [TestCase("ADDIW <rd>, <rs1>, <shamt>", "[OP_I|[OP|ADDIW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Add Immediate
        [TestCase("SUBW <rd>, <rs1>, <rs2>", "[OP_R|[OP|SUBW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Subtract
        // // Logical RV32I
        [TestCase("XOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|XOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Exclusive OR
        [TestCase("XORI <rd>, <rs1>, <shamt>", "[OP_I|[OP|XORI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Exclusive OR Immediate
        [TestCase("OR <rd>, <rs1>, <rs2>", "[OP_R|[OP|OR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // OR
        [TestCase("ORI <rd>, <rs1>, <shamt>", "[OP_I|[OP|ORI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // OR Immediate
        [TestCase("AND <rd>, <rs1>, <rs2>", "[OP_R|[OP|AND]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AND
        [TestCase("ANDI <rd>, <rs1>, <shamt>", "[OP_I|[OP|ANDI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // AND Immediate
        // // Compare RV32I
        [TestCase("SLT <rd>, <rs1>, <rs2>", "[OP_R|[OP|SLT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Set <
        [TestCase("SLTI <rd>, <rs1>, <shamt>", "[OP_I|[OP|SLTI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Set < Immediate
        [TestCase("SLTU <rd>, <rs1>, <rs2>", "[OP_R|[OP|SLTU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Set < Unsigned
        [TestCase("SLTIU <rd>, <rs1>, <shamt>", "[OP_I|[OP|SLTIU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Set < Immediate Unsigned
        // // Branches RV32I
        [TestCase("BEQ <rs1>, <rs2>, <imm>", "[OP_B|[OP|BEQ]|[REGISTER|<rs1>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Branch =
        [TestCase("BNE <rs1>, <rs2>, <imm>", "[OP_B|[OP|BNE]|[REGISTER|<rs1>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Branch !=
        [TestCase("BLT <rs1>, <rs2>, <imm>", "[OP_B|[OP|BLT]|[REGISTER|<rs1>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Branch <
        [TestCase("BGE <rs1>, <rs2>, <imm>", "[OP_B|[OP|BGE]|[REGISTER|<rs1>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Branch >=
        [TestCase("BLTU <rs1>, <rs2>, <imm>", "[OP_B|[OP|BLTU]|[REGISTER|<rs1>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Branch < Unsigned
        [TestCase("BGEU <rs1>, <rs2>, <imm>", "[OP_B|[OP|BGEU]|[REGISTER|<rs1>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Branch >= Unsigned
        // // Jump & Link RV32I
        [TestCase("JAL <rd>, <imm>", "[OP_J|[OP|JAL]|[REGISTER|<rd>]|[NUMBER_HEX|<imm>]]")] // Jump and Link
        [TestCase("JALR <rd>, <offset>(<rs1>)", "[OP_I|[OP|JALR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<offset>(<rs1>)]]")] // Jump and Link Register
        // // Synch RV32I
        [TestCase("FENCE <rd>, <rs1>, <shamt>", "[OP_I|[OP|FENCE]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Synch thread
        [TestCase("FENCE.I <rd>, <rs1>, <shamt>", "[OP_I|[OP|FENCE.I]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Synch Instruction and Data
        // // Environment RV32I
        [TestCase("ECALL <rd>, <rs1>, <shamt>", "[OP_I|[OP|ECALL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // CALL
        [TestCase("EBREAK <rd>, <rs1>, <shamt>", "[OP_I|[OP|EBREAK]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // BREAK
        // // Control Status Register (CSR) RV32I
        [TestCase("CSRRW <rd>, <csr>, <rs1>", "[OP_I|[OP|CSRRW]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Read/Write
        [TestCase("CSRRS <rd>, <csr>, <rs1>", "[OP_I|[OP|CSRRS]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Read and Set Bit
        [TestCase("CSRRC <rd>, <csr>, <rs1>", "[OP_I|[OP|CSRRC]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Read and Clear Bit
        [TestCase("CSRRWI <rd>, <csr>, <zimm[4:0]>", "[OP_I|[OP|CSRRWI]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[NUMBER_HEX|<zimm[4:0]>]]")] // Read/Write Immediate
        [TestCase("CSRRSI <rd>, <csr>, <zimm[4:0]>", "[OP_I|[OP|CSRRSI]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[NUMBER_HEX|<zimm[4:0]>]]")] // Read and Set Immediate
        [TestCase("CSRRCI <rd>, <csr>, <zimm[4:0]>", "[OP_I|[OP|CSRRCI]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[NUMBER_HEX|<zimm[4:0]>]]")] // Read and Clear Immediate
        // // Loads RV32I
        [TestCase("LB <rd>, <rs1>, <shamt>", "[OP_I|[OP|LB]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Byte
        [TestCase("LH <rd>, <rs1>, <shamt>", "[OP_I|[OP|LH]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Halfword
        [TestCase("LBU <rd>, <rs1>, <shamt>", "[OP_I|[OP|LBU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Byte Unsigned
        [TestCase("LHU <rd>, <rs1>, <shamt>", "[OP_I|[OP|LHU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Half Unsigned
        [TestCase("LW <rd>, <rs1>, <shamt>", "[OP_I|[OP|LW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Word
        // // Loads RV64I
        [TestCase("LWU <rd>, <rs1>, <shamt>", "[OP_I|[OP|LWU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Word Unsigned
        [TestCase("LD <rd>, <rs1>, <shamt>", "[OP_I|[OP|LD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Double Word
        // // Stores RV32I
        [TestCase("SB <rd>, <rs2>, <imm>", "[OP_S|[OP|SB]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Store Byte
        [TestCase("SH <rd>, <rs2>, <imm>", "[OP_S|[OP|SH]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Store Halfword
        [TestCase("SW <rd>, <rs2>, <imm>", "[OP_S|[OP|SW]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Store Word
        // // Stores RV64I
        [TestCase("SD <rd>, <rs2>, <imm>", "[OP_S|[OP|SD]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Store Double Word

        // // ----------------------------------------------------------------
        // // RV Privileged Instructions
        // // ----------------------------------------------------------------
        // // Trap
        [TestCase("MRET <rd>, <rs1>, <rs2>", "[OP_R|[OP|MRET]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Machine-mode trap return
        [TestCase("SRET <rd>, <rs1>, <rs2>", "[OP_R|[OP|SRET]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Supervisor-mode trap return
        // // Interrupt
        [TestCase("WFI <rd>, <rs1>, <rs2>", "[OP_R|[OP|WFI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Wait for interrupt
        // // MMU
        [TestCase("SFENCE.VMA <rd>, <rs1>, <rs2>", "[OP_R|[OP|SFENCE.VMA]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Virtual Memory FENCE

        // // ----------------------------------------------------------------
        // // The 60 Pseudoinstructions
        // // ----------------------------------------------------------------
        [TestCase("NOP", "[OP_I|[OP|ADDI]|[REGISTER|x0]|[REGISTER|x0]|[NUMBER_HEX|0]]")] // No Operation // NOP expands to addi x0, x0, 0
        [TestCase("NEG <rd> <rs2>", "[OP_R|[OP|SUB]|[REGISTER|<rd>]|[REGISTER|x0]|[REGISTER|<rs2>]]")] // Two's Complement // NEG rd,rs2 expands to sub rd, x0, rs2
        [TestCase("NEGW <rd>, <rs2>", "[OP_R|[OP|SUBW]|[REGISTER|<rd>]|[REGISTER|x0]|[REGISTER|<rs2>]]")] // Two's Complement Word // NEGW rd, rs2 expands to subw rd, x0, rs2
        // // --------
        [TestCase("SNEZ <rd>, <rs2>", "[OP_R|[OP|SLTU]|[REGISTER|<rd>]|[REGISTER|<x0]|[REGISTER|<rs2>]]")] // Set if != zero // SNEZ rd, rs2 expands to sltu rd, x0, rs2
        [TestCase("SLTZ <rd>, <rs1>", "[OP_R|[OP|SLT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|x0]]")] // Set if < zero // sltz rd, rs1 expands to slt rd, rs1, x0
        [TestCase("SGTZ <rd>, <rs2>", "[OP_R|[OP|SLT]|[REGISTER|<rd>]|[REGISTER|x0]|[REGISTER|<rs2>]]")] // Set if > zero // SGTZ rd, rs2 expands to slt rd, x0, rs2
        // // --------
        [TestCase("BEQZ <rs1>, <offset>", "[OP_B|[OP|BEQ]|[REGISTER|<rs1>]|[REGISTER|x0]|[NUMBER_HEX|<offset>]]")] // Branch if == zero // BEQZ rs1, offset expands to beq rs1, x0, offset
        [TestCase("BNEZ <rs1>, <offset>", "[OP_B|[OP|BNE]|[REGISTER|<rs1>]|[REGISTER|x0]|[NUMBER_HEX|<offset>]]")] // Branch if != zero // BNEZ rs1, offset expands to bne rs1, x0, offset
        [TestCase("BLEZ <rs1>, <offset>", "[OP_B|[OP|BLE]|[REGISTER|<rs1>]|[REGISTER|x0]|[NUMBER_HEX|<offset>]]")] // Branch if <= zero // BLEZ rs1, offset expands to ble rs1, x0, offset
        [TestCase("BGEZ <rs1>, <offset>", "[OP_B|[OP|BGE]|[REGISTER|<rs1>]|[REGISTER|x0]|[NUMBER_HEX|<offset>]]")] // Branch if >= zero // BGEZ rs1, offset expands to bge rs1, x0, offset
        [TestCase("BLTZ <rs1>, <offset>", "[OP_B|[OP|BLT]|[REGISTER|<rs1>]|[REGISTER|x0]|[NUMBER_HEX|<offset>]]")] // Branch if < zero // BLTZ rs1, offset expands to blt rs1, x0, offset
        [TestCase("BGTZ <rs1>, <offset>", "[OP_B|[OP|BGT]|[REGISTER|<rs1>]|[REGISTER|x0]|[NUMBER_HEX|<offset>]]")] // Branch if > zero // BGTZ rs1, offset expands to bgt rs1, x0, offset
        // // --------
        // [TestCase("J", KuickTokenizer.Token.OP_PSEUDO)] // Jump
        // [TestCase("JR", KuickTokenizer.Token.OP_PSEUDO)] // Jump to Register value
        // [TestCase("RET", KuickTokenizer.Token.OP_PSEUDO)] // Return from subroutine
        // // --------
        // [TestCase("TAIL", KuickTokenizer.Token.OP_PSEUDO)] // Tail call far-away subroutine
        // // --------
        // [TestCase("RDINSTRET[H]", KuickTokenizer.Token.OP_PSEUDO)] // Read instructions-retired counter
        // [TestCase("RDCYCLE[H]", KuickTokenizer.Token.OP_PSEUDO)] // Read Cycle counter
        // [TestCase("RDTIME[H]", KuickTokenizer.Token.OP_PSEUDO)] // Read realtime clock
        // // --------
        [TestCase("csrr <rd>, <csr>", "[OP_I|[OP|CSRRS]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Read CSR // csrr rd, csr expands to csrrs rd, csr, x0
        [TestCase("csrw <csr>, <rs1>", "[OP_I|[OP|CSRRW]|[REGISTER|x0]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Write CSR // csrw csr, rs1 expands to csrrw x0, csr, rs1
        [TestCase("csrs <rd>, <csr>", "[OP_I|[OP|CSRRS]|[REGISTER|<rd>]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Set bits in CSR // csrs csr, rs1 expands to csrrs x0, csr, rs1
        [TestCase("csrc <csr>, <rs1>", "[OP_I|[OP|CSRRC]|[REGISTER|x0]|[CSR_REGISTER|<csr>]|[REGISTER|<rs1>]]")] // Clear bits in CSR // csrc csr, rs1 expands to csrrc x0, csr, rs1`
        // // --------
        [TestCase("CSRWI <csr>, <zimm[4:0]>", "[OP_I|[OP|CSRRWI]|[REGISTER|x0]|[CSR_REGISTER|<csr>]|[NUMBER_HEX|<zimm[4:0]>]]")] // Write CSR, Immediate // csrwi csr, zimm[4:0] expands to csrrwi x0, csr, zimm
        [TestCase("CSRSI <csr>, <zimm[4:0]>", "[OP_I|[OP|CSRRSI]|[REGISTER|x0]|[CSR_REGISTER|<csr>]|[NUMBER_HEX|<zimm[4:0]>]]")] // Set bits in CSR, Immediate // csrsi csr, imm expands to csrrsi x0, csr, zimm
        [TestCase("CSRCI <csr>, <zimm[4:0]>", "[OP_I|[OP|CSRRCI]|[REGISTER|x0]|[CSR_REGISTER|<csr>]|[NUMBER_HEX|<zimm[4:0]>]]")] // Clear bits in CSR, Immediate // csrci csr, zimm[4:0] expands to csrrci x0, csr, zimm
        // // --------
        // [TestCase("FRCSR", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Read FP control/status register
        // [TestCase("FSCSR", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Write FP control/status register
        // // --------
        // [TestCase("FRRM", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Read FP rounding mode
        // [TestCase("FSRM", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Write FP rounding mode
        // // --------
        // [TestCase("FRFLAGS", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Read FP exception flags
        // [TestCase("FSFLAGS", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Write FP exception flags
        // // --------
        // [TestCase("LLA", KuickTokenizer.Token.OP_PSEUDO)] // Load local address
        // // --------
        // [TestCase("LA", KuickTokenizer.Token.OP_PSEUDO)] // Load address
        // // --------
        // //TODO: figure out how I want to hand these cases because they conflict
        // // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        // [TestCase("LB", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global byte
        // [TestCase("LH", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global half
        // [TestCase("LW", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global word
        // [TestCase("LD", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global double
        // // --------
        // //TODO: figure out how I want to hand these cases because they conflict
        // // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        // [TestCase("SB", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global byte
        // [TestCase("SH", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global half
        // [TestCase("SW", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global word
        // [TestCase("SD", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global double
        // // --------
        // //TODO: Check if these also colide with RV32V/RV64V
        // [TestCase("flw", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Floating-point load global
        // [TestCase("fld", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Floating-point load global
        // // --------
        // //TODO: Check if these also colide with RV32V/RV64V
        // [TestCase("fsw", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Floating-point store global
        // [TestCase("fsd", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Floating-point store global
        // // --------
        // [TestCase("li", KuickTokenizer.Token.OP_PSEUDO)] // Load immediate
        // [TestCase("mv", KuickTokenizer.Token.OP_PSEUDO)] // Copy register
        // [TestCase("not", KuickTokenizer.Token.OP_PSEUDO)] // One's complement
        // [TestCase("sext.w", KuickTokenizer.Token.OP_PSEUDO)] // Sign extend word
        // [TestCase("seqz", KuickTokenizer.Token.OP_PSEUDO)] // Set if = zero
        // // --------
        // [TestCase("fmv.s", KuickTokenizer.Token.OP_PSEUDO)] // Copy single-precision register
        // [TestCase("fabs.s", KuickTokenizer.Token.OP_PSEUDO)] // Single-precision absolute value
        // [TestCase("fneg.s", KuickTokenizer.Token.OP_PSEUDO)] // Single-precision negate
        // [TestCase("fmv.d", KuickTokenizer.Token.OP_PSEUDO)] // Copy double-precision register
        // [TestCase("fabs.d", KuickTokenizer.Token.OP_PSEUDO)] // Double-precision absolute value
        // [TestCase("fneg.d", KuickTokenizer.Token.OP_PSEUDO)] // Double-precision negate
        // // --------
        [TestCase("bgt", KuickTokenizer.Token.OP_PSEUDO)] // Branch if > // bgt rs1, rs2, offset expands to blt rs2, rs1, offset
        [TestCase("ble", KuickTokenizer.Token.OP_PSEUDO)] // Branch if <= // ble rs1, rs2, offset expands to bge rs2, rs1, offset
        [TestCase("bgtu", KuickTokenizer.Token.OP_PSEUDO)] // Branch if >, unsigned // bgtu rs1, rs2, offset expands to bltu rs2, rs1, offset
        [TestCase("bleu", KuickTokenizer.Token.OP_PSEUDO)] // Branch if <=, unsigned // bleu rs1, rs2, offset expands to bgeu rs2, rs1, offset
        // // --------
        // //TODO: figure out how I want to hand these cases because they conflict
        // // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        // [TestCase("jal", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Jump and Link by offset
        // [TestCase("jalr", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Jump and Link to register value
        // // --------
        // //TODO: figure out how I want to hand these cases because they conflict
        // // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("call rd <offsetHILO>", "[EXPRESSION_LIST|[[OP_U|[OP|AUIPC]|[REGISTER|<rd>]|[NUMBER_HEX|<offsetHI>]],[OP_I|[OP|JALR]|[REGISTER|<rd>]|[REGISTER|<rd>]|[NUMBER_HEX|<offsetLO>]]]]")] // [RV32I][RV64I] call rd offset // CALL rd, symbol expands to `auipc rd, offsetHi` then `jalr rd, offsetLo(rd)`
        [TestCase("call <offsetHILO>", "[EXPRESSION_LIST|[[OP_U|[OP|AUIPC]|[REGISTER|x1]|[NUMBER_HEX|<offsetHI>]],[OP_I|[OP|JALR]|[REGISTER|x1]|[REGISTER|x1]|[NUMBER_HEX|<offsetLO>]]]]")] // [RV32I][RV64I] call offset // CALL symbol expands to `auipc x1, offsetHi` then `jalr x1, offsetLo(x1)`
        // // --------
        // //TODO: figure out how I want to hand these cases because they conflict
        // // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        // [TestCase("fence", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Fence on all memory and I/O
        // // --------
        // [TestCase("fscsr", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Swap FP control/status register
        // [TestCase("fsrm", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Swap FP rounding mode
        // [TestCase("fsflags", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Swap FP exception flags

        // // ----------------------------------------------------------------
        // // Multiply-Divide Instructions: RVM
        // // ----------------------------------------------------------------
        // // Multiply RV32M
        [TestCase("MUL <rd>, <rs1>, <rs2>", "[OP_R|[OP|MUL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply
        [TestCase("MULH <rd>, <rs1>, <rs2>", "[OP_R|[OP|MULH]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply High
        [TestCase("MULHSU <rd>, <rs1>, <rs2>", "[OP_R|[OP|MULHSU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply High Sign/Unsigned
        [TestCase("MULHU <rd>, <rs1>, <rs2>", "[OP_R|[OP|MULHU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply High Unsigned
        // // Divide RV32M
        [TestCase("DIV <rd>, <rs1>, <rs2>", "[OP_R|[OP|DIV]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Divide
        [TestCase("DIVU <rd>, <rs1>, <rs2>", "[OP_R|[OP|DIVU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Divide Unsigned
        // // Remainder RV32M
        [TestCase("REM <rd>, <rs1>, <rs2>", "[OP_R|[OP|REM]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Remainder
        [TestCase("REMU <rd>, <rs1>, <rs2>", "[OP_R|[OP|REMU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Remainder Unsigned
        // // Multiply RV64M
        [TestCase("MULW <rd>, <rs1>, <rs2>", "[OP_R|[OP|MULW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply Word
        // // Divide RV64M
        [TestCase("DIVW <rd>, <rs1>, <rs2>", "[OP_R|[OP|DIVW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Divide Word
        // // Remainder RV64M
        [TestCase("REMW <rd>, <rs1>, <rs2>", "[OP_R|[OP|REMW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Remainder Word
        [TestCase("REMUW <rd>, <rs1>, <rs2>", "[OP_R|[OP|REMUW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Remainder //TODO: Confirm this opcode I think it should be REMWU but the book "The RISC-V Reader" has it as REMUW so yeah

        // // ----------------------------------------------------------------
        // // Atomic Instructions: RVA
        // // ----------------------------------------------------------------
        // // Load RV32A
        [TestCase("LR.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|LR.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Load Reserved Word
        // // Load RV64A
        [TestCase("LR.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|LR.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Load Reserved Double Word
        // // Store RV32A
        [TestCase("SC.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|SC.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Store Reserved Word
        // // Store RV64A
        [TestCase("SC.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|SC.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Store Reserved Double Word
        // // Swap RV32A
        [TestCase("AMOSWAP.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOSWAP.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Swap RV64A
        [TestCase("AMOSWAP.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOSWAP.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Add RV32A
        [TestCase("AMOADD.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOADD.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Add RV64AA
        [TestCase("AMOADD.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOADD.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Logical RV32A
        [TestCase("AMOXOR.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOXOR.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOAND.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOAND.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOOR.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOOR.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Logical RV64A
        [TestCase("AMOXOR.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOXOR.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOAND.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOAND.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOOR.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOOR.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Min/Max RV32A
        [TestCase("AMOMIN.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMIN.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOMAX.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMAX.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOMINU.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMINU.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOMAXU.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMAXU.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        // // Min/Max RV64A
        [TestCase("AMOMIN.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMIN.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOMAX.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMAX.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOMINU.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMINU.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 
        [TestCase("AMOMAXU.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMAXU.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // 

        // // ----------------------------------------------------------------
        // // Two Optional Floating-Point Instructions: RVF & RVD
        // // ----------------------------------------------------------------
        // // Move RV32{F|D}
        [TestCase("FMV.W.X <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMV.W.X]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Move form Integer
        [TestCase("FMV.X.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMV.X.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Move to Integer
        // // Move RV64{F|D}
        [TestCase("FMV.D.X <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMV.D.X]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Move form Integer
        [TestCase("FMV.X.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMV.X.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Move to Integer
        // // Convert RV32{F|D}
        [TestCase("FCVT.S.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.S.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int
        [TestCase("FCVT.D.W <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.D.W]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int
        [TestCase("FCVT.S.WU <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.S.WU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int unsigned
        [TestCase("FCVT.D.WU <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.D.WU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int unsigned
        [TestCase("FCVT.W.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.W.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int
        [TestCase("FCVT.W.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.W.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int
        [TestCase("FCVT.WU.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.WU.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int unsigned
        [TestCase("FCVT.WU.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.WU.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int unsigned
        // // Convert RV64{F|D}
        [TestCase("FCVT.S.L <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.S.L]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int
        [TestCase("FCVT.D.L <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.D.L]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int
        [TestCase("FCVT.S.LU <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.S.LU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int unsigned
        [TestCase("FCVT.D.LU <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.D.LU]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert form int unsigned
        [TestCase("FCVT.L.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.L.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int
        [TestCase("FCVT.L.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.L.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int
        [TestCase("FCVT.LU.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.LU.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int unsigned
        [TestCase("FCVT.LU.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCVT.LU.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert to int unsigned
        // // Load RV32{F|D}
        [TestCase("FLW <rd>, <rs1>, <shamt>", "[OP_I|[OP|FLW]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Word
        [TestCase("FLD <rd>, <rs1>, <shamt>", "[OP_I|[OP|FLD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load Double Word
        // // Store RV32{F|D}
        [TestCase("FSW <rd>, <rs2>, <imm>", "[OP_S|[OP|FSW]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Save Word
        [TestCase("FSD <rd>, <rs2>, <imm>", "[OP_S|[OP|FSD]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Save Double Word
        // // Arithmetic RV32{F|D}
        [TestCase("FADD.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FADD.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Add (Single Float)
        [TestCase("FADD.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FADD.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Add (Double Float)
        [TestCase("FSUB.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSUB.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Subtract (Single Float)
        [TestCase("FSUB.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSUB.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Subtract (Double Float)
        [TestCase("FMUL.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMUL.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply (Single Float)
        [TestCase("FMUL.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMUL.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply (Double Float)
        [TestCase("FDIV.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FDIV.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Divide (Single Float)
        [TestCase("FDIV.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FDIV.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Divide (Double Float)
        [TestCase("FSQRT.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSQRT.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Square Root (Single Float)
        [TestCase("FSQRT.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSQRT.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Square Root (Double Float)
        // // Mul-Add RV32{F|D}
        [TestCase("FMADD.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMADD.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply-Add (Single Float)
        [TestCase("FMADD.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMADD.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply-Add (Double Float)
        [TestCase("FMSUB.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMSUB.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply-Subtract (Single Float)
        [TestCase("FMSUB.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMSUB.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply-Subtract (Double Float)
        [TestCase("FNMADD.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FNMADD.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negative Multiply-Add (Single Float)
        [TestCase("FNMADD.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FNMADD.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negative Multiply-Add (Double Float)
        [TestCase("FNMSUB.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FNMSUB.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negative Multiply-Subtract (Single Float)
        [TestCase("FNMSUB.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FNMSUB.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negative Multiply-Subtract (Double Float)
        // // Sign Inject RV32{F|D}
        [TestCase("FSGNJ.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSGNJ.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Sign Source (Single Float)
        [TestCase("FSGNJ.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSGNJ.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Sign Source (Double Float)
        [TestCase("FSGNJN.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSGNJN.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negative Sign Source (Single Float)
        [TestCase("FSGNJN.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSGNJN.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negative Sign Source (Double Float)
        [TestCase("FSGNJX.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSGNJX.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // XOR Sign Source (Single Float)
        [TestCase("FSGNJX.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSGNJX.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // XOR Sign Source (Double Float)
        // // Min/Max RV32{F|D}
        [TestCase("FMIN.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMIN.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Minumum (Single Float)
        [TestCase("FMIN.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMIN.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Minumum (Double Float)
        [TestCase("FMAX.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMAX.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Maximum (Single Float)
        [TestCase("FMAX.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FMAX.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Maximum (Double Float)
        // // Compare RV32{F|D}
        [TestCase("FEQ.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FEQ.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Compare FLoat = (Single Float)
        [TestCase("FEQ.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FEQ.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Compare FLoat = (Double Float)
        [TestCase("FLT.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FLT.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Compare FLoat < (Single Float)
        [TestCase("FLT.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FLT.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Compare FLoat < (Double Float)
        [TestCase("FLE.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FLE.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Compare FLoat >= (Single Float)
        [TestCase("FLE.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FLE.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Compare FLoat >= (Double Float)
        // // Catagorize RV32{F|D}
        [TestCase("FCLASS.S <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCLASS.S]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Classify type (Single Float)
        [TestCase("FCLASS.D <rd>, <rs1>, <rs2>", "[OP_R|[OP|FCLASS.D]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Classify type (Double Float)
        // // Configure RV32{F|D}
        [TestCase("FRCSR <rd>, <rs1>, <rs2>", "[OP_R|[OP|FRCSR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Read Status
        [TestCase("FRRM <rd>, <rs1>, <rs2>", "[OP_R|[OP|FRRM]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Read Rounding Mode
        [TestCase("FRFLAGS <rd>, <rs1>, <rs2>", "[OP_R|[OP|FRFLAGS]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Read Flags
        [TestCase("FSCSR <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSCSR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Swap Status Reg
        [TestCase("FSRM <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSRM]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Swap Rounding Mode
        [TestCase("FSFLAGS <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSFLAGS]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Swap Flags
        [TestCase("FSRMI <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSRMI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Swap Rounding Mode Imm
        [TestCase("FSFLAGSI <rd>, <rs1>, <rs2>", "[OP_R|[OP|FSFLAGSI]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Swap Flags Imm

        // // ----------------------------------------------------------------
        // // Vector Instructions: RVV
        // // ----------------------------------------------------------------
        [TestCase("SETVL <rd>, <rs1>, <rs2>", "[OP_R|[OP|SETVL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Set Vector Len

        [TestCase("VMULH <rd>, <rs1>, <rs2>", "[OP_R|[OP|VMULH]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply High
        [TestCase("VREM <rd>, <rs1>, <rs2>", "[OP_R|[OP|VREM]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Remainder

        [TestCase("VSLL <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSLL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Left Logical
        [TestCase("VSRL <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSRL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Right Logical
        [TestCase("VSRA <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSRA]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Shift Right Arithmatic

        [TestCase("VLD <rd>, <rs1>, <shamt>", "[OP_I|[OP|VLD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[NUMBER_HEX|<shamt>]]")] // Load
        [TestCase("VLDS <rd>, <rs1>, <rs2>", "[OP_R|[OP|VLDS]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Load Strided
        [TestCase("VLDX <rd>, <rs1>, <rs2>", "[OP_R|[OP|VLDX]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Load Indexed

        [TestCase("VST <rd>, <rs2>, <imm>", "[OP_S|[OP|VST]|[REGISTER|<rd>]|[REGISTER|<rs2>]|[NUMBER_HEX|<imm>]]")] // Store
        [TestCase("VSTS <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSTS]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Store Strided
        [TestCase("VSTX <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSTX]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Store Indexed

        [TestCase("AMOSWAP <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOSWAP]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AMO Swap
        [TestCase("AMOADD <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOADD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AMO ADD
        [TestCase("AMOXOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOXOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AMO XOR
        [TestCase("AMOAND <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOAND]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AMO AND
        [TestCase("AMOOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AND OR
        [TestCase("AMOMIN <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMIN]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AMO Minimum
        [TestCase("AMOMAX <rd>, <rs1>, <rs2>", "[OP_R|[OP|AMOMAX]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AMO Maximum

        [TestCase("VPEQ <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPEQ]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate =
        [TestCase("VPNE <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPNE]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate !=
        [TestCase("VPLT <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPLT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate <
        [TestCase("VPGE <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPGE]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate >=

        [TestCase("VPAND <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPAND]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate AND
        [TestCase("VPANDN <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPANDN]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate AND NOT
        [TestCase("VPOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate OR
        [TestCase("VPXOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPXOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate XOR
        [TestCase("VPNOT <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPNOT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate NOT
        [TestCase("VPSWAP <rd>, <rs1>, <rs2>", "[OP_R|[OP|VPSWAP]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Predicate SWAP

        [TestCase("VMOV <rd>, <rs1>, <rs2>", "[OP_R|[OP|VMOV]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Move

        [TestCase("VCVT <rd>, <rs1>, <rs2>", "[OP_R|[OP|VCVT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Convert

        [TestCase("VADD <rd>, <rs1>, <rs2>", "[OP_R|[OP|VADD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Add
        [TestCase("VSUB <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSUB]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Subtract
        [TestCase("VMUL <rd>, <rs1>, <rs2>", "[OP_R|[OP|VMUL]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply
        [TestCase("VDIV <rd>, <rs1>, <rs2>", "[OP_R|[OP|VDIV]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Divide
        [TestCase("VSQRET <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSQRET]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Square Root

        [TestCase("VFMADD <rd>, <rs1>, <rs2>", "[OP_R|[OP|VFMADD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply-ADD
        [TestCase("VFMSUB <rd>, <rs1>, <rs2>", "[OP_R|[OP|VFMSUB]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Multiply-SUB
        [TestCase("VFNMADD <rd>, <rs1>, <rs2>", "[OP_R|[OP|VFNMADD]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negated Multiply-ADD
        [TestCase("VFNMSUB <rd>, <rs1>, <rs2>", "[OP_R|[OP|VFNMSUB]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negated Multiply-SUB

        [TestCase("VSGNJ <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSGNJ]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Sign Inject
        [TestCase("VSGNJN <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSGNJN]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Negated Sign Inject
        [TestCase("VSGNJX <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSGNJX]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // XOR Sign Inject

        [TestCase("VMIN <rd>, <rs1>, <rs2>", "[OP_R|[OP|VMIN]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Minimum
        [TestCase("VMAX <rd>, <rs1>, <rs2>", "[OP_R|[OP|VMAX]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Maximum

        [TestCase("VXOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|VXOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // XOR
        [TestCase("VOR <rd>, <rs1>, <rs2>", "[OP_R|[OP|VOR]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // OR
        [TestCase("VAND <rd>, <rs1>, <rs2>", "[OP_R|[OP|VAND]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // AND

        [TestCase("VCLASS <rd>, <rs1>, <rs2>", "[OP_R|[OP|VCLASS]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // CLASS

        [TestCase("VSETDCFG <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSETDCFG]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // Set Data Conf

        [TestCase("VEXTRACT <rd>, <rs1>, <rs2>", "[OP_R|[OP|VEXTRACT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // EXTRACT
        [TestCase("VMERGE <rd>, <rs1>, <rs2>", "[OP_R|[OP|VMERGE]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // MERGE
        [TestCase("VSELECT <rd>, <rs1>, <rs2>", "[OP_R|[OP|VSELECT]|[REGISTER|<rd>]|[REGISTER|<rs1>]|[REGISTER|<rs2>]]")] // SELECT

        // // ----------------------------------------------------------------
        // // Optional Compressed Instructions: RV32C
        // // ----------------------------------------------------------------
        // [TestCase("C.LW", KuickTokenizer.Token.OP_CL)] // Load Word
        // [TestCase("C.LWSP", KuickTokenizer.Token.OP_CI)] // Load Word SP
        // [TestCase("C.FLW", KuickTokenizer.Token.OP_CL)] // Float Load Word
        // [TestCase("C.FLWSP", KuickTokenizer.Token.OP_CI)] // Float Load Word SP
        // [TestCase("C.FLD", KuickTokenizer.Token.OP_CL)] // Float Load Double
        // [TestCase("C.FLDSP", KuickTokenizer.Token.OP_CI)] // Float Load Double SP

        // [TestCase("C.SW", KuickTokenizer.Token.OP_CS)] // Store Word
        // [TestCase("C.SWSP", KuickTokenizer.Token.OP_CSS)] // Store Word SP
        // [TestCase("C.FSW", KuickTokenizer.Token.OP_CS)] // Float Store Word
        // [TestCase("C.FSWSP", KuickTokenizer.Token.OP_CSS)] // Float Store Word SP
        // [TestCase("C.FSD", KuickTokenizer.Token.OP_CS)] //  Float Store Double
        // [TestCase("C.FSDSP", KuickTokenizer.Token.OP_CSS)] // Float Store Double SP

        // [TestCase("C.ADD", KuickTokenizer.Token.OP_CR)] // ADD
        // [TestCase("C.ADDI", KuickTokenizer.Token.OP_CI)] // ADD Immediate
        // [TestCase("C.ADDI16SP", KuickTokenizer.Token.OP_CI)] // ADD SP Immediate * 16
        // [TestCase("C.ADDI4SPN", KuickTokenizer.Token.OP_CIW)] // ADD SP Immediate * 4
        // [TestCase("C.SUB", KuickTokenizer.Token.OP_CR)] // SUB
        // [TestCase("C.AND", KuickTokenizer.Token.OP_CR)] // AND
        // [TestCase("C.ANDI", KuickTokenizer.Token.OP_CI)] // AND Immediate
        // [TestCase("C.OR", KuickTokenizer.Token.OP_CR)] // OR
        // [TestCase("C.XOR", KuickTokenizer.Token.OP_CR)] // Exlusive OR
        // [TestCase("C.MV", KuickTokenizer.Token.OP_CR)] // Move
        // [TestCase("C.LI", KuickTokenizer.Token.OP_CI)] // Load Immediate
        // [TestCase("C.LUI", KuickTokenizer.Token.OP_CI)] // Load Upper Imm

        // [TestCase("C.SLLI", KuickTokenizer.Token.OP_CI)] // Shift Left Immediate
        // [TestCase("C.SRAI", KuickTokenizer.Token.OP_CI)] // Shift Right Arithmetic Immediate
        // [TestCase("C.SRLI", KuickTokenizer.Token.OP_CI)] // Shift Left Logical Immediate

        // [TestCase("C.BEQZ", KuickTokenizer.Token.OP_CB)] // Branch == 0
        // [TestCase("C.BNEZ", KuickTokenizer.Token.OP_CB)] // Branch != 0

        // [TestCase("C.J", KuickTokenizer.Token.OP_CJ)] // Jump
        // [TestCase("C.JR", KuickTokenizer.Token.OP_CR)] // Jump Register

        // [TestCase("C.JAL", KuickTokenizer.Token.OP_CJ)] // J&L
        // [TestCase("C.JALR", KuickTokenizer.Token.OP_CR)] // Jump & Link Register

        // [TestCase("C.EBRAKE", KuickTokenizer.Token.OP_CI)] // Enviorment Brake

        // // ----------------------------------------------------------------
        // // Optional Compressed Extention: RV64C
        // // ----------------------------------------------------------------

        // // RV64C does not include 

        // [TestCase("C.ADDW", KuickTokenizer.Token.OP_CR)] // ADD Word
        // [TestCase("C.ADDIW", KuickTokenizer.Token.OP_CI)] // ADD Immediate Word

        // [TestCase("C.SUBW", KuickTokenizer.Token.OP_CR)] // Subtract Word

        // [TestCase("C.LD", KuickTokenizer.Token.OP_CL)] // Load Doubleword
        // [TestCase("C.LDSP", KuickTokenizer.Token.OP_CI)] // Load Doubleword SP

        // [TestCase("C.SD", KuickTokenizer.Token.OP_CS)] // Store Doubleword
        // [TestCase("C.SDSP", KuickTokenizer.Token.OP_CSS)] // Store Doubleword SP

        public void sanityCheckOps(string test, string output, bool debug = false)
        {
            for (int i = 0; i < 10; i++)
            {
                string ot = test;
                string oo = output;
                fillTestSet(ref ot, ref oo);

                testOp(ot, oo, debug);
                testOp(ot.ToLower(), oo, debug);
                testOp(ot.ToUpper(), oo, debug);
            }
        }

        void fillTestSet(ref string test, ref string output){
            fillToken(ref test, ref output, "<rd>", getRandomRegister());
            fillToken(ref test, ref output, "<rs1>", getRandomRegister());
            fillToken(ref test, ref output, "<rs2>", getRandomRegister());
            fillToken(ref test, ref output, "<shamt>", getRandomRegister());
            fillToken(ref test, ref output, "<imm>", getRandomRegister());
        }

        static string getRandomRegister(bool includeZero = true){
            return allRegisters[rand.Next(allRegisters.Length)];
        }

        static void fillToken(ref string str1, ref string str2, string token, string replacement){
            if(str1.Contains(token)){
                if(token == "<shamt>"){
                    var num = rand.Next(0, 100);
                    str1 = str1.Replace(token, num.ToString());
                    str2 = str2.Replace(token, Convert.ToUInt64("0x"+num, 16).ToString());
                    return;
                }
                if(token == "<imm>"){
                    var num = rand.Next(0, 100);
                    str1 = str1.Replace(token, num.ToString());
                    str2 = str2.Replace(token, Convert.ToUInt64("0x"+num, 16).ToString());
                    return;
                }
                str1 = str1.Replace(token, replacement);
                str2 = str2.Replace(token, replacement);
            }
        }

        static void testOp(string test, string result, bool debug = false)
        {
            if(debug) Console.WriteLine("Test Debug: " + test + "|"+result);
            KuickParser parser = new KuickParser();
            int padding = rand.Next();
            string page1 = " //" + padding + " \n " + test + " // " + padding + " ";
            string page2 = "" + test + " //asgfjsdkahn";

            KuickParser.ParseData r1 = parser.parse(page1);
            KuickParser.ParseData r2 = parser.parse(page2);

            Assert.AreEqual("[PAGE|" + result + "]", (r1.ToString()));
            Assert.AreEqual("[PAGE|" + result + "]", (r2.ToString()));
        }

        static string[] allRegisters = new string[] { "x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11", "x12", "x13", "x14", "x15", "x16", "x17", "x18", "x19", "x20", "x21", "x22", "x23", "x24", "x25", "x26", "x27", "x28", "x29", "x30", "x31" };
    }
}
