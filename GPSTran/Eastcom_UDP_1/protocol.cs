using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Common;

/*
 * 此协议为交换与解析数据包协议
 * 该协议加入转发实现对解析发送数据包功能
 * 以PDT格式转发给解析
 */
namespace Eastcom_UDP_1
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PDT
    {
        public Header head;
        public SDU sdu;
    }

    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct Header
    {
        /// <summary>
        /// 字节序
        /// </summary>
        public Int32 Issi;
        public Byte Identitier;

        /// <summary>
        /// GPS数据的长度，单位为比特，值为：SDU信息单元的比特数+8
        /// 长度为2字节，采用网络字节序（大端模式）
        /// 是否采用网络字节续，C++ C#通信不需要网络字节序
        /// </summary>
        public Int16 Len;

        public void SetValue(TBody body)
        {
            Issi = Convert.ToInt32(body.GetIssi());
            Identitier = Convert.ToByte(36);
            Len = Convert.ToInt16(96);
        }
    }

    /* PDT
     * 信息单元	    长度（单位：位）	说明
        NMEA TYPE	    1	            NMEA数据包的类型
     *                      值	                    说明
                            0	                    短NMEA格式
                            1	                    长NMEA格式，暂不启用
        NMEA LENGTH	    7	            NMEA CONTENT的长度
        NMEA CONTENT	可变	        GPS数据内容
       
     * 信息单元	    长度（单位：位）	    说明
        C	        1	                0	数据不加密
		                                1	数据加密
        NS	        1	                0	南纬
		                                1	北纬
        EW	        1	                0	西经
		                                1	东经
        Q	        1	                0	GPS质量指示－不固定
		                                1   GPS质量指示－固定
        SPEED	    7	                0～126	速度节
        NDEG	    7	                0～89	纬度
        NMINmm	    6	                0～59	纬分的整数部分
        NMINF	    14	                0～9999	纬分的小数部分
        EDEG	    8	                0～179	经度
        EMINmm	    6	                0～59	经分的整数部分
        EMINF	    14	                0～9999	经分的小数部分
        SPARE1	    6	                02	保留
        SPARE2	    8	                02	保留
    */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SDU
    {
        ///NMEA TYPE+NMEA LENGTH
        public Byte TypeLen;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Byte[] SDUContent;

        public void SetValue(TBody body)
        {
            
            //TypeLen = Convert.ToByte(TypeLen | 0x80);
            TypeLen = Convert.ToByte(TypeLen | 0x50);
            var array = new BitArray(80,false);
            array.Set(0, false);
            array.Set(1, body.lat >= 0);
            array.Set(2, body.lon >= 0);
            array.Set(3, true);
            ///Speed
            var speed=Convert.ToString(body.speed, 2).PadLeft(7,'0');
            for (var i = 0; i < speed.Length; i++)
            {
                array.Set(i + 4, speed[i]!= 48);
            }

            ///Lat
            GetByteFromLonLat(body.lat, ref array, 11, 27, 7);
            ///Lon
            GetByteFromLonLat(body.lon, ref array, 38, 28, 8);
            SDUContent = Enumerable.Repeat<Byte>(0, 10).ToArray();
            array.CopyTo(SDUContent, 0);
            array = null;
        }

        public void GetByteFromLonLat(Double lonlat,ref BitArray array,params int[] ints)
        {
            var degree =  Math.Floor(lonlat);
            var fraction = lonlat - degree;
            fraction =fraction * 60;
            var fracint = Math.Floor(fraction);
            var fraction2= Convert.ToInt32((fraction-fracint)*10000);
            var sb = new StringBuilder(ints[1]);
            sb.Append(Convert.ToString(Convert.ToInt32(degree), 2).PadLeft(ints[2], '0'));
            sb.Append(Convert.ToString(Convert.ToInt32(fracint), 2).PadLeft(6, '0'));
            sb.Append(Convert.ToString(fraction2, 2).PadLeft(14, '0'));
            for (var i = 0; i < sb.Length; i++)
            {
                array.Set(ints[0] + i, sb[i]!=48);
            }
            sb.Clear();
        }
    }
}
