using Kore;
using Kore.RiscISA.Instruction;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KoreTests
{
    public static class KuickTokenizerTestUtils
    {
    }
    public class KuickTokenizerTest
    {
        private static Random rand = new Random();
        KuickTokenizer tokenizer = new KuickTokenizer();

        /*
        [TestCase(".text", new Type[] { typeof(KuickDirectiveToken)})] 
        [TestCase(".global _start", new KuickLexerToken[] { new KuickDirectiveToken(".text"), new KuickLexerToken("_start") })]
        [TestCase(".type _start, @function", new KuickLexerToken[] { new KuickDirectiveToken(".text"), new KuickLexerToken("_start"), new KuickLexerToken("@function") })]
*/

        // ----------------------------------------------------------------
        // RISC-V Registers
        // ---------------------------------------------------------------
        [TestCase("x0", KuickTokenizer.Token.REGISTER)]
        [TestCase("x1", KuickTokenizer.Token.REGISTER)]
        [TestCase("x2", KuickTokenizer.Token.REGISTER)]
        [TestCase("x3", KuickTokenizer.Token.REGISTER)]
        [TestCase("x4", KuickTokenizer.Token.REGISTER)]
        [TestCase("x5", KuickTokenizer.Token.REGISTER)]
        [TestCase("x6", KuickTokenizer.Token.REGISTER)]
        [TestCase("x7", KuickTokenizer.Token.REGISTER)]
        [TestCase("x8", KuickTokenizer.Token.REGISTER)]
        [TestCase("x9", KuickTokenizer.Token.REGISTER)]
        [TestCase("x10", KuickTokenizer.Token.REGISTER)]
        [TestCase("x11", KuickTokenizer.Token.REGISTER)]
        [TestCase("x12", KuickTokenizer.Token.REGISTER)]
        [TestCase("x13", KuickTokenizer.Token.REGISTER)]
        [TestCase("x14", KuickTokenizer.Token.REGISTER)]
        [TestCase("x15", KuickTokenizer.Token.REGISTER)]
        [TestCase("x16", KuickTokenizer.Token.REGISTER)]
        [TestCase("x17", KuickTokenizer.Token.REGISTER)]
        [TestCase("x18", KuickTokenizer.Token.REGISTER)]
        [TestCase("x19", KuickTokenizer.Token.REGISTER)]
        [TestCase("x20", KuickTokenizer.Token.REGISTER)]
        [TestCase("x21", KuickTokenizer.Token.REGISTER)]
        [TestCase("x22", KuickTokenizer.Token.REGISTER)]
        [TestCase("x23", KuickTokenizer.Token.REGISTER)]
        [TestCase("x24", KuickTokenizer.Token.REGISTER)]
        [TestCase("x25", KuickTokenizer.Token.REGISTER)]
        [TestCase("x26", KuickTokenizer.Token.REGISTER)]
        [TestCase("x27", KuickTokenizer.Token.REGISTER)]
        [TestCase("x28", KuickTokenizer.Token.REGISTER)]
        [TestCase("x29", KuickTokenizer.Token.REGISTER)]
        [TestCase("x30", KuickTokenizer.Token.REGISTER)]
        [TestCase("x31", KuickTokenizer.Token.REGISTER)]

        // ----------------------------------------------------------------
        // Base Integer Instructions: RV32I and RV64I
        // ----------------------------------------------------------------

        // Shifts RV32I
        [TestCase("SLL", KuickTokenizer.Token.OP_R)] // Shift Left Logical
        [TestCase("SLLI", KuickTokenizer.Token.OP_I)] // Shift Left Logical Immediate
        [TestCase("SRL", KuickTokenizer.Token.OP_R)] // Shift Right Logical
        [TestCase("SRLI", KuickTokenizer.Token.OP_I)] // Shift Right Logical Immediate
        [TestCase("SRA", KuickTokenizer.Token.OP_R)] // Shift Right Arithmetic
        [TestCase("SRAI", KuickTokenizer.Token.OP_I)] // Shift Right Arithmetic Immediate
        // Shifts RV64I
        [TestCase("SLLW", KuickTokenizer.Token.OP_R)] // Shift Left Logical
        [TestCase("SLLIW", KuickTokenizer.Token.OP_I)] // Shift Left Logical Immediate
        [TestCase("SRLW", KuickTokenizer.Token.OP_R)] // Shift Right Logical
        [TestCase("SRLIW", KuickTokenizer.Token.OP_I)] // Shift Right Logical Immediate
        [TestCase("SRAW", KuickTokenizer.Token.OP_R)] // Shift Right Arithmetic
        [TestCase("SRAIW", KuickTokenizer.Token.OP_I)] // Shift Right Arithmetic Immediate
        // Arithmetic RV32I
        [TestCase("ADD", KuickTokenizer.Token.OP_R)] // Add
        [TestCase("ADDI", KuickTokenizer.Token.OP_I)] // Add Immediate
        [TestCase("SUB", KuickTokenizer.Token.OP_R)] // Subtract
        [TestCase("LUI", KuickTokenizer.Token.OP_U)] // Load Upper Immediate
        [TestCase("AUIPC", KuickTokenizer.Token.OP_U)] // Add Upper Immediate to PC
        // Arithmetic RV64I
        [TestCase("ADDW", KuickTokenizer.Token.OP_R)] // Add
        [TestCase("ADDIW", KuickTokenizer.Token.OP_I)] // Add Immediate
        [TestCase("SUBW", KuickTokenizer.Token.OP_R)] // Subtract
        // Logical RV32I
        [TestCase("XOR", KuickTokenizer.Token.OP_R)] // Exclusive OR
        [TestCase("XORI", KuickTokenizer.Token.OP_I)] // Exclusive OR Immediate
        [TestCase("OR", KuickTokenizer.Token.OP_R)] // OR
        [TestCase("ORI", KuickTokenizer.Token.OP_I)] // OR Immediate
        [TestCase("AND", KuickTokenizer.Token.OP_R)] // AND
        [TestCase("ANDI", KuickTokenizer.Token.OP_I)] // AND Immediate
        // Compare RV32I
        [TestCase("SLT", KuickTokenizer.Token.OP_R)] // Set <
        [TestCase("SLTI", KuickTokenizer.Token.OP_I)] // Set < Immediate
        [TestCase("SLTU", KuickTokenizer.Token.OP_R)] // Set < Unsigned
        [TestCase("SLTIU", KuickTokenizer.Token.OP_I)] // Set < Immediate Unsigned
        // Branches RV32I
        [TestCase("BEQ", KuickTokenizer.Token.OP_B)] // Branch =
        [TestCase("BNE", KuickTokenizer.Token.OP_B)] // Branch !=
        [TestCase("BLT", KuickTokenizer.Token.OP_B)] // Branch <
        [TestCase("BGE", KuickTokenizer.Token.OP_B)] // Branch >=
        [TestCase("BLTU", KuickTokenizer.Token.OP_B)] // Branch < Unsigned
        [TestCase("BGEU", KuickTokenizer.Token.OP_B)] // Branch >= Unsigned
        // Jump & Link RV32I
        [TestCase("JAL", KuickTokenizer.Token.OP_J)] // Jump and Link
        [TestCase("JALR", KuickTokenizer.Token.OP_I)] // Jump and Link Register
        // Synch RV32I
        [TestCase("FENCE", KuickTokenizer.Token.OP_I)] // Synch thread
        [TestCase("FENCE.I", KuickTokenizer.Token.OP_I)] // Synch Instruction and Data
        // Environment RV32I
        [TestCase("ECALL", KuickTokenizer.Token.OP_I)] // CALL
        [TestCase("EBREAK", KuickTokenizer.Token.OP_I)] // BREAK
        // Control Status Register (CSR) RV32I
        [TestCase("CSRRW", KuickTokenizer.Token.OP_I)] // Read/Write
        [TestCase("CSRRS", KuickTokenizer.Token.OP_I)] // Read and Set Bit
        [TestCase("CSRRC", KuickTokenizer.Token.OP_I)] // Read and Clear Bit
        [TestCase("CSRRWI", KuickTokenizer.Token.OP_I)] // Read/Write Immediate
        [TestCase("CSRRSI", KuickTokenizer.Token.OP_I)] // Read and Set Immediate
        [TestCase("CSRRCI", KuickTokenizer.Token.OP_I)] // Read and Clear Immediate
        // Loads RV32I
        [TestCase("LB", KuickTokenizer.Token.OP_I)] // Load Byte
        [TestCase("LH", KuickTokenizer.Token.OP_I)] // Load Halfword
        [TestCase("LBU", KuickTokenizer.Token.OP_I)] // Load Byte Unsigned
        [TestCase("LHU", KuickTokenizer.Token.OP_I)] // Load Half Unsigned
        [TestCase("LW", KuickTokenizer.Token.OP_I)] // Load Word
        // Loads RV64I
        [TestCase("LWU", KuickTokenizer.Token.OP_I)] // Load Word Unsigned
        [TestCase("LD", KuickTokenizer.Token.OP_I)] // Load Double Word
        // Stores RV32I
        [TestCase("SB", KuickTokenizer.Token.OP_S)] // Store Byte
        [TestCase("SH", KuickTokenizer.Token.OP_S)] // Store Halfword
        [TestCase("SW", KuickTokenizer.Token.OP_S)] // Store Word
        // Stores RV64I
        [TestCase("SD", KuickTokenizer.Token.OP_S)] // Store Double Word
        
        // ----------------------------------------------------------------
        // RV Privileged Instructions
        // ----------------------------------------------------------------
        // Trap
        [TestCase("MRET", KuickTokenizer.Token.OP_R)] // Machine-mode trap return
        [TestCase("SRET", KuickTokenizer.Token.OP_R)] // Supervisor-mode trap return
        // Interrupt
        [TestCase("WFI", KuickTokenizer.Token.OP_R)] // Wait for interrupt
        // MMU
        [TestCase("SFENCE.VMA", KuickTokenizer.Token.OP_R)] // Virtual Memory FENCE
        
        // ----------------------------------------------------------------
        // The 60 Pseudoinstructions
        // ----------------------------------------------------------------
        [TestCase("NOP", KuickTokenizer.Token.OP_PSEUDO)] // No Operation
        [TestCase("NEG", KuickTokenizer.Token.OP_PSEUDO)] // Two's Complement
        [TestCase("NEGW", KuickTokenizer.Token.OP_PSEUDO)] // Two's Complement Word
        // --------
        [TestCase("SNEZ", KuickTokenizer.Token.OP_PSEUDO)] // Set if != zero
        [TestCase("SLTZ", KuickTokenizer.Token.OP_PSEUDO)] // Set if < zero
        [TestCase("SGTZ", KuickTokenizer.Token.OP_PSEUDO)] // Set if > zero
        // --------
        [TestCase("BEQZ", KuickTokenizer.Token.OP_PSEUDO)] // Branch if == zero
        [TestCase("BNEZ", KuickTokenizer.Token.OP_PSEUDO)] // Branch if != zero
        [TestCase("BLEZ", KuickTokenizer.Token.OP_PSEUDO)] // Branch if <= zero
        [TestCase("BGEZ", KuickTokenizer.Token.OP_PSEUDO)] // Branch if >= zero
        [TestCase("BLTZ", KuickTokenizer.Token.OP_PSEUDO)] // Branch if < zero
        [TestCase("BGTZ", KuickTokenizer.Token.OP_PSEUDO)] // Branch if > zero
        // --------
        [TestCase("J", KuickTokenizer.Token.OP_PSEUDO)] // Jump
        [TestCase("JR", KuickTokenizer.Token.OP_PSEUDO)] // Jump to Register value
        [TestCase("RET", KuickTokenizer.Token.OP_PSEUDO)] // Return from subroutine
        // --------
        [TestCase("TAIL", KuickTokenizer.Token.OP_PSEUDO)] // Tail call far-away subroutine
        // --------
        [TestCase("RDINSTRET[H]", KuickTokenizer.Token.OP_PSEUDO)] // Read instructions-retired counter
        [TestCase("RDCYCLE[H]", KuickTokenizer.Token.OP_PSEUDO)] // Read Cycle counter
        [TestCase("RDTIME[H]", KuickTokenizer.Token.OP_PSEUDO)] // Read realtime clock
        // --------
        [TestCase("csrr", KuickTokenizer.Token.OP_PSEUDO)] // Read CSR
        [TestCase("csrw", KuickTokenizer.Token.OP_PSEUDO)] // Write CSR
        [TestCase("csrs", KuickTokenizer.Token.OP_PSEUDO)] // Set bits in CSR
        [TestCase("csrc", KuickTokenizer.Token.OP_PSEUDO)] // Clear bits in CSR
        // --------
        [TestCase("CSRWI", KuickTokenizer.Token.OP_PSEUDO)] // Write CSR, Immediate
        [TestCase("CSRSI", KuickTokenizer.Token.OP_PSEUDO)] // Set bits in CSR, Immediate
        [TestCase("CSRCI", KuickTokenizer.Token.OP_PSEUDO)] // Clear bits in CSR, Immediate
        // --------
        [TestCase("FRCSR", KuickTokenizer.Token.OP_PSEUDO)] // Read FP control/status register
        [TestCase("FSCSR", KuickTokenizer.Token.OP_PSEUDO)] // Write FP control/status register
        // --------
        [TestCase("FRRM", KuickTokenizer.Token.OP_PSEUDO)] // Read FP rounding mode
        [TestCase("FSRM", KuickTokenizer.Token.OP_PSEUDO)] // Write FP rounding mode
        // --------
        [TestCase("FRFLAGS", KuickTokenizer.Token.OP_PSEUDO)] // Read FP exception flags
        [TestCase("FSFLAGS", KuickTokenizer.Token.OP_PSEUDO)] // Write FP exception flags
        // --------
        [TestCase("LLA", KuickTokenizer.Token.OP_PSEUDO)] // Load local address
        // --------
        [TestCase("LA", KuickTokenizer.Token.OP_PSEUDO)] // Load address
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("LB", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global byte
        [TestCase("LH", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global half
        [TestCase("LW", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global word
        [TestCase("LD", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Load global double
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("SB", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global byte
        [TestCase("SH", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global half
        [TestCase("SW", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global word
        [TestCase("SD", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Store global double
        // --------
        //TODO: Check if these also colide with RV32V/RV64V
        [TestCase("flw", KuickTokenizer.Token.OP_PSEUDO)] // Floating-point load global
        [TestCase("fld", KuickTokenizer.Token.OP_PSEUDO)] // Floating-point load global
        // --------
        //TODO: Check if these also colide with RV32V/RV64V
        [TestCase("fsw", KuickTokenizer.Token.OP_PSEUDO)] // Floating-point store global
        [TestCase("fsd", KuickTokenizer.Token.OP_PSEUDO)] // Floating-point store global
        // --------
        [TestCase("li", KuickTokenizer.Token.OP_PSEUDO)] // Load immediate
        [TestCase("mv", KuickTokenizer.Token.OP_PSEUDO)] // Copy register
        [TestCase("not", KuickTokenizer.Token.OP_PSEUDO)] // One's complement
        [TestCase("sext.w", KuickTokenizer.Token.OP_PSEUDO)] // Sign extend word
        [TestCase("seqz", KuickTokenizer.Token.OP_PSEUDO)] // Set if = zero
        // --------
        [TestCase("fmv.s", KuickTokenizer.Token.OP_PSEUDO)] // Copy single-precision register
        [TestCase("fabs.s", KuickTokenizer.Token.OP_PSEUDO)] // Single-precision absolute value
        [TestCase("fneg.s", KuickTokenizer.Token.OP_PSEUDO)] // Single-precision negate
        [TestCase("fmv.d", KuickTokenizer.Token.OP_PSEUDO)] // Copy double-precision register
        [TestCase("fabs.d", KuickTokenizer.Token.OP_PSEUDO)] // Double-precision absolute value
        [TestCase("fneg.d", KuickTokenizer.Token.OP_PSEUDO)] // Double-precision negate
        // --------
        [TestCase("bgt", KuickTokenizer.Token.OP_PSEUDO)] // Branch if >
        [TestCase("ble", KuickTokenizer.Token.OP_PSEUDO)] // Branch if <=
        [TestCase("bgtu", KuickTokenizer.Token.OP_PSEUDO)] // Branch if >, unsigned
        [TestCase("bleu", KuickTokenizer.Token.OP_PSEUDO)] // Branch if <=, unsigned
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("jal", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Jump and Link by offset
        [TestCase("jalr", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // Jump and Link to register value
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("call", KuickTokenizer.Token.OP_PSEUDO, Ignore="Duplicate Operation Name (Handeling in Parser)")] // call by offset
        // --------
        //TODO: figure out how I want to hand these cases because they conflict
        // Fornow I am just going to ignore the pseudoinstruction in the tokenizer and it will be handeled in the parser
        [TestCase("fence", KuickTokenizer.Token.OP_PSEUDO)] // Fence on all memory and I/O
        // --------
        [TestCase("fscsr", KuickTokenizer.Token.OP_PSEUDO)] // Swap FP control/status register
        [TestCase("fsrm", KuickTokenizer.Token.OP_PSEUDO)] // Swap FP rounding mode
        [TestCase("fsflags", KuickTokenizer.Token.OP_PSEUDO)] // Swap FP exception flags

        // ----------------------------------------------------------------
        // Multiply-Divide Instructions: RVM
        // ----------------------------------------------------------------
        // Multiply RV32M
        [TestCase("MUL", KuickTokenizer.Token.OP_R)] // Multiply
        [TestCase("MULH", KuickTokenizer.Token.OP_R)] // Multiply High
        [TestCase("MULHSU", KuickTokenizer.Token.OP_R)] // Multiply High Sign/Unsigned
        [TestCase("MULHU", KuickTokenizer.Token.OP_R)] // Multiply High Unsigned
        // Divide RV32M
        [TestCase("DIV", KuickTokenizer.Token.OP_R)] // Divide
        [TestCase("DIVU", KuickTokenizer.Token.OP_R)] // Divide Unsigned
        // Remainder RV32M
        [TestCase("REM", KuickTokenizer.Token.OP_R)] // Remainder
        [TestCase("REMU", KuickTokenizer.Token.OP_R)] // Remainder Unsigned
        // Multiply RV64M
        [TestCase("MULW", KuickTokenizer.Token.OP_R)] // Multiply Word
        // Divide RV64M
        [TestCase("DIVW", KuickTokenizer.Token.OP_R)] // Divide Word
        // Remainder RV64M
        [TestCase("REMW", KuickTokenizer.Token.OP_R)] // Remainder Word
        [TestCase("REMUW", KuickTokenizer.Token.OP_R)] // Remainder //TODO: Confirm this opcode I think it should be REMWU but the book "The RISC-V Reader" has it as REMUW so yeah

        // ----------------------------------------------------------------
        // Atomic Instructions: RVA
        // ----------------------------------------------------------------
        // Load RV32A
        [TestCase("LR.W", KuickTokenizer.Token.OP_R)] // Load Reserved Word
        // Load RV64A
        [TestCase("LR.D", KuickTokenizer.Token.OP_R)] // Load Reserved Double Word
        // Store RV32A
        [TestCase("SC.W", KuickTokenizer.Token.OP_R)] // Store Reserved Word
        // Store RV64A
        [TestCase("SC.D", KuickTokenizer.Token.OP_R)] // Store Reserved Double Word
        // Swap RV32A
        [TestCase("AMOSWAP.W", KuickTokenizer.Token.OP_R)] // 
        // Swap RV64A
        [TestCase("AMOSWAP.D", KuickTokenizer.Token.OP_R)] // 
        // Add RV32A
        [TestCase("AMOADD.W", KuickTokenizer.Token.OP_R)] // 
        // Add RV64AA
        [TestCase("AMOADD.D", KuickTokenizer.Token.OP_R)] // 
        // Logical RV32A
        [TestCase("AMOXOR.W", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOAND.W", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOOR.W", KuickTokenizer.Token.OP_R)] // 
        // Logical RV64A
        [TestCase("AMOXOR.D", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOAND.D", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOOR.D", KuickTokenizer.Token.OP_R)] // 
        // Min/Max RV32A
        [TestCase("AMOMIN.W", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOMAX.W", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOMINU.W", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOMAXU.W", KuickTokenizer.Token.OP_R)] // 
        // Min/Max RV64A
        [TestCase("AMOMIN.D", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOMAX.D", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOMINU.D", KuickTokenizer.Token.OP_R)] // 
        [TestCase("AMOMAXU.D", KuickTokenizer.Token.OP_R)] // 

        // ----------------------------------------------------------------
        // Two Optional Floating-Point Instructions: RVF & RVD
        // ----------------------------------------------------------------
        // [TestCase("AMOMAXU.D", KuickTokenizer.Token.OP_R)] // 

        // ----------------------------------------------------------------
        // Vector Instructions: RVV
        // ----------------------------------------------------------------

        // ----------------------------------------------------------------
        // Optional Compressed Instructions: RV32C
        // ----------------------------------------------------------------

        // ----------------------------------------------------------------
        // Optional Compressed Extention: RV64C
        // ----------------------------------------------------------------


        public void sanityCheckOps(string test, KuickTokenizer.Token token)
        {
            readToken(test, token, test);
        }

        // ----------------------------------------------------------------
        // RISC-V Registers
        // ---------------------------------------------------------------
        //TODO: this should likely move to the parser but not sure I like the idea of it in the tokenizer
        [TestCase("x00", KuickTokenizer.Token.REGISTER, "x0")]
        [TestCase("x01", KuickTokenizer.Token.REGISTER, "x1")]
        [TestCase("x02", KuickTokenizer.Token.REGISTER, "x2")]
        [TestCase("x03", KuickTokenizer.Token.REGISTER, "x3")]
        [TestCase("x04", KuickTokenizer.Token.REGISTER, "x4")]
        [TestCase("x05", KuickTokenizer.Token.REGISTER, "x5")]
        [TestCase("x06", KuickTokenizer.Token.REGISTER, "x6")]
        [TestCase("x07", KuickTokenizer.Token.REGISTER, "x7")]
        [TestCase("x08", KuickTokenizer.Token.REGISTER, "x8")]
        [TestCase("x09", KuickTokenizer.Token.REGISTER, "x9")]
        [TestCase("zero", KuickTokenizer.Token.REGISTER, "x0")]
        [TestCase("ra", KuickTokenizer.Token.REGISTER, "x1")]
        [TestCase("sp", KuickTokenizer.Token.REGISTER, "x2")]
        [TestCase("gp", KuickTokenizer.Token.REGISTER, "x3")]
        [TestCase("tp", KuickTokenizer.Token.REGISTER, "x4")]
        [TestCase("t0", KuickTokenizer.Token.REGISTER, "x5")]
        [TestCase("t1", KuickTokenizer.Token.REGISTER, "x6")]
        [TestCase("t2", KuickTokenizer.Token.REGISTER, "x7")]
        [TestCase("s0", KuickTokenizer.Token.REGISTER, "x8")]
        [TestCase("fp", KuickTokenizer.Token.REGISTER, "x8")]
        [TestCase("s1", KuickTokenizer.Token.REGISTER, "x9")]
        [TestCase("a0", KuickTokenizer.Token.REGISTER, "x10")]
        [TestCase("a1", KuickTokenizer.Token.REGISTER, "x11")]
        [TestCase("a2", KuickTokenizer.Token.REGISTER, "x12")]
        [TestCase("a3", KuickTokenizer.Token.REGISTER, "x13")]
        [TestCase("a4", KuickTokenizer.Token.REGISTER, "x14")]
        [TestCase("a5", KuickTokenizer.Token.REGISTER, "x15")]
        [TestCase("a6", KuickTokenizer.Token.REGISTER, "x16")]
        [TestCase("a7", KuickTokenizer.Token.REGISTER, "x17")]
        [TestCase("s2", KuickTokenizer.Token.REGISTER, "x18")]
        [TestCase("s3", KuickTokenizer.Token.REGISTER, "x19")]
        [TestCase("s4", KuickTokenizer.Token.REGISTER, "x20")]
        [TestCase("s5", KuickTokenizer.Token.REGISTER, "x21")]
        [TestCase("s6", KuickTokenizer.Token.REGISTER, "x22")]
        [TestCase("s7", KuickTokenizer.Token.REGISTER, "x23")]
        [TestCase("s8", KuickTokenizer.Token.REGISTER, "x24")]
        [TestCase("s9", KuickTokenizer.Token.REGISTER, "x25")]
        [TestCase("s10", KuickTokenizer.Token.REGISTER, "x26")]
        [TestCase("s11", KuickTokenizer.Token.REGISTER, "x27")]
        [TestCase("s12", KuickTokenizer.Token.REGISTER, "x28")]
        [TestCase("t04", KuickTokenizer.Token.REGISTER, "x29")]
        [TestCase("t05", KuickTokenizer.Token.REGISTER, "x30")]
        [TestCase("t06", KuickTokenizer.Token.REGISTER, "x31")]
        [TestCase("s01", KuickTokenizer.Token.REGISTER, "x9")]
        [TestCase("a00", KuickTokenizer.Token.REGISTER, "x10")]
        [TestCase("a01", KuickTokenizer.Token.REGISTER, "x11")]
        [TestCase("a02", KuickTokenizer.Token.REGISTER, "x12")]
        [TestCase("a03", KuickTokenizer.Token.REGISTER, "x13")]
        [TestCase("a04", KuickTokenizer.Token.REGISTER, "x14")]
        [TestCase("a05", KuickTokenizer.Token.REGISTER, "x15")]
        [TestCase("a06", KuickTokenizer.Token.REGISTER, "x16")]
        [TestCase("a07", KuickTokenizer.Token.REGISTER, "x17")]
        [TestCase("s02", KuickTokenizer.Token.REGISTER, "x18")]
        [TestCase("s03", KuickTokenizer.Token.REGISTER, "x19")]
        [TestCase("s04", KuickTokenizer.Token.REGISTER, "x20")]
        [TestCase("s05", KuickTokenizer.Token.REGISTER, "x21")]
        [TestCase("s06", KuickTokenizer.Token.REGISTER, "x22")]
        [TestCase("s07", KuickTokenizer.Token.REGISTER, "x23")]
        [TestCase("s08", KuickTokenizer.Token.REGISTER, "x24")]
        [TestCase("s09", KuickTokenizer.Token.REGISTER, "x25")]
        [TestCase("t04", KuickTokenizer.Token.REGISTER, "x29")]
        [TestCase("t05", KuickTokenizer.Token.REGISTER, "x30")]
        [TestCase("t06", KuickTokenizer.Token.REGISTER, "x31")]

        // ----------------------------------------------------------------
        // General RISC-V ASM Tests
        // ---------------------------------------------------------------
        [TestCase(".text", KuickTokenizer.Token.DIRECTIVE, ".text")]
        [TestCase("     .text", KuickTokenizer.Token.WHITESPACE, "     ")]
        [TestCase("yolo:", KuickTokenizer.Token.LABEL, "yolo:")]
        [TestCase("yolo", KuickTokenizer.Token.IDENTIFIER, "yolo")]
        [TestCase(".string \"str\"", KuickTokenizer.Token.DIRECTIVE, ".string")]
        [TestCase("\"str\"", KuickTokenizer.Token.STRING, "\"str\"")]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", KuickTokenizer.Token.DIRECTIVE, ".byte")]
        [TestCase("0xFF, 0xf2, 0x02, 0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0xFF")]
        [TestCase("0xf2, 0x02, 0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0xf2")]
        [TestCase("0x02, 0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0x02")]
        [TestCase("0x85, 0x05", KuickTokenizer.Token.NUMBER_HEX, "0x85")]
        [TestCase("0x05", KuickTokenizer.Token.NUMBER_HEX, "0x05")]
        [TestCase(".half 0xFFf2, 0x0285, 0x0563", KuickTokenizer.Token.DIRECTIVE, ".half")]
        [TestCase(".word 0xFAB3EA23, 0x63424253, 0x2535A244", KuickTokenizer.Token.DIRECTIVE, ".word")]
        [TestCase(".dword 0xFAB3EA2363424253, 0x634242532535A244", KuickTokenizer.Token.DIRECTIVE, ".dword")]
        [TestCase(".float 0.1f, 42.2f, 151326.52562f", KuickTokenizer.Token.DIRECTIVE, ".float")]
        [TestCase("0.1f, 42.2f, 151326.52562f", KuickTokenizer.Token.NUMBER_FLOAT, "0.1f")]
        [TestCase("42.2f, 151326.52562f", KuickTokenizer.Token.NUMBER_FLOAT, "42.2f")]
        [TestCase("151326.52562f", KuickTokenizer.Token.NUMBER_FLOAT, "151326.52562f")]
        [TestCase(".double 0.1d, 4.5d, 2.4d, 2414.125125d", KuickTokenizer.Token.DIRECTIVE, ".double")]
        [TestCase("0.1d, 4.5d, 2.4d, 2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "0.1d")]
        [TestCase("4.5d, 2.4d, 2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "4.5d")]
        [TestCase("2.4d, 2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "2.4d")]
        [TestCase("2414.125125d", KuickTokenizer.Token.NUMBER_DOUBLE, "2414.125125d")]
        [TestCase(".option rvc", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("rvc", KuickTokenizer.Token.IDENTIFIER, "rvc")]
        [TestCase(".option norvc", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("norvc", KuickTokenizer.Token.IDENTIFIER, "norvc")]
        [TestCase(".option relax", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("relax", KuickTokenizer.Token.IDENTIFIER, "relax")]
        [TestCase(".option norelax", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("norelax", KuickTokenizer.Token.IDENTIFIER, "norelax")]
        [TestCase(".option pic", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("pic", KuickTokenizer.Token.IDENTIFIER, "pic")]
        [TestCase(".option nopic", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("nopic", KuickTokenizer.Token.IDENTIFIER, "nopic")]
        [TestCase(".option push", KuickTokenizer.Token.DIRECTIVE, ".option")]
        [TestCase("push", KuickTokenizer.Token.IDENTIFIER, "push")]
        public void readToken(string test, KuickTokenizer.Token token, string value)
        {
            tokenizer.load(test);
            KuickTokenizer.TokenData tokenData = tokenizer.readToken();
            Assert.AreEqual(token, tokenData.token);
            Assert.AreEqual(value, tokenData.value);
        }

        [Test]
        [TestCase(".text",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { ".text", default(string), default(string), default(string) }
        )]
        [TestCase("     .text",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { "     ", ".text", default(string), default(string), default(string) }
        )]
        [TestCase("yolo:",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.LABEL, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { "yolo:", default(string), default(string), default(string) }
        )]
        [TestCase("yolo",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { "yolo", default(string), default(string), default(string) }
        )]
        [TestCase(".string \"str\"",
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.STRING, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF },
            new string[] { ".string", " ", "\"str\"", default(string), default(string), default(string) }
        )]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".byte"," ","0xFF",", ","0xf2",", ","0x02",", ","0x85",", ","0x05", default(string), default(string), default(string) }
        )]
        [TestCase(".byte 0xFF, 0xf2, 0x02, 0x85, 0x05", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".byte"," ","0xFF",", ","0xf2",", ","0x02",", ","0x85",", ","0x05", default(string), default(string), default(string) }
        )]
        [TestCase(".half 0xFFf2, 0x0285, 0x0563", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".half"," ","0xFFf2",", ","0x0285",", ","0x0563", default(string), default(string), default(string) }
        )]
        [TestCase(".word 0xFAB3EA23, 0x63424253, 0x2535A244", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".word"," ","0xFAB3EA23",", ","0x63424253",", ","0x2535A244", default(string), default(string), default(string) }
        )]
        [TestCase(".dword 0xFAB3EA2363424253, 0x634242532535A244", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_HEX, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".dword"," ","0xFAB3EA2363424253",", ","0x634242532535A244", default(string), default(string), default(string) }
        )]
        [TestCase(".float 0.1f, 42.2f, 151326.52562f", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_FLOAT, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_FLOAT, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_FLOAT, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".float"," ","0.1f",", ","42.2f",", ","151326.52562f", default(string), default(string), default(string) }
        )]
        [TestCase(".double 0.1d, 4.5d, 2.4d, 2414.125125d", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.NUMBER_DOUBLE, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".double"," ","0.1d",", ","4.5d",", ","2.4d",", ","2414.125125d", default(string), default(string), default(string) }
        )]
        [TestCase(".option rvc", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","rvc", default(string), default(string), default(string) }
        )]
        [TestCase(".option norvc", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","norvc", default(string), default(string), default(string) }
        )]
        [TestCase(".option relax", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","relax", default(string), default(string), default(string) }
        )]
        [TestCase(".option norelax", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","norelax", default(string), default(string), default(string) }
        )]
        [TestCase(".option pic", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","pic", default(string), default(string), default(string) }
        )]
        [TestCase(".option nopic", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","nopic", default(string), default(string), default(string) }
        )]
        [TestCase(".option push", 
            new KuickTokenizer.Token[] { KuickTokenizer.Token.DIRECTIVE, KuickTokenizer.Token.WHITESPACE, KuickTokenizer.Token.IDENTIFIER, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF, KuickTokenizer.Token.EOF }, 
            new string[] { ".option"," ","push", default(string), default(string), default(string) }
        )]
        public void fullReadToken(string test, KuickTokenizer.Token[] tokens, string[] values)
        {
            tokenizer.load(test);
            for (int i = 0; i < tokens.Length; i++)
            {
                KuickTokenizer.TokenData tokenData = tokenizer.readToken();
                Assert.AreEqual(tokens[i], tokenData.token);
                Assert.AreEqual(values[i], tokenData.value);
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
