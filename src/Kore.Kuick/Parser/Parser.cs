/*
*/
using Kore.AST;
using Kore.RiscMeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.Kuick {
    public static class Parser {
        #region Utilities
        public class SyntaxException : Exception { public SyntaxException(string msg) : base(msg) { } }
        private static Exception ThrowUnexpected(Lexer.TokenData currentToken, string expectation) {
            throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Expected {expectation}.");
        }
        private static Exception ThrowUnimplemented(Lexer.TokenData currentToken) {
            throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Operation Not implimented.");
        }
        private static Exception ThrowParserPanic(Lexer.TokenData currentToken) {
            throw new SyntaxException($"PARSER PANIC: Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}.");
        }

        /// <summary>
        /// Expects a token of a specific type from the lexer and returns it. If the token read from the lexer does not match the
        /// expected type, a SyntaxException will be thrown.
        /// </summary>
        /// <param name="lexer">The lexer to read the next token from.</param>
        /// <param name="expectation">The expected token type.</param>
        /// <param name="ignoreWhitespace">Whether or not to ignore whitespace tokens when reading.</param>
        /// <returns>The next token in the lexer's stream if it matches the expected type.</returns>
        private static Lexer.TokenData ExpectToken(Lexer lexer, Lexer.Token expectation, bool ignoreWhitespace = true) {
            var currentToken = lexer.ReadToken(ignoreWhitespace);

            if(currentToken.token != expectation) throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Expected {expectation.ToString()}.");

            return currentToken;
        }

        private static Lexer.TokenData ExpectToken(Lexer lexer, params Lexer.Token[] expectedTokens) {
            return ExpectToken(lexer, false, expectedTokens);
        }

        /// <summary>
        /// Expects a token of any of the specified types from the lexer and returns it. If the token read from the lexer does not match any of the
        /// expected types, a SyntaxException will be thrown.
        /// </summary>
        /// <param name="lexer">The lexer to read the next token from.</param>
        /// <param name="expectedTypes">The expected token types.</param>
        /// <returns>The next token in the lexer's stream if it matches any of the expected types.</returns>
        private static Lexer.TokenData ExpectToken(Lexer lexer, bool peak, params Lexer.Token[] expectedTokens) {
            var currentToken = peak ? lexer.PeakToken(true) : lexer.ReadToken(true);

            foreach(var expectation in expectedTokens) {
                if(currentToken.token == expectation) {
                    return currentToken;
                }
            }

            throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Expected {string.Join(" or ", expectedTokens.Select(e => e.ToString()))}.");
        }

        private static T ParseOP<T>(Lexer.TokenData currentToken, Lexer lexer, Lexer.Token expectedToken)
        where T : struct, Enum {
            if(currentToken.token != expectedToken) {
                throw ThrowUnexpected(currentToken, expectedToken.ToString());
            }

            if(Enum.TryParse(currentToken.value, true, out T op) == false) {
                throw ThrowUnexpected(currentToken, "OP");
            }

            return op;
        }

        private static Register ParseRegister(Lexer lexer) {
            var currentToken = ExpectToken(lexer, Lexer.Token.REGISTER);
            if(Enum.TryParse(currentToken.value, true, out Kore.RiscMeta.Register reg) == false) {
                throw ThrowUnexpected(currentToken, "Register");
            }
            return reg;
        }

        private static int ParseImmediate(Lexer lexer) {
            var currentToken = ExpectToken(lexer, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            if(currentToken.token == Lexer.Token.NUMBER_INT) return int.Parse(currentToken.value);
            if(currentToken.token == Lexer.Token.NUMBER_HEX) return Convert.ToInt32(currentToken.value, 16);
            throw ThrowParserPanic(currentToken);
        }

        #endregion


        private static LabelNode ParseNodeLabel(Lexer.TokenData currentToken, Lexer lexer) {
            if(currentToken.token != Lexer.Token.LABEL) throw ThrowUnexpected(currentToken, "LABEL");
            // Make the node
            var labelNode = new LabelNode(currentToken.value);

            // Get the next token
            currentToken = lexer.ReadToken();

            // If not the end of file or line
            // Check EOL first because its more likely
            if(currentToken.token != Lexer.Token.EOL || currentToken.token != Lexer.Token.EOF) {
                throw ThrowUnexpected(currentToken, "EOL || EOF");
            }
            return labelNode;
        }

        private static CommentNode ProcessNodeComment(Lexer.TokenData currentToken, Lexer lexer) {
            if(currentToken.token != Lexer.Token.LABEL) throw ThrowUnexpected(currentToken, "LABEL");
            // Make the node
            var labelNode = new CommentNode(currentToken.value);

            // Get the next token
            currentToken = lexer.ReadToken();

            // If not the end of file or line
            // Check EOL first because its more likely
            if(currentToken.token != Lexer.Token.EOL || currentToken.token != Lexer.Token.EOF) {
                throw ThrowUnexpected(currentToken, "EOL || EOF");
            }
            return labelNode;
        }

        private static InstructionNodeTypeR ParseRInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // add x1, x2, x3
            // OP  rd, rs1, rs2

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeR>(currentToken, lexer, Lexer.Token.OP_R);
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            var rd1 = ParseRegister(lexer); // Get the destination register (rd1)
            var rd2 = ParseRegister(lexer); // Get the destination register (rd2)

            return expectReturnEOL(new InstructionNodeTypeR(op, rd, rd1, rd2), lexer);
        }


        private static InstructionNodeTypeI ParseIInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // addi x1, x2, 15
            // OP  rd, rs, imm

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeI>(currentToken, lexer, Lexer.Token.OP_I);
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            var rs = ParseRegister(lexer); // Get the source register (rs)
            int imm = ParseImmediate(lexer); // Get the immediate value, which can be in decimal or hexadecimal format

            return expectReturnEOL(new InstructionNodeTypeI(op, rd, rs, imm), lexer);
        }
        private static InstructionNodeTypeS ParseSInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            var op = ParseOP<Kore.RiscMeta.Instructions.TypeS>(currentToken, lexer, Lexer.Token.OP_S);

            var rs2 = ParseRegister(lexer);
            var imm = ParseImmediate(lexer);
            ExpectToken(lexer, Lexer.Token.PARREN_OPEN);
            var rs1 = ParseRegister(lexer);
            ExpectToken(lexer, Lexer.Token.PARREN_CLOSE);

            return expectReturnEOL(new InstructionNodeTypeS(op, rs1, rs2, imm), lexer);
        }
        private static InstructionNode<Kore.RiscMeta.Instructions.TypeB> ParseBInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // bne x1, x2, label
            // OP  rs1, rs2, offset

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeB>(currentToken, lexer, Lexer.Token.OP_B);
            var rs1 = ParseRegister(lexer); // Get the first source register (rs1)
            var rs2 = ParseRegister(lexer); // Get the second source register (rs2)

            var token = ExpectToken(lexer, true, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            switch(token.token) {
                case Lexer.Token.NUMBER_INT:
                case Lexer.Token.NUMBER_HEX:
                    return expectReturnEOL(new InstructionNodeTypeBImmidiate(op, rs1, rs2, ParseImmediate(lexer)), lexer);
                case Lexer.Token.IDENTIFIER:
                    return expectReturnEOL(new InstructionNodeTypeBLabel(op, rs1, rs2, lexer.ReadToken(true).value), lexer);
                default:
                    throw ThrowUnexpected(currentToken, "Label Identifier or Number");
            }

        }
        private static InstructionNode<Kore.RiscMeta.Instructions.TypeJ> ParseJInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // bne x1, x2, label
            // OP  rs1, rs2, offset

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeJ>(currentToken, lexer, Lexer.Token.OP_J);
            var rd = ParseRegister(lexer); // Get the destination register (rd)

            var token = ExpectToken(lexer, true, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            switch(token.token) {
                case Lexer.Token.NUMBER_INT:
                case Lexer.Token.NUMBER_HEX:
                    return expectReturnEOL(new InstructionNodeTypeJImmidiate(op, rd, ParseImmediate(lexer)), lexer);
                case Lexer.Token.IDENTIFIER:
                    return expectReturnEOL(new InstructionNodeTypeJLabel(op, rd, lexer.ReadToken(true).value), lexer);
                default:
                    throw ThrowUnexpected(currentToken, "Label Identifier or Number");
            }

        }

        private static InstructionNodeTypeU ParseUInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            var op = ParseOP<RiscMeta.Instructions.TypeU>(currentToken, lexer, Lexer.Token.OP_U);
            var rd = ParseRegister(lexer);
            var immediate = ParseImmediate(lexer);

            return expectReturnEOL(new InstructionNodeTypeU(op, rd, immediate), lexer);
        }

        private static AstNode ParseInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            switch (currentToken.token)
            {
                
                case Lexer.Token.OP_B:
                    return ParseBInstruction(currentToken, lexer);
                case Lexer.Token.OP_I:
                    return ParseIInstruction(currentToken, lexer);
                case Lexer.Token.OP_J:
                    return ParseJInstruction(currentToken, lexer);
                case Lexer.Token.OP_R:
                    return ParseRInstruction(currentToken, lexer);
                case Lexer.Token.OP_S:
                    return ParseSInstruction(currentToken, lexer);
                case Lexer.Token.OP_U:
                    return ParseUInstruction(currentToken, lexer);
                default:
                    throw ThrowUnimplemented(currentToken);
            }
        }


        private static SectionNode ParseNodeSection(SectionNode section, Lexer.TokenData currentToken, Lexer lexer) {
            // Get the next token
            //currentToken = lexer.ReadToken();// EOL
            // Expect EOL
            ExpectToken(lexer, Lexer.Token.EOL);

            while(true) {
                // Get the next token
                currentToken = lexer.ReadToken(true);

                if(
                    currentToken.token == Lexer.Token.EOF ||
                    (currentToken.token == Lexer.Token.DIRECTIVE && getSectionFromDirectiveToken(currentToken) != default(string))
                    ) return section;

                //TODO: Refactor to a switch statement
                switch(currentToken.token) {
                    case Lexer.Token.LABEL: // Label
                        section.Contents.Add(ParseNodeLabel(currentToken, lexer)); //TODO: Maybe should cascade into this processor and only process instructions there but that would mean no double labels though I don't know if thats valid anyhow.
                        continue;
                    case Lexer.Token.DIRECTIVE:
                        section.Contents.Add(ParseNodeDirective(currentToken, lexer)); // Directive
                        continue;
                    case Lexer.Token.OP_B:
                    case Lexer.Token.OP_CB:
                    case Lexer.Token.OP_CI:
                    case Lexer.Token.OP_CIW:
                    case Lexer.Token.OP_CJ:
                    case Lexer.Token.OP_CL:
                    case Lexer.Token.OP_CR:
                    case Lexer.Token.OP_CS:
                    case Lexer.Token.OP_CSS:
                    case Lexer.Token.OP_I:
                    case Lexer.Token.OP_J:
                    case Lexer.Token.OP_R:
                    case Lexer.Token.OP_S:
                    case Lexer.Token.OP_U:
                        section.Contents.Add(ParseInstruction(currentToken, lexer)); // Instruction
                        continue;
                    case Lexer.Token.OP_PSEUDO:
                        throw ThrowUnimplemented(currentToken);
                    case Lexer.Token.COMMENT:
                        section.Contents.Add(ProcessNodeComment(currentToken, lexer)); // Comment
                        continue;
                    default:
                        throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}.");
                }
            }

            //TODO: Read the other nodes and add them to the section
            
        }

        private static T expectReturnEOL<T>(T rt, Lexer lexer, bool ignoreWhitespace = true) {
            var currentToken = lexer.ReadToken(ignoreWhitespace);
            if(currentToken.token != Lexer.Token.EOL && currentToken.token != Lexer.Token.EOF) {
                throw ThrowUnexpected(currentToken, "EOL || EOF");
            }
            return rt;
        }

        private static string getSectionFromDirectiveToken(Lexer.TokenData currentToken) {
            var directiveValue = currentToken.value.ToLower();
            if(directiveValue.StartsWith(".section")) return directiveValue.Substring(9);
            if(StandardSections.Contains(directiveValue) || SpecialTextSections.Contains(directiveValue)) return directiveValue;
            return default(string);
        }

        private static SectionNode CreateSectionNode(string section, Lexer.TokenData currentToken, Lexer lexer) {
            return ParseNodeSection(new SectionNode(section), currentToken, lexer);
        }

        private static readonly string[] StandardSections = new string[] { ".text", ".data", ".rodata", ".bss", ".comment", ".debug" };
        private static readonly string[] SpecialTextSections = new string[] { ".text.startup", ".text.exit", ".text.hot", ".text.unlikely", ".text.cold" };

        private static AstNode ParseNodeDirective(Lexer.TokenData currentToken, Lexer lexer) {
            var name = currentToken.value;
            currentToken = lexer.ReadToken();

            if(currentToken.token == Lexer.Token.STRING || currentToken.token == Lexer.Token.IDENTIFIER) {
                return expectReturnEOL(new StringDirectiveNode { Name = name, Value = currentToken.value }, lexer);
            } else if(currentToken.token == Lexer.Token.NUMBER_INT) {
                var value = int.Parse(currentToken.value);
                return expectReturnEOL(new IntDirectiveNode { Name = name, Value = value }, lexer);
            } else if(currentToken.token == Lexer.Token.NUMBER_HEX) {
                var value = int.Parse(currentToken.value, System.Globalization.NumberStyles.HexNumber);
                return expectReturnEOL(new IntDirectiveNode { Name = name, Value = value }, lexer);
            } else {

                return expectReturnEOL(new DirectiveNode { Name = name }, lexer);
            }
        }

        public static ProgramNode Parse(Lexer lexer) {
            var programNode = new ProgramNode();

            Lexer.TokenData currentToken = default(Lexer.TokenData);
            while(currentToken.token != Lexer.Token.EOF) {
                // Get Next Token
                currentToken = lexer.ReadToken(true);

                if(currentToken.token == Lexer.Token.EOL || currentToken.token == Lexer.Token.EOF) continue;

                // Get section
                var directiveValue = getSectionFromDirectiveToken(currentToken);
                if(directiveValue != default(string)) {
                    programNode.Sections.Add(CreateSectionNode(directiveValue, currentToken, lexer));
                    continue;
                }

                throw ThrowUnexpected(currentToken, "Section");
            }

            return programNode;
        }
    }
}