using NUnit.Framework;

namespace KoreTests
{
    public class CPUTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Instantiation()
        {
            Kore.CPU cpu = new Kore.CPU();
            Assert.IsInstanceOf<Kore.CPU>(cpu);
        }
    }
}