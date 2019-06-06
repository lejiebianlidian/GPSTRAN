using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Common;
namespace GPSTran
{
   public class EntityAndIssiCacheThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        private SqlHelper WebGisHelper;
        //private SqlHelper TranHelper;
        private Thread UpdateEntityAndIssiThread;
        private ThreadState threadStatus;

        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
           
        }

       public EntityAndIssiCacheThread() 
        {
            WebGisHelper = new SqlHelper(Resource.WebGisConn);
            //TranHelper = new SqlHelper(Resource.TranConn);
            UpdateEntityAndIssiThread = new Thread(new ThreadStart(Run));
            threadStatus = ThreadState.Unstarted;
         }


        private void Run() 
        {
            string sql = "with lmenu(id,name,parentid) as(select id,name,parentid from entity where id=@id union all select a.id,a.name,a.parentid from entity a,lmenu b    where a.parentid = b.id ) select u.ISSI from lmenu m inner join user_info u on m.id=u.entity_id ";
            DataTable entityDt = null;
            DataTable userIdDt = null;
            while (Enabled)
            {
                threadStatus = ThreadState.Running;
                int flag = 0;
                try
                {
                    entityDt = WebGisHelper.ExecuteDataReader(CommandType.Text, "select id,name,parentid from entity",null);
                    //entityDt = WebGisHelper.ExecuteRead(CommandType.Text, "select id,name,parentid from entity", "entity", null);
                    if (entityDt != null)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            Resource.ISSIOfEntity.Clear();
                            for (int i = 0; i < entityDt.Rows.Count; i++)
                            {
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                //userIdDt = WebGisHelper.ExecuteRead(CommandType.Text, sql, "entity", new SqlParameter[] { new SqlParameter("@id", entityDt.Rows[i]["id"]) });
                                userIdDt = WebGisHelper.ExecuteDataReader(CommandType.Text, sql,new[] {new SqlParameter("@id", entityDt.Rows[i]["id"])});
                                if (userIdDt != null&& userIdDt.Rows.Count>0)
                                {
                                   for (int j = 0; j < userIdDt.Rows.Count; j++)
                                    {
                                        if (!dic.ContainsKey(userIdDt.Rows[j]["ISSI"].ToString()))
                                            dic.Add(userIdDt.Rows[j]["ISSI"].ToString(), string.Empty);
                                    }
                                }
                                Resource.ISSIOfEntity.Add(entityDt.Rows[i]["id"].ToString(), dic);

                            }
                        }
                    }
                    if (entityDt != null)
                    {
                        entityDt.Dispose();
                    }
                    if (userIdDt != null)
                    {
                        userIdDt.Dispose();
                    }

                }
                catch (SqlException ex)
                {
                    Logger.Error("error occur when updating EntityAndISSI cache,error message:" + ex.ToString());
                }
                //try to sleep 1 minute
                while (flag < 60 * 10) 
                {
                    //try to sleep until Suspended is false or Enabled is false
                    if (Suspended&&Enabled) 
                    {
                        Thread.Sleep(100);
                    
                    }
                    //if enabled is false ,then function will return and thread will stop
                    if (!Enabled) 
                    {
                        threadStatus = ThreadState.Stopped;
                        Logger.Info("Thread: UpdateEntityAndISSI exit safely");
                        return;
                    
                    }
                    Thread.Sleep(100);
                    flag++;
                }


            }
            threadStatus = ThreadState.Stopped;
            Logger.Info("Thread UpdateEntityAndISSI stop");
        }
        public int Start() 
        {
            Enabled = true;
       
            try
            {
                UpdateEntityAndIssiThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: UpdateEntityAndISSI is running");
            }
            catch (Exception ex)
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error(string.Format("can't start thread: UpdateEntityAndISSI:{0}",ex.ToString()));
                return 0;
            }

            return 1;
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
