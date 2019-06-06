using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Common;
/**
 * 清理线程，负责清理本地数据库部分过时的数据，清理策略根据配置文件中表容量上限来进行清理，
 * 如果数据量超过了配置上限，那么清空该表。
 * 
**/

namespace GPSTran
{
   public class TableMaintenanceThread:IThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        private ThreadState threadStatus;
        private SqlHelper TranHelper;
        //thread sleep interval,unit minute
        private int Interval;
        
        private Thread TableMaintenance;
       
        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
           
        }
        public TableMaintenanceThread() 
        {
            TranHelper = new SqlHelper(Resource.TranConn);
           
            TableMaintenance = new Thread(new ThreadStart(Run));
            threadStatus = ThreadState.Unstarted;
            Interval = 60;
         }

        public int Start() 
        {
            Enabled = true;
            Suspended = false;
            //try to start thread
            try
            {
                TableMaintenance.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: TableMaintenance is running");
            }
            catch (Exception ex)
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error("can't start thread: TableMaintenance" + ex.ToString());
                return 0;
            }

            return 1;
        }
       //clear gps data from database
        private void Run() 
        {
            
            string strCount = "";
            string strDelete = "";
            DataTable dt;
            int count;
            while (Enabled) 
            {
                int flag = 0;
               
                lock (Resource.lckDBInstanceList) 
                {
                    foreach (DBManager dbManager in Resource.DBInstanceList.Values) 
                    {
                        strCount = "select max(rowcnt) from sysindexes where id=OBJECT_ID(@tableName)";
                        strDelete = "truncate table " + dbManager.GetModel().TableName;
                        try
                        {
                            dt = TranHelper.ExecuteRead(CommandType.Text, strCount, "count", new SqlParameter[] { new SqlParameter("@tableName", dbManager.GetModel().TableName) });

                            if (dt != null)
                            {
                                count = Convert.ToInt32(dt.Rows[0][0]);
                                //if table row count is more than MaxCount,then clear table
                                if (count > dbManager.GetModel().MaxCount)
                                {
                                    TranHelper.ExecuteNonQuery(CommandType.Text, strDelete);
                                    Logger.Info("clear data from table " + dbManager.GetModel().TableName + " successfully");
                                }
                            }
                        }
                        catch (Exception ex) 
                        {
                            if (Resource.GPSDebugEnabled) 
                            {
                                Logger.Error("error occur when clear data from table :" + dbManager.GetModel().TableName + ", error message:" + ex.ToString());
                            
                            }
                        
                        }


                    }
                
                }
                lock (Resource.lckPluginInstanceList) 
                {
                    foreach (PluginManager pManager in Resource.PluginInstanceList.Values)
                    {

                        if (pManager.GetPluginType() == PluginType.TranDB)
                        {
                            strCount = "select max(rowcnt) from sysindexes where id=OBJECT_ID(@tableName)";
                            strDelete = "truncate table " + pManager.GetPluginModel().TableName;
                            try
                            {
                                dt = TranHelper.ExecuteRead(CommandType.Text, strCount, "count", new SqlParameter[] { new SqlParameter("@tableName", pManager.GetPluginModel().TableName) });

                                if (dt != null)
                                {
                                    count = Convert.ToInt32(dt.Rows[0][0]);
                                    //if table row count is more than MaxCount,then clear table
                                    if (count > pManager.GetPluginModel().MaxCount)
                                    {
                                        TranHelper.ExecuteNonQuery(CommandType.Text, strDelete);
                                        Logger.Info("clear data from table " + pManager.GetPluginModel().TableName + " successfully");

                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                if (Resource.GPSDebugEnabled)
                                {
                                    Logger.Error("error occur when clear data from table :" + pManager.GetPluginModel().TableName + ", error message:" + ex.ToString());

                                }

                            }
                        }

                    }
                
                
                }


                //thread sleep,sleep time depends on the value of the variable: Interval
                while (flag < Interval * 10*60) 
                {
                    threadStatus = ThreadState.Running;
                    //suspend the thread
                    while (Suspended && Enabled) 
                    {
                        threadStatus = ThreadState.Suspended;
                        Thread.Sleep(100);
                    }
                    //if there are no table to clear,thread will suspend
                    while (Resource.DBInstanceList.Count <= 0&&Resource.PluginInstanceList.Count<=0 && Enabled) 
                    {
                        threadStatus = ThreadState.Suspended;
                        Thread.Sleep(100);
                    }
                    //if enabled is false then return
                    if (!Enabled)
                    {
                        threadStatus = ThreadState.Stopped;
                        Logger.Info("Thread TableMaintenance stop");
                        return;

                    }


                    Thread.Sleep(100);
                    flag++;
                
                }



            
            }
            threadStatus = ThreadState.Stopped;

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
