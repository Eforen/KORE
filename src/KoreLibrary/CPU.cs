using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class CPU
    {
        #region Register Handling

        public enum Register: byte
        {
            /// <summary>
            /// Accumulator
            /// </summary>
            A = 0,
            /// <summary>
            /// Base index (for use with arrays)
            /// </summary>
            B = 1,
            /// <summary>
            /// Counter (for use with loops, and strings, etc)
            /// </summary>
            C = 2,
            /// <summary>
            /// General Data Register
            /// </summary>
            D = 3,
            /// <summary>
            /// Stack pointer for top address of the stack
            /// </summary>
            SP = 4,
            /// <summary>
            /// Stack Base pointer for holding the address of the current stack frame
            /// </summary>
            BP = 5,
            /// <summary>
            /// Instruction pointer. Holds the program counter (the address of the next instruction).
            /// </summary>
            IP = 6
        }

        protected ulong[] registers = new ulong[7];

        #region R?X Register
        /// <summary>
        /// Set Register R?X to ulong value
        /// </summary>
        /// <param name="register">Which R?X should be set</param>W
        /// <param name="value">What value to place in register. Converted to non signed ulong Irelivent of the type it actually is.</param>
        public void setRX(Register register, ulong value) { registers[(int)register] = value; }

        /// <summary>
        /// Get Register R?X to ulong value
        /// </summary>
        /// <param name="register">Which R?X should be returned</param>
        /// <returns>The Register Value as a ulong no mater what the actual value is.</returns>
        public ulong getRX(Register register) { return registers[(int)register]; }
        #endregion // End of R?X Register

        #region E?X Register
        /// <summary>
        /// Set Register E?X to uint value
        /// </summary>
        /// <param name="register">Which E?X should be set</param>
        /// <param name="value">What value to place in register. Converted to non signed uint Irelivent of the type it actually is.</param>
        public void setEX(Register register, uint value) { registers[(int)register] = value; }

        /// <summary>
        /// Get Register E?X to uint value
        /// </summary>
        /// <param name="register">Which E?X should be returned</param>
        /// <returns>The Register Value as a uint no mater what the actual value is.</returns>
        public uint getEX(Register register) { return (uint)(registers[(int)register] & 0xFFFFFFFF); }
        #endregion // End of E?X Register

        #region ?X Register
        /// <summary>
        /// Set Register ?X to ushort value
        /// </summary>
        /// <param name="register">Which ?X should be set</param>
        /// <param name="value">What value to place in register. Converted to non signed ushort Irelivent of the type it actually is.</param>
        public void setX(Register register, ushort value) { registers[(int)register] = value; }

        /// <summary>
        /// Get Register ?X to ushort value
        /// </summary>
        /// <param name="register">Which ?X should be returned</param>
        /// <returns>The Register Value as a ushort no mater what the actual value is.</returns>
        public ushort getX(Register register) { return (ushort)(registers[(int)register] & 0xFFFF); }
        #endregion // End of ?X Register

        #region ?H Register
        /// <summary>
        /// Set Register ?H to byte value
        /// </summary>
        /// <param name="register">Which ?H should be set</param>
        /// <param name="value">What value to place in register. Converted to non signed byte Irelivent of the type it actually is.</param>
        public void setH(Register register, byte value) { registers[(int)register] = (ushort) (value << 2); }

        /// <summary>
        /// Get Register ?H to byte value
        /// </summary>
        /// <param name="register">Which ?H should be returned</param>
        /// <returns>The Register Value as a byte no mater what the actual value is.</returns>
        public byte getH(Register register) { return (byte)((registers[(int)register] >> 2) & 0xFF); }
        #endregion // End of ?H Register
        #region ?L Register
        /// <summary>
        /// Set Register ?L to byte value
        /// </summary>
        /// <param name="register">Which ?L should be set</param>
        /// <param name="value">What value to place in register. Converted to non signed byte Irelivent of the type it actually is.</param>
        public void setL(Register register, byte value) { registers[(int)register] = value; }

        /// <summary>
        /// Get Register ?L to byte value
        /// </summary>
        /// <param name="register">Which ?L should be returned</param>
        /// <returns>The Register Value as a byte no mater what the actual value is.</returns>
        public byte getL(Register register) { return (byte)(registers[(int)register] & 0x00FF); }
        #endregion // End of ?L Register
        #endregion // End of Register Handling
    }
}
