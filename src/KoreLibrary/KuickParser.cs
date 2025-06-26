using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public partial class KuickParser
    {

        string _string;

        KuickTokenizer _tokenizer = new KuickTokenizer();
        /// <summary>
        /// Look-ahead token for predictive parsing
        /// </summary>
        KuickTokenizer.TokenData _nextToken = default(KuickTokenizer.TokenData);


        /// <summary>
        /// Parse a sting into internal format
        /// </summary>
        /// <param name="str">String to be parsed</param>
        /// <returns></returns>
        public ParseData parse(string str)
        {
            _string = str;
            _tokenizer.load(str);

            // prime the look-ahead
            _nextToken = _tokenizer.readToken();
            _voidAnyWhitespaceTokens();

            // Initiate recurive parse and return the result
            return Page();
        }

        /**
         * Main entry point.
         * 
         * Page
         *   : Expression
         *   ;
         */
        ParseData Page()
        {
            return new ParseData { type = StatementType.PAGE, value = ExpressionList() };
        }

        /**
         * ExpressionList
         *    : Expression
         *    : ExpressionList Expression
         *    ;
         */
        ParseData ExpressionList()
        {
            ArrayList expressions = new ArrayList(1);
            while(_nextToken.token != KuickTokenizer.Token.EOF)
            {
                _voidAnyWhitespaceTokens();
                var expression = Expression();
                if(expression.type != StatementType.EOF) expressions.Add(expression);
            }
            if (expressions.Count == 1) return (ParseData)expressions[0];
            return new ParseData() { type = StatementType.EXPRESSION_LIST, value = expressions.ToArray() };
        }

        /**
         * Expression
         *    : DirectiveCall
         *    : Literal
         *    ;
         */
        ParseData Expression()
        {
            switch (_nextToken.token)
            {
                case KuickTokenizer.Token.DIRECTIVE:
                    return DirectiveCall();
                case KuickTokenizer.Token.OP_R:
                case KuickTokenizer.Token.OP_I:
                case KuickTokenizer.Token.OP_S:
                case KuickTokenizer.Token.OP_B:
                case KuickTokenizer.Token.OP_U:
                case KuickTokenizer.Token.OP_J:
                case KuickTokenizer.Token.OP_PSEUDO:
                case KuickTokenizer.Token.OP_CR:
                case KuickTokenizer.Token.OP_CI:
                case KuickTokenizer.Token.OP_CSS:
                case KuickTokenizer.Token.OP_CIW:
                case KuickTokenizer.Token.OP_CL:
                case KuickTokenizer.Token.OP_CS:
                case KuickTokenizer.Token.OP_CB:
                case KuickTokenizer.Token.OP_CJ:
                    return Operation();
                case KuickTokenizer.Token.NUMBER_INT:
                case KuickTokenizer.Token.STRING:
                    return Literal();
                case KuickTokenizer.Token.EOF:
                    return new ParseData() { type = StatementType.EOF, value = null };
                default:
                    throw new ParserSyntaxException("Expression: unexpected expression `" + _nextToken.value + "`");
            }
        }

        /**
         * OperationLiteral
         *    : Operation_Type_R (OP_R rd, rs1, rs2)
         *    : Operation_Type_I (OP_I rd, rs1, shamt)
         *    : Operation_Type_S (OP_S rd, rs2, imm)
         *    : Operation_Type_B (OP_B rs1, rs2, imm)
         *    : Operation_Type_U (OP_U rd, imm)
         *    : Operation_Type_J (OP_J rd, imm)
         *    : OP_PSEUDO
         *    : OP_CR
         *    : OP_CI
         *    : OP_CSS
         *    : OP_CIW
         *    : OP_CL
         *    : OP_CS
         *    : OP_CB
         *    : OP_CJ
         *    ;
         */
        ParseData Operation()
        {
            switch (_nextToken.token)
            {
                case KuickTokenizer.Token.OP_R:
                    return Operation_Type_R();
                case KuickTokenizer.Token.OP_I:
                    return Operation_Type_I();
                case KuickTokenizer.Token.OP_S:
                    return Operation_Type_S();
                case KuickTokenizer.Token.OP_B:
                    return Operation_Type_B();
                case KuickTokenizer.Token.OP_J:
                    return Operation_Type_J();
                case KuickTokenizer.Token.OP_U:
                    return Operation_Type_U();
                default:
                    throw new ParserSyntaxException("OperationLiteral: unexpected operation `" + _nextToken.value + "`");
            }
        }


        /**
         * Operation_Type_R
         *    : OP_R rd, rs1, rs2
         */
        ParseData Operation_Type_R()
        {
            var op = _consume(KuickTokenizer.Token.OP_R);
            var rd = _consume(KuickTokenizer.Token.REGISTER);
            var rs1 = _consume(KuickTokenizer.Token.REGISTER);
            var rs2 = _consume(KuickTokenizer.Token.REGISTER);

            return new ParseData(
                StatementType.OP_R,
                new ParseData[] {
                    new ParseData(StatementType.OP, op.value),
                    new ParseData(StatementType.REGISTER, rd.value),
                    new ParseData(StatementType.REGISTER, rs1.value),
                    new ParseData(StatementType.REGISTER, rs2.value)
                }
            );
        }

        /**
         * Operation_Type_I
         *    : OP_I rd, rs1, shamt
         */
        ParseData Operation_Type_I()
        {
            var op = _consume(KuickTokenizer.Token.OP_I);
            var rd = _consume(KuickTokenizer.Token.REGISTER);
            var rs1 = _consume(KuickTokenizer.Token.REGISTER);
            ParseData shamt = HexLiteral();

            return new ParseData(
                StatementType.OP_I,
                new ParseData[] {
                    new ParseData(StatementType.OP, op.value),
                    new ParseData(StatementType.REGISTER, rd.value),
                    new ParseData(StatementType.REGISTER, rs1.value),
                    shamt
                }
            );
        }

        /**
         * Operation_Type_S
         *    : OP_S rd, rs2, imm
         */
        ParseData Operation_Type_S()
        {
            var op = _consume(KuickTokenizer.Token.OP_S);
            var rd = _consume(KuickTokenizer.Token.REGISTER);
            var rs2 = _consume(KuickTokenizer.Token.REGISTER);
            ParseData imm = HexLiteral();

            return new ParseData(
                StatementType.OP_S,
                new ParseData[] {
                    new ParseData(StatementType.OP, op.value),
                    new ParseData(StatementType.REGISTER, rd.value),
                    new ParseData(StatementType.REGISTER, rs2.value),
                    imm
                }
            );
        }

        /**
         * Operation_Type_B
         *    : OP_B rs1, rs2, imm
         */
        ParseData Operation_Type_B()
        {
            var op = _consume(KuickTokenizer.Token.OP_B);
            var rs1 = _consume(KuickTokenizer.Token.REGISTER);
            var rs2 = _consume(KuickTokenizer.Token.REGISTER);
            ParseData imm = HexLiteral();

            return new ParseData(
                StatementType.OP_B,
                new ParseData[] {
                    new ParseData(StatementType.OP, op.value),
                    new ParseData(StatementType.REGISTER, rs1.value),
                    new ParseData(StatementType.REGISTER, rs2.value),
                    imm
                }
            );
        }

        /**
         * Operation_Type_J
         *    : OP_J rd, imm
         */
        ParseData Operation_Type_J()
        {
            var op = _consume(KuickTokenizer.Token.OP_J);
            var rd = _consume(KuickTokenizer.Token.REGISTER);
            ParseData imm = HexLiteral();

            return new ParseData(
                StatementType.OP_J,
                new ParseData[] {
                    new ParseData(StatementType.OP, op.value),
                    new ParseData(StatementType.REGISTER, rd.value),
                    imm
                }
            );
        }

        /**
         * Operation_Type_U
         *    : OP_U rd, imm
         */
        ParseData Operation_Type_U()
        {
            var op = _consume(KuickTokenizer.Token.OP_U);
            var rd = _consume(KuickTokenizer.Token.REGISTER);
            ParseData imm = HexLiteral();

            return new ParseData(
                StatementType.OP_J,
                new ParseData[] {
                    new ParseData(StatementType.OP, op.value),
                    new ParseData(StatementType.REGISTER, rd.value),
                    imm
                }
            );
        }

        /**
         * Literal
         *    : NumericLiteral
         *    : StringLiteral
         *    ;
         */
        ParseData Literal()
        {
            switch (_nextToken.token)
            {
                case KuickTokenizer.Token.NUMBER_INT:
                    return IntLiteral();
                case KuickTokenizer.Token.STRING:
                    return StringLiteral();
                default:
                    throw new ParserSyntaxException("Literal: unexpected literal `" + _nextToken.value + "`");
            }
        }

        /**
         * NumericLiteral
         *   : NumericLiteral
         *   : HexLiteral
         *   ;
         */
        ParseData NumericLiteral()
        {
            if (_nextToken.token == KuickTokenizer.Token.NUMBER_INT) return new ParseData { type = StatementType.NUMBER_LITERAL, value = _consume(KuickTokenizer.Token.NUMBER_INT).value };
            return new ParseData { type = StatementType.NUMBER_LITERAL, value = _consume(KuickTokenizer.Token.NUMBER_HEX).value };
        }

        /**
         * NumericLiteral
         *   : NUMBER
         *   ;
         */
        ParseData IntLiteral()
        {
            var token = _consume(KuickTokenizer.Token.NUMBER_INT);
            return new ParseData { type = StatementType.NUMBER_INT, value = Int32.Parse(token.value) };
        }
        ParseData HexLiteral()
        {
            var token = _consume(KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.NUMBER_INT);
            return new ParseData { type = StatementType.NUMBER_HEX, value = Convert.ToInt64(token.value.Replace("0x", ""), 16) };
        }

        /**
         * StringLiteral
         *   : STRING
         *   ;
         */
        ParseData StringLiteral()
        {
            var token = _consume(KuickTokenizer.Token.STRING);
            return new ParseData { type = StatementType.STRING, value = token.value.Substring(1, token.value.Length - 2) };
        }

        /**
         * Compiler Directive Call
         *   : DirectiveSimple
         *   : DirectiveSection
         *   : DirectiveAlign
         *   : DirectiveBAlign
         *   : DirectiveGlobal
         *   : DirectiveString
         *   : DirectiveByte
         *   : DirectiveHalf
         *   : DirectiveWord
         *   : DirectiveDword
         *   : DirectiveFloat
         *   : DirectiveDouble
         *   : DirectiveOption
         *   ;
         */
        ParseData DirectiveCall()
        {
            switch (_nextToken.value.ToLower().Substring(1))
            {
                case "text":
                    return DirectiveSimple();
                case "data":
                    return DirectiveSimple();
                case "bss":
                    return DirectiveSimple();
                case "section":
                    return DirectiveSection();
                case "align":
                    return DirectiveAlign();
                case "balign":
                    return DirectiveBAlign();
                case "global":
                    return DirectiveGlobal();
                case "local":
                    return DirectiveLocal();
                case "string":
                    return DirectiveString();
                case "byte":
                    return DirectiveByte();
                case "half":
                    return DirectiveHalf();
                case "word":
                    return DirectiveWord();
                case "dword":
                    return DirectiveDword();
                case "float":
                    return DirectiveFloat();
                case "double":
                    return DirectiveDouble();
                case "option":
                    return DirectiveOption();
                default:
                    throw new ParserSyntaxException("DirectiveCall: unexpected directive `" + _nextToken.value.Substring(1) + "`");
            }
        }



        /**
         * DirectiveSimple
         *   : .text
         *   : .data
         *   : .bss
         *   ;
         */

        /// <summary>
        /// Serves Text, Data, and BSS directives
        /// </summary>
        /// <returns></returns>
        ParseData DirectiveSimple()
        {
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            StatementType type = StatementType.NO_STATEMENT;

            switch (token.value.ToLower().Substring(1))
            {
                case "text":
                    type = StatementType.DIRECTIVE_TEXT;
                    break;
                case "data":
                    type = StatementType.DIRECTIVE_DATA;
                    break;
                case "bss":
                    type = StatementType.DIRECTIVE_BSS;
                    break;
                default:
                    throw new ParserSyntaxException("DirectiveSimple: unexpected directive `" + token.value.Substring(1) + "`");
            }
            if(type == StatementType.NO_STATEMENT) throw new ParserSyntaxException("Unsupported compiler directive, `" + Enum.GetName(typeof(KuickTokenizer.Token), token.token) + "`");
            return new ParseData(type, token.value.Substring(1));
        }

        ParseData DirectiveSection()
        {
            throw new NotImplementedException("Section Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_SECTION, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveAlign()
        {
            throw new NotImplementedException("Align Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_ALIGN, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveBAlign()
        {
            throw new NotImplementedException("BAlign Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_BALIGN, value = token.value.Substring(1, token.value.Length - 2) };
        }

        /**
         * DirectiveGlobal
         *   : .global sym
         *   ;
         */
        ParseData DirectiveGlobal()
        {
            // In out current KuickAssembler this does nothing because its a single file assembler
            // Consume the Directive Call
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            // Consume the Symbol Identifier
            var sym = Identifier();
            // return an options directive
            return new ParseData { type = StatementType.DIRECTIVE_GLOBAL, value = sym.value};
        }

        /**
         * DirectiveLocal
         *   : .local sym
         *   ;
         */
        ParseData DirectiveLocal()
        {
            // Consume the Directive Call
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            // Consume the Symbol Identifier
            var sym = Identifier();
            // return a local directive
            return new ParseData { type = StatementType.DIRECTIVE_LOCAL, value = sym.value};
        }

        ParseData DirectiveString()
        {
            throw new NotImplementedException("String Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_STRING, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveByte()
        {
            throw new NotImplementedException("Byte Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_BYTE, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveHalf()
        {
            throw new NotImplementedException("Half Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_HALF, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveWord()
        {
            throw new NotImplementedException("Word Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_WORD, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveDword()
        {
            throw new NotImplementedException("Dword Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_DWORD, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveFloat()
        {
            throw new NotImplementedException("Float Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_FLOAT, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveDouble()
        {
            throw new NotImplementedException("Double Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = StatementType.DIRECTIVE_DOUBLE, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveOption()
        {
            // Consume the Directive Call
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            // Consume the Option Flag
            var option = Identifier();
            // return an options directive
            return new ParseData { type = StatementType.DIRECTIVE_OPTION, value = ((string)(option.value)).ToLower() };
        }


        /**
         * Identifier
         *   : IDENTIFIER
         *   ;
         */
        ParseData Identifier()
        {
            var token = _consume(KuickTokenizer.Token.IDENTIFIER);
            return new ParseData { type = StatementType.IDENTIFIER, value = token.value };
        }


        /// <summary>
        /// Expects that the next token is of expectedToken and if it is not will throw an error
        /// </summary>
        /// <param name="expectedToken">The expected token type</param>
        /// <returns>token data</returns>
        private KuickTokenizer.TokenData _consume(params KuickTokenizer.Token[] expectedTokens)
        {
            _voidAnyWhitespaceTokens();

            // if there is no next token
            if (_nextToken == KuickTokenizer.Token.NO_TOKEN)
            {
                // Throw sytax error
                throw new ParserSyntaxException("TokenConsumer: Unexpected end of input, expected `" + getTokenStrings(expectedTokens) + "`");
            }

            // If the token is not what we expected to come next
            if (expectedTokens.Contains(_nextToken.token) == false)
            {
                // Throw sytax error
                throw new ParserSyntaxException("TokenConsumer: Unexpected token `" + getTokenStrings(_nextToken.token) + "`, expected `" + getTokenStrings(expectedTokens) + "`");
            }


            // Cache next token
            KuickTokenizer.TokenData token = _nextToken;

            // Advance token loo-ahead
            _nextToken = _tokenizer.readToken();

            // Return our cached token to caller
            return token;
        }

        private String getTokenStrings(params KuickTokenizer.Token[] tokens)
        {
            string r = "";
            bool first = true;
            foreach (var token in tokens)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    r += ", ";
                }
                r += Enum.GetName(typeof(KuickTokenizer.Token), token);
            }
            return r;
        }

        private void _voidAnyWhitespaceTokens()
        {
            // if its a null token advance consuming all null tokens until non null token found (Whitespace and/or Comments for example)
            while (_nextToken == KuickTokenizer.Token.NULL || _nextToken == KuickTokenizer.Token.WHITESPACE || _nextToken == KuickTokenizer.Token.COMMENT )
            {
                // Advance token loo-ahead
                _nextToken = _tokenizer.readToken();
            }
        }
    }
}
