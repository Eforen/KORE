using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreLibrary
{
    public class ALU32
    {
        public enum FUNC : byte
        {
            ADD,
            SUB,
            AND,
            OR,
            XOR,
            SLT,
            SLTU,
            SLL,
            SRA,
            SRL,
        }

        public enum BR_FUNC: byte
        {
            /// <summary>
            /// Equal
            /// </summary>
            EQ,
            /// <summary>
            /// Not Equal
            /// </summary>
            NEQ,
            /// <summary>
            /// Less than Signed 
            /// </summary>
            LT,
            /// <summary>
            /// Less than Unsigned
            /// </summary>
            LTU,
            /// <summary>
            /// Greater than Signed
            /// </summary>
            GE,
            /// <summary>
            /// Greater than Unsigned
            /// </summary>
            GEU,
        }

        public bool branch(uint a, uint b, BR_FUNC func)
        {
            switch (func)
            {
                case BR_FUNC.EQ:
                    return a == b;
                case BR_FUNC.NEQ:
                    return a != b;
                case BR_FUNC.LT:
                    return (int)(a) < (int)(b);
                case BR_FUNC.LTU:
                    return a < b;
                case BR_FUNC.GE:
                    return (int)(a) > (int)(b);
                case BR_FUNC.GEU:
                    return a > b;
                default:
                    return false;
            }
        }
    }
}
