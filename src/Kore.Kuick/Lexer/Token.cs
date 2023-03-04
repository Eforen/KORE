namespace Kore.Kuick {
    public partial class Lexer
    {
        public enum Token : int
        {
            /// <summary><b>IMPORTANT:</b> This must always be the 0th token so that default will return this type of token.</summary>
            NO_TOKEN = 0,

            /// <summary>Represents a null value.</summary>
            NULL,

            /// <summary>Represents a comment.</summary>
            COMMENT,

            /// <summary>Represents whitespace.</summary>
            WHITESPACE,

            /// <summary>Represents an integer number.</summary>
            NUMBER_INT,

            /// <summary>Represents a float number.</summary>
            NUMBER_FLOAT,

            /// <summary>Represents a double number.</summary>
            NUMBER_DOUBLE,

            /// <summary>Represents a hexadecimal number.</summary>
            NUMBER_HEX,

            /// <summary>Represents a directive.</summary>
            DIRECTIVE,

            /// <summary>Represents a string.</summary>
            STRING,

            /// <summary>Represents a label.</summary>
            LABEL,

            /// <summary>Represents a register operand.</summary>
            REGISTER,

            /// <summary>Represents a CSR (Control and Status Register) operand.</summary>
            CSR,

            /// <summary>Represents an instruction opcode that uses 'rd', 'rs1' and 'rs2' operands.</summary>
            OP_R,

            /// <summary>Represents an instruction opcode that uses 'rd', 'rs1' and an immediate operand.</summary>
            OP_I,

            /// <summary>Represents an instruction opcode that uses 'rd', 'rs1' and 'shamt' operands.</summary>
            OP_S,

            /// <summary>Represents an instruction opcode that uses 'rs1' and 'rs2' operands.</summary>
            OP_B,

            /// <summary>Represents an instruction opcode that uses an unsigned immediate operand.</summary>
            OP_U,

            /// <summary>Represents an instruction opcode that uses a jump target operand.</summary>
            OP_J,

            /// <summary>Represents a pseudo instruction opcode.</summary>
            OP_PSEUDO,

            /// <summary>Represents an instruction opcode that uses 'rd', 'rs1', and 'rs2' operands for the Compressed R-type format.</summary>
            OP_CR,

            /// <summary>Represents an instruction opcode that uses 'rd' and an immediate operand for the Compressed I-type format.</summary>
            OP_CI,

            /// <summary>Represents an instruction opcode that uses 'rs2' and an immediate operand for the Compressed Store format.</summary>
            OP_CSS,

            /// <summary>Represents an instruction opcode that uses 'rd' and an immediate operand for the Compressed I-type format with 'lwsp'/'swsp' encoding.</summary>
            OP_CIW,

            /// <summary>Represents an instruction opcode that uses 'rs1' and 'rs2' operands for the Compressed Load format.</summary>
            OP_CL,

            /// <summary>Represents an instruction opcode that uses 'rs1' and 'rs2' operands for the Compressed Store format.</summary>
            OP_CS,

            /// <summary>Represents an instruction opcode that uses 'rs1' and 'rs2' operands for the Compressed Branch format.</summary>
            OP_CB,

            /// <summary>Represents an instruction opcode that uses a jump target operand for the Compressed J-type format.</summary>
            OP_CJ,

            /// <summary>Represents an identifier token.</summary>
            IDENTIFIER,

            /// <summary>Represents a compiler directive or logic statement.</summary>
            COMPILER_LOGIC,

            /// <summary>Represents the end of the input stream.</summary>
            EOF,
            /// <summary> End of Line </summary>
            EOL
        }
    }
}
