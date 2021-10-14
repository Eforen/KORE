using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class KuickTokenizer
    {
        public class TokenSyntaxException: Exception
        {
            public TokenSyntaxException(string message) : base(message){}
            public TokenSyntaxException(string message, Exception innerException) : base(message, innerException){}
        }

        public enum Token : int
        {
            /// <summary><b>IMPORTANT:</b> This must always be the 0th token so that default will return this type of token</summary>
            NO_TOKEN = 0,
            NULL,
            PAGE,
            NUMBER_INT,
            DIRECTIVE,
            STRING,
            START,
            LABEL,
            OP,
            REGISTER,
            PREPROCESSOR,
            PARREN_OPEN,
            PARREN_CLOSE
        }

        public struct TokenFinder
        {
            public TokenFinder(string test, Token token)
            {
                this.test = new Regex(test, RegexOptions.Compiled);
                this.token = token;
            }
            public Regex test;
            public Token token;
        }

        public struct TokenData
        {
            public TokenData(Token token, string value)
            {
                this.token = token;
                this.value = value;
            }
            public Token token;
            public string value;
            public override bool Equals(object obj)
            {
                // if its a token just compair it to our token type
                if (obj.GetType() == typeof(Token)) return (Token)obj == token;
                // if its a string just compair it to our value
                if (obj.GetType() == typeof(string)) return ((string)obj).Equals(obj);
                // if its another TokenData then compair everything
                if (obj.GetType() == typeof(TokenData)) return ((TokenData)obj).value.Equals(obj) && ((TokenData)obj).token == token;
                // if its none of these then just dump to false
                return false;
            }

            public static bool operator ==(TokenData tokenData, Token token)
            {
                return tokenData.Equals(token);
            }

            public static bool operator !=(TokenData tokenData, Token token)
            {
                return tokenData.Equals(token) == false;
            }

            public static bool operator ==(TokenData tokenData, string value)
            {
                return tokenData.Equals(value);
            }

            public static bool operator !=(TokenData tokenData, string value)
            {
                return tokenData.Equals(value) == false;
            }

            public static bool operator ==(TokenData tokenData, TokenData tokenData2)
            {
                return tokenData.Equals(tokenData2);
            }

            public static bool operator !=(TokenData tokenData, TokenData tokenData2)
            {
                return tokenData.Equals(tokenData2) == false;
            }
        }

        public TokenFinder[] Spec =
        {
            new TokenFinder(@"^\s+", Token.NULL), // Whitespace
            new TokenFinder(@"^\/\/.*", Token.NULL), // Throw away // comments
            new TokenFinder(@"^#.*", Token.NULL), // Throw away # comments
            new TokenFinder(@"^\/\*[\s\S]*?\*\/", Token.NULL), // Throw away /* {ANY} */ comments
            new TokenFinder(@"^\d+", Token.NUMBER_INT), // Number
            new TokenFinder(@"^""[^""]*""", Token.STRING), // " String //TODO: Make this allow escapes
            new TokenFinder(@"^'[^']*'", Token.STRING), // ' String //TODO: Make this allow escapes
        };

        /// <summary>
        /// This holds the string that is currently being Tokenized
        /// </summary>
        string _string;

        /// <summary>
        /// This holds the current position in _string that we are at in tokenization
        /// </summary>
        public int _cursor { get; private set; }

        /// <summary>
        /// Load a string into memory for tokenization
        /// </summary>
        public void load(string str)
        {
            _string = str;
            _cursor = 0;
        }

        /// <summary>
        /// Are there any tokens remaining to be read?
        /// </summary>
        /// <returns></returns>
        public bool hasMoreTokens { get { return _cursor < _string.Length; } }

        public bool isEOF { get { return _cursor >= _string.Length; } }

        private string getNextCharInStr(string str)
        {
            return str.Substring(0, 1);
        }

        /// <summary>
        /// Obtain next token from current _cursor position
        /// </summary>
        /// <returns></returns>
        public TokenData readToken()
        {
            // if no more tokens then return default
            if (isEOF) return default;

            string str = this._string.Substring(_cursor);
            string nextChar = getNextCharInStr(str);

            foreach (TokenFinder finder in Spec){
                // Try to get the value of the token
                string value = _match(finder, str);

                // Go to next token if this token is not found
                if (value == null) continue;

                // We must have found a token if we made it to this line so return apropriate TokenData
                return new TokenData(finder.token, value);
            }

            throw new TokenSyntaxException("Unexpected token: `" + nextChar + "`");
        }

        private string _match(TokenFinder finder, string target)
        {
            // Try the regex on the target string
            Match matched = finder.test.Match(target);
            // Return null if no match found
            if (matched.Success == false) return null;
            // There now must be a match because we would have returned
            // Move the cursor to the end of the match
            _cursor += matched.Value.Length;
            // Return the actual Token value of the match
            return matched.Value;
        }
    }
}
