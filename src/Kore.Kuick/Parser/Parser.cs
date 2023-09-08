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
            //TODO: Get rid of this signature, it was needed when the lexer had no peak function. 
            // (We should also get rid of all the other signatures that take a Lexer.TokenData currentToken)
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
            if(currentToken.token != Lexer.Token.EOL || currentToken.token != Lexer.Token.EOF) {
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
                    return ParsePseudoInstructionTypeI_Z_IMM_REGISTER(currentToken, RiscMeta.Instructions.TypeI.csrrs, lexer);
                case "CSRC":
                    return ParsePseudoInstructionTypeI_Z_IMM_REGISTER(currentToken, RiscMeta.Instructions.TypeI.csrrc, lexer);
                case "CSRCI":
                    return ParsePseudoInstructionTypeI_Z_IMM_REGISTER(currentToken, RiscMeta.Instructions.TypeI.csrrci, lexer);
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
        /// Psudo SYMBOL/IMM REGISTER -> OP rd, symbol/imm, rt
        /// </summary>
        /// <param name="startToken"></param>
        /// <param name="op"></param>
        /// <param name="lexer"></param>
        /// <returns></returns>
        private static AstNode[] ParsePseudoInstructionTypeI_Z_IMM_REGISTER(Lexer.TokenData startToken, RiscMeta.Instructions.TypeI op, Lexer lexer) {

            // Format Example: OP rd, symbol, rt
            // Skip the first register because its hardcoded to x0
            var token = ExpectToken(lexer, true, Lexer.Token.IDENTIFIER, Lexer.Token.NUMBER_INT, Lexer.Token.NUMBER_HEX);
            int imm = 0;
            switch(token.token) {
                case Lexer.Token.NUMBER_INT:
                case Lexer.Token.NUMBER_HEX:
                    imm = ParseImmediate(lexer);
                    break;
                case Lexer.Token.IDENTIFIER:
                    throw ThrowUnimplemented(token);
                default:
                    throw ThrowUnexpected(token, "Label Identifier or Offset Number");
            }
            var rt = ParseRegister(lexer); // Get the destination register (rd)

            return ComposeInstructionArray(expectReturnEOL(new InstructionNodeTypeI(
                op,
                Register.x0,
                rt,
                imm
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
                        ParseImmediate(lexer
                    )), lexer);
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
                // Get the next token
                currentToken = lexer.ReadToken(true);

                if(
                    currentToken.token == Lexer.Token.EOF ||
                    (currentToken.token == Lexer.Token.DIRECTIVE && getSectionFromDirectiveToken(currentToken) != default(string))
                    ) return section;

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
            currentToken = lexer.ReadToken();

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