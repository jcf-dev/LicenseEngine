namespace KeyCommon
{
    public class KeyByteSet
    {
        public KeyByteSet(int keyByteNo, byte keyByteA, byte keyByteB, byte keyByteC)
        {
            KeyByteNo = keyByteNo;
            KeyByteA = keyByteA;
            KeyByteB = keyByteB;
            KeyByteC = keyByteC;
        }

        public int KeyByteNo { get; }
        public byte KeyByteA { get; }
        public byte KeyByteB { get; }
        public byte KeyByteC { get; }
    }
}