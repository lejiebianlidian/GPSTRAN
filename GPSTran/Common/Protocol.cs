﻿using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Common
{
   public class Protocol:BitProtocol
    {
        //get issi from byte[]
        public  string GetIssi(byte[] buf,int begin,int length) 
        {
            byte[] buffer = buf;
            StringBuilder tmp = new StringBuilder();
            int tempint = 0;
            for (int i = begin; i < begin+length; i++)
            {
                tempint = Convert.ToInt32(buffer[i].ToString(), 16);
                tmp.Append(tempint.ToString("X2")); // <-- 不?足?2位?的?前?面?补10
            }
            string issi = tmp.ToString();
            return Convert.ToInt32(issi).ToString();
        
        }
        public string GetIssi(byte[] buf) 
        {
            byte[] buffer = buf;
            StringBuilder tmp = new StringBuilder();
            int tempint = 0;
            for (int i = 4; i < 10; i++)
            {
                tempint = Convert.ToInt32(buffer[i].ToString(), 16);
                tmp.Append(tempint.ToString("X2")); // <-- 不?足?2位?的?前?面?补10
            }
            string issi = tmp.ToString();
            return Convert.ToUInt32(issi).ToString();
        
        }
        //get issi from TGPS object
        public  string GetIssi(TGPS gps)
        {
            byte[] buffer = gps.TBody.id;
            StringBuilder tmp = new StringBuilder();
            int tempint = 0;
            for (int i = 0; i < 6; i++)
            {
                tempint = Convert.ToInt32(buffer[i].ToString(), 16);
                tmp.Append(tempint.ToString("X2")); // <-- 不?足?2位?的?前?面?补10
            }
            string issi = tmp.ToString();
            return Convert.ToInt32(issi).ToString();

        }
        //get issi from TBody object
        public  string GetIssi(TBody body) 
        {
            byte[] buffer = body.id;
            StringBuilder tmp = new StringBuilder();
            int tempint = 0;
            for (int i = 0; i < 6; i++)
            {
                tempint = Convert.ToInt32(buffer[i].ToString(), 16);
                tmp.Append(tempint.ToString("X2")); // <-- 不?足?2位?的?前?面?补10
            }
            string issi = tmp.ToString();
            return Convert.ToUInt32(issi).ToString();
        
        }
        //get issi from GPSData object
        public  string GetIssi(GPSData data)
        {
            return data.id;
        }
        //convert to type of GPSData from TBody
        public GPSData ToGPSData(TBody body)
        {
            GPSData data = new GPSData();
            data.id = GetIssi(body);
            data.lat = body.lat;
            data.lon = body.lon;
            data.speed = body.speed;
            data.dir = body.dir;
            data.state = body.state;
            data.nMsgtype = body.nMsgtype;
            data.time = DateTime.FromOADate(body.time).ToString("yyyy-MM-dd HH:mm:ss.fff");
            data.height = body.height;

            return data;

        }

        public GPSData ToGPSData(TGPS gps) 
        {
            return ToGPSData(gps.TBody);
        }

        public TGPS ToGPS(byte[] buf) 
        {

            Int32 size = Marshal.SizeOf(typeof(TGPS));
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buf, 0, buffer, size);
                return (TGPS)Marshal.PtrToStructure(buffer, typeof(TGPS));
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
                //GC.Collect();
            }
        
        }
        public GPSData ToGPSData(byte[] bytes) 
        {
            try
            {
                TGPS gps = ToGPS(bytes);
                return ToGPSData(gps);
            }
            catch (Exception ex) 
            {
                throw ex;
            
            }
        }

        public TBody ToTBody(byte[] buf) 
        {
            return ToGPS(buf).TBody;
        
        }
        

        public byte[] GetByte(TGPS body) 
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(body);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(body, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;

        }

        public byte[] GetByte(Object obj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(obj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(obj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;

        }

       //get longitude
        public double GetLon(byte[] buf) 
        {
            byte[] buffer = buf;
            double lon;

            lon = BitConverter.ToDouble(buffer, 10);
            return lon;

        }
       //get latitude
        public double GetLa(byte[] buf)
        {
            byte[] buffer = buf;
            double la;

            la = BitConverter.ToDouble(buffer, 18);
            return la;

        }
       //modify lontitude or latitude
        public byte[] ModifyLaLon(byte[] buf,double lonOffset,double latOffset)
        {
            byte[] buffer = ByteClone(buf);
            byte[] lonByte;
            byte[] laByte;
            double lon = GetLon(buf);
            double la = GetLa(buf);
            lon = lon + lonOffset;
            la = la + latOffset;
            lonByte = BitConverter.GetBytes(lon);
            laByte = BitConverter.GetBytes(la);

            for (int i = 10; i < 18; i++)
            {
                buffer[i] = lonByte[i - 10];

            }
            for (int i = 18; i < 26; i++)
            {
                buffer[i] = laByte[i - 18];

            }
            return buffer;
        
        }
        public GPSData ModifyLaLon(ref GPSData data, double lonOffset, double latOffset) 
        {
            GPSData temp = data;

            temp.lon = temp.lon + lonOffset;
            temp.lat = temp.lat + latOffset;
            return temp;

        }
       //clone a byte array
        public byte[] ByteClone(byte[] buf)
        {
            byte[] temp = new byte[buf.Length];

            for (int i = 0; i < buf.Length; i++)
            {
                temp[i] = buf[i];

            }
            return temp;
        
        }
       //cloen a GPSData
        public GPSData GPSDataClone(GPSData data) 
        {
            GPSData temp = new GPSData();

            temp.id = data.id;
            temp.lat = data.lat;
            temp.lon = data.lon;
            temp.speed = data.speed;
            temp.dir = data.dir;
            temp.state = data.state;
            temp.nMsgtype = data.nMsgtype;
            temp.time = data.time;
            temp.height = data.height;
            return temp;
        
        }

    }
    //protocol PGIS head definition
    public struct THead
    {
        public ushort wHeader;
        public ushort cmdFlag;
    }
    //protocol PGIS body definition
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TBody
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Byte[] id;
        public double lon;
        public double lat;
        public byte speed;
        public byte dir;
        public int state;
        public short nMsgtype;
        public short height;
        public double time;

        #region 
        /// QJJ 2015-07-09
        /// <summary>
        /// 新增方法获取号码
        /// </summary>
        /// <returns>终端号码</returns>
        public string GetIssi()
        {
            var issi = new StringBuilder();
            foreach (var i in id)
            {
                issi.Append(i.ToString("D2"));
            }
            return issi.ToString().TrimStart('0');
        }

        public void ModifyLonLat(Double lon, Double lat)
        {
            this.lon += lon;
            this.lat += lat;
        }

        public Byte[] GetASCIIBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
            //for (var i=0;i<bytes.Length;i++)
            //{
            //    bytes[i] = Convert.ToByte(string.Format("{0:X}",bytes[i]));
            //}
            //return bytes;
        }

        public String GetDateTime()
        {
            return DateTime.FromOADate(time).ToString();
        }

        public DateTime GetDateTime2()
        {
            return DateTime.FromOADate(time);
        }

        /// <summary>
        /// 网络字节序
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        private void Converts(ref Byte[] bytes, int start, int end, Double value)
        {
            var tmpbytes = BitConverter.GetBytes(value);
            Array.Reverse(tmpbytes);
            for (var i = start; i <= end; i++)
            {
                bytes[i] = tmpbytes[i - start];
            }
        }
        #endregion
    }
    public struct GPSData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public string id;
        public double lon;
        public double lat;
        public byte speed;
        public byte dir;
        public int state;
        public short nMsgtype;
        public short height;
        public string time;
    }
    //protocol PGIS  definition
    public struct TGPS
    {
        public THead Head;
        public TBody TBody;
    }
}
