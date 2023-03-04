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

    public enum TypeB {
        beq,
        bne,
        blt,
        bge,
        bltu,
        bgeu
    }
    public enum TypeI {
        jalr,
        lb,
        lh,
        lw,
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
        sltiw,
        sltiuw,
        xoriw,
        oriw,
        andiw,
        fence,
        fence_i,
        ecall,
        ebreak,
        csrrw,
        csrrc,
        csrrwi,
        csrrsi,
        csrrci
    }
    public enum TypeS {
        sb,
        sh,
        sw,
        sd
    }
    public enum TypeR {
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
    public enum TypeU {
        auipc,
        lui
    }
    public enum TypeJ {
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
