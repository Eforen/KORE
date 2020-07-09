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
            mem = new byte[MemorySize];
        }

        public ulong MemorySize { get; private set; }
        protected byte[] mem;

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

        public void store(byte[] memsrc, ulong index, ulong length, ulong dest)
        {
            for (ulong i = 0; i < length; i++)
            {
                mem[dest + i] = memsrc[index + i];
            }
        }

        public byte getByte(ulong address)
        {
            if (address >= MemorySize)
                throw new IndexOutOfRangeException("The RAM Controller called does not have enough ram to serve the address 0x" + Convert.ToString((long)address, 16) + " it only has " + MemorySize + " bytes.");
            return mem[address];
        }

        public void setByte(ulong address, byte data)
        {
            if (address >= MemorySize)
                throw new IndexOutOfRangeException("The RAM Controller called does not have enough ram to store at address 0x" + Convert.ToString((long)address, 16)+" it only has "+MemorySize+" bytes.");
            mem[address] = data;
        }
    }
}
