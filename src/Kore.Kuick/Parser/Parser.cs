/*
*/
using Kore.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.Kuick {
    public static class Parser {
        public class SyntaxException : Exception { public SyntaxException(string msg) : base(msg) { } }
        private static Exception ThrowUnexpected(Lexer.TokenData currentToken, string expectation) {
            throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Expected {expectation}.");
        }
        private static Lexer.TokenData ExpectToken(Lexer lexer, Lexer.Token expectation, bool ignoreWhitespace = true) {
            var currentToken = lexer.ReadToken(ignoreWhitespace);

            if(currentToken.token != expectation) throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Expected {expectation.ToString()}.");

            return currentToken;
        }

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
            if(currentToken.token != Lexer.Token.OP_R) throw ThrowUnexpected(currentToken, "OP_R");

            // add x1, x2, x3
            // OP  rd, rs1, rs2

            //var OP = currentToken.value;
            if(Enum.TryParse(currentToken.value, true, out Kore.RiscMeta.Instructions.TypeR op) == false) {
                throw ThrowUnexpected(currentToken, "OP_R");
            }

            // Get Expect rd
            currentToken = ExpectToken(lexer, Lexer.Token.REGISTER);
            if(Enum.TryParse(currentToken.value, true, out Kore.RiscMeta.Register rd) == false) {
                throw ThrowUnexpected(currentToken, "Register");
            }

            // Get Expect rd1
            currentToken = ExpectToken(lexer, Lexer.Token.REGISTER);
            if(Enum.TryParse(currentToken.value, true, out Kore.RiscMeta.Register rd1) == false) {
                throw ThrowUnexpected(currentToken, "Register");
            }

            // Get Expect rd2
            currentToken = ExpectToken(lexer, Lexer.Token.REGISTER);
            if(Enum.TryParse(currentToken.value, true, out Kore.RiscMeta.Register rd2) == false) {
                throw ThrowUnexpected(currentToken, "Register");
            }

            return expectReturnEOL(new InstructionNodeTypeR(op, rd, rd1, rd2), lexer);
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

                // Label
                if(currentToken.token == Lexer.Token.LABEL) {
                    section.Contents.Add(ParseNodeLabel(currentToken, lexer)); //TODO: Maybe should cascade into this processor and only process instructions there but that would mean no double labels though I don't know if thats valid anyhow.
                    continue;
                }
                // Directive
                if(currentToken.token == Lexer.Token.DIRECTIVE) {
                    section.Contents.Add(ParseNodeDirective(currentToken, lexer));
                    continue;
                }
                // Instruction
                if(currentToken.token == Lexer.Token.OP_R) {
                    var instructionNode = ParseRInstruction(currentToken, lexer);
                    section.Contents.Add(instructionNode);
                    continue;
                }
                // Comment
                if(currentToken.token == Lexer.Token.COMMENT) {
                    section.Contents.Add(ProcessNodeComment(currentToken, lexer));
                    continue;
                }
                throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}.");
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