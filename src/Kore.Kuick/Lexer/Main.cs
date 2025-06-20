using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.Kuick
{
    public partial class Lexer
    {

        public TokenFinder[] Spec =
        {
            new TokenFinder(@"^# ?undef", Token.COMPILER_LOGIC), // `#undef` undefine the following sym //TODO: Impliment after v1
            new TokenFinder(@"^# ?define", Token.COMPILER_LOGIC), // `#define` define following sym //TODO: Impliment after v1
            new TokenFinder(@"^# ?ifdef", Token.COMPILER_LOGIC), // `#ifdef` if defined //TODO: Impliment after v1
            new TokenFinder(@"^# ?ifndef", Token.COMPILER_LOGIC), // `#ifndef` if not defined //TODO: Impliment after v1
            new TokenFinder(@"^# ?else", Token.COMPILER_LOGIC), // `#else` if else breaker //TODO: Impliment after v1
            new TokenFinder(@"^# ?endif", Token.COMPILER_LOGIC), // `#endif` end of a compiler if //TODO: Impliment after v1
            
            new TokenFinder(@"^\n", Token.EOL), // End of Line
            new TokenFinder(@"^[\s,]+", Token.WHITESPACE), // Whitespace
            new TokenFinder(@"^\/\/.*", Token.COMMENT), // Throw away // comments
            new TokenFinder(@"^#.*", Token.COMMENT), // Throw away # comments
            new TokenFinder(@"^\/\*[\s\S]*?\*\/", Token.COMMENT), // Throw away /* {ANY} */ comments
            new TokenFinder(@"^-?\d+\.?\d*[fF]", Token.NUMBER_FLOAT), // NUMBER_FLOAT 0.1f || 15f
            new TokenFinder(@"^-?\d+\.?\d*[dD]", Token.NUMBER_DOUBLE), // NUMBER_DOUBLE 0.12412D || 1245D
            new TokenFinder(@"^-?\d+\.\d+", Token.NUMBER_DOUBLE), // NUMBER_DOUBLE 0.12412 this finder is for when its a decimal with no marker
            new TokenFinder(@"^0x[\da-fA-F]+", Token.NUMBER_HEX), // NUMBER_HEX 0x1244
            new TokenFinder(@"^0b[01]+", Token.NUMBER_BIN), // NUMBER_BIN 0b101010
            new TokenFinder(@"^-?\d+", Token.NUMBER_INT), // NUMBER_INT 12578
            new TokenFinder(@"^""[^""]*""", Token.STRING), // " String //TODO: Make this allow escapes
            new TokenFinder(@"^'[^']*'", Token.STRING), // ' String //TODO: Make this allow escapes
            new TokenFinder(@"^\.[a-zA-Z]*", Token.DIRECTIVE), // Directive
            new TokenFinder(@"^[a-zA-Z]*:", Token.LABEL), // Label
            new TokenFinder(@"^[\w\[\]\._]+", Token.IDENTIFIER), // Identifier
            new TokenFinder(@"^\(", Token.PARREN_OPEN), // Open Parren (
            new TokenFinder(@"^\)", Token.PARREN_CLOSE), // Close Parren )
            // %pcrel_hi() %pcrel_lo() %hi() %lo() %got() %gotpcrel() %tls_ie() %tls_gd() %tprel_hi() %tprel_lo() %dtprel_hi() %dtprel_lo() %gottprel() %call26() %call_lo() %call_hi() %call20() %call12() %call16() %callhi() %calllo() %add_pc() %got_disp() %got_lo() %got_hi() %gotoff_lo() %gotoff_hi() %gotpc_rel() %gotpc_lo() %gotpc_hi() %tls_gottprel() %tls_ldm() %tls_ldo() %tls_ie_ld() %tls_le() %tls_ie_le() %tls_tpoff() %tls_ie_tpoff() %tls_tprel_hi() %tls_tprel_lo() %tls_tpoff_hi() %tls_tpoff_lo() %tls_ld() %tls_ie_ld() %tls_ie_ldx() %tls_ie_ldx_hi() %tls_ie_ldx_lo() %tls_ie_add() %tls_ie_add_hi() %tls_ie_add_lo() %tls_gd_add() %tls_gd_add_hi() %tls_gd_add_lo() %tls_gd_ldm() %tls_gd_ldm_hi() %tls_gd_ldm_lo() %tls_gd_ldo() %tls_gd_addtprel() %tls_gd_addtprel_hi() %tls_gd_addtprel_lo() %tls_gd_tpoff() %tls_gd_tpoff_hi() %tls_gd_tpoff_lo() %tls_ie_tpoff() %tls_ie_tpoff_hi() %tls_ie_tpoff_lo() %tls_ie_ldp() %tls_ie_ldp_hi() %tls_ie_ldp_lo() %tls_ie_ldp_x() %tls_ie_ldp_x_hi() %tls_ie_ldp_x_lo() %tls_ie_ldp_pc() %tls_ie_ldp_pc_hi() %tls_ie_ldp_pc_lo() %tls_ie_ldp_got() %tls_ie_ldp_got_hi() %tls_ie_ldp_got_lo() %tls_ie_ldp_gottprel() %tls_ie_ldp_gottprel_hi() %tls_ie_ldp_gottprel_lo() %tls_ie_ldp_tpoff() %tls_ie
            new TokenFinder(@"^%[a-zA-Z_]+", Token.INLINE_DIRECTIVE), // Inline Directive
        };

        Replacement[] REGISTER_REPLACEMENTS = new Replacement[] {
            new Replacement("x0", "x0"), 
            new Replacement("x1", "x1"), 
            new Replacement("x2", "x2"), 
            new Replacement("x3", "x3"), 
            new Replacement("x4", "x4"), 
            new Replacement("x5", "x5"), 
            new Replacement("x6", "x6"), 
            new Replacement("x7", "x7"), 
            new Replacement("x8", "x8"), 
            new Replacement("x9", "x9"), 
            new Replacement("x10", "x10"), 
            new Replacement("x11", "x11"), 
            new Replacement("x12", "x12"), 
            new Replacement("x13", "x13"), 
            new Replacement("x14", "x14"), 
            new Replacement("x15", "x15"), 
            new Replacement("x16", "x16"), 
            new Replacement("x17", "x17"), 
            new Replacement("x18", "x18"), 
            new Replacement("x19", "x19"), 
            new Replacement("x20", "x20"), 
            new Replacement("x21", "x21"), 
            new Replacement("x22", "x22"), 
            new Replacement("x23", "x23"), 
            new Replacement("x24", "x24"), 
            new Replacement("x25", "x25"), 
            new Replacement("x26", "x26"), 
            new Replacement("x27", "x27"), 
            new Replacement("x28", "x28"), 
            new Replacement("x29", "x29"), 
            new Replacement("x30", "x30"), 
            new Replacement("x31", "x31"), 
            new Replacement("x00", "x0"), 
            new Replacement("x01", "x1"), 
            new Replacement("x02", "x2"), 
            new Replacement("x03", "x3"), 
            new Replacement("x04", "x4"), 
            new Replacement("x05", "x5"), 
            new Replacement("x06", "x6"), 
            new Replacement("x07", "x7"), 
            new Replacement("x08", "x8"), 
            new Replacement("x09", "x9"), 
            new Replacement("zero", "x0"), 
            new Replacement("ra", "x1"), 
            new Replacement("sp", "x2"), 
            new Replacement("gp", "x3"), 
            new Replacement("tp", "x4"), 
            new Replacement("t0", "x5"), 
            new Replacement("t1", "x6"), 
            new Replacement("t2", "x7"), 
            new Replacement("s0", "x8"), 
            new Replacement("fp", "x8"), 
            new Replacement("s1", "x9"), 
            new Replacement("a0", "x10"), 
            new Replacement("a1", "x11"), 
            new Replacement("a2", "x12"), 
            new Replacement("a3", "x13"), 
            new Replacement("a4", "x14"), 
            new Replacement("a5", "x15"), 
            new Replacement("a6", "x16"), 
            new Replacement("a7", "x17"), 
            new Replacement("s2", "x18"), 
            new Replacement("s3", "x19"), 
            new Replacement("s4", "x20"), 
            new Replacement("s5", "x21"), 
            new Replacement("s6", "x22"), 
            new Replacement("s7", "x23"), 
            new Replacement("s8", "x24"), 
            new Replacement("s9", "x25"), 
            new Replacement("s10", "x26"), 
            new Replacement("s11", "x27"), 
            new Replacement("t3", "x28"), 
            new Replacement("t4", "x29"), 
            new Replacement("t5", "x30"), 
            new Replacement("t6", "x31"),
            new Replacement("s01", "x9"), 
            new Replacement("a00", "x10"), 
            new Replacement("a01", "x11"), 
            new Replacement("a02", "x12"), 
            new Replacement("a03", "x13"), 
            new Replacement("a04", "x14"), 
            new Replacement("a05", "x15"), 
            new Replacement("a06", "x16"), 
            new Replacement("a07", "x17"), 
            new Replacement("s02", "x18"), 
            new Replacement("s03", "x19"), 
            new Replacement("s04", "x20"), 
            new Replacement("s05", "x21"), 
            new Replacement("s06", "x22"), 
            new Replacement("s07", "x23"), 
            new Replacement("s08", "x24"), 
            new Replacement("s09", "x25"), 
            new Replacement("t03", "x28"), 
            new Replacement("t04", "x29"), 
            new Replacement("t05", "x30"), 
            new Replacement("t06", "x31"),
            // Floating-point registers
            new Replacement("f0", "f0"), 
            new Replacement("f1", "f1"), 
            new Replacement("f2", "f2"), 
            new Replacement("f3", "f3"), 
            new Replacement("f4", "f4"), 
            new Replacement("f5", "f5"), 
            new Replacement("f6", "f6"), 
            new Replacement("f7", "f7"), 
            new Replacement("f8", "f8"), 
            new Replacement("f9", "f9"), 
            new Replacement("f10", "f10"), 
            new Replacement("f11", "f11"), 
            new Replacement("f12", "f12"), 
            new Replacement("f13", "f13"), 
            new Replacement("f14", "f14"), 
            new Replacement("f15", "f15"), 
            new Replacement("f16", "f16"), 
            new Replacement("f17", "f17"), 
            new Replacement("f18", "f18"), 
            new Replacement("f19", "f19"), 
            new Replacement("f20", "f20"), 
            new Replacement("f21", "f21"), 
            new Replacement("f22", "f22"), 
            new Replacement("f23", "f23"), 
            new Replacement("f24", "f24"), 
            new Replacement("f25", "f25"), 
            new Replacement("f26", "f26"), 
            new Replacement("f27", "f27"), 
            new Replacement("f28", "f28"), 
            new Replacement("f29", "f29"), 
            new Replacement("f30", "f30"), 
            new Replacement("f31", "f31"),
            // Floating-point register aliases
            new Replacement("ft0", "f0"), 
            new Replacement("ft1", "f1"), 
            new Replacement("ft2", "f2"), 
            new Replacement("ft3", "f3"), 
            new Replacement("ft4", "f4"), 
            new Replacement("ft5", "f5"), 
            new Replacement("ft6", "f6"), 
            new Replacement("ft7", "f7"), 
            new Replacement("fs0", "f8"), 
            new Replacement("fs1", "f9"), 
            new Replacement("fa0", "f10"), 
            new Replacement("fa1", "f11"), 
            new Replacement("fa2", "f12"), 
            new Replacement("fa3", "f13"), 
            new Replacement("fa4", "f14"), 
            new Replacement("fa5", "f15"), 
            new Replacement("fa6", "f16"), 
            new Replacement("fa7", "f17"), 
            new Replacement("fs2", "f18"), 
            new Replacement("fs3", "f19"), 
            new Replacement("fs4", "f20"), 
            new Replacement("fs5", "f21"), 
            new Replacement("fs6", "f22"), 
            new Replacement("fs7", "f23"), 
            new Replacement("fs8", "f24"), 
            new Replacement("fs9", "f25"), 
            new Replacement("fs10", "f26"), 
            new Replacement("fs11", "f27"), 
            new Replacement("ft8", "f28"), 
            new Replacement("ft9", "f29"), 
            new Replacement("ft10", "f30"), 
            new Replacement("ft11", "f31") };
        Dictionary<Token, string[]> OP_TYPES = new Dictionary<Token, string[]>()
        {
            [Token.OP_R] = new string[] { 
                "SLL", "SRL", "SRA", "SLLW", "SRLW", "SRAW", "ADD", "SUB", 
                "ADDW", "SUBW", "XOR", "OR", "AND", "SLT", "SLTU", "MRET", 
                "SRET", "WFI", "SFENCE.VMA", "MUL", "MULH", "MULHSU", "MULHU", 
                "DIV", "DIVU", "REM", "REMU", "MULW", "DIVW", "REMW", "REMUW", 
                "LR.W", "LR.D", "SC.W", "SC.D", 
                "AMOSWAP.W", "AMOSWAP.D", "AMOADD.W", "AMOADD.D", "AMOXOR.W", 
                "AMOAND.W", "AMOOR.W", "AMOXOR.D", "AMOAND.D", "AMOOR.D", "AMOMIN.W", 
                "AMOMAX.W", "AMOMINU.W", "AMOMAXU.W", "AMOMIN.D", "AMOMAX.D", "AMOMINU.D", 
                "AMOMAXU.D", "FMV.W.X", "FMV.X.W", "FMV.D.X", "FMV.X.D", "FCVT.S.W", "FCVT.D.W", 
                "FCVT.S.WU", "FCVT.D.WU", "FCVT.W.S", "FCVT.W.D", "FCVT.WU.S", "FCVT.WU.D", 
                "FCVT.S.L", "FCVT.D.L", "FCVT.S.LU", "FCVT.D.LU", "FCVT.L.S", "FCVT.L.D", 
                "FCVT.LU.S", "FCVT.LU.D", "FADD.S", "FADD.D", "FSUB.S", "FSUB.D", "FMUL.S", 
                "FMUL.D", "FDIV.S", "FDIV.D", "FSQRT.S", "FSQRT.D", "FMADD.S", "FMADD.D", 
                "FMSUB.S", "FMSUB.D", "FNMADD.S", "FNMADD.D", "FNMSUB.S", "FNMSUB.D", "FSGNJ.S", 
                "FSGNJ.D", "FSGNJN.S", "FSGNJN.D", "FSGNJX.S", "FSGNJX.D", "FMIN.S", "FMIN.D", 
                "FMAX.S", "FMAX.D", "FEQ.S", "FEQ.D", "FLT.S", "FLT.D", "FLE.S", "FLE.D", 
                "FCLASS.S", "FCLASS.D", "SETVL", "VMULH", "VREM", "VSLL", "VSRL", "VSRA", "VLDS", 
                "VLDX", "VSTS", "VSTX", "AMOSWAP", "AMOADD", "AMOXOR", "AMOAND", "AMOOR", "AMOMIN", 
                "AMOMAX", "VPEQ", "VPNE", "VPLT", "VPGE", "VPAND", "VPANDN", "VPOR", "VPXOR", "VPNOT", 
                "VPSWAP", "VMOV", "VCVT", "VADD", "VSUB", "VMUL", "VDIV", "VSQRET", "VFMADD", "VFMSUB", 
                "VFNMADD", "VFNMSUB", "VSGNJ", "VSGNJN", "VSGNJX", "VMIN", "VMAX", "VXOR", "VOR", "VAND", 
                "VCLASS", "VSETDCFG", "VEXTRACT", "VMERGE", "VSELECT"
            },
            [Token.OP_I] = new string[] { 
                "SLLI", "SRLI", "SRAI", "SLLIW", "SRLIW", "SRAIW", "ADDI", "ADDIW", "XORI", "ORI", "ANDI", "SLTI", "SLTIU", "JALR", 
                "FENCE", "FENCE.I", "ECALL", "EBREAK", "CSRRW", "CSRRS", "CSRRC", "CSRRWI", "CSRRSI", "CSRRCI", "LB", "LH", "LBU", 
                "LHU", "LW", "LWU", "LD", "FLW", "FLD", "VLD"
            },
            [Token.OP_U] = new string[] { "LUI", "AUIPC" },
            [Token.OP_B] = new string[] { "BEQ", "BNE", "BLT", "BGE", "BLTU", "BGEU" },
            [Token.OP_J] = new string[] { "JAL" },
            [Token.OP_S] = new string[] { "SB", "SH", "SW", "SD", "FSW", "FSD", "VST" },
            [Token.OP_PSEUDO] = new string[] { 
                "NOP", "NEG", "NEGW", "SNEZ", "SLTZ", "SGTZ", "BEQZ", "BNEZ", "BLEZ", "BGEZ", "BLTZ", "BGTZ", "J", "JR", "RET", "TAIL", 
                "RDINSTRET", "RDINSTRETH", "RDCYCLE", "RDCYCLEH", "RDTIME", "RDTIMEH", "CSRR", "CSRW", "CSRS", "CSRC", "CSRWI", "CSRSI", 
                "CSRCI", "FRCSR", "FSCSR", "FRRM", "FSRM", "FRFLAGS", "FSFLAGS", "FSRMI", "FSFLAGSI", "LLA", "LA", "LB", "LH", "LW", "LD", "SB", "SH", "SW", "SD", 
                "FLW", "FLD", "FSW", "FSD", "LI", "MV", "NOT", "SEXT.W", "SEQZ", "FMV.S", "FABS.S", "FNEG.S", "FMV.D", "FABS.D", "FNEG.D", 
                "BGT", "BLE", "BGTU", "BLEU", "JAL", "JALR", "CALL", "FENCE"
            },
            [Token.OP_CR] = new string[] { "C.ADD", "C.SUB", "C.AND", "C.OR", "C.XOR", "C.MV", "C.JR", "C.JALR", "C.ADDW", "C.SUBW" },
            [Token.OP_CI] = new string[] { "C.LWSP", "C.FLWSP", "C.FLDSP", "C.ADDI", "C.ADDI16SP", "C.ANDI", "C.LI", "C.LUI", "C.SLLI", "C.SRAI", "C.SRLI", "C.EBRAKE", "C.ADDIW", "C.LDSP" },
            [Token.OP_CSS] = new string[] { "C.SWSP", "C.FSWSP", "C.FSDSP", "C.SDSP" },
            [Token.OP_CIW] = new string[] { "C.ADDI4SPN" },
            [Token.OP_CL] = new string[] { "C.LW", "C.FLW", "C.FLD", "C.LD" },
            [Token.OP_CS] = new string[] { "C.SW", "C.FSW", "C.FSD", "C.SD" },
            [Token.OP_CB] = new string[] { "C.BEQZ", "C.BNEZ" },
            [Token.OP_CJ] = new string[] { "C.J", "C.JAL" }
        };

        /// <summary>
        /// CSR (Control and Status Register) names that should be recognized as CSR tokens
        /// </summary>
        string[] CSR_NAMES = new string[] {
            "ustatus", "uie", "utvec", "uscratch", "uepc", "ucause", "utval", "uip",
            "fflags", "frm", "fcsr",
            "cycle", "time", "instret", "hpmcounter3", "hpmcounter4", "hpmcounter31",
            "cycleh", "timeh", "instreth", "hpmcounter3h", "hpmcounter4h", "hpmcounter31h",
            "sstatus", "sedeleg", "sideleg", "sie", "stvec", "scounteren", "sscratch",
            "sepc", "scause", "stval", "sip", "satp", "scontext",
            "hstatus", "hedeleg", "hideleg", "hie", "hcounteren", "htegie", "htval",
            "hip", "hvip", "htinst", "hgeip", "hgatp", "hcontext", "htimedelta", "htimedeltah",
            "vsstatus", "vsie", "vstvec", "vsscratch", "vsepc", "vscause", "vstval", "vsip", "bsatp",
            "mvendorid", "marchid", "mimpid", "mhartid", "mstatus", "misa", "medeleg", "mideleg",
            "mie", "mtvec", "mcounteren", "mstatush", "mscratch", "mepc", "hcause", "htval2",
            "pmpcfg0", "pmpcfg1", "pmpcfg2", "pmpcfg3", "pmpcfg14", "pmpcfg15",
            "pmpaddr0", "pmpaddr1", "pmpaddr63",
            "mcountinhibit", "mhpmevent3", "mhpmevent4", "mhpmevent31",
            "tselect", "tdata1", "tdata2", "tdata3", "mcontext",
            "dcsr", "dpc", "dscratch0", "dscratch1"
        };

        /// <summary>
        /// This holds the string that is currently being Tokenized
        /// </summary>
        string _string;

        /// <summary>
        /// This holds the current position in _string that we are at in tokenization
        /// </summary>
        private int __cursor;

        /// <summary>
        /// This holds the current position in _string that we are at in tokenization
        /// </summary>
        public int _cursor { 
            get { return __cursor; }
            set {
                if(value == __cursor) return;
                if(value == 0) {
                    _lineCursorCachePos = 0;
                    _colCursorCachePos = 0;
                    __cursor = 0;
                }
                // Loop through every char from where we are to where we will be.
                int start = __cursor;
                int end = value;
                bool reverse = false;
                if(start > end) {
                    reverse = true;
                    start = value;
                    end = __cursor;
                }

                if(reverse==false) {
                    // forward
                    for(int i = start; i < end; i++) {
                        // If new line reset x and advance y
                        if(_string[i] == '\n') {
                            _lineCursorCachePos++;
                            _colCursorCachePos = 0;
                            continue;
                        }
                        // else advance x
                        _colCursorCachePos++;
                    }
                } else {
                    // backwards
                    // Right now I don't count backwards because I am too tired to think about that so this code will probably survive way too long
                    // This code just recounts from the beginning
                    _lineCursorCachePos = 0;
                    _colCursorCachePos = 0;
                    for(int i = 0; i < value; i++) {
                        // If new line reset x and advance y
                        if(_string[i] == '\n') {
                            _lineCursorCachePos++;
                            _colCursorCachePos = 0;
                            continue;
                        }
                        // else advance x
                        _colCursorCachePos++;
                    }
                }
                __cursor = value;
            }
        }
        private int _lineCursorCachePos = 0;
        private int _colCursorCachePos = 0;

        /*
        /// <summary>
        /// An attempt to minimize the impact of a situation where there is a large source
        /// file and _line is called multiple times at the cost of 8 bytes of ram 
        /// because I am paranoid
        ///    - Ariel
        private int _lineCursorCache = 0;
        /// </summary>
        /// <summary>
        /// The cursor position at last line cache
        /// </summary>
        private int _lineCursorCachePos = 0;
        /// <summary>
        /// This computes the current line of the cursor. <br/>
        /// <b>Be careful with this call because it is computationally intensive.</b>
        /// </summary>
        public int _line { get; private set; }


        /// <summary>
        /// An attempt to minimize the impact of a situation where there is a large source
        /// file and _col is called multiple times at the cost of 8 bytes of ram 
        /// because I am paranoid
        ///    - Ariel
        /// </summary>
        private int _colCursorCache = 0;
        /// <summary>
        /// The cursor position at last col cache
        /// </summary>
        private int _colCursorCachePos = 0;
        /// <summary>
        /// This computes the current line of the cursor. <br/>
        /// <b>Be careful with this call because it is computationally intensive.</b>
        /// </summary>
        public int _col { 
            get { if(_cursor == _colCursorCachePos) return _colCursorCache; _colCursorCachePos = _cursor; _colCursorCache = _cursor; return ; } 
            set { } 
        }
         */


        /// <summary>
        /// Load a string into memory for tokenization
        /// </summary>
        public void Load(string str)
        {
            _string = str;
            _cursor = 0;
            _lineCursorCachePos = 0;
            _colCursorCachePos = 0;
    }

        /// <summary>
        /// Are there any tokens remaining to be read?
        /// </summary>
        /// <returns></returns>
        public bool hasMoreTokens { get { return _cursor < _string.Length; } }

        public bool isEOF { get { return _cursor >= _string.Length; } }

        private string getNextCharInStr(string str)
        {
            return str.Substring(0, 1);
        }

        public TokenData PeakToken(bool ignoreWitespace = true) {
            int x = _colCursorCachePos;
            int y = _lineCursorCachePos;
            int cursor = __cursor;

            TokenData tokenData = ReadToken(ignoreWitespace);

            _colCursorCachePos = x;
            _lineCursorCachePos = y;
            __cursor = cursor;

            return tokenData;
        }

        /// <summary>
        /// Obtain next token from current _cursor position
        /// </summary>
        /// <returns></returns>
        public TokenData ReadToken(bool ignoreWhitespace = false)//TODO: Add an ignoreWhitespace option
        {
            int x = _colCursorCachePos;
            int y = _lineCursorCachePos;

            // if no more tokens then return default
            if (isEOF) return new TokenData(Lexer.Token.EOF, null, y, x);

            string str = this._string.Substring(_cursor);
            string nextChar = getNextCharInStr(str);

            foreach (TokenFinder finder in Spec){
                // Try to get the value of the token
                string value = _match(finder, str);

                // Go to next token if this token is not found
                if (value == null) continue;
                string valueLow = value.ToLower();
                string valueUpper = value.ToUpper();

                if (finder.token == Token.IDENTIFIER)
                {
                    //TODO: check if in register array and thus actually a register and return register instead
                    foreach (Replacement replacement in REGISTER_REPLACEMENTS)
                    {
                        if (valueLow == replacement.test)
                        {
                            return new TokenData(Token.REGISTER, replacement.target, y, x);
                        }
                    }

                    // Check if this identifier is a CSR name
                    if (Array.IndexOf(CSR_NAMES, valueLow) >= 0) return new TokenData(Token.CSR, valueLow, y, x);

                    //TODO: check if in op arrays (One for each type) and thus actually an OP and return Correct OP type instead
                    // foreach(var tokenSet in OP_TYPES){

                    // }
                    if (Array.IndexOf(OP_TYPES[Token.OP_R], valueUpper) >= 0) return new TokenData(Token.OP_R, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_I], valueUpper) >= 0) return new TokenData(Token.OP_I, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_U], valueUpper) >= 0) return new TokenData(Token.OP_U, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_B], valueUpper) >= 0) return new TokenData(Token.OP_B, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_J], valueUpper) >= 0) return new TokenData(Token.OP_J, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_S], valueUpper) >= 0) return new TokenData(Token.OP_S, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CR], valueUpper) >= 0) return new TokenData(Token.OP_CR, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CI], valueUpper) >= 0) return new TokenData(Token.OP_CI, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CSS], valueUpper) >= 0) return new TokenData(Token.OP_CSS, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CIW], valueUpper) >= 0) return new TokenData(Token.OP_CIW, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CL], valueUpper) >= 0) return new TokenData(Token.OP_CL, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CS], valueUpper) >= 0) return new TokenData(Token.OP_CS, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CB], valueUpper) >= 0) return new TokenData(Token.OP_CB, valueUpper, y, x);
                    if (Array.IndexOf(OP_TYPES[Token.OP_CJ], valueUpper) >= 0) return new TokenData(Token.OP_CJ, valueUpper, y, x);

                    if (Array.IndexOf(OP_TYPES[Token.OP_PSEUDO], valueUpper) >= 0) return new TokenData(Token.OP_PSEUDO, valueUpper, y, x);
                }
                if(finder.token == Token.LABEL) return new TokenData(finder.token, value.Substring(0, value.Length - 1), y, x);
                if(ignoreWhitespace && finder.token == Token.WHITESPACE) return ReadToken(ignoreWhitespace);
                if(finder.token == Token.INLINE_DIRECTIVE) return new TokenData(finder.token, value.Substring(1), y, x);
                // We must have found a token if we made it to this line so return apropriate TokenData
                return new TokenData(finder.token, value, y, x);
            }

            throw new TokenSyntaxException("Unexpected char: `" + nextChar + "`");
        }

        private string _match(TokenFinder finder, string target)
        {
            // Try the regex on the target string
            Match matched = finder.test.Match(target);
            // Return null if no match found
            if (matched.Success == false) return null;
            // There now must be a match because we would have returned
            // Move the cursor to the end of the match
            _cursor += matched.Value.Length;
            // Return the actual Token value of the match
            return matched.Value;
        }
    }
}
