
using Kore;
//using Kore.RiscISA.Instruction;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Kore.Kuick;

namespace Kore.Kuick.Tests.LexerTests {
    public static class KuickTestUtils {
    }
    public class LexerTests {
        private static Random rand = new Random();
        Lexer tokenizer = new Lexer();

        /*
        [TestCase(".text", new Type[] { typeof(KuickDirectiveToken)})] 
        [TestCase(".global _start", new KuickLexerToken[] { new KuickDirectiveToken(".text"), new KuickLexerToken("_start") })]
        [TestCase(".type _start, @function", new KuickLexerToken[] { new KuickDirectiveToken(".text"), new KuickLexerToken("_start"), new KuickLexerToken("@function") })]
*/

        // ----------------------------------------------------------------
        // RISC-V Registers
        // ---------------------------------------------------------------
        [TestCase("x0", Lexer.Token.REGISTER)]
        [TestCase("x1", Lexer.Token.REGISTER)]
        [TestCase("x2", Lexer.Token.REGISTER)]
        [TestCase("x3", Lexer.Token.REGISTER)]
        [TestCase("x4", Lexer.Token.REGISTER)]
        [TestCase("x5", Lexer.Token.REGISTER)]
        [TestCase("x6", Lexer.Token.REGISTER)]
        [TestCase("x7", Lexer.Token.REGISTER)]
        [TestCase("x8", Lexer.Token.REGISTER)]
        [TestCase("x9", Lexer.Token.REGISTER)]
        [TestCase("x10", Lexer.Token.REGISTER)]
        [TestCase("x11", Lexer.Token.REGISTER)]
        [TestCase("x12", Lexer.Token.REGISTER)]
        [TestCase("x13", Lexer.Token.REGISTER)]
        [TestCase("x14", Lexer.Token.REGISTER)]
        [TestCase("x15", Lexer.Token.REGISTER)]
        [TestCase("x16", Lexer.Token.REGISTER)]
        [TestCase("x17", Lexer.Token.REGISTER)]
        [TestCase("x18", Lexer.Token.REGISTER)]
        [TestCase("x19", Lexer.Token.REGISTER)]
        [TestCase("x20", Lexer.Token.REGISTER)]
        [TestCase("x21", Lexer.Token.REGISTER)]
        [TestCase("x22", Lexer.Token.REGISTER)]
        [TestCase("x23", Lexer.Token.REGISTER)]
        [TestCase("x24", Lexer.Token.REGISTER)]
        [TestCase("x25", Lexer.Token.REGISTER)]
        [TestCase("x26", Lexer.Token.REGISTER)]
        [TestCase("x27", Lexer.Token.REGISTER)]
        [TestCase("x28", Lexer.Token.REGISTER)]
        [TestCase("x29", Lexer.Token.REGISTER)]
        [TestCase("x30", Lexer.Token.REGISTER)]
        [TestCase("x31", Lexer.Token.REGISTER)]

        // ----------------------------------------------------------------
        // Base Integer Instructions: RV32I and RV64I
        // ----------------------------------------------------------------

        // Shifts RV32I
        [TestCase("SLL", Lexer.Token.OP_R)] // Shift Left Logical
        [TestCase("SLLI", Lexer.Token.OP_I)] // Shift Left Logical Immediate
        [TestCase("SRL", Lexer.Token.OP_R)] // Shift Right Logical
        [TestCase("SRLI", Lexer.Token.OP_I)] // Shift Right Logical Immediate
        [TestCase("SRA", Lexer.Token.OP_R)] // Shift Right Arithmetic
        [TestCase("SRAI", Lexer.Token.OP_I)] // Shift Right Arithmetic Immediate
        // Shifts RV64I
        [TestCase("SLLW", Lexer.Token.OP_R)] // Shift Left Logical
        [TestCase("SLLIW", Lexer.Token.OP_I)] // Shift Left Logical Immediate
        [TestCase("SRLW", Lexer.Token.OP_R)] // Shift Right Logical
        [TestCase("SRLIW", Lexer.Token.OP_I)] // Shift Right Logical Immediate
        [TestCase("SRAW", Lexer.Token.OP_R)] // Shift Right Arithmetic
        [TestCase("SRAIW", Lexer.Token.OP_I)] // Shift Right Arithmetic Immediate
        // Arithmetic RV32I
        [TestCase("ADD", Lexer.Token.OP_R)] // Add
        [TestCase("ADDI", Lexer.Token.OP_I)] // Add Immediate
        [TestCase("SUB", Lexer.Token.OP_R)] // Subtract
        [TestCase("LUI", Lexer.Token.OP_U)] // Load Upper Immediate
        [TestCase("AUIPC", Lexer.Token.OP_U)] // Add Upper Immediate to PC
        // Arithmetic RV64I
        [TestCase("ADDW", Lexer.Token.OP_R)] // Add
        [TestCase("ADDIW", Lexer.Token.OP_I)] // Add Immediate
        [TestCase("SUBW", Lexer.Token.OP_R)] // Subtract
        // Logical RV32I
        [TestCase("XOR", Lexer.Token.OP_R)] // Exclusive OR
        [TestCase("XORI", Lexer.Token.OP_I)] // Exclusive OR Immediate
        [TestCase("OR", Lexer.Token.OP_R)] // OR
        [TestCase("ORI", Lexer.Token.OP_I)] // OR Immediate
        [TestCase("AND", Lexer.Token.OP_R)] // AND
        [TestCase("ANDI", Lexer.Token.OP_I)] // AND Immediate
        // Compare RV32I
        [TestCase("SLT", Lexer.Token.OP_R)] // Set <
        [TestCase("SLTI", Lexer.Token.OP_I)] // Set < Immediate
        [TestCase("SLTU", Lexer.Token.OP_R)] // Set < Unsigned
        [TestCase("SLTIU", Lexer.Token.OP_I)] // Set < Immediate Unsigned
        // Branches RV32I
        [TestCase("BEQ", Lexer.Token.OP_B)] // Branch =
        [TestCase("BNE", Lexer.Token.OP_B)] // Branch !=
        [TestCase("BLT", Lexer.Token.OP_B)] // Branch <
        [TestCase("BGE", Lexer.Token.OP_B)] // Branch >=
        [TestCase("BLTU", Lexer.Token.OP_B)] // Branch < Unsigned
        [TestCase("BGEU", Lexer.Token.OP_B)] // Branch >= Unsigned
        // Jump & Link RV32I
        [TestCase("JAL", Lexer.Token.OP_J)] // Jump and Link
        [TestCase("JALR", Lexer.Token.OP_I)] // Jump and Link Register
        // Synch RV32I
        [TestCase("FENCE", Lexer.Token.OP_I)] // Synch thread
        [TestCase("FENCE.I", Lexer.Token.OP_I)] // Synch Instruction and Data
        // Environment RV32I
        [TestCase("ECALL", Lexer.Token.OP_I)] // CALL
        [TestCase("EBREAK", Lexer.Token.OP_I)] // BREAK
        // Control Status Register (CSR) RV32I
        [TestCase("CSRRW", Lexer.Token.OP_I)] // Read/Write
        [TestCase("CSRRS", Lexer.Token.OP_I)] // Read and Set Bit
        [TestCase("CSRRC", Lexer.Token.OP_I)] // Read and Clear Bit
        [TestCase("CSRRWI", Lexer.Token.OP_I)] // Read/Write Immediate
        [TestCase("CSRRSI", Lexer.Token.OP_I)] // Read and Set Immediate
        [TestCase("CSRRCI", Lexer.Token.OP_I)] // Read and Clear Immediate
        // Loads RV32I
        [TestCase("LB", Lexer.Token.OP_I)] // Load Byte
        [TestCase("LH", Lexer.Token.OP_I)] // Load Halfword
        [TestCase("LBU", Lexer.Token.OP_I)] // Load Byte Unsigned
        [TestCase("LHU", Lexer.Token.OP_I)] // Load Half Unsigned
        [TestCase("LW", Lexer.Token.OP_I)] // Load Word
        // Loads RV64I
        [TestCase("LWU", Lexer.Token.OP_I)] // Load Word Unsigned
        [TestCase("LD", Lexer.Token.OP_I)] // Load Double Word
        // Stores RV32I
        [TestCase("SB", Lexer.Token.OP_S)] // Store Byte
        [TestCase("SH", Lexer.Token.OP_S)] // Store Halfword
        [TestCase("SW", Lexer.Token.OP_S)] // Store Word
        // Stores RV64I
        [TestCase("SD", Lexer.Token.OP_S)] // Store Double Word

        // ----------------------------------------------------------------
        // RV Privileged Instructions
        // ----------------------------------------------------------------
        // Trap
        [TestCase("MRET", Lexer.Token.OP_R)] // Machine-mode trap return
        [TestCase("SRET", Lexer.Token.OP_R)] // Supervisor-mode trap return
        // Interrupt
        [TestCase("WFI", Lexer.Token.OP_R)] // Wait for interrupt
        // MMU
        [TestCase("SFENCE.VMA", Lexer.Token.OP_R)] // Virtual Memory FENCE

        // ----------------------------------------------------------------
        // The 60 Pseudoinstructions
        // ----------------------------------------------------------------
        [TestCase("NOP", Lexer.Token.OP_PSEUDO)] // No Operation
        [TestCase("NEG", Lexer.Token.OP_PSEUDO)] // Two's Complement
        [TestCase("NEGW", Lexer.Token.OP_PSEUDO)] // Two's Complement Word
        // --------
        [TestCase("SNEZ", Lexer.Token.OP_PSEUDO)] // Set if != zero
        [TestCase("SLTZ", Lexer.Token.OP_PSEUDO)] // Set if < zero
        [TestCase("SGTZ", Lexer.Token.OP_PSEUDO)] // Set if > zero
        // --------
        [TestCase("BEQZ", Lexer.Token.OP_PSEUDO)] // Branch if == zero
        [TestCase("BNEZ", Lexer.Token.OP_PSEUDO)] // Branch if != zero
        [TestCase("BLEZ", Lexer.Token.OP_PSEUDO)] // Branch if <= zero
        [TestCase("BGEZ", Lexer.Token.OP_PSEUDO)] // Branch if >= zero
        [TestCase("BLTZ", Lexer.Token.OP_PSEUDO)] // Branch if < zero
        [TestCase("BGTZ", Lexer.Token.OP_PSEUDO)] // Branch if > zero
        // --------
        [TestCase("J", Lexer.Token.OP_PSEUDO)] // Jump
        [TestCase("JR", Lexer.Token.OP_PSEUDO)] // Jump to Register value
        [TestCase("RET", Lexer.Token.OP_PSEUDO)] // Return from subroutine
        // --------
        [TestCase("TAIL", Lexer.Token.OP_PSEUDO)] // Tail call far-away subroutine
        // --------
        [TestCase("RDINSTRET[H]", Lexer.Token.OP_PSEUDO)] // Read instructions-retired counter
        [TestCase("RDCYCLE[H]", Lexer.Token.OP_PSEUDO)] // Read Cycle counter
        [TestCase("RDTIME[H]", Lexer.Token.OP_PSEUDO)] // Read realtime clock
        // --------
        [TestCase("csrr", Lexer.Token.OP_PSEUDO)] // Read CSR
        [TestCase("csrw", Lexer.Token.OP_PSEUDO)] // Write CSR
        [TestCase("csrs", Lexer.Token.OP_PSEUDO)] // Set bits in CSR
        [TestCase("csrc", Lexer.Token.OP_PSEUDO)] // Clear bits in CSR
        // --------
        [TestCase("CSRWI", Lexer.Token.OP_PSEUDO)] // Write CSR, Immediate
        [TestCase("CSRSI", Lexer.Token.OP_PSEUDO)] // Set bits in CSR, Immediate
        [TestCase("CSRCI", Lexer.Token.OP_PSEUDO)] // Clear bits in CSR, Immediate
        // --------
        [TestCase("FRCSR", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Read FP control/status register
        [TestCase("FSCSR", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Write FP control/status register
        // --------
        [TestCase("FRRM", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Read FP rounding mode
        [TestCase("FSRM", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Write FP rounding mode
        // --------
        [TestCase("FRFLAGS", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Read FP exception flags
        [TestCase("FSFLAGS", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Write FP exception flags
        // --------
        [TestCase("LLA", Lexer.Token.OP_PSEUDO)] // Load local address
        // --------
        [TestCase("LA", Lexer.Token.OP_PSEUDO)] // Load address
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("LB", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Load global byte
        [TestCase("LH", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Load global half
        [TestCase("LW", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Load global word
        [TestCase("LD", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Load global double
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("SB", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Store global byte
        [TestCase("SH", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Store global half
        [TestCase("SW", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Store global word
        [TestCase("SD", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Store global double
        // --------
        //TODO: Check if these also colide with RV32V/RV64V
        [TestCase("flw", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Floating-point load global
        [TestCase("fld", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Floating-point load global
        // --------
        //TODO: Check if these also colide with RV32V/RV64V
        [TestCase("fsw", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Floating-point store global
        [TestCase("fsd", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Floating-point store global
        // --------
        [TestCase("li", Lexer.Token.OP_PSEUDO)] // Load immediate
        [TestCase("mv", Lexer.Token.OP_PSEUDO)] // Copy register
        [TestCase("not", Lexer.Token.OP_PSEUDO)] // One's complement
        [TestCase("sext.w", Lexer.Token.OP_PSEUDO)] // Sign extend word
        [TestCase("seqz", Lexer.Token.OP_PSEUDO)] // Set if = zero
        // --------
        [TestCase("fmv.s", Lexer.Token.OP_PSEUDO)] // Copy single-precision register
        [TestCase("fabs.s", Lexer.Token.OP_PSEUDO)] // Single-precision absolute value
        [TestCase("fneg.s", Lexer.Token.OP_PSEUDO)] // Single-precision negate
        [TestCase("fmv.d", Lexer.Token.OP_PSEUDO)] // Copy double-precision register
        [TestCase("fabs.d", Lexer.Token.OP_PSEUDO)] // Double-precision absolute value
        [TestCase("fneg.d", Lexer.Token.OP_PSEUDO)] // Double-precision negate
        // --------
        [TestCase("bgt", Lexer.Token.OP_PSEUDO)] // Branch if >
        [TestCase("ble", Lexer.Token.OP_PSEUDO)] // Branch if <=
        [TestCase("bgtu", Lexer.Token.OP_PSEUDO)] // Branch if >, unsigned
        [TestCase("bleu", Lexer.Token.OP_PSEUDO)] // Branch if <=, unsigned
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("jal", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Jump and Link by offset
        [TestCase("jalr", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Jump and Link to register value
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("call", Lexer.Token.OP_PSEUDO)] // call by offset
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("fence", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Fence on all memory and I/O
        // --------
        [TestCase("fscsr", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Swap FP control/status register
        [TestCase("fsrm", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Swap FP rounding mode
        [TestCase("fsflags", Lexer.Token.OP_PSEUDO, Ignore = "Duplicate Operation Name (Handeling in Parser)")] // Swap FP exception flags

        // ----------------------------------------------------------------
        // Multiply-Divide Instructions: RVM
        // ----------------------------------------------------------------
        // Multiply RV32M
        [TestCase("MUL", Lexer.Token.OP_R)] // Multiply
        [TestCase("MULH", Lexer.Token.OP_R)] // Multiply High
        [TestCase("MULHSU", Lexer.Token.OP_R)] // Multiply High Sign/Unsigned
        [TestCase("MULHU", Lexer.Token.OP_R)] // Multiply High Unsigned
        // Divide RV32M
        [TestCase("DIV", Lexer.Token.OP_R)] // Divide
        [TestCase("DIVU", Lexer.Token.OP_R)] // Divide Unsigned
        // Remainder RV32M
        [TestCase("REM", Lexer.Token.OP_R)] // Remainder
        [TestCase("REMU", Lexer.Token.OP_R)] // Remainder Unsigned
        // Multiply RV64M
        [TestCase("MULW", Lexer.Token.OP_R)] // Multiply Word
        // Divide RV64M
        [TestCase("DIVW", Lexer.Token.OP_R)] // Divide Word
        // Remainder RV64M
        [TestCase("REMW", Lexer.Token.OP_R)] // Remainder Word
        [TestCase("REMUW", Lexer.Token.OP_R)] // Remainder //TODO: Confirm this opcode I think it should be REMWU but the book "The RISC-V Reader" has it as REMUW so yeah

        // ----------------------------------------------------------------
        // Atomic Instructions: RVA
        // ----------------------------------------------------------------
        // Load RV32A
        [TestCase("LR.W", Lexer.Token.OP_R)] // Load Reserved Word
        // Load RV64A
        [TestCase("LR.D", Lexer.Token.OP_R)] // Load Reserved Double Word
        // Store RV32A
        [TestCase("SC.W", Lexer.Token.OP_R)] // Store Reserved Word
        // Store RV64A
        [TestCase("SC.D", Lexer.Token.OP_R)] // Store Reserved Double Word
        // Swap RV32A
        [TestCase("AMOSWAP.W", Lexer.Token.OP_R)] // 
        // Swap RV64A
        [TestCase("AMOSWAP.D", Lexer.Token.OP_R)] // 
        // Add RV32A
        [TestCase("AMOADD.W", Lexer.Token.OP_R)] // 
        // Add RV64AA
        [TestCase("AMOADD.D", Lexer.Token.OP_R)] // 
        // Logical RV32A
        [TestCase("AMOXOR.W", Lexer.Token.OP_R)] // 
        [TestCase("AMOAND.W", Lexer.Token.OP_R)] // 
        [TestCase("AMOOR.W", Lexer.Token.OP_R)] // 
        // Logical RV64A
        [TestCase("AMOXOR.D", Lexer.Token.OP_R)] // 
        [TestCase("AMOAND.D", Lexer.Token.OP_R)] // 
        [TestCase("AMOOR.D", Lexer.Token.OP_R)] // 
        // Min/Max RV32A
        [TestCase("AMOMIN.W", Lexer.Token.OP_R)] // 
        [TestCase("AMOMAX.W", Lexer.Token.OP_R)] // 
        [TestCase("AMOMINU.W", Lexer.Token.OP_R)] // 
        [TestCase("AMOMAXU.W", Lexer.Token.OP_R)] // 
        // Min/Max RV64A
        [TestCase("AMOMIN.D", Lexer.Token.OP_R)] // 
        [TestCase("AMOMAX.D", Lexer.Token.OP_R)] // 
        [TestCase("AMOMINU.D", Lexer.Token.OP_R)] // 
        [TestCase("AMOMAXU.D", Lexer.Token.OP_R)] // 

        // ----------------------------------------------------------------
        // Two Optional Floating-Point Instructions: RVF & RVD
        // ----------------------------------------------------------------
        // Move RV32{F|D}
        [TestCase("FMV.W.X", Lexer.Token.OP_R)] // Move form Integer
        [TestCase("FMV.X.W", Lexer.Token.OP_R)] // Move to Integer
        // Move RV64{F|D}
        [TestCase("FMV.D.X", Lexer.Token.OP_R)] // Move form Integer
        [TestCase("FMV.X.D", Lexer.Token.OP_R)] // Move to Integer
        // Convert RV32{F|D}
        [TestCase("FCVT.S.W", Lexer.Token.OP_R)] // Convert form int
        [TestCase("FCVT.D.W", Lexer.Token.OP_R)] // Convert form int
        [TestCase("FCVT.S.WU", Lexer.Token.OP_R)] // Convert form int unsigned
        [TestCase("FCVT.D.WU", Lexer.Token.OP_R)] // Convert form int unsigned
        [TestCase("FCVT.W.S", Lexer.Token.OP_R)] // Convert to int
        [TestCase("FCVT.W.D", Lexer.Token.OP_R)] // Convert to int
        [TestCase("FCVT.WU.S", Lexer.Token.OP_R)] // Convert to int unsigned
        [TestCase("FCVT.WU.D", Lexer.Token.OP_R)] // Convert to int unsigned
        // Convert RV64{F|D}
        [TestCase("FCVT.S.L", Lexer.Token.OP_R)] // Convert form int
        [TestCase("FCVT.D.L", Lexer.Token.OP_R)] // Convert form int
        [TestCase("FCVT.S.LU", Lexer.Token.OP_R)] // Convert form int unsigned
        [TestCase("FCVT.D.LU", Lexer.Token.OP_R)] // Convert form int unsigned
        [TestCase("FCVT.L.S", Lexer.Token.OP_R)] // Convert to int
        [TestCase("FCVT.L.D", Lexer.Token.OP_R)] // Convert to int
        [TestCase("FCVT.LU.S", Lexer.Token.OP_R)] // Convert to int unsigned
        [TestCase("FCVT.LU.D", Lexer.Token.OP_R)] // Convert to int unsigned
        // Load RV32{F|D}
        [TestCase("FLW", Lexer.Token.OP_I)] // Load Word
        [TestCase("FLD", Lexer.Token.OP_I)] // Load Double Word
        // Store RV32{F|D}
        [TestCase("FSW", Lexer.Token.OP_S)] // Save Word
        [TestCase("FSD", Lexer.Token.OP_S)] // Save Double Word
        // Arithmetic RV32{F|D}
        [TestCase("FADD.S", Lexer.Token.OP_R)] // Add (Single Float)
        [TestCase("FADD.D", Lexer.Token.OP_R)] // Add (Double Float)
        [TestCase("FSUB.S", Lexer.Token.OP_R)] // Subtract (Single Float)
        [TestCase("FSUB.D", Lexer.Token.OP_R)] // Subtract (Double Float)
        [TestCase("FMUL.S", Lexer.Token.OP_R)] // Multiply (Single Float)
        [TestCase("FMUL.D", Lexer.Token.OP_R)] // Multiply (Double Float)
        [TestCase("FDIV.S", Lexer.Token.OP_R)] // Divide (Single Float)
        [TestCase("FDIV.D", Lexer.Token.OP_R)] // Divide (Double Float)
        [TestCase("FSQRT.S", Lexer.Token.OP_R)] // Square Root (Single Float)
        [TestCase("FSQRT.D", Lexer.Token.OP_R)] // Square Root (Double Float)
        // Mul-Add RV32{F|D}
        [TestCase("FMADD.S", Lexer.Token.OP_R)] // Multiply-Add (Single Float)
        [TestCase("FMADD.D", Lexer.Token.OP_R)] // Multiply-Add (Double Float)
        [TestCase("FMSUB.S", Lexer.Token.OP_R)] // Multiply-Subtract (Single Float)
        [TestCase("FMSUB.D", Lexer.Token.OP_R)] // Multiply-Subtract (Double Float)
        [TestCase("FNMADD.S", Lexer.Token.OP_R)] // Negative Multiply-Add (Single Float)
        [TestCase("FNMADD.D", Lexer.Token.OP_R)] // Negative Multiply-Add (Double Float)
        [TestCase("FNMSUB.S", Lexer.Token.OP_R)] // Negative Multiply-Subtract (Single Float)
        [TestCase("FNMSUB.D", Lexer.Token.OP_R)] // Negative Multiply-Subtract (Double Float)
        // Sign Inject RV32{F|D}
        [TestCase("FSGNJ.S", Lexer.Token.OP_R)] // Sign Source (Single Float)
        [TestCase("FSGNJ.D", Lexer.Token.OP_R)] // Sign Source (Double Float)
        [TestCase("FSGNJN.S", Lexer.Token.OP_R)] // Negative Sign Source (Single Float)
        [TestCase("FSGNJN.D", Lexer.Token.OP_R)] // Negative Sign Source (Double Float)
        [TestCase("FSGNJX.S", Lexer.Token.OP_R)] // XOR Sign Source (Single Float)
        [TestCase("FSGNJX.D", Lexer.Token.OP_R)] // XOR Sign Source (Double Float)
        // Min/Max RV32{F|D}
        [TestCase("FMIN.S", Lexer.Token.OP_R)] // Minumum (Single Float)
        [TestCase("FMIN.D", Lexer.Token.OP_R)] // Minumum (Double Float)
        [TestCase("FMAX.S", Lexer.Token.OP_R)] // Maximum (Single Float)
        [TestCase("FMAX.D", Lexer.Token.OP_R)] // Maximum (Double Float)
        // Compare RV32{F|D}
        [TestCase("FEQ.S", Lexer.Token.OP_R)] // Compare FLoat = (Single Float)
        [TestCase("FEQ.D", Lexer.Token.OP_R)] // Compare FLoat = (Double Float)
        [TestCase("FLT.S", Lexer.Token.OP_R)] // Compare FLoat < (Single Float)
        [TestCase("FLT.D", Lexer.Token.OP_R)] // Compare FLoat < (Double Float)
        [TestCase("FLE.S", Lexer.Token.OP_R)] // Compare FLoat >= (Single Float)
        [TestCase("FLE.D", Lexer.Token.OP_R)] // Compare FLoat >= (Double Float)
        // Catagorize RV32{F|D}
        [TestCase("FCLASS.S", Lexer.Token.OP_R)] // Classify type (Single Float)
        [TestCase("FCLASS.D", Lexer.Token.OP_R)] // Classify type (Double Float)
        // Configure RV32{F|D}
        [TestCase("FRCSR", Lexer.Token.OP_R)] // Read Status
        [TestCase("FRRM", Lexer.Token.OP_R)] // Read Rounding Mode
        [TestCase("FRFLAGS", Lexer.Token.OP_R)] // Read Flags
        [TestCase("FSCSR", Lexer.Token.OP_R)] // Swap Status Reg
        [TestCase("FSRM", Lexer.Token.OP_R)] // Swap Rounding Mode
        [TestCase("FSFLAGS", Lexer.Token.OP_R)] // Swap Flags
        [TestCase("FSRMI", Lexer.Token.OP_R)] // Swap Rounding Mode Imm
        [TestCase("FSFLAGSI", Lexer.Token.OP_R)] // Swap Flags Imm

        // ----------------------------------------------------------------
        // Vector Instructions: RVV
        // ----------------------------------------------------------------
        [TestCase("SETVL", Lexer.Token.OP_R)] // Set Vector Len

        [TestCase("VMULH", Lexer.Token.OP_R)] // Multiply High
        [TestCase("VREM", Lexer.Token.OP_R)] // Remainder

        [TestCase("VSLL", Lexer.Token.OP_R)] // Shift Left Logical
        [TestCase("VSRL", Lexer.Token.OP_R)] // Shift Right Logical
        [TestCase("VSRA", Lexer.Token.OP_R)] // Shift Right Arithmatic

        [TestCase("VLD", Lexer.Token.OP_I)] // Load
        [TestCase("VLDS", Lexer.Token.OP_R)] // Load Strided
        [TestCase("VLDX", Lexer.Token.OP_R)] // Load Indexed

        [TestCase("VST", Lexer.Token.OP_S)] // Store
        [TestCase("VSTS", Lexer.Token.OP_R)] // Store Strided
        [TestCase("VSTX", Lexer.Token.OP_R)] // Store Indexed

        [TestCase("AMOSWAP", Lexer.Token.OP_R)] // AMO Swap
        [TestCase("AMOADD", Lexer.Token.OP_R)] // AMO ADD
        [TestCase("AMOXOR", Lexer.Token.OP_R)] // AMO XOR
        [TestCase("AMOAND", Lexer.Token.OP_R)] // AMO AND
        [TestCase("AMOOR", Lexer.Token.OP_R)] // AND OR
        [TestCase("AMOMIN", Lexer.Token.OP_R)] // AMO Minimum
        [TestCase("AMOMAX", Lexer.Token.OP_R)] // AMO Maximum

        [TestCase("VPEQ", Lexer.Token.OP_R)] // Predicate =
        [TestCase("VPNE", Lexer.Token.OP_R)] // Predicate !=
        [TestCase("VPLT", Lexer.Token.OP_R)] // Predicate <
        [TestCase("VPGE", Lexer.Token.OP_R)] // Predicate >=

        [TestCase("VPAND", Lexer.Token.OP_R)] // Predicate AND
        [TestCase("VPANDN", Lexer.Token.OP_R)] // Predicate AND NOT
        [TestCase("VPOR", Lexer.Token.OP_R)] // Predicate OR
        [TestCase("VPXOR", Lexer.Token.OP_R)] // Predicate XOR
        [TestCase("VPNOT", Lexer.Token.OP_R)] // Predicate NOT
        [TestCase("VPSWAP", Lexer.Token.OP_R)] // Predicate SWAP

        [TestCase("VMOV", Lexer.Token.OP_R)] // Move

        [TestCase("VCVT", Lexer.Token.OP_R)] // Convert

        [TestCase("VADD", Lexer.Token.OP_R)] // Add
        [TestCase("VSUB", Lexer.Token.OP_R)] // Subtract
        [TestCase("VMUL", Lexer.Token.OP_R)] // Multiply
        [TestCase("VDIV", Lexer.Token.OP_R)] // Divide
        [TestCase("VSQRET", Lexer.Token.OP_R)] // Square Root

        [TestCase("VFMADD", Lexer.Token.OP_R)] // Multiply-ADD
        [TestCase("VFMSUB", Lexer.Token.OP_R)] // Multiply-SUB
        [TestCase("VFNMADD", Lexer.Token.OP_R)] // Negated Multiply-ADD
        [TestCase("VFNMSUB", Lexer.Token.OP_R)] // Negated Multiply-SUB

        [TestCase("VSGNJ", Lexer.Token.OP_R)] // Sign Inject
        [TestCase("VSGNJN", Lexer.Token.OP_R)] // Negated Sign Inject
        [TestCase("VSGNJX", Lexer.Token.OP_R)] // XOR Sign Inject

        [TestCase("VMIN", Lexer.Token.OP_R)] // Minimum
        [TestCase("VMAX", Lexer.Token.OP_R)] // Maximum

        [TestCase("VXOR", Lexer.Token.OP_R)] // XOR
        [TestCase("VOR", Lexer.Token.OP_R)] // OR
        [TestCase("VAND", Lexer.Token.OP_R)] // AND

        [TestCase("VCLASS", Lexer.Token.OP_R)] // CLASS

        [TestCase("VSETDCFG", Lexer.Token.OP_R)] // Set Data Conf

        [TestCase("VEXTRACT", Lexer.Token.OP_R)] // EXTRACT
        [TestCase("VMERGE", Lexer.Token.OP_R)] // MERGE
        [TestCase("VSELECT", Lexer.Token.OP_R)] // SELECT

        // ----------------------------------------------------------------
        // Optional Compressed Instructions: RV32C
        // ----------------------------------------------------------------
        [TestCase("C.LW", Lexer.Token.OP_CL)] // Load Word
        [TestCase("C.LWSP", Lexer.Token.OP_CI)] // Load Word SP
        [TestCase("C.FLW", Lexer.Token.OP_CL)] // Float Load Word
        [TestCase("C.FLWSP", Lexer.Token.OP_CI)] // Float Load Word SP
        [TestCase("C.FLD", Lexer.Token.OP_CL)] // Float Load Double
        [TestCase("C.FLDSP", Lexer.Token.OP_CI)] // Float Load Double SP

        [TestCase("C.SW", Lexer.Token.OP_CS)] // Store Word
        [TestCase("C.SWSP", Lexer.Token.OP_CSS)] // Store Word SP
        [TestCase("C.FSW", Lexer.Token.OP_CS)] // Float Store Word
        [TestCase("C.FSWSP", Lexer.Token.OP_CSS)] // Float Store Word SP
        [TestCase("C.FSD", Lexer.Token.OP_CS)] //  Float Store Double
        [TestCase("C.FSDSP", Lexer.Token.OP_CSS)] // Float Store Double SP

        [TestCase("C.ADD", Lexer.Token.OP_CR)] // ADD
        [TestCase("C.ADDI", Lexer.Token.OP_CI)] // ADD Immediate
        [TestCase("C.ADDI16SP", Lexer.Token.OP_CI)] // ADD SP Immediate * 16
        [TestCase("C.ADDI4SPN", Lexer.Token.OP_CIW)] // ADD SP Immediate * 4
        [TestCase("C.SUB", Lexer.Token.OP_CR)] // SUB
        [TestCase("C.AND", Lexer.Token.OP_CR)] // AND
        [TestCase("C.ANDI", Lexer.Token.OP_CI)] // AND Immediate
        [TestCase("C.OR", Lexer.Token.OP_CR)] // OR
        [TestCase("C.XOR", Lexer.Token.OP_CR)] // Exlusive OR
        [TestCase("C.MV", Lexer.Token.OP_CR)] // Move
        [TestCase("C.LI", Lexer.Token.OP_CI)] // Load Immediate
        [TestCase("C.LUI", Lexer.Token.OP_CI)] // Load Upper Imm

        [TestCase("C.SLLI", Lexer.Token.OP_CI)] // Shift Left Immediate
        [TestCase("C.SRAI", Lexer.Token.OP_CI)] // Shift Right Arithmetic Immediate
        [TestCase("C.SRLI", Lexer.Token.OP_CI)] // Shift Left Logical Immediate

        [TestCase("C.BEQZ", Lexer.Token.OP_CB)] // Branch == 0
        [TestCase("C.BNEZ", Lexer.Token.OP_CB)] // Branch != 0

        [TestCase("C.J", Lexer.Token.OP_CJ)] // Jump
        [TestCase("C.JR", Lexer.Token.OP_CR)] // Jump Register

        [TestCase("C.JAL", Lexer.Token.OP_CJ)] // J&L
        [TestCase("C.JALR", Lexer.Token.OP_CR)] // Jump & Link Register

        [TestCase("C.EBRAKE", Lexer.Token.OP_CI)] // Enviorment Brake

        // ----------------------------------------------------------------
        // Optional Compressed Extention: RV64C
        // ----------------------------------------------------------------

        // RV64C does not include 

        [TestCase("C.ADDW", Lexer.Token.OP_CR)] // ADD Word
        [TestCase("C.ADDIW", Lexer.Token.OP_CI)] // ADD Immediate Word

        [TestCase("C.SUBW", Lexer.Token.OP_CR)] // Subtract Word

        [TestCase("C.LD", Lexer.Token.OP_CL)] // Load Doubleword
        [TestCase("C.LDSP", Lexer.Token.OP_CI)] // Load Doubleword SP

        [TestCase("C.SD", Lexer.Token.OP_CS)] // Store Doubleword
        [TestCase("C.SDSP", Lexer.Token.OP_CSS)] // Store Doubleword SP

        // ----------------------------------------------------------------
        // CSRs (These are all reserved words)
        // ----------------------------------------------------------------
        [TestCase("ustatus", Lexer.Token.CSR)] // CSR ustatus = 0x000  (User status register.)
        [TestCase("uie", Lexer.Token.CSR)] // CSR uie = 0x004  (User interrupt-enable register.)
        [TestCase("utvec", Lexer.Token.CSR)] // CSR utvec = 0x005  (User trap handler base address.)
        [TestCase("uscratch", Lexer.Token.CSR)] // CSR uscratch = 0x040  (Scratch register for user trap handlers.)
        [TestCase("uepc", Lexer.Token.CSR)] // CSR uepc = 0x041  ( User exception program counter.)
        [TestCase("ucause", Lexer.Token.CSR)] // CSR ucause = 0x042  ( User trap cause.)
        [TestCase("utval", Lexer.Token.CSR)] // CSR utval = 0x043  ( User bad address or instruction.)
        [TestCase("uip", Lexer.Token.CSR)] // CSR uip = 0x044  ( User interrupt pending.)
        [TestCase("fflags", Lexer.Token.CSR)] // CSR fflags = 0x001  (Floating-Point Accrued Exceptions.)
        [TestCase("frm", Lexer.Token.CSR)] // CSR frm = 0x002  (Floating-Point Dynamic Rounding Mode.)
        [TestCase("fcsr", Lexer.Token.CSR)] // CSR fcsr = 0x003  (Floating-Point Control and Status Register.)
        [TestCase("cycle", Lexer.Token.CSR)] // CSR cycle = 0xC00  (Cycle counter for RDCYCLE instruction.)
        [TestCase("time", Lexer.Token.CSR)] // CSR time = 0xC01  (Timer for RDTIME instruction.)
        [TestCase("instret", Lexer.Token.CSR)] // CSR instret = 0xC02  (Instructions-retired counter for RDINSTRET instruction )
        [TestCase("hpmcounter3", Lexer.Token.CSR)] // CSR hpmcounter3 = 0xC03  (Performance-monitoring counter.)
        [TestCase("hpmcounter4", Lexer.Token.CSR)] // CSR hpmcounter4 = 0xC04  (Performance-monitoring counter.)
        [TestCase("hpmcounter31", Lexer.Token.CSR)] // CSR hpmcounter31 = 0xC1F  (Performance-monitoring counter.)
        [TestCase("cycleh", Lexer.Token.CSR)] // CSR cycleh = 0xC80  (Upper 32 bits of cycle, RV32 only )
        [TestCase("timeh", Lexer.Token.CSR)] // CSR timeh = 0xC81  (Upper 32 bits of time, RV32 only )
        [TestCase("instreth", Lexer.Token.CSR)] // CSR instreth = 0xC82  (Upper 32 bits of instret, RV32 only )
        [TestCase("hpmcounter3h", Lexer.Token.CSR)] // CSR hpmcounter3h = 0xC83  (Upper 32 bits of hpmcounter3, RV32 only )
        [TestCase("hpmcounter4h", Lexer.Token.CSR)] // CSR hpmcounter4h = 0xC84  (Upper 32 bits of hpmcounter4, RV32 only )
        [TestCase("hpmcounter31h", Lexer.Token.CSR)] // CSR hpmcounter31h = 0xC9F  (Upper 32 bits of hpmcounter31, RV32 only )
        [TestCase("sstatus", Lexer.Token.CSR)] // CSR sstatus = 0x100  (Supervisor status register.)
        [TestCase("sedeleg", Lexer.Token.CSR)] // CSR sedeleg = 0x102  (Supervisor exception delegation register.)
        [TestCase("sideleg", Lexer.Token.CSR)] // CSR sideleg = 0x103  (Supervisor interrupt delegation register.)
        [TestCase("sie", Lexer.Token.CSR)] // CSR sie = 0x104  (Supervisor interrupt-enable register.)
        [TestCase("stvec", Lexer.Token.CSR)] // CSR stvec = 0x105  (Supervisor trap handler base address.)
        [TestCase("scounteren", Lexer.Token.CSR)] // CSR scounteren = 0x106  (Supervisor counter enable.)
        [TestCase("sscratch", Lexer.Token.CSR)] // CSR sscratch = 0x140  (Scratch register for supervisor trap handlers.)
        [TestCase("sepc", Lexer.Token.CSR)] // CSR sepc = 0x141  (Supervisor exception program counter.)
        [TestCase("scause", Lexer.Token.CSR)] // CSR scause = 0x142  (Supervisor trap cause.)
        [TestCase("stval", Lexer.Token.CSR)] // CSR stval = 0x143  (Supervisor bad address or instruction.)
        [TestCase("sip", Lexer.Token.CSR)] // CSR sip = 0x144  (Supervisor interrupt pending.)
        [TestCase("satp", Lexer.Token.CSR)] // CSR satp = 0x180  (Supervisor address translation and protection.)
        [TestCase("scontext", Lexer.Token.CSR)] // CSR scontext = 0x5A8  (Supervisor-mode context register.)
        [TestCase("hstatus", Lexer.Token.CSR)] // CSR hstatus = 0x600  (Hypervisor status register.)
        [TestCase("hedeleg", Lexer.Token.CSR)] // CSR hedeleg = 0x602  (Hypervisor exception delegation register.)
        [TestCase("hideleg", Lexer.Token.CSR)] // CSR hideleg = 0x603  (Hypervisor interrupt delegation register.)
        [TestCase("hie", Lexer.Token.CSR)] // CSR hie = 0x604  (Hypervisor interrupt-enable register.)
        [TestCase("hcounteren", Lexer.Token.CSR)] // CSR hcounteren = 0x606  (Hypervisor counter enable.)
        [TestCase("htegie", Lexer.Token.CSR)] // CSR htegie = 0x607  (Hypervisor guest external interrupt-enable register )
        [TestCase("htval", Lexer.Token.CSR)] // CSR htval = 0x643  (Hypervisor bad address or instruction.)
        [TestCase("hip", Lexer.Token.CSR)] // CSR hip = 0x644  (Hypervisor interrupt pending.)
        [TestCase("hvip", Lexer.Token.CSR)] // CSR hvip = 0x645  (Hypervisor virtual interrupt pending.)
        [TestCase("htinst", Lexer.Token.CSR)] // CSR htinst = 0x64A  (Hypervisor trap instruction (transformed))
        [TestCase("hgeip", Lexer.Token.CSR)] // CSR hgeip = 0xE12  (Hypervisor guest external interrupt pending.)
        [TestCase("hgatp", Lexer.Token.CSR)] // CSR hgatp = 0x680  (Hypervisor guest address translation and protection.)
        [TestCase("hcontext", Lexer.Token.CSR)] // CSR hcontext = 0x6A8  (Hypervisor-mode context register.)
        [TestCase("htimedelta", Lexer.Token.CSR)] // CSR htimedelta = 0x605  (Delta for VS/VU-mode timer.)
        [TestCase("htimedeltah", Lexer.Token.CSR)] // CSR htimedeltah = 0x615  (Upper 32 bits of htimedelta, RV32 only.)
        [TestCase("vsstatus", Lexer.Token.CSR)] // CSR vsstatus = 0x200  (Virtual supervisor status register )
        [TestCase("vsie", Lexer.Token.CSR)] // CSR vsie = 0x204  (Virtual supervisor interrupt-enable register )
        [TestCase("vstvec", Lexer.Token.CSR)] // CSR vstvec = 0x205  (Virtual supervisor trap handler base address.)
        [TestCase("vsscratch", Lexer.Token.CSR)] // CSR vsscratch = 0x240  (Virtual supervisor scratch register.)
        [TestCase("vsepc", Lexer.Token.CSR)] // CSR vsepc = 0x241  (Virtual supervisor exception program counter.)
        [TestCase("vscause", Lexer.Token.CSR)] // CSR vscause = 0x242  (Virtual supervisor trap cause.)
        [TestCase("vstval", Lexer.Token.CSR)] // CSR vstval = 0x243  (Virtual supervisor bad address or instruction.)
        [TestCase("vsip", Lexer.Token.CSR)] // CSR vsip = 0x244  (Virtual supervisor interrupt pending.)
        [TestCase("bsatp", Lexer.Token.CSR)] // CSR bsatp = 0x280  (Supervisor address translation and protection.)
        [TestCase("mvendorid", Lexer.Token.CSR)] // CSR mvendorid = 0xF11  (Vendor ID.)
        [TestCase("marchid", Lexer.Token.CSR)] // CSR marchid = 0xF12  (Architecture ID.)
        [TestCase("mimpid", Lexer.Token.CSR)] // CSR mimpid = 0xF13  (Implementation ID.)
        [TestCase("mhartid", Lexer.Token.CSR)] // CSR mhartid = 0xF14  (Hardware thread ID.)
        [TestCase("mstatus", Lexer.Token.CSR)] // CSR mstatus = 0x300  (Machine status register.)
        [TestCase("misa", Lexer.Token.CSR)] // CSR misa = 0x301  (ISA and extentions )
        [TestCase("medeleg", Lexer.Token.CSR)] // CSR medeleg = 0x302  (Machine exception delegation register.)
        [TestCase("mideleg", Lexer.Token.CSR)] // CSR mideleg = 0x303  (Machine interrupt delegation register.)
        [TestCase("mie", Lexer.Token.CSR)] // CSR mie = 0x304  (Machine interrupt-enable register.)
        [TestCase("mtvec", Lexer.Token.CSR)] // CSR mtvec = 0x305  (Machine trap-handler base address.)
        [TestCase("mcounteren", Lexer.Token.CSR)] // CSR mcounteren = 0x306  (Machine counter enable.)
        [TestCase("mstatush", Lexer.Token.CSR)] // CSR mstatush = 0x310  (Additional machine status register, RV32 only.)
        [TestCase("mscratch", Lexer.Token.CSR)] // CSR mscratch = 0x340  (Scratch register for machine trap handlers.)
        [TestCase("mepc", Lexer.Token.CSR)] // CSR mepc = 0x341  (Machine exception program counter.)
        [TestCase("hcause", Lexer.Token.CSR)] // CSR hcause = 0x342  (Machine trap cause.)
        [TestCase("htval", Lexer.Token.CSR)] // CSR htval = 0x343  (Machine bad address or instruction.)
        [TestCase("hip", Lexer.Token.CSR)] // CSR hip = 0x344  (Machine interrupt pending.)
        [TestCase("htinst", Lexer.Token.CSR)] // CSR htinst = 0x34A  (Machine trap instruction (transformed))
        [TestCase("htval2", Lexer.Token.CSR)] // CSR htval2 = 0x34B  (Machine guest bad address or instruction.)
        [TestCase("pmpcfg0", Lexer.Token.CSR)] // CSR pmpcfg0 = 0x3A0  (Physical memory protection configuration.)
        [TestCase("pmpcfg1", Lexer.Token.CSR)] // CSR pmpcfg1 = 0x3A1  (Physical memory protection configuration, RV32 only.)
        [TestCase("pmpcfg2", Lexer.Token.CSR)] // CSR pmpcfg2 = 0x3A2  (Physical memory protection configuration.)
        [TestCase("pmpcfg3", Lexer.Token.CSR)] // CSR pmpcfg3 = 0x3A3  (Physical memory protection configuration, RV32 only.)
        [TestCase("pmpcfg14", Lexer.Token.CSR)] // CSR pmpcfg14 = 0x3AE  (Physical memory protection configuration.)
        [TestCase("pmpcfg15", Lexer.Token.CSR)] // CSR pmpcfg15 = 0x3AF  (Physical memory protection configuration, RV32 only.)
        [TestCase("pmpaddr0", Lexer.Token.CSR)] // CSR pmpaddr0 = 0x3B0  (Physical memory protection address configuration.)
        [TestCase("pmpaddr1", Lexer.Token.CSR)] // CSR pmpaddr1 = 0x3B1  (Physical memory protection address configuration.)
        [TestCase("pmpaddr63", Lexer.Token.CSR)] // CSR pmpaddr63 = 0x3EF  (Physical memory protection address configuration.)
        [TestCase("cycle", Lexer.Token.CSR)] // CSR cycle = 0xB00  (Cycle counter for RDCYCLE instruction.)
        [TestCase("instret", Lexer.Token.CSR)] // CSR instret = 0xB02  (Instructions-retired counter for RDINSTRET instruction )
        [TestCase("hpmcounter3", Lexer.Token.CSR)] // CSR hpmcounter3 = 0xB03  (Performance-monitoring counter.)
        [TestCase("hpmcounter4", Lexer.Token.CSR)] // CSR hpmcounter4 = 0xB04  (Performance-monitoring counter.)
        [TestCase("hpmcounter31", Lexer.Token.CSR)] // CSR hpmcounter31 = 0xB1F  (Performance-monitoring counter.)
        [TestCase("cycleh", Lexer.Token.CSR)] // CSR cycleh = 0xB80  (Upper 32 bits of cycle, RV32 only )
        [TestCase("instreth", Lexer.Token.CSR)] // CSR instreth = 0xB82  (Upper 32 bits of instret, RV32 only )
        [TestCase("hpmcounter3h", Lexer.Token.CSR)] // CSR hpmcounter3h = 0xB83  (Upper 32 bits of hpmcounter3, RV32 only )
        [TestCase("hpmcounter4h", Lexer.Token.CSR)] // CSR hpmcounter4h = 0xB84  (Upper 32 bits of hpmcounter4, RV32 only )
        [TestCase("hpmcounter31h", Lexer.Token.CSR)] // CSR hpmcounter31h = 0xB9F  (Upper 32 bits of hpmcounter31, RV32 only )
        [TestCase("mcountinhibit", Lexer.Token.CSR)] // CSR mcountinhibit = 0x320  (Machine counter-inhibit register.)
        [TestCase("mhpmevent3", Lexer.Token.CSR)] // CSR mhpmevent3 = 0x323  (Machine performance-monitoring event selector.)
        [TestCase("mhpmevent4", Lexer.Token.CSR)] // CSR mhpmevent4 = 0x324  (Machine performance-monitoring event selector.)
        [TestCase("mhpmevent31", Lexer.Token.CSR)] // CSR mhpmevent31 = 0x33F  (Machine performance-monitoring event selector.)
        [TestCase("tselect", Lexer.Token.CSR)] // CSR tselect = 0x7A0  (Debug/Trace trigger register select.)
        [TestCase("tdata1", Lexer.Token.CSR)] // CSR tdata1 = 0x7A1  (First Debug/Trace trigger data register.)
        [TestCase("tdata2", Lexer.Token.CSR)] // CSR tdata2 = 0x7A2  (Sedcond Debug/Trace trigger data register.)
        [TestCase("tdata3", Lexer.Token.CSR)] // CSR tdata3 = 0x7A3  (Third Debug/Trace trigger data register.)
        [TestCase("mcontext", Lexer.Token.CSR)] // CSR mcontext = 0x7A8  (Machine-mode context register.)
        [TestCase("dcsr", Lexer.Token.CSR)] // CSR dcsr = 0x7B0  (Debug control and status register.)
        [TestCase("dpc", Lexer.Token.CSR)] // CSR dpc = 0x7B1  (Debug PC.)
        [TestCase("dscratch0", Lexer.Token.CSR)] // CSR dscratch0 = 0x7B2  (Debug scratch register 0.)
        [TestCase("dscratch1", Lexer.Token.CSR)] // CSR dscratch1 = 0x7B3  (Debug scratch register 1.)

        public void sanityCheckOps(string test, Lexer.Token token) {
            readToken(test, token, test);
            readToken(test.ToLower(), token, test);
            readToken(test.ToUpper(), token, test);
        }

        // ----------------------------------------------------------------
        // RISC-V Registers
        // ---------------------------------------------------------------
        //TODO: this should likely move to the parser but not sure I like the idea of it in the tokenizer
        [TestCase("x00", Lexer.Token.REGISTER, "x0")]
        [TestCase("x01", Lexer.Token.REGISTER, "x1")]
        [TestCase("x02", Lexer.Token.REGISTER, "x2")]
        [TestCase("x03", Lexer.Token.REGISTER, "x3")]
        [TestCase("x04", Lexer.Token.REGISTER, "x4")]
        [TestCase("x05", Lexer.Token.REGISTER, "x5")]
        [TestCase("x06", Lexer.Token.REGISTER, "x6")]
        [TestCase("x07", Lexer.Token.REGISTER, "x7")]
        [TestCase("x08", Lexer.Token.REGISTER, "x8")]
        [TestCase("x09", Lexer.Token.REGISTER, "x9")]
        [TestCase("zero", Lexer.Token.REGISTER, "x0")]
        [TestCase("ra", Lexer.Token.REGISTER, "x1")]
        [TestCase("sp", Lexer.Token.REGISTER, "x2")]
        [TestCase("gp", Lexer.Token.REGISTER, "x3")]
        [TestCase("tp", Lexer.Token.REGISTER, "x4")]
        [TestCase("t0", Lexer.Token.REGISTER, "x5")]
        [TestCase("t1", Lexer.Token.REGISTER, "x6")]
        [TestCase("t2", Lexer.Token.REGISTER, "x7")]
        [TestCase("s0", Lexer.Token.REGISTER, "x8")]
        [TestCase("fp", Lexer.Token.REGISTER, "x8")]
        [TestCase("s1", Lexer.Token.REGISTER, "x9")]
        [TestCase("a0", Lexer.Token.REGISTER, "x10")]
        [TestCase("a1", Lexer.Token.REGISTER, "x11")]
        [TestCase("a2", Lexer.Token.REGISTER, "x12")]
        [TestCase("a3", Lexer.Token.REGISTER, "x13")]
        [TestCase("a4", Lexer.Token.REGISTER, "x14")]
        [TestCase("a5", Lexer.Token.REGISTER, "x15")]
        [TestCase("a6", Lexer.Token.REGISTER, "x16")]
        [TestCase("a7", Lexer.Token.REGISTER, "x17")]
        [TestCase("s2", Lexer.Token.REGISTER, "x18")]
        [TestCase("s3", Lexer.Token.REGISTER, "x19")]
        [TestCase("s4", Lexer.Token.REGISTER, "x20")]
        [TestCase("s5", Lexer.Token.REGISTER, "x21")]
        [TestCase("s6", Lexer.Token.REGISTER, "x22")]
        [TestCase("s7", Lexer.Token.REGISTER, "x23")]
        [TestCase("s8", Lexer.Token.REGISTER, "x24")]
        [TestCase("s9", Lexer.Token.REGISTER, "x25")]
        [TestCase("s10", Lexer.Token.REGISTER, "x26")]
        [TestCase("s11", Lexer.Token.REGISTER, "x27")]
        [TestCase("t3", Lexer.Token.REGISTER, "x28")]
        [TestCase("t4", Lexer.Token.REGISTER, "x29")]
        [TestCase("t5", Lexer.Token.REGISTER, "x30")]
        [TestCase("t6", Lexer.Token.REGISTER, "x31")]
        [TestCase("s01", Lexer.Token.REGISTER, "x9")]
        [TestCase("a00", Lexer.Token.REGISTER, "x10")]
        [TestCase("a01", Lexer.Token.REGISTER, "x11")]
        [TestCase("a02", Lexer.Token.REGISTER, "x12")]
        [TestCase("a03", Lexer.Token.REGISTER, "x13")]
        [TestCase("a04", Lexer.Token.REGISTER, "x14")]
        [TestCase("a05", Lexer.Token.REGISTER, "x15")]
        [TestCase("a06", Lexer.Token.REGISTER, "x16")]
        [TestCase("a07", Lexer.Token.REGISTER, "x17")]
        [TestCase("s02", Lexer.Token.REGISTER, "x18")]
        [TestCase("s03", Lexer.Token.REGISTER, "x19")]
        [TestCase("s04", Lexer.Token.REGISTER, "x20")]
        [TestCase("s05", Lexer.Token.REGISTER, "x21")]
        [TestCase("s06", Lexer.Token.REGISTER, "x22")]
        [TestCase("s07", Lexer.Token.REGISTER, "x23")]
        [TestCase("s08", Lexer.Token.REGISTER, "x24")]
        [TestCase("s09", Lexer.Token.REGISTER, "x25")]
        [TestCase("t03", Lexer.Token.REGISTER, "x28")]
        [TestCase("t04", Lexer.Token.REGISTER, "x29")]
        [TestCase("t05", Lexer.Token.REGISTER, "x30")]
        [TestCase("t06", Lexer.Token.REGISTER, "x31")]

        // ----------------------------------------------------------------
        // General RISC-V ASM Tests
        // ---------------------------------------------------------------
        [TestCase(".text", Lexer.Token.DIRECTIVE, ".text")]
        [TestCase("     .text", Lexer.Token.WHITESPACE, "     ")]
        [TestCase("yolo:", Lexer.Token.LABEL, "yolo")]
        [TestCase("yolo", Lexer.Token.IDENTIFIER, "yolo")]
        [TestCase(".string \"str\"", Lexer.Token.DIRECTIVE, ".string")]
        [TestCase("\"str\"", Lexer.Token.STRING, "\"str\"")]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", Lexer.Token.DIRECTIVE, ".byte")]
        [TestCase("0xFF, 0xf2, 0x02, 0x85, 0x05", Lexer.Token.NUMBER_HEX, "0xFF")]
        [TestCase("0xf2, 0x02, 0x85, 0x05", Lexer.Token.NUMBER_HEX, "0xf2")]
        [TestCase("0x02, 0x85, 0x05", Lexer.Token.NUMBER_HEX, "0x02")]
        [TestCase("0x85, 0x05", Lexer.Token.NUMBER_HEX, "0x85")]
        [TestCase("0x05", Lexer.Token.NUMBER_HEX, "0x05")]
        [TestCase(".half 0xFFf2, 0x0285, 0x0563", Lexer.Token.DIRECTIVE, ".half")]
        [TestCase(".word 0xFAB3EA23, 0x63424253, 0x2535A244", Lexer.Token.DIRECTIVE, ".word")]
        [TestCase(".dword 0xFAB3EA2363424253, 0x634242532535A244", Lexer.Token.DIRECTIVE, ".dword")]
        [TestCase(".float 0.1f, 42.2f, 151326.52562f", Lexer.Token.DIRECTIVE, ".float")]
        [TestCase("0.1f, 42.2f, 151326.52562f", Lexer.Token.NUMBER_FLOAT, "0.1f")]
        [TestCase("42.2f, 151326.52562f", Lexer.Token.NUMBER_FLOAT, "42.2f")]
        [TestCase("-42.2f, 151326.52562f", Lexer.Token.NUMBER_FLOAT, "-42.2f")]
        [TestCase("151326.52562f", Lexer.Token.NUMBER_FLOAT, "151326.52562f")]
        [TestCase(".double 0.1d, 4.5d, 2.4d, 2414.125125d", Lexer.Token.DIRECTIVE, ".double")]
        [TestCase("0.1d, 4.5d, 2.4d, 2414.125125d", Lexer.Token.NUMBER_DOUBLE, "0.1d")]
        [TestCase("4.5d, 2.4d, 2414.125125d", Lexer.Token.NUMBER_DOUBLE, "4.5d")]
        [TestCase("2.4d, 2414.125125d", Lexer.Token.NUMBER_DOUBLE, "2.4d")]
        [TestCase("-2.4d, 2414.125125d", Lexer.Token.NUMBER_DOUBLE, "-2.4d")]
        [TestCase("2414.125125d", Lexer.Token.NUMBER_DOUBLE, "2414.125125d")]
        [TestCase("242", Lexer.Token.NUMBER_INT, "242")]
        [TestCase("-242", Lexer.Token.NUMBER_INT, "-242")]
        [TestCase(".option rvc", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("rvc", Lexer.Token.IDENTIFIER, "rvc")]
        [TestCase(".option norvc", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("norvc", Lexer.Token.IDENTIFIER, "norvc")]
        [TestCase(".option relax", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("relax", Lexer.Token.IDENTIFIER, "relax")]
        [TestCase(".option norelax", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("norelax", Lexer.Token.IDENTIFIER, "norelax")]
        [TestCase(".option pic", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("pic", Lexer.Token.IDENTIFIER, "pic")]
        [TestCase(".option nopic", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("nopic", Lexer.Token.IDENTIFIER, "nopic")]
        [TestCase(".option push", Lexer.Token.DIRECTIVE, ".option")]
        [TestCase("push", Lexer.Token.IDENTIFIER, "push")]

        [TestCase(", SLL", Lexer.Token.WHITESPACE, ", ")] // Bug fix
        [TestCase("SLL ,", Lexer.Token.OP_R, "SLL")] // Bug fix

        // ---------------------------------------------------------------
        // RISC-V ASM Number Format Tests
        // ---------------------------------------------------------------
        // Hex Tests (Base 16)
        [TestCase("0x0", Lexer.Token.NUMBER_HEX, "0x0")]
        [TestCase("0x1", Lexer.Token.NUMBER_HEX, "0x1")]
        [TestCase("0x2", Lexer.Token.NUMBER_HEX, "0x2")]
        [TestCase("0x3", Lexer.Token.NUMBER_HEX, "0x3")]
        [TestCase("0x4", Lexer.Token.NUMBER_HEX, "0x4")]
        [TestCase("0x5", Lexer.Token.NUMBER_HEX, "0x5")]
        [TestCase("0x6", Lexer.Token.NUMBER_HEX, "0x6")]
        [TestCase("0x7", Lexer.Token.NUMBER_HEX, "0x7")]
        [TestCase("0x8", Lexer.Token.NUMBER_HEX, "0x8")]
        [TestCase("0x9", Lexer.Token.NUMBER_HEX, "0x9")]
        [TestCase("0xa", Lexer.Token.NUMBER_HEX, "0xa")]
        [TestCase("0xb", Lexer.Token.NUMBER_HEX, "0xb")]
        [TestCase("0xc", Lexer.Token.NUMBER_HEX, "0xc")]
        [TestCase("0xd", Lexer.Token.NUMBER_HEX, "0xd")]
        [TestCase("0xe", Lexer.Token.NUMBER_HEX, "0xe")]
        [TestCase("0xf", Lexer.Token.NUMBER_HEX, "0xf")]
        [TestCase("0x10", Lexer.Token.NUMBER_HEX, "0x10")]
        [TestCase("0x11", Lexer.Token.NUMBER_HEX, "0x11")]
        // Binary Tests (Base 2)
        [TestCase("0b0", Lexer.Token.NUMBER_BIN, "0b0")]
        [TestCase("0b1", Lexer.Token.NUMBER_BIN, "0b1")]
        [TestCase("0b10", Lexer.Token.NUMBER_BIN, "0b10")]
        [TestCase("0b11", Lexer.Token.NUMBER_BIN, "0b11")]
        [TestCase("0b100", Lexer.Token.NUMBER_BIN, "0b100")]
        [TestCase("0b101", Lexer.Token.NUMBER_BIN, "0b101")]
        [TestCase("0b110", Lexer.Token.NUMBER_BIN, "0b110")]
        [TestCase("0b111", Lexer.Token.NUMBER_BIN, "0b111")]
        [TestCase("0b1000", Lexer.Token.NUMBER_BIN, "0b1000")]
        [TestCase("0b1001", Lexer.Token.NUMBER_BIN, "0b1001")]
        // Decimal Tests (Base 10)
        [TestCase("0", Lexer.Token.NUMBER_INT, "0")]
        [TestCase("1", Lexer.Token.NUMBER_INT, "1")]
        [TestCase("2", Lexer.Token.NUMBER_INT, "2")]
        [TestCase("3", Lexer.Token.NUMBER_INT, "3")]
        [TestCase("4", Lexer.Token.NUMBER_INT, "4")]
        [TestCase("5", Lexer.Token.NUMBER_INT, "5")]
        [TestCase("6", Lexer.Token.NUMBER_INT, "6")]
        [TestCase("7", Lexer.Token.NUMBER_INT, "7")]
        [TestCase("8", Lexer.Token.NUMBER_INT, "8")]
        [TestCase("9", Lexer.Token.NUMBER_INT, "9")]
        [TestCase("10", Lexer.Token.NUMBER_INT, "10")]
        [TestCase("11", Lexer.Token.NUMBER_INT, "11")]
        [TestCase("12", Lexer.Token.NUMBER_INT, "12")]
        [TestCase("13", Lexer.Token.NUMBER_INT, "13")]
        [TestCase("14", Lexer.Token.NUMBER_INT, "14")]
        // Floating Point Tests
        [TestCase("0.0f", Lexer.Token.NUMBER_FLOAT, "0.0f")]
        [TestCase("1.0f", Lexer.Token.NUMBER_FLOAT, "1.0f")]
        [TestCase("2.0f", Lexer.Token.NUMBER_FLOAT, "2.0f")]
        [TestCase("3.0f", Lexer.Token.NUMBER_FLOAT, "3.0f")]
        [TestCase("4.0f", Lexer.Token.NUMBER_FLOAT, "4.0f")]
        [TestCase("5.0f", Lexer.Token.NUMBER_FLOAT, "5.0f")]
        [TestCase("6.0f", Lexer.Token.NUMBER_FLOAT, "6.0f")]
        [TestCase("7.0f", Lexer.Token.NUMBER_FLOAT, "7.0f")]
        [TestCase("8.0f", Lexer.Token.NUMBER_FLOAT, "8.0f")]
        [TestCase("9.0f", Lexer.Token.NUMBER_FLOAT, "9.0f")]
        [TestCase("10.0f", Lexer.Token.NUMBER_FLOAT, "10.0f")]
        [TestCase("11.0f", Lexer.Token.NUMBER_FLOAT, "11.0f")]
        [TestCase("12.0f", Lexer.Token.NUMBER_FLOAT, "12.0f")]
        [TestCase("13.0f", Lexer.Token.NUMBER_FLOAT, "13.0f")]
        [TestCase("14.0f", Lexer.Token.NUMBER_FLOAT, "14.0f")]
        // %pcrel_hi(myVar)
        [TestCase("%pcrel_hi(myVar)", Lexer.Token.INLINE_DIRECTIVE, "pcrel_hi")]
        // %pcrel_lo(myVar)
        [TestCase("%pcrel_lo(myVar)", Lexer.Token.INLINE_DIRECTIVE, "pcrel_lo")]
        // %hi(myVar)
        [TestCase("%hi(myVar)", Lexer.Token.INLINE_DIRECTIVE, "hi")]
        // %lo(myVar)
        [TestCase("%lo(myVar)", Lexer.Token.INLINE_DIRECTIVE, "lo")]
        // GOT Tests are only relevant for PIC code so are not implemented for now but lets properly lex their tokens anyway
        // %got(myVar)
        [TestCase("%got(myVar)", Lexer.Token.INLINE_DIRECTIVE, "got")]
        // %got_pcrel(myVar)
        [TestCase("%got_pcrel(myVar)", Lexer.Token.INLINE_DIRECTIVE, "got_pcrel")]
        // %got_pcrel_hi(myVar)
        [TestCase("%got_pcrel_hi(myVar)", Lexer.Token.INLINE_DIRECTIVE, "got_pcrel_hi")]
        // %got_pcrel_lo(myVar)
        [TestCase("%got_pcrel_lo(myVar)", Lexer.Token.INLINE_DIRECTIVE, "got_pcrel_lo")]
        // TODO: Consider the rest %pcrel_hi() %pcrel_lo() %hi() %lo() %got() %gotpcrel() %tls_ie() %tls_gd() %tprel_hi() %tprel_lo() %dtprel_hi() %dtprel_lo() %gottprel() %call26() %call_lo() %call_hi() %call20() %call12() %call16() %callhi() %calllo() %add_pc() %got_disp() %got_lo() %got_hi() %gotoff_lo() %gotoff_hi() %gotpc_rel() %gotpc_lo() %gotpc_hi() %tls_gottprel() %tls_ldm() %tls_ldo() %tls_ie_ld() %tls_le() %tls_ie_le() %tls_tpoff() %tls_ie_tpoff() %tls_tprel_hi() %tls_tprel_lo() %tls_tpoff_hi() %tls_tpoff_lo() %tls_ld() %tls_ie_ld() %tls_ie_ldx() %tls_ie_ldx_hi() %tls_ie_ldx_lo() %tls_ie_add() %tls_ie_add_hi() %tls_ie_add_lo() %tls_gd_add() %tls_gd_add_hi() %tls_gd_add_lo() %tls_gd_ldm() %tls_gd_ldm_hi() %tls_gd_ldm_lo() %tls_gd_ldo() %tls_gd_addtprel() %tls_gd_addtprel_hi() %tls_gd_addtprel_lo() %tls_gd_tpoff() %tls_gd_tpoff_hi() %tls_gd_tpoff_lo() %tls_ie_tpoff() %tls_ie_tpoff_hi() %tls_ie_tpoff_lo() %tls_ie_ldp() %tls_ie_ldp_hi() %tls_ie_ldp_lo() %tls_ie_ldp_x() %tls_ie_ldp_x_hi() %tls_ie_ldp_x_lo() %tls_ie_ldp_pc() %tls_ie_ldp_pc_hi() %tls_ie_ldp_pc_lo() %tls_ie_ldp_got() %tls_ie_ldp_got_hi() %tls_ie_ldp_got_lo() %tls_ie_ldp_gottprel() %tls_ie_ldp_gottprel_hi() %tls_ie_ldp_gottprel_lo() %tls_ie_ldp_tpoff() %tls_ie


        public void readToken(string test, Lexer.Token token, string value) {
            tokenizer.Load(test);
            Lexer.TokenData tokenData = tokenizer.ReadToken();
            Assert.AreEqual(token, tokenData.token);
            Assert.AreEqual(value.ToLower(), tokenData.value.ToLower());
        }

        [Test]
        [TestCase(".text",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".text", default(string), default(string), default(string) }
        )]
        [TestCase("     .text",
            new Lexer.Token[] { Lexer.Token.WHITESPACE, Lexer.Token.DIRECTIVE, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { "     ", ".text", default(string), default(string), default(string) }
        )]
        [TestCase("yolo:",
            new Lexer.Token[] { Lexer.Token.LABEL, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { "yolo", default(string), default(string), default(string) }
        )]
        [TestCase("yolo",
            new Lexer.Token[] { Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { "yolo", default(string), default(string), default(string) }
        )]
        [TestCase(".string \"str\"",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.STRING, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".string", " ", "\"str\"", default(string), default(string), default(string) }
        )]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".byte", " ", "0xFF", ", ", "0xf2", ", ", "0x02", ", ", "0x85", ", ", "0x05", default(string), default(string), default(string) }
        )]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".byte", " ", "0xFF", ", ", "0xf2", ", ", "0x02", ", ", "0x85", ", ", "0x05", default(string), default(string), default(string) }
        )]
        [TestCase(".half 0xFFf2, 0x0285, 0x0563",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".half", " ", "0xFFf2", ", ", "0x0285", ", ", "0x0563", default(string), default(string), default(string) }
        )]
        [TestCase(".word 0xFAB3EA23, 0x63424253, 0x2535A244",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".word", " ", "0xFAB3EA23", ", ", "0x63424253", ", ", "0x2535A244", default(string), default(string), default(string) }
        )]
        [TestCase(".dword 0xFAB3EA2363424253, 0x634242532535A244",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_HEX, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".dword", " ", "0xFAB3EA2363424253", ", ", "0x634242532535A244", default(string), default(string), default(string) }
        )]
        [TestCase(".float 0.1f, 42.2f, 151326.52562f",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_FLOAT, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_FLOAT, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_FLOAT, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".float", " ", "0.1f", ", ", "42.2f", ", ", "151326.52562f", default(string), default(string), default(string) }
        )]
        [TestCase(".double 0.1d, 4.5d, 2.4d, 2414.125125d",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_DOUBLE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_DOUBLE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_DOUBLE, Lexer.Token.WHITESPACE, Lexer.Token.NUMBER_DOUBLE, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".double", " ", "0.1d", ", ", "4.5d", ", ", "2.4d", ", ", "2414.125125d", default(string), default(string), default(string) }
        )]
        [TestCase(".option rvc",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "rvc", default(string), default(string), default(string) }
        )]
        [TestCase(".option norvc",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "norvc", default(string), default(string), default(string) }
        )]
        [TestCase(".option relax",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "relax", default(string), default(string), default(string) }
        )]
        [TestCase(".option norelax",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "norelax", default(string), default(string), default(string) }
        )]
        [TestCase(".option pic",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "pic", default(string), default(string), default(string) }
        )]
        [TestCase(".option nopic",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "nopic", default(string), default(string), default(string) }
        )]
        [TestCase(".option push",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { ".option", " ", "push", default(string), default(string), default(string) }
        )]
        public void fullReadToken(string test, Lexer.Token[] tokens, string[] values) {
            tokenizer.Load(test);
            for(int i = 0; i < tokens.Length; i++) {
                Lexer.TokenData tokenData = tokenizer.ReadToken();
                Assert.AreEqual(tokens[i], tokenData.token);
                Assert.AreEqual(values[i], tokenData.value);
            }
        }

        [Test]
        [TestCase(" sh x2, 0(x1)",
            new Lexer.Token[] { Lexer.Token.OP_S, Lexer.Token.REGISTER, Lexer.Token.NUMBER_INT, Lexer.Token.PARREN_OPEN, Lexer.Token.REGISTER, Lexer.Token.PARREN_CLOSE, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { "SH", "x2", "0", "(", "x1", ")", default(string), default(string), default(string) }
        )]
        [TestCase(" sh x2, 0x10(x1)",
            new Lexer.Token[] { Lexer.Token.OP_S, Lexer.Token.REGISTER, Lexer.Token.NUMBER_HEX, Lexer.Token.PARREN_OPEN, Lexer.Token.REGISTER, Lexer.Token.PARREN_CLOSE, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new string[] { "SH", "x2", "0x10", "(", "x1", ")", default(string), default(string), default(string) }
        )]
        public void noWhiteSpaceReadTokens(string test, Lexer.Token[] tokens, string[] values) {
            tokenizer.Load(test);
            for(int i = 0; i < tokens.Length; i++) {
                Lexer.TokenData tokenData = tokenizer.ReadToken(true);
                Assert.AreEqual(tokens[i], tokenData.token);
                Assert.AreEqual(values[i], tokenData.value);
            }
        }

        [TestCase(".option push\n.option nopic",
            new Lexer.Token[] { Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOL, Lexer.Token.DIRECTIVE, Lexer.Token.WHITESPACE, Lexer.Token.IDENTIFIER, Lexer.Token.EOF, Lexer.Token.EOF, Lexer.Token.EOF },
            new int[] { 0, 0, 0,  0, 1, 1, 1,  1,  1,  1 },
            new int[] { 0, 7, 8, 12, 0, 7, 8, 13, 13, 13 },
            new string[] { ".option", " ", "push", "\n", ".option", " ", "nopic", default(string), default(string), default(string) }
        )]
        public void tokenPositions(string test, Lexer.Token[] tokens, int[]rows, int[] cols, string[] values) {
            tokenizer.Load(test);
            for(int i = 0; i < tokens.Length; i++) {
                Lexer.TokenData tokenData = tokenizer.ReadToken();
                Assert.AreEqual(tokens[i], tokenData.token, $"Token is #{i}");
                Assert.AreEqual(rows[i], tokenData.lineNumber, $"Token is #{i}");
                Assert.AreEqual(cols[i], tokenData.columnNumber, $"Token is #{i}");
                Assert.AreEqual(values[i], tokenData.value, $"Token is #{i}");
            }
        }

        // .string "str"
        // .byte b1, b2,...., bn
        // .half w1, w2,...., wn
        // .word w1, w2,...., wn
        // .dword w1, w2,...., wn
        // .float f1, f2,...., fn
        // .double d1, d2,...., dn
        // .option rvc
        // .option norvc
        // .option relax
        // .option norelax
        // .option pic
        // .option nopic
        // .option push
        /* .repeat
         * asm
         * .endr
         * this will not be implimented until later down the road
         * */
        // .option push
    }
}
