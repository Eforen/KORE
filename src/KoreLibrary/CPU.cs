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
            this.MemorySize = MemorySize;
            this.registers.setR(RegisterFile.Register.sp, MemorySize - 1);
        }

        public ulong MemorySize { get; private set; }
        public RegisterFile registers { get; protected set; } = new RegisterFile();

        public override void clockFall() {}

        public override void clockRise() {}
    }
}
