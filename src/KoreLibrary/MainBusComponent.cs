namespace Kore
{
    public abstract class MainBusComponent
    {
        protected MainBus bus;

        public MainBusComponent(MainBus bus)
        {
            this.bus = bus;
            this.bus.components.Add(this);
        }

        public abstract void clockRise();
        public abstract void clockFall();

    }
}