using Kore;
using Kore.RiscISA.Instruction;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KoreTests
{
    public static class KuickTokenizerTestUtils
    {
    }
    public class KuickTokenizerTest
    {
        private static Random rand = new Random();
        KuickTokenizer tokenizer = new KuickTokenizer();

        /*
        [TestCase(".text", new Type[] { typeof(KuickDirectiveToken)})] 
        [TestCase(".global _start", new KuickLexerToken[] { new KuickDirectiveToken(".text"), new KuickLexerToken("_start") })]
        [TestCase(".type _start, @function", new KuickLexerToken[] { new KuickDirectiveToken(".text"), new KuickLexerToken("_start"), new KuickLexerToken("@function") })]
*/

        [TestCase(".text", KuickTokenizer.Token.DIRECTIVE, ".text")]
        [TestCase("     .text", KuickTokenizer.Token.WHITESPACE, "     ")]
        [TestCase("yolo:", KuickTokenizer.Token.LABEL, "yolo:")]
        [TestCase("yolo", KuickTokenizer.Token.IDENTIFIER, "yolo")]
        [TestCase(".string \"str\"", KuickTokenizer.Token.DIRECTIVE, ".string")]
        [TestCase("\"str\"", KuickTokenizer.Token.STRING, "\"str\"")]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", KuickTokenizer.Token.DIRECTIVE, ".byte")]
        [TestCase("0xFF, 0xf2, 0x02, 0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0xFF")]
        [TestCase("0xf2, 0x02, 0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0xf2")]
        [TestCase("0x02, 0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0x02")]
        [TestCase("0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0x85")]
        [TestCase("0x05", KuickTokenizer.Token.NUMBER_HEX, "0x05")]
        [TestCase(".half 0xFFf2, 0x0285, 0x0563", KuickTokenizer.Token.DIRECTIVE, ".half")]
        [TestCase(".word 0xFAB3EA23, 0x63424253, 0x2535A244", KuickTokenizer.Token.DIRECTIVE, ".word")]
        [TestCase(".dword 0xFAB3EA2363424253, 0x634242532535A244", KuickTokenizer.Token.DIRECTIVE, ".dword")]
        [TestCase(".float 0.1f, 42.2f, 151326.52562f", KuickTokenizer.Token.DIRECTIVE, ".float")]
        [TestCase("0.1f, 42.2f, 151326.52562f", KuickTokenizer.Token.NUMBER_FLOAT, "0.1f")]
        [TestCase("42.2f, 151326.52562f", KuickTokenizer.Token.NUMBER_FLOAT, "42.2f")]
        [TestCase("151326.52562f", KuickTokenizer.Token.NUMBER_FLOAT, "151326.52562f")]
        [TestCase(".double 0.1d, 4.5d, 2.4d, 2414.125125d", KuickTokenizer.Token.DIRECTIVE, ".double")]
        [TestCase("0.1d, 4.5d, 2.4d, 2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "0.1d")]
        [TestCase("4.5d, 2.4d, 2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "4.5d")]
        [TestCase("2.4d, 2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "2.4d")]
        [TestCase("2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "2414.125125d")]
        [TestCase(".option rvc", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("rvc", KuickTokenizer.Token.IDENTIFIER, "rvc")]
        [TestCase(".option norvc", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("norvc", KuickTokenizer.Token.IDENTIFIER, "norvc")]
        [TestCase(".option relax", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("relax", KuickTokenizer.Token.IDENTIFIER, "relax")]
        [TestCase(".option norelax", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("norelax", KuickTokenizer.Token.IDENTIFIER, "norelax")]
        [TestCase(".option pic", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("pic", KuickTokenizer.Token.IDENTIFIER, "pic")]
        [TestCase(".option nopic", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("nopic", KuickTokenizer.Token.IDENTIFIER, "nopic")]
        [TestCase(".option push", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("push", KuickTokenizer.Token.IDENTIFIER, "push")]
        public void readToken(string test, KuickTokenizer.Token token, string value)
        {
            tokenizer.load(test);
            KuickTokenizer.TokenData tokenData = tokenizer.readToken();
            Assert.AreEqual(token, tokenData.token);
            Assert.AreEqual(value, tokenData.value);
        }

        [Test]
        [TestCase(".text",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { ".text", default(string), default(string), default(string) }
        )]
        [TestCase("     .text",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { "     ", ".text", default(string), default(string), default(string) }
        )]
        [TestCase("yolo:",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.LABEL, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { "yolo:", default(string), default(string), default(string) }
        )]
        [TestCase("yolo",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { "yolo", default(string), default(string), default(string) }
        )]
        [TestCase(".string \"str\"",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.STRING, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { ".string", " ", "\"str\"", default(string), default(string), default(string) }
        )]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".byte"," ","0xFF",", ","0xf2",", ","0x02",", ","0x85",", ","0x05", default(string), default(string), default(string) }
        )]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".byte"," ","0xFF",", ","0xf2",", ","0x02",", ","0x85",", ","0x05", default(string), default(string), default(string) }
        )]
        [TestCase(".half 0xFFf2, 0x0285, 0x0563", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".half"," ","0xFFf2",", ","0x0285",", ","0x0563", default(string), default(string), default(string) }
        )]
        [TestCase(".word 0xFAB3EA23, 0x63424253, 0x2535A244", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".word"," ","0xFAB3EA23",", ","0x63424253",", ","0x2535A244", default(string), default(string), default(string) }
        )]
        [TestCase(".dword 0xFAB3EA2363424253, 0x634242532535A244", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".dword"," ","0xFAB3EA2363424253",", ","0x634242532535A244", default(string), default(string), default(string) }
        )]
        [TestCase(".float 0.1f, 42.2f, 151326.52562f", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_FLOAT, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_FLOAT, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_FLOAT, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".float"," ","0.1f",", ","42.2f",", ","151326.52562f", default(string), default(string), default(string) }
        )]
        [TestCase(".double 0.1d, 4.5d, 2.4d, 2414.125125d", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".double"," ","0.1d",", ","4.5d",", ","2.4d",", ","2414.125125d", default(string), default(string), default(string) }
        )]
        [TestCase(".option rvc", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","rvc", default(string), default(string), default(string) }
        )]
        [TestCase(".option norvc", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","norvc", default(string), default(string), default(string) }
        )]
        [TestCase(".option relax", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","relax", default(string), default(string), default(string) }
        )]
        [TestCase(".option norelax", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","norelax", default(string), default(string), default(string) }
        )]
        [TestCase(".option pic", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","pic", default(string), default(string), default(string) }
        )]
        [TestCase(".option nopic", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","nopic", default(string), default(string), default(string) }
        )]
        [TestCase(".option push", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","push", default(string), default(string), default(string) }
        )]
        public void fullReadToken(string test, KuickTokenizer.Token[] tokens, string[] values)
        {
            tokenizer.load(test);
            for (int i = 0; i < tokens.Length; i++)
            {
                KuickTokenizer.TokenData tokenData = tokenizer.readToken();
                Assert.AreEqual(tokens[i], tokenData.token);
                Assert.AreEqual(values[i], tokenData.value);
            }
        }

        // .string "str"
        // .byte b1, b2,...., bn
        // .half w1, w2,...., wn
        // .word w1, w2,...., wn
        // .dword w1, w2,...., wn
        // .float f1, f2,...., fn
        // .double d1, d2,...., dn
        // .option rvc
        // .option norvc
        // .option relax
        // .option norelax
        // .option pic
        // .option nopic
        // .option push
        /* .repeat
         * asm
         * .endr
         * this will not be implimented until later down the road
         * */
        // .option push
    }
}
