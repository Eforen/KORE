using Kore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    public class MainBus
    {
        public class StubMainBusComponent : MainBusComponent
        {
            public StubMainBusComponent(Kore.MainBus bus) : base(bus)
            {
            }

            public override void clockFall(){}

            public override void clockRise(){}
        }

        private Kore.MainBus bus;
        [SetUp]
        public void Setup()
        {
            bus = new Kore.MainBus();
        }

        [Test]
        public void Instantiate()
        {
            Kore.MainBus busTest = new Kore.MainBus();
            Assert.AreEqual(typeof(Kore.MainBus), busTest.GetType());
        }

        [Test]
        public void Tick()
        {
            MainBusComponent mock0 = NSubstitute.Substitute.ForPartsOf<StubMainBusComponent>(bus);
            MainBusComponent mock1 = NSubstitute.Substitute.ForPartsOf<StubMainBusComponent>(bus);
            MainBusComponent mock2 = NSubstitute.Substitute.ForPartsOf<StubMainBusComponent>(bus);

            Assert.AreEqual(3, bus.components.Count);

            bus.tick();

            Received.InOrder(() =>
            {
                mock0.clockRise();
                mock1.clockRise();
                mock2.clockRise();
                mock0.clockFall();
                mock1.clockFall();
                mock2.clockFall();
            });        }
    }
}
