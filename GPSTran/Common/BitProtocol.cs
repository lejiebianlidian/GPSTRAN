namespace Common
{
    public class BitProtocol
    {
        private byte[] BitReverseTable =
        {
            	0x00,0x08,0x04,0x0C,
                0x02,0x0A,0x06,0x0E,
                0x01,0x09,0x05,0x0D,
                0x03,0x0B,0x07,0x0F 
        };

        public void ReverseBits(ref byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = ReverseBits(bytes[i]);
            }
        }

        private byte ReverseBits(byte data)
        {
            byte ln = BitReverseTable[data & 0x0F];
            byte hn = BitReverseTable[(data >> 4) & 0x0F];
            return (byte)((ln << 4) | hn);
        }
    }
}
