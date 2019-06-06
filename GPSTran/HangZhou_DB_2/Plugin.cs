using Common;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Linq;

namespace HangZhou_DB_2
{
    public class Plugin : IPlugin
    {
        private readonly Protocol Pro;
        private readonly ConcurrentQueue<byte[]> dataQueue = new ConcurrentQueue<byte[]>();
        private Thread dealDataThread;
        private int isSlow;
        private readonly object lckHelper = new object();
        private readonly object lckPModel = new object();
        private PluginModel pModel;
        private PluginType pType = PluginType.TranDB;
        private readonly SQLFactory sqlfc;
        //杭州
        private const string sqlstr = "insert into gpsinfo.gps_info(SID,GPSID,X,Y,SPEED,DIR,STATE,GPSTIME) Values(gpsinfo.seq_gpsinfonownowqqb.nextval,'{0}',{1},{2},{3},{4},{5},To_Date('{6}','yyyy-MM-dd hh24:Mi:ss'))";

        //private const string sqlstr = "insert into gpsinfo.gps_info(SID,GPSID,X,Y,SPEED,DIR,STATE,GPSTIME) Values(gpsinfo.seq_gpsinfonownowqqb.nextval,'{0}',{1},{2},{3},{4},{5},To_Date('{6}','yyyy-MM-dd hh24:Mi:ss'))";
        //宁波
        //private const string sqlstr = "insert into gpsinfo.gps_info(SID,GPSID,X,Y,SPEED,DIR,STATE,GPSTIME) Values(gpsinfo.seq_gpsinfo.nextval,'{0}',{1},{2},{3},{4},{5},To_Date('{6}','yyyy-MM-dd hh24:Mi:ss'))";
        //杭州测试
        //const string sqlstr = "insert into gpsinfo.gps_info_dftx(SID,GPSID,X,Y,SPEED,DIR,STATE,GPSTIME) Values(gpsinfo.seq_gpsinfodftx.nextval,'{0}',{1},{2},{3},{4},{5},To_Date('{6}','yyyy-MM-dd hh24:Mi:ss'))";
        private long statistics;
        private int status;

        public Plugin()
        {

        }

        public Plugin(PluginModel pModel, bool threadStart)
        {
            Pro = new Protocol();
            this.pModel = pModel;
            this.status = 1;
            this.isSlow = 0;
            this.statistics = 0L;
            this.sqlfc = new OracleHelper(pModel.DesIP, pModel.DesPort, pModel.DesCatalog, pModel.DesUser, pModel.DesPwd);
            if (threadStart)
            {
                this.dealDataThread = new Thread(new ThreadStart(this.Run));
                this.dealDataThread.Start();
            }
        }

        private bool CheckIssi(string issi)
        {
            foreach (int num in this.pModel.EntityID)
            {
                lock (Resource.lckISSIOfEntity)
                {
                    if (Resource.ISSIOfEntity.ContainsKey(num.ToString()) && Resource.ISSIOfEntity[num.ToString()].ContainsKey(issi))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Execute(byte[] data)
        {
            this.dataQueue.Enqueue(data);
            this.isSlow = 0;
        }

        public PluginModel GetPluginModel()
        {
            return this.pModel;
        }

        public PluginType GetPluginType()
        {
            return this.pType;
        }

        public long GetStatistics()
        {
            return this.statistics;
        }

        public int GetStatus()
        {
            if ((this.status != 0) && (this.isSlow == 1))
            {
                return 2;
            }
            return this.status;
        }

        public bool IsStop()
        {
            if (this.pModel.Enabled)
            {
                this.pModel.Enabled = false;
            }
            return !this.pModel.Enabled;
        }

        private void Run()
        {
            HZ hz = new HZ();
            while (true)
            {

                if (!dataQueue.Any())
                {
                    Thread.Sleep(5000);
                    continue;
                }

                byte[] buffer;
                while (this.dataQueue.TryDequeue(out buffer))
                {
                    TGPS tgps = Pro.ToGPS(buffer);
                    string issi = tgps.TBody.GetIssi();
                    if (this.CheckIssi(issi))
                    {
                        hz.SetValue(tgps.TBody);
                        try
                        {
                            int num = sqlfc.ExecuteNonQuery(CommandType.Text,
                                string.Format(sqlstr, hz.Gpsid, hz.X, hz.Y, hz.Speed, hz.Dir, hz.State, hz.GpsTime));
                            //int num = 1;
                            statistics += num;
                            status = 1;
                            if (Resource.GPSDebugEnabled)
                            {
                                Logger.Info(num > 0
                                    ? string.Format("实例：{0} 数据：{1} {2} {3}", this.pModel.Name, hz.Gpsid, hz.X, hz.Y)
                                    : string.Format("实例：{0} 数据：{1} {2} {3} 入库未成功", this.pModel.Name, hz.Gpsid, hz.X,
                                        hz.Y));
                            }
                        }
                        catch (Exception exception)
                        {
                            status = 0;
                            Logger.Error(string.Format("实例：{0},Error:{1}", this.pModel.Name, exception));
                            continue;
                        }
                        if (!this.pModel.Enabled && !this.dataQueue.Any())
                        {
                            Logger.Info("Thread:plugin instance(" + this.pModel.Name + ") thread exit safely");
                            return;
                        }
                    }
                    else
                    {
                        Logger.Error(string.Format("实例：{0},Error:号码不存在：{1}", this.pModel.Name, issi));
                    }
                }
            }
        }

        public void SetDebugEnabled(bool enabled)
        {
        }

        public void SetEnabled(bool enabled)
        {
            if (this.pModel.Enabled != enabled)
            {
                this.pModel.Enabled = enabled;
            }
        }

        public void SetPluginModel(PluginModel pModel)
        {
            lock (this.lckPModel)
            {
                this.pModel = pModel;
                lock (this.lckHelper)
                {
                    this.sqlfc.ChangConn(pModel.DesIP, pModel.DesPort, pModel.DesCatalog, pModel.DesUser, pModel.DesPwd);
                }
            }
        }
    }
}

