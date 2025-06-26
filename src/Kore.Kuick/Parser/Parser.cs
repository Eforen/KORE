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

            if(currentToken.token != expectation) throw new SyntaxException($"Unexpected token {currentToken.token}({currentToken.value}) at line {currentToken.lineNumber}, column {currentToken.columnNumber}. Expected {expectation.ToString()}.");

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

        private static T ParseOP<T>(Lexer lexer, Lexer.Token expectedToken)
        where T : struct, Enum {
            Lexer.TokenData currentToken = ExpectToken(lexer, expectedToken);
            return ParseOP<T>(currentToken, lexer, expectedToken);
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
        private static int ParseControlStatusRegister(Lexer lexer) {
            var currentToken = ExpectToken(lexer, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX, Lexer.Token.IDENTIFIER, Lexer.Token.CSR);
            if(currentToken.token == Lexer.Token.NUMBER_INT) return int.Parse(currentToken.value);
            if(currentToken.token == Lexer.Token.NUMBER_HEX) return Convert.ToInt32(currentToken.value, 16);
            if(currentToken.token == Lexer.Token.IDENTIFIER || currentToken.token == Lexer.Token.CSR) {
                // Convert CSR names to their numeric values using the CSR enum
                if(Enum.TryParse(currentToken.value, true, out Kore.RiscMeta.ControlStateRegisters.CSR csr)) {
                    return (int)csr;
                } else {
                    throw ThrowUnexpected(currentToken, $"Known CSR name. '{currentToken.value}' is not a recognized CSR name.");
                }
            }
            throw ThrowParserPanic(currentToken);
        }

        private static int ParseImmediate(Lexer lexer) {
            var currentToken = ExpectToken(lexer, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            if(currentToken.token == Lexer.Token.NUMBER_INT) return int.Parse(currentToken.value);
            if(currentToken.token == Lexer.Token.NUMBER_HEX) return Convert.ToInt32(currentToken.value, 16);
            throw ThrowParserPanic(currentToken);
        }

        #endregion


        private static AstNode[] ParseNodeLabel(Lexer lexer) {
            Lexer.TokenData currentToken = ExpectToken(lexer, Lexer.Token.LABEL);
            return ParseNodeLabel(currentToken, lexer);
        }

        private static AstNode[] ParseNodeLabel(Lexer.TokenData currentToken, Lexer lexer) {
            if(currentToken.token != Lexer.Token.LABEL) throw ThrowUnexpected(currentToken, "LABEL");
            // Make the node
            var labelNode = new LabelNode(currentToken.value);

            // Get the next token
            currentToken = lexer.ReadToken();

            // If not the end of file or line
            // Check EOL first because its more likely
            if(currentToken.token != Lexer.Token.EOL && currentToken.token != Lexer.Token.EOF) {
                throw ThrowUnexpected(currentToken, "EOL || EOF");
            }
            return ComposeInstructionArray(labelNode);
        }

        private static AstNode[] ProcessNodeComment(Lexer.TokenData currentToken, Lexer lexer) {
            if(currentToken.token != Lexer.Token.LABEL) throw ThrowUnexpected(currentToken, "LABEL");
            // Make the node
            var labelNode = new CommentNode(currentToken.value);

            // Get the next token
            currentToken = lexer.ReadToken();

            // If not the end of file or line
            // Check EOL first because its more likely
            if(currentToken.token != Lexer.Token.EOL && currentToken.token != Lexer.Token.EOF) {
                throw ThrowUnexpected(currentToken, "EOL || EOF");
            }
            return ComposeInstructionArray(labelNode);
        }

        private static AstNode[] ParseRInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // add x1, x2, x3
            // OP  rd, rs1, rs2

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeR>(currentToken, lexer, Lexer.Token.OP_R);
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            var rd1 = ParseRegister(lexer); // Get the destination register (rd1)
            var rd2 = ParseRegister(lexer); // Get the destination register (rd2)

            return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeR(op, rd, rd1, rd2), lexer));
        }


        private static AstNode[] ParseIInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // addi x1, x2, 15
            // OP  rd, rs, imm
            // OR
            // LX rd, imm(rs)
            var originalCursor = lexer._cursor;

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeI>(currentToken, lexer, Lexer.Token.OP_I);
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            Register rs = Register.x0; // Storage Point for later use
            int imm = 0; // Storage Point for later use
            switch(op) {
                case RiscMeta.Instructions.TypeI.lb:
                case RiscMeta.Instructions.TypeI.lbu:
                case RiscMeta.Instructions.TypeI.ld:
                case RiscMeta.Instructions.TypeI.lh:
                case RiscMeta.Instructions.TypeI.lhu:
                case RiscMeta.Instructions.TypeI.lw:
                case RiscMeta.Instructions.TypeI.lwu:
                    if(lexer.PeakToken().token == Lexer.Token.IDENTIFIER){
                        // This means its the pseudo instruction
                        // LX rd, symbol
                        // Reset the lexer cursor
                        lexer._cursor = originalCursor;
                        // Send to pseudo instruction parser
                        return ParsePseudoInstruction(currentToken, lexer);
                    }
                    LabeledInlineDirectiveNode<InstructionNodeTypeI> wrapper = null;
                    if(PeakLabelInlineDirective(lexer)){
                        wrapper = ParseNWrapLabelInlineDirective(lexer, new InstructionNodeTypeI(op, rd, rs, 0));
                    } else {
                        imm = ParseImmediate(lexer); // Get the immediate value, which can be in decimal or hexadecimal format
                    }
                    ExpectToken(lexer, Lexer.Token.PARREN_OPEN);
                    var rs1 = ParseRegister(lexer);
                    ExpectToken(lexer, Lexer.Token.PARREN_CLOSE);
                    if(wrapper != null){
                        (wrapper.WrappedInstruction as InstructionNodeTypeI).rs = rs1;
                        return ComposeInstructionArray(expectReturnEOL(wrapper, lexer));
                    }
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(op, rd, rs1, imm), lexer));
                case RiscMeta.Instructions.TypeI.csrrc:
                case RiscMeta.Instructions.TypeI.csrrs:
                case RiscMeta.Instructions.TypeI.csrrw:
                    imm = ParseControlStatusRegister(lexer); // Get the immediate value, which can be in decimal or hexadecimal format
                    rs = ParseRegister(lexer); // Get the source register (rs)
                    break;
                case RiscMeta.Instructions.TypeI.csrrci:
                case RiscMeta.Instructions.TypeI.csrrsi:
                case RiscMeta.Instructions.TypeI.csrrwi:
                    imm = ParseControlStatusRegister(lexer); // Get the immediate value, which can be in decimal or hexadecimal format
                    int zimm = (ParseImmediate(lexer)) & 0b11111; // Get the immidiate value (zimm[4:0])
                    break;
                default:
                    rs = ParseRegister(lexer); // Get the source register (rs)
                    if(PeakLabelInlineDirective(lexer)) {
                        return ComposeInstructionArray(expectReturnEOL(ParseNWrapLabelInlineDirective(lexer, new InstructionNodeTypeI(op, rd, rs, 0)), lexer));
                    }
                    imm = ParseImmediate(lexer); // Get the immediate value, which can be in decimal or hexadecimal format
                    break;
            }
            return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(op, rd, rs, imm), lexer));
        }
        private static AstNode[] ParseSInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            var originalCursor = lexer._cursor;

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeS>(currentToken, lexer, Lexer.Token.OP_S);

            var rs2 = ParseRegister(lexer);
            
            if(lexer.PeakToken().token == Lexer.Token.IDENTIFIER){
                // This means its the pseudo instruction
                // SX rd, symbol
                // Reset the lexer cursor
                lexer._cursor = originalCursor;
                // Send to pseudo instruction parser
                return ParsePseudoInstruction(currentToken, lexer);
            }

            LabeledInlineDirectiveNode<InstructionNodeTypeS> labelInlineDirective = null;
            int imm = 0;
            if(PeakLabelInlineDirective(lexer)){
                labelInlineDirective = ParseNWrapLabelInlineDirective<InstructionNodeTypeS>(lexer, null);
            } else {
                imm = ParseImmediate(lexer);
            }
            ExpectToken(lexer, Lexer.Token.PARREN_OPEN);
            var rs1 = ParseRegister(lexer);
            ExpectToken(lexer, Lexer.Token.PARREN_CLOSE);
            
            var node = new InstructionNodeTypeS(op, rs1, rs2, imm);
            if(labelInlineDirective != null){
                labelInlineDirective.WrappedInstruction = node;
                return ComposeInstructionArray(expectReturnEOL(labelInlineDirective, lexer));
            }

            return ComposeInstructionArray(expectReturnEOL(node, lexer));
        }
        private static AstNode[] ParseBInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // bne x1, x2, label
            // OP  rs1, rs2, offset

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeB>(currentToken, lexer, Lexer.Token.OP_B);
            var rs1 = ParseRegister(lexer); // Get the first source register (rs1)
            var rs2 = ParseRegister(lexer); // Get the second source register (rs2)

            if(PeakLabelInlineDirective(lexer)){
                return ComposeInstructionArray(expectReturnEOL(ParseNWrapLabelInlineDirective<InstructionNodeTypeBImmediate>(lexer, new InstructionNodeTypeBImmediate(op, rs1, rs2, 0)), lexer));
            }

            var token = ExpectToken(lexer, true, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            switch(token.token) {
                case Lexer.Token.NUMBER_INT:
                case Lexer.Token.NUMBER_HEX:
                case Lexer.Token.NUMBER_BIN:
                // case Lexer.Token.NUMBER_OCT:
                case Lexer.Token.NUMBER_DOUBLE: //TODO: Evaluate if we want to support doubles in another way
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeBImmediate(op, rs1, rs2, ParseImmediate(lexer)), lexer));
                case Lexer.Token.IDENTIFIER:
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeBLabel(op, rs1, rs2, lexer.ReadToken(true).value), lexer));
                default:
                    throw ThrowUnexpected(currentToken, "Label Identifier or Number");
            }

        }
        private static AstNode[] ParseJInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            // bne x1, x2, label
            // OP  rs1, rs2, offset

            var op = ParseOP<Kore.RiscMeta.Instructions.TypeJ>(currentToken, lexer, Lexer.Token.OP_J);
            var rd = ParseRegister(lexer); // Get the destination register (rd)

            if(PeakLabelInlineDirective(lexer)){
                return ComposeInstructionArray(expectReturnEOL(ParseNWrapLabelInlineDirective<InstructionNodeTypeJImmediate>(lexer, new InstructionNodeTypeJImmediate(op, rd, 0)), lexer));
            }

            var token = ExpectToken(lexer, true, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            switch(token.token) {
                case Lexer.Token.NUMBER_INT:
                case Lexer.Token.NUMBER_HEX:
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeJImmediate(op, rd, ParseImmediate(lexer)), lexer));
                case Lexer.Token.IDENTIFIER:
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeJLabel(op, rd, lexer.ReadToken(true).value), lexer));
                default:
                    throw ThrowUnexpected(currentToken, "Label Identifier or Number");
            }

        }

        private static AstNode[] ParseUInstruction(Lexer.TokenData currentToken, Lexer lexer) {
            var op = ParseOP<RiscMeta.Instructions.TypeU>(currentToken, lexer, Lexer.Token.OP_U);
            var rd = ParseRegister(lexer);
            if(PeakLabelInlineDirective(lexer)){
                return ComposeInstructionArray(expectReturnEOL(ParseNWrapLabelInlineDirective(lexer, new InstructionNodeTypeU(op, rd, 0)), lexer));
            }
            var immediate = ParseImmediate(lexer);

            return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeU(op, rd, immediate), lexer));
        }

        private static bool PeakLabelInlineDirective(Lexer lexer) {
            var token = lexer.PeakToken();
            return token.token == Lexer.Token.INLINE_DIRECTIVE;
        }

        private static LabeledInlineDirectiveNode<T> ParseNWrapLabelInlineDirective<T>(Lexer lexer, T instruction) where T : InstructionNode{
            // var directive = ExpectToken(lexer, Lexer.Token.INLINE_DIRECTIVE);
            var op = ParseOP<InlineDirectiveNode.InlineDirectiveType>(lexer, Lexer.Token.INLINE_DIRECTIVE);
            ExpectToken(lexer, Lexer.Token.PARREN_OPEN); // (
            var label = ExpectToken(lexer, Lexer.Token.IDENTIFIER);
            ExpectToken(lexer, Lexer.Token.PARREN_CLOSE); // )
            return new LabeledInlineDirectiveNode<T>(){
                Name = op,
                Label = label.value,
                WrappedInstruction = instruction};
        }

        private static AstNode[] ParseInstruction(Lexer.TokenData currentToken, Lexer lexer) {
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
                case Lexer.Token.OP_PSEUDO:
                    return ParsePseudoInstruction(currentToken, lexer);
                default:
                    throw ThrowUnimplemented(currentToken);
            }
        }

        private static AstNode[] ComposeInstructionArray(params AstNode[] nodes){
            return nodes;
        }

        private static AstNode[] ParsePseudoInstruction(Lexer.TokenData currentToken, Lexer lexer) {

            switch(currentToken.value.ToUpper()){
                case "NOP": // Pseduo instruction: NOP -> ADDI x0, x0, 0 [TYPE I]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(RiscMeta.Instructions.TypeI.addi, Register.x0, Register.x0, 0), lexer));
                case "NEG": // Pseduo instruction: NEG rd, rs -> SUB rd, x0, rs [TYPE R]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeR(RiscMeta.Instructions.TypeR.sub, ParseRegister(lexer), Register.x0, ParseRegister(lexer)), lexer));
                // TODO: RV54I
                // case "NEGW": // Pseduo instruction: NEGW rd, rs -> SUBW rd, x0, rs [TYPE R][RV64I]
                //     return expectReturnEOL(new InstructionNodeTypeR(RiscMeta.Instructions.TypeR.subw, ParseRegister(lexer), Register.x0, ParseRegister(lexer)), lexer);
                ////////////////////////////////////////////////////////////////////////
                case "SNEZ": // Pseduo instruction: SNEZ rd, rs -> SLTU rd, x0, rs [TYPE R]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeR(RiscMeta.Instructions.TypeR.sltu, ParseRegister(lexer), Register.x0, ParseRegister(lexer)), lexer));
                case "SLTZ": // Pseduo instruction: SLTZ rd, rs -> SLT rd, rs, x0 [TYPE R]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeR(RiscMeta.Instructions.TypeR.slt, ParseRegister(lexer), ParseRegister(lexer), Register.x0), lexer));
                case "SGTZ": // Pseduo instruction: SGTZ rd, rs -> SLT rd, x0, rs [TYPE R]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeR(RiscMeta.Instructions.TypeR.slt, ParseRegister(lexer), Register.x0, ParseRegister(lexer)), lexer));
                ////////////////////////////////////////////////////////////////////////
                case "BEQZ": // Pseduo instruction: BEQZ rs, offset/label -> BEQ rs, x0, offset/label [TYPE B]
                    return ComposeInstructionArray(ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB.beq, true, lexer));
                case "BNEZ": // Pseduo instruction: BNEZ rs, offset/label -> BNE rs, x0, offset/label [TYPE B]
                    return ComposeInstructionArray(ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB.bne, true, lexer));
                case "BLEZ": // Pseduo instruction: BLEZ rs, offset/label -> BGE x0, rs, offset/label [TYPE B]
                    return ComposeInstructionArray(ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB.bge, false, lexer));
                case "BGEZ": // Pseduo instruction: BGEZ rs, offset/label -> BGE rs, x0, offset/label [TYPE B]
                    return ComposeInstructionArray(ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB.bge, true, lexer));
                case "BLTZ": // Pseduo instruction: BLTZ rs, offset/label -> BLT rs, x0, offset/label [TYPE B]
                    return ComposeInstructionArray(ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB.blt, true, lexer));
                case "BGTZ": // Pseduo instruction: BGTZ rs, offset/label -> BLT x0, rs, offset/label [TYPE B]
                    return ComposeInstructionArray(ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB.blt, false, lexer));
                ////////////////////////////////////////////////////////////////////////
                case "RET": // Pseduo instruction: RET -> JALR x0, 0(x1) [TYPE I] OP rd, offset(rs1)
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(RiscMeta.Instructions.TypeI.jalr, Register.x0, Register.x1, 0), lexer));
                ////////////////////////////////////////////////////////////////////////
                case "J": // Pseduo instruction: J offset/label -> JAL x0, offset/label [TYPE J]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeJImmediate(RiscMeta.Instructions.TypeJ.jal, Register.x0, ParseImmediate(lexer)), lexer));
                case "JR": // Pseduo instruction: JR rs -> JALR x0, 0(rs) [TYPE I]
                    return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(RiscMeta.Instructions.TypeI.jalr, Register.x0, ParseRegister(lexer), 0), lexer));
                ////////////////////////////////////////////////////////////////////////
                case "LB":
                    return ParsePseudoInstructionLoad(currentToken, RiscMeta.Instructions.TypeI.lb, lexer);
                case "LH":
                    return ParsePseudoInstructionLoad(currentToken, RiscMeta.Instructions.TypeI.lh, lexer);
                case "LW":
                    return ParsePseudoInstructionLoad(currentToken, RiscMeta.Instructions.TypeI.lw, lexer);
                case "LD":
                    return ParsePseudoInstructionLoad(currentToken, RiscMeta.Instructions.TypeI.ld, lexer);
                ////////////////////////////////////////////////////////////////////////
                case "SB":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeS.sb, lexer);
                case "SH":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeS.sh, lexer);
                case "SW":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeS.sw, lexer);
                case "SD": 
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeS.sd, lexer);
                ////////////////////////////////////////////////////////////////////////
                case "FLW":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeI.flw, lexer);
                case "FLD":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeI.fld, lexer);
                case "FSW":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeS.fsw, lexer);
                case "FSD":
                    return ParsePseudoInstructionStoreAndFloats(currentToken, RiscMeta.Instructions.TypeS.fsd, lexer);
                ////////////////////////////////////////////////////////////////////////
                case "CSRR":
                    return ParsePseudoInstructionCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, lexer);
                case "CSRW":
                    return ParsePseudoInstructionCSR(currentToken, RiscMeta.Instructions.TypeI.csrrw, lexer);
                case "CSRS":
                    return ParsePseudoInstructionCSR(currentToken, RiscMeta.Instructions.TypeI.csrrs, lexer);
                case "CSRC":
                    return ParsePseudoInstructionCSR(currentToken, RiscMeta.Instructions.TypeI.csrrc, lexer);
                case "CSRCI":
                    return ParsePseudoInstructionCSRImmediate(currentToken, RiscMeta.Instructions.TypeI.csrrci, lexer);
                case "CSRWI":
                    return ParsePseudoInstructionCSRImmediate(currentToken, RiscMeta.Instructions.TypeI.csrrwi, lexer);
                case "CSRSI":
                    return ParsePseudoInstructionCSRImmediate(currentToken, RiscMeta.Instructions.TypeI.csrrsi, lexer);
                case "RDCYCLE": // Pseduo instruction: RDCYCLE rd -> CSRRS rd, cycle, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "cycle", lexer);
                case "RDCYCLEH": // Pseduo instruction: RDCYCLEH rd -> CSRRS rd, cycleh, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "cycleh", lexer);
                case "RDTIME": // Pseduo instruction: RDTIME rd -> CSRRS rd, time, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "time", lexer);
                case "RDTIMEH": // Pseduo instruction: RDTIMEH rd -> CSRRS rd, timeh, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "timeh", lexer);
                case "RDINSTRET": // Pseduo instruction: RDINSTRET rd -> CSRRS rd, instret, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "instret", lexer);
                case "RDINSTRETH": // Pseduo instruction: RDINSTRETH rd -> CSRRS rd, instreth, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "instreth", lexer);
                ////////////////////////////////////////////////////////////////////////
                case "FRCSR": // Pseudo instruction: FRCSR rd -> CSRRS rd, fcsr, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "fcsr", lexer);
                case "FSCSR": // Pseudo instruction: FSCSR rd -> CSRRS x0, fcsr, rd [TYPE I]
                    return ParsePseudoInstructionSpecificCSRWrite(currentToken, RiscMeta.Instructions.TypeI.csrrs, "fcsr", lexer);
                case "FRRM": // Pseudo instruction: FRRM rd -> CSRRS rd, frm, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "frm", lexer);
                case "FSRM": // Pseudo instruction: FSRM rd -> CSRRS x0, frm, rd [TYPE I]
                    return ParsePseudoInstructionSpecificCSRWrite(currentToken, RiscMeta.Instructions.TypeI.csrrs, "frm", lexer);
                case "FRFLAGS": // Pseudo instruction: FRFLAGS rd -> CSRRS rd, fflags, x0 [TYPE I]
                    return ParsePseudoInstructionSpecificCSRRead(currentToken, RiscMeta.Instructions.TypeI.csrrs, "fflags", lexer);
                case "FSFLAGS": // Pseudo instruction: FSFLAGS rd -> CSRRS x0, fflags, rd [TYPE I]
                    return ParsePseudoInstructionSpecificCSRWrite(currentToken, RiscMeta.Instructions.TypeI.csrrs, "fflags", lexer);
                case "FSRMI": // Pseudo instruction: FSRMI imm -> CSRRWI x0, frm, imm [TYPE I]
                    return ParsePseudoInstructionSpecificCSRWriteImmediate(currentToken, RiscMeta.Instructions.TypeI.csrrwi, "frm", lexer);
                case "FSFLAGSI": // Pseudo instruction: FSFLAGSI imm -> CSRRWI x0, fflags, imm [TYPE I]
                    return ParsePseudoInstructionSpecificCSRWriteImmediate(currentToken, RiscMeta.Instructions.TypeI.csrrwi, "fflags", lexer);
                default:
                    throw ThrowUnimplemented(currentToken);
            }
        }

        /// <summary>
        /// Psudo REGISTER SYMBOL
        /// </summary>
        /// <param name="startToken"></param>
        /// <param name="op"></param>
        /// <param name="lexer"></param>
        /// <returns></returns>
        private static AstNode[] ParsePseudoInstructionLoad(Lexer.TokenData startToken, RiscMeta.Instructions.TypeI op, Lexer lexer) {
            // Format Example: LB rd, symbol, rt
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            var symbol = ExpectToken(lexer, Lexer.Token.IDENTIFIER); // Get the symbol
            // Create the instruction node for the auipc instruction
            // auipc rt, symbol[31:12]
            // auipc rt, %pcrel_hi(symbol)
            var command1 = new LabeledInlineDirectiveNode<InstructionNodeTypeU>() {
                Name = InlineDirectiveNode.InlineDirectiveType.PCREL_HI,
                Label = symbol.value,
                WrappedInstruction = new InstructionNodeTypeU(
                    RiscMeta.Instructions.TypeU.auipc,
                    rd,
                    0
                )
            };

            // Create the node wrapper for the load instruction
            // op rd, symbol[11:0](rt)
            // op rd, %pcrel_lo(symbol)(rt)
            var command2 = new LabeledInlineDirectiveNode<InstructionNodeTypeI>() {
                Name = InlineDirectiveNode.InlineDirectiveType.PCREL_LO,
                Label = symbol.value,
                WrappedInstruction = new InstructionNodeTypeI(
                    op,
                    rd,
                    rd,
                    0
                )
            };

            return ComposeInstructionArray(command1, expectReturnEOL(command2, lexer));
        }

        /// <summary>
        /// Pseudo CSR read instruction parser that handles "csrr rd, csr" format
        /// Format: csrr rd, csr -> csrrs rd, csr, x0
        /// </summary>
        /// <param name="startToken"></param>
        /// <param name="op"></param>
        /// <param name="lexer"></param>
        /// <returns></returns>
        private static AstNode[] ParsePseudoInstructionCSRRead(Lexer.TokenData startToken, RiscMeta.Instructions.TypeI op, Lexer lexer) {
            // Parse the destination register first
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            
            // Parse the CSR using the existing method that handles CSR name to number conversion
            int csrValue = ParseControlStatusRegister(lexer);
            
            // Create the instruction: csrrs rd, csr, x0
            return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(
                op,
                rd,
                Register.x0,
                csrValue
            ), lexer));
        }

        /// <summary>
        /// Pseudo CSR instruction parser that handles both CSR tokens and numerical CSR addresses
        /// Format: csrs <csr>, <register> -> csrrs x0, <csr>, <register>
        /// </summary>
        /// <param name="startToken"></param>
        /// <param name="op"></param>
        /// <param name="lexer"></param>
        /// <returns></returns>
        private static AstNode[] ParsePseudoInstructionCSR(Lexer.TokenData startToken, RiscMeta.Instructions.TypeI op, Lexer lexer) {
            // Parse the CSR (can be either a CSR token like "cycle" or a number like 0x7C0)
            var csrToken = ExpectToken(lexer, Lexer.Token.CSR, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            int csrValue = 0;
            
            switch(csrToken.token) {
                case Lexer.Token.NUMBER_INT:
                    csrValue = Convert.ToInt32(csrToken.value, 10);
                    break;
                case Lexer.Token.NUMBER_HEX:
                    csrValue = Convert.ToInt32(csrToken.value, 16);
                    break;
                case Lexer.Token.CSR:
                case Lexer.Token.IDENTIFIER:
                    // For CSR tokens, we need to get the numeric value
                    // For now, we'll create a fake instruction and let the normal CSR instruction parser handle it
                    var register = ParseRegister(lexer); // Get the source register
                    
                    // Create fake tokens to simulate "csrrs x0, <csr>, <register>"
                    // We'll create a new lexer with the transformed instruction
                    var transformedInstruction = $"csrrs x0, {csrToken.value}, {register}";
                    var fakeLexer = new Lexer();
                    fakeLexer.Load(".text\n" + transformedInstruction);
                    
                    // Skip the .text directive
                    fakeLexer.ReadToken(); // .text
                    fakeLexer.ReadToken(); // EOL
                    
                    // Parse as a normal I-type instruction
                    var fakeToken = fakeLexer.ReadToken(); // csrrs
                    return ParseIInstruction(fakeToken, fakeLexer);
                default:
                    throw ThrowUnexpected(csrToken, "CSR name or CSR address");
            }
            
            // Handle numeric CSR case
            var rt = ParseRegister(lexer); // Get the source register
            
            return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(
                op,
                Register.x0,
                rt,
                csrValue
            ), lexer));
        }

        private static AstNode[] ParsePseudoInstructionStoreAndFloats<T>(Lexer.TokenData startToken, T op, Lexer lexer) where T : Enum{
            // Format Example: SB rd, symbol, rt
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            var symbol = ExpectToken(lexer, Lexer.Token.IDENTIFIER); // Get the symbol
            var rt = ParseRegister(lexer); // Get the source register (rt)
            // Create the instruction node for the auipc instruction
            // auipc rt, symbol[31:12]
            // auipc rt, %pcrel_hi(symbol)
            var command1 = new LabeledInlineDirectiveNode<InstructionNodeTypeU>(){ 
                Name = InlineDirectiveNode.InlineDirectiveType.PCREL_HI,
                Label = symbol.value,
                WrappedInstruction = new InstructionNodeTypeU(
                    RiscMeta.Instructions.TypeU.auipc, 
                    rt, 
                    0
                )
            };

            // Create the node wrapper for the store instruction
            // op rd, symbol[11:0](rt)
            // op rd, %pcrel_lo(symbol)(rt)
            var command2 = new LabeledInlineDirectiveNode<InstructionNode<T>>(){ 
                Name = InlineDirectiveNode.InlineDirectiveType.PCREL_LO,
                Label = symbol.value,
                WrappedInstruction = null
            };

            // Create the actual instruction node for the store instruction
            // Pattern matching on generic T is not supported before C# 7.4
            // Using if statements instead
            if (Enum.TryParse(startToken.value.ToLower(), out RiscMeta.Instructions.TypeS sTypeOP)){
                // sb rd, symbol[11:0](rt)
                command2.WrappedInstruction = new InstructionNodeTypeS(
                    sTypeOP, 
                    rt, 
                    rd, 
                    0
                );
            } else if (Enum.TryParse(startToken.value.ToLower(), out RiscMeta.Instructions.TypeI iTypeOP)){
                // flw rd, symbol[11:0](rt)
                // fsw rd, symbol[11:0](rt)
                // converts to
                // f[l/s][w/d] rs2, offset(rs1)
                command2.WrappedInstruction = new InstructionNodeTypeI(
                    iTypeOP, 
                    rd, 
                    rt, 
                    0
                );
            } else {
                throw ThrowUnimplemented(startToken);
            }

            return ComposeInstructionArray( command1, expectReturnEOL(command2, lexer));
        }

        private static AstNode ParsePseudoInstructionBType(RiscMeta.Instructions.TypeB op, bool firstRegister, Lexer lexer){
            var rs1 = ParseRegister(lexer); // Get the first source register (rs1)
            // Skip the second register because its hardcoded to x0
            var token = ExpectToken(lexer, true, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            switch(token.token) {
                case Lexer.Token.NUMBER_INT:
                case Lexer.Token.NUMBER_HEX:
                    return expectReturnEOL(new InstructionNodeTypeBImmediate(
                        op, 
                        firstRegister == true ? rs1 : Register.x0, 
                        firstRegister == false ? rs1 : Register.x0, 
                        ParseImmediate(lexer)
                    ), lexer);
                case Lexer.Token.IDENTIFIER:
                    return expectReturnEOL(new InstructionNodeTypeBLabel(
                        op, 
                        firstRegister == true ? rs1 : Register.x0, 
                        firstRegister == false ? rs1 : Register.x0, 
                        lexer.ReadToken(true).value
                    ), lexer);
                default:
                    throw ThrowUnexpected(token, "Label Identifier or Offset Number");
            }
        }
        private static SectionNode ParseNodeSection(SectionNode section, Lexer.TokenData currentToken, Lexer lexer) {
            // Get the next token
            //currentToken = lexer.ReadToken();// EOL
            // Expect EOL
            ExpectToken(lexer, Lexer.Token.EOL);

            while(true) {
                // Check if the next token is a section directive without consuming it
                var peekToken = lexer.PeakToken(true);
                if(peekToken.token == Lexer.Token.EOF) {
                    return section;
                }
                
                // If the next token is a section directive, don't consume it - let the main parser handle it
                if(peekToken.token == Lexer.Token.DIRECTIVE && getSectionFromDirectiveToken(peekToken) != default(string)) {
                    return section;
                }
                
                // Now actually consume the token since we know it's not a section directive
                currentToken = lexer.ReadToken(true);

                AstNode[] newNodes = null;
                //TODO: Refactor to a switch statement
                switch(currentToken.token) {
                    case Lexer.Token.LABEL: // Label
                        newNodes = ParseNodeLabel(currentToken, lexer); //TODO: Maybe should cascade into this processor and only process instructions there but that would mean no double labels though I don't know if thats valid anyhow.
                        break;
                    case Lexer.Token.DIRECTIVE:
                        newNodes = ParseNodeDirective(currentToken, lexer); // Directive
                        break;
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
                        newNodes = ParseInstruction(currentToken, lexer); // Instruction
                        break;
                    case Lexer.Token.OP_PSEUDO:
                        newNodes = ParsePseudoInstruction(currentToken, lexer);
                        break;
                    case Lexer.Token.COMMENT:
                        newNodes = ProcessNodeComment(currentToken, lexer); // Comment
                        break;
                    default:
                        throw new SyntaxException($"Unexpected token {currentToken.token} at line {currentToken.lineNumber}, column {currentToken.columnNumber}.");
                }
                foreach(var instruction in newNodes) {
                    section.Contents.Add(instruction);
                }
                continue;
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

        private static AstNode[] ParseNodeDirective(Lexer.TokenData currentToken, Lexer lexer) {
            var name = currentToken.value;
            currentToken = lexer.ReadToken(true); // Skip whitespace when reading the next token

            // Handle symbol directives (.global and .local)
            if (name == ".global" || name == ".local") {
                if (currentToken.token == Lexer.Token.IDENTIFIER) {
                    var symbolName = currentToken.value;
                    var directiveType = name == ".global" ? 
                        SymbolDirectiveNode.DirectiveType.Global : 
                        SymbolDirectiveNode.DirectiveType.Local;
                    
                    return ComposeInstructionArray(expectReturnEOL(new SymbolDirectiveNode(directiveType, symbolName), lexer));
                } else {
                    throw new SyntaxException($"Expected symbol name after {name} directive");
                }
            }

            if(currentToken.token == Lexer.Token.STRING || currentToken.token == Lexer.Token.IDENTIFIER) {
                return ComposeInstructionArray(expectReturnEOL(new StringDirectiveNode { Name = name, Value = currentToken.value }, lexer));
            } else if(currentToken.token == Lexer.Token.NUMBER_INT) {
                var value = int.Parse(currentToken.value);
                return ComposeInstructionArray(expectReturnEOL(new IntDirectiveNode { Name = name, Value = value }, lexer));
            } else if(currentToken.token == Lexer.Token.NUMBER_HEX) {
                var value = int.Parse(currentToken.value, System.Globalization.NumberStyles.HexNumber);
                return ComposeInstructionArray(expectReturnEOL(new IntDirectiveNode { Name = name, Value = value }, lexer));
            } else {

                return ComposeInstructionArray(expectReturnEOL(new DirectiveNode { Name = name }, lexer));
            }
        }

        /// <summary>
        /// Pseudo CSR immediate instruction parser that handles both CSR tokens and numerical CSR addresses
        /// Format: csrwi/csrsi/csrci <csr>, <zimm> -> csrrwi/csrrsi/csrrci x0, <csr>, <zimm>
        /// </summary>
        /// <param name="startToken"></param>
        /// <param name="op"></param>
        /// <param name="lexer"></param>
        /// <returns></returns>
        private static AstNode[] ParsePseudoInstructionCSRImmediate(Lexer.TokenData startToken, RiscMeta.Instructions.TypeI op, Lexer lexer) {
            // Parse the CSR (can be either a CSR token like "cycle" or a number like 0x7C0)
            var csrToken = ExpectToken(lexer, Lexer.Token.CSR, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            
            // Parse the immediate value (zimm)
            var zimm = ParseImmediate(lexer);
            
            // Create fake tokens to simulate "csrrwi/csrrsi/csrrci x0, <csr>, <zimm>"
            var opName = startToken.value.ToLower();
            string fullOpName;
            if (opName == "csrwi") fullOpName = "csrrwi";
            else if (opName == "csrsi") fullOpName = "csrrsi"; 
            else if (opName == "csrci") fullOpName = "csrrci";
            else throw new SyntaxException($"Unknown CSR immediate instruction: {opName}");
            
            var transformedInstruction = $"{fullOpName} x0, {csrToken.value}, {zimm}";
            var fakeLexer = new Lexer();
            fakeLexer.Load(".text\n" + transformedInstruction);
            
            // Skip the .text directive
            fakeLexer.ReadToken(); // .text
            fakeLexer.ReadToken(); // EOL
            
            // Parse as a normal I-type instruction
            var fakeToken = fakeLexer.ReadToken(); // csrrwi/csrrsi/csrrci
            return ParseIInstruction(fakeToken, fakeLexer);
        }

        private static AstNode[] ParsePseudoInstructionSpecificCSRRead(Lexer.TokenData currentToken, RiscMeta.Instructions.TypeI op, string csrName, Lexer lexer) {
            // Parse the destination register first
            var rd = ParseRegister(lexer); // Get the destination register (rd)
            
            // Convert the hardcoded CSR name to its numeric value using the CSR enum
            if(Enum.TryParse(csrName, true, out Kore.RiscMeta.ControlStateRegisters.CSR csr)) {
                int csrValue = (int)csr;
                
                // Create the instruction: csrrs rd, csr, x0
                return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(
                    op,
                    rd,
                    Register.x0,
                    csrValue
                ), lexer));
            } else {
                throw new SyntaxException($"Unknown CSR name: {csrName}");
            }
        }

        private static AstNode[] ParsePseudoInstructionSpecificCSRWrite(Lexer.TokenData currentToken, RiscMeta.Instructions.TypeI op, string csrName, Lexer lexer) {
            // Parse the source register 
            var rs = ParseRegister(lexer); // Get the source register (rs)
            
            // Convert the hardcoded CSR name to its numeric value using the CSR enum
            if(Enum.TryParse(csrName, true, out Kore.RiscMeta.ControlStateRegisters.CSR csr)) {
                int csrValue = (int)csr;
                
                // Create the instruction: csrrs x0, csr, rs
                return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(
                    op,
                    Register.x0,  // destination = x0
                    rs,           // source register 
                    csrValue      // CSR value
                ), lexer));
            } else {
                throw new SyntaxException($"Unknown CSR name: {csrName}");
            }
        }

        private static AstNode[] ParsePseudoInstructionSpecificCSRWriteImmediate(Lexer.TokenData currentToken, RiscMeta.Instructions.TypeI op, string csrName, Lexer lexer) {
            // Parse the immediate value (zimm)
            var zimm = ParseImmediate(lexer);
            
            // Convert the hardcoded CSR name to its numeric value using the CSR enum
            if(Enum.TryParse(csrName, true, out Kore.RiscMeta.ControlStateRegisters.CSR csr)) {
                int csrValue = (int)csr;
                
                // For CSR immediate instructions, the CSR address goes in bits [31:20] and immediate in [19:15]
                // We encode them in the immediate field: (csr << 20) | (zimm & 0x1F)
                int encodedImmediate = (csrValue << 20) | (zimm & 0x1F);
                
                // Create the instruction: csrrwi x0, csr, zimm
                return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(
                    op,
                    Register.x0,      // destination = x0
                    Register.x0,      // source register (unused for immediate instructions)
                    encodedImmediate  // CSR and immediate encoded together
                ), lexer));
            } else {
                throw new SyntaxException($"Unknown CSR name: {csrName}");
            }
        }

        public static ProgramNode Parse(Lexer lexer) {
            var programNode = new ProgramNode();
            var currentSection = ".text"; // Default section

            Lexer.TokenData currentToken = default(Lexer.TokenData);
            while(currentToken.token != Lexer.Token.EOF) {
                // Get Next Token
                currentToken = lexer.ReadToken(true);

                if(currentToken.token == Lexer.Token.EOL || currentToken.token == Lexer.Token.EOF) continue;

                // Get section
                var directiveValue = getSectionFromDirectiveToken(currentToken);
                if(directiveValue != default(string)) {
                    currentSection = directiveValue; // Update current section
                    var sectionNode = CreateSectionNode(directiveValue, currentToken, lexer);
                    programNode.Sections.Add(sectionNode);
                    continue;
                }

                throw ThrowUnexpected(currentToken, "Section");
            }

            // Post-process to integrate symbol directives with symbol table
            ProcessSymbolDirectives(programNode, currentSection);

            return programNode;
        }

        /// <summary>
        /// Post-processes the AST to integrate SymbolDirectiveNode instances with the program's symbol table
        /// and to define labels in the symbol table
        /// </summary>
        private static void ProcessSymbolDirectives(ProgramNode program, string defaultSection) {
            foreach (var section in program.Sections) {
                foreach (var node in section.Contents) {
                    if (node is SymbolDirectiveNode symbolDirective) {
                        // Process the symbol directive using the ParserExtensions
                        if (symbolDirective.Type == SymbolDirectiveNode.DirectiveType.Global) {
                            var processedDirective = program.ProcessGlobalDirective(symbolDirective.SymbolName);
                            symbolDirective.Symbol = processedDirective.Symbol;
                        }
                        else if (symbolDirective.Type == SymbolDirectiveNode.DirectiveType.Local) {
                            var processedDirective = program.ProcessLocalDirective(symbolDirective.SymbolName);
                            symbolDirective.Symbol = processedDirective.Symbol;
                        }
                    }
                    else if (node is LabelNode labelNode) {
                        // Define the label in the symbol table
                        program.DefineLabel(labelNode.Name, labelNode.lineNumber, section.Name);
                    }
                    else if (node is InstructionNodeTypeJLabel jLabelInstruction) {
                        // J-type instruction with label reference - add to symbol table if not already present
                        program.SymbolTable.GetOrCreateSymbol(jLabelInstruction.label, SymbolScope.Unknown, SymbolType.Label);
                    }
                    else if (node is InstructionNodeTypeBLabel bLabelInstruction) {
                        // B-type instruction with label reference - add to symbol table if not already present
                        program.SymbolTable.GetOrCreateSymbol(bLabelInstruction.label, SymbolScope.Unknown, SymbolType.Label);
                    }
                }
            }
        }
    }
}