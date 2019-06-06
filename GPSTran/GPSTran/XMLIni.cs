using System;
using System.Collections.Generic;
using System.Xml;
using Common;
using Microsoft.Win32;
using System.Windows.Forms;

/**
 * 配置文件相关处理类
 * 提供读取和修改配置文件的接口
 **/ 

namespace GPSTran
{
   public class XMLIni
    {
        //public readonly static string[] ProtocolList = new string[] {"PGIS"};
        //public readonly static string[] NetProtocolList = new string[] {"UDP"};
        private string Path;
        private XmlDocument Document= new XmlDocument();
        public XMLIni() 
        {
            this.Path = AppDomain.CurrentDomain.BaseDirectory+"Tran.config";
            try
            {
               
                Document.Load(Path);
               
            }
            catch (Exception ex) 
            {
                throw ex;
            
            }
        }
        public XMLIni(string Path) 
        {

            this.Path = Path;
            try
            {
                Document.Load(Path);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
     
       //读取log4net（日志组件）的配置文件
        public int InitLogPath()
        {
            XmlDocument docu = new XmlDocument();

            try
            {
                docu.Load(AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config");

            }
            catch (Exception ex)
            {
                Logger.Error("can't find log config file,log file path:" + AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config");
                Logger.Error(ex.ToString());
                return 0;
            }
            //init log file format and file pattern
            try
            {
                XmlNodeList nodeList = docu.SelectSingleNode("log4net").SelectSingleNode("appender").SelectNodes("param");
                int isMatch = 0;
                foreach (XmlNode nd in nodeList)
                {
                    if (nd.Attributes["name"].Value.ToUpper().Equals("DATEPATTERN"))
                    {
                        string s = nd.Attributes["value"].Value;
                        Resource.logFileFormat = s.Substring(s.LastIndexOf("."));
                        Resource.logFilePattern = s.Substring(0, s.LastIndexOf("."));

                        isMatch = 1;
                        break;
                    }

                }

                if (isMatch == 0) 
                {
                    Logger.Error("can't find node: <param name=\"DatePattern\" value=\"\" />" );
                    return 0;
                
                }


            }
            catch (Exception ex)
            {
                Logger.Error("can't find node: <param name=\"DatePattern\" value=\"\" />,error message:" + ex.ToString());
                return 0;

            }


            //init log file path
            try
            {
                XmlNodeList nodeList = docu.SelectSingleNode("log4net").SelectSingleNode("appender").SelectNodes("param");
                int isMatch = 0;
                foreach (XmlNode nd in nodeList)
                {
                    if (nd.Attributes["name"].Value.ToUpper().Equals("FILE"))
                    {
                        isMatch = 1;
                        Resource.logPath = AppDomain.CurrentDomain.BaseDirectory + nd.Attributes["value"].Value.Substring(0, nd.Attributes["value"].Value.LastIndexOf("\\"));
                        break;
                    }

                }
                if (isMatch == 0)
                {
                    Logger.Error("can't find node: <param name=\"File\" value=\"\" />");
                    return 0;

                }

            }
            catch (Exception ex)
            {
                Logger.Error("can't find node: <param name=\"File\" value=\"\" />,error message:" + ex.ToString());
                return 0;

            }

            return 1;

        }
      
       //初始化一般配置
        public int InitGeneral() 
        {
            try
            {
                XmlNode generalNode = Document.SelectSingleNode("configuration").SelectSingleNode("General");
                string s = generalNode.SelectSingleNode("Language").Attributes["value"].Value;

                if (s.ToString().ToUpper().Equals(Language.en_us.ToString().ToUpper())) 
                {
                    Resource.lHelper = new LanguageHelper(Language.en_us, AppDomain.CurrentDomain.BaseDirectory + "language//en_us.xml");
                    MessageBoxManager.OK = Resource.lHelper.Key("n37");
                    MessageBoxManager.Yes = Resource.lHelper.Key("n63");
                    MessageBoxManager.No = Resource.lHelper.Key("n64");
                    MessageBoxManager.Register();
                }
                else if (s.ToString().ToUpper().Equals(Language.zh_cn.ToString().ToUpper()))
                {
                    Resource.lHelper = new LanguageHelper(Language.zh_cn, AppDomain.CurrentDomain.BaseDirectory + "language//zh_cn.xml");
                    MessageBoxManager.OK = Resource.lHelper.Key("n37");
                    MessageBoxManager.Yes = Resource.lHelper.Key("n63");
                    MessageBoxManager.No = Resource.lHelper.Key("n64");
                    MessageBoxManager.Register();
                }
                else 
                {
                    Logger.Warn("unknown language:"+s+", language is set default zh_cn");
                    Resource.lHelper = new LanguageHelper(Language.zh_cn, AppDomain.CurrentDomain.BaseDirectory + "language//zh_cn.xml");
                    MessageBoxManager.OK = Resource.lHelper.Key("n37");
                    MessageBoxManager.Yes = Resource.lHelper.Key("n63");
                    MessageBoxManager.No = Resource.lHelper.Key("n64");
                    MessageBoxManager.Register();
                }
                string ss = generalNode.SelectSingleNode("LogKeepDays").Attributes["value"].Value;
                Resource.logKeepDays = Convert.ToInt32(ss);



            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when init General attributes from config file,error message:"+ex.ToString());
                return 0;
            }
            return 1;
        
        }
       //检查是否开机启动
        public int CheckAutoRun() 
        {
            try
            {
             
                //register auto run when operation system start

                RegistryKey Local = Registry.LocalMachine;
                RegistryKey runKey = Local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\");

                if (runKey.GetValue("EastcomGPSTran") == null)
                {
                    Resource.IsAutoRun = false;

                }
                else
                {
                    Resource.IsAutoRun = true;
                }


            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when check auto run,error message:" + ex.ToString());
                return 0;
            }
            return 1;
        
        }

       //初始化webgis数据库配置
        public int InitWebGis()
        {
            try
            {
                XmlNode WebGisNode = Document.SelectSingleNode("configuration").SelectSingleNode("WebGis");
                Resource.WebGisIp = WebGisNode.SelectSingleNode("IP").Attributes["value"].Value.ToString().Trim();
                Resource.WebGisDb = WebGisNode.SelectSingleNode("Catalog").Attributes["value"].Value.ToString().Trim();
                Resource.WebGisUser = WebGisNode.SelectSingleNode("User").Attributes["value"].Value.ToString().Trim();
                Resource.WebGisPwd = WebGisNode.SelectSingleNode("Pwd").Attributes["value"].Value.ToString().Trim();
                Resource.WebGisInstance = WebGisNode.SelectSingleNode("Instance").Attributes["value"].Value.ToString().Trim();
                Resource.WebGisPort = Convert.ToInt32(WebGisNode.SelectSingleNode("Port").Attributes["value"].Value.ToString().Trim());

                if (Resource.WebGisDb.ToUpper().Equals("DAGDB_TRAN"))
                {
                    Logger.Error("转发数据库库名称和webGis数据库名称不能同为Dagdb_Tran");
                }

            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when init WebGis attributes from config file:"+ex.ToString());
                return 0;
            }
            return 1;
        }
       //初始化转发数据库配置
        public int InitTran() 
        {
            try
            {
                XmlNode WebGisNode = Document.SelectSingleNode("configuration").SelectSingleNode("Tran");
                Resource.TranIp = WebGisNode.SelectSingleNode("IP").Attributes["value"].Value.ToString().Trim();
                Resource.TranDb = WebGisNode.SelectSingleNode("Catalog").Attributes["value"].Value.ToString().Trim();
                Resource.TranUser = WebGisNode.SelectSingleNode("User").Attributes["value"].Value.ToString().Trim();
                Resource.TranPwd = WebGisNode.SelectSingleNode("Pwd").Attributes["value"].Value.ToString().Trim();
                Resource.TranInstance = WebGisNode.SelectSingleNode("Instance").Attributes["value"].Value.ToString().Trim();
                Resource.TranPort = Convert.ToInt32(WebGisNode.SelectSingleNode("Port").Attributes["value"].Value.ToString().Trim());

                if (Resource.TranDb.ToUpper().Equals("DAGDB")) 
                {
                    Logger.Error("转发数据库库名称和webGis数据库名称不能同为Dagdb");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when init Tran attributes from config file,error message:"+ex.ToString());
                return 0;
            }
            return 1;
        
        }
       //初始化用户表配置
        public int InitUserInfo() 
        {
            try
            {
                XmlNode Userinfo = Document.SelectSingleNode("configuration").SelectSingleNode("UserInfo");
                string s = Userinfo.Attributes["enabled"].Value.ToString().Trim().ToUpper();
                if (s.Equals("TRUE"))
                {
                    Resource.UserinfoEnabled = true;
                }
                else 
                {
                    Logger.Warn("Userinfo maintenance enabled is false");
                    Resource.UserinfoEnabled = false;
                }
                string s2 = Userinfo.SelectSingleNode("FlushInterval").Attributes["value"].Value.ToString().Trim();
                try
                {
                    Resource.FlushInterval = Convert.ToInt32(s2);
                }
                catch (Exception ex) 
                {
                    Resource.FlushInterval = 10;
                    Logger.Warn("FlushInterval can't convert FlushInterval to int,set FlushInterval default 10:"+ex.ToString());
                
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when init Tran attributes from config file,error message:"+ex.ToString());
                return 0;
            }
            return 1;
        }
       //初始化IP转发配置
        public int InitIPTransfer() 
        {
            Resource.IPInstanceList.Clear();
            Resource.IPList.Clear();
            StatusDetect detect = new StatusDetect();
            try
            {
                XmlNodeList IPInstanceNode = Document.SelectSingleNode("configuration").SelectSingleNode("IPTransfer").SelectNodes("Instance");
                
                
                foreach (XmlNode node in IPInstanceNode) 
                {
                    IPModel ipModel = new IPModel();
                    try
                    {
                        ipModel.Name = node.Attributes["name"].Value.ToString().Trim();


                        if (string.IsNullOrEmpty(ipModel.Name))
                        {
                            Logger.Error("IPInstance name can't be null or empty");
                            return 0;
                        }
                        if (CheckIPInstanceName(ipModel.Name))
                        {
                            Logger.Warn("duplicated IPInstance name,followed will be ignored");
                            continue;

                        }

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init IPInstance attributes(name) from config file:"+ex.ToString());
                        return 0; 
                    }
                    try
                    {
                        string s = node.Attributes["enabled"].Value.ToString().Trim().ToUpper();
                        if (s.Equals("TRUE"))
                        {
                            ipModel.Enabled = true;
                        }
                        else
                        {
                            ipModel.Enabled = false;
                        }
                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init IPInstance attributes(enabled) from config file:"+ex.ToString());
                        return 0; 
                    }
                    //try
                    //{
                    //    ipModel.Protocol = node.SelectSingleNode("Protocol").Attributes["value"].Value.ToString().Trim().ToUpper();
                        

                    //}
                    //catch (Exception ex) 
                    //{
                    //    Logger.Error("Error occur when init IPInstance attributes(Protocol) from config file:"+ex.ToString());
                    //    return 0; 
                    //}
                    try
                    {
                        ipModel.Ip = node.SelectSingleNode("IP").Attributes["value"].Value.ToString().Trim();
                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init IPInstance attributes(IP) from config file:"+ex.ToString());
                        return 0; 
                    }
                    try
                    {
                        ipModel.Port = Convert.ToInt32(node.SelectSingleNode("Port").Attributes["value"].Value.ToString().Trim());

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init IPInstance attributes(Port) from config file:"+ex.ToString());
                        return 0; 
                    
                    }
                    //try
                    //{
                    //    ipModel.NetProtocol = node.SelectSingleNode("NetProtocol").Attributes["value"].Value.ToString().Trim().ToUpper();
                        
                    //}
                    //catch (Exception ex) 
                    //{
                    //    Logger.Error("Error occur when init IPInstance attributes(NetProtocol) from config file:"+ex.ToString());
                    //    return 0; 
                    //}

                    try
                    {
                        ipModel.LonOffset = Convert.ToDouble(node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim());

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init IPInstance attributes(LonOffset) from config file:"+ex.ToString());
                        return 0; 
                    
                    }

                    try
                    {
                        ipModel.LatOffset = Convert.ToDouble(node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim());

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init IPInstance attributes(LatOffset) from config file:"+ex);
                        return 0; 
                    }

                    try
                    {
                        XmlNodeList ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                        List<int> EntityList = new List<int>();
                        foreach(XmlNode no in ls)
                        {
                            int id = Convert.ToInt32(no.Attributes["value"].Value.ToString().Trim());
                            EntityList.Add(id);
                        }

                        ipModel.EntityID = EntityList;
                    
                    }catch(Exception ex)
                    {
                         Logger.Error("Error occur when init IPInstance attributes(EntityID) from config file:"+ex.ToString());
                        return 0; 
                    }
                    //if (!ProtocolList.Contains(ipModel.Protocol)) 
                    //{
                    //    Logger.Error("unknown Protocol:"+ipModel.Protocol);
                    //    return 0;
                    //}
                    //if (!NetProtocolList.Contains(ipModel.NetProtocol)) 
                    //{
                    //    Logger.Error("unknown NetProtocol:" + ipModel.NetProtocol);
                    //    return 0;
                    //}

                    if (detect.IpDetect(ipModel.EndPoint)) 
                    {
                        ipModel.IpAvailable = true;
                    
                    }else
                    {
                        ipModel.IpAvailable=false;
                    }
                    if (!Resource.IPList.ContainsKey(ipModel.Name))
                    {
                        Resource.IPList.Add(ipModel.Name, new IPManager(ipModel,false));
                    }
                    else 
                    {
                        Logger.Warn("duplicated IPInstance name,this IPInstance will be ignored");
                    }
                    //detect duplicated instance name
                   if(!Resource.IPInstanceList.ContainsKey(ipModel.Name)&&ipModel.Enabled==true)
                   {
                       int flag = 0;

                        foreach( string s2 in Resource.IPInstanceList.Keys)
                        {
                            //detect duplicated ip and port
                            if(Resource.IPInstanceList[s2].GetModel().Ip.Equals(ipModel.Ip)&&Resource.IPInstanceList[s2].GetModel().Port==ipModel.Port)
                            {
                                Logger.Warn("duplicated instance ip address,followed will be ingored,ip and port:"+ipModel.Ip+":"+ipModel.Port.ToString());
                                flag = 1;
                                break;
                             }

                        }
                        if (flag == 1) 
                        {
                            continue;
                        }

                        Resource.IPInstanceList.Add(ipModel.Name, new IPManager(ipModel, true));
                        Logger.Info("detect ip address to transfer GPS data,ip address:" + ipModel.Ip + ":" + ipModel.Port.ToString());
                    }else
                    {
                        Logger.Warn("duplicated instance name or enabled=false,instance name:" + ipModel.Name);
                    }
                }
            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when init IPInstance attributes from config file:"+ex.ToString());
                return 0;
            }
         



            return 1;
        }
       //初始化数据库转发配置
        public int InitDBTransfer() 
        {
            Resource.DBInstanceList.Clear();
            Resource.DBList.Clear();
            try
            {
                XmlNodeList DBInstanceNode = Document.SelectSingleNode("configuration").SelectSingleNode("DBTransfer").SelectNodes("Instance");
                foreach (XmlNode node in DBInstanceNode) 
                {
                    DBModel dbModel = new DBModel();

                    try
                    {
                        dbModel.Name = node.Attributes["name"].Value.ToString().Trim();
                        if (string.IsNullOrEmpty(dbModel.Name))
                        {
                            Logger.Error("DBInstance name can't be null or empty");
                            return 0;
                        }
                        if (CheckDBInstanceName(dbModel.Name))
                        {
                            Logger.Warn("duplicated DBInstance name,followed will be ignored");
                            continue;

                        }

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init DBInstance attributes(name) from config file:"+ex.ToString());
                        return 0;
                    
                    }

                    try
                    {
                        string s = node.Attributes["enabled"].Value.ToString().Trim().ToUpper();
                        if (s.Equals("TRUE"))
                        {
                            dbModel.Enabled = true;
                        }
                        else 
                        {
                            dbModel.Enabled = false;
                        }
                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init DBInstance attributes(enabled) from config file:"+ex.ToString());
                        return 0;
                    }

                    try 
                    {
                        dbModel.TableName = node.SelectSingleNode("TableName").Attributes["value"].Value.ToString().Trim().ToUpper();

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error occur when init DBInstance attributes(TableName) from config file:"+ex.ToString());
                        return 0;
                    }

                     try
                    {
                        dbModel.LonOffset = Convert.ToDouble(node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim());

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init DBInstance attributes(LonOffset) from config file:"+ex.ToString());
                        return 0; 
                    
                    }

                    try
                    {
                        dbModel.LatOffset = Convert.ToDouble(node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim());

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("Error occur when init DBInstance attributes(LatOffset) from config file:"+ex.ToString());
                        return 0; 
                    
                    }

                    try
                    {
                        string s = node.SelectSingleNode("MaxCount").Attributes["value"].Value.ToString().Trim();
                        dbModel.MaxCount = Convert.ToInt32(s);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("error occur when read init DBTransfer attributes(maxCount) from config file,invalid maxCount value:"+ex.ToString());
                    }

                    try
                    {
                        XmlNodeList ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                        List<int> EntityList = new List<int>();
                        foreach (XmlNode no in ls)
                        {
                            int id = Convert.ToInt32(no.Attributes["value"].Value.ToString().Trim());
                            EntityList.Add(id);
                        }

                        dbModel.EntityID = EntityList;

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error occur when init IPInstance attributes(EntityID) from config file:"+ex.ToString());
                        return 0;
                    }

                    if (!Resource.DBList.ContainsKey(dbModel.Name))
                    {
                        Resource.DBList.Add(dbModel.Name, new DBManager(dbModel,false));
                    }
                    else 
                    {
                        Logger.Warn("duplicated DBInstance name,this DBInstance will be ignored"); 
                    }
                    //detect duplicated instance name
                    if ((!Resource.DBInstanceList.ContainsKey(dbModel.Name))&&dbModel.Enabled == true)
                    {
                        int flag = 0;
                        foreach(DBManager dm in Resource.DBInstanceList.Values)
                        {
                            if (dm.GetModel().TableName.ToUpper().Equals(dbModel.TableName.ToUpper())) 
                            {
                                Logger.Warn("duplicated DBInstance tablename table name:" + dbModel.TableName);
                                flag = 1;
                                break;
                            
                            }

                        }
                        if (flag == 1) 
                        {
                            continue;
                        }

                        Resource.DBInstanceList.Add(dbModel.Name, new DBManager(dbModel, true));
                        Logger.Info("detect table to insert GPS data,tablename:" + dbModel.TableName+" instance name:"+dbModel.Name);
                    }
                    else
                    {
                        Logger.Warn("duplicated DBInstance name or enabled=false,instance name:" +dbModel.Name);
                    }
                
                }

            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when init DBInstance attributes from config file:" + ex.ToString());
                return 0;
            }

            return 1;
        }
       //初始化插件转发配置
        public int InitPlugin() 
        {
            Resource.PluginInstanceList.Clear();
            Resource.PluginList.Clear();
            try
            {
                XmlNodeList PluginList = Document.SelectSingleNode("configuration").SelectSingleNode("Plugin").SelectNodes("Instance");
                //todo : init PluginInstanceList,init PluginList
                foreach (XmlNode node in PluginList) 
                {
                    PluginModel pModel = new PluginModel();
                   
                    try
                    {
                        pModel.Name = node.Attributes["name"].Value.Trim();
                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("error occur when init Plugin attribute(name) from config file:" + ex.ToString());
                        return 0;
                    }
                    try
                    {
                        string s = node.Attributes["enabled"].Value.Trim().ToUpper();
                        if (s.Equals("TRUE"))
                        {
                            pModel.Enabled = true;
                        }
                        else 
                        {
                            pModel.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("error occur when init Plugin attribute(enabled) from config file:" + ex.ToString());
                        return 0;
                    }
                    try 
                    {
                        string s = node.SelectSingleNode("Dll").Attributes["name"].Value.Trim();
                        if (!Resource.DllList.ContainsKey(s))
                        {
                            Logger.Warn("unknown Dll name,this plugin instance will be ignored");
                            continue;
                        }
                        else 
                        {
                            pModel.DllModel = Resource.DllList[s];
                        
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("error occur when init Plugin attribute(Dll) from config file:" + ex.ToString());
                        return 0;
                    }

                    if (pModel.DllModel.PType == PluginType.RemoteIP)
                    {
                        //try
                        //{
                        //    pModel.Protocol = node.SelectSingleNode("Protocol").Attributes["value"].Value.ToString().Trim().ToUpper();
                        //}
                        //catch (Exception ex)
                        //{
                        //    Logger.Error("Error occur when init IPInstance attributes(Protocol) from config file:" + ex.ToString());
                        //    return 0;
                        //}
                        try
                        {
                            pModel.Ip = node.SelectSingleNode("IP").Attributes["value"].Value.ToString().Trim();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init IPInstance attributes(IP) from config file:" + ex.ToString());
                            return 0;
                        }
                        try
                        {
                            pModel.Port = Convert.ToInt32(node.SelectSingleNode("Port").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init IPInstance attributes(Port) from config file:" + ex.ToString());
                            return 0;

                        }
                        //try
                        //{
                        //    pModel.NetProtocol = node.SelectSingleNode("NetProtocol").Attributes["value"].Value.ToString().Trim().ToUpper();

                        //}
                        //catch (Exception ex)
                        //{
                        //    Logger.Error("Error occur when init IPInstance attributes(NetProtocol) from config file:" + ex.ToString());
                        //    return 0;
                        //}

                        try
                        {
                            pModel.LonOffset = Convert.ToDouble(node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init IPInstance attributes(LonOffset) from config file:" + ex.ToString());
                            return 0;

                        }

                        try
                        {
                            pModel.LatOffset = Convert.ToDouble(node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init IPInstance attributes(LatOffset) from config file:" + ex.ToString());
                            return 0;
                        }

                        try
                        {
                            XmlNodeList ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                            List<int> EntityList = new List<int>();
                            foreach (XmlNode no in ls)
                            {
                                int id = Convert.ToInt32(no.Attributes["value"].Value.ToString().Trim());
                                EntityList.Add(id);
                            }

                            pModel.EntityID = EntityList;

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init IPInstance attributes(EntityID) from config file:" + ex.ToString());
                            return 0;
                        }
                        //if (!ProtocolList.Contains(pModel.Protocol))
                        //{
                        //    Logger.Error("unknown Protocol:" + pModel.Protocol);
                        //    return 0;
                        //}
                        //if (!NetProtocolList.Contains(pModel.NetProtocol))
                        //{
                        //    Logger.Error("unknown NetProtocol:" + pModel.NetProtocol);
                        //    return 0;
                        //}
                        if (!Resource.PluginList.ContainsKey(pModel.Name))
                        {
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                               
                        }
                        else
                        {
                            Logger.Warn("duplicated PluginInstance name,this DBInstance will be ignored");
                            continue;
                        }

                        //detect duplicated instance name
                        if (!Resource.PluginInstanceList.ContainsKey(pModel.Name) && pModel.Enabled == true)
                        {
                            int flag = 0;

                            foreach (string s2 in Resource.PluginInstanceList.Keys)
                            {
                                //detect duplicated ip and port
                                if (Resource.PluginInstanceList[s2].GetPluginModel().DllModel.PType == PluginType.RemoteIP)
                                {
                                    if (Resource.PluginInstanceList[s2].GetPluginModel().Ip.Equals(pModel.Ip) && Resource.PluginInstanceList[s2].GetPluginModel().Port == pModel.Port)
                                    {
                                        Logger.Warn("duplicated Plugin instance ip address against itself,followed will be ingored,ip and port:" + pModel.Ip + ":" + pModel.Port.ToString());
                                        flag = 1;
                                        break;
                                    }
                                }
                            }
                            foreach (string s2 in Resource.IPInstanceList.Keys)
                            {
                                //detect duplicated ip and port
                                if (Resource.IPInstanceList[s2].GetModel().Ip.Equals(pModel.Ip) && Resource.IPInstanceList[s2].GetModel().Port == pModel.Port)
                                {
                                    Logger.Warn("duplicated instance ip address against IPInstance,followed will be ingored,ip and port:" + pModel.Ip + ":" + pModel.Port.ToString());
                                    flag = 1;
                                    break;
                                }

                            }


                            if (flag == 1)
                            {
                                continue;
                            }

                            try
                            {

                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));

                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when init PluginManager,instance name:" + pModel.Name + " ,error message:" + ex.ToString());
                                return 0;
                            }
                            Logger.Info("detect RemoteIP Plugin address to transfer GPS data,ip address:" + pModel.Ip + ":" + pModel.Port.ToString());

                        }
                        else
                        {

                            Logger.Warn("duplicated instance name or enabled=false,instance name:" + pModel.Name);

                        }




                    }
                    else if(pModel.DllModel.PType==PluginType.RemoteDB)
                    {
                        try
                        {
                            pModel.TableName = node.SelectSingleNode("TableName").Attributes["value"].Value.ToString().Trim().ToUpper();

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init PluginInstance attributes(TableName) from config file:" + ex.ToString());
                            return 0;
                        }

                        try
                        {
                            pModel.LonOffset = Convert.ToDouble(node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init PluginInstance attributes(LonOffset) from config file:" + ex.ToString());
                            return 0;

                        }

                        try
                        {
                            pModel.LatOffset = Convert.ToDouble(node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error occur when init PluginInstance attributes(LatOffset) from config file:{0}",ex.ToString()));
                            return 0;

                        }

                        try
                        {
                            string s = node.SelectSingleNode("MaxCount").Attributes["value"].Value.ToString().Trim();
                            pModel.MaxCount = Convert.ToInt32(s);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("error occur when read init PluginInstance attributes(MaxCount) from config file,invalid MaxCount value:{0}",ex.ToString()));
                        }



                        try
                        {
                            XmlNodeList ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                            List<int> EntityList = new List<int>();
                            foreach (XmlNode no in ls)
                            {
                                int id = Convert.ToInt32(no.Attributes["value"].Value.ToString().Trim());
                                EntityList.Add(id);
                            }

                            pModel.EntityID = EntityList;

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error occur when init PluginInstance attributes(EntityID) from config file:{0}",ex.ToString()));
                            return 0;
                        }

                        try
                        { 
                            pModel.DesIP = node.SelectSingleNode("IP").Attributes["value"].Value.ToString().Trim();
                            pModel.DesCatalog = node.SelectSingleNode("Catalog").Attributes["value"].Value.ToString().Trim();
                            pModel.DesUser = node.SelectSingleNode("User").Attributes["value"].Value.ToString().Trim();
                            pModel.DesPwd = node.SelectSingleNode("Pwd").Attributes["value"].Value.ToString().Trim();
                            pModel.DesInstance = node.SelectSingleNode("Instance").Attributes["value"].Value.ToString().Trim();
                            pModel.DesPort = Convert.ToInt32(node.SelectSingleNode("Port").Attributes["value"].Value.ToString().Trim());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error occur when init PluginInstance attributes(remote DB address) from config file:{0}",ex.ToString()));
                            return 0;
                        }



                        if (!Resource.PluginList.ContainsKey(pModel.Name))
                        {
                            Resource.PluginList.Add(pModel.Name, new PluginManager(pModel, false));
                              
                        }
                        else
                        {
                            Logger.Warn("duplicated PluginInstance name,this DBInstance will be ignored");
                            continue;
                        }
                        //detect duplicated instance name
                        if ((!Resource.PluginInstanceList.ContainsKey(pModel.TableName)) && pModel.Enabled == true)
                        {

                            try
                            {

                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));

                            }
                            catch (Exception ex)
                            {
                                Logger.Error("error occur when init PluginManager,instance name:" + pModel.Name + " ,error message:" + ex.ToString());
                                return 0;
                            }
                            Logger.Info("detect RemoteDB Instance table to insert GPS data,tablename:" + pModel.TableName);
                        }
                        else
                        {

                            Logger.Warn("duplicated PluginInstance name or enabled=false,instance name:" + pModel.TableName);

                        }



                    }
                    else if (pModel.DllModel.PType== PluginType.TranDB)
                    {
                        try
                        {
                            pModel.TableName = node.SelectSingleNode("TableName").Attributes["value"].Value.ToString().Trim().ToUpper();

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error occur when init PluginInstance attributes(TableName) from config file:{0}",ex.ToString()));
                            return 0;
                        }

                        try
                        {
                            pModel.LonOffset = Convert.ToDouble(node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error occur when init PluginInstance attributes(LonOffset) from config file:{0}",ex.ToString()));
                            return 0;

                        }

                        try
                        {
                            pModel.LatOffset = Convert.ToDouble(node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim());

                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Error occur when init PluginInstance attributes(LatOffset) from config file:" + ex.ToString());
                            return 0;

                        }

                        try
                        {
                            string s = node.SelectSingleNode("MaxCount").Attributes["value"].Value.ToString().Trim();
                            pModel.MaxCount = Convert.ToInt32(s);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("error occur when read init PluginInstance attributes(MaxCount) from config file,invalid MaxCount value:\n"+ ex.ToString());
                        }

                        if (node.SelectSingleNode("CityCode") != null)
                        {
                            pModel.Citycode =
                                Convert.ToInt32(node.SelectSingleNode("CityCode").Attributes["value"].Value.Trim());
                        }

                        try
                        {
                            XmlNodeList ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                            List<int> EntityList = new List<int>();
                            foreach (XmlNode no in ls)
                            {
                                int id = Convert.ToInt32(no.Attributes["value"].Value.ToString().Trim());
                                EntityList.Add(id);
                            }

                            pModel.EntityID = EntityList;

                        }
                        catch (Exception ex)
                        {
                            Logger.Error(string.Format("Error occur when init PluginInstance attributes(EntityID) from config file:{0}",ex.ToString()));
                            return 0;
                        }

                        pModel.DesIP = Resource.TranIp;
                        pModel.DesCatalog = Resource.TranDb;
                        pModel.DesUser = Resource.TranUser;
                        pModel.DesPwd = Resource.TranPwd;
                        pModel.DesPort = Resource.TranPort;
                        pModel.DesInstance = Resource.TranInstance;



                        if (!Resource.PluginList.ContainsKey(pModel.Name))
                        {
                            Resource.PluginList.Add(pModel.Name,new  PluginManager(pModel, false));
                        }
                        else
                        {
                            Logger.Warn("duplicated PluginInstance name,this Instance will be ignored");
                            continue;
                        }
                        //detect duplicated instance name
                        if ((!Resource.PluginInstanceList.ContainsKey(pModel.TableName)) && pModel.Enabled == true)
                        {
                            try
                            {

                                Resource.PluginInstanceList.Add(pModel.Name, new PluginManager(pModel, true));

                            }
                            catch (Exception ex) 
                            {
                                Logger.Error("error occur when init PluginManager,instance name:"+pModel.Name+" ,error message:"+ex.ToString());
                                return 0;
                            }
                            Logger.Info("detect TranDB Plugin Instance table to insert GPS data,tablename:" + pModel.TableName);
                        }
                        else
                        {

                            Logger.Warn("duplicated  PluginInstance name or enabled=false,instance name:" + pModel.TableName);

                        }

                    }
                    else 
                    {
                        Logger.Warn("unknown Plugin type,this plugin instance will be ignored");
                        
                    }

                }



            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when init Plugin attributes from config file:" + ex.ToString());
                return 0;
            }



            return 1;
        }
       //method must be executed before InitPlugin
        public int InitPluginDll()
        {

            Resource.DllList.Clear();
            try
            {
                XmlNodeList PluginList = Document.SelectSingleNode("configuration").SelectSingleNode("PluginList").SelectNodes("Dll");
                foreach (XmlNode node in PluginList)
                {
                    DllModel dModel = new DllModel();

                    try
                    {
                        dModel.Name = node.Attributes["name"].Value.ToString().Trim();
                        if (string.IsNullOrEmpty(dModel.Name))
                        {
                            Logger.Error("PluginDll name can't be null or empty");
                            return 0;
                        }
                        if (Resource.DllList.ContainsKey(dModel.Name))
                        {
                            Logger.Warn("duplicated PluginDll name,followed will be ignored");
                            continue;
                        }



                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error occur when init PluginDll attributes(name) from config file:" + ex.ToString());
                        return 0;
                    }

                    try
                    {
                        dModel.MainDllPath = AppDomain.CurrentDomain.BaseDirectory+node.Attributes["path"].Value.ToString().Trim();
                        if (string.IsNullOrEmpty(dModel.MainDllPath))
                        {
                            Logger.Error("PluginDll path can't be null or empty");
                            return 0;

                        }
                        //check path format
                        if (!dModel.MainDllPath.Substring(dModel.MainDllPath.LastIndexOf(".") + 1).Equals("dll"))
                        {
                            Logger.Error("invalid PluginDll path");
                            return 0;
                        }
                        if (!System.IO.File.Exists(dModel.MainDllPath))
                        {
                            Logger.Error(dModel.MainDllPath+":PluginDll path can not be found");
                            return 0;
                        }


                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error occur when init PluginDll attributes(path) from config file:" + ex.ToString());
                        return 0;
                    }
                    try
                    {
                        PluginHelper helper = new PluginHelper(dModel.MainDllPath);
                        dModel.PType = helper.GetPluginType();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error occur when get Plugin type using PluginHelper,error message:" + ex.ToString());
                        return 0;
                    }

                    try
                    {
                        XmlNodeList ls = node.SelectSingleNode("Depend").SelectNodes("Add");
                        List<string> DependList = new List<string>();
                        foreach (XmlNode no in ls)
                        {
                            string dependPath = AppDomain.CurrentDomain.BaseDirectory+ no.Attributes["path"].Value.ToString().Trim();
                            if (string.IsNullOrEmpty(dependPath))
                            {
                                Logger.Error("Depend Dll path can't be null or empty");
                                return 0;

                            }
                            //check path format
                            if (!dependPath.Substring(dependPath.LastIndexOf(".") + 1).Equals("dll"))
                            {
                                Logger.Error("invalid Depend Dll path");
                                return 0;
                            }
                            if (!System.IO.File.Exists(dependPath))
                            {
                                Logger.Error("Depend Dll path can not be found");
                                return 0;
                            }
                            DependList.Add(dependPath);

                        }

                        dModel.DependDllPath = DependList;

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error occur when init PluginDll depend dll from config file:"+ex.ToString());
                        return 0;
                    }

                    if (Resource.DllList.ContainsKey(dModel.Name))
                    {
                        Logger.Warn("duplicated Dll name,this plugin Dll will be ignored");
                        
                    }
                    else
                    {
                        //detect duplicated dll path
                        int flag=0;
                        foreach (DllModel dm in Resource.DllList.Values) 
                        {
                            if (dm.MainDllPath.Equals(dModel.MainDllPath)) 
                            {
                                Logger.Warn("duplicated plugin Dll path,this plugin Dll will be ignored,Dll path :"+dModel.MainDllPath);
                                flag = 1;
                                break;
                            }
                        
                        }
                        //if flag==1,this plugin dll will be ignored
                        if (flag == 1) 
                        {
                            continue;
                        }

                        Resource.DllList.Add(dModel.Name, dModel);


                    }
                }


            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when init PluginDll attributes from config file:"+ex.ToString());
                return 0;
            
            }




            return 1;
        }

   //初始化监听端口配置
        public int InitListeningPort() 
        {
            try
            {
                XmlNode ListeningPortNode = Document.SelectSingleNode("configuration").SelectSingleNode("ListeningPort");
                Resource.ListeningPort = Convert.ToInt32(ListeningPortNode.Attributes["value"].Value.ToString().Trim());
            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when init ListeningPort from config file,error message:" + ex.ToString());
                return 0;
            
            }
            return 1;
        }
       //初始化连接字符串
        public int InitConnStrings() 
        {
            if (string.IsNullOrEmpty(Resource.WebGisIp) || string.IsNullOrEmpty(Resource.WebGisDb) || string.IsNullOrEmpty(Resource.WebGisUser) || string.IsNullOrEmpty(Resource.WebGisPwd)) 
            {
                Logger.Error("can't init WebGisConn because some arguments is null or empty");
                return 0;
            }
            if (string.IsNullOrEmpty(Resource.TranIp) || string.IsNullOrEmpty(Resource.TranDb) || string.IsNullOrEmpty(Resource.TranUser) || string.IsNullOrEmpty(Resource.TranPwd))
            {
                Logger.Error("can't init TranConn because some arguments is null or empty");
                return 0;
            }


            Resource.WebGisConn = string.Format("Data Source={0},{1}\\{2};Initial Catalog={3};uid={4};pwd={5};pooling=true;min pool size =1;max pool size=50", Resource.WebGisIp, Resource.WebGisPort, Resource.WebGisInstance, Resource.WebGisDb, Resource.WebGisUser, Resource.WebGisPwd);

            Resource.TranConn = string.Format("Data Source={0},{1}\\{2};Initial Catalog={3};uid={4};pwd={5};pooling=true;min pool size =1;max pool size=50", Resource.TranIp, Resource.TranPort, Resource.TranInstance, Resource.TranDb, Resource.TranUser, Resource.TranPwd);
            
            Logger.Info("Init WebGis connection,ConnectionString:"+Resource.WebGisConn);
            Logger.Info("Init Tran connection,ConnectionString:" + Resource.TranConn);
            
            
            
            return 1;
        }

        private bool CheckIPInstanceName(string name) 
        {
            foreach (IPManager ipManager in Resource.IPInstanceList.Values) 
            {
                if (ipManager.GetModel().Name.Equals(name)) 
                {
                    return true;
                }
            
            }
            return false;
        }
        private bool CheckDBInstanceName(string name)
        {
            foreach (DBManager dbManager in Resource.DBInstanceList.Values)
            {
                if (dbManager.GetModel().Name.Equals(name))
                {
                    return true;
                }

            }
            return false;
        }
       




       //init variables from config file
        public int Init() 
        {
            Logger.Info("config file path:"  + Path.Replace(".\\", ""));
            if (InitGeneral() == 0) 
            {
                return 0;
            }
            if (InitLogPath() == 0) 
            {
                return 0;
            }
            if (CheckAutoRun() == 0) 
            {
                return 0;
            }
            if (InitWebGis() == 0)
            {
                return 0;
            }
            if (InitTran() == 0) 
            {
                return 0;
            }
            if (InitUserInfo() == 0) 
            {
                return 0;
            }
            if (InitConnStrings() == 0)
            {
                return 0;
            }
            if (InitIPTransfer() == 0)
            {
                return 0;
            }
            if (InitDBTransfer() == 0) 
            {
                return 0;
            }
           
            if (InitListeningPort() == 0) 
            {
                return 0;
            }
           
            if (InitPluginDll() == 0) 
            {
                return 0;
            }
            if (InitPlugin() == 0) 
            {
                return 0;
            }
            return 1;
        
        }

        public int DBInstanceNameExist(string  insName) 
        {
            XmlNodeList nodeList = Document.SelectSingleNode("configuration").SelectSingleNode("DBTransfer").SelectNodes("Instance");

            foreach(XmlNode node in nodeList)
            {
                try
                {
                    string name = node.Attributes["name"].Value.ToUpper().Trim();


                    if (insName.ToUpper().Equals(name)) 
                    {
                        return 1;
                        
                    }


                }
                catch (Exception ex) 
                {
                    throw ex;
                
                }    
            
            }

            return 0;
        }

        public int IPInstanceNameExist(string  insName)
        {
            XmlNodeList nodeList = Document.SelectSingleNode("configuration").SelectSingleNode("IPTransfer").SelectNodes("Instance");

            foreach (XmlNode node in nodeList)
            {
                try
                {
                    string name = node.Attributes["name"].Value.ToUpper().Trim();


                    if (insName.ToUpper().Equals(name))
                    {
                        return 1;

                    }


                }
                catch (Exception ex)
                {
                    throw ex;

                }

            }

            return 0;
        }

       //检查IP和PORT是否已经在IP转发配置中存在
        public int IPPortExist(string ip,int port) 
        {

            XmlNodeList nodeList = Document.SelectSingleNode("configuration").SelectSingleNode("IPTransfer").SelectNodes("Instance");

            foreach (XmlNode node in nodeList)
            {
                try
                {
                    string tmpIP = node.SelectSingleNode("IP").Attributes["value"].Value.ToUpper().Trim();
                    string tmpPort = node.SelectSingleNode("Port").Attributes["value"].Value.ToUpper().Trim();


                    if (ip.ToUpper().Equals(tmpIP) && port == Convert.ToInt32(tmpPort))
                    {
                        return 1;

                    }


                }
                catch (Exception ex)
                {
                    throw ex;

                }

            }

            return 0;

        }
       //检查表名称是否存在
        public int TableNameExist(string tableName)
        {

            XmlNodeList nodeListDBInstance = Document.SelectSingleNode("configuration").SelectSingleNode("DBTransfer").SelectNodes("Instance");
         

            foreach (XmlNode node in nodeListDBInstance)
            {
                try
                {
                    string tmpTable = node.SelectSingleNode("TableName").Attributes["value"].Value.ToUpper().Trim();


                    if (tmpTable.Equals(tableName.ToUpper()))
                    {
                        return 1;

                    }


                }
                catch (Exception ex)
                {
                    throw ex;

                }

            }


            return 0;

        }

    
        public int ModifyLanguage(string lang)
        {

            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("General");
         
            try
            {
                XmlElement xe = (XmlElement)node.SelectSingleNode("Language");
                RegistryKey Local = Registry.LocalMachine;
                RegistryKey runKey = Local.CreateSubKey(@"SOFTWARE\Eastcom\GPSTran\");
                

                if(lang.Equals("简体中文"))
                {
                    xe.SetAttribute("value","zh_cn");
                    runKey.SetValue("Language","zh_cn");
                }
                else if (lang.Equals("English"))
                {
                    xe.SetAttribute("value", "en_us");
                    runKey.SetValue("Language", "en_us");
                }
                else 
                {
                    throw new Exception("unsupported language");
                }
                
                Document.Save(Path);

                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public int AddDBInstance(DBModel dbModel) 
        {

           try
            {
                if (DBInstanceNameExist(dbModel.Name.ToUpper()) >= 1)
                {
                    throw new Exception("DB instance name already exists,duplicated instance name:"+dbModel.Name);

                }
                if (TableNameExist(dbModel.TableName.ToUpper()) >= 1)
                {
                    throw new Exception("DB instance table name already exists,duplicated table name:"+dbModel.TableName);
                }
            }
           catch (Exception ex)
           {
               throw ex;
           }


            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("DBTransfer");

            XmlElement dbElement= Document.CreateElement("Instance");
            dbElement.SetAttribute("name", dbModel.Name);
            dbElement.SetAttribute("enabled", dbModel.Enabled.ToString());
            XmlElement tableNameElement = Document.CreateElement("TableName");
            tableNameElement.SetAttribute("value", dbModel.TableName);
            XmlElement lonOffsetElement = Document.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", dbModel.LonOffset.ToString());
            XmlElement latOffsetElement = Document.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", dbModel.LatOffset.ToString());
            XmlElement maxCountElement = Document.CreateElement("MaxCount");
            maxCountElement.SetAttribute("value", dbModel.MaxCount.ToString());
            
            XmlElement entityIDElement = Document.CreateElement("EntityID");
            foreach (int i in dbModel.EntityID)
            {
                XmlElement tmp = Document.CreateElement("Add");
                tmp.SetAttribute("value",i.ToString());
                entityIDElement.AppendChild(tmp);
            }
            try
            {
                dbElement.AppendChild(tableNameElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(maxCountElement);
                if (dbModel.Citycode > 0)
                {
                    XmlElement citycode = Document.CreateElement("CityCode");
                    citycode.SetAttribute("value", dbModel.Citycode.ToString());
                    dbElement.AppendChild(citycode);
                }
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);
                Document.Save(Path);
                return 1;
            }
            catch (Exception ex) 
            {
                Logger.Error("add DB Instance failed , error message"+ex.ToString());
            
            }
            return 1;
        }
        public int RemoveDBInstance(string InstanceName)
        {
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("DBTransfer");
            XmlNodeList nodeList = node.SelectNodes("Instance");
            foreach (XmlNode nd in nodeList) 
            {
                string name = nd.Attributes["name"].Value.ToUpper().Trim();
                if (name.Equals(InstanceName.ToUpper())) 
                {
                    try
                    {
                        node.RemoveChild(nd);
                        Document.Save(Path);
                        return 1;
                    }
                    catch (Exception ex) 
                    {

                        throw ex;
                    }
                
                }
            
            }



            return 1;
        }
        public int ModifyDBInstance(string instanceName,DBModel dbModel)
        {
           
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("DBTransfer");
            XmlNodeList nodeList = node.SelectNodes("Instance");
            XmlElement nd = null;
            foreach (XmlNode n in nodeList) 
            {
                string name = n.Attributes["name"].Value.ToUpper().Trim();
                if (name.Equals(instanceName.ToUpper())) 
                {
                    nd = (XmlElement)n;
                }

            }
            if (nd == null) 
            {
                //throw new Exception("DB instance does not exists,instance name:" + instanceName) ;
                Logger.Error(string.Format("XML INIT ModifyDBInstance DB instance does not exists,instance name:{0}", instanceName));
            }

            try
            {
                XmlElement xe = (XmlElement)nd;
                xe.SetAttribute("name", dbModel.Name);
                xe.SetAttribute("enabled", dbModel.Enabled.ToString());
                XmlElement tableNameElement = (XmlElement)nd.SelectSingleNode("TableName");
                tableNameElement.SetAttribute("value", dbModel.TableName);
                XmlElement LonOffsetElement = (XmlElement)nd.SelectSingleNode("LonOffset");
                LonOffsetElement.SetAttribute("value", dbModel.LonOffset.ToString());
                XmlElement LatOffsetElement = (XmlElement)nd.SelectSingleNode("LatOffset");
                LatOffsetElement.SetAttribute("value", dbModel.LatOffset.ToString());
                XmlElement maxCountElement = (XmlElement)nd.SelectSingleNode("MaxCount");
                maxCountElement.SetAttribute("value", dbModel.MaxCount.ToString());
                XmlElement entityIDElement = (XmlElement)nd.SelectSingleNode("EntityID");

                entityIDElement.RemoveAll();
                foreach (int i in dbModel.EntityID) 
                {
                    XmlElement el = Document.CreateElement("Add");
                    el.SetAttribute("value", i.ToString());
                    entityIDElement.AppendChild(el);
                        
                }

                Document.Save(Path);
                        
                return 1;

            }catch(Exception ex)
            {
                //throw ex;
                Logger.Error(string.Format("XML INIT ModifyDBInstance :{0}", ex.ToString()));
            }

               
            
            

            return 1;
        }
        public int AddIPInstance(IPModel ipModel) 
        {

            try
            {
                if (IPInstanceNameExist(ipModel.Name.ToUpper()) >= 1)
                {
                    //throw new Exception("IP Instance name already exists,ip instance name:"+ipModel.Name);
                    Logger.Error(string.Format("IP Instance name already exists,ip instance name:{0}", ipModel.Name));
                }
                if (IPPortExist(ipModel.Ip, ipModel.Port) >= 1)
                {
                    //throw new Exception("IP and Port already exsits");
                    Logger.Error("IP and Port already exsits");
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Logger.Error(string.Format("XML INIT AddIPInstance :{0}", ex.ToString()));
            }
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("IPTransfer");

            XmlElement dbElement = Document.CreateElement("Instance");
            dbElement.SetAttribute("name", ipModel.Name);
            dbElement.SetAttribute("enabled", ipModel.Enabled.ToString());
            //XmlElement protocolElement = Document.CreateElement("Protocol");
            //protocolElement.SetAttribute("value", ipModel.Protocol);
            XmlElement ipElement = Document.CreateElement("IP");
            ipElement.SetAttribute("value", ipModel.Ip);
            XmlElement portElement = Document.CreateElement("Port");
            portElement.SetAttribute("value", ipModel.Port.ToString());
            //XmlElement netProtocolElement = Document.CreateElement("NetProtocol");
            //netProtocolElement.SetAttribute("value", ipModel.NetProtocol);
            XmlElement lonOffsetElement = Document.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", ipModel.LonOffset.ToString());
            XmlElement latOffsetElement = Document.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", ipModel.LatOffset.ToString());
            XmlElement entityIDElement = Document.CreateElement("EntityID");
            foreach (int i in ipModel.EntityID) 
            {
                XmlElement el = Document.CreateElement("Add");
                el.SetAttribute("value", i.ToString());
                entityIDElement.AppendChild(el);
            
            }
            try
            {
                //dbElement.AppendChild(protocolElement);
                dbElement.AppendChild(ipElement);
                dbElement.AppendChild(portElement);
                //dbElement.AppendChild(netProtocolElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);

                Document.Save(Path);
               
            }
            catch (Exception ex) 
            {
                throw ex;
                
            }

            return 1;
        }
        public int RemoveIPInstance(string InstanceName)
        {
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("IPTransfer");
            XmlNodeList nodeList = node.SelectNodes("Instance");
            foreach (XmlNode nd in nodeList)
            {
                string name = nd.Attributes["name"].Value.ToUpper().Trim();
                if (name.Equals(InstanceName.ToUpper()))
                {
                    try
                    {
                        node.RemoveChild(nd);
                        Document.Save(Path);
                        return 1;
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }

            }

            return 1;

        }

        public int ModifyIPInstance(string instanceName,IPModel ipModel)
        {
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("IPTransfer");
            XmlNodeList nodeList = node.SelectNodes("Instance");
            XmlElement xe = null;
            foreach (XmlNode nd in nodeList) 
            {
                string name = nd.Attributes["name"].Value.ToUpper().Trim();
                if (name.Equals(instanceName.ToUpper()))
                {
                    xe = (XmlElement)nd;
                }
            }
            if (xe == null) 
            {
                throw new Exception("IP instance does not exists,ipInstanceName:" + instanceName);
            
            }


            try
            {
                xe.SetAttribute("name", ipModel.Name);
                xe.SetAttribute("enabled", ipModel.Enabled.ToString());
                //XmlElement protocolNameElement = (XmlElement)xe.SelectSingleNode("Protocol");
                //protocolNameElement.SetAttribute("value", ipModel.Protocol);
                XmlElement ipElement = (XmlElement)xe.SelectSingleNode("IP");
                ipElement.SetAttribute("value", ipModel.Ip);
                XmlElement portElement = (XmlElement)xe.SelectSingleNode("Port");
                portElement.SetAttribute("value", ipModel.Port.ToString());
                //XmlElement netProtocolElement = (XmlElement)xe.SelectSingleNode("NetProtocol");
                //netProtocolElement.SetAttribute("value", ipModel.NetProtocol);
                XmlElement lonOffsetElement = (XmlElement)xe.SelectSingleNode("LonOffset");
                lonOffsetElement.SetAttribute("value", ipModel.LonOffset.ToString());
                XmlElement latOffsetElement = (XmlElement)xe.SelectSingleNode("LatOffset");
                latOffsetElement.SetAttribute("value", ipModel.LatOffset.ToString());


                XmlElement entityIDElement = (XmlElement)xe.SelectSingleNode("EntityID");

                entityIDElement.RemoveAll();
                foreach (int i in ipModel.EntityID)
                {
                    XmlElement el = Document.CreateElement("Add");
                    el.SetAttribute("value", i.ToString());
                    entityIDElement.AppendChild(el);

                }

                Document.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int PluginInstanceExist(string insName) 
        {
            XmlNodeList nodeList = Document.SelectSingleNode("configuration").SelectSingleNode("Plugin").SelectNodes("Instance");

            foreach (XmlNode node in nodeList)
            {
                try
                {
                    string name = node.Attributes["name"].Value.ToUpper().Trim();


                    if (insName.ToUpper().Equals(name))
                    {
                        return 1;

                    }


                }
                catch (Exception ex)
                {
                    throw ex;

                }

            }

            return 0;
        
        }

        public int AddPluginInstance(PluginModel pModel) 
        {
            try
            {
                if (PluginInstanceExist(pModel.Name.ToUpper()) >= 1)
                {
                    throw new Exception("IP Instance name already exists,duplicated ip instance name:"+pModel.Name);
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("Plugin");

            if (pModel.DllModel.PType == PluginType.RemoteIP) 
            {
                XmlElement dbElement = Document.CreateElement("Instance");
                dbElement.SetAttribute("name", pModel.Name);
                dbElement.SetAttribute("enabled", pModel.Enabled.ToString());
                XmlElement dllElement = Document.CreateElement("Dll");
                dllElement.SetAttribute("name", pModel.DllModel.Name);
                XmlElement protocolElement = Document.CreateElement("Protocol");

                protocolElement.SetAttribute("value", pModel.Protocol);
                XmlElement ipElement = Document.CreateElement("IP");
                ipElement.SetAttribute("value", pModel.Ip);
                XmlElement portElement = Document.CreateElement("Port");
                portElement.SetAttribute("value", pModel.Port.ToString());
                //XmlElement netProtocolElement = Document.CreateElement("NetProtocol");
                //netProtocolElement.SetAttribute("value", pModel.NetProtocol);
                XmlElement lonOffsetElement = Document.CreateElement("LonOffset");
                lonOffsetElement.SetAttribute("value", pModel.LonOffset.ToString());
                XmlElement latOffsetElement = Document.CreateElement("LatOffset");
                latOffsetElement.SetAttribute("value", pModel.LatOffset.ToString());
                XmlElement entityIDElement = Document.CreateElement("EntityID");
                foreach (int i in pModel.EntityID)
                {
                    XmlElement el = Document.CreateElement("Add");
                    el.SetAttribute("value", i.ToString());
                    entityIDElement.AppendChild(el);

                }
                try
                {
                    dbElement.AppendChild(dllElement);
                    dbElement.AppendChild(protocolElement);
                    dbElement.AppendChild(ipElement);
                    dbElement.AppendChild(portElement);
                    //dbElement.AppendChild(netProtocolElement);
                    dbElement.AppendChild(lonOffsetElement);
                    dbElement.AppendChild(latOffsetElement);
                    dbElement.AppendChild(entityIDElement);
                    node.AppendChild(dbElement);

                    Document.Save(Path);

                }
                catch (Exception ex)
                {
                    throw ex;

                }


            }
            else if (pModel.DllModel.PType == PluginType.RemoteDB) 
            {
                XmlElement dbElement = Document.CreateElement("Instance");
                dbElement.SetAttribute("name", pModel.Name);
                dbElement.SetAttribute("enabled", pModel.Enabled.ToString());
                XmlElement dllElement = Document.CreateElement("Dll");
                dllElement.SetAttribute("name", pModel.DllModel.Name);
                XmlElement tableNameElement = Document.CreateElement("TableName");
                tableNameElement.SetAttribute("value", pModel.TableName);
                XmlElement maxCountElement = Document.CreateElement("MaxCount");
                maxCountElement.SetAttribute("value", pModel.MaxCount.ToString());
                XmlElement lonOffsetElement = Document.CreateElement("LonOffset");
                lonOffsetElement.SetAttribute("value", pModel.LonOffset.ToString());
                XmlElement latOffsetElement = Document.CreateElement("LatOffset");
                latOffsetElement.SetAttribute("value", pModel.LatOffset.ToString());
                XmlElement entityIDElement = Document.CreateElement("EntityID");

                foreach (int i in pModel.EntityID)
                {
                    XmlElement el = Document.CreateElement("Add");
                    el.SetAttribute("value", i.ToString());
                    entityIDElement.AppendChild(el);

                }
                XmlElement ipElement = Document.CreateElement("IP");
                ipElement.SetAttribute("value", pModel.DesIP.ToString());
                XmlElement catalogElement = Document.CreateElement("Catalog");
                catalogElement.SetAttribute("value", pModel.DesCatalog.ToString());
                XmlElement userElement = Document.CreateElement("User");
                userElement.SetAttribute("value", pModel.DesUser.ToString());
                XmlElement pwdElement = Document.CreateElement("Pwd");
                pwdElement.SetAttribute("value", pModel.DesPwd.ToString());
                XmlElement portElement = Document.CreateElement("Port");
                portElement.SetAttribute("value", pModel.DesPort.ToString());
                XmlElement instanceElement = Document.CreateElement("Instance");
                instanceElement.SetAttribute("value", pModel.DesInstance.ToString());




                try
                {
                    dbElement.AppendChild(dllElement);
                    dbElement.AppendChild(tableNameElement);
                    dbElement.AppendChild(maxCountElement);
                    dbElement.AppendChild(lonOffsetElement);
                    dbElement.AppendChild(latOffsetElement);
                    dbElement.AppendChild(entityIDElement);
                    dbElement.AppendChild(ipElement);
                    dbElement.AppendChild(catalogElement);
                    dbElement.AppendChild(userElement);
                    dbElement.AppendChild(pwdElement);
                    dbElement.AppendChild(portElement);
                    dbElement.AppendChild(instanceElement);
                    node.AppendChild(dbElement);

                    Document.Save(Path);

                }
                catch (Exception ex)
                {
                    throw ex;

                }


            }
            else if (pModel.DllModel.PType == PluginType.TranDB)
            {
                XmlElement dbElement = Document.CreateElement("Instance");
                dbElement.SetAttribute("name", pModel.Name);
                dbElement.SetAttribute("enabled", pModel.Enabled.ToString());
                XmlElement dllElement = Document.CreateElement("Dll");
                dllElement.SetAttribute("name", pModel.DllModel.Name);
                XmlElement tableNameElement = Document.CreateElement("TableName");
                tableNameElement.SetAttribute("value", pModel.TableName);
                XmlElement maxCountElement = Document.CreateElement("MaxCount");
                maxCountElement.SetAttribute("value", pModel.MaxCount.ToString());
                XmlElement lonOffsetElement = Document.CreateElement("LonOffset");
                lonOffsetElement.SetAttribute("value", pModel.LonOffset.ToString());
                XmlElement latOffsetElement = Document.CreateElement("LatOffset");
                latOffsetElement.SetAttribute("value", pModel.LatOffset.ToString());
                XmlElement entityIDElement = Document.CreateElement("EntityID");

                foreach (int i in pModel.EntityID)
                {
                    XmlElement el = Document.CreateElement("Add");
                    el.SetAttribute("value", i.ToString());
                    entityIDElement.AppendChild(el);

                }


                try
                {
                    dbElement.AppendChild(dllElement);
                    dbElement.AppendChild(tableNameElement);
                    dbElement.AppendChild(maxCountElement);
                    dbElement.AppendChild(lonOffsetElement);
                    dbElement.AppendChild(latOffsetElement);
                    if (pModel.Citycode > 0)
                    {
                        XmlElement citycode = Document.CreateElement("CityCode");
                        citycode.SetAttribute("value", pModel.Citycode.ToString());
                        dbElement.AppendChild(citycode);
                    }
                    dbElement.AppendChild(entityIDElement);
                    node.AppendChild(dbElement);

                    Document.Save(Path);

                }
                catch (Exception ex)
                {
                    throw ex;

                }


            }
            else 
            {
                throw new Exception("unknown plugin type");
            }

            return 1;

        
        }
        public int ModifyPluginInstance(string instanceName, PluginModel pModel) 
        {
           
            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("Plugin");
            XmlNodeList nodeList = node.SelectNodes("Instance");
            XmlElement desElement=null;
            foreach (XmlNode nd in nodeList) 
            {
                
                if (nd.Attributes["name"].Value.ToString().ToUpper().Equals(instanceName.ToUpper())) 
                {
                    desElement = (XmlElement)nd;
                    break;
                }
            
            }
            if (desElement == null) 
            {
                throw new Exception("plugin instance does not exists,plugin instance name:"+instanceName);
            }

            if (pModel.DllModel.PType == PluginType.RemoteIP)
            {
                try
                {
                    desElement.SetAttribute("name", pModel.Name);
                    desElement.SetAttribute("enabled", pModel.Enabled.ToString());
                    XmlElement protocolElement = (XmlElement)desElement.SelectSingleNode("Protocol");
                    protocolElement.SetAttribute("value", pModel.Protocol);
                    XmlElement ipElement = (XmlElement)desElement.SelectSingleNode("IP");
                    ipElement.SetAttribute("value", pModel.Ip);
                    XmlElement portElement = (XmlElement)desElement.SelectSingleNode("Port");
                    portElement.SetAttribute("value", pModel.Port.ToString());
                    //XmlElement netProtocolElement = (XmlElement)desElement.SelectSingleNode("NetProtocol");
                    //netProtocolElement.SetAttribute("value", pModel.NetProtocol);
                    XmlElement lonOffsetElement = (XmlElement)desElement.SelectSingleNode("LonOffset");
                    lonOffsetElement.SetAttribute("value", pModel.LonOffset.ToString());
                    XmlElement latOffsetElement = (XmlElement)desElement.SelectSingleNode("LatOffset");
                    latOffsetElement.SetAttribute("value", pModel.LatOffset.ToString());
                    XmlElement entityIDElement = (XmlElement)desElement.SelectSingleNode("EntityID");
                    entityIDElement.RemoveAll();
                    foreach (int i in pModel.EntityID)
                    {
                        XmlElement el = Document.CreateElement("Add");
                        el.SetAttribute("value", i.ToString());
                        entityIDElement.AppendChild(el);

                    }

                    Document.Save(Path);
                    return 1;
                   
                }
                catch (Exception ex)
                {
                    throw ex;

                }


            }
            else if (pModel.DllModel.PType == PluginType.RemoteDB)
            {
                try
                {
                    desElement.SetAttribute("name", pModel.Name);
                    desElement.SetAttribute("enabled", pModel.Enabled.ToString());

                    XmlElement tableNameElement = (XmlElement)desElement.SelectSingleNode("TableName");
                    tableNameElement.SetAttribute("value", pModel.TableName);
                    XmlElement maxCountElement = (XmlElement)desElement.SelectSingleNode("MaxCount");
                    maxCountElement.SetAttribute("value", pModel.MaxCount.ToString());
                    XmlElement lonOffsetElement = (XmlElement)desElement.SelectSingleNode("LonOffset");
                    lonOffsetElement.SetAttribute("value", pModel.LonOffset.ToString());
                    XmlElement latOffsetElement = (XmlElement)desElement.SelectSingleNode("LatOffset");
                    latOffsetElement.SetAttribute("value", pModel.LatOffset.ToString());
                    XmlElement entityIDElement = (XmlElement)desElement.SelectSingleNode("EntityID");
                    entityIDElement.RemoveAll();
                    foreach (int i in pModel.EntityID)
                    {
                        XmlElement el = Document.CreateElement("Add");
                        el.SetAttribute("value", i.ToString());
                        entityIDElement.AppendChild(el);

                    }
                    XmlElement ipElement = (XmlElement)desElement.SelectSingleNode("IP");
                    ipElement.SetAttribute("value", pModel.DesIP.ToString());
                    XmlElement catalogElement = (XmlElement)desElement.SelectSingleNode("Catalog");
                    catalogElement.SetAttribute("value", pModel.DesCatalog.ToString());
                    XmlElement userElement = (XmlElement)desElement.SelectSingleNode("User");
                    userElement.SetAttribute("value", pModel.DesUser.ToString());
                    XmlElement pwdElement = (XmlElement)desElement.SelectSingleNode("Pwd");
                    pwdElement.SetAttribute("value", pModel.DesPwd.ToString());
                    XmlElement portElement = (XmlElement)desElement.SelectSingleNode("Port");
                    portElement.SetAttribute("value", pModel.DesPort.ToString());
                    XmlElement instanceElement = (XmlElement)desElement.SelectSingleNode("Instance");
                    instanceElement.SetAttribute("value", pModel.DesInstance.ToString());

                    Document.Save(Path);
                    return 1;


                }
                catch (Exception ex)
                {
                    throw ex;

                }


            }
            else if (pModel.DllModel.PType == PluginType.TranDB)
            {
                try
                {
              
                    desElement.SetAttribute("name", pModel.Name);
                    desElement.SetAttribute("enabled", pModel.Enabled.ToString());

                    XmlElement tableNameElement = (XmlElement)desElement.SelectSingleNode("TableName");
                    tableNameElement.SetAttribute("value", pModel.TableName);
                    XmlElement maxCountElement = (XmlElement)desElement.SelectSingleNode("MaxCount");
                    maxCountElement.SetAttribute("value", pModel.MaxCount.ToString());
                    XmlElement lonOffsetElement = (XmlElement)desElement.SelectSingleNode("LonOffset");
                    lonOffsetElement.SetAttribute("value", pModel.LonOffset.ToString());
                    XmlElement latOffsetElement = (XmlElement)desElement.SelectSingleNode("LatOffset");
                    latOffsetElement.SetAttribute("value", pModel.LatOffset.ToString());
                    XmlElement entityIDElement = (XmlElement)desElement.SelectSingleNode("EntityID");
                    entityIDElement.RemoveAll();
                    foreach (var i in pModel.EntityID)
                    {
                        XmlElement el = Document.CreateElement("Add");
                        el.SetAttribute("value", i.ToString());
                        entityIDElement.AppendChild(el);
                    }

                        XmlElement citycode = (XmlElement)desElement.SelectSingleNode("CityCode");
                        citycode.SetAttribute("value", pModel.Citycode.ToString());

                    Document.Save(Path);
                    return 1;
                }
                catch (Exception ex)
                {
                    throw ex;

                }


            }
            else
            {
                throw new Exception("unknown plugin type");
            }

        }
        public int RemovePluginInstance(string instanceName) 
        {

            XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("Plugin");
            XmlNodeList nodeList = node.SelectNodes("Instance");
            foreach (XmlNode nd in nodeList)
            {
                try
               {
                    string name = nd.Attributes["name"].Value.ToUpper().Trim();
                    if (name.Equals(instanceName.ToUpper()))
                    {
                    
                            node.RemoveChild(nd);
                            Document.Save(Path);
                            return 1;


                    }
               }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

            throw new Exception("Plugin instance name not found,instance name" + instanceName);

        
        }

        public int ModifyPort(int port) 
        {
            try
            {
                XmlNode node = Document.SelectSingleNode("configuration");
                XmlElement element = (XmlElement)node.SelectSingleNode("ListeningPort");
                element.SetAttribute("value", port.ToString());
                Document.Save(Path);
                return 1;

            }
            catch (Exception ex) 
            {
                throw ex;
            }

        
        }

        public int ModifyTran(string ip,string catalog,string user,string pwd,string port,string instance) 
        {
            try
            {
                XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("Tran");
                XmlElement ipElement = (XmlElement)node.SelectSingleNode("IP");
                ipElement.SetAttribute("value", ip);
                XmlElement catalogElement = (XmlElement)node.SelectSingleNode("Catalog");
                catalogElement.SetAttribute("value", catalog);
                XmlElement userElement = (XmlElement)node.SelectSingleNode("User");
                userElement.SetAttribute("value", user);
                XmlElement pwdElement = (XmlElement)node.SelectSingleNode("Pwd");
                pwdElement.SetAttribute("value", pwd);
                XmlElement portElement = (XmlElement)node.SelectSingleNode("Port");
                portElement.SetAttribute("value", port);
                XmlElement instanceElement = (XmlElement)node.SelectSingleNode("Instance");
                instanceElement.SetAttribute("value", instance);


                Document.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public int ModifyWebGis(string ip, string catalog, string user, string pwd, string port, string instance)
        {
            try
            {
                XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("WebGis");
                XmlElement ipElement = (XmlElement)node.SelectSingleNode("IP");
                ipElement.SetAttribute("value", ip);
                XmlElement catalogElement = (XmlElement)node.SelectSingleNode("Catalog");
                catalogElement.SetAttribute("value", catalog);
                XmlElement userElement = (XmlElement)node.SelectSingleNode("User");
                userElement.SetAttribute("value", user);
                XmlElement pwdElement = (XmlElement)node.SelectSingleNode("Pwd");
                pwdElement.SetAttribute("value", pwd);
                XmlElement portElement = (XmlElement)node.SelectSingleNode("Port");
                portElement.SetAttribute("value", port);
                XmlElement instanceElement = (XmlElement)node.SelectSingleNode("Instance");
                instanceElement.SetAttribute("value", instance);


                Document.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int ModifyUserInfo(bool enabled,int flushInterval)
        {
            try
            {
                XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("UserInfo");
                ((XmlElement)node).SetAttribute("enabled", enabled.ToString());
                XmlElement flushElement = (XmlElement)node.SelectSingleNode("FlushInterval");
                flushElement.SetAttribute("value", flushInterval.ToString());
               
                Document.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int ModifyLogKeepDays(int logKeepDays) 
        {
            try
            {
                XmlNode node = Document.SelectSingleNode("configuration").SelectSingleNode("General");
                XmlElement logTimeElement = (XmlElement)node.SelectSingleNode("LogKeepDays");
                logTimeElement.SetAttribute("value", logKeepDays.ToString());

                Document.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

       public void SaveFilePath()
       {
           Document.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Tran.config"));
       }
    }
}
