using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore
{
    namespace Utility
    {
        public static class Misc
        {
            public static ulong toDWord(byte byte8, byte byte7 = 0, byte byte6 = 0, byte byte5 = 0, byte byte4 = 0, byte byte3 = 0, byte byte2 = 0, byte byte1 = 0)
            {
                return byte8 |
                   ((ulong)byte7 << 8) |
                   ((ulong)byte6 << 16) |
                   ((ulong)byte5 << 24) |
                   ((ulong)byte4 << 32) |
                   ((ulong)byte3 << 40) |
                   ((ulong)byte2 << 48) |
                   ((ulong)byte1 << 56);
            }
            public static ulong[] toDWords(byte[] data)
            {
                int totalCount = data.Length / 8;
                
                if(data.Length % 8 != 0)
                {
                    // Not exactly 8 byte aligned
                    totalCount++;
                }

                ulong[] output = new ulong[totalCount];


                int r = totalCount - 1;

                if (data.Length % 8 != 0)
                {
                    // Not Exactly 8 byte aligned
                    int leftover = data.Length % 8;
                    output[r] = toDWord(
                        leftover >= 1 ? data[r * 8] : (byte)0x00u,
                        leftover >= 2 ? data[r * 8 + 1] : (byte)0x00u,
                        leftover >= 3 ? data[r * 8 + 2] : (byte)0x00u,
                        leftover >= 4 ? data[r * 8 + 3] : (byte)0x00u,
                        leftover >= 5 ? data[r * 8 + 4] : (byte)0x00u,
                        leftover >= 6 ? data[r * 8 + 5] : (byte)0x00u,
                        leftover >= 7 ? data[r * 8 + 6] : (byte)0x00u
                        );
                    r--;
                }
                while (r >= 0)
                {
                    output[r] = toDWord(
                        data[r * 8],
                        data[r * 8 + 1],
                        data[r * 8 + 2],
                        data[r * 8 + 3],
                        data[r * 8 + 4],
                        data[r * 8 + 5],
                        data[r * 8 + 6],
                        data[r * 8 + 7]
                        );
                    r--;
                }
                return output;
            }
//             public static byte[] toBytes(ulong data)
//             {
// 
//             }
//             public static byte[] toBytes(ulong[] data)
//             {
// 
//             }
        }
    }
}
