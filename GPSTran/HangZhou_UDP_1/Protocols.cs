using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Common;

/*
 * 
 * 杭州武警总队协议
 * 对接方杭州志远
 * 
 * 
 */
namespace HangZhou_UDP_1
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Hzwj
    {
        public Head head;
        public Body body;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Head
    {
        public Byte H1;
        public Byte H2;
        public Byte H3;
        public Byte Flag1;
        public Byte Flag2;
        public Int32 Length;

        public void SetValue()
        {
            H1 = 0x2A;
            H2 = 0x5A;
            H3 = 0x59;
            Flag1 = 0xAA;
            Flag2 = 0xEE;
            Length = IPAddress.HostToNetworkOrder(59);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Body
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public Byte[] Issi;
        public Byte CC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public Byte[] Time;
        public Double Lon;
        public Double Lat;
        public Double Speed;
        public Int16 Dir;

        public void SetValue(TBody body)
        {
            Issi = Enumerable.Repeat<Byte>(0x25, 20).ToArray();
            var bodyssi = body.GetASCIIBytes(body.GetIssi());
            Array.Copy(bodyssi, Issi, bodyssi.Length);
            //var bodyssi = body.GetASCIIBytes(body.GetIssi());
            //Issi = new Byte[20];
            //for (var i = 0; i < bodyssi.Length; i++)
            //{
            //    Issi[i] = bodyssi[i];
            //}
            //for (var i = bodyssi.Length; i <20 ; i++)
            //{
            //    Issi[i] = 0x25;
            //}
            CC = 0x26;
            Time = body.GetASCIIBytes(new DateTime(1899, 12, 30).AddDays(body.time).ToString("yyMMddHHmmss"));
            Lon = body.lon;
            Lat = body.lat;
            Speed =Convert.ToDouble(body.speed);
            Dir = IPAddress.HostToNetworkOrder(body.dir);
        }

        public void Convertbytes(ref Byte[] bytes)
        {
            Converts(ref bytes, 42, 49, this.Lon);
            Converts(ref bytes, 50, 57, this.Lat);
            Converts(ref bytes, 58, 65, this.Speed);
        }

        private void Converts(ref Byte[] bytes, int start, int end, Double value)
        {
            var tmpbytes = BitConverter.GetBytes(value);
            Array.Reverse(tmpbytes);
            for (var i = start; i <= end; i++)
            {
                bytes[i] = tmpbytes[i - start];
            }
        }
    }
}
