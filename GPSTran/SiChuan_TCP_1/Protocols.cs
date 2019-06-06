using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Common;

namespace SiChuan_TCP_1
{
    /*******************************
     *四川广安项目我方终端的定位信息需要转发给西安航天华讯的“北斗平台”
     * 联系人： 广安 GIS 柳锋 18691895313
     * 1.必须先做登录操作只发送一次
     * 2.登录成功后发送数据包
    *******************************/

    /// <summary>
    /// 登录包体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SCLogin
    {
        public ushort Header;  //标志头  固定值=0xAAAA /43690
        public ushort CmdFlag;  //命令字  固定值=0xBBBB /48059
        public ushort Version; //版本号 固定值=0x0200 /512
        public int Length;  //包体大小 固定值=50 字节序
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] Login;

        public void SetValue()
        {
            Header = 0xAAAA;
            CmdFlag = 0xBBBB;
            Version = 0x0200;
            Length = IPAddress.HostToNetworkOrder(50);
            Login = Enumerable.Repeat<Byte>(0, 50).ToArray();
        }
    }


    public struct SC
    {
        public SCHead header;
        public SCBody body;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SCHead
    {
        public ushort Header;  //标志头  固定值=0xAAAA /43690
        public ushort CmdFlag;  //命令字  固定值=0xCCCC /52428
        public ushort Version; //版本号 固定值=0x0200 /512
        public int Length;  //包体大小 固定值=51 字节序

        public void SetValue()
        {
            Header = 0xAAAA;
            CmdFlag = 0xCCCC;
            Version = 0x0200;
            Length = IPAddress.HostToNetworkOrder(51);
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SCBody
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public Char[] Issi;
        public double Lon;
        public double Lat;
        public ushort Speed;
        public ushort Dir;
        public ushort Height;
        public ushort Precision; //精度 无此值固定=0
        public ushort Year;
        public byte Month;
        public byte Day;
        public byte Hour;
        public byte Minute;
        public byte Second;

        public void SetValue(TBody body)
        {
            var dtime = DateTime.FromOADate(body.time);
            Issi = Enumerable.Repeat('\0', 20).ToArray();
            var sourcearray = body.GetIssi().ToArray();
            Array.Copy(sourcearray, Issi, sourcearray.Length);
            Lon = body.lon;
            Lat = body.lat;
            Speed = (ushort)body.speed;
            Dir = (ushort)body.dir;
            Height = (ushort)body.height;
            Precision = 0;
            Year = Convert.ToUInt16(dtime.Year);
            Month = Convert.ToByte(dtime.Month);
            Day = Convert.ToByte(dtime.Day);
            Hour = Convert.ToByte(dtime.Hour);
            Minute = Convert.ToByte(dtime.Minute);
            Second = Convert.ToByte(dtime.Second);
        }

        //public void Convertbytes(ref Byte[] bytes)
        //{
        //    Converts(ref bytes, 30, 37, Lon);
        //    Converts(ref bytes, 38, 45, Lat);
        //}

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
