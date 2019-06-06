using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Common;

namespace HuaiBei_TCP_0
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
        private Protocol gps;
        //private SQLHelper sqlHelper;
        private Thread dealDataThread;
        //private Protocol Pro;
        private int TranLimit;
        private bool isStop;
        private TcpClient Tcp;
        private NetworkStream nws;

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
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = pModel.Enabled;
            gps = new Protocol();
            this.pModel = pModel;
            if (threadStart)
            {
                dealDataThread = new Thread(Run);
                dealDataThread.Start();
            }
        }

        private void Run()
        {
            //建立连接
            CreateSocket();
            byte[] data;
            //byte[] temp;
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
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(gps.GetIssi(data)))
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

                    //temp = gps.ByteClone(data);
                    data = gps.ModifyLaLon(data, pModel.LonOffset, pModel.LatOffset);

                    try
                    {
                        var ar = nws.BeginWrite(data, 0, data.Length, null, null);
                        if (ar.AsyncWaitHandle.WaitOne())
                        {
                            statistics += 1;
                        }
                        nws.EndWrite(ar);
                        if (Resource.GPSDebugEnabled)
                        {
                            Logger.Info(String.Format("实例：{0} 号码:{1}", pModel.Name, gps.GetIssi(data)));
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

            if (Tcp!=null)
            {
                Tcp.Close();
                Tcp = null;
            }
        }


        private void CreateSocket()
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

