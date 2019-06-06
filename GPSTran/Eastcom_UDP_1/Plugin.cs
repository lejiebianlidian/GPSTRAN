using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Common;

namespace Eastcom_UDP_1
{
    public class Plugin:Protocol
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
        private Thread dealDataThread;
        //private Protocol Pro;
        private int TranLimit;
        private bool isStop;

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
            TranLimit = 1000;
            this.pModel = pModel;
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = pModel.Enabled;

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

            var pdt = new PDT();
            byte[] data;
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
                    var gps = ToGPS(data);
                    int valid = 0;
                    foreach (int i in pModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(gps.TBody.GetIssi()))
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
                    gps.TBody.ModifyLonLat(pModel.LonOffset, pModel.LatOffset);
                    pdt.head.SetValue(gps.TBody);
                    pdt.sdu.SetValue(gps.TBody);
                    ReverseBits(ref pdt.sdu.SDUContent);
                    data = GetByte(pdt);
                    //hzwj.body.Convertbytes(ref data);
                    //neimeng.body.Convertbytes(ref data);

                    //------------------------------------------

                    //if (!pModel.NetProtocol.Equals("UDP")) 
                    //    continue;
                    try
                    {
                        udpClient.Send(data, data.Length, pModel.EndPoint);
                        statistics += 1;
                        status = 1;
                        if (Resource.GPSDebugEnabled)
                        {
                            Logger.Info(String.Format("实例：{0} 号码:{1}", pModel.Name, gps.TBody.GetIssi()));
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

        public PluginModel GetPluginModel()
        {
            return pModel;
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

            return status;
        }

        public long GetStatistics()
        {
            return statistics;
        }

        public PluginType GetPluginType()
        {
            return pType;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;

        }

        public void SetDebugEnabled(bool enabled)
        {

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

                //lock (lckHelper)
                //{
                //    string s = "Data Source=" + pModel.DesIP + "," + pModel.DesPort + "\\" + pModel.DesInstance + ";Initial Catalog=" + pModel.DesCatalog + ";uid=" + pModel.DesUser + ";pwd=" + pModel.DesPwd;
                //    sqlHelper = new SQLHelper(s);
                //}

            }
        }
    }
}
