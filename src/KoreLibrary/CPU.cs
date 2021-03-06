using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kore.RiscISA;
using Kore.RiscISA.Instruction;

namespace Kore
{
    public class CPU: MainBusComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemorySize">Mem in Bytes [Defaults to 1024 * 1024 * 128 (128MB)]</param>
        public CPU(MainBus bus, ulong MemorySize = (1024 * 1024 * 128)) : base(bus)
        {
            this.alu = new ALU64();

            this.MemorySize = MemorySize;
            this.registers.setR(Register.sp, MemorySize - 1);

            this.state = Cycle.Off;
        }

        public ulong MemorySize { get; private set; }
        public RegisterFile registers { get; protected set; } = new RegisterFile();
        public ALU64 alu { get; set; }

        // Step 1: Fetch Instruction
        // Step 2: Decode Instruction
        // Step 3: Read Source Operands [This step may include interaction with the Main Bus]
        // Step 3.up: put command and address on the bus
        // Step 3.down: pull the data off the bus and clear it
        // Step 4: Execute
        // Step 5: Write Destination Operand
        // Step 6: Move PC to next position +4 or jump location

        public enum Cycle: byte
        {
            Off     = 0x00,
            Init    = 0x10,
            Fetch   = 0x30,
            Decode  = 0x40,
            Read    = 0x50,
            Exec    = 0x60,
            Write   = 0x70,
            MovPC   = 0x80,
            Halt    = 0xFF
        }

        public ulong currentInstruction, currentPC;

        /// <summary>
        /// Temp Instruction Register
        /// </summary>
        public RType currentRType { get; protected set; } = new RType();

        /// <summary>
        /// Temp Instruction Register
        /// </summary>
        public IType currentIType { get; protected set; } = new IType();

        /// <summary>
        /// Temp Instruction Register
        /// </summary>
        public SType currentSType { get; protected set; } = new SType();

        /// <summary>
        /// Temp Instruction Register
        /// </summary>
        public BType currentBType { get; protected set; } = new BType();

        /// <summary>
        /// Temp Instruction Register
        /// </summary>
        public UType currentUType { get; protected set; } = new UType();

        /// <summary>
        /// Temp Instruction Register
        /// </summary>
        public JType currentJType { get; protected set; } = new JType();

        public INST_TYPE currentInstructionType = INST_TYPE.Unkwn;

        byte currenOPCODE = 0;

        ulong currentReadRS1 = 0;
        ulong currentReadRS2 = 0;

        ulong currentBranch = 0;
        bool currentBranchPending = false;

        public enum PIPELINE_WRITE_MODE
        {
            NOP,
            WRITE_REGISTER,
            WRITE_MEMORY,
            LOAD_TO_REGISTER,
        }
        public PIPELINE_WRITE_MODE pipeWriteMode { get; private set; }
        public ulong pipeWriteData = 0;
        public ulong pipeWriteAddress = 0;
        public byte pipeWriteByteCount = 0;

        public Cycle state { get; protected set; }


        public override void clockRise()
        {
            switch (state)
            {
                case Cycle.Off:
                    break;
                case Cycle.Init:
                    break;
                case Cycle.Fetch:
                    bus.op = MainBus.OP.ld_4b;
                    currentPC = registers.getR(Register.PC);
                    bus.address = currentPC;
                    bus.data = 0;
                    break;
                case Cycle.Decode:
                    currenOPCODE = (byte)(0b0111_1111 & (byte)currentInstruction);
                    InstructionType op = (InstructionType)(0b0111_1100 & currenOPCODE);
                    switch (op)
                    {
                        case InstructionType.LOAD:
                            currentInstructionType = INST_TYPE.IType;
                            break;
                        case InstructionType.OP:
                            currentInstructionType = INST_TYPE.RType;
                            break;
                        case InstructionType.OP_IMM:
                            currentInstructionType = INST_TYPE.IType;
                            break;
                        case InstructionType.LOAD_FP:
                        case InstructionType.CUSTOM_0:
                        case InstructionType.MISC_MEM:
                        case InstructionType.AUIPC:
                            break;
                        case InstructionType.OP_IMM_32:
                            currentInstructionType = INST_TYPE.IType;
                            break;
                        case InstructionType.STORE:
                            currentInstructionType = INST_TYPE.SType;
                            break;
                        case InstructionType.STORE_FP:
                        case InstructionType.CUSTOM_1:
                        case InstructionType.AMO:
                        case InstructionType.LUI:
                        case InstructionType.OP_32:
                        case InstructionType.MADD:
                        case InstructionType.MSUB:
                        case InstructionType.NMSUB:
                        case InstructionType.NMADD:
                        case InstructionType.OP_FP:
                        case InstructionType.RESERVED_0:
                        case InstructionType.CUSTOM_2:
                            break;
                        case InstructionType.BRANCH:
                            currentInstructionType = INST_TYPE.BType;
                            break;
                        case InstructionType.JALR:
                        case InstructionType.RESERVED_1:
                        case InstructionType.JAL:
                        case InstructionType.SYSTEM:
                        case InstructionType.RESERVED_2:
                        case InstructionType.CUSTOM_3:
                        default:
                            break;
                    }
                    switch (currentInstructionType)
                    {
                        case INST_TYPE.RType:
                            currentRType.Decode(currentInstruction);
                            break;
                        case INST_TYPE.IType:
                            currentIType.Decode(currentInstruction);
                            break;
                        case INST_TYPE.SType:
                            currentSType.Decode(currentInstruction);
                            break;
                        case INST_TYPE.BType:
                            currentBType.Decode(currentInstruction);
                            break;
                        case INST_TYPE.UType:
                            currentUType.Decode(currentInstruction);
                            break;
                        case INST_TYPE.JType:
                            currentJType.Decode(currentInstruction);
                            break;
                        case INST_TYPE.Unkwn:
                        default:
                            throw new NotImplementedException("Instruction Type Not Set Correctly");
                    }
                    break;
                case Cycle.Read:
                    switch (currentInstructionType)
                    {
                        case INST_TYPE.RType:
                            currentReadRS1 = registers.getR(currentRType.rs1);
                            currentReadRS2 = registers.getR(currentRType.rs2);
                            break;
                        case INST_TYPE.IType:
                            currentReadRS1 = registers.getR(currentIType.rs1);
                            break;
                        case INST_TYPE.SType:
                            currentReadRS1 = registers.getR(currentSType.rs1);
                            currentReadRS2 = registers.getR(currentSType.rs2); 
                            break;
                        case INST_TYPE.BType:
                            currentReadRS1 = registers.getR(currentBType.rs1);
                            currentReadRS2 = registers.getR(currentBType.rs2);
                            break;
                        case INST_TYPE.UType:
                            break;
                        case INST_TYPE.JType:
                            break;
                        case INST_TYPE.Unkwn:
                        default:
                            throw new NotImplementedException("Instruction Type Not Set Correctly");
                    }
                    break;
                case Cycle.Exec:
                    switch ((OPCODE) currenOPCODE)
                    {
                        case OPCODE.B32_LOAD_IMM:
                            bus.address = (ulong)((long)currentReadRS1 + currentIType.imm); //Set target Address
                            bus.data = 0; //Clear the bus

                            pipeWriteMode = PIPELINE_WRITE_MODE.LOAD_TO_REGISTER; // Tell write cycle what we want to do.
                            pipeWriteAddress = (byte)currentIType.rd; // Tell write cycle where we want to do it.
                            switch ((FUNC3_MEMORY)currentIType.func3)
                            {
                                case FUNC3_MEMORY.UNSIGNED_BYTE:
                                case FUNC3_MEMORY.BYTE:
                                    pipeWriteByteCount = 1;
                                    bus.op = MainBus.OP.ld_1b;
                                    break;
                                case FUNC3_MEMORY.UNSIGNED_HALFWORD:
                                case FUNC3_MEMORY.HALFWORD:
                                    pipeWriteByteCount = 2;
                                    bus.op = MainBus.OP.ld_2b;
                                    break;
                                case FUNC3_MEMORY.UNSIGNED_WORD:
                                case FUNC3_MEMORY.WORD:
                                    pipeWriteByteCount = 4;
                                    bus.op = MainBus.OP.ld_4b;
                                    break;
                                case FUNC3_MEMORY.UNSIGNED_DOUBLEWORD:
                                case FUNC3_MEMORY.DOUBLEWORD:
                                    pipeWriteByteCount = 8;
                                    bus.op = MainBus.OP.ld_8b;
                                    break;
                                default:
                                    throw new Exception("KORE HARDWARE PANIC", new Exception("Unsigned FUNC3 Store OPs are not supported."));
                            }
                            if(currentIType.func3 == (byte)FUNC3_MEMORY.UNSIGNED_BYTE || currentIType.func3 == (byte)FUNC3_MEMORY.UNSIGNED_HALFWORD || currentIType.func3 == (byte)FUNC3_MEMORY.UNSIGNED_WORD || currentIType.func3 == (byte)FUNC3_MEMORY.UNSIGNED_DOUBLEWORD)
                            {
                                pipeWriteData = 0; // Tell Pipe to NOT sign extend
                            }
                            else
                            {
                                pipeWriteData = 1; // Tell Pipe to sign extend
                            }
                            break;
                        case OPCODE.B32_ADDI: //ADDI is not the correct term for this but I have not gotten to the correct one yet
                            switch (currentIType.func3)
                            {
                                case 0b000: //ADDI
                                    pipeWriteMode = PIPELINE_WRITE_MODE.WRITE_REGISTER;
                                    pipeWriteAddress = (ulong)currentIType.rd;
                                    pipeWriteData = (ulong)((long)currentReadRS1 + currentIType.imm);
                                    pipeWriteByteCount = 8;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case OPCODE.B32_OP: //ADD is not the correct term for this but I have not gotten to the correct one yet
                            switch ((FUNC3_ALU) currentIType.func3)
                            {
                                case FUNC3_ALU.ADD: //ADD
                                    pipeWriteMode = PIPELINE_WRITE_MODE.WRITE_REGISTER;
                                    pipeWriteAddress = (ulong)currentRType.rd;
                                    if(currentRType.func7 == 0b0100000) pipeWriteData = currentReadRS1 - currentReadRS2;
                                    else pipeWriteData = currentReadRS1 + currentReadRS2;
                                    pipeWriteByteCount = 8;
                                    break;
                                case FUNC3_ALU.SLL:
                                case FUNC3_ALU.SLT:
                                case FUNC3_ALU.SLTU:
                                case FUNC3_ALU.XOR:
                                case FUNC3_ALU.SR:
                                case FUNC3_ALU.OR:
                                case FUNC3_ALU.AND:
                                default:
                                    break;
                            }
                            break;
                        case OPCODE.B32_BRANCH:
                            currentBranch = (ulong)((long)currentPC + (long)currentBType.imm);
                            switch (currentBType.func3)
                            {
                                case 0b000: //BEQ
                                    currentBranchPending = currentReadRS1 == currentReadRS2;
                                    break;
                                case 0b001: //BNE
                                    currentBranchPending = currentReadRS1 != currentReadRS2;
                                    break;
                                case 0b010: //?
                                case 0b011: //?
                                    break;
                                case 0b100: //BLT (signed)
                                    currentBranchPending = (long) currentReadRS1 < (long)currentReadRS2;
                                    break;
                                case 0b101: //BGE (signed)
                                    currentBranchPending = (long)currentReadRS1 >= (long)currentReadRS2;
                                    break;
                                case 0b110: //BLTU (unsigned)
                                    currentBranchPending = currentReadRS1 < currentReadRS2;
                                    break;
                                case 0b111: //BGEU (unsigned)
                                    currentBranchPending = currentReadRS1 >= currentReadRS2;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case OPCODE.B32_STORE_S:
                            pipeWriteMode = PIPELINE_WRITE_MODE.WRITE_MEMORY;
                            pipeWriteAddress = (ulong)((long) currentReadRS1 + currentSType.imm);
                            pipeWriteData = currentReadRS2;
                            switch ((FUNC3_MEMORY)currentSType.func3)
                            {
                                case FUNC3_MEMORY.BYTE:
                                    pipeWriteByteCount = 1;
                                    break;
                                case FUNC3_MEMORY.HALFWORD:
                                    pipeWriteByteCount = 2;
                                    break;
                                case FUNC3_MEMORY.WORD:
                                    pipeWriteByteCount = 4;
                                    break;
                                case FUNC3_MEMORY.DOUBLEWORD:
                                    pipeWriteByteCount = 8;
                                    break;
                                default:
                                    throw new Exception("KORE HARDWARE PANIC", new Exception("Unsigned FUNC3 Store OPs are not supported."));
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case Cycle.Write:
                    switch (pipeWriteMode)
                    {
                        case PIPELINE_WRITE_MODE.NOP:
                            break;
                        case PIPELINE_WRITE_MODE.WRITE_REGISTER:
                            registers.setR((Register)pipeWriteAddress, pipeWriteData);
                            break;
                        case PIPELINE_WRITE_MODE.WRITE_MEMORY:
                            switch (pipeWriteByteCount)
                            {
                                case 1:
                                    bus.op = MainBus.OP.st_1b;
                                    break;
                                case 2:
                                    bus.op = MainBus.OP.st_2b;
                                    break;
                                case 4:
                                    bus.op = MainBus.OP.st_4b;
                                    break;
                                case 8:
                                    bus.op = MainBus.OP.st_8b;
                                    break;
                                default:
                                    break;
                            }
                            bus.address = pipeWriteAddress;
                            bus.data = pipeWriteData;
                            break;
                        case PIPELINE_WRITE_MODE.LOAD_TO_REGISTER:

                            if(pipeWriteData == 1) //Should Sign Extend
                            {
                                switch (pipeWriteByteCount)
                                {
                                    case 1:
                                        registers.setR((Register)pipeWriteAddress, (ulong)(((short)(ushort)bus.data << 8) >> 8));
                                        break;
                                    case 2:
                                        registers.setR((Register)pipeWriteAddress, (ulong)(long)(short)(ushort)bus.data);
                                        break;
                                    case 4:
                                        registers.setR((Register)pipeWriteAddress, (ulong)(long)(int)(uint)bus.data);
                                        break;
                                    case 8:
                                    default:
                                        registers.setR((Register)pipeWriteAddress, bus.data);
                                        break;
                                }
                            } else
                            {
                                registers.setR((Register)pipeWriteAddress, bus.data);
                            }
                            break;
                        default:
                            break;
                    }
                    pipeWriteMode = PIPELINE_WRITE_MODE.NOP;
                    pipeWriteData = 0;
                    pipeWriteAddress = 0;
                    pipeWriteByteCount = 0;
                    break;
                case Cycle.MovPC:
                    if (currentBranchPending) registers.setR(Register.PC, currentBranch);
                    else registers.setR(Register.PC, currentPC + 0x04u);
                    break;
                case Cycle.Halt:
                    break;
                default:
                    break;
            }
        }

        public override void clockFall() {
            switch (state)
            {
                case Cycle.Off:
                    break;
                case Cycle.Init:
                    state = Cycle.Fetch;
                    break;
                case Cycle.Fetch:
                    state = Cycle.Decode;
                    currentInstruction = bus.data;
                    break;
                case Cycle.Decode:
                    state = Cycle.Read;
                    break;
                case Cycle.Read:
                    state = Cycle.Exec;
                    break;
                case Cycle.Exec:
                    state = Cycle.Write;
                    break;
                case Cycle.Write:
                    state = Cycle.MovPC;
                    break;
                case Cycle.MovPC:
                    state = Cycle.Fetch;
                    break;
                case Cycle.Halt:
                    break;
                default:
                    break;
            }
        }

        public void turnOn()
        {
            if (state == Cycle.Off) state = Cycle.Init;
        }
        public void turnOff()
        {
            state = Cycle.Off;
        }

        public void reset()
        {
            state = Cycle.Init;
        }
    }
}
