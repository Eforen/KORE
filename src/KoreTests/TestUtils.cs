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
            return String.Format("Data Mismatch\nExpect: {0}\nActual: {1}", getRootDataSplitString(expected), getRootDataSplitString(actual));
//             return "Data Mismatch\n" +
//                 Convert.ToString((int)(code), 2).PadLeft(32, '0') +
//                 "\n" +
//                 Convert.ToString((int)(result), 2).PadLeft(32, '0');
        }
    }
}
