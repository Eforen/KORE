using Kore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            ulong[] a = { 0x98416847, 0x63541831, 0x37814815 };

            Assert.AreEqual(a, Kore.Utility.Misc.toDWords(new byte[] { 0x47, 0x68, 0x41, 0x98, 0x0, 0x0, 0x0, 0x0, 0x31, 0x18, 0x54, 0x63, 0x0, 0x0, 0x0, 0x0, 0x15, 0x48, 0x81, 0x37 }));
        }
        [Test]
        public void fromDWord()
        {
            ulong a = 0x98416847;
            ulong b = 0x63541831;
            ulong c = 0x37814815;
            ulong d = 0x9841684763541831;
            ulong e = 0x6354183137814815;
            byte[] ta = { 0x47, 0x68, 0x41, 0x98, 0x00, 0x00, 0x00, 0x00 };
            byte[] tb = { 0x31, 0x18, 0x54, 0x63, 0x00, 0x00, 0x00, 0x00 };
            byte[] tc = { 0x15, 0x48, 0x81, 0x37, 0x00, 0x00, 0x00, 0x00 };
            byte[] td = { 0x31, 0x18, 0x54, 0x63, 0x47, 0x68, 0x41, 0x98 };
            byte[] te = { 0x15, 0x48, 0x81, 0x37, 0x31, 0x18, 0x54, 0x63 };

            Assert.AreEqual(ta, Kore.Utility.Misc.fromDWord(a));
            Assert.AreEqual(tb, Kore.Utility.Misc.fromDWord(b));
            Assert.AreEqual(tc, Kore.Utility.Misc.fromDWord(c));
            Assert.AreEqual(td, Kore.Utility.Misc.fromDWord(d));
            Assert.AreEqual(te, Kore.Utility.Misc.fromDWord(e));
        }

        [Test]
        public void fromDWordSpesificBytes()
        {
            ulong a = 0x98416847;
            ulong b = 0x63541831;
            ulong c = 0x37814815;
            ulong d = 0x9841684763541831;
            ulong e = 0x6354183137814815;
            // ulong[] ta = new byte[] { 0x47, 0x68, 0x41, 0x98 };
            // ulong[] tb = new byte[] { 0x31, 0x18, 0x54, 0x63 };
            // ulong[] tc = new byte[] { 0x15, 0x48, 0x81, 0x37 };
            // ulong[] td = new byte[] { 0x31, 0x18, 0x54, 0x63, 0x47, 0x68, 0x41, 0x98 };
            // ulong[] te = new byte[] { 0x15, 0x48, 0x81, 0x37, 0x31, 0x18, 0x54, 0x63 };

            Assert.AreEqual(new byte[] { 0x47, 0x68, 0x41, 0x98, 0x00, 0x00, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(a, bytes: 8));
            Assert.AreEqual(new byte[] { 0x47, 0x68, 0x41, 0x98, 0x00, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(a, bytes: 7));
            Assert.AreEqual(new byte[] { 0x47, 0x68, 0x41, 0x98, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(a, bytes: 6));
            Assert.AreEqual(new byte[] { 0x47, 0x68, 0x41, 0x98, 0x00 }, Kore.Utility.Misc.fromDWord(a, bytes: 5));
            Assert.AreEqual(new byte[] { 0x47, 0x68, 0x41, 0x98 }, Kore.Utility.Misc.fromDWord(a, bytes: 4));
            Assert.AreEqual(new byte[] { 0x47, 0x68, 0x41 }, Kore.Utility.Misc.fromDWord(a, bytes: 3));
            Assert.AreEqual(new byte[] { 0x47, 0x68 }, Kore.Utility.Misc.fromDWord(a, bytes: 2));
            Assert.AreEqual(new byte[] { 0x47 }, Kore.Utility.Misc.fromDWord(a, bytes: 1));

            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x00, 0x00, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(b, bytes: 8));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x00, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(b, bytes: 7));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(b, bytes: 6));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x00 }, Kore.Utility.Misc.fromDWord(b, bytes: 5));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63 }, Kore.Utility.Misc.fromDWord(b, bytes: 4));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54 }, Kore.Utility.Misc.fromDWord(b, bytes: 3));
            Assert.AreEqual(new byte[] { 0x31, 0x18 }, Kore.Utility.Misc.fromDWord(b, bytes: 2));
            Assert.AreEqual(new byte[] { 0x31 }, Kore.Utility.Misc.fromDWord(b, bytes: 1));

            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x00, 0x00, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(c, bytes: 8));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x00, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(c, bytes: 7));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x00, 0x00 }, Kore.Utility.Misc.fromDWord(c, bytes: 6));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x00 }, Kore.Utility.Misc.fromDWord(c, bytes: 5));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37 }, Kore.Utility.Misc.fromDWord(c, bytes: 4));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81 }, Kore.Utility.Misc.fromDWord(c, bytes: 3));
            Assert.AreEqual(new byte[] { 0x15, 0x48 }, Kore.Utility.Misc.fromDWord(c, bytes: 2));
            Assert.AreEqual(new byte[] { 0x15 }, Kore.Utility.Misc.fromDWord(c, bytes: 1));

            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x47, 0x68, 0x41, 0x98 }, Kore.Utility.Misc.fromDWord(d, bytes: 8));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x47, 0x68, 0x41 }, Kore.Utility.Misc.fromDWord(d, bytes: 7));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x47, 0x68 }, Kore.Utility.Misc.fromDWord(d, bytes: 6));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63, 0x47 }, Kore.Utility.Misc.fromDWord(d, bytes: 5));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54, 0x63 }, Kore.Utility.Misc.fromDWord(d, bytes: 4));
            Assert.AreEqual(new byte[] { 0x31, 0x18, 0x54 }, Kore.Utility.Misc.fromDWord(d, bytes: 3));
            Assert.AreEqual(new byte[] { 0x31, 0x18 }, Kore.Utility.Misc.fromDWord(d, bytes: 2));
            Assert.AreEqual(new byte[] { 0x31 }, Kore.Utility.Misc.fromDWord(d, bytes: 1));

            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x31, 0x18, 0x54, 0x63 }, Kore.Utility.Misc.fromDWord(e, bytes: 8));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x31, 0x18, 0x54 }, Kore.Utility.Misc.fromDWord(e, bytes: 7));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x31, 0x18}, Kore.Utility.Misc.fromDWord(e, bytes: 6));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37, 0x31 }, Kore.Utility.Misc.fromDWord(e, bytes: 5));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81, 0x37 }, Kore.Utility.Misc.fromDWord(e, bytes: 4));
            Assert.AreEqual(new byte[] { 0x15, 0x48, 0x81 }, Kore.Utility.Misc.fromDWord(e, bytes: 3));
            Assert.AreEqual(new byte[] { 0x15, 0x48 }, Kore.Utility.Misc.fromDWord(e, bytes: 2));
            Assert.AreEqual(new byte[] { 0x15 }, Kore.Utility.Misc.fromDWord(e, bytes: 1));
        }
    }
}
