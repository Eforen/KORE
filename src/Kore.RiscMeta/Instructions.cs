using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.RiscMeta.Instructions {
    public enum INST_TYPE : byte {
        Unkwn = 0x00,
        RType = 0x01,
        IType = 0x02,
        SType = 0x03,
        BType = 0x04,
        UType = 0x05,
        JType = 0x06,
        /// <summary>
        /// Pseudoinstructions
        /// </summary>
        PType = 0x07,
        /// <summary>
        /// Simple Pseudoinstruction Replacements
        /// </summary>
        PRType = 0x08
    }

    public enum TYPE_OF_INST : byte {
        lui = (byte)INST_TYPE.UType,
        auipc = (byte)INST_TYPE.UType,
        jal = (byte)INST_TYPE.JType,
        /// <summary> jalr rd, offset(rs1) # Jump and Link</summary>
        jalr = (byte)INST_TYPE.IType,
        beq = (byte)INST_TYPE.BType,
        bne = (byte)INST_TYPE.BType,
        blt = (byte)INST_TYPE.BType,
        bge = (byte)INST_TYPE.BType,
        bltu = (byte)INST_TYPE.BType,
        bgeu = (byte)INST_TYPE.BType,
        lb = (byte)INST_TYPE.IType,
        lh = (byte)INST_TYPE.IType,
        lw = (byte)INST_TYPE.IType,
        ld = (byte)INST_TYPE.IType,
        lhu = (byte)INST_TYPE.IType,
        sb = (byte)INST_TYPE.SType,
        sh = (byte)INST_TYPE.SType,
        sw = (byte)INST_TYPE.SType,
        sd = (byte)INST_TYPE.SType,
        addi = (byte)INST_TYPE.IType,
        slti = (byte)INST_TYPE.IType,
        sltiu = (byte)INST_TYPE.IType,
        xori = (byte)INST_TYPE.IType,
        ori = (byte)INST_TYPE.IType,
        andi = (byte)INST_TYPE.IType,
        slli = (byte)INST_TYPE.IType,
        srli = (byte)INST_TYPE.IType,
        srai = (byte)INST_TYPE.IType,
        addiw = (byte)INST_TYPE.IType,  // RV64I Only
        slliw = (byte)INST_TYPE.IType,  // RV64I Only
        srliw = (byte)INST_TYPE.IType,  // RV64I Only
        sraiw = (byte)INST_TYPE.IType,  // RV64I Only
        sltiw = (byte)INST_TYPE.IType,  // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
        sltiuw = (byte)INST_TYPE.IType, // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
        xoriw = (byte)INST_TYPE.IType,  // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
        oriw = (byte)INST_TYPE.IType,   // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
        andiw = (byte)INST_TYPE.IType,  // I don't think this instruction exists in the RiscV ISA  // Would be RV64I Only because would do nothing on RV32
        add = (byte)INST_TYPE.RType,
        sub = (byte)INST_TYPE.RType,
        sll = (byte)INST_TYPE.RType,
        slt = (byte)INST_TYPE.RType,
        sltu = (byte)INST_TYPE.RType,
        xor = (byte)INST_TYPE.RType,
        srl = (byte)INST_TYPE.RType,
        sra = (byte)INST_TYPE.RType,
        or = (byte)INST_TYPE.RType,
        and = (byte)INST_TYPE.RType,
        fence = (byte)INST_TYPE.IType,
        fence_i = (byte)INST_TYPE.IType,
        ecall = (byte)INST_TYPE.IType,
        ebreak = (byte)INST_TYPE.IType,
        csrrw = (byte)INST_TYPE.IType,
        csrrs = (byte)INST_TYPE.IType,
        csrrc = (byte)INST_TYPE.IType,
        csrrwi = (byte)INST_TYPE.IType,
        csrrsi = (byte)INST_TYPE.IType,
        csrrci = (byte)INST_TYPE.IType,
        li = (byte)INST_TYPE.PType,
        mv = (byte)INST_TYPE.PType,
        nop = (byte)INST_TYPE.PRType,
        ret = (byte)INST_TYPE.PRType,
        wfi = (byte)INST_TYPE.PRType
    }

    public enum TypeB
    {
        beq,
        bne,
        blt,
        bge,
        bltu,
        bgeu
    }
    
    public enum TypeIOpcode : uint
    {
        /// <summary> jalr rd, offset(rs1) </summary>
        jalr = 0b1100111,

        // Integer Loads - LOAD opcode
        lb = 0b0000011,
        lbu = 0b0000011,
        lh = 0b0000011,
        lw = 0b0000011,
        lwu = 0b0000011,
        ld = 0b0000011,
        lhu = 0b0000011,

        // OP-IMM
        addi = 0b0010011,
        slti = 0b0010011,
        sltiu = 0b0010011,
        xori = 0b0010011,
        ori = 0b0010011,
        andi = 0b0010011,

        // Shifts are also OP-IMM (funct3 differs)
        slli = 0b0010011,
        srli = 0b0010011,
        srai = 0b0010011,

        // RV64I word ops
        addiw = 0b0011011,
        slliw = 0b0011011,
        srliw = 0b0011011,
        sraiw = 0b0011011,

        // Memory ordering
        fence = 0b0001111,
        fence_i = 0b0001111,

        // System / CSR / Environment - SYSTEM opcode
        ecall = 0b1110011,
        ebreak = 0b1110011,
        csrrw = 0b1110011,
        csrrs = 0b1110011,
        csrrc = 0b1110011,
        csrrwi = 0b1110011,
        csrrsi = 0b1110011,
        csrrci = 0b1110011,

        // Floating-point loads - LOAD-FP opcode
        flw = 0b0000111,
        fld = 0b0000111,
    }

    public enum TypeI
    {
        /// <summary> jalr rd, offset(rs1) # Jump and Link</summary>
        jalr,
        lb,
        lbu,
        lh,
        lw,
        lwu,
        ld,
        lhu,
        addi,
        slti,
        sltiu,
        xori,
        ori,
        andi,
        slli,
        srli,
        srai,
        addiw,
        slliw,
        srliw,
        sraiw,
        // sltiw,
        // sltiuw,
        // xoriw,
        // oriw,
        // andiw,
        fence,
        fence_i,
        ecall,
        ebreak,
        csrrw,
        csrrs,
        csrrc,
        csrrwi,
        csrrsi,
        csrrci,
        flw,
        fld,
    }
    public enum TypeS {
        sb,
        sh,
        sw,
        sd,
        fsw,
        fsd
    }
    public enum TypeSOpcode {
        sb = 0b0100011,
        sh = 0b0100011,
        sw = 0b0100011,
        sd = 0b0100011,
        fsw = 0b0100011,
        fsd = 0b0100011,
    }
    public enum TypeSFunct3 {
        sb = 0b000,
        sh = 0b001,
        sw = 0b010,
        sd = 0b011,
        fsw = 0b010,
        fsd = 0b011
    }

    public static class InstructionHelper
    {
        public static uint GetFunct3(TypeI op)
        {
            switch (op)
            {
                case TypeI.addi:
                    return 0b000;
                case TypeI.slti:
                    return 0b010;
                case TypeI.xori:
                    return 0b100;
                case TypeI.ori:
                    return 0b110;
                case TypeI.andi:
                    return 0b111;
                default:
                    throw new Exception($"Invalid instruction type: {op}");
            }
        }
        public static uint GetFunct3(TypeR op)
        {
            switch (op)
            {
                case TypeR.add:
                    return 0b000;
                case TypeR.sub:
                    return 0b001;
                case TypeR.sll:
                    return 0b010;
                case TypeR.slt:
                    return 0b011;
                case TypeR.sltu:
                    return 0b100;
                case TypeR.xor:
                    return 0b101;
                case TypeR.srl:
                    return 0b110;
                case TypeR.sra:
                    return 0b111;
                case TypeR.or:
                    return 0b100;
                case TypeR.and:
                    return 0b101;
                default:
                    throw new Exception($"Invalid instruction type: {op}");
            }
        }
        public static uint GetFunct7(TypeI op)
        {
            switch (op)
            {
                case TypeI.slli:
                    return 0b0000000;
                case TypeI.srai:
                    return 0b0100000;
                default:
                    return 0b0000000;
            }
        }
        public static uint GetFunct7(TypeR op)
        {
            switch (op)
            {
                case TypeR.sub:
                    return 0b0100000;
                default:
                    return 0b0000000;
            }
        }
        
        public static uint GetOpcode(TypeI op)
        {
            switch (op)
            {
                case TypeI.addi:
                    return (uint)TypeIOpcode.addi;
                case TypeI.slti:
                    return (uint)TypeIOpcode.slti;
                case TypeI.xori:
                    return (uint)TypeIOpcode.xori;
                case TypeI.ori:
                    return (uint)TypeIOpcode.ori;
                case TypeI.andi:
                    return (uint)TypeIOpcode.andi;
                case TypeI.jalr:
                    return (uint)TypeIOpcode.jalr;
                case TypeI.lb:
                    return (uint)TypeIOpcode.lb;
                case TypeI.lbu:
                    return (uint)TypeIOpcode.lbu;
                case TypeI.lh:
                    return (uint)TypeIOpcode.lh;
                case TypeI.lw:
                    return (uint)TypeIOpcode.lw;
                case TypeI.lwu:
                    return (uint)TypeIOpcode.lwu;
                case TypeI.ld:
                    return (uint)TypeIOpcode.ld;
                case TypeI.lhu:
                    return (uint)TypeIOpcode.lhu;
                case TypeI.addiw:
                    return (uint)TypeIOpcode.addiw;
                case TypeI.slliw:
                    return (uint)TypeIOpcode.slliw;
                case TypeI.srliw:
                    return (uint)TypeIOpcode.srliw;
                case TypeI.sraiw:
                    return (uint)TypeIOpcode.sraiw;
                case TypeI.fence:
                    return (uint)TypeIOpcode.fence;
                case TypeI.fence_i:
                    return (uint)TypeIOpcode.fence_i;
                case TypeI.ecall:
                    return (uint)TypeIOpcode.ecall;
                case TypeI.ebreak:
                    return (uint)TypeIOpcode.ebreak;
                case TypeI.csrrw:
                    return (uint)TypeIOpcode.csrrw;
                case TypeI.csrrs:
                    return (uint)TypeIOpcode.csrrs;
                case TypeI.csrrc:
                    return (uint)TypeIOpcode.csrrc;
                case TypeI.csrrwi:
                    return (uint)TypeIOpcode.csrrwi;
                case TypeI.csrrsi:
                    return (uint)TypeIOpcode.csrrsi;
                case TypeI.csrrci:
                    return (uint)TypeIOpcode.csrrci;
                case TypeI.flw:
                    return (uint)TypeIOpcode.flw;
                case TypeI.fld:
                    return (uint)TypeIOpcode.fld;
                default:
                    throw new Exception($"Invalid instruction type: {op}");
            }
        }
        public static uint GetOpcode(TypeR op)
        {
            switch (op)
            {
                case TypeR.add:
                    return (uint)TypeROpcode.add;
                case TypeR.sub:
                    return (uint)TypeROpcode.sub;
                case TypeR.sll:
                    return (uint)TypeROpcode.sll;
                case TypeR.slt:
                    return (uint)TypeROpcode.slt;
                case TypeR.sltu:
                    return (uint)TypeROpcode.sltu;
                case TypeR.xor:
                    return (uint)TypeROpcode.xor;
                case TypeR.srl:
                    return (uint)TypeROpcode.srl;
                case TypeR.sra:
                    return (uint)TypeROpcode.sra;
                case TypeR.or:
                    return (uint)TypeROpcode.or;
                case TypeR.and:
                    return (uint)TypeROpcode.and;
                default:
                    throw new Exception($"Invalid instruction type: {op}");
            }
        }
    }

    public enum TypeR
    {
        add,
        sub,
        sll,
        slt,
        sltu,
        xor,
        srl,
        sra,
        or,
        and
    }
    
    public enum TypeROpcode: uint
    {
        add = 0b0110011,
        sub = 0b0110011,
        sll = 0b0110011,
        slt = 0b0110011,
        sltu = 0b0110011,
        xor = 0b0110011,
        srl = 0b0110011,
        sra = 0b0110011,
        or = 0b0110011,
        and = 0b0110011,
    }

    public enum TypeU
    {
        auipc,
        lui
    }
    public enum TypeJ {
        /// <summary> jal rd, offset # Jump and Link</summary>
        jal
    }
    
    /// <summary>
    /// Pseudoinstructions
    /// </summary>
    public enum TypeP {
        li,
        mv
    }
    
    /// <summary>
    /// Simple Pseudoinstruction Replacements
    /// </summary>
    public enum TypePR {
        nop,
        ret,
        wfi
    }
}
