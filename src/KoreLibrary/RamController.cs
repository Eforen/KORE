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
        public RamController(MainBus bus, ulong MemorySize = (1024 * 1024 * 128), ulong firstAddress = 0) : base(bus)
        {
            this.MemorySize = MemorySize;
            mem = new byte[MemorySize];
            this.firstAddress = firstAddress;
        }

        public ulong MemorySize { get; private set; }
        public ulong firstAddress { get; private set; }
        protected byte[] mem;

        //TODO: make test for behavior of this
        public override void clockFall() { }


        //TODO: Make test for behavior of this
        public override void clockRise() {
            if(bus.address >= firstAddress || bus.address < firstAddress + this.MemorySize)
            {
                // Safely Get OP
                MainBus.OP op = MainBus.OP.nop;
                try
                {
                    op = bus.op;
                }
                catch (Exception)
                {
                    throw new KoreRAMFaultUnknownOP((byte) bus.op);
                }

                byte[] data = null;
                // Do OP
                switch (op)
                {
                    case MainBus.OP.ld_8b:
                        // NOTE: The + 0x00 here should be removed by the compiler and thus not cost us anything but make it more readable
                        bus.data = Utility.Misc.toDWord(
                            mem[bus.address + 0x00],
                            mem[bus.address + 0x01],
                            mem[bus.address + 0x02],
                            mem[bus.address + 0x03],
                            mem[bus.address + 0x04],
                            mem[bus.address + 0x05],
                            mem[bus.address + 0x06],
                            mem[bus.address + 0x07]
                        );
                        break;
                    case MainBus.OP.st_8b:
                        data = Utility.Misc.fromDWord(bus.data);
                        mem[bus.address + 0x00] = data[0x00];
                        mem[bus.address + 0x01] = data[0x01];
                        mem[bus.address + 0x02] = data[0x02];
                        mem[bus.address + 0x03] = data[0x03];
                        mem[bus.address + 0x04] = data[0x04];
                        mem[bus.address + 0x05] = data[0x05];
                        mem[bus.address + 0x06] = data[0x06];
                        mem[bus.address + 0x07] = data[0x07];
                        break;
                    case MainBus.OP.ld_4b:
                        // NOTE: The + 0x00 here should be removed by the compiler and thus not cost us anything but make it more readable
                        bus.data = Utility.Misc.toDWord(
                            mem[bus.address + 0x00],
                            mem[bus.address + 0x01],
                            mem[bus.address + 0x02],
                            mem[bus.address + 0x03]
                        );
                        break;
                    case MainBus.OP.st_4b:
                        data = Utility.Misc.fromDWord(bus.data, bytes: 4);
                        mem[bus.address + 0x00] = data[0x00];
                        mem[bus.address + 0x01] = data[0x01];
                        mem[bus.address + 0x02] = data[0x02];
                        mem[bus.address + 0x03] = data[0x03];
                        break;
                    case MainBus.OP.ld_2b:
                        // NOTE: The + 0x00 here should be removed by the compiler and thus not cost us anything but make it more readable
                        bus.data = Utility.Misc.toDWord(
                            mem[bus.address + 0x00],
                            mem[bus.address + 0x01]
                        );
                        break;
                    case MainBus.OP.st_2b:
                        data = Utility.Misc.fromDWord(bus.data, bytes: 2);
                        mem[bus.address + 0x00] = data[0x00];
                        mem[bus.address + 0x01] = data[0x01];
                        break;
                    case MainBus.OP.ld:
                    case MainBus.OP.ld_1b:
                        bus.data = Utility.Misc.toDWord(
                            mem[bus.address]
                        );
                        break;
                    case MainBus.OP.st:
                    case MainBus.OP.st_1b:
                        data = Utility.Misc.fromDWord(bus.data, bytes: 2);
                        mem[bus.address] = data[0x00];
                        break;
                    case MainBus.OP.nop:
                        // Don't do anything
                        break;
                    default:
                        throw new KoreRAMFaultUnknownOP((byte)bus.op);
                }
            }
        }

        /// <summary>
        /// TODO: This should be moved to a fault signal in the CPU with a global Enum for fault codes so it can be standardized and set in stone.
        /// </summary>
        public class KoreRAMFaultUnknownOP : Exception
        {
            public KoreRAMFaultUnknownOP(byte op)
            {
                this.requestedOP = op;
            }
            byte requestedOP;
        }

        /// <summary>
        /// DEFUNCT: This should not actually be possible but I coded it so I am leaving it here for now.
        /// Create RAM Fault indicating an attempt to access a bad address.
        /// TODO: This should be moved to a fault signal in the CPU with a global Enum for fault codes so it can be standardized and set in stone.
        /// </summary>
        public class KoreRAMFaultBadAddress : Exception
        {
            /// <summary>
            /// Create RAM Fault indicating an attempt to access a bad address
            /// </summary>
            /// <param name="addr">The address requested</param>
            /// <param name="addrSpaceStart">The first RAM Addresses</param>
            /// <param name="addrSpaceEnd">The Last RAM Address</param>
            public KoreRAMFaultBadAddress(ulong addr, ulong addrSpaceStart, ulong addrSpaceEnd)
            {
                this.address = addr;
                this.addrSpaceStart = addrSpaceStart;
                this.addrSpaceEnd = addrSpaceEnd;
            }

            /// <summary>
            /// The address requested
            /// </summary>
            ulong address;
            /// <summary>
            /// The first RAM Addresses
            /// </summary>
            ulong addrSpaceStart;
            /// <summary>
            /// The Last RAM Address
            /// </summary>
            ulong addrSpaceEnd;
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
