using System.Runtime.InteropServices;

namespace LiaoNing_TCP_1
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LiaoNinGps
    {
        public ushort shuzizengtou; //数据帧头 2  固定值 = 0x2929
        public byte cmdFlag;  //命令字 1  固定值=0x80;
       // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort dataLength; // 数据长度 2  固定值 = 0x2800
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] issiNumber; //终端序号	4
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] year;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] month;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] day;
         [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] hour;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] minute;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] second;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] lat;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] lon;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] speed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] dir;
        public byte loction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] licheng; //里程
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] cheliangStatues; //车辆状态
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] v1v2;
        public byte v3;
        public byte v4;
        public byte v5;
        public byte v6;
        public byte v7;
        public byte v8;
        public byte JiaoYan ; //校验	1
        public byte shujuWeiZhen; //数据帧尾	1

    }
 
   
  
}

