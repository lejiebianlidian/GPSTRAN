using System;
using System.Runtime.InteropServices;
using System.Text;
using Common;

namespace LiaoNing_UDP_1
{
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct Lnyk
    {
        public Head head;
        public Body body;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Head
    {
        /// <summary>
        /// 起始符 $
        /// </summary>
        public Byte Flag;

        /// <summary>
        /// 生产厂商 JJ
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 2)]
        public Byte[] Manufacturer;

        /// <summary>
        /// 数据类型 0x44
        /// </summary>
        public Byte DataType;

        public void InitValue()
        {
            Flag = 0024;
            Manufacturer =Encoding.ASCII.GetBytes("JJ");
            DataType = 0x44;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Body
    {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 4)]
        public Byte[] BCD;

        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 6)]
        public Byte[] Dtime;

        public Int32 Lon;

        public Int32 Lat;

        public Byte Speed;

        public short Dir;

        /// <summary>
        /// 车速传感器
        /// </summary>
        public Byte Sensor;

        /// <summary>
        /// GPS状态数据
        /// </summary>
        public Byte GPSType;

        /// <summary>
        /// 车辆状态
        /// </summary>
        public Int32 CarState;

        /// <summary>
        /// 报警状态
        /// </summary>
        public Byte AlertState;

        /// <summary>
        /// 驾驶员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] 
        public Byte[] DriverCode;

        public Byte DefaultCode;

        public void SetValue(TBody data)
        {
            BCD = BcdtoId(data.id);
            Dtime = BcdToTime(data.time);
            Lon = Convert.ToInt32(data.lon*1000000);
            Lat = Convert.ToInt32(data.lat*1000000);
            Speed = data.speed;
            Dir = data.dir;
            Sensor = 0;
            GPSType = 0;
            CarState = 0;
            AlertState = 0;
            DriverCode = new byte[]{0,0,0};
            DefaultCode = 0x55;
        }

        private Byte[] BcdtoId(Byte[] id)
        {
            var b = new byte[4];
            for (var i = 0; i <4; i++)
            {
                b[i] = Convert.ToByte(id[i+2].ToString("D2"));
            }
            return b;
        }

        private Byte[] BcdToTime(double d)
        {
            var t = DateTime.FromOADate(d);
            var bt = new byte[6];
            bt[0] = Convert.ToByte(t.Year.ToString().Substring(2));
            bt[1] = Convert.ToByte(t.Month.ToString("D2"));
            bt[2] = Convert.ToByte(t.Day.ToString("D2"));
            bt[3] = Convert.ToByte(t.Hour.ToString("D2"));
            bt[4] = Convert.ToByte(t.Minute.ToString("D2"));
            bt[5] = Convert.ToByte(t.Second.ToString("D2"));
            return bt;
        }
    }
}
