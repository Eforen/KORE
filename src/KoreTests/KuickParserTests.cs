using Kore;
using Kore.RiscISA.Instruction;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace KoreTests
{
    public class KuickParserTests
    {
        private static Random rand = new Random();


        [Test]
        public void simpleDevTest()
        {
            KuickParser parser = new KuickParser();

            string page = "42";

            object r = parser.parse(page);

            Console.WriteLine(r.ToString());
            Assert.AreEqual("[PAGE|[NUMBER_INT|42]]", (r.ToString()));
        }

        [Test]
        public void numberTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = "  " + target + "  ";
                string page2 = "" + target;

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.NUMBER_INT, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[NUMBER_INT|" + target + "]]", (r2.ToString()));
            }


        }
    }
}
