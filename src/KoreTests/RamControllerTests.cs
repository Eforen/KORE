using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    class RamControllerTests
    {
        private Kore.MainBus bus;
        private Kore.CPU cpu;
        private Kore.RamController ram;
        [SetUp]
        public void Setup()
        {
            bus = new Kore.MainBus();
            cpu = new Kore.CPU(bus);
            ram = new Kore.RamController(bus, cpu.MemorySize);
        }

        [Test]
        public void Instantiate()
        {
            Assert.AreEqual(typeof(Kore.RamController), ram.GetType());
        }

        [Test, Ignore("Not Coded")]
        public void StackPointerAtInstantiate()
        {
            //If Correct command is on the bus at clockRise then place the answer on the bus
            //Or store the answer in the ram from the bus on rise if that is the command
        }
    }
}
