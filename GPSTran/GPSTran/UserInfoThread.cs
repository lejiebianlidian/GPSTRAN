using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using Common;

/**
 * 用户线程类
 * 在转发数据库中保存一份GIS数据库的用户表，每隔一段时间同步一次
 * 
 **/
namespace GPSTran
{
    public class UserInfoThread : IThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        private SqlHelper WebGisHelper;
        private SqlHelper TranHelper;
        private Thread UpdateUserInfoThread;
        private ThreadState threadStatus;
        //private SqlHelper sqlHelper;


        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
        }

        public UserInfoThread()
        {
            WebGisHelper = new SqlHelper(Resource.WebGisConn);
            TranHelper = new SqlHelper(Resource.TranConn);
            UpdateUserInfoThread = new Thread(new ThreadStart(Run));
            threadStatus = ThreadState.Unstarted;
            //sqlHelper = new SqlHelper(Resource.TranConn);

        }

        public int Start()
        {

            Enabled = true;
            Suspended = false;
            //try to start thread
            try
            {
                UpdateUserInfoThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: UpdateUserInfo is running");
            }
            catch (Exception ex)
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error("can't start thread: UpdateUserInfo" + ex.ToString());
                return 0;
            }
            return 1;
        }


        private void Run()
        {
            Thread.Sleep(5000);
            CheckTable();

            if (!Resource.UserinfoEnabled)
            {
                Logger.Info("UserinfoEnabled is false,Thread UpdateUserInfo will not start");
                Enabled = false;
                Suspended = false;
                threadStatus = ThreadState.Stopped;
                return;
            }

            //const string sqlStr = "select 0 as id,info.Nam as Nam,info.Num as Num,info.ISSI as ISSI,en.ID as Entity_ID,info.type as Type,en.Name as Entity_Name from user_info info left join Entity en on info.Entity_ID=en.ID";
            const string sqlStr =
                "select 0 as id,info.Nam as Nam,info.Num as Num,info.ISSI as ISSI,en.ID as Entity_ID,info.type as Type,en.Name as Entity_Name ,case when issiinfo.typename='tetra' then 0 else 1 end as ispdt,info.telephone " +
                "from user_info info inner join Entity en on info.Entity_ID=en.ID " +
                "inner join ISSI_info issiinfo on issiinfo.ISSI=info.ISSI";
            while (Enabled)
            {
                int flag = 0;

                try
                {
                    //dtWebGis = WebGisHelper.ExecuteRead(CommandType.Text, sqlStr, "UserInfo", null);
                    DataTable dtWebGis = WebGisHelper.ExecuteDataReader(CommandType.Text, sqlStr);
                    if (dtWebGis != null && dtWebGis.Rows.Count > 0)
                    {
                        TranHelper.ExecuteNonQuery(CommandType.Text, "truncate table UserInfo");
                        var bulk = new SqlBulkCopy(Resource.TranConn, SqlBulkCopyOptions.UseInternalTransaction);
                        bulk.DestinationTableName = "UserInfo";

                        bulk.WriteToServer(dtWebGis);
                        Logger.Info("update table userinfo successfully!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("error occur when update userinfo,error message:" + ex.ToString());
                }
                //sleep interval 
                while (flag < (Resource.FlushInterval) * 600)
                {
                    //try to sleep until Suspended is false 
                    while (Suspended)
                    {
                        threadStatus = ThreadState.Suspended;
                        Thread.Sleep(50);
                    }
                    threadStatus = ThreadState.Running;

                    //if enabled is false ,then function will return and thread will stop
                    if (!Enabled)
                    {
                        threadStatus = ThreadState.Stopped;
                        Logger.Info("Thread UpdateUserInfo stop");
                        return;

                    }
                    Thread.Sleep(100);
                    flag++;
                }
            }
            threadStatus = ThreadState.Stopped;
            Logger.Info("Thread UpdateUserInfo stop");
        }

        public int CheckTable()
        {
            const string sql = @"select count(0) as count from sysobjects where xtype='U' and name=@name";
            //int count = 0;
            //DataTable dt;

            var hascount = TranHelper.ExecuteScalar(CommandType.Text, sql, new SqlParameter[] { new SqlParameter("@name", "UserInfo") });
            if (hascount <= 0)
            {
                //Logger.Warn("detect table UserInfo does not exists,try to create it...");
                if (RepairTable() <= 0)
                {
                    Logger.Error("Error occur when try to repair table UserInfo");
                    return 0;
                }
            }
            
            Enabled = true;
            Suspended = false;
            return 1;
        }

        private int RepairTable()
        {
            const string sql = "CREATE TABLE [dbo].[UserInfo]([id] [int] IDENTITY(1,1) NOT NULL,[nam] [varchar](50) NULL,[num] [varchar](30) NULL,[issi] [varchar](30) NULL,[entity_id] [varchar](30) NULL,[type] [varchar](30) NULL,[entity_name] [varchar](50) NULL,[ispdt] [int],[telephone] [varchar](50),PRIMARY KEY CLUSTERED([id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";
            try
            {
                return TranHelper.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                Logger.Info("error occur when repair table UserInfo in UpdateUserInfo thread,error message:" + ex.ToString());
                return 0;
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
                //throw new Exception("Thread is unstarted or stopped");
                Logger.Error(string.Format("userinfothread error:{0}", "Thread is unstarted or stopped"));
            }
            Suspended = true;
        }
        //try to resume thread
        public void BeginResume()
        {

            if (threadStatus == ThreadState.Stopped || threadStatus == ThreadState.Unstarted)
            {
                //throw new Exception("Thread is unstarted or stopped");
                Logger.Error(string.Format("userinfothread error:{0}", "Thread is unstarted or stopped"));
            }
            Suspended = false;
        }

    }
}
