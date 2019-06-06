using System;
using System.Linq;
using System.Threading;
using Common;
/**
 *插件实例分发线程
 *作用：因为要求每一个插件都要在异步的处理GPS消息，所以需要一个线程将GPS消息分发给插件的实例
 * 
**/



namespace GPSTran
{
   public class CommonPluginThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        private ThreadState threadStatus;
        //private int TranLimit=1000;
        //private Protocol Pro;

        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
           
        }
        private Thread PluginExecuteThread;
       
        public CommonPluginThread() 
        {
            threadStatus = ThreadState.Unstarted;
            PluginExecuteThread = new Thread(new ThreadStart(Run));
        
        }


        public int Start() 
        {
           
            Enabled = true;
            Suspended = false;
            //try to start thread
            try
            {
                PluginExecuteThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: PluginExecuteThread is running");
            }
            catch (Exception ex)
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error("can't start thread: PluginExecuteThread" + ex.ToString());
                return 0;
            }

         
            return 1;
        }
     

        private void Run() 
        {
           
          
            byte[] data=new byte[Resource.ProtocolLength];
           
            while (true) 
            {
                while (Resource.PluginDataQueue.Count > 0&&(!Suspended))
                {
                    if (!Resource.PluginDataQueue.TryDequeue(out data))
                    {
                        if (!Enabled)
                        {
                            threadStatus = ThreadState.Stopped;
                            return;
                        }
                        Thread.Sleep(5);
                        continue;
                    }
                    //lock PluginInstanceList

                    try
                    {
                        lock (Resource.lckPluginInstanceList)
                        {

                           


                                int i = 0;

                                foreach (PluginManager item in Resource.PluginList.Values)
                                {

                                    if (data[2] != 221)
                                    {


                                        if (item.GetPluginModel().DllModel.Name != "UDP_Version_8"&& item.GetPluginModel().DllModel.Name != "DB_Version_8")
                                        {
                                            if (Resource.PluginInstanceList.ElementAt(i).Value.GetStatus() == 0)
                                            {
                                                i++;//xzj--20190201--oracle--没有i++,若出现异常协议，会导致后面的协议都无法执行
                                                continue;
                                            }
                                            Resource.PluginInstanceList.ElementAt(i).Value.Execute(data);
                                        }
                                    }

                                    else
                                    {

                                        if (item.GetPluginModel().DllModel.Name == "UDP_Version_8" || item.GetPluginModel().DllModel.Name == "DB_Version_8")
                                        {
                                          
                                            if (Resource.PluginInstanceList.ElementAt(i).Value.GetStatus() == 0)
                                            {
                                                i++;//xzj--20190201--oracle--没有i++,若出现异常协议，会导致后面的协议都无法执行
                                                continue;
                                            }
                                            Resource.PluginInstanceList.ElementAt(i).Value.Execute(data);
                                        }
                                    
                                    
                                    }
                                    i++;

                                }


                            
                          

                                //int n = 0;

                                //foreach (PluginManager item in Resource.PluginList.Values)
                                //{


                                //    if (item.GetPluginModel().DllModel.Name == "UDP_Version_8")
                                //    {
                                //        if (Resource.PluginInstanceList.ElementAt(n).Value.GetStatus() == 0)
                                //        {
                                //            continue;
                                //        }
                                //        Resource.PluginInstanceList.ElementAt(n).Value.Execute(data);
                                //    }
                                //    n++;
                                //}
                            


                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("错误"+ex.Message);
                    
                    }

                    
                }
                
                if (!Enabled)
                {
                    if (Resource.PluginDataQueue.Count > 0) 
                    {
                        continue;
                    }
 
                    threadStatus = ThreadState.Stopped;
                    Logger.Info("Thread: commonPluginThread exit safely");
                    return;

                }

                while (Suspended&&Enabled)
                {
                    threadStatus = ThreadState.Suspended;
                    Thread.Sleep(50);
                }
                threadStatus = ThreadState.Running;
                //if there is no PluginInstance to deal with,then thread will suspend
                while (Resource.PluginInstanceList.Count <= 0&&Enabled) 
                {
                    threadStatus = ThreadState.Suspended;
                    Thread.Sleep(100);
                
                }
                while (Resource.PluginDataQueue.Count <= 0 && Enabled) 
                {
                    Thread.Sleep(1);
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
                throw new Exception("Thread PluginExecuteThread is unstarted or stopped");
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
