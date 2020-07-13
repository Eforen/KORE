using Kore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoreTests
{
    public class UtilitiesTests
    {
        [Test]
        public void toDWord()
        {
            ulong a = 0x98416847;
            ulong b = 0x63541831;
            ulong c = 0x37814815;

            Assert.AreEqual(a, Kore.Utility.Misc.toDWord(0x47, 0x68, 0x41, 0x98));
            Assert.AreEqual(b, Kore.Utility.Misc.toDWord(0x31, 0x18, 0x54, 0x63));
            Assert.AreEqual(c, Kore.Utility.Misc.toDWord(0x15, 0x48, 0x81, 0x37));
        }

        [Test]
        public void toDWords()
        {
            ulong[] a = {0x98416847, 0x63541831, 0x37814815};

            Assert.AreEqual(a, Kore.Utility.Misc.toDWords(new byte[] { 0x47, 0x68, 0x41, 0x98, 0x0, 0x0, 0x0, 0x0, 0x31, 0x18, 0x54, 0x63, 0x0, 0x0, 0x0, 0x0, 0x15, 0x48, 0x81, 0x37 }));
        }
    }
}
