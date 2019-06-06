using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace Common
{
    public class IPManager
    {
        private IPModel ipModel;
        private Thread ipThread;
        private ConcurrentQueue<byte[]> byteQueue = new ConcurrentQueue<byte[]>();
        private int status;
        private long statistics;
        private bool enabled;
        private Protocol pro;
        private bool isStop;
        private UdpClient udpClient;
        private int isSlow;
        private int maxQueueCount = 50000;
        public bool IsStop()
        {
            return this.isStop;
        }


        public IPManager(IPModel ipModel, bool threadStart)
        {
            if (ipModel == null)
            {
                throw new NullReferenceException("null reference");
            }

            isSlow = 0;
            this.ipModel = ipModel;
            this.pro = new Protocol();
            this.status = 1;
            this.enabled = ipModel.Enabled;
            statistics = 0;
            isStop = false;
            if (threadStart)
            {
                ipThread = new Thread(new ThreadStart(Run));
                ipThread.Start();

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
            while (true)
            {
                while (byteQueue.Count > 0)
                {
                    if (byteQueue.TryDequeue(out data) == false)
                    {
                        if (!enabled)
                        {
                            isStop = true;
                            return;
                        }
                        continue;
                    }
                    int valid = 0;
                    foreach (int i in ipModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(pro.GetIssi(data)))
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

                    temp = pro.ByteClone(data);
                    temp = pro.ModifyLaLon(temp, ipModel.LonOffset, ipModel.LatOffset);
                    try
                    {
                        udpClient.Send(temp, temp.Length, ipModel.EndPoint);
                        statistics += 1;
                        status = 1;
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
                    if (byteQueue.Count > 0)
                    {
                        continue;
                    }

                    Logger.Info("Thread:IPInstance instance(" + ipModel.Name + ") exit safely");
                    isStop = true;
                    return;
                }
                while (enabled && byteQueue.Count == 0)
                {
                    Thread.Sleep(10);
                }

            }

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

        public void Execute(byte[] data)
        {
            if (this.byteQueue.Count > maxQueueCount)
            {
                this.isSlow = 1;
            }
            else
            {
                byteQueue.Enqueue(data);
                isSlow = 0;
            }
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        public IPModel GetModel()
        {
            return ipModel;

        }

        public long GetStatistics()
        {
            return this.statistics;
        }

    }
}
