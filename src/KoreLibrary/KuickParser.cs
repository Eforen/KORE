using System;
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
                return "[" + Enum.GetName(typeof(KuickTokenizer.Token), type) + "|" + value.ToString() +"]";
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
            _voidAnyNullTokens();

            // Initiate recurive parse and return the result
            return Page();
        }

        /**
         * Main entry point.
         * 
         * Page
         *   : NumericLiteral
         *   ;
         */
        ParseData Page()
        {
            return new ParseData { type = KuickTokenizer.Token.PAGE, value = Literal() };
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

        /// <summary>
        /// Expects that the next token is of expectedToken and if it is not will throw an error
        /// </summary>
        /// <param name="expectedToken">The expected token type</param>
        /// <returns>token data</returns>
        private KuickTokenizer.TokenData _consume(KuickTokenizer.Token expectedToken)
        {
            _voidAnyNullTokens();

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

        private void _voidAnyNullTokens()
        {
            // if its a null token advance consuming all null tokens until non null token found (Whitespace and/or Comments for example)
            while (_nextToken == KuickTokenizer.Token.NULL)
            {
                // Advance token loo-ahead
                _nextToken = _tokenizer.readToken();
            }
        }
    }
}
