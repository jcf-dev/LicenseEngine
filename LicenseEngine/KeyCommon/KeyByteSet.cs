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

        public int KeyByteNo { get; private set; }
        public byte KeyByteA { get; private set; }
        public byte KeyByteB { get; private set; }
        public byte KeyByteC { get; private set; }
    }
}
