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
    public class KuickCompilerTest
    {
        /*  0 */ [TestCase("addi  a3,a0,4   # Lorem ipsum dolor sit amet, consectetur adipiscing elit.", 0x00450693u)]
        /*  4 */ [TestCase("addi  a4,x0,1   # Sed eget nisl eget ipsum rutrum congue id in libero.", 0x00100713u)]
        /*  8 */ [TestCase("bltu  a4,a1,10  # Aenean nec lacus eget diam placerat accumsan nec et nunc.", 0x00b76463u)]
        /*  C */ [TestCase("jalr  x0,x1,0   # Ut ultrices diam et bibendum bibendum.", 0x00008067u)]
        /* 10 */ [TestCase("lw    a6, 0(a3) # Aenean congue nunc non molestie accumsan.", 0x0006a803u)]
        /* 14 */ [TestCase("addi  a2,a3,0   # Maecenas tincidunt nisi non pretium vulputate.", 0x00068613u)]
        /* 18 */ [TestCase("addi  a5,a4,0   # Aliquam pharetra justo eget erat consectetur pharetra.", 0x00070793u)]
        /* 1C */ [TestCase("lw    a7,-4(a2) # Nunc condimentum felis eget fermentum sodales.", 0xffc62883u)]
        /* 20 */ [TestCase("bge   a6,a7,34  # Praesent vel nulla varius metus consequat mattis.", 0x01185a63u)]
        /* 24 */ [TestCase("sw    a7,0(a2)  # Curabitur volutpat quam convallis dolor tempus viverra.", 0x01162023u)]
        /* 28 */ [TestCase("addi  a5,a5,-1   ", 0xfff78793u)]
        /* 2C */ [TestCase("addi  a2,a2,-4  # Duis sagittis ante a quam faucibus suscipit.", 0xffc60613u)]
        /* 30 */ [TestCase("bne   a5,x0,1c   ", 0xfe0796e3u)]
        /* 34 */ [TestCase("slli  a5,a5,2 # Pellentesque ut lacus at tellus hendrerit iaculis ac in nunc.", 0x00279793u)]
        /* 38 */ [TestCase("add a5,a0,a5 # Curabitur quis nisl eget nisi posuere tempus in a nibh.", 0x00f507b3u)]
        /* 3C */ [TestCase("sw a6,0(a5) # Morbi rutrum risus viverra, placerat leo in, facilisis urna.", 0x0107a023u)]
        /* 40 */ [TestCase("addi a4,a4,1 # Fusce in erat eu turpis convallis ultrices sed bibendum justo.", 0x00170713u)]
        /* 44 */ [TestCase("addi a3,a3,4 # foo", 0x00468693u)]
        /* 48 */ [TestCase("jal x0, 8 # Bar", 0xfc1ff06fu)]
        public void compileDataSingle(string str, uint code)
        {
            uint result = KuickCompiler.compile(str);
            Assert.AreEqual(code, result, 
                "Data Mismatch\n" + 
                Convert.ToString((int)(code), 2).PadLeft(32, '0') + 
                "\n" +
                Convert.ToString((int)(result), 2).PadLeft(32, '0')
            );
        }

        public void compileDataMulti(string[] strings, uint[] codes)
        {
        }

        public void compileRTypeStruct(string str, RType inst)
        {
        }

        public void compileITypeStruct(string str, IType inst)
        {
        }

        public void compileSTypeStruct(string str, SType inst)
        {
        }

        public void compileBTypeStruct(string str, BType inst)
        {
        }

        public void compileUTypeStruct(string str, UType inst)
        {
        }

        public void compileJTypeStruct(string str, JType inst)
        {
        }
    }
}
