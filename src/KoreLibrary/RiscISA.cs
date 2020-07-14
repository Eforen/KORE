using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    namespace RiscISA
    {
        /// <summary>
        /// This needs to be combined with something else because it only represents inst[6:2]
        /// </summary>
        public enum InstructionType : byte
        {
            LOAD        = 0b00_000_00, // IType
            LOAD_FP     = 0b00_001_00, 
            CUSTOM_0    = 0b00_010_00,
            MISC_MEM    = 0b00_011_00, 
            OP_IMM      = 0b00_100_00, // IType
            AUIPC       = 0b00_101_00,
            OP_IMM_32   = 0b00_110_00,
            STORE       = 0b01_000_00, // SType
            STORE_FP    = 0b01_001_00, 
            CUSTOM_1    = 0b01_010_00,
            AMO         = 0b01_011_00, 
            OP          = 0b01_100_00, // RType?
            LUI         = 0b01_101_00, //UType
            OP_32       = 0b01_110_00,
            MADD        = 0b10_000_00,
            MSUB        = 0b10_001_00,
            NMSUB       = 0b10_010_00,
            NMADD       = 0b10_011_00,
            OP_FP       = 0b10_100_00,
            RESERVED_0  = 0b10_101_00, 
            CUSTOM_2    = 0b10_110_00, // (custom-2/rv128)
            BRANCH      = 0b11_000_00, // BType
            JALR        = 0b11_001_00, // IType
            RESERVED_1  = 0b11_010_00,
            JAL         = 0b11_011_00, // JType
            SYSTEM      = 0b11_100_00, 
            RESERVED_2  = 0b11_101_00,
            CUSTOM_3    = 0b11_110_00 // (custom-3/rv128)
        }
        public enum Register : byte
        {
            //32 registers to x31
            // x0 = 0, x1 = 1, x2 = 2, x3 = 3, x4 = 4, x5 = 5, x6 = 6, x7 = 7, x8 = 8, x9 = 9, x10 = 10, x11 = 11, x12 = 12, x13 = 13, x14 = 14, x15 = 15, x16 = 16, x17 = 17, x18 = 18, x19 = 19, x20 = 20, x21 = 21, x22 = 22, x23 = 23, x24 = 24, x25 = 25, x26 = 26, x27 = 27, x28 = 28, x29 = 29, x30 = 30, x31 = 31
            PC = 0xFE,

            /// <summary> Zero [Immutable] </summary>
            x0 = 0x0, zero = 0x0,

            /// <summary> Return Address [Preserved Across calls: No] </summary>
            ra = 1, x1 = 1,

            /// <summary> Stack Pointer [Preserved Across calls: Yes] </summary>
            sp = 2, x2 = 2,

            /// <summary> Global Pointer [Unallocable] </summary>
            gp = 3, x3 = 3,

            /// <summary> Thread Pointer [Unallocable] </summary>
            tp = 4, x4 = 4,


            /// <summary> 1st Temporary Register [Preserved Across calls: No] </summary>
            t0 = 5,
            /// <summary> 2nd Temporary Register [Preserved Across calls: No] </summary>
            t1 = 6,
            /// <summary> 3rd Temporary Register [Preserved Across calls: No] </summary>
            t2 = 7,
            x5 = 5, x6 = 6, x7 = 7,

            /// <summary> {Optional} Frame Pointer Register [Preserved Across calls: Yes] </summary>
            fp = 8,
            /// <summary> 1st Callee-saved Register [Preserved Across calls: Yes] </summary>
            s0 = 8,
            /// <summary> 2nd Callee-saved Register [Preserved Across calls: Yes] </summary>
            s1 = 9,
            x8 = 8, x9 = 9,

            /// <summary> 1st Argument Register [Preserved Across calls: No] </summary>
            a0 = 10,
            /// <summary> 2nd Argument Register [Preserved Across calls: No] </summary>
            a1 = 11,
            /// <summary> 3rd Argument Register [Preserved Across calls: No] </summary>
            a2 = 12,
            /// <summary> 4th Argument Register [Preserved Across calls: No] </summary>
            a3 = 13,
            /// <summary> 5th Argument Register [Preserved Across calls: No] </summary>
            a4 = 14,
            /// <summary> 6th Argument Register [Preserved Across calls: No] </summary>
            a5 = 15,
            /// <summary> 7th Argument Register [Preserved Across calls: No] </summary>
            a6 = 16,
            /// <summary> 8th Argument Register [Preserved Across calls: No] </summary>
            a7 = 17,
            x10 = 10, x11 = 11, x12 = 12, x13 = 13, x14 = 14, x15 = 15, x16 = 16, x17 = 17,

            /// <summary> 3rd Callee-saved Register [Preserved Across calls: Yes] </summary>
            s2 = 18,
            /// <summary> 4th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s3 = 19,
            /// <summary> 5th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s4 = 20,
            /// <summary> 6th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s5 = 21,
            /// <summary> 7th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s6 = 22,
            /// <summary> 8th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s7 = 23,
            /// <summary> 9th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s8 = 24,
            /// <summary> 10th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s9 = 25,
            /// <summary> 11th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s10 = 26,
            /// <summary> 12th Callee-saved Register [Preserved Across calls: Yes] </summary>
            s11 = 27,
            x18 = 18, x19 = 19, x20 = 20, x21 = 21, x22 = 22, x23 = 23, x24 = 24, x25 = 25, x26 = 26, x27 = 27,

            /// <summary> 4th Temporary Register [Preserved Across calls: No] </summary>
            t3 = 28,
            /// <summary> 5th Temporary Register [Preserved Across calls: No] </summary>
            t4 = 29,
            /// <summary> 6th Temporary Register [Preserved Across calls: No] </summary>
            t5 = 30,
            /// <summary> 7th Temporary Register [Preserved Across calls: No] </summary>
            t6 = 31,
            x28 = 28, x29 = 29, x30 = 30, x31 = 31,
        }

        namespace Instruction
        {
            public static class Transcoder
            {
                //                                            30|         20|          10|         00|
                //                                             1098 7654 3210 9876 5432 1098_7654_3210
                public static readonly ulong SEGMENT_06_00 = 0b0000_0000_0000_0000_0000_0000_0111_1111;
                public static readonly ulong SEGMENT_11_07 = 0b0000_0000_0000_0000_0000_1111_1000_0000;
                public static readonly ulong SEGMENT_14_12 = 0b0000_0000_0000_0000_0111_0000_0000_0000;
                public static readonly ulong SEGMENT_19_15 = 0b0000_0000_0000_1111_1000_0000_0000_0000;
                public static readonly ulong SEGMENT_24_20 = 0b0000_0001_1111_0000_0000_0000_0000_0000;
                public static readonly ulong SEGMENT_31_25 = 0b1111_1110_0000_0000_0000_0000_0000_0000;
                public static readonly ulong SEGMENT_31_20 = SEGMENT_31_25 | SEGMENT_24_20;
                //                           SEGMENT_31_20 = 0b1111_1111_1111_0000_0000_0000_0000_0000;
                public static readonly ulong SEGMENT_31_12 = SEGMENT_31_20 | SEGMENT_19_15 | SEGMENT_14_12;
                //                           SEGMENT_31_12 = 0b1111_1111_1111_1111_1111_0000_0000_0000;

                // public static readonly byte SHIFT_00 = 0;
                // public static readonly byte SHIFT_07 = 7;
                // public static readonly byte SHIFT_12 = 12;
                // public static readonly byte SHIFT_15 = 15;
                // public static readonly byte SHIFT_20 = 20;
                // public static readonly byte SHIFT_25 = 25;

                /// <summary>
                /// opcode (6-0) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift"></param>
                /// <returns></returns>
                public static ulong from_opcode(byte data)
                {
                    return data & SEGMENT_06_00;
                }
                /// <summary>
                /// (from right aligned storage) rd (11-7) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift"></param>
                /// <returns></returns>
                public static ulong from_rd(byte data)
                {
                    return ((uint)data << 7) & SEGMENT_11_07;
                }
                /// <summary>
                /// (from right aligned storage) imm[4:0] (11-7) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift"></param>
                /// <returns></returns>
                public static ulong from_imm_4_0(byte data)
                {
                    return ((uint)data << 7) & SEGMENT_11_07;
                }
                /// <summary>
                /// (from right aligned storage) func3 (14-12) 3 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_func3(byte data)
                {
                    return ((uint)data << 12) & SEGMENT_14_12;
                }
                /// <summary>
                /// (from right aligned storage) rs1 (14-12) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_rs1(byte data)
                {
                    return ((uint)data << 15) & SEGMENT_19_15;
                }
                /// <summary>
                /// (from right aligned storage) rs2 (24-20) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_rs2(byte data)
                {
                    return ((uint)data << 20) & SEGMENT_24_20;
                }
                /// <summary>
                /// (from right aligned storage) func7 (31-25) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_func7(byte data)
                {
                    return ((uint)data << 25) & SEGMENT_31_25;
                }
                /// <summary>
                /// (from right aligned storage) imm[11:0] (31-20) 7+5+12 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_imm_11_0(uint data)
                {
                    return ((uint)data << 20) & SEGMENT_31_20;
                }
                /// <summary>
                /// (from right aligned storage) imm[11:5] (31-25) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_imm_11_5(byte data)
                {
                    return ((uint)data << 25) & SEGMENT_31_25;
                }
                /// <summary>
                /// (from right aligned storage) imm[12|10:5] (31-25) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_imm_12_10_5(byte data)
                {
                    return ((uint)data << 25) & SEGMENT_31_25;
                }
                /// <summary>
                /// (from right aligned storage) imm[31:12] (31-12) 7+5+5+3 = 20 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_imm_31_12(uint data)
                {
                    return ((uint)data << 12) & SEGMENT_31_12;
                }
                /// <summary>
                /// (from right aligned storage) imm[20|10:1|11|19:12] (31-12) 7+5+5+3 + 20 bits
                /// </summary>
                /// <param name="data"></param>
                /// <returns></returns>
                public static ulong from_imm_20_10_1_11_19_12(uint data)
                {
                    return ((uint)data << 12) & SEGMENT_31_12;
                }




                /// <summary>
                /// (to storage) opcode (6-0) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift"></param>
                /// <returns></returns>
                public static ulong to_opcode(ulong data, bool shift)
                {
                    return data & SEGMENT_06_00;
                }

                /// <summary>
                /// (to storage) rd (11-7) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_rd(ulong data, bool shift)
                {
                    if (shift) return (data & SEGMENT_11_07) >> 7;
                    return data & SEGMENT_11_07;
                }
                /// <summary>
                /// (to storage) imm[4:0] (11-7) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_imm_4_0(ulong data, bool shift)
                {
                    if (shift) return (data & SEGMENT_11_07) >> 7;
                    return data & SEGMENT_11_07;
                }
                /// <summary>
                /// (to storage) func3 (14-12) 3 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_func3(ulong data, bool shift)
                {
                    if(shift) return (data & SEGMENT_14_12) >> 12;
                    return data & SEGMENT_14_12;
                }
                /// <summary>
                /// (to storage) rs1 (14-12) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_rs1(ulong data, bool shift)
                {
                    if (shift) return ((uint)data & SEGMENT_19_15) >> 15;
                    return (uint)data & SEGMENT_19_15;
                }
                /// <summary>
                /// (to storage) rs2 (24-20) 5 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_rs2(ulong data, bool shift)
                {
                    if(shift) return ((uint)data & SEGMENT_24_20) >> 20;
                    return (uint)data & SEGMENT_24_20;
                }
                /// <summary>
                /// (to storage) func7 (31-25) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_func7(ulong data, bool shift)
                {
                    if(shift) return ((uint)data & SEGMENT_31_25) >> 25;
                    return (uint)data & SEGMENT_31_25;
                }
                /// <summary>
                /// (to storage) imm[11:0] (31-20) 7+5=12 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_imm_11_0(ulong data, bool shift)
                {
                    if(shift) return ((uint)data & SEGMENT_31_20) >> 20;
                    return (uint)data & SEGMENT_31_20;
                }
                /// <summary>
                /// (to storage) imm[11:5] (31-25) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_imm_11_5(ulong data, bool shift)
                {
                    if(shift) return (((uint)data & SEGMENT_31_25) >> 25);
                    return (uint)data & SEGMENT_31_25;
                }
                /// <summary>
                /// (to storage) imm[12|10:5] (31-25) 7 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_imm_12_10_5(ulong data, bool shift)
                {
                    if(shift) return ((uint)data & SEGMENT_31_25) >> 25;
                    return (uint)data & SEGMENT_31_25;
                }
                /// <summary>
                /// (to storage) imm[31:12] (31-12) 7+5+5+3 = 20 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_imm_31_12(ulong data, bool shift)
                {
                    if(shift) return ((uint)data & SEGMENT_31_12) >> 12;
                    return (uint)data & SEGMENT_31_12;
                }
                /// <summary>
                /// (to storage) imm[20|10:1|11|19:12] (31-12) 7+5+5+3 = 20 bits
                /// </summary>
                /// <param name="data"></param>
                /// <param name="shift">[True|shift to the right][False|Leave in place]</param>
                /// <returns></returns>
                public static ulong to_imm_20_10_1_11_19_12(ulong data, bool shift)
                {
                    if(shift) return ((uint)data & SEGMENT_31_12) >> 12;
                    return (uint)data & SEGMENT_31_12;
                }

                public static INST_TYPE getInstructionTypeFromOp(OPCODE op)
                {
                    switch (op)
                    {
                        case OPCODE.ADDI:
                            return INST_TYPE.IType;
                        case OPCODE.ADD:
                            return INST_TYPE.RType;
                        case OPCODE.unknown00:
                        case OPCODE.unknown01:
                        case OPCODE.unknown02:
                        case OPCODE.unknown03:
                        case OPCODE.unknown04:
                        case OPCODE.unknown05:
                        case OPCODE.unknown06:
                        case OPCODE.unknown07:
                        case OPCODE.unknown08:
                        case OPCODE.unknown09:
                        case OPCODE.unknown0a:
                        case OPCODE.unknown0b:
                        case OPCODE.unknown0c:
                        case OPCODE.unknown0d:
                        case OPCODE.unknown0e:
                        case OPCODE.unknown0f:
                        case OPCODE.unknown10:
                        case OPCODE.unknown11:
                        case OPCODE.unknown12:
                        case OPCODE.unknown14:
                        case OPCODE.unknown15:
                        case OPCODE.unknown16:
                        case OPCODE.unknown17:
                        case OPCODE.unknown18:
                        case OPCODE.unknown19:
                        case OPCODE.unknown1a:
                        case OPCODE.unknown1b:
                        case OPCODE.unknown1c:
                        case OPCODE.unknown1d:
                        case OPCODE.unknown1e:
                        case OPCODE.unknown1f:
                        case OPCODE.unknown20:
                        case OPCODE.unknown21:
                        case OPCODE.unknown22:
                        case OPCODE.unknown23:
                        case OPCODE.unknown24:
                        case OPCODE.unknown25:
                        case OPCODE.unknown26:
                        case OPCODE.unknown27:
                        case OPCODE.unknown28:
                        case OPCODE.unknown29:
                        case OPCODE.unknown2a:
                        case OPCODE.unknown2b:
                        case OPCODE.unknown2c:
                        case OPCODE.unknown2d:
                        case OPCODE.unknown2e:
                        case OPCODE.unknown2f:
                        case OPCODE.unknown30:
                        case OPCODE.unknown31:
                        case OPCODE.unknown32:
                        case OPCODE.unknown34:
                        case OPCODE.unknown35:
                        case OPCODE.unknown36:
                        case OPCODE.unknown37:
                        case OPCODE.unknown38:
                        case OPCODE.unknown39:
                        case OPCODE.unknown3a:
                        case OPCODE.unknown3b:
                        case OPCODE.unknown3c:
                        case OPCODE.unknown3d:
                        case OPCODE.unknown3e:
                        case OPCODE.unknown3f:
                        case OPCODE.unknown40:
                        case OPCODE.unknown41:
                        case OPCODE.unknown42:
                        case OPCODE.unknown43:
                        case OPCODE.unknown44:
                        case OPCODE.unknown45:
                        case OPCODE.unknown46:
                        case OPCODE.unknown47:
                        case OPCODE.unknown48:
                        case OPCODE.unknown49:
                        case OPCODE.unknown4a:
                        case OPCODE.unknown4b:
                        case OPCODE.unknown4c:
                        case OPCODE.unknown4d:
                        case OPCODE.unknown4e:
                        case OPCODE.unknown4f:
                        case OPCODE.unknown50:
                        case OPCODE.unknown51:
                        case OPCODE.unknown52:
                        case OPCODE.unknown53:
                        case OPCODE.unknown54:
                        case OPCODE.unknown55:
                        case OPCODE.unknown56:
                        case OPCODE.unknown57:
                        case OPCODE.unknown58:
                        case OPCODE.unknown59:
                        case OPCODE.unknown5a:
                        case OPCODE.unknown5b:
                        case OPCODE.unknown5c:
                        case OPCODE.unknown5d:
                        case OPCODE.unknown5e:
                        case OPCODE.unknown5f:
                        case OPCODE.unknown60:
                        case OPCODE.unknown61:
                        case OPCODE.unknown62:
                        case OPCODE.unknown63:
                        case OPCODE.unknown64:
                        case OPCODE.unknown65:
                        case OPCODE.unknown66:
                        case OPCODE.unknown67:
                        case OPCODE.unknown68:
                        case OPCODE.unknown69:
                        case OPCODE.unknown6a:
                        case OPCODE.unknown6b:
                        case OPCODE.unknown6c:
                        case OPCODE.unknown6d:
                        case OPCODE.unknown6e:
                        case OPCODE.unknown6f:
                        case OPCODE.unknown70:
                        case OPCODE.unknown71:
                        case OPCODE.unknown72:
                        case OPCODE.unknown73:
                        case OPCODE.unknown74:
                        case OPCODE.unknown75:
                        case OPCODE.unknown76:
                        case OPCODE.unknown77:
                        case OPCODE.unknown78:
                        case OPCODE.unknown79:
                        case OPCODE.unknown7a:
                        case OPCODE.unknown7b:
                        case OPCODE.unknown7c:
                        case OPCODE.unknown7d:
                        case OPCODE.unknown7e:
                        case OPCODE.unknown7f:
                        default:
                            return INST_TYPE.Unkwn;
                    }
                }
            }

            public enum INST_TYPE : byte
            {
                Unkwn = 0x00,
                RType = 0x01,
                IType = 0x02,
                SType = 0x03,
                BType = 0x04,
                UType = 0x05,
                JType = 0x06
            }
            
            public enum OPCODE : byte
            {
                unknown00 = 0x00,
                unknown01 = 0x01,
                unknown02 = 0x02,
                unknown03 = 0x03,
                unknown04 = 0x04,
                unknown05 = 0x05,
                unknown06 = 0x06,
                unknown07 = 0x07,
                unknown08 = 0x08,
                unknown09 = 0x09,
                unknown0a = 0x0a,
                unknown0b = 0x0b,
                unknown0c = 0x0c,
                unknown0d = 0x0d,
                unknown0e = 0x0e,
                unknown0f = 0x0f,
                unknown10 = 0x10,
                unknown11 = 0x11,
                unknown12 = 0x12,
                /// <summary>
                /// I-Type
                /// </summary>
                ADDI = 0b10011, //0x13
                unknown14 = 0x14,
                unknown15 = 0x15,
                unknown16 = 0x16,
                unknown17 = 0x17,
                unknown18 = 0x18,
                unknown19 = 0x19,
                unknown1a = 0x1a,
                unknown1b = 0x1b,
                unknown1c = 0x1c,
                unknown1d = 0x1d,
                unknown1e = 0x1e,
                unknown1f = 0x1f,
                unknown20 = 0x20,
                unknown21 = 0x21,
                unknown22 = 0x22,
                unknown23 = 0x23,
                unknown24 = 0x24,
                unknown25 = 0x25,
                unknown26 = 0x26,
                unknown27 = 0x27,
                unknown28 = 0x28,
                unknown29 = 0x29,
                unknown2a = 0x2a,
                unknown2b = 0x2b,
                unknown2c = 0x2c,
                unknown2d = 0x2d,
                unknown2e = 0x2e,
                unknown2f = 0x2f,
                unknown30 = 0x30,
                unknown31 = 0x31,
                unknown32 = 0x32,
                /// <summary>
                /// R-Type
                /// </summary>
                ADD = 0x33,
                unknown34 = 0x34,
                unknown35 = 0x35,
                unknown36 = 0x36,
                unknown37 = 0x37,
                unknown38 = 0x38,
                unknown39 = 0x39,
                unknown3a = 0x3a,
                unknown3b = 0x3b,
                unknown3c = 0x3c,
                unknown3d = 0x3d,
                unknown3e = 0x3e,
                unknown3f = 0x3f,
                unknown40 = 0x40,
                unknown41 = 0x41,
                unknown42 = 0x42,
                unknown43 = 0x43,
                unknown44 = 0x44,
                unknown45 = 0x45,
                unknown46 = 0x46,
                unknown47 = 0x47,
                unknown48 = 0x48,
                unknown49 = 0x49,
                unknown4a = 0x4a,
                unknown4b = 0x4b,
                unknown4c = 0x4c,
                unknown4d = 0x4d,
                unknown4e = 0x4e,
                unknown4f = 0x4f,
                unknown50 = 0x50,
                unknown51 = 0x51,
                unknown52 = 0x52,
                unknown53 = 0x53,
                unknown54 = 0x54,
                unknown55 = 0x55,
                unknown56 = 0x56,
                unknown57 = 0x57,
                unknown58 = 0x58,
                unknown59 = 0x59,
                unknown5a = 0x5a,
                unknown5b = 0x5b,
                unknown5c = 0x5c,
                unknown5d = 0x5d,
                unknown5e = 0x5e,
                unknown5f = 0x5f,
                unknown60 = 0x60,
                unknown61 = 0x61,
                unknown62 = 0x62,
                unknown63 = 0x63,
                unknown64 = 0x64,
                unknown65 = 0x65,
                unknown66 = 0x66,
                unknown67 = 0x67,
                unknown68 = 0x68,
                unknown69 = 0x69,
                unknown6a = 0x6a,
                unknown6b = 0x6b,
                unknown6c = 0x6c,
                unknown6d = 0x6d,
                unknown6e = 0x6e,
                unknown6f = 0x6f,
                unknown70 = 0x70,
                unknown71 = 0x71,
                unknown72 = 0x72,
                unknown73 = 0x73,
                unknown74 = 0x74,
                unknown75 = 0x75,
                unknown76 = 0x76,
                unknown77 = 0x77,
                unknown78 = 0x78,
                unknown79 = 0x79,
                unknown7a = 0x7a,
                unknown7b = 0x7b,
                unknown7c = 0x7c,
                unknown7d = 0x7d,
                unknown7e = 0x7e,
                unknown7f = 0x7f
            }

            public interface Instruction
            {
                ulong Encode();
                void Decode(ulong data);
            }
            public class RType : Instruction
            {
                /// <summary>
                /// opcode (bits 0 to 6 [from 0 on right])
                /// </summary>
                public Kore.RiscISA.Instruction.OPCODE opcode;
                /// <summary>
                /// rd (bits 7 to 11 [from 0 on right])
                /// </summary>
                public Register rd;
                /// <summary>
                /// func3 (bits 14 to 12 [from 0 on right])
                /// </summary>
                public byte func3;

                public Register rs1;
                public Register rs2;
                public byte func7;
                public ulong Encode()
                {
                    return (byte) opcode | 
                        Transcoder.from_rd((byte) rd) |
                        Transcoder.from_func3(func3) |
                        Transcoder.from_rs1((byte) rs1) |
                        Transcoder.from_rs2((byte) rs2) |
                        Transcoder.from_func7(func7);
                }
                public void Decode(ulong data)
                {
                    opcode = (OPCODE) Transcoder.to_opcode(data, true);
                    rd = (Register)Transcoder.to_rd(data, true);
                    func3 = (byte)Transcoder.to_func3(data, true);
                    rs1 = (Register)Transcoder.to_rs1(data, true);
                    rs2 = (Register)Transcoder.to_rs2(data, true);
                    func7 = (byte)Transcoder.to_func7(data, true);
                }
            }
            public class IType : Instruction
            {
                /// <summary>
                /// opcode (bits 0 to 6 [from 0 on right])
                /// </summary>
                public Kore.RiscISA.Instruction.OPCODE opcode;
                /// <summary>
                /// rd (bits 7 to 11 [from 0 on right])
                /// </summary>
                public Register rd;
                public byte func3;
                public Register rs1;
                public byte imm;

                public ulong Encode()
                {
                    return (byte) opcode |
                        Transcoder.from_rd((byte) rd) |
                        Transcoder.from_func3(func3) |
                        Transcoder.from_rs1((byte) rs1) |
                        Transcoder.from_imm_11_0(imm);
                }
                public void Decode(ulong data)
                {
                    opcode = (Kore.RiscISA.Instruction.OPCODE) Transcoder.to_opcode(data, true);
                    rd = (Register) Transcoder.to_rd(data, true);
                    func3 = (byte)Transcoder.to_func3(data, true);
                    rs1 = (Register) Transcoder.to_rs1(data, true);
                    imm = (byte)Transcoder.to_imm_11_0(data, true);
                }
            }
            public class SType : Instruction
            {
                /// <summary>
                /// opcode (bits 0 to 6 [from 0 on right]) 7
                /// </summary>
                public Kore.RiscISA.Instruction.OPCODE opcode;
                public byte imm_4_0;
                public byte func3;
                public Register rs1;
                public Register rs2;
                public byte imm_11_5;

                public ulong Encode()
                {
                    return (byte)opcode |
                        Transcoder.from_imm_4_0(imm_4_0) |
                        Transcoder.from_func3(func3) |
                        Transcoder.from_rs1((byte) rs1) |
                        Transcoder.from_rs2((byte) rs2) |
                        Transcoder.from_imm_11_5(imm_11_5);
                }
                public void Decode(ulong data)
                {
                    opcode = (Kore.RiscISA.Instruction.OPCODE) Transcoder.to_opcode(data, true);
                    imm_4_0 = (byte)Transcoder.to_imm_4_0(data, true);
                    func3 = (byte)Transcoder.to_func3(data, true);
                    rs1 = (Register) Transcoder.to_rs1(data, true);
                    rs2 = (Register) Transcoder.to_rs2(data, true);
                    imm_11_5 = (byte)Transcoder.to_imm_11_5(data, true);
                }

            }
            public class BType : Instruction
            {
                /// <summary>
                /// opcode (bits 0 to 6 [from 0 on right])
                /// </summary>
                public Kore.RiscISA.Instruction.OPCODE opcode;
                public byte imm_4_1_11;
                public byte func3;
                public Register rs1;
                public Register rs2;
                public byte imm_12_10_5;

                public ulong Encode()
                {
                    return (byte)opcode |
                        Transcoder.from_imm_4_0(imm_4_1_11) |
                        Transcoder.from_func3(func3) |
                        Transcoder.from_rs1((byte) rs1) |
                        Transcoder.from_rs2((byte) rs2) |
                        Transcoder.from_imm_12_10_5(imm_12_10_5);
                }
                public void Decode(ulong data)
                {
                    opcode = (Kore.RiscISA.Instruction.OPCODE) Transcoder.to_opcode(data, true);
                    imm_4_1_11 = (byte)Transcoder.to_imm_4_0(data, true);
                    func3 = (byte)Transcoder.to_func3(data, true);
                    rs1 = (Register) Transcoder.to_rs1(data, true);
                    rs2 = (Register) Transcoder.to_rs2(data, true);
                    imm_12_10_5 = (byte)Transcoder.to_imm_12_10_5(data, true);
                }
            }
            public class UType : Instruction
            {
                /// <summary>
                /// opcode (bits 0 to 6 [from 0 on right])
                /// </summary>
                public Kore.RiscISA.Instruction.OPCODE opcode;
                /// <summary>
                /// rd (bits 7 to 11 [from 0 on right])
                /// </summary>
                public Register rd;
                public byte imm_31_12;
                public ulong Encode()
                {
                    return (byte)opcode |
                        Transcoder.from_rd((byte) rd) |
                        Transcoder.from_imm_31_12(imm_31_12);
                }
                public void Decode(ulong data)
                {
                    opcode = (Kore.RiscISA.Instruction.OPCODE) Transcoder.to_opcode(data, true);
                    rd = (Register) Transcoder.to_rd(data, true);
                    imm_31_12 = (byte)Transcoder.to_imm_31_12(data, true);
                }
            }
            public class JType : Instruction
            {
                /// <summary>
                /// opcode (bits 0 to 6 [from 0 on right])
                /// </summary>
                public Kore.RiscISA.Instruction.OPCODE opcode;
                /// <summary>
                /// rd (bits 7 to 11 [from 0 on right])
                /// </summary>
                public Register rd;
                public byte imm_20_10_1_11_19_12;

                public ulong Encode()
                {
                    return (byte)opcode |
                        Transcoder.from_rd((byte) rd) |
                        Transcoder.from_imm_20_10_1_11_19_12(imm_20_10_1_11_19_12);
                }
                public void Decode(ulong data)
                {
                    opcode = (Kore.RiscISA.Instruction.OPCODE) Transcoder.to_opcode(data, true);
                    rd = (Register) Transcoder.to_rd(data, true);
                    imm_20_10_1_11_19_12 = (byte)Transcoder.to_imm_20_10_1_11_19_12(data, true);
                }
            }
        }
    }
}
