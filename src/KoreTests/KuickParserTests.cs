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

        [Test]
        public void stringTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = "  \"" + target + "\"  ";
                string page2 = "\"    " + target + "  \"";

                KuickParser.ParseData r1 = parser.parse(page1);
                Assert.AreEqual(KuickTokenizer.Token.STRING, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual("" + target, ((KuickParser.ParseData)r1.value).value);

                KuickParser.ParseData r2 = parser.parse(page2);
                Assert.AreEqual(KuickTokenizer.Token.STRING, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual("    " + target + "  ", ((KuickParser.ParseData)r2.value).value);

                Assert.AreEqual("[PAGE|[STRING|" + target + "]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[STRING|    " + target + "  ]]", (r2.ToString()));
            }
        }

        [Test]
        public void commentDoubleSlashTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = " //asdghjjheu \n " + target + " // " + target + " ";
                string page2 = "" + target + " //asgfjsdkahn";

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

        [Test]
        public void commentHashTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = " #asdghjjheu \n " + target + " # " + target + " ";
                string page2 = "" + target + " #asgfjsdkahn";

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

        [Test]
        public void commentSlashStarTest()
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int target = rand.Next();
                string page1 = " /*asdghjjheu*/ \n " + target + " /* \n" + target + "\n*/ ";
                string page2 = "" + target + " /*\n\n\n\n\n\nasgfjsdkahn*/";

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

        [TestCase("text", KuickTokenizer.Token.DIRECTIVE_TEXT)]
        [TestCase("data", KuickTokenizer.Token.DIRECTIVE_DATA)]
        [TestCase("bss", KuickTokenizer.Token.DIRECTIVE_BSS)]
        public void SimpleDirectiveTest(string directive, KuickTokenizer.Token type)
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int padding = rand.Next();
                string page1 = " ." + directive + " \n " + padding + " /* \n." + directive + "\n" + padding + "\n*/ ";
                string page2 = "" + padding + " \n\n\n\n\n\n." + directive + "";
                string page3 = "." + directive + "";

                KuickParser.ParseData r1 = parser.parse(page1);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);*/

                KuickParser.ParseData r2 = parser.parse(page2);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);*/
                KuickParser.ParseData r3 = parser.parse(page3);

                /* TestActionAttribute debugging lines
                Console.WriteLine("r1: " + page1 + "\n|\n" + r1.ToString());
                Console.WriteLine("r2: " + page2 + "\n|\n" + r2.ToString());
                */

                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[DIRECTIVE_" + directive.ToUpper() + "],[NUMBER_INT|" + padding + "]]]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[NUMBER_INT|" + padding + "],[DIRECTIVE_" + directive.ToUpper() + "]]]]", (r2.ToString()));
                Assert.AreEqual("[PAGE|[DIRECTIVE_" + directive.ToUpper() + "]]", (r3.ToString()));
            }
        }

        [TestCase("rvc")]
        [TestCase("norvc")]
        [TestCase("relax")]
        [TestCase("norelax")]
        [TestCase("pic")]
        [TestCase("nopic")]
        [TestCase("push")]
        [TestCase("pop")]
        public void OptionDirectiveTest(string option)
        {
            KuickParser parser = new KuickParser();

            for (int i = 0; i < 100; i++)
            {
                int padding = rand.Next();
                string page1 = " .option " + option + " \n " + padding + " /* \n.option " + option + "\n" + padding + "\n*/ ";
                string page2 = "" + padding + " \n\n\n\n\n\n.option " + option + "";
                string page3 = ".option " + option + "";

                KuickParser.ParseData r1 = parser.parse(page1);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r1.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r1.value).value);*/

                KuickParser.ParseData r2 = parser.parse(page2);
                /*Assert.AreEqual(type, ((KuickParser.ParseData)r2.value).type);
                Assert.AreEqual(target, ((KuickParser.ParseData)r2.value).value);*/
                KuickParser.ParseData r3 = parser.parse(page3);

                /* TestActionAttribute debugging lines
                Console.WriteLine("r1: " + page1 + "\n|\n" + r1.ToString());
                Console.WriteLine("r2: " + page2 + "\n|\n" + r2.ToString());
                */

                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[DIRECTIVE_OPTION|" + option + "],[NUMBER_INT|" + padding + "]]]]", (r1.ToString()));
                Assert.AreEqual("[PAGE|[EXPRESSION_LIST|[[NUMBER_INT|" + padding + "],[DIRECTIVE_OPTION|" + option + "]]]]", (r2.ToString()));
                Assert.AreEqual("[PAGE|[DIRECTIVE_OPTION|" + option + "]]", (r3.ToString()));
            }
        }
    }
}
