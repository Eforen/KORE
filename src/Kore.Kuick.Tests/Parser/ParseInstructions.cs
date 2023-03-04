using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Kore.Kuick;

namespace Kore.Kuick.Tests.Parser {
    class ParseInstructions {
            [SetUp]
            public void Setup() {
            }

            [Test]
            public void Test() {
                /*
                // create instance of parser
                Parser parser = new Parser();

                // get the parseInstruction method using reflection
                MethodInfo parseInstructionMethod = typeof(Parser).GetMethod("parseInstruction", BindingFlags.NonPublic | BindingFlags.Instance);

                // create instruction string to parse
                string instructionString = "add x1, x2, x3";

                // parse instruction
                InstructionNode instructionNode = (InstructionNode)parseInstructionMethod.Invoke(parser, new object[] { instructionString });

                // assert that the instruction node is not null and is of the correct type
                Assert.NotNull(instructionNode);
                Assert.IsType<InstructionNodeTypeR>(instructionNode);

                // cast the instruction node to an InstructionNodeTypeR and assert its properties
                InstructionNodeTypeR rTypeInstructionNode = (InstructionNodeTypeR)instructionNode;
                        Assert.Equal("add", rTypeInstructionNode.Name);
                Assert.Equal(Register.x1, rTypeInstructionNode.rd);
                Assert.Equal(Register.x2, rTypeInstructionNode.rs1);
                Assert.Equal(Register.x3, rTypeInstructionNode.rs2);
                 */
            }
    }
}