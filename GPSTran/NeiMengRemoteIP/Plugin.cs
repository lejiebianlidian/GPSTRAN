using System;
using System.Linq;
using Common;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;
namespace NeiMengRemoteIP 
{
    public class Plugin : IPlugin
    {
        private ConcurrentQueue<byte[]> dataQueue = new ConcurrentQueue<byte[]>();
        //private int maxDataCount = 10000;
        private int isSlow;
        private UdpClient udpClient;
        private readonly PluginType pType = PluginType.RemoteIP;
        private PluginModel pModel;
        private readonly Object lckPModel = new Object();
        private int status;
        private long statistics;
        private bool enabled;
        private bool debugEnabled;
        private NeiMengProtocol neimengpro;
        private SqlHelper sqlHelper;
        private readonly Object lckHelper = new Object();
        private Thread dealDataThread;
        //private Protocol Pro;
        private NeiMengProtocol neimengPro;
        private int TranLimit;
        private bool isStop;
        private NeiMengGps neimengGPS;
        private NeiMengTBody neimengTBody;
        private NeiMengTHead neimengTHead;
      
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
            byte[] boteHead = new byte[2];
            boteHead[0] = 0x22;
            boteHead[1] = 0x00;        
            neimengPro = new NeiMengProtocol();
            neimengGPS=new NeiMengGps();
            neimengTHead = new NeiMengTHead();
           
            neimengTHead.wHeader = 0xAAAA;
            neimengTHead.cmdFlag = 0xCCCC;
            neimengTHead.versionFlag = 0x2200;
            neimengTHead.bodyLength = System.Net.IPAddress.HostToNetworkOrder(51);  
   
            neimengTBody=new NeiMengTBody();
          
            TranLimit = 1000;
            this.pModel = pModel;
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = pModel.Enabled;
            debugEnabled = false;
            neimengpro = new NeiMengProtocol();
   
            if (threadStart)
            {
                this.dealDataThread = new Thread(new ThreadStart(Run));
                this.dealDataThread.Start();

            }
        }

        private void Run()
        {
            if (udpClient == null)
            {
                try
                {
                    udpClient = new UdpClient();
                }
                catch (Exception ex)
                {
                    Logger.Error("create UdpClient failed in IPManager，error message:" + ex.ToString());
                    return;
                }
            }

            byte[] data;
            byte[] temp;
            GPSData tempData;
            while (true)
            {
                while (dataQueue.Count > 0)
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
                    int valid = 0;
                    foreach (int i in pModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(neimengpro.GetIssi(data)))
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

                    temp = neimengpro.ByteClone(data);
               
                    //temp = neimengPro.GPSDataClone(data);
                    //add lon,lat offset,add protocol type confirm
                    //if (pModel.Protocol.Equals("PGIS"))
                    //{

                        temp = neimengpro.ModifyLaLon(temp, pModel.LonOffset, pModel.LatOffset);

                    //}
                    //解包temp 修改ISSI号，修改包文件头，重新封包
                        tempData = neimengPro.ToGPSData(temp);                  
                        temp = PacketForNeiMeng(tempData);

                    //------------------------------------------

                    //if (pModel.NetProtocol.Equals("UDP"))
                    //{
                        try
                        {
                            udpClient.Send(temp, temp.Length, pModel.EndPoint);
                            statistics += 1;
                            status = 1;
                            if (Resource.GPSDebugEnabled)
                            {
                                Logger.Info(String.Format("实例：{0} 号码:{1}", pModel.Name, tempData.id));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Resource.GPSDebugEnabled)
                            {
                                Logger.Error("error occur when send GPS data with UDP protocol,error message:" + ex.ToString());
                            }
                            status = 0;
                        }
                    //}
                    

                }
                if (!enabled)
                {
                    if (dataQueue.Count > 0)
                    {
                        continue;
                    }

                    Logger.Info("Thread:IPInstance instance(" + pModel.Name + ") exit safely");
                    isStop = true;
                    return;
                }
                while (enabled && dataQueue.Count == 0)
                {
                    Thread.Sleep(10);
                }

            }

        }


        // 转换整形数据网络次序的字节数组
        public static byte[] IntToBytes(ushort i)
        {
            byte[] t = BitConverter.GetBytes(i);
            byte b = t[0];
            t[0] = t[1];
            t[1] = b;        
            return (t);
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

        [System.Runtime.InteropServices.DllImport("Oleaut32.dll", EntryPoint = "VariantTimeToSystemTime")]
        public static extern int VariantTimeToSystemTime(double time_double, ref Time time);
        public byte[] strId = null;
        private byte[] PacketForNeiMeng(GPSData temp)
        {

            neimengTBody.lat = temp.lat;
            neimengTBody.lon = temp.lon;
          
            neimengTBody.speed = (ushort)System.Net.IPAddress.HostToNetworkOrder(temp.speed);
          
            neimengTBody.height = (ushort)System.Net.IPAddress.HostToNetworkOrder(temp.height); 
            neimengTBody.precision = 0;

   

            neimengTBody.dir = (ushort)System.Net.IPAddress.HostToNetworkOrder(temp.dir);
            neimengTBody.id = new char[20];
            now = DateTime.Now;
       
            neimengTBody.year = (ushort)System.Net.IPAddress.HostToNetworkOrder(Convert.ToInt16(now.Year));
            neimengTBody.month = Convert.ToByte(now.Month);
            neimengTBody.day = Convert.ToByte(now.Day);
            neimengTBody.hour = Convert.ToByte(now.Hour);
            neimengTBody.minute = Convert.ToByte(now.Minute);
            neimengTBody.second = Convert.ToByte(now.Second);
        
            int leng = temp.id.Length;

            for (int i = 0; i < 20; i++)
            {
                if (i < leng)
                {
                    neimengTBody.id[i] = temp.id.ElementAt<char>(i);

                }
                else
                {
                    neimengTBody.id[i] = '\0';
                }

            }
            byte[] result=null;
            string content = "";
            neimengGPS.neimengTHead = neimengTHead;
            neimengGPS.neimengTBody = neimengTBody;
            result= neimengpro.GetByte(neimengGPS);
            for (int i = 0; i < result.Length; i++)
            {
                content = content + result[i]+"\r\n";
            }
          
            return result;
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
            //if too slow
            if (isSlow == 1)
            {
                return 2;
            }

            return this.status;
        }

        public long GetStatistics()
        {
            return this.statistics;
        }

        public Common.PluginType GetPluginType()
        {
            return this.pType;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;

        }

        public void SetDebugEnabled(bool enabled)
        {
            this.debugEnabled = enabled;
        }


        public void Execute(byte[] data)
        {
            if (TranLimit >= Resource.MaxTranLimit)
            {
                this.isSlow = 1;
            }
            else
            {
                dataQueue.Enqueue(data);
                this.isSlow = 0;

            }

        }

        public void SetPluginModel(PluginModel pModel)
        {
            lock (lckPModel)
            {
                this.pModel = pModel;
                enabled = pModel.Enabled;

                lock (lckHelper)
                {
                    string s = "Data Source=" + pModel.DesIP + "," + pModel.DesPort + "\\" + pModel.DesInstance + ";Initial Catalog=" + pModel.DesCatalog + ";uid=" + pModel.DesUser + ";pwd=" + pModel.DesPwd;
                    sqlHelper = new SqlHelper(s);
                }

            }
        }
    }
}
