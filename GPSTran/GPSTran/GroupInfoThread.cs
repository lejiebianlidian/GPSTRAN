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
    public class GroupInfoThread : IThread
    {
        private bool Enabled = false;
        private bool Suspended = false;
        //private SqlHelper WebGisHelper;
        private SqlHelper TranHelper;
        private Thread UpdategroupInfoThread;
        private ThreadState threadStatus;
        //private SqlHelper sqlHelper;


        public ThreadState ThreadStatus
        {
            get { return threadStatus; }
        }

        public GroupInfoThread()
        {
            TranHelper = new SqlHelper(Resource.TranConn);
            UpdategroupInfoThread = new Thread(new ThreadStart(Run));
            threadStatus = ThreadState.Unstarted;
        }

        public int Start()
        {

            Enabled = true;
            Suspended = false;
            //try to start thread
            try
            {
                UpdategroupInfoThread.Start();
                threadStatus = ThreadState.Running;
                Logger.Info("Thread: UpdategroupInfoThread is running");
            }
            catch (Exception ex)
            {
                Enabled = false;
                threadStatus = ThreadState.Stopped;
                Suspended = false;
                Logger.Error("can't start thread: UpdategroupInfoThread" + ex.ToString());
                return 0;
            }
            return 1;
        }


        private void Run()
        {
            Thread.Sleep(5000);
            CheckTable();

            threadStatus = ThreadState.Stopped;
            Logger.Info("Thread GROUPINFO stop");
        }

        public int CheckTable()
        {
            const string sql = @"select count(0) as count from sysobjects where xtype='U' and name=@name";

            var hascount = TranHelper.ExecuteScalar(CommandType.Text, sql, new SqlParameter[] { new SqlParameter("@name", "groupinfo") });
            if (hascount <= 0)
            {
                if (RepairTable() <= 0)
                {
                    Logger.Error("Error occur when try to repair table UpdategroupInfoThread");
                    return 0;
                }
            }
            
            Enabled = true;
            Suspended = false;
            return 1;
        }

        private int RepairTable()
        {
            const string sql = "CREATE TABLE [dbo].[GroupInfo]([ISSI] [int] primary key,[GSSI] [int] NULL,[WorkState] [int] NULL,[DailString] [int] default(0),[GroupDailString] [int] default(0))";
            try
            {
                return TranHelper.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                Logger.Info("error occur when repair table UpdategroupInfoThread in UpdategroupInfoThread thread,error message:" + ex.ToString());
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

        public void BeginStop()
        {
            Enabled = false;
            Suspended = false;
        }

        public void BeginSuspend()
        {

            if (threadStatus == ThreadState.Stopped || threadStatus == ThreadState.Unstarted)
            {
                Logger.Error(string.Format("UpdategroupInfoThread error:{0}", "Thread is unstarted or stopped"));
            }
            Suspended = true;
        }

        public void BeginResume()
        {

            if (threadStatus == ThreadState.Stopped || threadStatus == ThreadState.Unstarted)
            {
                Logger.Error(string.Format("UpdategroupInfoThread error:{0}", "Thread is unstarted or stopped"));
            }
            Suspended = false;
        }

    }
}
