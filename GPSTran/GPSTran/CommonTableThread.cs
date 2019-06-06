using System;
using System.Threading;
using Common;
/**
 *数据库转发实例分发线程
 *作用：因为要求每一个数据库转发实例都要在异步的处理GPS消息，所以需要一个线程将GPS消息分发给数据库转发的实例
 * 
**/


namespace GPSTran
{
   public class CommonTableThread : IThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        //private SQLHelper TranHelper;
        private ThreadState threadStatus;
        //private int TranLimit=1000;
        private Protocol Pro;

        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
           
        }
        private Thread InsertCommonThread;
        private SqlHelper sqlHelper;

        public CommonTableThread() 
        {
            threadStatus = ThreadState.Unstarted;
            InsertCommonThread = new Thread(new ThreadStart(Run));
            sqlHelper = new SqlHelper(Resource.TranConn);
            Pro = new Protocol();
        
        }


        public int Start() 
        {
            Enabled = true;
            Suspended = false;
            //try to start thread
            try
            {
                InsertCommonThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: InsertCommon Thread is running");
            }
            catch (Exception ex)
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error("can't start thread: InsertCommonThread" + ex.ToString());
                return 0;
            }

         
            return 1;
        }
       

        private void Run() 
        {
            
            GPSData data=new GPSData();
            while (true) 
            {
                 if (!Resource.GPSDataQueue.TryDequeue(out data))
                 {
                     if (!Enabled)
                     {
                         threadStatus = ThreadState.Stopped;
                         return;
                     }
                     Thread.Sleep(5);
                    continue;
                    
                 }
                    lock (Resource.lckDBInstanceList) 
                    {
                        foreach (DBManager dbManager in Resource.DBInstanceList.Values)
                        {
                            dbManager.Execute(data);   
                        
                        }
                    
                    }

                if (!Enabled)
                {
                    if (Resource.GPSDataQueue.Count > 0) 
                    {
                        continue;
                    }

                    threadStatus = ThreadState.Stopped;
                    Logger.Info("Thread: commomTableThread exit safely");
                    return;

                }

                while (Suspended&&Enabled)
                {
                    threadStatus = ThreadState.Suspended;
                    Thread.Sleep(50);
                }
                threadStatus = ThreadState.Running;
                //if there is no table to insert,thread will suspend
                while (Resource.DBInstanceList.Count <= 0&&Enabled) 
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
            if (threadStatus != ThreadState.Running)
            {
                throw new Exception("Thread InsertCommon is unstarted or stopped");
            }
            Suspended = true;
            
        }
        //try to resume thread
        public void BeginResume()
        {
            Suspended = false;
        }


    }
}
