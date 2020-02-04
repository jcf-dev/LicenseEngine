using System.Collections;
using KeyCommon;

namespace KeyGenerate
{
    public class KeyByteSetComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var kbs1 = (KeyByteSet) x;
            var kbs2 = (KeyByteSet) y;

            if (kbs1.KeyByteNo > kbs2.KeyByteNo) return 1;

            if (kbs1.KeyByteNo < kbs2.KeyByteNo) return -1;

            return 0;
        }
    }
}