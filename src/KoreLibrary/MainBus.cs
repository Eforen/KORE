using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class MainBus
    {
        public enum OP : byte
        {
            nop = 0x0,
            /// <summary>
            /// Load from address
            /// </summary>
            ld = 0x01,
            /// <summary>
            /// Store at address
            /// </summary>
            st = 0x02,
            /// <summary>
            /// Load double word from address
            /// </summary>
            ld_8b = 0x03,
            /// <summary>
            /// Store double word at address
            /// </summary>
            st_8b = 0x04,
            /// <summary>
            /// Load word from address
            /// </summary>
            ld_4b = 0x05,
            /// <summary>
            /// Store word at address
            /// </summary>
            st_4b = 0x06,
            /// <summary>
            /// Load nibble from address (half word)
            /// </summary>
            ld_2b = 0x07,
            /// <summary>
            /// Store nibble at address (half word)
            /// </summary>
            st_2b = 0x08,
            /// <summary>
            /// Load byte from address
            /// </summary>
            ld_1b = 0x09,
            /// <summary>
            /// Store byte at address
            /// </summary>
            st_1b = 0x10
        }

        public OP op { get; set; }
        public ulong address { get; set; }
        public ulong data { get; set; }

        // This may not be the most desirable way to do this. It may be better to just do a single tick.
        public void tick()
        {
            //
            foreach (MainBusComponent component in components)
            {
                component.clockRise();
            }
            foreach (MainBusComponent component in components)
            {
                component.clockFall();
            }
            // Work Backwards
            //for (int i = components.Count-1; i >= 0; i--)
            //{
            //components[i].clockFall();
            //}
        }

        public List<MainBusComponent> components { get; protected set; } = new List<MainBusComponent>();

    }
}
