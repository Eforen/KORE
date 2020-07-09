using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class RamController : MainBusComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemorySize">Mem in Bytes [Defaults to 1024 * 1024 * 128 (128MB)]</param>
        public RamController(MainBus bus, ulong MemorySize = (1024 * 1024 * 128)) : base(bus)
        {
            this.MemorySize = MemorySize;
        }

        public ulong MemorySize { get; private set; }

        public override void clockFall() { }

        public override void clockRise() { }

        public enum OP: byte
        {
            /// <summary>
            /// Load from address
            /// </summary>
            ld,
            /// <summary>
            /// Store at address
            /// </summary>
            st
        }

    }
}
