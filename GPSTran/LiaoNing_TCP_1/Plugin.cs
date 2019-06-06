using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;
using Common;

namespace LiaoNing_TCP_1
{
    public class Plugin : IPlugin
    {
        private ConcurrentQueue<byte[]> dataQueue = new ConcurrentQueue<byte[]>();
        //private int maxDataCount = 10000;
        private int isSlow;
        //private UdpClient udpClient;
        private readonly PluginType pType = PluginType.RemoteIP;
        private PluginModel pModel;
        private readonly Object lckPModel = new Object();
        private int status;
        private long statistics;
        private bool enabled;
        private bool debugEnabled;
        private Protocol liaoNinpro;
        //private SQLHelper sqlHelper;
        private readonly Object lckHelper = new Object();
        private Thread dealDataThread;
        //private Protocol Pro;
        private int TranLimit;
        private bool isStop;
        private LiaoNinGps liaoninggps;
        private TcpClient Tcp;
        private NetworkStream nws;

        private DateTime now;

        public bool IsStop()
        {
            return isStop;
        }

        public Plugin()
        {

        }
        public Plugin(PluginModel pModel, bool threadStart)
        {
            if (pModel == null)
            {
                throw new NullReferenceException("null reference");

            }
            liaoninggps = new LiaoNinGps();
            liaoninggps.shuzizengtou = 0x2929;
            liaoninggps.cmdFlag = 0x80;
            liaoninggps.dataLength = 0x2800;
            liaoninggps.shujuWeiZhen = 0x0D;
            TranLimit = 1000;
            this.pModel = pModel;
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = pModel.Enabled;
            debugEnabled = false;
            liaoNinpro = new Protocol();

            if (threadStart)
            {
                dealDataThread = new Thread(Run);
                dealDataThread.Start();
            }
        }

        private void Run()
        {
            CreateSocket();
            //建立连接
            byte[] data;
            byte[] temp;
            GPSData tempData;
            int valid = 0;
            while (true)
            {
                while (dataQueue.Count > 0 && status > 0)
                {
                    if (dataQueue.TryDequeue(out data) == false)
                    {
                        if (!enabled)
                        {
                            isStop = true;
                            return;
                        }
                        continue;
                    }
                    valid = 0;
                    foreach (var i in pModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(liaoNinpro.GetIssi(data)))
                                {
                                    valid = 1;
                                    break;
                                }
                            }
                        }
                    }
                    if (valid == 0)
                    {
                        continue;
                    }

                    //can access to the remote IP
                    temp = liaoNinpro.ByteClone(data);

                    //temp = neimengPro.GPSDataClone(data);
                    //add lon,lat offset,add protocol type confirm
                    //if (pModel.Protocol.Equals("PGIS"))
                    //{
                    temp = liaoNinpro.ModifyLaLon(temp, pModel.LonOffset, pModel.LatOffset);
                    //}
                    //解包temp 修改ISSI号，修改包文件头，重新封包
                    tempData = liaoNinpro.ToGPSData(temp);
                    temp = PacketForNeiMeng(tempData);

                    //------------------------------------------
                    //if (pModel.NetProtocol.Equals("UDP"))
                    //{
                    try
                    {
                        //udpClient.Send(temp, temp.Length, pModel.EndPoint);
                        //Tcp.(temp, temp.Length, SocketFlags.None);
                        var ar = nws.BeginWrite(temp, 0, temp.Length, null, null);
                        if (ar.AsyncWaitHandle.WaitOne())
                        {
                            statistics += 1;
                            status = 1;
                        }
                        nws.EndWrite(ar);
                        if (Resource.GPSDebugEnabled)
                        {
                            Logger.Info(String.Format("实例：{0} 号码:{1}", pModel.Name, liaoninggps.issiNumber));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("error occur when send GPS data with TCP protocol,error message:" + ex);
                        status = 0;
                    }
                    finally
                    {
                        if (status == 0 && enabled)
                        {
                            DisposeSocket();
                            CreateSocket();
                        }
                    }
                }

                if (!enabled)
                {
                    if (dataQueue.Count > 0)
                    {
                        continue;
                    }
                    Logger.Info("Task:IPInstance instance(" + pModel.Name + ") exit safely");
                    isStop = true;
                    DisposeSocket();
                    return;
                }

                //while (enabled && dataQueue.Count == 0)
                //{
                //    Thread.Sleep(10);
                //}
            }
        }

        private void DisposeSocket()
        {
            status = 0;
            Logger.Error("TCP Fail to connect server");
            if (nws != null)
            {
                nws.Close();
            }

            if (Tcp != null)
            {
                Tcp.Close();
                Tcp = null;
            }
        }


        void CreateSocket()
        {
            new Action(() =>
            {
                //尝试连接
                Logger.Info("TCP 发送请求.....");
                Tcp = new TcpClient();
                //Tcp.Connect(pModel.EndPoint);
                var ar = Tcp.BeginConnect(pModel.Ip, pModel.Port, null, Tcp);
                if (ar.AsyncWaitHandle.WaitOne())
                {
                    Tcp = ar.AsyncState as TcpClient;
                    try
                    {
                        Tcp.EndConnect(ar);
                        nws = Tcp.GetStream();
                        status = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Tcp Reconnect Error:" + e);
                        status = 0;
                    }
                    finally
                    {
                        if (status == 0 && enabled)
                        {
                            DisposeSocket();
                            Thread.Sleep(60000 * 5);
                            CreateSocket();
                        }
                    }
                }
            }).BeginInvoke(null, null);
        }

        //经纬度转换函数
        public static byte[] ConvertDigitalToDegreesForLat(double digitalDegree)
        {
            byte[] lat = new Byte[4]; //经度
            const double num = 60;
            int degree = (int)digitalDegree;
            string tmp = Convert.ToDouble((digitalDegree - degree) * num).ToString("0.000");
            int tempInt = Convert.ToInt32(Convert.ToDouble(tmp) * 1000);
            string latStr = degree + tempInt.ToString();

            if (latStr.Length < 8)
            {
                for (int i = latStr.Length; i < 8; i++)
                {
                    latStr = 0 + latStr;
                }
            }
            latStr = latStr.Replace(" ", "");
            if ((latStr.Length % 2) != 0)
                latStr += " ";
            byte[] returnBytes = new byte[latStr.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(latStr.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        public static byte[] ConvertDigitalToDegreesForLon(double digitalDegree)
        {
            byte[] lon = new Byte[4]; //纬度
            const double num = 60;
            int degree = (int)digitalDegree;
            string tmp = Convert.ToDouble((digitalDegree - degree) * num).ToString("0.000");
            //int j = 0;
            string lonStr = degree + tmp;

            if (lonStr.Length < 8)
            {
                for (int i = lonStr.Length; i < 8; i++)
                {
                    lonStr = 0 + lonStr;
                }
            }
            lonStr = lonStr.Replace(" ", "");
            if ((lonStr.Length % 2) != 0)
                lonStr += " ";
            byte[] returnBytes = new byte[lonStr.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(lonStr.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        #region old code
        // 转换整形数据网络次序的字节数组
        public static byte[] IntToBytes(ushort i)
        {
            byte[] t = BitConverter.GetBytes(i);
            byte b = t[0];
            t[0] = t[1];
            t[1] = b;
            return t;
        }
        public static byte[] IntToBytesForLength(uint i)
        {
            byte[] t = BitConverter.GetBytes(i);
            byte b = t[0];
            t[0] = t[3];
            t[3] = b;
            b = t[1];
            t[1] = t[2];
            t[2] = b;
            return (t);
        }

        public static byte[] ConvertToHexForDir(byte dir)
        {
            string hexString = Convert.ToInt32(dir).ToString();
            if (hexString.Length < 4)
            {
                for (int i = hexString.Length; i < 4; i++)
                {
                    hexString = 0 + hexString;
                }
            }
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {

                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        #endregion

        public static byte[] ConvertToHexForSpeed(byte speed)
        {
            string hexString = Convert.ToInt32(speed).ToString();
            if (hexString.Length < 4)
            {
                for (int i = hexString.Length; i < 4; i++)
                {
                    hexString = 0 + hexString;
                }
            }
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {

                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        public byte[] strId = null;
        private byte[] PacketForNeiMeng(GPSData temp)
        {
            string date = temp.time;
            // liaoninggps.issiNumber = {0x09,0x01,0xC,0x3b}; //strToToHexByte(temp.id);          
            liaoninggps.issiNumber = strToToHexByteForID(temp.id);
            liaoninggps.lat = ConvertDigitalToDegreesForLat(temp.lat);
            liaoninggps.lon = ConvertDigitalToDegreesForLat(temp.lon);
            liaoninggps.speed = new Byte[2];
            liaoninggps.speed = ConvertToHexForSpeed(temp.speed);
            liaoninggps.dir = new Byte[2];
            liaoninggps.dir = ConvertToHexForSpeed(temp.dir);
            liaoninggps.loction = 0;
            liaoninggps.v1v2 = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                liaoninggps.v1v2[i] = 0;
            }
            liaoninggps.cheliangStatues = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                liaoninggps.cheliangStatues[i] = 0;
            }
            liaoninggps.licheng = new byte[3];
            for (int i = 0; i < 3; i++)
            {
                liaoninggps.licheng[i] = 0;
            }


            now = DateTime.Now;

            liaoninggps.year = strToToBCD((Convert.ToInt32(now.Year) - 2000).ToString());// Convert.ToByte((Convert.ToInt32(now.Year) - 2000).ToString("X"), 16);

            liaoninggps.month = strToToBCD(now.Month.ToString());// Convert.ToByte(now.Month.ToString("X"), 16);

            liaoninggps.day = strToToBCD(now.Day.ToString());//Convert.ToByte(now.Day.ToString("X"), 16);

            liaoninggps.hour = strToToBCD(now.Hour.ToString());// Convert.ToByte(now.Hour.ToString("X"), 16);

            liaoninggps.minute = strToToBCD(now.Minute.ToString());// Convert.ToByte(now.Minute.ToString("X"), 16);

            liaoninggps.second = strToToBCD(now.Second.ToString());// Convert.ToByte(now.Second.ToString("X"), 16);          

            var result_temp = liaoNinpro.GetByte(liaoninggps);
            liaoninggps.JiaoYan = getChecksum(result_temp);
            var result = liaoNinpro.GetByte(liaoninggps);
            return result;
        }



        private static byte[] strToToBCD(string hexString)
        {
            if (hexString.Length < 2)
            {
                for (int i = hexString.Length; i < 2; i++)
                {
                    hexString = 0 + hexString;
                }
            }
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                //int temp = Convert.ToInt32(hexString.Substring(i * 2, 2));
                string hex = "0x" + hexString;
                returnBytes[i] = Convert.ToByte(hex, 16);
            }
            return returnBytes;
        }

        private static byte[] strToToHexByteForID(string hexString)
        {
            if (hexString.Length < 8)
            {
                for (int i = hexString.Length; i < 8; i++)
                {
                    hexString = 0 + hexString;
                }
            }
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                if (i == 1 || i == 2)
                {

                    int temp = Convert.ToInt32(hexString.Substring(i * 2, 2)) + 128;
                    string tempString = temp.ToString("X");
                    returnBytes[i] = Convert.ToByte(tempString, 16);

                }
                else
                {
                    int temp = Convert.ToInt32(hexString.Substring(i * 2, 2));
                    returnBytes[i] = Convert.ToByte(temp.ToString("X"), 16);
                }
            return returnBytes;
        }

        private static byte[] strToToHexByte(string hexString)
        {
            if (hexString.Length < 8)
            {
                for (int i = hexString.Length; i < 8; i++)
                {
                    hexString = 0 + hexString;
                }
            }
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                int temp = Convert.ToInt32(hexString.Substring(i * 2, 2));
                returnBytes[i] = Convert.ToByte(temp.ToString("X"), 16);
            }
            return returnBytes;
        }

        public static byte getChecksum(byte[] data)
        {
            return getChecksum(data, 0, data.Length - 2);
        }
        /**
        * 对传入的数组，从第offset位开始计算校验码，直至指定的长度(length)
        * @param data
        * @param offset 从第offset位开始计算校验码
        * @param length 需要计算的数据总长度
        * @return
        */
        public static byte getChecksum(byte[] data, int offset, int length)
        {
            if (data.Length < (offset + length))
            {
                //throw new IllegalArgumentException("# Index out of boundary");
            }

            // 如果只有1个元素，直接返回该元素的值
            if (data.Length == 1)
            {
                return data[0];
            }

            byte checksum = (byte)(data[offset] ^ data[offset + 1]);
            for (int i = 2; i < length; i++)
            {
                checksum ^= data[i + offset];
            }

            return checksum;
        }


        public PluginModel GetPluginModel()
        {
            return this.pModel;
        }

        public int GetStatus()
        {
            if (status == 0)
            {
                return status;
            }

            if (isSlow == 1)
            {
                return 2;
            }

            return status;
        }

        public long GetStatistics()
        {
            return statistics;
        }

        public Common.PluginType GetPluginType()
        {
            return pType;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;

        }

        public void SetDebugEnabled(bool enabled)
        {
            debugEnabled = enabled;
        }


        public void Execute(byte[] data)
        {
            if (TranLimit >= Resource.MaxTranLimit)
            {
                isSlow = 1;
            }
            else
            {
                dataQueue.Enqueue(data);
                isSlow = 0;

            }

        }

        public void SetPluginModel(PluginModel pModel)
        {
            lock (lckPModel)
            {
                this.pModel = pModel;
                enabled = pModel.Enabled;

                //    //lock (lckHelper)
                //    //{
                //    //    string s = "Data Source=" + pModel.DesIP + "," + pModel.DesPort + "\\" + pModel.DesInstance + ";Initial Catalog=" + pModel.DesCatalog + ";uid=" + pModel.DesUser + ";pwd=" + pModel.DesPwd;
                //    //    sqlHelper = new SQLHelper(s);
                //    //}

            }
        }
    }
}
