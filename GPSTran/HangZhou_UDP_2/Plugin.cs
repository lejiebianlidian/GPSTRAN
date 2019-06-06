using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Common;

namespace HangZhou_UDP_2
{
    public class Plugin : Protocol, IPlugin
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
                this.dealDataThread = new Thread(Run);
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
                    Logger.Error("create UdpClient failed in IPManager，error message:" + ex);
                    return;
                }
            }

            var wj28 = new Wj28();
            wj28.InitValue();
            byte[] data;
            while (true)
            {
                while (dataQueue.Count > 0)
                {
                    if (!dataQueue.TryDequeue(out data))
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
                    foreach (var i in pModel.EntityID)
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

                    gps.TBody.ModifyLonLat(pModel.LonOffset, pModel.LatOffset);
                    wj28.SetValue(gps.TBody);
                    var temp = GetByte(wj28);
                    //wj28.SetCheckSum(getChecksum(temp));
                    //temp = GetByte(wj28);
                    try
                    {
                        udpClient.Send(temp, temp.Length, pModel.EndPoint);
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
                            Logger.Error(string.Format("实例：{0} error occur when send GPS data with UDP protocol,error message:{1}", pModel.Name, ex));
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

        public Byte getChecksum(byte[] data)
        {
            var ba = new BitArray(data);
            var bt = ba.Get(0);
            for (var i=1;i<ba.Length;i++)
            {
                //OddParityCheck(b);
                bt ^= ba.Get(i);
            }
            return Convert.ToByte(bt);
        }

        #region 奇偶校验
        /// 对byte逐位异或进行奇校验并返回校验结果
        /// 要取bit值的byte，一个byte有8个bit
        /// 如果byte里有奇数个1则返回true，如果有偶数个1则返回false
        public bool OddParityCheck(byte b)
        {
            return getBit(b, 0) ^ getBit(b, 1) ^ getBit(b, 2) ^ getBit(b, 3)
               ^ getBit(b, 4) ^ getBit(b, 5) ^ getBit(b, 6) ^ getBit(b, 7);
        }

        /// 对byte逐位异或进行偶校验并返回校验结果
        /// 要取bit值的byte，一个byte有8个bit
        /// 如果byte里有偶数个1则返回true，如果有奇数个1则返回false
        public bool EvenParityCheck(byte b)
        {
            return !getBit(b, 0) ^ getBit(b, 1) ^ getBit(b, 2) ^ getBit(b, 3)
               ^ getBit(b, 4) ^ getBit(b, 5) ^ getBit(b, 6) ^ getBit(b, 7);
        }

        /// 取一个byte中的第几个bit的值，
        /// 实在查不到c#有什么方法，才动手写了这个函数
        /// 要取bit值的byte，一个byte有8个bit
        /// 在byte中要取bit的位置，一个byte从左到右的位置分别是0,1,2,3,4,5,6,7
        /// 返回bit的值，不知道C#中bit用什么表示，似乎bool就是bit，就用它来代替bit吧
        private bool getBit(byte b, int iIndex)
        {
            //MessageBox.Show((b >> (7 - iIndex) & 1).ToString());
            //将要取的bit右移到第一位，再与1与运算将其它位置0
            return (b >> (7 - iIndex) & 1) != 0;
        }
        #endregion 

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
            }
        }
    }
}
