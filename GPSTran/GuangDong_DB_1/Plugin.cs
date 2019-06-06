using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Common;

///珠海省厅接口 800M
namespace GuangDong_DB_1
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

            var sql = new StringBuilder();
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
                            Logger.Info(string.Format("Thread:plugin instance({0}) thread exit safely", pModel.Name));
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
                                    sql.AppendFormat("insert into {0}(devid,longitude,latitude,SPEED,direction,locationtime,city) values('{1}',{2},{3},{4},{5},'{6}',{7})\n", pModel.TableName, temp.id, temp.lon, temp.lat, temp.speed, temp.dir, temp.time,pModel.Citycode);
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
                            Logger.Warn(string.Format("insert into table {0} too slow", pModel.TableName));
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
                            Logger.Info(string.Format("Thread:plugin instance({0}) thread exit safely", pModel.Name));
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
                        Logger.Error(string.Format("Error occur when flush GPSData into table {0} ,error message:{1}", pModel.TableName, ex.ToString()));
                    }
                    status = 0;
                }

                if (!enabled)
                {
                    if (dataQueue.Count > 0)
                    {
                        continue;
                    }
                    Logger.Info(string.Format("Thread:plugin instance({0}) thread exit safely", pModel.Name));
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
            try
            {
                int count = sqlHelper.ExecuteScalar(CommandType.Text, sql, new[] { new SqlParameter("@name", pModel.TableName) });
                if (count <= 0)
                {
                    Logger.Warn(string.Format("detect table {0} does not exists,try to create it...", pModel.TableName));
                    if (RepairTable(pModel.TableName) <= 0)
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("error occur when check table in plugin instance({0}),error message:{1}", pModel.Name, ex));
                return 0;
            }
            return 1;
        }

        private int RepairTable(string name)
        {
            //创建表GPS
            string sql = string.Format("create table {0}(id int identity(1,1) primary key,devbtype int,devstype varchar(100),devid varchar(100),city varchar(100),longitude decimal(10,7),latitude  decimal(9,7),altitude  decimal(10,5),speed decimal(10,5),direction decimal(5,2),locationtime varchar(30),workstate int,clzl varchar(100),hphm varchar(100),jzlx int,jybh varchar(100),jymc varchar(100),lxdh varchar(100),ssdwdm varchar(100),ssdwmc varchar(100),teamno varchar(100),dth varchar(100),reserve1 varchar(100),reserve2 varchar(100),reserve3 varchar(100))", name);
            try
            {
                sqlHelper.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("error occur when repair table {0} in InsertCommonThread thread,error message:{1}", pModel.TableName, ex));
                return 0;
            }

            //创建表对应触发器
            var sb = new StringBuilder();
            sb.AppendFormat("create Trigger Tri_{0}_insert on {0} instead of insert as begin ",name);
            sb.Append("declare @Gssi int; \n");
            sb.Append("declare @num varchar(100); \n");
            sb.Append("declare @nam varchar(100); \n");
            sb.Append("declare @entityname varchar(100); \n");
            sb.Append("declare @entitycode varchar(100); \n");
            sb.Append("declare @type varchar(100); \n");
            sb.Append("declare @lxdh varchar(100); \n");
            sb.Append("declare @clzl varchar(100); \n");
            sb.Append("declare @hphm varchar(100); \n");
            sb.Append("declare @pdtortetra int; \n");
            sb.Append("declare @pd varchar(100); \n");
            sb.Append("declare @i int; \n");
            sb.Append("select @nam=nam,@num=num,@entityname=entity_name,@type=[type],@pdtortetra=ispdt,@lxdh=telephone from UserInfo B inner join inserted A on B.issi=A.devid; \n");
            sb.Append("if @pdtortetra=0 \n");
            sb.Append("begin \n");
            sb.Append("set @pd='800M'; \n");
            sb.Append("set @pdtortetra=2; \n");
            sb.Append("select @Gssi=c.gssi from GroupInfo C INNER JOIN inserted D on C.ISSI=D.devid; \n");
            sb.Append("end \n");
            sb.Append("else \n");
            sb.Append("begin \n");
            sb.Append("set @pd='350M'; \n");
            sb.Append("set @pdtortetra=1; \n");
            sb.Append("select @Gssi=c.groupdailstring from GroupInfo C INNER JOIN inserted D on C.DailString=D.devid; \n");
            sb.Append("end \n");
            sb.Append("if @Gssi is null \n");
            sb.Append(" set @Gssi=0; \n");
            sb.Append("if PATINDEX('%车%',@type)>0 \n");
            sb.Append("begin \n");
            sb.Append(" set @clzl=@type; \n");
            sb.Append(" set @hphm=@num; \n");
            sb.Append(" set @num=NULL; \n");
            sb.Append(" set @nam=NULL; \n");
            sb.Append("end \n");
            sb.Append("set @i=CHARINDEX('_',@entityname); \n");
            sb.Append("if @i>0 \n");
            sb.Append("begin \n");
            sb.Append("set @entitycode=right(@entityname,len(@entityname)-@i); \n");
            sb.Append("set @entityname=left(@entityname,@i-1); \n");
            sb.Append("end \n");
            sb.AppendFormat("insert into {0}(devbtype,devstype,devid,city,longitude,latitude,speed,direction,locationtime,clzl,hphm,jybh,jymc,lxdh,ssdwdm,ssdwmc,teamno) \n", name);
            sb.Append("select @pdtortetra,@pd,A.devid,A.city,A.longitude,A.latitude,A.SPEED,A.direction,A.locationtime,@clzl,@hphm,@num,@nam,@lxdh,@entitycode,@entityname,@Gssi from inserted A \n");
            sb.Append("end");
            try
            {
                sqlHelper.ExecuteNonQuery(CommandType.Text, sb.ToString());
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("error occur when repair table {0} in InsertCommonThread thread,error message:{1}", pModel.TableName, ex));
                return 0;
            }
            sb.Clear();
            return 1;
        }


        public PluginModel GetPluginModel()
        {
            return pModel;
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

            return status;
        }

        public long GetStatistics()
        {
            return statistics;
        }

        public PluginType GetPluginType()
        {
            return pType;
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
                    string s = string.Format("Data Source={0},{1}\\{2};Initial Catalog={3};uid={4};pwd={5}", pModel.DesIP, pModel.DesPort, pModel.DesInstance, pModel.DesCatalog, pModel.DesUser, pModel.DesPwd);
                    sqlHelper = new SqlHelper(s);
                }

            }
        }
    }
}
