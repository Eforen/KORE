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

        public enum Token : int
        {
            /// <summary><b>IMPORTANT:</b> This must always be the 0th token so that default will return this type of token</summary>
            NO_TOKEN = 0,
            NULL,
            COMMENT,
            WHITESPACE,
            PAGE,
            NUMBER_INT,
            NUMBER_FLOAT,
            NUMBER_DOUBLE,
            NUMBER_HEX,
            DIRECTIVE,
            STRING,
            START,
            LABEL,
            OP,
            REGISTER,
            PREPROCESSOR,
            PARREN_OPEN,
            PARREN_CLOSE,
            IDENTIFIER,
            DIRECTIVE_TEXT,
            DIRECTIVE_DATA,
            DIRECTIVE_BSS,
            DIRECTIVE_SECTION,
            DIRECTIVE_ALIGN,
            DIRECTIVE_BALIGN,
            DIRECTIVE_GLOBAL,
            DIRECTIVE_STRING,
            DIRECTIVE_BYTE,
            DIRECTIVE_HALF,
            DIRECTIVE_WORD,
            DIRECTIVE_DWORD,
            DIRECTIVE_FLOAT,
            DIRECTIVE_DOUBLE,
            DIRECTIVE_OPTION,
            COMPILER_LOGIC,
            EOF,
            EXPRESSION_LIST
        }

        public TokenFinder[] Spec =
        {
            new TokenFinder(@"^# ?undef", Token.COMPILER_LOGIC), // `#undef` undefine the following sym //TODO: Impliment after v1
            new TokenFinder(@"^# ?define", Token.COMPILER_LOGIC), // `#define` define following sym //TODO: Impliment after v1
            new TokenFinder(@"^# ?ifdef", Token.COMPILER_LOGIC), // `#ifdef` if defined //TODO: Impliment after v1
            new TokenFinder(@"^# ?ifndef", Token.COMPILER_LOGIC), // `#ifndef` if not defined //TODO: Impliment after v1
            new TokenFinder(@"^# ?else", Token.COMPILER_LOGIC), // `#else` if else breaker //TODO: Impliment after v1
            new TokenFinder(@"^# ?endif", Token.COMPILER_LOGIC), // `#endif` end of a compiler if //TODO: Impliment after v1

            new TokenFinder(@"^[\s\n,]+", Token.WHITESPACE), // Whitespace
            new TokenFinder(@"^\/\/.*", Token.COMMENT), // Throw away // comments
            new TokenFinder(@"^#.*", Token.COMMENT), // Throw away # comments
            new TokenFinder(@"^\/\*[\s\S]*?\*\/", Token.COMMENT), // Throw away /* {ANY} */ comments
            new TokenFinder(@"^\d+\.?\d*[fF]", Token.NUMBER_FLOAT), // NUMBER_FLOAT 0.1f || 15f
            new TokenFinder(@"^\d+\.?\d*[dD]", Token.NUMBER_DOUBLE), // NUMBER_DOUBLE 0.12412D || 1245D
            new TokenFinder(@"^\d+\.\d+", Token.NUMBER_DOUBLE), // NUMBER_DOUBLE 0.12412 this finder is for when its a decimal with no marker
            new TokenFinder(@"^0x[\da-fA-F]+", Token.NUMBER_HEX), // NUMBER_HEX 0x1244
            new TokenFinder(@"^\d+", Token.NUMBER_INT), // NUMBER_INT 12578
            new TokenFinder(@"^""[^""]*""", Token.STRING), // " String //TODO: Make this allow escapes
            new TokenFinder(@"^'[^']*'", Token.STRING), // ' String //TODO: Make this allow escapes
            new TokenFinder(@"^\.[a-zA-Z]*", Token.DIRECTIVE), // Directive
            new TokenFinder(@"^[a-zA-Z]*:", Token.LABEL), // Label
            new TokenFinder(@"^\w+", Token.IDENTIFIER), // Identifier
        };
        public class TokenSyntaxException : Exception
        {
            public TokenSyntaxException(string message) : base(message) { }
            public TokenSyntaxException(string message, Exception innerException) : base(message, innerException) { }
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

        /// <summary>
        /// This holds the string that is currently being Tokenized
        /// </summary>
        string _string;

        /// <summary>
        /// This holds the current position in _string that we are at in tokenization
        /// </summary>
        public int _cursor { get; private set; }

        /// <summary>
        /// An attempt to minimize the impact of a situation where there is a large source
        /// file and _line is called multiple times at the cost of 8 bytes of ram 
        /// because I am paranoid
        ///    - Ariel
        /// </summary>
        private int _lineCursorCache = 0;
        /// <summary>
        /// The cursor position at last line cache
        /// </summary>
        private int _lineCursorCachePos = 0;
        /// <summary>
        /// This computes the current line of the cursor. <br/>
        /// <b>Be careful with this call because it is computationally intensive.</b>
        /// </summary>
        public int _line { get; private set; }


        /// <summary>
        /// An attempt to minimize the impact of a situation where there is a large source
        /// file and _col is called multiple times at the cost of 8 bytes of ram 
        /// because I am paranoid
        ///    - Ariel
        /// </summary>
        private int _colCursorCache = 0;
        /// <summary>
        /// The cursor position at last col cache
        /// </summary>
        private int _colCursorCachePos = 0;
        /// <summary>
        /// This computes the current line of the cursor. <br/>
        /// <b>Be careful with this call because it is computationally intensive.</b>
        /// </summary>
        public int _col { get; private set; }

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
            if (isEOF) return new TokenData(KuickTokenizer.Token.EOF, null);

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
