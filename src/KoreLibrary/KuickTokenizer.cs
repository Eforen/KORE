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
            OP_R,
            OP_I,
            OP_S,
            OP_B,
            OP_U,
            OP_J,
            OP_PSEUDO,
            OP_CR,
            OP_CI,
            OP_CSS,
            OP_CIW,
            OP_CL,
            OP_CS,
            OP_CB,
            OP_CJ,
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
            new TokenFinder(@"^[\w\[\]\._]+", Token.IDENTIFIER), // Identifier
        };

        Replacement[] REGISTER_REPLACEMENTS = new Replacement[] { new Replacement("x0", "x0"), new Replacement("x1", "x1"), new Replacement("x2", "x2"), new Replacement("x3", "x3"), new Replacement("x4", "x4"), new Replacement("x5", "x5"), new Replacement("x6", "x6"), new Replacement("x7", "x7"), new Replacement("x8", "x8"), new Replacement("x9", "x9"), new Replacement("x10", "x10"), new Replacement("x11", "x11"), new Replacement("x12", "x12"), new Replacement("x13", "x13"), new Replacement("x14", "x14"), new Replacement("x15", "x15"), new Replacement("x16", "x16"), new Replacement("x17", "x17"), new Replacement("x18", "x18"), new Replacement("x19", "x19"), new Replacement("x20", "x20"), new Replacement("x21", "x21"), new Replacement("x22", "x22"), new Replacement("x23", "x23"), new Replacement("x24", "x24"), new Replacement("x25", "x25"), new Replacement("x26", "x26"), new Replacement("x27", "x27"), new Replacement("x28", "x28"), new Replacement("x29", "x29"), new Replacement("x30", "x30"), new Replacement("x31", "x31"), new Replacement("x00", "x0"), new Replacement("x01", "x1"), new Replacement("x02", "x2"), new Replacement("x03", "x3"), new Replacement("x04", "x4"), new Replacement("x05", "x5"), new Replacement("x06", "x6"), new Replacement("x07", "x7"), new Replacement("x08", "x8"), new Replacement("x09", "x9"), new Replacement("zero", "x0"), new Replacement("ra", "x1"), new Replacement("sp", "x2"), new Replacement("gp", "x3"), new Replacement("tp", "x4"), new Replacement("t0", "x5"), new Replacement("t1", "x6"), new Replacement("t2", "x7"), new Replacement("s0", "x8"), new Replacement("fp", "x8"), new Replacement("s1", "x9"), new Replacement("a0", "x10"), new Replacement("a1", "x11"), new Replacement("a2", "x12"), new Replacement("a3", "x13"), new Replacement("a4", "x14"), new Replacement("a5", "x15"), new Replacement("a6", "x16"), new Replacement("a7", "x17"), new Replacement("s2", "x18"), new Replacement("s3", "x19"), new Replacement("s4", "x20"), new Replacement("s5", "x21"), new Replacement("s6", "x22"), new Replacement("s7", "x23"), new Replacement("s8", "x24"), new Replacement("s9", "x25"), new Replacement("s10", "x26"), new Replacement("s11", "x27"), new Replacement("s12", "x28"), new Replacement("t04", "x29"), new Replacement("t05", "x30"), new Replacement("t06", "x31"), new Replacement("s01", "x9"), new Replacement("a00", "x10"), new Replacement("a01", "x11"), new Replacement("a02", "x12"), new Replacement("a03", "x13"), new Replacement("a04", "x14"), new Replacement("a05", "x15"), new Replacement("a06", "x16"), new Replacement("a07", "x17"), new Replacement("s02", "x18"), new Replacement("s03", "x19"), new Replacement("s04", "x20"), new Replacement("s05", "x21"), new Replacement("s06", "x22"), new Replacement("s07", "x23"), new Replacement("s08", "x24"), new Replacement("s09", "x25"), new Replacement("t04", "x29"), new Replacement("t05", "x30"), new Replacement("t06", "x31") };
        string[] OP_TYPE_R = new string[] { "SLL", "SRL", "SRA", "SLLW", "SRLW", "SRAW", "ADD", "SUB", "ADDW", "SUBW", "XOR", "OR", "AND", "SLT", "SLTU", "MRET", "SRET", "WFI", "SFENCE.VMA", "MUL", "MULH", "MULHSU", "MULHU", "DIV", "DIVU", "REM", "REMU", "MULW", "DIVW", "REMW", "REMUW", "LR.W", "LR.D", "SC.W", "SC.D", "AMOSWAP.W", "AMOSWAP.D", "AMOADD.W", "AMOADD.D", "AMOXOR.W", "AMOAND.W", "AMOOR.W", "AMOXOR.D", "AMOAND.D", "AMOOR.D", "AMOMIN.W", "AMOMAX.W", "AMOMINU.W", "AMOMAXU.W", "AMOMIN.D", "AMOMAX.D", "AMOMINU.D", "AMOMAXU.D" };
        string[] OP_TYPE_I = new string[] { "SLLI", "SRLI", "SRAI", "SLLIW", "SRLIW", "SRAIW", "ADDI", "ADDIW", "XORI", "ORI", "ANDI", "SLTI", "SLTIU", "JALR", "FENCE", "FENCE.I", "ECALL", "EBREAK", "CSRRW", "CSRRS", "CSRRC", "CSRRWI", "CSRRSI", "CSRRCI", "LB", "LH", "LBU", "LHU", "LW", "LWU", "LD" };
        string[] OP_TYPE_U = new string[] { "LUI", "AUIPC" };
        string[] OP_TYPE_B = new string[] { "BEQ", "BNE", "BLT", "BGE", "BLTU", "BGEU" };
        string[] OP_TYPE_J = new string[] { "JAL" };
        string[] OP_TYPE_S = new string[] { "SB", "SH", "SW", "SD" };
        string[] PSUDO_OPs = new string[] { "NOP", "NEG", "NEGW", "SNEZ", "SLTZ", "SGTZ", "BEQZ", "BNEZ", "BLEZ", "BGEZ", "BLTZ", "BGTZ", "J", "JR", "RET", "TAIL", "RDINSTRET[H]", "RDCYCLE[H]", "RDTIME[H]", "csrr", "csrw", "csrs", "csrc", "CSRWI", "CSRSI", "CSRCI", "FRCSR", "FSCSR", "FRRM", "FSRM", "FRFLAGS", "FSFLAGS", "LLA", "LA", "LB", "LH", "LW", "LD", "SB", "SH", "SW", "SD", "flw", "fld", "fsw", "fsd", "li", "mv", "not", "sext.w", "seqz", "fmv.s", "fabs.s", "fneg.s", "fmv.d", "fabs.d", "fneg.d", "bgt", "ble", "bgtu", "bleu", "jal", "jalr", "call", "fence", "fscsr", "fsrm", "fsflags" };


        public class TokenSyntaxException : Exception
        {
            public TokenSyntaxException(string message) : base(message) { }
            public TokenSyntaxException(string message, Exception innerException) : base(message, innerException) { }
        }

        public struct Replacement
        {
            public Replacement(string test, string target)
            {
                this.test = test;
                this.target = target;
            }
            public string test;
            public string target;
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

                if (finder.token == Token.IDENTIFIER)
                {
                    //TODO: check if in register array and thus actually a register and return register instead
                    foreach (Replacement replacement in REGISTER_REPLACEMENTS)
                    {
                        if (value == replacement.test)
                        {
                            return new TokenData(Token.REGISTER, replacement.target);
                        }
                    }

                    //TODO: check if in op arrays (One for each type) and thus actually an OP and return Correct OP type instead
                    if (Array.IndexOf(OP_TYPE_R, value) >= 0)
                    {
                        return new TokenData(Token.OP_R, value);
                    }
                    if (Array.IndexOf(OP_TYPE_I, value) >= 0)
                    {
                        return new TokenData(Token.OP_I, value);
                    }
                    if (Array.IndexOf(OP_TYPE_U, value) >= 0)
                    {
                        return new TokenData(Token.OP_U, value);
                    }
                    if (Array.IndexOf(OP_TYPE_B, value) >= 0)
                    {
                        return new TokenData(Token.OP_B, value);
                    }
                    if (Array.IndexOf(OP_TYPE_J, value) >= 0)
                    {
                        return new TokenData(Token.OP_J, value);
                    }
                    if (Array.IndexOf(OP_TYPE_S, value) >= 0)
                    {
                        return new TokenData(Token.OP_S, value);
                    }
                    if (Array.IndexOf(PSUDO_OPs, value) >= 0)
                    {
                        return new TokenData(Token.OP_PSEUDO, value);
                    }
                }
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
