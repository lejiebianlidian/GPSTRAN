using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Common;

/*
 * 武警浙江总队
 * 28所对接
 */
namespace HangZhou_UDP_2
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Head
    {
        public Int32 Serials;

        public Byte EastCode;

        public Byte Type;

        public Byte PackageCount;

        public Byte Reserve;

        public Int32 Seconds;

        public Int32 Length;

        public Int32 CheckSum;

        public void InitValue()
        {
            EastCode = 0xA1;
            Type = 0x02;
            PackageCount = 1;
            Length = 72;
            CheckSum = 0;
        }

        public void UpSerial()
        {
            ++Serials;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Body
    {
        public Byte Ctype;

        public Byte Reserve;

        public short Length;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] EastCode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public Byte[] ID;

        public Int32 Seconds;

        public Byte LocateType;

        public Byte Reserve1;

        public Double Lon;

        public Double Lat;

        public short Heigh;

        public short Dir;

        public ushort Speed;

        public void InitValue()
        {
            Ctype = 0x01;
            Length = 68;
            EastCode = Encoding.ASCII.GetBytes("A1");
            LocateType = 0x01;
        }

        public void SetValue(TBody body)
        {
            var issi = body.GetASCIIBytes(body.GetIssi());
            ID = Enumerable.Repeat<Byte>(0x00, 38).ToArray();
            Array.Copy(issi, ID, issi.Length);
            Lon = body.lon;
            Lat = body.lat;
            Heigh = body.height;
            Dir = body.dir;
            Speed = body.speed;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Wj28
    {
        public Head header;
        public Body body;

        public void InitValue()
        {
            header.InitValue();
            body.InitValue();
        }

        public void SetValue(TBody cbody)
        {
            header.UpSerial();
            body.SetValue(cbody);
            body.Seconds = header.Seconds = Convert.ToInt32(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public void SetCheckSum(Byte b)
        {
            header.CheckSum=b;
        }
    }
}
