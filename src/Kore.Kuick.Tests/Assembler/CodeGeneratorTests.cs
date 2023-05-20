using Kore.AST;
using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Kore.Kuick.Tests.Assembler {
    public class CodeGeneratorTests {
        [SetUp]
        public void Setup() {
        }

        // Test code generator output.
        [Test]
        [TestCase(@".text
    li      a5,0      # should result in 0x00000793u
    lui     a0,10     # should result in 0x00010537u
    ret               # should result in 0x00008067u
    auipc   gp,0x2    # should result in 0x00002197u
    sub     a2,a2,a0  # should result in 0x40a60633u
    li      a1,0      # should result in 0x00000593u
    auipc   a0,0      # should result in 0x00000517u
    lw      a0,0(sp)  # should result in 0x00012503u
    addi    a1,sp,8   # should result in 0x00810593u
    li      a2,0      # should result in 0x00000613u
    addi    sp,sp,-16 # should result in 0xff010113u
    sd      s0,0(sp)  # should result in 0x00813023u
", new uint[] {0x00000793u, 0x00010537u, 0x00008067u, 0x00002197u, 0x40a60633u, 0x00000593u, 0x00000517u, 0x00012503u, 0x00810593u, 0x00000613u, 0xff010113u, 0x00813023u })]

        public void MachineCode(string asm, uint[] data) {
            
            // Setup the lexer and parse the input into tokens
            var lexer = new Lexer();
            lexer.Load(asm);

            // Setup the parser and parse the tokens into an AST
            var ast = Kore.Kuick.Parser.Parse(lexer);

            // Run the code generator on the AST
            Kore.Kuick.Assembler.CodeGenerator generator = new Kore.Kuick.Assembler.CodeGenerator();
            byte[] machineCode = generator.Generate(ast);

            // Check that the machine code length matches 4x the data provided's length.
            Assert.AreEqual(machineCode.Length, data.Length * 4);

            // Loop through all the uints provided as the data and check that the machine code matches.
            // Advance the index by 4 each time because each uint is 4 bytes.
            for (int i = 0; i < data.Length; i+=4) {
                // Create a uint from the 4 bytes of machine code at the current index via bitwise operations and compare to the data via assertion.
                Assert.AreEqual((uint)(machineCode[i] << 24 | machineCode[i + 1] << 16 | machineCode[i + 2] << 8 | machineCode[i + 3]), data[i], "Machine code does not match data.");
            }
        }

    }

}
