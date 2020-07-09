using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class CPU
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemorySize">Mem in Bytes [Defaults to 1024 * 1024 * 128 (128MB)]</param>
        public CPU(ulong MemorySize = (1024 * 1024 * 128))
        {
            this.MemorySize = MemorySize;
            this.setR(Register.sp, MemorySize - 1);
        }

        public ulong MemorySize { get; private set; }

        #region Register Handling

        public enum Register: byte
        {
            //32 registers to x31
            // x0 = 0, x1 = 1, x2 = 2, x3 = 3, x4 = 4, x5 = 5, x6 = 6, x7 = 7, x8 = 8, x9 = 9, x10 = 10, x11 = 11, x12 = 12, x13 = 13, x14 = 14, x15 = 15, x16 = 16, x17 = 17, x18 = 18, x19 = 19, x20 = 20, x21 = 21, x22 = 22, x23 = 23, x24 = 24, x25 = 25, x26 = 26, x27 = 27, x28 = 28, x29 = 29, x30 = 30, x31 = 31
            PC = 0xFE,

            /// <summary> Zero [Immutable] </summary>
            x0 = 0xFF, zero = 0xFF,
            
            /// <summary> Return Address [Preserved Across calls: No] </summary>
            ra = 0, x1 = 0,

            /// <summary> Stack Pointer [Preserved Across calls: Yes] </summary>
            sp = 1, x2 = 1,

            /// <summary> Global Pointer [Unallocable] </summary>
            gp = 2, x3 = 2,

            /// <summary> Thread Pointer [Unallocable] </summary>
            tp = 3, x4 = 3,


            /// <summary> 1st Temporary Register [Preserved Across calls: No] </summary>
            t0 = 4,
            /// <summary> 2nd Temporary Register [Preserved Across calls: No] </summary>
            t1 = 5,
            /// <summary> 3rd Temporary Register [Preserved Across calls: No] </summary>
            t2 = 6,
            x5 = 4, x6 = 5, x7 = 6,

            /// <summary> {Optional} Frame Pointer Register [Preserved Across calls: Yes] </summary>
            fp = 7,
            /// <summary> 1st Callee-saved Register [Preserved Across calls: Yes] </summary>
            s0 = 7,
            /// <summary> 2nd Callee-saved Register [Preserved Across calls: Yes] </summary>
            s1 = 8,
            x8 = 7, x9 = 8,

            /// <summary> 1st Argument Register [Preserved Across calls: No] </summary>
            a0 = 9,
            /// <summary> 2nd Argument Register [Preserved Across calls: No] </summary>
            a1 = 10,
            /// <summary> 3rd Argument Register [Preserved Across calls: No] </summary>
            a2 = 11,
            /// <summary> 4th Argument Register [Preserved Across calls: No] </summary>
            a3 = 12,
            /// <summary> 5th Argument Register [Preserved Across calls: No] </summary>
            a4 = 13,
            /// <summary> 6th Argument Register [Preserved Across calls: No] </summary>
            a5 = 14,
            /// <summary> 7th Argument Register [Preserved Across calls: No] </summary>
            a6 = 15,
            /// <summary> 8th Argument Register [Preserved Across calls: No] </summary>
            a7 = 16,
            x10 = 9, x11 = 10, x12 = 11, x13 = 12, x14 = 13, x15 = 14, x16 = 15, x17 = 16,

            /// <summary> 3rd Callee-saved Register [Preserved Across calls: Yes] </summary>
            s2 = 17,
            /// <summary> 4th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s3 = 18,
            /// <summary> 5th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s4 = 19,
            /// <summary> 6th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s5 = 20,
            /// <summary> 7th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s6 = 21,
            /// <summary> 8th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s7 = 22,
            /// <summary> 9th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s8 = 23,
            /// <summary> 10th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s9 = 24,
            /// <summary> 11th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s10 = 25, 
            /// <summary> 12th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s11 = 26,
            x18 = 17, x19 = 18, x20 = 19, x21 = 20, x22 = 21, x23 = 22, x24 = 23, x25 = 24, x26 = 25, x27 = 26,

            /// <summary> 4th Temporary Register [Preserved Across calls: No] </summary>
            t3 = 27,
            /// <summary> 5th Temporary Register [Preserved Across calls: No] </summary>
            t4 = 28,
            /// <summary> 6th Temporary Register [Preserved Across calls: No] </summary>
            t5 = 29,
            /// <summary> 7th Temporary Register [Preserved Across calls: No] </summary>
            t6 = 30,
            x28 = 27, x29 = 28, x30 = 29, x31 = 30,
        }

        protected ulong[] registers = new ulong[31];

        #region R?X Register
        /// <summary>
        /// Set Register R?X to ulong value
        /// </summary>
        /// <param name="register">Which R?X should be set</param>W
        /// <param name="value">What value to place in register. Converted to non signed ulong Irelivent of the type it actually is.</param>
        public void setR(Register register, ulong value) { if((byte)register != (byte)Register.zero) registers[(int)register] = value; }

        /// <summary>
        /// Get Register R?X to ulong value
        /// </summary>
        /// <param name="register">Which R?X should be returned</param>
        /// <returns>The Register Value as a ulong no mater what the actual value is.</returns>
        public ulong getR(Register register) {
            if ((byte)register == (byte)Register.zero) return 0;
            return registers[(int)register];
        }
        #endregion // End of R?X Register

        #region E?X Register
        /// <summary>
        /// Set Register E?X to uint value
        /// </summary>
        /// <param name="register">Which E?X should be set</param>
        /// <param name="value">What value to place in register. Converted to non signed uint Irelivent of the type it actually is.</param>
        public void setEX(Register register, uint value) { registers[(int)register] = registers[(int)register] & 0xFF_FF_FF_FF_00_00_00_00 | value; }

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
        public void setX(Register register, ushort value) { registers[(int)register] = (registers[(int)register] & 0xFF_FF_FF_FF_FF_FF_00_00) | value; }

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
        public void setH(Register register, byte value) {
            ushort conv = value;
            conv = (ushort) (conv << 8);
            registers[(int)register] = (registers[(int)register] & 0xFF_FF_FF_FF_FF_FF_00_FF) | conv;
        }

        /// <summary>
        /// Get Register ?H to byte value
        /// </summary>
        /// <param name="register">Which ?H should be returned</param>
        /// <returns>The Register Value as a byte no mater what the actual value is.</returns>
        public byte getH(Register register) { return (byte)((registers[(int)register] >> 8) & 0xFF); }
        #endregion // End of ?H Register
        #region ?L Register
        /// <summary>
        /// Set Register ?L to byte value
        /// </summary>
        /// <param name="register">Which ?L should be set</param>
        /// <param name="value">What value to place in register. Converted to non signed byte Irelivent of the type it actually is.</param>
        public void setL(Register register, byte value) { registers[(int)register] = (registers[(int)register] & 0xFF_FF_FF_FF_FF_FF_FF_00) | value; }

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
