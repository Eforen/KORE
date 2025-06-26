namespace Kore
{
    public partial class KuickParser
    {
        public enum StatementType : int
        {
            /// <summary><b>IMPORTANT:</b> This must always be the 0th token so that default will return this type of token</summary>
            NO_STATEMENT = 0,
            PAGE,
            EXPRESSION_LIST,
            EOF,
            DIRECTIVE_TEXT,
            DIRECTIVE_DATA,
            DIRECTIVE_BSS,
            DIRECTIVE_SECTION,
            DIRECTIVE_ALIGN,
            DIRECTIVE_BALIGN,
            DIRECTIVE_GLOBAL,
            DIRECTIVE_LOCAL,
            DIRECTIVE_STRING,
            DIRECTIVE_BYTE,
            DIRECTIVE_HALF,
            DIRECTIVE_WORD,
            DIRECTIVE_DWORD,
            DIRECTIVE_FLOAT,
            DIRECTIVE_DOUBLE,
            DIRECTIVE_OPTION,
            IDENTIFIER,
            STRING, //TODO: Needs to be removed or refactored
            NUMBER_INT, //TODO: Needs to be removed or moved to a DATA_NUMBER thing
            REGISTER,
            OP,
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
            NUMBER_LITERAL,
            NUMBER_HEX,
        }
    }
}
