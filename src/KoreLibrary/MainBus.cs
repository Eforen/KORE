using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    public class MainBus
    {
        public byte op { get; set; }
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
