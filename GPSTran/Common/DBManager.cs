using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common
{
    public class DBManager
    {
        //cache to store GPSData
        private ConcurrentQueue<GPSData> dataQueue = new ConcurrentQueue<GPSData>();

        private int isSlow;

        private DBModel dbModel;
        private readonly Object lckPModel = new Object();
        private int status;
        private long statistics;
        private bool enabled;
        private Protocol pro;
        private SqlHelper sqlHelper;
        private readonly Object lckHelper = new Object();
        private Thread dealDataThread;
        private Protocol Pro;
        private int TranLimit;
        private bool isStop;
        public bool IsStop()
        {
            return isStop;
        }


        public DBManager(DBModel dbModel, bool threadStart)
        {
            if (dbModel == null)
            {
                throw new NullReferenceException("null reference");

            }
            Pro = new Protocol();
            TranLimit = 1000;
            this.dbModel = dbModel;
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = dbModel.Enabled;

            pro = new Protocol();

            sqlHelper = new SqlHelper(Resource.TranConn);

            if (threadStart)
            {
                this.dealDataThread = new Thread(new ThreadStart(Run));
                this.dealDataThread.Start();

            }
        }
        private void Run()
        {
            Thread.Sleep(5000);
            //if status==0,then check table status 

            if (CheckTable() > 0)
            {
                status = 1;

            }

            int flag = 0;
            GPSData data;
            GPSData temp;
            StringBuilder sql = new StringBuilder();
            while (true)
            {



                flag = 0;
                sql.Clear();
                while (dataQueue.Count > 0 && flag < TranLimit)
                {
                    if (!dataQueue.TryDequeue(out data))
                    {
                        if (!enabled)
                        {
                            isStop = true;
                            Logger.Info("Thread:DBInstance instance(" + dbModel.Name + ") exit safely");
                            return;
                        }

                        continue;
                    }

                    temp = Pro.GPSDataClone(data);
                    //issi filter
                    foreach (int i in this.dbModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(temp.id))
                                {
                                    temp = Pro.ModifyLaLon(ref temp, dbModel.LonOffset, dbModel.LatOffset);
                                    sql.AppendFormat("insert into {0}(Send_time,Inserttb_time,ISSI ,Latitude,Longitude,Horizontal_velocity,Direction_travel ,state,nMsgType,height )  values('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10})\n", dbModel.TableName, temp.time, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), temp.id, temp.lat, temp.lon, temp.speed, temp.dir, temp.state, temp.nMsgtype, temp.height);
                                    flag++;
                                    break;
                                }
                            }
                        }
                    }

                }
                //modify TranLimit dynamicly
                if (flag >= TranLimit)
                {
                    TranLimit = TranLimit + 999;
                    if (TranLimit > Resource.MaxTranLimit)
                    {
                        TranLimit = Resource.MaxTranLimit;
                        if (Resource.GPSDebugEnabled)
                        {
                            Logger.Debug("insert into table " + dbModel.TableName + " too slow");
                        }

                    }

                }
                else if (TranLimit - flag > Resource.MinTranLimit)
                {
                    TranLimit = TranLimit - 1;
                    if (TranLimit < Resource.MinTranLimit)
                    {
                        TranLimit = Resource.MinTranLimit;
                    }

                }


                try
                {
                    if (flag <= 0)
                    {
                        if (!enabled)
                        {
                            isStop = true;
                            Logger.Info("Thread:DBInstance instance(" + dbModel.Name + ") exit safely");
                            return;
                        }
                        continue;
                    }
                    sqlHelper.ExecuteNonQuery(CommandType.Text, sql.ToString());
                    statistics += flag;
                    status = 1;

                }
                catch (Exception ex)
                {
                    if (Resource.GPSDebugEnabled)
                    {
                        Logger.Debug("Error occur when flush GPSData into table " + dbModel.TableName + " ,error message:" + ex.ToString());
                    }
                    status = 0;
                }

                if (!enabled)
                {
                    if (dataQueue.Count > 0)
                    {
                        continue;
                    }

                    Logger.Info("Thread:DBInstance instance(" + dbModel.Name + ") exit safely");
                    isStop = true;
                    return;
                }
                while (enabled && dataQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }
            }

        }

        private int CheckTable()
        {
            string sql = "select count(0) as count from sys.sysobjects where name =@name and xtype='U'";
            int count = 0;
            DataTable dt;

            try
            {
                dt = sqlHelper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@name", dbModel.TableName) });

                if (dt != null)
                {
                    count = Convert.ToInt32(dt.Rows[0]["count"]);

                }


                if (count <= 0)
                {
                    Logger.Warn("detect table " + dbModel.TableName + " does not exists,try to create it...");
                    if (RepairTable(dbModel.TableName) <= 0)
                    {
                        return 0;
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.Info("error occur when check table in plugin instance(" + dbModel.Name + "),error message:" + ex.ToString());
                return 0;
            }



            return 1;
        }

        private int RepairTable(string name)
        {
            string sql = "CREATE TABLE dbo." + dbModel.TableName + "(" +
                        "[ISSI] [varchar](50) NULL," +
                        "[Longitude] [numeric](10, 7) NULL," +
                        "[Latitude] [numeric](9, 7) NULL," +
                        "[Send_time] [datetime] NOT NULL," +
                        "[Inserttb_time] [datetime] NULL," +
                        "[Horizontal_velocity] [nvarchar](20) NULL," +
                        "[Direction_travel] [nvarchar](10) NULL," +
                        "[Send_reason] [nvarchar](30) NULL," +
                        "[Position_err] [nvarchar](30) NULL," +
                        "[User_ID] [varchar](10) NULL," +
                        "[id] [bigint] IDENTITY(1,1) NOT NULL," +
                        "[remarks] [nvarchar](100) NULL," +
                        "[state] [int] NULL," +
                        "[height] [int] NULL," +
                        "[nMsgType] [int] NULL," +
                        "CONSTRAINT [PK_" + name + "] PRIMARY KEY CLUSTERED " +
                        "(" +
                        "[id] ASC" +
                        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]" +
                        ") ON [PRIMARY]";

            try
            {
                sqlHelper.ExecuteNonQuery(CommandType.Text, sql);

            }
            catch (Exception ex)
            {
                Logger.Info("error occur when repair table " + dbModel.TableName + " in InsertCommonThread thread,error message:" + ex.ToString());
                return 0;
            }



            return 1;
        }


        public DBModel GetModel()
        {
            return this.dbModel;
        }

        public int GetStatus()
        {
            if (status == 0)
            {
                return status;
            }
            //if too slow
            if (isSlow == 1)
            {
                return 2;
            }

            return this.status;
        }

        public long GetStatistics()
        {
            return this.statistics;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;

        }

        //interface to receive GPSData and cache them in dataQueue
        public void Execute(GPSData data)
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

    }
}
