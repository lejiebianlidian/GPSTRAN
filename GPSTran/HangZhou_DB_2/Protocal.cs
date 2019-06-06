using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Common;

namespace HangZhou_DB_2
{
    //    连接方式	
    //ip：	10.118.128.212
    //端口：	1521
    //实例名：	orcl
    //用户名：	gpsdftx 
    //密码：	gpsdftx 
    //表名：	gpsinfo.gps_info
    /// <summary>
    /// </summary>
    public struct HZ
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public String Gpsid;

        public Double X;

        public Double Y;

        public Double Speed;

        public Int32 State;

        public Int32 Dir;

        public String GpsTime;

        public void SetValue(TBody gps)
        {
            Gpsid = gps.GetIssi().PadLeft(12, '0');
            X = gps.lon;
            Y = gps.lat;
            Dir = Convert.ToInt32(gps.dir);
            State = Convert.ToInt32(gps.state);
            GpsTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }
    }
}