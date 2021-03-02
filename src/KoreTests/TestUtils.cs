using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KoreTests
{
    public static class TestUtils
    {
        public static string getRootDataSplitString(uint data)
        {
            Match m = Regex.Match(Convert.ToString((int)(data), 2).PadLeft(32, '0'), @"(.......)(.....)(.....)(...)(.....)(.......)");
            GroupCollection g = m.Groups;
            return String.Format("|{0}|{1}|{2}|{3}|{4}|{5}|", g[1].Value, g[2].Value, g[3].Value, g[4].Value, g[5].Value, g[6].Value);
        }
        public static string getDataMismatchString(uint expected, uint actual)
        {
                                                           // Expect:
            return String.Format("Data Mismatch\n  Bit#: {0}\n        {1}\nExpect: {2}\nActual: {3}", "|3332222|22222|21111|111|11100|0000000|", "|2109876|54321|09876|543|21098|7654321|", getRootDataSplitString(expected), getRootDataSplitString(actual));
//             return "Data Mismatch\n" +
//                 Convert.ToString((int)(code), 2).PadLeft(32, '0') +
//                 "\n" +
//                 Convert.ToString((int)(result), 2).PadLeft(32, '0');
        }
    }
}
