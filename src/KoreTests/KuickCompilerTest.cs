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
    public static class KuickCompilerTestUtils {
    }
    public class KuickCompilerTest
    {
        private static Random rand = new Random();
        
        /** get random 4 byte aligned byte possition */
        public static int getRandomInstructionStartingPoint(){
            return rand.Next(1000)*4;
        }

        /*  0 */ [TestCase(0x00, "addi  a3,a0,4   # Lorem ipsum dolor sit amet, consectetur adipiscing elit.", 0x00450693u)]
        /*  4 */ [TestCase(0x04, "addi  a4,x0,1   # Sed eget nisl eget ipsum rutrum congue id in libero.", 0x00100713u)]
        /*  8 */ [TestCase(0x08, "bltu  a4,a1,10  # Aenean nec lacus eget diam placerat accumsan nec et nunc.", 0x00b76463u)]
        /*  C */ [TestCase(0x0C, "jalr  x0,x1,0   # Ut ultrices diam et bibendum bibendum.", 0x00008067u)]
        /* 10 */ [TestCase(0x10, "lw    a6, 0(a3) # Aenean congue nunc non molestie accumsan.", 0x0006a803u)]
        /* 14 */ [TestCase(0x14, "addi  a2,a3,0   # Maecenas tincidunt nisi non pretium vulputate.", 0x00068613u)]
        /* 18 */ [TestCase(0x18, "addi  a5,a4,0   # Aliquam pharetra justo eget erat consectetur pharetra.", 0x00070793u)]
        /* 1C */ [TestCase(0x1C, "lw    a7,-4(a2) # Nunc condimentum felis eget fermentum sodales.", 0xffc62883u)]
        /* 20 */ [TestCase(0x20, "bge   a6,a7,34  # Praesent vel nulla varius metus consequat mattis.", 0b0000000_10001_10000_101_10100_1100011u)]
        /* 24 */ [TestCase(0x24, "sw    a7,0(a2)  # Curabitur volutpat quam convallis dolor tempus viverra.", 0x01162023u)]
        /* 28 */ [TestCase(0x28, "addi  a5,a5,-1   ", 0xfff78793u)]
        /* 2C */ [TestCase(0x2C, "addi  a2,a2,-4  # Duis sagittis ante a quam faucibus suscipit.", 0xffc60613u)]
        /* 30 */ [TestCase(0x30, "bne   a5,x0,1c   ", 0xfe0796e3u)]
        /* 34 */ [TestCase(0x34, "slli  a5,a5,2 # Pellentesque ut lacus at tellus hendrerit iaculis ac in nunc.", 0x00279793u)]
        /* 38 */ [TestCase(0x38, "add a5,a0,a5 # Curabitur quis nisl eget nisi posuere tempus in a nibh.", 0x00f507b3u)]
        /* 3C */ [TestCase(0x3C, "sw a6,0(a5) # Morbi rutrum risus viverra, placerat leo in, facilisis urna.", 0x0107a023u)]
        /* 40 */ [TestCase(0x40, "addi a4,a4,1 # Fusce in erat eu turpis convallis ultrices sed bibendum justo.", 0x00170713u)]
        /* 44 */ [TestCase(0x44, "addi a3,a3,4 # foo", 0x00468693u)]
        /* 48 */ [TestCase(0x48, "jal x0, 8 # Bar", 0xfc1ff06fu)]
        [TestCase(0x20, "bne     a6,a7,28 # From Compiler", 0x01181463u)]
        [TestCase(-1, "li      a5,0", 0x00000793u)]
        [TestCase(-1, "lui     a0,0x10", 0x00010537u)]
        [TestCase(-1, "ret", 0x00008067u)]
        [TestCase(-1, "auipc   gp,0x2", 0x00002197u)]
        [TestCase(-1, "sub     a2,a2,a0", 0x40a60633u)]
        [TestCase(-1, "li      a1,0", 0x00000593u)]
        [TestCase(-1, "auipc   a0,0x0", 0x00000517u)]
        [TestCase(-1, "lw      a0,0(sp)", 0x00012503u)]
        [TestCase(-1, "addi    a1,sp,8", 0x00810593u)]
        [TestCase(-1, "li      a2,0", 0x00000613u)]
        [TestCase(-1, "addi    sp,sp,-16", 0xff010113u)]
        [TestCase(-1, "sd      s0,0(sp)", 0x00813023u)]
        [TestCase(-1, "mv      s0,a5", 0x00078413u)]
        [TestCase(-1, "sd      ra,8(sp)", 0x00113423u)]
        [TestCase(-1, "lui     a0,0x11", 0x00011537u)]
        [TestCase(-1, "auipc   ra,0x0", 0x00000097u)]
        [TestCase(-1, "li      a5,1", 0x00100793u)]
        [TestCase(-1, "ld      ra,8(sp)", 0x00813083u)]
        [TestCase(-1, "ld      s0,0(sp)", 0x00013403u)]
        [TestCase(-1, "addi    sp,sp,16", 0x01010113u)]
        [TestCase(-1, "auipc   t1,0x0", 0x00000317u)]
        [TestCase(-1, "mv      s0,a0", 0x00050413u)]
        [TestCase(-1, "ld      a5,88(a0)", 0x05853783u)]
        [TestCase(-1, "jalr    a5", 0x000780e7u)]
        [TestCase(-1, "mv      a0,s0", 0x00040513u)]
        [TestCase(-1, "addi    sp,sp,-32", 0xfe010113u)]
        [TestCase(-1, "sd      s0,16(sp)", 0x00813823u)]
        [TestCase(-1, "sd      s2,0(sp)", 0x01213023u)]
        [TestCase(-1, "lui     s0,0x11", 0x00011437u)]
        [TestCase(-1, "lui     s2,0x11", 0x00011937u)]
        [TestCase(-1, "sub     s2,s2,a5", 0x40f90933u)]
        [TestCase(-1, "sd      ra,24(sp)", 0x00113c23u)]
        [TestCase(-1, "sd      s1,8(sp)", 0x00913423u)]
        [TestCase(-1, "srai    s2,s2,0x3", 0x40395913u)]
        [TestCase(-1, "addi    s0,s0,1468", 0x5bc40413u)]
        [TestCase(-1, "li      s1,0", 0x00000493u)]
        [TestCase(-1, "ld      a5,0(s0)", 0x00043783u)]
        [TestCase(-1, "addi    s1,s1,1", 0x00148493u)]
        [TestCase(-1, "addi    s0,s0,8", 0x00840413u)]
        [TestCase(-1, "addi    s0,s0,1472", 0x5c040413u)]
        [TestCase(-1, "ld      ra,24(sp)", 0x01813083u)]
        [TestCase(-1, "ld      s0,16(sp)", 0x01013403u)]
        [TestCase(-1, "ld      s1,8(sp)", 0x00813483u)]
        [TestCase(-1, "ld      s2,0(sp)", 0x00013903u)]
        [TestCase(-1, "addi    sp,sp,32", 0x02010113u)]
        [TestCase(-1, "li      t1,15", 0x00f00313u)]
        [TestCase(-1, "mv      a4,a0", 0x00050713u)]
        [TestCase(-1, "andi    a5,a4,15", 0x00f77793u)]
        [TestCase(-1, "andi    a3,a2,-16", 0xff067693u)]
        [TestCase(-1, "andi    a2,a2,15", 0x00f67613u)]
        [TestCase(-1, "add     a3,a3,a4", 0x00e686b3u)]
        [TestCase(-1, "sd      a1,0(a4)", 0x00b73023u)]
        [TestCase(-1, "sd      a1,8(a4)", 0x00b73423u)]
        [TestCase(-1, "addi    a4,a4,16", 0x01070713u)]
        [TestCase(-1, "sub     a3,t1,a2", 0x40c306b3u)]
        [TestCase(-1, "slli    a3,a3,0x2", 0x00269693u)]
        [TestCase(-1, "auipc   t0,0x0", 0x00000297u)]
        [TestCase(-1, "add     a3,a3,t0", 0x005686b3u)]
        [TestCase(-1, "jr      12(a3)", 0x00c68067u)]
        [TestCase(-1, "sb      a1,14(a4)", 0x00b70723u)]
        [TestCase(-1, "sb      a1,13(a4)", 0x00b706a3u)]
        [TestCase(-1, "sb      a1,12(a4)", 0x00b70623u)]
        [TestCase(-1, "sb      a1,11(a4)", 0x00b705a3u)]
        [TestCase(-1, "sb      a1,10(a4)", 0x00b70523u)]
        [TestCase(-1, "sb      a1,9(a4)", 0x00b704a3u)]
        [TestCase(-1, "sb      a1,8(a4)", 0x00b70423u)]
        [TestCase(-1, "sb      a1,7(a4)", 0x00b703a3u)]
        [TestCase(-1, "sb      a1,6(a4)", 0x00b70323u)]
        [TestCase(-1, "sb      a1,5(a4)", 0x00b702a3u)]
        [TestCase(-1, "sb      a1,4(a4)", 0x00b70223u)]
        [TestCase(-1, "sb      a1,3(a4)", 0x00b701a3u)]
        [TestCase(-1, "sb      a1,2(a4)", 0x00b70123u)]
        [TestCase(-1, "sb      a1,1(a4)", 0x00b700a3u)]
        [TestCase(-1, "sb      a1,0(a4)", 0x00b70023u)]
        [TestCase(-1, "andi    a1,a1,255", 0x0ff5f593u)]
        [TestCase(-1, "slli    a3,a1,0x8", 0x00859693u)]
        [TestCase(-1, "or      a1,a1,a3", 0x00d5e5b3u)]
        [TestCase(-1, "slli    a3,a1,0x10", 0x01059693u)]
        [TestCase(-1, "slli    a3,a1,0x20", 0x02059693u)]
        [TestCase(-1, "slli    a3,a5,0x2", 0x00279693u)]
        [TestCase(-1, "mv      t0,ra", 0x00008293u)]
        [TestCase(-1, "jalr    -104(a3)", 0xf98680e7u)]
        [TestCase(-1, "mv      ra,t0", 0x00028093u)]
        [TestCase(-1, "addi    a5,a5,-16", 0xff078793u)]
        [TestCase(-1, "sub     a4,a4,a5", 0x40f70733u)]
        [TestCase(-1, "add     a2,a2,a5", 0x00f60633u)]
        [TestCase(-1, "addi    sp,sp,-80", 0xfb010113u)]
        [TestCase(-1, "sd      s4,32(sp)", 0x03413023u)]
        [TestCase(-1, "sd      s2,48(sp)", 0x03213823u)]
        [TestCase(-1, "sd      ra,72(sp)", 0x04113423u)]
        [TestCase(-1, "ld      s2,504(s4)", 0x1f8a3903u)]
        [TestCase(-1, "sd      s0,64(sp)", 0x04813023u)]
        [TestCase(-1, "sd      s1,56(sp)", 0x02913c23u)]
        [TestCase(-1, "sd      s3,40(sp)", 0x03313423u)]
        [TestCase(-1, "sd      s5,24(sp)", 0x01513c23u)]
        [TestCase(-1, "sd      s6,16(sp)", 0x01613823u)]
        [TestCase(-1, "sd      s7,8(sp)", 0x01713423u)]
        [TestCase(-1, "sd      s8,0(sp)", 0x01813023u)]
        [TestCase(-1, "mv      s6,a0", 0x00050b13u)]
        [TestCase(-1, "mv      s7,a1", 0x00058b93u)]
        [TestCase(-1, "li      s5,1", 0x00100a93u)]
        [TestCase(-1, "li      s3,-1", 0xfff00993u)]
        [TestCase(-1, "lw      s1,8(s2)", 0x00892483u)]
        [TestCase(-1, "addiw   s0,s1,-1", 0xfff4841bu)]
        [TestCase(-1, "slli    s1,s1,0x3", 0x00349493u)]
        [TestCase(-1, "add     s1,s2,s1", 0x009904b3u)]
        [TestCase(-1, "ld      a5,520(s1)", 0x2084b783u)]
        [TestCase(-1, "addiw   s0,s0,-1", 0xfff4041bu)]
        [TestCase(-1, "addi    s1,s1,-8", 0xff848493u)]
        [TestCase(-1, "ld      ra,72(sp)", 0x04813083u)]
        [TestCase(-1, "ld      s0,64(sp)", 0x04013403u)]
        [TestCase(-1, "ld      s1,56(sp)", 0x03813483u)]
        [TestCase(-1, "ld      s2,48(sp)", 0x03013903u)]
        [TestCase(-1, "ld      s3,40(sp)", 0x02813983u)]
        [TestCase(-1, "ld      s4,32(sp)", 0x02013a03u)]
        [TestCase(-1, "ld      s5,24(sp)", 0x01813a83u)]
        [TestCase(-1, "ld      s6,16(sp)", 0x01013b03u)]
        [TestCase(-1, "ld      s7,8(sp)", 0x00813b83u)]
        [TestCase(-1, "ld      s8,0(sp)", 0x00013c03u)]
        [TestCase(-1, "addi    sp,sp,80", 0x05010113u)]
        [TestCase(-1, "lw      a5,8(s2)", 0x00892783u)]
        [TestCase(-1, "ld      a4,8(s1)", 0x0084b703u)]
        [TestCase(-1, "addiw   a5,a5,-1", 0xfff7879bu)]
        [TestCase(-1, "sd      zero,8(s1)", 0x0004b423u)]
        [TestCase(-1, "lw      a5,784(s2)", 0x31092783u)]
        [TestCase(-1, "sllw    a3,s5,s0", 0x008a96bbu)]
        [TestCase(-1, "lw      s8,8(s2)", 0x00892c03u)]
        [TestCase(-1, "and     a5,a5,a3", 0x00d7f7b3u)]
        [TestCase(-1, "sext.w  a5,a5", 0x0007879bu)]
        [TestCase(-1, "jalr    a4", 0x000700e7u)]
        [TestCase(-1, "lw      a4,8(s2)", 0x00892703u)]
        [TestCase(-1, "ld      a5,504(s4)", 0x1f8a3783u)]
        [TestCase(-1, "mv      s2,a5", 0x00078913u)]
        [TestCase(-1, "lw      a5,788(s2)", 0x31492783u)]
        [TestCase(-1, "ld      a1,264(s1)", 0x1084b583u)]
        [TestCase(-1, "mv      a0,s6", 0x000b0513u)]
        [TestCase(-1, "sw      s0,8(s2)", 0x00892423u)]
        [TestCase(-1, "mv      a0,a1", 0x00058513u)]
        [TestCase(-1, "mv      a1,a0", 0x00050593u)]
        [TestCase(-1, "li      a3,0", 0x00000693u)]
        [TestCase(-1, "li      a0,0", 0x00000513u)]
        [TestCase(-1, "lui     a5,0x11", 0x000117b7u)]
        [TestCase(-1, "sub     a5,a5,s0", 0x408787b3u)]
        [TestCase(-1, "srai    s1,a5,0x3", 0x4037d493u)]
        [TestCase(-1, "addi    a5,a5,-8", 0xff878793u)]
        [TestCase(-1, "add     s0,a5,s0", 0x00878433u)]
        [TestCase(-1, "addi    s1,s1,-1", 0xfff48493u)]
        [TestCase(-1, "addi    s0,s0,-8", 0xff840413u)]
        [TestCase(-1, "ld      a5,504(a4)", 0x1f873783u)]
        [TestCase(-1, "lw      a4,8(a5)", 0x0087a703u)]
        [TestCase(-1, "li      a6,31", 0x01f00813u)]
        [TestCase(-1, "slli    a6,a4,0x3", 0x00371813u)]
        [TestCase(-1, "add     a6,a5,a6", 0x01078833u)]
        [TestCase(-1, "sd      a2,272(a6)", 0x10c83823u)]
        [TestCase(-1, "lw      a7,784(a5)", 0x3107a883u)]
        [TestCase(-1, "li      a2,1", 0x00100613u)]
        [TestCase(-1, "sllw    a2,a2,a4", 0x00e6163bu)]
        [TestCase(-1, "or      a7,a7,a2", 0x00c8e8b3u)]
        [TestCase(-1, "sw      a7,784(a5)", 0x3117a823u)]
        [TestCase(-1, "sd      a3,528(a6)", 0x20d83823u)]
        [TestCase(-1, "li      a3,2", 0x00200693u)]
        [TestCase(-1, "addi    a3,a4,2", 0x00270693u)]
        [TestCase(-1, "slli    a3,a3,0x3", 0x00369693u)]
        [TestCase(-1, "addiw   a4,a4,1", 0x0017071bu)]
        [TestCase(-1, "sw      a4,8(a5)", 0x00e7a423u)]
        [TestCase(-1, "add     a5,a5,a3", 0x00d787b3u)]
        [TestCase(-1, "sd      a1,0(a5)", 0x00b7b023u)]
        [TestCase(-1, "addi    a5,a4,512", 0x20070793u)]
        [TestCase(-1, "sd      a5,504(a4)", 0x1ef73c23u)]
        [TestCase(-1, "lw      a3,788(a5)", 0x3147a683u)]
        [TestCase(-1, "or      a2,a3,a2", 0x00c6e633u)]
        [TestCase(-1, "sw      a2,788(a5)", 0x30c7aa23u)]
        [TestCase(-1, "li      a0,-1", 0xfff00513u)]
        [TestCase(-1, "li      a4,0", 0x00000713u)]
        [TestCase(-1, "li      a7,93", 0x05d00893u)]
        [TestCase(-1, "ecall", 0x00000073u)]
        [TestCase(-1, "negw    s0,s0", 0x4080043bu)]
        [TestCase(-1, "sw      s0,0(a0)", 0x00852023u)]
        [TestCase(-1, "ebreak", 0x00100073u)]
        public void compileDataSingle(int opStartByte, string str, uint code)
        {
            if(opStartByte < 0) opStartByte = getRandomInstructionStartingPoint();
            uint result = KuickCompiler.assembleSingleLine(opStartByte, str);
            Assert.AreEqual(code, result,
                TestUtils.getDataMismatchString(code,result)
            );
        }

        // [TestCase(0x04 * 00, "beqz    a5,100c4 # <register_fini+0x14>", 0x00078863u)]
        [TestCase(0x04 * 01, "addi    a0,a0,1128 # 10468 # <__libc_fini_array>", 0x46850513u)]
        [TestCase(0x04 * 02, "j       10454 # <atexit>", 0x3940006fu)]
        [TestCase(0x04 * 03, "addi    gp,gp,-752 # 11dd8 # <__global_pointer$>", 0xd1018193u)]
        [TestCase(0x04 * 04, "addi    a0,gp,-160 # 11d38 # <completed.1>", 0xf6018513u)]
        [TestCase(0x04 * 05, "addi    a2,gp,-104 # 11d70 # <__BSS_END__>", 0xf9818613u)]
        [TestCase(0x04 * 06, "jal     ra,10250 # <memset>", 0x170000efu)]
        [TestCase(0x04 * 07, "addi    a0,a0,880 # 10454 # <atexit>", 0x37050513u)]
        // [TestCase(0x04 * 08, "beqz    a0,100fc # <_start+0x34>", 0x00050863u)]
        [TestCase(0x04 * 09, "addi    a0,a0,888 # 10468 # <__libc_fini_array>", 0x37850513u)]
        [TestCase(0x04 * 10, "jal     ra,10454 # <atexit>", 0x35c000efu)]
        [TestCase(0x04 * 11, "jal     ra,101b4 # <__libc_init_array>", 0x0b8000efu)]
        [TestCase(0x04 * 12, "jal     ra,10180 # <main>", 0x074000efu)]
        [TestCase(0x04 * 13, "j       10184 # <exit>", 0x0740006fu)]
        [TestCase(0x04 * 14, "lbu     a4,-160(gp) # 11d38 # <completed.1>", 0xf601c703u)]
        // [TestCase(0x04 * 15, "bnez    a4,1015c # <__do_global_dtors_aux+0x48>", 0x04071263u)]
        // [TestCase(0x04 * 16, "beqz    a5,10144 # <__do_global_dtors_aux+0x30>", 0x00078a63u)]
        [TestCase(0x04 * 17, "addi    a0,a0,1464 # 115b8 # <__FRAME_END__>", 0x5b850513u)]
        [TestCase(0x04 * 18, "jalr    zero # 0 # <register_fini-0x100b0>", 0x000000e7u)]
        [TestCase(0x04 * 19, "sb      a5,-160(gp) # 11d38 # <completed.1>", 0xf6f18023u)]
        // [TestCase(0x04 * 20, "beqz    a5,1017c # <frame_dummy+0x1c>", 0x00078c63u)]
        [TestCase(0x04 * 21, "addi    a1,gp,-152 # 11d40 # <object.0>", 0xf6818593u)]
        [TestCase(0x04 * 22, "jr      zero # 0 # <register_fini-0x100b0>", 0x00000067u)]
        [TestCase(0x04 * 23, "addi    t0,t1,10 # 1017e # <frame_dummy+0x1e>", 0x00a30293u)]
        [TestCase(0x04 * 24, "jal     ra,1032c # <__call_exitprocs>", 0x194000efu)]
        [TestCase(0x04 * 25, "ld      a0,-184(gp) # 11d20 # <_global_impure_ptr>", 0xf481b503u)]
        // [TestCase(0x04 * 26, "beqz    a5,101ac # <exit+0x28>", 0x00078463u)]
        [TestCase(0x04 * 27, "jal     ra,1056c # <_exit>", 0x3bc000efu)]
        [TestCase(0x04 * 28, "addi    a5,s0,1468 # 115bc # <__preinit_array_end>", 0x5bc40793u)]
        [TestCase(0x04 * 29, "addi    s2,s2,1468 # 115bc # <__preinit_array_end>", 0x5bc90913u)]
        // [TestCase(0x04 * 30, "beqz    s2,10200 # <__libc_init_array+0x4c>", 0x02090063u)]
        [TestCase(0x04 * 31, "bne     s2,s1,101ec # <__libc_init_array+0x38>", 0xfe9918e3u)]
        [TestCase(0x04 * 32, "addi    a5,s0,1472 # 115c0 # <__init_array_start>", 0x5c040793u)]
        [TestCase(0x04 * 33, "addi    s2,s2,1488 # 115d0 # <__do_global_dtors_aux_fini_array_entry>", 0x5d090913u)]
        // [TestCase(0x04 * 34, "beqz    s2,10238 # <__libc_init_array+0x84>", 0x02090063u)]
        [TestCase(0x04 * 35, "bne     s2,s1,10224 # <__libc_init_array+0x70>", 0xfe9918e3u)]
        [TestCase(0x04 * 36, "bgeu    t1,a2,1028c # <memset+0x3c>", 0x02c37a63u)]
        // [TestCase(0x04 * 37, "bnez    a5,10300 # <memset+0xb0>", 0x0a079063u)]
        // [TestCase(0x04 * 38, "bnez    a1,102e0 # <memset+0x90>", 0x06059e63u)]
        [TestCase(0x04 * 39, "bltu    a4,a3,10274 # <memset+0x24>", 0xfed76ae3u)]
        // [TestCase(0x04 * 40, "bnez    a2,1028c # <memset+0x3c>", 0x00061463u)]
        [TestCase(0x04 * 41, "j       10268 # <memset+0x18>", 0xf6dff06fu)]
        [TestCase(0x04 * 42, "bgeu    t1,a2,1028c # <memset+0x3c>", 0xf6c374e3u)]
        [TestCase(0x04 * 43, "j       10264 # <memset+0x14>", 0xf3dff06fu)]
        [TestCase(0x04 * 44, "ld      s4,-184(gp) # 11d20 # <_global_impure_ptr>", 0xf481ba03u)]
        // [TestCase(0x04 * 45, "beqz    s2,103a0 # <__call_exitprocs+0x74>", 0x04090063u)]
        [TestCase(0x04 * 46, "bltz    s0,103a0 # <__call_exitprocs+0x74>", 0x02044263u)]
        // [TestCase(0x04 * 47, "beqz    s7,103d0 # <__call_exitprocs+0xa4>", 0x040b8463u)]
        [TestCase(0x04 * 48, "beq     a5,s7,103d0 # <__call_exitprocs+0xa4>", 0x05778063u)]
        [TestCase(0x04 * 49, "bne     s0,s3,10388 # <__call_exitprocs+0x5c>", 0xff3416e3u)]
        [TestCase(0x04 * 50, "beq     a5,s0,10440 # <__call_exitprocs+0x114>", 0x06878263u)]
        // [TestCase(0x04 * 51, "beqz    a4,10394 # <__call_exitprocs+0x68>", 0xfa0708e3u)]
        // [TestCase(0x04 * 52, "bnez    a5,10420 # <__call_exitprocs+0xf4>", 0x02079263u)]
        [TestCase(0x04 * 53, "bne     a4,s8,10414 # <__call_exitprocs+0xe8>", 0x01871463u)]
        [TestCase(0x04 * 54, "beq     a5,s2,10394 # <__call_exitprocs+0x68>", 0xf92782e3u)]
        // [TestCase(0x04 * 55, "beqz    a5,103a0 # <__call_exitprocs+0x74>", 0xf80786e3u)]
        [TestCase(0x04 * 56, "j       10374 # <__call_exitprocs+0x48>", 0xf59ff06fu)]
        // [TestCase(0x04 * 57, "bnez    a5,10448 # <__call_exitprocs+0x11c>", 0x00079c63u)]
        [TestCase(0x04 * 58, "j       10404 # <__call_exitprocs+0xd8>", 0xfc9ff06fu)]
        [TestCase(0x04 * 59, "j       103e4 # <__call_exitprocs+0xb8>", 0xfa1ff06fu)]
        [TestCase(0x04 * 60, "j       10404 # <__call_exitprocs+0xd8>", 0xfb5ff06fu)]
        [TestCase(0x04 * 61, "j       104c4 # <__register_exitproc>", 0x0600006fu)]
        [TestCase(0x04 * 62, "addi    s0,s0,1488 # 115d0 # <__do_global_dtors_aux_fini_array_entry>", 0x5d040413u)]
        [TestCase(0x04 * 63, "addi    a5,a5,1496 # 115d8 # <impure_data>", 0x5d878793u)]
        // [TestCase(0x04 * 64, "beqz    s1,104b0 # <__libc_fini_array+0x48>", 0x02048063u)]
        // [TestCase(0x04 * 65, "bnez    s1,1049c # <__libc_fini_array+0x34>", 0xfe0498e3u)]
        [TestCase(0x04 * 66, "ld      a4,-184(gp) # 11d20 # <_global_impure_ptr>", 0xf481b703u)]
        // [TestCase(0x04 * 67, "beqz    a5,1052c # <__register_exitproc+0x68>", 0x06078063u)]
        [TestCase(0x04 * 68, "blt     a6,a4,10564 # <__register_exitproc+0xa0>", 0x08e84663u)]
        // [TestCase(0x04 * 69, "beqz    a0,1050c # <__register_exitproc+0x48>", 0x02050863u)]
        [TestCase(0x04 * 70, "beq     a0,a3,10538 # <__register_exitproc+0x74>", 0x02d50863u)]
        [TestCase(0x04 * 71, "j       104d0 # <__register_exitproc+0xc>", 0xf9dff06fu)]
        [TestCase(0x04 * 72, "bltz    a0,10590 # <_exit+0x24>", 0x00054463u)]
        [TestCase(0x04 * 73, "j       1058c # <_exit+0x20>", 0x0000006fu)]
        [TestCase(0x04 * 74, "jal     ra,105b0 # <__errno>", 0x00c000efu)]
        [TestCase(0x04 * 75, "j       105ac # <_exit+0x40>", 0x0000006fu)]
        [TestCase(0x04 * 76, "ld      a0,-168(gp) # 11d30 # <_impure_ptr>", 0xf581b503u)]
        public void compileDataSingleComplexRef(int opStartByte, string str, uint code)
        {
            if(opStartByte < 0) opStartByte = getRandomInstructionStartingPoint();
            uint result = KuickCompiler.assembleSingleLine(opStartByte, str);
            Assert.AreEqual(code, result, TestUtils.getDataMismatchString(code, result));
        }

        [Test, Ignore("Not Understood Yet")]
        public void asemblerMacros(string[] strings, uint[] codes)
        {
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
