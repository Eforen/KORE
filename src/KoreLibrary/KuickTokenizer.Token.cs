namespace Kore
{
    public partial class KuickTokenizer
    {
        public enum Token : int
        {
            /// <summary><b>IMPORTANT:</b> This must always be the 0th token so that default will return this type of token</summary>
            NO_TOKEN = 0,
            NULL,
            COMMENT,
            WHITESPACE,
            NUMBER_INT,
            NUMBER_FLOAT,
            NUMBER_DOUBLE,
            NUMBER_HEX,
            DIRECTIVE,
            STRING,
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
            CSR,
            IDENTIFIER,
            COMPILER_LOGIC,
            EOF
        }
    }
}
