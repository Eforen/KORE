using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class KuickParser
    {
        public class ParserSyntaxException : Exception
        {
            public ParserSyntaxException(string message) : base(message) { }
            public ParserSyntaxException(string message, Exception innerException) : base(message, innerException) { }
        }

        string _string;
        public struct ParseData
        {
            public KuickTokenizer.Token type;
            public object value;
            public override string ToString()
            {
                if(value != null && value.GetType().IsArray)
                {
                    string str = "[";
                    bool first = true;
                    foreach (object item in (object[])value)
                    {
                        if (item == null) continue;
                        if (first) first = false;
                        else str += ",";
                        str += item.ToString();
                    }
                    str += "]";
                    return "[" + Enum.GetName(typeof(KuickTokenizer.Token), type) + "|" + str + "]";
                }
                return "[" + Enum.GetName(typeof(KuickTokenizer.Token), type) + (value == null ? "" : "|" + value.ToString()) +"]";
            }
        }

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
            return new ParseData { type = KuickTokenizer.Token.PAGE, value = ExpressionList() };
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
                if(expression.type != KuickTokenizer.Token.EOF) expressions.Add(expression);
            }
            if (expressions.Count == 1) return (ParseData)expressions[0];
            return new ParseData() { type = KuickTokenizer.Token.EXPRESSION_LIST, value = expressions.ToArray() };
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
                case KuickTokenizer.Token.NUMBER_INT:
                case KuickTokenizer.Token.STRING:
                    return Literal();
                case KuickTokenizer.Token.EOF:
                    return new ParseData() { type = KuickTokenizer.Token.EOF, value = null };
                default:
                    throw new ParserSyntaxException("Expression: unexpected expression `" + _nextToken.value + "`");
            }
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
         *   : NUMBER
         *   ;
         */
        ParseData IntLiteral()
        {
            var token = _consume(KuickTokenizer.Token.NUMBER_INT);
            return new ParseData { type = KuickTokenizer.Token.NUMBER_INT, value = Int32.Parse(token.value) };
        }

        /**
         * StringLiteral
         *   : STRING
         *   ;
         */
        ParseData StringLiteral()
        {
            var token = _consume(KuickTokenizer.Token.STRING);
            return new ParseData { type = KuickTokenizer.Token.STRING, value = token.value.Substring(1, token.value.Length - 2) };
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

            switch (token.value.ToLower().Substring(1))
            {
                case "text":
                    token.token = KuickTokenizer.Token.DIRECTIVE_TEXT;
                    break;
                case "data":
                    token.token = KuickTokenizer.Token.DIRECTIVE_DATA;
                    break;
                case "bss":
                    token.token = KuickTokenizer.Token.DIRECTIVE_BSS;
                    break;
                default:
                    throw new ParserSyntaxException("DirectiveSimple: unexpected directive `" + token.value.Substring(1) + "`");
            }
            return new ParseData { type = token.token, value = null };
        }

        ParseData DirectiveSection()
        {
            throw new NotImplementedException("Section Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_SECTION, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveAlign()
        {
            throw new NotImplementedException("Align Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_ALIGN, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveBAlign()
        {
            throw new NotImplementedException("BAlign Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_BALIGN, value = token.value.Substring(1, token.value.Length - 2) };
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
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_GLOBAL, value = sym.value};
        }
        ParseData DirectiveString()
        {
            throw new NotImplementedException("String Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_STRING, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveByte()
        {
            throw new NotImplementedException("Byte Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_BYTE, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveHalf()
        {
            throw new NotImplementedException("Half Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_HALF, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveWord()
        {
            throw new NotImplementedException("Word Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_WORD, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveDword()
        {
            throw new NotImplementedException("Dword Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_DWORD, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveFloat()
        {
            throw new NotImplementedException("Float Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_FLOAT, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveDouble()
        {
            throw new NotImplementedException("Double Directive not supported yet");
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_DOUBLE, value = token.value.Substring(1, token.value.Length - 2) };
        }
        ParseData DirectiveOption()
        {
            // Consume the Directive Call
            var token = _consume(KuickTokenizer.Token.DIRECTIVE);
            // Consume the Option Flag
            var option = Identifier();
            // return an options directive
            return new ParseData { type = KuickTokenizer.Token.DIRECTIVE_OPTION, value = ((string)(option.value)).ToLower() };
        }


        /**
         * Identifier
         *   : IDENTIFIER
         *   ;
         */
        ParseData Identifier()
        {
            var token = _consume(KuickTokenizer.Token.IDENTIFIER);
            return new ParseData { type = KuickTokenizer.Token.IDENTIFIER, value = token.value };
        }


        /// <summary>
        /// Expects that the next token is of expectedToken and if it is not will throw an error
        /// </summary>
        /// <param name="expectedToken">The expected token type</param>
        /// <returns>token data</returns>
        private KuickTokenizer.TokenData _consume(KuickTokenizer.Token expectedToken)
        {
            _voidAnyWhitespaceTokens();

            // if there is no next token
            if (_nextToken == KuickTokenizer.Token.NO_TOKEN)
            {
                // Throw sytax error
                throw new ParserSyntaxException("Unexpected end of input, expected `" + Enum.GetName(typeof(KuickTokenizer.Token), expectedToken) + "`");
            }

            // If the token is not what we expected to come next
            if (_nextToken.token != expectedToken)
            {
                // Throw sytax error
                throw new ParserSyntaxException("Unexpected token `" + Enum.GetName(typeof(KuickTokenizer.Token), _nextToken.token) + "`, expected `" + Enum.GetName(typeof(KuickTokenizer.Token), expectedToken) + "`");
            }


            // Cache next token
            KuickTokenizer.TokenData token = _nextToken;

            // Advance token loo-ahead
            _nextToken = _tokenizer.readToken();

            // Return our cached token to caller
            return token;
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
