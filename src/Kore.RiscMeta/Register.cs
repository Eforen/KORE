namespace Kore.RiscMeta {
    public enum Register : byte {
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
}