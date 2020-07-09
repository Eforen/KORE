using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    public class CPUDev
    {
        /// <summary>
        /// 5 bits used with 5 leftover values
        /// </summary>
        public enum Ops: int
        {
            /// <summary>
            /// No Operation
            /// </summary>
            nop = 0,
            /// <summary>
            /// Raw Copy
            /// </summary>
            mov = 1,
            /// <summary>
            /// Sign aware copy
            /// </summary>
            movSigned = 2,
            /// <summary>
            /// Copy value and zero out the sign bit
            /// </summary>
            movZero = 3,
            /// <summary>
            /// Push onto stack and advance stack pointer
            /// </summary>
            pushToStack = 4,
            /// <summary>
            /// Pop off of the stack and advance stack pointer
            /// </summary>
            popToStack = 5,

            /// <summary>
            /// Load effective address into register (deref register1 and Load the value into register 2)
            /// </summary>
            load = 6,

            /// <summary>
            /// Increment the value in a register storing it in that register
            /// </summary>
            increment = 7,
            decrement = 8,
            negate = 9,
            mathAdd = 10,
            mathSub = 11,
            mathMul = 12,
            mathDiv = 13,
            bitWiseNot = 14,
            bitWiseXor = 15,
            bitWiseOr = 16,
            bitWiseAnd = 17,
            /// <summary>
            /// Convert to or from one byte size to another
            /// </summary>
            convert = 18,
            shiftLeft = 19,
            shiftRight = 20,
            shiftRightClearZero = 21,
            /// <summary>
            /// Bitwise Rotate Right
            /// </summary>
            shiftCircularRight = 22,
            /// <summary>
            /// Bitwise Rotate Left
            /// </summary>
            shiftCircularLeft = 23,
            /// <summary>
            /// Bitwise Rotate Right
            /// </summary>
            shiftCircularRightThroughCarry = 24,
            /// <summary>
            /// Bitwise Rotate Left
            /// </summary>
            shiftCircularLeftThroughCarry = 25,
            /// <summary>
            /// Jump Always Don't actually do this on its own should be just an alias in assembly language
            /// </summary>
            jump = 26, // 0b11010
            uop0 = 27, // 0b11011
            uop1 = 28, // 0b11100
            uop2 = 29, // 0b11101
            uop3 = 30, // 0b11110
            uop4 = 31  // 0b11111
        }

        /// <summary>
        /// 4 bits used with 3 leftover values
        /// </summary>
        public enum conditionCodes
        {
            /// <summary>
            /// No Condition
            /// </summary>
            nc = 0b0000,
            /// <summary>
            /// equal to zero
            /// </summary>
            e = 0b0001,
            /// <summary>
            /// not equal to zero
            /// </summary>
            ne = 0b0010,
            /// <summary>
            /// negative
            /// </summary>
            n = 0b0011,
            /// <summary>
            /// not negative
            /// </summary>
            nn = 0b0100,
            /// <summary>
            /// greater sign sensitive
            /// </summary>
            g = 0b0101,
            /// <summary>
            /// greater or equal sign sensitive
            /// </summary>
            ge = 0b0110,
            /// <summary>
            /// less sign sensitive
            /// </summary>
            l = 0b0111,
            /// <summary>
            /// less or equal sign sensitive
            /// </summary>
            le = 0b1000,
            /// <summary>
            /// above sign insensitive
            /// </summary>
            a = 0b1001,
            /// <summary>
            /// above or equal sign insensitive
            /// </summary>
            ae = 0b1010,
            /// <summary>
            /// below sign insensitive
            /// </summary>
            b = 0b1011,
            /// <summary>
            /// below or equal sign insensitive
            /// </summary>
            be = 0b1100,
            udc0 = 0b1101,
            udc1 = 0b1110,
            udc2 = 0b1111
        }

        /// <summary>
        /// 5 bits used with 8 leftover values
        /// </summary>
        public enum RegisterCodes
        {
            /// <summary>
            /// No Register
            /// </summary>
            n   = 0b00000,
            rax = 0b00001,
            eax = 0b00010,
            ax  = 0b00011,
            ah  = 0b00100,
            al  = 0b00101,
            rbx = 0b00110,
            ebx = 0b00111,
            bx  = 0b01000,
            bh  = 0b01001,
            bl  = 0b01010,
            rcx = 0b01011,
            ecx = 0b01100,
            cx  = 0b01101,
            ch  = 0b01110,
            cl  = 0b01111,
            rdx = 0b10000,
            edx = 0b10001,
            dx  = 0b10010,
            dh  = 0b10011,
            dl  = 0b10100,
            sp  = 0b10101,
            bp  = 0b10110,
            ip  = 0b10111,
            ur0 = 0b11000,
            ur1 = 0b11001,
            ur2 = 0b11010,
            ur3 = 0b11011,
            ur4 = 0b11100,
            ur5 = 0b11101,
            ur6 = 0b11110,
            ur7 = 0b11111,
        }

        /*
         * | condition codes   | op 5 bits              | Register 1 (5 bits)    | Register 3 (5 bits)    |    
         *                                                                                 | Half Memory Address                                                           |
         * | 32 | 31 | 30 | 29 | 28 | 27 | 26 | 25 | 24 | 23 | 22 | 21 | 20 | 19 | 18 | 17 | 16 | 15 | 14 | 13 | 12 | 11 | 10 | 9  | 8  | 7  | 6  | 5  | 4  | 3  | 2  | 1  |
         */
        [Test(Description = "This is not actually a test but a way to design/build the structure of the opcodes for the CPU."), Explicit]
        public void Instantiation()
        {
            
        }

        [Test, Explicit]
        public void GenerateDataMasks()
        {
            byte BIT_COUNT = 64;
            //0b11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, // 64, 0x40 = -1, 0xFF_FF_FF_FF_FF_FF_FF_FF
            //                                                                                                                                 XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX_XXXXXXXX
            string resultHigh = "\t\t/// <summary>\n\t\t/// Used to get the top ith bits  \n\t\t/// </summary>\n\t\tprivate readonly ulong[] dataMaskHigh = {\n\t\t\t0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000, // High 0, 0x0\n";
            string resultLow  = "\t\t/// <summary>\n\t\t/// Used to get the bottom ith bits\n\t\t/// </summary>\n\t\tprivate readonly ulong[] dataMaskLow = {\n\t\t\t0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000, //  Low 0, 0x0\n";
            for (int i = 1; i <= BIT_COUNT; i++)
            {
                resultHigh += "\t\t\t0b";
                resultLow += "\t\t\t0b";

                for (int c = 1; c <= BIT_COUNT; c++)
                {
                    if (c % 8 == 1 && c != 1)
                    {
                        resultHigh += "_";
                        resultLow += "_";
                    }

                    resultHigh += c < i ? "1": "0";
                    resultLow += (BIT_COUNT - c) < i ? "1" : "0";
                }

                if (i != BIT_COUNT)
                {
                    resultHigh += ",";
                    resultLow += ",";
                }
                resultHigh += " // High " + i + ", 0x" + Convert.ToString(i, 16) + "\n";
                resultLow += " // Low " + i + ", 0x" + Convert.ToString(i, 16) + "\n";
            }
            resultHigh += "\t\t};";
            resultLow += "\t\t};";
            Console.WriteLine(resultHigh);
            Console.WriteLine();
            Console.WriteLine(resultLow);
        }
    }
}
