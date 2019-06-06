using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Common;

namespace Lte_UDP_1
{
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct LTE
    {
        public Byte Head;

        public int Length;

        public Byte Ver;

        public short Area;

        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 8)]
        public Byte[] UserId;

        public Byte Msgtype;

        public Byte MsgId;

        public Body body;

        public Byte Check;

        public void InitValue()
        {
            Head = 0xFD;
            Length = 42;
            Ver = 0x02;
            Msgtype = 0x01;
            MsgId = 0x01;
            Check = 0x00;
        }

        public void SetValue(String issi)
        {
            UserId = Enumerable.Repeat<Byte>(0xaa, 8).ToArray();
            var len = issi.Length;
            if (len % 2 == 0)
            {
                for (var i = 0; i < len / 2; i++)
                {
                    UserId[i] = Convert.ToByte(issi.Substring(i*2, 2),16);
                }
            }
            else
            {
                for (var i = 0; i <len / 2; i++)
                {
                    if (i==len/2-1)
                    {
                        UserId[i] = Convert.ToByte(Convert.ToByte(issi.Substring(i * 2, 1)) << 4 | UserId[i] >> 4);
                    }
                    else
                    {
                        UserId[i] = Convert.ToByte(issi.Substring(i * 2, 2),16);
                    }
                }
            }
        }

        public void CheckSum(Byte[] bytes)
        {
            Check = 0;
            for (int index = 1; index < bytes.Length-1; index++)
            {
                Check = (Byte)(Check + bytes[index]);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct Body
    {
        public Byte Year;

        public Byte Month;

        public Byte Day;

        public Byte Hour;

        public Byte Minute;

        public Byte Second;

        public Byte CoordinateType;

        public Byte Lon_Lat_Mark;

        public Int32 Lon;

        public Int32 Lat;

        public short Speed;

        public short Dir;

        public short Height;

        public Byte Status;

        public void InitValue()
        {
            CoordinateType = 0x03;
        }

        public void SetValue(TBody body)
        {
            var dt = DateTime.FromOADate(body.time);
            Year = Convert.ToByte((dt.Year - 2000).ToString("D2"),10);
            Month = Convert.ToByte(dt.Month.ToString("D2"),10);
            Day = Convert.ToByte(dt.Day.ToString("D2"),10);
            Hour = Convert.ToByte(dt.Hour.ToString("D2"), 10);
            Minute = Convert.ToByte(dt.Minute.ToString("D2"), 10);
            Second = Convert.ToByte(dt.Second.ToString("D2"), 10);
            Speed = body.speed;
            Dir = body.height;
            Status = 0x02;
            Lon_Lat_Mark = 0x00;
            if (body.lat>0)
            {
                Lon_Lat_Mark |= 0x01;
            }
            if (body.lon>0)
            {
                Lon_Lat_Mark |= 0x02;
            }
            Lon = Convert.ToInt32(body.lon*1000*10000);
            Lat = Convert.ToInt32(body.lat*1000*10000);
        }
    }
}
