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
        ulong currentExeResult = 0;

        ulong currentBranch = 0;
        bool currentBranchPending = false;

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
                        case InstructionType.OP:
                            currentInstructionType = INST_TYPE.RType;
                            break;
                        case InstructionType.OP_IMM:
                            currentInstructionType = INST_TYPE.IType;
                            break;
                        case InstructionType.LOAD:
                        case InstructionType.LOAD_FP:
                        case InstructionType.CUSTOM_0:
                        case InstructionType.MISC_MEM:
                        case InstructionType.AUIPC:
                            break;
                        case InstructionType.OP_IMM_32:
                            currentInstructionType = INST_TYPE.IType;
                            break;
                        case InstructionType.STORE:
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
                        case OPCODE.B32_ADDI: //ADDI is not the correct term for this but I have not gotten to the correct one yet
                            switch (currentIType.func3)
                            {
                                case 0b000: //ADDI
                                    currentExeResult = (ulong)((long)currentReadRS1 + currentIType.imm);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case OPCODE.B32_ADD: //ADD is not the correct term for this but I have not gotten to the correct one yet
                            switch (currentIType.func3)
                            {
                                case 0b000: //ADD
                                    currentExeResult = currentReadRS1 + currentReadRS2;
                                    break;
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
                        default:
                            break;
                    }
                    break;
                case Cycle.Write:
                    switch (currentInstructionType)
                    {
                        case INST_TYPE.RType:
                            registers.setR(currentRType.rd, currentExeResult);
                            break;
                        case INST_TYPE.IType:
                            registers.setR(currentIType.rd, currentExeResult);
                            break;
                        case INST_TYPE.SType:
                            break;
                        case INST_TYPE.BType:
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
