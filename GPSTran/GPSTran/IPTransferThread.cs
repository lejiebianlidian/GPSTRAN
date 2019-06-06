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
   public class IPTransferThread:IThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        private Thread TransferThread;
        private ThreadState threadStatus;

        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
           
        }
        public static long Total;
      
        public IPTransferThread() 
        {
            Enabled = false;
            Suspended = false;
            threadStatus = ThreadState.Unstarted;
            Total = 0;
            TransferThread = new Thread(new ThreadStart(Run));
          
        }
        public int Start() 
        {
            Enabled = true;
            Suspended = false;

            try
            {
                TransferThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: transferThread is running");
            }
            catch (Exception ex) 
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error(string.Format("can't start thread: transferThread:{0}",ex.ToString()));
                return 0;
            
            }
            return 1;
        
        }


        private void Run() 
        {
            byte[] data=new byte[Resource.ProtocolLength];
            while (true) 
            {
                while (Resource.GPSByteQueue.Count > 0) 
                {
                    if (Resource.GPSByteQueue.TryDequeue(out data) == false)
                    {
                        if (!Enabled)
                        {
                            threadStatus = ThreadState.Stopped;
                            return;
                        }
                       
                        Thread.Sleep(5);
                        continue;
                    }
                    lock (Resource.lckIPInstanceList)
                    {
                        foreach (IPManager ipManager in Resource.IPInstanceList.Values)
                        {
                            ipManager.Execute(data);
                        }

                    }

                   
                }

              


                if (!Enabled)
                {
                    if (Resource.GPSByteQueue.Count > 0) 
                    {
                        continue;
                    }

                    threadStatus = ThreadState.Stopped;
                    Logger.Info("Thread: ipTransferThread exit safely");
                    return;

                }


                while (Suspended && Enabled)
                {
                    threadStatus = ThreadState.Suspended;
                    Thread.Sleep(50);
                }
                threadStatus = ThreadState.Running;


                //if there is no IP to transfer GPSData,then thread will suspend
                while (Resource.IPInstanceList.Count <= 0&&Enabled) 
                {
                    threadStatus = ThreadState.Suspended;
                    Thread.Sleep(100);
                }
            
            }
          
        }

        public int Safe_Stop()
        {
            Enabled = false;
            Suspended = false;




            while (ThreadStatus != ThreadState.Stopped)
            {
                Thread.Sleep(10);
                Enabled = false;
                Suspended = false;

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
