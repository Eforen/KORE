using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kore.RiscISA;

namespace Kore
{
    public class RegisterFile
    {

        #region Register Handling

        protected ulong[] registers = new ulong[32];
        protected ulong ProgramCounter = 0;

        #region R?X Register
        /// <summary>
        /// Set Register R?X to ulong value
        /// </summary>
        /// <param name="register">Which R?X should be set</param>W
        /// <param name="value">What value to place in register. Converted to non signed ulong Irelivent of the type it actually is.</param>
        public void setR(Register register, ulong value) {
            if ((byte)register == (byte)Register.PC)
            {
                ProgramCounter = value;
                return;
            }
            if ((byte)register != (byte)Register.zero) registers[(int)register] = value; 
        }

        /// <summary>
        /// Get Register R?X to ulong value
        /// </summary>
        /// <param name="register">Which R?X should be returned</param>
        /// <returns>The Register Value as a ulong no mater what the actual value is.</returns>
        public ulong getR(Register register)
        {
            if ((byte)register == (byte)Register.PC) return ProgramCounter;
            if ((byte)register == (byte)Register.zero) return 0;
            return registers[(int)register];
        }
        #endregion // End of R?X Register

//         #region E?X Register
//         /// <summary>
//         /// Set Register E?X to uint value
//         /// </summary>
//         /// <param name="register">Which E?X should be set</param>
//         /// <param name="value">What value to place in register. Converted to non signed uint Irelivent of the type it actually is.</param>
//         public void setEX(Register register, uint value) { registers[(int)register] = registers[(int)register] & 0xFF_FF_FF_FF_00_00_00_00 | value; }
// 
//         /// <summary>
//         /// Get Register E?X to uint value
//         /// </summary>
//         /// <param name="register">Which E?X should be returned</param>
//         /// <returns>The Register Value as a uint no mater what the actual value is.</returns>
//         public uint getEX(Register register) { return (uint)(registers[(int)register] & 0xFFFFFFFF); }
//         #endregion // End of E?X Register
// 
//         #region ?X Register
//         /// <summary>
//         /// Set Register ?X to ushort value
//         /// </summary>
//         /// <param name="register">Which ?X should be set</param>
//         /// <param name="value">What value to place in register. Converted to non signed ushort Irelivent of the type it actually is.</param>
//         public void setX(Register register, ushort value) { registers[(int)register] = (registers[(int)register] & 0xFF_FF_FF_FF_FF_FF_00_00) | value; }
// 
//         /// <summary>
//         /// Get Register ?X to ushort value
//         /// </summary>
//         /// <param name="register">Which ?X should be returned</param>
//         /// <returns>The Register Value as a ushort no mater what the actual value is.</returns>
//         public ushort getX(Register register) { return (ushort)(registers[(int)register] & 0xFFFF); }
//         #endregion // End of ?X Register
// 
//         #region ?H Register
//         /// <summary>
//         /// Set Register ?H to byte value
//         /// </summary>
//         /// <param name="register">Which ?H should be set</param>
//         /// <param name="value">What value to place in register. Converted to non signed byte Irelivent of the type it actually is.</param>
//         public void setH(Register register, byte value)
//         {
//             ushort conv = value;
//             conv = (ushort)(conv << 8);
//             registers[(int)register] = (registers[(int)register] & 0xFF_FF_FF_FF_FF_FF_00_FF) | conv;
//         }
// 
//         /// <summary>
//         /// Get Register ?H to byte value
//         /// </summary>
//         /// <param name="register">Which ?H should be returned</param>
//         /// <returns>The Register Value as a byte no mater what the actual value is.</returns>
//         public byte getH(Register register) { return (byte)((registers[(int)register] >> 8) & 0xFF); }
//         #endregion // End of ?H Register
//         #region ?L Register
//         /// <summary>
//         /// Set Register ?L to byte value
//         /// </summary>
//         /// <param name="register">Which ?L should be set</param>
//         /// <param name="value">What value to place in register. Converted to non signed byte Irelivent of the type it actually is.</param>
//         public void setL(Register register, byte value) { registers[(int)register] = (registers[(int)register] & 0xFF_FF_FF_FF_FF_FF_FF_00) | value; }
// 
//         /// <summary>
//         /// Get Register ?L to byte value
//         /// </summary>
//         /// <param name="register">Which ?L should be returned</param>
//         /// <returns>The Register Value as a byte no mater what the actual value is.</returns>
//         public byte getL(Register register) { return (byte)(registers[(int)register] & 0x00FF); }
//         #endregion // End of ?L Register
        #endregion // End of Register Handling
    }
}
