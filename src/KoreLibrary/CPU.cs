using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.registers.setR(RegisterFile.Register.sp, MemorySize - 1);

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

        public Cycle state { get; protected set; }

        public override void clockFall() {
            switch (state)
            {
                case Cycle.Off:
                    break;
                case Cycle.Init:
                    break;
                case Cycle.Fetch:
                    break;
                case Cycle.Decode:
                    break;
                case Cycle.Read:
                    break;
                case Cycle.Exec:
                    break;
                case Cycle.Write:
                    break;
                case Cycle.MovPC:
                    break;
                case Cycle.Halt:
                    break;
                default:
                    break;
            }
        }

        public override void clockRise()
        {
            switch (state)
            {
                case Cycle.Off:
                    break;
                case Cycle.Init:
                    state = Cycle.Fetch;
                    break; 
                case Cycle.Fetch:
                    state = Cycle.Decode;
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
                    registers.setR(RegisterFile.Register.PC, registers.getR(RegisterFile.Register.PC)+0x04u);

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
