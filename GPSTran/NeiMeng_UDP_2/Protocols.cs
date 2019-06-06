using System;
using System.Runtime.InteropServices;
using Common;
using System.Net;
namespace NeiMeng_UDP_2
{
    #region 结构体
    public struct Neimeng
    {
        public Head head;
        public Body body;
    }

    public struct Head
    {
        public Byte MsgType; //报文类型  0无线定位
        public Byte Version; //报文版本  初始化版本为1
        public Byte Cmd;     //报文命令字 命令字 50
        public Char DeviceType; //M  手持GPS

        public void SetValue()
        {
            this.MsgType = 0;
            this.Version = 1;
            this.Cmd = 50;
            this.DeviceType = 'M';
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Body
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public Byte[] Issi;

        public Double Lon;

        public Double Lat;

        public Double Hight;

        public Double Speed;

        public Double Dir;

        public short Year;

        public Byte Month;

        public Byte Day;

        public Byte Hour;

        public Byte Minute;

        public Byte Second;

        /// <summary>
        /// 自定义数据长度
        /// </summary>
        public short Cdatalength;

        /// <summary>
        /// 自定义数据
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public Byte[] Cdata;

        public void SetValue(TBody gpsbody)
        {
            var tmpbytes = gpsbody.GetASCIIBytes(gpsbody.GetIssi());
            this.Issi = new Byte[33];
            for (var i = 0; i < 33; i++)
            {
                if (i < tmpbytes.Length)
                {
                    this.Issi[i] = tmpbytes[i];
                }
                else
                {
                    this.Issi[i] = 0;
                }
            }
            ///取小数点后5位
            this.Lon = Convert.ToDouble(String.Format("{0:N5}", gpsbody.lon));
            this.Lat = Convert.ToDouble(String.Format("{0:N5}",gpsbody.lat));
            this.Hight = gpsbody.height;
            this.Speed = gpsbody.speed;
            this.Dir = gpsbody.dir;
            this.Year = IPAddress.HostToNetworkOrder(Convert.ToInt16(DateTime.Now.Year));
            this.Month = Convert.ToByte(DateTime.Now.Month);
            this.Day = Convert.ToByte(DateTime.Now.Day);
            this.Hour = Convert.ToByte(DateTime.Now.Hour);
            this.Minute = Convert.ToByte(DateTime.Now.Minute);
            this.Second = Convert.ToByte(DateTime.Now.Second);
            this.Cdatalength = IPAddress.HostToNetworkOrder(Convert.ToInt16(12));
            this.Cdata = gpsbody.GetASCIIBytes("hello world!");
        }

        public void Convertbytes(ref Byte[] bytes)
        {
            Converts(ref bytes, 37, 44, this.Lon);
            Converts(ref bytes, 45, 52, this.Lat);
            Converts(ref bytes, 53, 60, this.Hight);
            Converts(ref bytes, 61, 68, this.Speed);
            Converts(ref bytes, 69, 76, this.Dir);
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
    #endregion
}
