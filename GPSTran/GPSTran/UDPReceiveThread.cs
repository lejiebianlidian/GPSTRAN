using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Common;
namespace GPSTran
{
  public class UDPReceiveThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        private Thread ReceiveThread;
        private ThreadState threadStatus;

        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
        }

        public long Total;
        private UdpClient ReceiveClient;
        private IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        
        private Protocol Pro;
        private ProtocolFile Pro1;

        public UDPReceiveThread() 
        {
            threadStatus = ThreadState.Unstarted;
            Total = 0;
            Pro = new Protocol();
            Pro1 = new ProtocolFile();
           
            ReceiveThread = new Thread(new ThreadStart(Receive));
        }


        private void Receive()
        {
            //Thread.Sleep(5000);
            try
            {
                //free port
                if (ReceiveClient != null)
                {
                    ReceiveClient.Close();
                }
                ReceiveClient = new UdpClient(Resource.ListeningPort);

            }
            catch (Exception ex)
            {
                Logger.Error("create UDPClient failed,error message:" + ex.ToString());
                return;
            }


            byte[] bytes = null;
            GPSData data;
            GPSDataFile data1;
            while (Enabled)
            {
       
                try
                {
                    bytes = ReceiveClient.Receive(ref remote);

                    //confirm data length
                    if (bytes.Length != Resource.ProtocolLength && bytes.Length!=22)
                    {
                        
                        //UDPReceiveThreadField 
                
                        continue;
                    }
                    if (bytes[0] != 170)
                    {
                        continue;
                    }
                    if (bytes[2] != 204 &&bytes[2] !=221)
                    {
                        continue;
                    }

                    //------------------------------------
                    if (bytes[2] == 221)//场强
                    {
                  
                        //parse byte to GPSData
                        data1 = Pro1.ToGPSDataFile(bytes);
                        if (Resource.GPSLogEnabled)
                        {
                            Logger.Info(string.Format("RECEIVE GPS [ISSI：{0}，nMsRSSI：{1}，nULRSSI：{2}, nBattery：{3},nReasonForSending{4},Time：{5}]", data1.id, data1.nMsRSSI, data1.nULRSSI, data1.nBattery,data1.nReasonForSending, data1.time));
                        }

                        //if ((Resource.IPInstanceList.Count > 0))
                        //{
                        //    //the count of queue should be in control
                        //    if (Resource.GPSByteQueue.Count <= Resource.CacheLength)
                        //    {
                        //        Resource.GPSByteQueue.Enqueue(bytes);
                        //    }
                        //    //else
                        //    //{
                        //    //}
                        //}
                        if (Resource.PluginInstanceList.Count > 0)
                        {
                            if (Resource.PluginDataQueue.Count <= Resource.CacheLength)
                            {
                                Resource.PluginDataQueue.Enqueue(bytes);
                            }
                            //else
                            //{
                            //}
                        }

                        //enqueue data to GPSDataQueue 
                        //if (Resource.DBInstanceList.Count > 0)
                        //{
                        //    if (Resource.GPSDataQueue.Count <= Resource.CacheLength)
                        //    {
                        //        Resource.GPSDataQueueFile.Enqueue(data1);
                        //    }
                        //    //else
                        //    //{
                        //    //}
                        //}

                        if ( Resource.PluginDataQueue.Count > Resource.CacheLength)
                        {
                            Resource.TranDelay = true;
                        }
                        else
                        {
                            Resource.TranDelay = false;
                        }


                    }
                    //-------------------------------------------------------

                    if (bytes[2] == 204)
                     {
                        //parse byte to GPSData
                        data = Pro.ToGPSData(bytes);
                        if (Resource.GPSLogEnabled)
                        {
                            Logger.Info(string.Format("RECEIVE GPS [ISSI：{0}，Longitude：{1}，Latitude：{2}, Speed：{3}, Direction：{4}, Message Type：{5}, Time：{6}]", data.id, data.lon.ToString(), data.lat.ToString(), data.speed, data.dir, data.nMsgtype, data.time));
                        }

                        if ((Resource.IPInstanceList.Count > 0))
                        {
                            //the count of queue should be in control
                            if (Resource.GPSByteQueue.Count <= Resource.CacheLength)
                            {
                                Resource.GPSByteQueue.Enqueue(bytes);
                            }
                            //else
                            //{
                            //}
                        }
                        if (Resource.PluginInstanceList.Count > 0)
                        {
                            if (Resource.PluginDataQueue.Count <= Resource.CacheLength)
                            {
                                Resource.PluginDataQueue.Enqueue(bytes);
                            }
                            //else
                            //{
                            //}
                        }

                        //enqueue data to GPSDataQueue 
                        if (Resource.DBInstanceList.Count > 0)
                        {
                            if (Resource.GPSDataQueue.Count <= Resource.CacheLength)
                            {
                                Resource.GPSDataQueue.Enqueue(data);
                            }
                            //else
                            //{
                            //}
                        }

                        if (Resource.GPSByteQueue.Count > Resource.CacheLength || Resource.GPSDataQueue.Count > Resource.CacheLength || Resource.PluginDataQueue.Count > Resource.CacheLength)
                        {
                            Resource.TranDelay = true;
                        }
                        else
                        {
                            Resource.TranDelay = false;
                        }

                    }


                    Total = Total + 1;
                }
                catch (Exception ex)
                {
                    if (Resource.GPSDebugEnabled)
                    {
                        Logger.Error("error occur when listening UDP port,error message:" + ex.ToString());
                    }
                }
                
                while (Suspended) 
                {
                    threadStatus = ThreadState.Suspended;
                    Thread.Sleep(50);
                }
                threadStatus = ThreadState.Running;
           

            }
            threadStatus = ThreadState.Stopped;
            Logger.Info("Thread ReceiveThread stop");
        }
        public int Start() 
        {
            Enabled = true;
            Suspended = false;

            try
            {
                ReceiveThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: ReceiveThread is running");
            }
            catch (Exception ex) 
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error(string.Format("can't start thread: ReceiveThread:{0}",ex.ToString()));
                return 0;
            
            }
            return 1;
        }
        public int Safe_Stop()
        {
            Enabled = false;
            Suspended = false;

            try
            {
                threadStatus = ThreadState.Stopped;
                ReceiveThread.Abort();
               
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("UDPReceive Safe_Stop :{0}", ex.ToString()));
            }


            while (ThreadStatus != ThreadState.Stopped)
            {
                Thread.Sleep(10);

            }

            return 1;
        }


        //try to stop thread
        public void BeginStop()
        {
            Enabled = false;
            Suspended = false;
        }
        //try to suspend thread
        public void BeginSuspend()
        {

            if (threadStatus == ThreadState.Stopped || threadStatus == ThreadState.Unstarted)
            {
                throw new Exception("Thread is unstarted or stopped");
            }
            Suspended = true;

        }
        //try to resume thread
        public void BeginResume()
        {

            if (threadStatus == ThreadState.Stopped || threadStatus == ThreadState.Unstarted)
            {
                throw new Exception("Thread is unstarted or stopped");
            }
            Suspended = false;
        }
    }
}
