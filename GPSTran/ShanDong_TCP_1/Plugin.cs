using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common;

///山东枣庄
namespace ShanDong_TCP_1
{
    public class Plugin : Protocol,IPlugin
    {
        private ConcurrentQueue<byte[]> dataQueue = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// 保留13位对象id
        /// </summary>
        private ConcurrentDictionary<Int32, Int32> IssiMapping = new ConcurrentDictionary<Int32, Int32>();

        private string username = "eastcom";
        private string password = "eastcom";
        private int isSlow;
        private readonly PluginType pType = PluginType.RemoteIP;
        private PluginModel pModel;
        private readonly Object lckPModel = new Object();
        private int status;
        private long statistics;
        private bool enabled;
        private Thread dealDataThread;
        private int TranLimit;
        private bool isStop;
        private TcpClient Tcp;
        private NetworkStream nws;

        public bool IsStop()
        {
            return isStop;
        }

        public Plugin()
        {

        }

        public Plugin(PluginModel pModel, bool threadStart)
        {
            TranLimit = 1000;
            status = 1;
            isSlow = 0;
            isStop = false;
            statistics = 0;
            enabled = pModel.Enabled;
            //gps = new TGPS();
            this.pModel = pModel;
            if (threadStart)
            {
                dealDataThread = new Thread(Run);
                dealDataThread.Start();
            }
        }

        void InitRefData()
        {
            new Action(() =>
            {
                while (enabled)
                {
                    SQLFactory sqlfc = new SqlHelper(Resource.TranConn);
                    var dt = sqlfc.ExecuteDataReader(CommandType.Text, @"select issi,refissi from C_IssiInfo");
                    var dic = dt.AsEnumerable().ToDictionary(row => Convert.ToInt32(row[0]), row => Convert.ToInt32(row[1]));
                    IssiMapping.Clear();
                    Parallel.ForEach(dic, d => IssiMapping.TryAdd(d.Key, d.Value));
                    Thread.Sleep(Resource.FlushInterval * 3600 * 24);
                }
            }).BeginInvoke(null,null);
        }

        void InitLoopIssiMap()
        {
            const string cresql = @"create table C_IssiInfo(issi int primary key,refissi bigint);";
            SQLFactory sqlfc = new SqlHelper(Resource.TranConn);
            if(sqlfc.ExecuteNonQueryNonTran(CommandType.Text, cresql)>0)
                Logger.Info("表 C_IssiInfo 被创建！");
        }


        private void Run()
        {
            InitLoopIssiMap();
            InitRefData();
            //建立连接
            CreateSocket();
            byte[] data;
            int valid = 0;
            int refssi = 0;
            //<locationinfo  objectid  ="{1}"  event_time="{2}"  sys_time="{3}"
            //longitude="{4}" latitude="{5}" speed="{6}" direction="{7}" starnum= "{8}"
            //altitude="{9}" isvalid="{10}" ison= "{11} " />
            const string msg = "<locationinfo objectid='{0}' event_time='{1}' sys_time='{1}' longitude='{2}' latitude='{3}' speed='{4}' direction='{5}' starnum= '10' altitude='0' isvalid='1' ison='1'/>";
            while (true)
            {
                while (dataQueue.Count > 0 && status > 0)
                {
                    if (dataQueue.TryDequeue(out data) == false)
                    {
                        if (!enabled)
                        {
                            isStop = true;
                            return;
                        }
                        continue;
                    }
                    
                    var gps = ToGPS(data);
                    foreach (var i in pModel.EntityID)
                    {
                        lock (Resource.lckISSIOfEntity)
                        {
                            if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                            {
                                if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(gps.TBody.GetIssi()))
                                {
                                    valid = 1;
                                    break;
                                }
                            }
                        }
                    }
                    if (valid == 0)
                    {
                        continue;
                    }

                    //SendObjectNameToServer(gps.TBody.GetIssi());

                    gps.TBody.ModifyLonLat(pModel.LonOffset, pModel.LatOffset);
                    var issi = Convert.ToInt32(gps.TBody.GetIssi());
                    if (!IssiMapping.TryGetValue(issi,out refssi))
                    {
                        continue;
                    }
                    ///发送对象码，并获取s
                    data = Encoding.UTF8.GetBytes(string.Format(msg, refssi, gps.TBody.GetDateTime(), gps.TBody.lon, gps.TBody.lat,
                        gps.TBody.speed, gps.TBody.dir));

                    try
                    {
                        var ar = nws.BeginWrite(data, 0, data.Length, null, null);
                        if (ar.AsyncWaitHandle.WaitOne())
                        {
                            statistics += 1;
                        }
                        nws.EndWrite(ar);
                        nws.Flush();
                        if (Resource.GPSDebugEnabled)
                        {
                            Logger.Info(String.Format("实例：{0} 号码:{1}-{2}", pModel.Name, issi,refssi));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("error occur when send GPS data with TCP protocol,error message:" + ex);
                        status = 0;
                    }
                    finally
                    {
                        if (status == 0 && enabled)
                        {
                            DisposeSocket();
                            CreateSocket();
                        }
                    }
                }

                if (!enabled)
                {
                    if (dataQueue.Count > 0)
                    {
                        continue;
                    }
                    Logger.Info("Task:IPInstance instance(" + pModel.Name + ") exit safely");
                    isStop = true;
                    DisposeSocket();

                    return;
                }
            }
        }

        private void LoginSend()
        {
            string msg = string.Format("<login username='{0}' password='{1}'/>",username,password);
            try
            {
                var data = Encoding.UTF8.GetBytes(msg);
                Logger.Info(String.Format("实例：{0} 发送注册登录", pModel.Name));
                var ar = nws.BeginWrite(data, 0, data.Length, null, null);
                if (ar.AsyncWaitHandle.WaitOne())
                {
                }
                nws.EndWrite(ar);
                nws.Flush();
                //ReciveLinkMsgFromServer();
                LinkSendMsgToServer();
                //if (nws.CanRead)
                //{
                //    var bytes = new Byte[62];
                //    nws.Read(bytes, 0, bytes.Length);
                //    Logger.Info(String.Format("实例：{0} 收到登录应答包体：{1}", pModel.Name,string.Join(",", bytes)));
                //    nws.Flush();
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("error occur when send GPS data with TCP protocol,error message:" + ex);
                status = 0;
            }
            finally
            {
                if (status == 0 && enabled)
                {
                    DisposeSocket();
                    CreateSocket();
                }
            }
        }

        /// <summary>
        /// 接收服务器消息
        /// </summary>
        //private void ReciveLinkMsgFromServer()
        //{
        //    new Action(() =>
        //    {
        //        var data = new byte[100];
        //        while (enabled)
        //        {
        //            try
        //            {
        //                Thread.Sleep(5000);
        //                if (nws.Read(data, 0, data.Length) <= 0)
        //                {
        //                    continue;
        //                }
        //                var msg = Encoding.UTF8.GetString(data);
        //                msg=msg.Replace("response", "").Replace("\0","");
        //                var xe = XElement.Parse(msg);
        //                if (xe.Attributes().Contains(new XAttribute("existed", 1)))
        //                {
        //                    var id = xe.Attribute("objectid").Value;
        //                    var name = xe.Attribute("name").Value;
        //                    //IssiMapping.AddOrUpdate(name, id, (k, v) => id);
        //                }
        //                else
        //                {
        //                    Logger.Info(string.Format("收到服务端:{0} 消息心跳：{1}", pModel.DesIP, msg));
        //                }
        //            }
        //            catch (Exception ex)
        //            {
                        
        //                Logger.Error("接收异常消息："+ex);
        //            }
                    
                    
        //        }
        //    }).BeginInvoke(null,null);
        //}

        private void LinkSendMsgToServer()
        {
            var msg = string.Format("<link username='{0}' password='{1}'>",username,password);
            new Action(() =>
            {
                var data = Encoding.UTF8.GetBytes(msg);
                while (enabled)
                {
                    Thread.Sleep(20000);
                    nws.Write(data,0,data.Length);
                    nws.Flush();
                    Logger.Info(string.Format("发送至服务端{0} 消息心跳：{1}", pModel.DesIP, msg));
                }
            }).BeginInvoke(null, null);
        }

        /// <summary>
        /// 服务器发送用户名称
        /// </summary>
        /// <param name="name"></param>
        //private void SendObjectNameToServer(string name)
        //{
        //    var msg = string.Format("<registerinfo regionid ='{0}' name='{1}'/>", 2563, name);
        //    var data = Encoding.UTF8.GetBytes(msg);
        //    try
        //    {
        //        var ar = nws.BeginWrite(data, 0, data.Length, null, null);
        //        if (ar.AsyncWaitHandle.WaitOne())
        //        {
        //        }
        //        nws.EndWrite(ar);
        //        nws.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void DisposeSocket()
        {
            status = 0;
            Logger.Error("TCP Fail to connect server");
            if (nws != null)
            {
                nws.Close();
            }

            if (Tcp!=null)
            {
                Tcp.Close();
                Tcp = null;
            }
        }

        private void CreateSocket()
        {
            new Action(() =>
            {
                //尝试连接
                Logger.Info("TCP 发送请求.....");
                Tcp = new TcpClient();
                //Tcp.Connect(pModel.EndPoint);
                var ar = Tcp.BeginConnect(pModel.Ip, pModel.Port, null, Tcp);
                if (ar.AsyncWaitHandle.WaitOne())
                {
                    Tcp = ar.AsyncState as TcpClient;
                    try
                    {
                        Tcp.EndConnect(ar);
                        nws = Tcp.GetStream();
                        LoginSend();
                        status = 1;
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Tcp Reconnect Error:" + e);
                        status = 0;
                    }
                    finally
                    {
                        if (status == 0 && enabled)
                        {
                            DisposeSocket();
                            Thread.Sleep(60000 * 5);
                            CreateSocket();
                        }
                    }
                }
            }).BeginInvoke(null, null);
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
        }

        public void Execute(byte[] data)
        {
            if (TranLimit >= Resource.MaxTranLimit)
            {
                isSlow = 1;
            }
            else
            {
                dataQueue.Enqueue(data);
                isSlow = 0;

            }

        }

        public void SetPluginModel(PluginModel pModel)
        {
            lock (lckPModel)
            {
                this.pModel = pModel;
                enabled = pModel.Enabled;
            }
        }
    }
}

