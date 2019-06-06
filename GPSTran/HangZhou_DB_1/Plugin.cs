using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Common;

/*
 *杭州公安
 *七巧板公司对接
 */
namespace HangZhou_DB_1
{
    public class Plugin : IPlugin
    {
        private ConcurrentQueue<byte[]> dataQueue = new ConcurrentQueue<byte[]>();
        private int isSlow;
        private readonly PluginType pType = PluginType.TranDB;
        private PluginModel pModel;
        private readonly Object lckPModel = new Object();
        private int status;
        private long statistics;
        private bool enabled;
        private bool debugEnabled;
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

        public Plugin()
        {

        }
        public Plugin(PluginModel pModel, bool threadStart)
        {
            if (pModel == null)
            {
                throw new NullReferenceException("null reference");

            }
            Pro = new Protocol();
            TranLimit = 1000;
            this.pModel = pModel;
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = pModel.Enabled;
            debugEnabled = false;
            string s = string.Format("Data Source={0},{1}\\{2};Initial Catalog={3};uid={4};pwd={5}", pModel.DesIP, pModel.DesPort, pModel.DesInstance, pModel.DesCatalog, pModel.DesUser, pModel.DesPwd);

            sqlHelper = new SqlHelper(s);



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
            byte[] test;
            GPSData data;
            GPSData temp;

            StringBuilder sql = new StringBuilder();
            while (true)
            {
                flag = 0;
                sql.Clear();
                while (dataQueue.Count > 0 && flag < TranLimit)
                {
                    if (!dataQueue.TryDequeue(out test))
                    {
                        if (!enabled)
                        {
                            isStop = true;
                            Logger.Info("Thread:plugin instance(" + pModel.Name + ") thread exit safely");
                            return;
                        }

                        continue;
                    }

                    data = Pro.ToGPSData(test);
                    temp = Pro.GPSDataClone(data);
                    //issi filter
                    foreach (int i in this.pModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(temp.id))
                                {
                                    temp = Pro.ModifyLaLon(ref temp, pModel.LonOffset, pModel.LatOffset);
                                    sql.AppendFormat("insert into {0}(Gpsid,X,Y,SPEED,Dir,State,GpsTime)  values('{1}',{2},{3},{4},{5},{6},'{7}')\n", pModel.TableName, temp.id.PadLeft(12, '0'), temp.lon, temp.lat, temp.speed, temp.dir,temp.state,temp.time);
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
                            Logger.Warn("insert into table " + pModel.TableName + " too slow");
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
                            Logger.Info("Thread:plugin instance(" + pModel.Name + ") thread exit safely");
                            return;
                        }
                        continue;
                    }
                    sqlHelper.ExecuteNonQuery(CommandType.Text, sql.ToString());
                    statistics += flag;
                    status = 1;
                    if (Resource.GPSDebugEnabled)
                    {
                        Logger.Info(String.Format("实例：{0} 号码:{1}", pModel.Name, sql));
                    }

                }
                catch (Exception ex)
                {
                    if (Resource.GPSDebugEnabled)
                    {
                        Logger.Error("Error occur when flush GPSData into table " + pModel.TableName + " ,error message:" + ex.ToString());
                    }
                    status = 0;
                }

                if (!enabled)
                {
                    if (dataQueue.Count > 0)
                    {
                        continue;
                    }

                    Logger.Info("Thread:plugin instance(" + pModel.Name + ") thread exit safely");
                    isStop = true;
                    return;
                }
                while (enabled && dataQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }
                //tell thread to sleep 1,for ignoring duplicated PK id which is valued of the current time
                Thread.Sleep(1);

            }

        }

        private int CheckTable()
        {
            string sql = "select count(0) as count from sys.sysobjects where name =@name and xtype='U'";
            int count = 0;
            DataTable dt;

            try
            {
                dt = sqlHelper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@name", pModel.TableName) });
                if (dt != null)
                {
                    count = Convert.ToInt32(dt.Rows[0]["count"]);
                }

                if (count <= 0)
                {
                    Logger.Warn("detect table " + pModel.TableName + " does not exists,try to create it...");
                    if (RepairTable(pModel.TableName) <= 0)
                    {
                        return 0;
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.Info("error occur when check table in plugin instance(" + pModel.Name + "),error message:" + ex.ToString());
                return 0;
            }



            return 1;
        }

        private int RepairTable(string name)
        {
            string sql = "CREATE TABLE " + pModel.TableName + "(" +
                         "[ID] [int] identity(1,1) primary key," +
                         "[Gpsid] [varchar](20) NOT NULL," +
                         "[X] [Decimal](10,7) NULL," +
                         "[Y] [Decimal](9,7) NULL," +
                         "[SPEED] [Decimal](9,2) NULL," +
                         "[Dir] Varchar(10) NULL," +
                         "[State] Varchar(10) NULL," +
                         "[GpsTime] [varchar](30) NULL)";


            try
            {
                sqlHelper.ExecuteNonQuery(CommandType.Text, sql);

            }
            catch (Exception ex)
            {
                Logger.Info("error occur when repair table " + pModel.TableName + " in InsertCommonThread thread,error message:" + ex.ToString());
                return 0;
            }



            return 1;
        }


        public PluginModel GetPluginModel()
        {
            return this.pModel;
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

        public PluginType GetPluginType()
        {
            return this.pType;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;

        }

        public void SetDebugEnabled(bool enabled)
        {
            this.debugEnabled = enabled;
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

                lock (lckHelper)
                {
                    string s = "Data Source=" + pModel.DesIP + "," + pModel.DesPort + "\\" + pModel.DesInstance + ";Initial Catalog=" + pModel.DesCatalog + ";uid=" + pModel.DesUser + ";pwd=" + pModel.DesPwd;

                    sqlHelper = new SqlHelper(s);
                }

            }
        }
    }
}
