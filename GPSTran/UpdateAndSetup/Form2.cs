using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace UpdateAndSetup
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private static int threadTag = 0;
        private string FileName = "";
        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = @"Tran.config|*.config";
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.openFileDialog1.FileName;
               
            }
            textBox1.Text = FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            DialogResult resault = MessageBox.Show(@"升级还未完成确定要取消升级吗？", @"取消升级", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (resault == DialogResult.OK)
            {
              
                Application.Exit();
            }
        }

        private XmlDocument sourDocument = new XmlDocument();
        private XmlDocument destDocument = new XmlDocument();
        //private XmlDocument document = new XmlDocument();
      
        public int ModifyPort(string port,string Path)
        {
            try
            {
                XmlNode node = destDocument.SelectSingleNode("configuration");
                XmlElement element = (XmlElement)node.SelectSingleNode("ListeningPort");
                element.SetAttribute("value", port);
                destDocument.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        public int ModifyTran(string ip, string catalog, string user, string pwd, string port, string instance, string Path)
        {
            try
            {
                XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("Tran");
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


                destDocument.Save(Path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void modifyPlginForDB(string destPathForConfig, INI readConfigIni)
        {
            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("Plugin");
            XmlElement dbElement = destDocument.CreateElement("Instance");
            dbElement.SetAttribute("name", "T_GPS_QC_TMP");
            dbElement.SetAttribute("enabled", "True");
            XmlElement dllElement = destDocument.CreateElement("Dll");
            dllElement.SetAttribute("name", "DB_Version_1");
            XmlElement tableNameElement = destDocument.CreateElement("TableName");
            tableNameElement.SetAttribute("value", "T_GPS_QC_TMP");
            XmlElement maxCountElement = destDocument.CreateElement("MaxCount");
            maxCountElement.SetAttribute("value", readConfigIni.ReadValue("HistoryGis_info", "rowsLimit"));
            XmlElement lonOffsetElement = destDocument.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", "0");
            XmlElement latOffsetElement = destDocument.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", "0");
            XmlElement entityIDElement = destDocument.CreateElement("EntityID");

            XmlElement el = destDocument.CreateElement("Add");
            //el.SetAttribute("value", "1");

            entityIDElement.AppendChild(el);
            try
            {
                dbElement.AppendChild(dllElement);
                dbElement.AppendChild(tableNameElement);
                dbElement.AppendChild(maxCountElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);

                destDocument.Save(destPathForConfig);

            }
            catch (Exception ex)
            {
                throw ex;

            }
              
        }
        
        public void createDBstance(string Path, string name,string tablename, string lon,string lat,string maxcount,List<int> EntityList)
        {
            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("DBTransfer");
            XmlElement dbElement = destDocument.CreateElement("Instance");
            dbElement.SetAttribute("name", name);
            dbElement.SetAttribute("enabled", "True");
            XmlElement tableElement = destDocument.CreateElement("TableName");
            tableElement.SetAttribute("value", tablename);
            XmlElement maxCountElement = destDocument.CreateElement("MaxCount");
            maxCountElement.SetAttribute("value", maxcount);
            XmlElement lonOffsetElement = destDocument.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", lon);
            XmlElement latOffsetElement = destDocument.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", lat);
            XmlElement entityIDElement = destDocument.CreateElement("EntityID");
            
            //el.SetAttribute("value", "1");
            for (var i = 0; i < EntityList.Count; i++)
            {
                XmlElement el = destDocument.CreateElement("Add");
                el.SetAttribute("value", EntityList[i].ToString());
                entityIDElement.AppendChild(el);
            }
            


            try
            {
                dbElement.AppendChild(tableElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(maxCountElement);
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);

                destDocument.Save(Path);

            }
            catch (Exception ex)
            {
                throw ex;

            }
 
 
        }

        public void createDBplugin(string Path,string Dllname,string lon,string lat, string maxcount, string tablename, string name,string citycode,List<int> EntityList)
        {
            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("Plugin");
            XmlElement dbElement = destDocument.CreateElement("Instance");
            dbElement.SetAttribute("name",name);
            dbElement.SetAttribute("enabled","True");
            XmlElement dllElement = destDocument.CreateElement("Dll");
            dllElement.SetAttribute("name", Dllname);
            XmlElement tableNameElement = destDocument.CreateElement("TableName");
            tableNameElement.SetAttribute("value", tablename);
            XmlElement maxCountElement = destDocument.CreateElement("MaxCount");
            maxCountElement.SetAttribute("value", maxcount);
            XmlElement lonOffsetElement = destDocument.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", lon);
            XmlElement latOffsetElement = destDocument.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", lat);
            XmlElement citycodeElement = destDocument.CreateElement("CityCode");
            citycodeElement.SetAttribute("value", citycode);
            XmlElement entityIDElement = destDocument.CreateElement("EntityID");

            for (var i = 0; i < EntityList.Count; i++)
            {
                XmlElement el = destDocument.CreateElement("Add");
                el.SetAttribute("value", EntityList[i].ToString());
                entityIDElement.AppendChild(el);
            }

            try
            {
                dbElement.AppendChild(dllElement);
                dbElement.AppendChild(tableNameElement);
                dbElement.AppendChild(maxCountElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(citycodeElement);
                dbElement.AppendChild(entityIDElement);             
                node.AppendChild(dbElement);
                
                destDocument.Save(Path);

            }
            catch (Exception ex)
            {
                throw ex;

            }
 
        }
        public void filesCopy(object filescopy)
        {
            bool s = true;
            while (s)
            {
               while(threadTag==1)
               {
                   FIlesCopy filecopy = (FIlesCopy)filescopy;
                   filecopy.beginCopyFiles();
                   DialogResult resault = MessageBox.Show("升级已完成是否重新启动程序", "重启程序", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                   if (resault == DialogResult.OK)
                   {
                       Process.Start(FileName.Substring(0, FileName.LastIndexOf("\\")) + "\\GPSTran.exe");
                       Process.GetCurrentProcess().CloseMainWindow();
                       Application.Exit();
                   }
                   else {
                       Application.Exit();
                   }

                   threadTag = 0;
                   s = false;
                   break;
               }
            }
           

        }
        public void createIPPlugin(string Path,string protol,string ip, string port,string netportol, string lon, string lat,string name, string dllname,List<int> EntityList)
        {

            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("Plugin");
            XmlElement dbElement = destDocument.CreateElement("Instance");
            dbElement.SetAttribute("name", name);
            dbElement.SetAttribute("enabled", "True");
            XmlElement dllElement = destDocument.CreateElement("Dll");
            dllElement.SetAttribute("name", dllname);
            XmlElement protocolElement = destDocument.CreateElement("Protocol");

            protocolElement.SetAttribute("value", "PGIS");
            XmlElement ipElement = destDocument.CreateElement("IP");
            ipElement.SetAttribute("value", ip);
            XmlElement portElement = destDocument.CreateElement("Port");
            portElement.SetAttribute("value", port);
            XmlElement netProtocolElement = destDocument.CreateElement("NetProtocol");
            netProtocolElement.SetAttribute("value","UDP");
            XmlElement lonOffsetElement = destDocument.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value",lon);
            XmlElement latOffsetElement = destDocument.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", lat);
            XmlElement entityIDElement = destDocument.CreateElement("EntityID");

            for (var i = 0; i < EntityList.Count; i++)
            {
                XmlElement el = destDocument.CreateElement("Add");
                el.SetAttribute("value", EntityList[i].ToString());
                entityIDElement.AppendChild(el);
            }
            try
            {
                dbElement.AppendChild(dllElement);
                dbElement.AppendChild(protocolElement);
                dbElement.AppendChild(ipElement);
                dbElement.AppendChild(portElement);
                dbElement.AppendChild(netProtocolElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);
                
                destDocument.Save(Path);

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public void createIpstance(string Path, string name, string ip, string port, string LonOffset, string LatOffset, List<int> EntityList)
        {
            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("IPTransfer");
            XmlElement dbElement = destDocument.CreateElement("Instance");
            dbElement.SetAttribute("name", name);
            dbElement.SetAttribute("enabled", "True");
            XmlElement protocolElement = destDocument.CreateElement("Protocol");
            protocolElement.SetAttribute("value", "PGIS");
            XmlElement ipElement = destDocument.CreateElement("IP");
            ipElement.SetAttribute("value", ip);
            XmlElement portElement = destDocument.CreateElement("Port");
            portElement.SetAttribute("value", port);
            XmlElement netProtocolElement = destDocument.CreateElement("NetProtocol");
            netProtocolElement.SetAttribute("value", "UDP");
            XmlElement lonOffsetElement = destDocument.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", LonOffset);
            XmlElement latOffsetElement = destDocument.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", LatOffset);
            XmlElement entityIDElement = destDocument.CreateElement("EntityID");
            for (var i = 0; i < EntityList.Count; i++)
            {
                XmlElement el = destDocument.CreateElement("Add");
                el.SetAttribute("value", EntityList[i].ToString());
                entityIDElement.AppendChild(el);
            }

            try
            {
                dbElement.AppendChild(protocolElement);
                dbElement.AppendChild(ipElement);
                dbElement.AppendChild(portElement);
                dbElement.AppendChild(netProtocolElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);

                destDocument.Save(Path);

            }
            catch (Exception ex)
            {
                throw ex;

            }
 
        }

        public void createSingleIPInstance(string Path,string i,string ip,string port)
        {
            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("IPTransfer");
            XmlElement dbElement = destDocument.CreateElement("Instance");
            dbElement.SetAttribute("name", i);
            dbElement.SetAttribute("enabled", "True");
            XmlElement protocolElement = destDocument.CreateElement("Protocol");
            protocolElement.SetAttribute("value", "PGIS");
            XmlElement ipElement = destDocument.CreateElement("IP");
            ipElement.SetAttribute("value", ip);
            XmlElement portElement = destDocument.CreateElement("Port");
            portElement.SetAttribute("value", port);
            XmlElement netProtocolElement = destDocument.CreateElement("NetProtocol");
            netProtocolElement.SetAttribute("value", "UDP");
            XmlElement lonOffsetElement = destDocument.CreateElement("LonOffset");
            lonOffsetElement.SetAttribute("value", "0");
            XmlElement latOffsetElement = destDocument.CreateElement("LatOffset");
            latOffsetElement.SetAttribute("value", "0");
            XmlElement entityIDElement = destDocument.CreateElement("EntityID");
            XmlElement el = destDocument.CreateElement("Add");
            el.SetAttribute("value", i.ToString());
            entityIDElement.AppendChild(el);
           

            try
            {
                dbElement.AppendChild(protocolElement);
                dbElement.AppendChild(ipElement);
                dbElement.AppendChild(portElement);
                dbElement.AppendChild(netProtocolElement);
                dbElement.AppendChild(lonOffsetElement);
                dbElement.AppendChild(latOffsetElement);
                dbElement.AppendChild(entityIDElement);
                node.AppendChild(dbElement);
              
                destDocument.Save(Path);

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public void createIpInstance(string destPathForConfig, INI readConfigIni)
        {
            string IPset = readConfigIni.ReadValue("sendIP_set", "IPSET");
            string[] IPInstances=null;
            if(IPset!="")
            {
               IPInstances = IPset.Split(';');
            }
            if(IPInstances!=null && IPInstances.Length>0)
            {
                for (int i = 0; i < IPInstances.Length; i++ )
                {
                    string[] instanceIp = IPInstances[i].Split(':');
                    if (instanceIp.Length > 0)
                    {
                        createSingleIPInstance(destPathForConfig, i.ToString(), instanceIp[0], instanceIp[1]);
                    }

                }
            }


        }
        public void  ModifyGeneral(string logKeepDays,string language,string Path)
        {
          try
            {
                XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("General");
                XmlElement logTimeElement = (XmlElement)node.SelectSingleNode("LogKeepDays");
                logTimeElement.SetAttribute("value", logKeepDays.ToString());
                XmlElement languageElement = (XmlElement)node.SelectSingleNode("Language");
                languageElement.SetAttribute("value", language);
                destDocument.Save(Path);

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ModifyUserInfo(string value,string enableValue, string Path)
        {
            XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("UserInfo");
            XmlElement enable = (XmlElement)destDocument.SelectSingleNode("configuration").SelectSingleNode("UserInfo");
            enable.SetAttribute("enabled", enableValue);
            XmlElement logTimeElement = (XmlElement)node.SelectSingleNode("FlushInterval");

            logTimeElement.SetAttribute("value",value);        
            destDocument.Save(Path);
        }

        public int ModifyWebGis(string ip, string catalog, string user, string pwd, string port, string instance,string path)
        {
            try
            {
                XmlNode node = destDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis");
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


                destDocument.Save(path);
                return 1;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void copyConfig()
        {
            string applationPath = AppDomain.CurrentDomain.BaseDirectory;
            FIlesCopy filesCopy = new FIlesCopy(applationPath + "Config", applationPath + "Release");
            filesCopy.beginCopyFiles();
        }



        public void dealWithConfig()
        {
            Thread copyConfig_Thread = new Thread(copyConfig);
            copyConfig_Thread.Start();
            //升级配置文件读取路径
            Thread.Sleep(1000);
            string destPathForConfig = AppDomain.CurrentDomain.BaseDirectory + "Release\\" + "Tran.config";
            try
            {
                destDocument.Load(destPathForConfig);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #region 旧代码 2015-07-03
            ///产生异常代码部分：进程未关闭，不能放在多线程中
            //Process[] p = Process.GetProcesses();
            //foreach (Process p1 in p)
            //{
            //    try
            //    {
            //        string processName = p1.ProcessName.ToLower().Trim();
            //        //判断是否包含阻碍更新的进程
            //        if (processName == "GPSTran")
            //        {
            //            p1.Kill();
            //        }
            //    }
            //    catch
            //    {
            //        MessageBox.Show("请手动关闭程序，重新尝试！");
            //    }
            //}
            #endregion


            //获取原配置文件xml文档
            if (FileName.Contains("configDataBase.ini"))
            {

                INI readConfigIni = new INI(FileName);

                string webgis_ip = readConfigIni.ReadValue("webGis", "ip");
                string webgis_Catalog = readConfigIni.ReadValue("webGis", "catalog");
                string webgis_User = readConfigIni.ReadValue("webGis", "user");
                string webgis_Pwd = readConfigIni.ReadValue("webGis", "pwd");

                //modify tran


                string tran_IP = readConfigIni.ReadValue("tran", "ip");
                string tran_Catalog = readConfigIni.ReadValue("tran", "catalog");
                string tran_User = readConfigIni.ReadValue("tran", "user");
                string tran_Pwd = readConfigIni.ReadValue("tran", "pwd");
                string tableName = readConfigIni.ReadValue("insertTable", "tableName");
                ModifyPort(readConfigIni.ReadValue("listeningPort", "port"), destPathForConfig);
                ModifyWebGis(webgis_ip, webgis_Catalog, webgis_User, webgis_Pwd, "1433", "MSSQLSERVER", destPathForConfig);
                ModifyTran(tran_IP, tran_Catalog, tran_User, tran_Pwd, "1433", "MSSQLSERVER", destPathForConfig);
                if (tableName != "")
                {
                    modifyPlginForDB(destPathForConfig, readConfigIni);
                }
                createIpInstance(destPathForConfig, readConfigIni);



            }
            if (FileName.Contains("Tran.config"))
            {
                try
                {

                    sourDocument.Load(FileName);

                }
                catch (Exception ex)
                {
                    throw ex;

                }
                //替换基本配置信息

                string keepdays = sourDocument.SelectSingleNode("configuration").SelectSingleNode("General").SelectSingleNode("LogKeepDays").Attributes["value"].Value.Trim().ToString();
                string language = sourDocument.SelectSingleNode("configuration").SelectSingleNode("General").SelectSingleNode("Language").Attributes["value"].Value.Trim().ToString();
                ModifyGeneral(keepdays, language, destPathForConfig);
                string port = sourDocument.SelectSingleNode("configuration").SelectSingleNode("ListeningPort").Attributes["value"].Value.Trim().ToString();
                ModifyPort(port, destPathForConfig);

                string webgis_ip = sourDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis").SelectSingleNode("IP").Attributes["value"].Value.Trim().ToString();
                string webgis_Catalog = sourDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis").SelectSingleNode("Catalog").Attributes["value"].Value.Trim().ToString();
                string webgis_User = sourDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis").SelectSingleNode("User").Attributes["value"].Value.Trim().ToString();
                string webgis_Pwd = sourDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis").SelectSingleNode("Pwd").Attributes["value"].Value.Trim().ToString();
                string webgis_instance = sourDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis").SelectSingleNode("Instance").Attributes["value"].Value.Trim().ToString();
                string webgis_port = sourDocument.SelectSingleNode("configuration").SelectSingleNode("WebGis").SelectSingleNode("Port").Attributes["value"].Value.Trim().ToString();
                ModifyWebGis(webgis_ip, webgis_Catalog, webgis_User, webgis_Pwd, webgis_port, webgis_instance, destPathForConfig);


                string Tran_ip = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Tran").SelectSingleNode("IP").Attributes["value"].Value.Trim().ToString();
                string Tran_Catalog = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Tran").SelectSingleNode("Catalog").Attributes["value"].Value.Trim().ToString();
                string Tran_User = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Tran").SelectSingleNode("User").Attributes["value"].Value.Trim().ToString();
                string Tran_Pwd = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Tran").SelectSingleNode("Pwd").Attributes["value"].Value.Trim().ToString();
                string Tran_instance = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Tran").SelectSingleNode("Instance").Attributes["value"].Value.Trim().ToString();
                string Tran_port = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Tran").SelectSingleNode("Port").Attributes["value"].Value.Trim().ToString();

                ModifyTran(Tran_ip, Tran_Catalog, Tran_User, Tran_Pwd, Tran_port, Tran_instance, destPathForConfig);

                string FlushInterval = sourDocument.SelectSingleNode("configuration").SelectSingleNode("UserInfo").SelectSingleNode("FlushInterval").Attributes["value"].Value.Trim().ToString();
                string enabled_value = sourDocument.SelectSingleNode("configuration").SelectSingleNode("UserInfo").Attributes["enabled"].Value.Trim().ToString();
                ModifyUserInfo(FlushInterval, enabled_value, destPathForConfig);


                XmlNodeList IPTranList = sourDocument.SelectSingleNode("configuration").SelectSingleNode("IPTransfer").SelectNodes("Instance");
                foreach (XmlNode node in IPTranList)
                {
                    string name = node.Attributes["name"].Value.ToString().Trim();
                    string enalbed_value = node.Attributes["enabled"].Value.ToString().Trim();
                    string ip = node.SelectSingleNode("IP").Attributes["value"].Value.ToString().Trim();
                    string port_value = node.SelectSingleNode("Port").Attributes["value"].Value.ToString().Trim();
                    string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim();
                    string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim();
                    var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                    List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                    createIpstance(destPathForConfig, name, ip, port_value, lon, lat, EntityList);

                }



                XmlNodeList DBTranList = sourDocument.SelectSingleNode("configuration").SelectSingleNode("DBTransfer").SelectNodes("Instance");
                foreach (XmlNode node in DBTranList)
                {
                    string name = node.Attributes["name"].Value.ToString().Trim();
                    string enalbed_value = node.Attributes["enabled"].Value.ToString().Trim();
                    string tablename = node.SelectSingleNode("TableName").Attributes["value"].Value.ToString().Trim();
                    string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.ToString().Trim();
                    string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.ToString().Trim();
                    string maxcount = node.SelectSingleNode("MaxCount").Attributes["value"].Value.Trim();
                    var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                    List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                    createDBstance(destPathForConfig, name, tablename, lon, lat, maxcount, EntityList);

                }

                var newDllName = string.Empty;
                XmlNodeList plugins = sourDocument.SelectSingleNode("configuration").SelectSingleNode("Plugin").SelectNodes("Instance");
                foreach (XmlNode node in plugins)
                {
                    string name = node.Attributes["name"].Value.Trim();
                    string dllName = node.SelectSingleNode("Dll").Attributes["name"].Value.Trim();
                    #region 新代码 2015-07-03
                    switch (dllName)
                    {
                        case "台州PGIS对接2":
                        {
                            // xn1.InnerText = "DB_Version_1";
                            newDllName = "DB_Version_3";
                            string tablename = node.SelectSingleNode("TableName").Attributes["value"].Value.Trim();
                            string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                            string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                            string maxcount = node.SelectSingleNode("MaxCount").Attributes["value"].Value.Trim();
                            var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                            List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                            createDBplugin(destPathForConfig, newDllName, lon, lat, maxcount, tablename, name,"0",EntityList);
                            break;
                        };
                        case "内蒙PGIS对接":
                        {
                            //xn1.InnerText = "UDP_Version_1";
                            newDllName = "UDP_Version_1";
                            string Protocol = "PGIS";
                            string NetProtocol = "UDP";
                            string IP = node.SelectSingleNode("IP").Attributes["value"].Value.Trim();
                            string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                            string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                            string por = node.SelectSingleNode("Port").Attributes["value"].Value.Trim();
                            var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                            List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                            createIPPlugin(destPathForConfig, Protocol, IP, por, NetProtocol, lon, lat, name, newDllName,EntityList);
                            break;
                        };
                        case "绍兴PGIS对接":
                        {
                            // xn1.InnerText = "DB_Version_2";
                            newDllName = "DB_Version_2";
                            string tablename = node.SelectSingleNode("TableName").Attributes["value"].Value.Trim();
                            string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                            string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                            string maxcount = node.SelectSingleNode("MaxCount").Attributes["value"].Value.Trim();
                            var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                            List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                            createDBplugin(destPathForConfig, newDllName, lon, lat, maxcount, tablename, name, "0", EntityList);
                            break;
                        }
                        default:
                        {
                            if (dllName.ToLower().Contains("db_version"))
                            {
                                string tablename = node.SelectSingleNode("TableName").Attributes["value"].Value.Trim();
                                string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                                string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                                string maxcount = node.SelectSingleNode("MaxCount").Attributes["value"].Value.Trim();
                                string citycode = string.Empty;
                                if (node.SelectSingleNode("CityCode") != null)
                                {
  //citycode = node.SelectSingleNode("CityCode").Value.Trim();//xzj--20190417--注释,改为下一行，先获取Attributes再获取Value
                                    citycode = node.SelectSingleNode("CityCode").Attributes["value"].Value.Trim();
                                }
                                else
                                {
                                    citycode = "0";
                                }
                                var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                                List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                                createDBplugin(destPathForConfig, dllName, lon, lat, maxcount, tablename, name, citycode, EntityList);
                            }
                            else
                            {
                                string Protocol = "PGIS";
                                string NetProtocol = "UDP";
                                string IP = node.SelectSingleNode("IP").Attributes["value"].Value.Trim();
                                string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                                string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                                string por = node.SelectSingleNode("Port").Attributes["value"].Value.Trim();
                                var ls = node.SelectSingleNode("EntityID").SelectNodes("Add");
                                List<int> EntityList = (from XmlNode no in ls select Convert.ToInt32(no.Attributes["value"].Value.Trim())).ToList();
                                createIPPlugin(destPathForConfig, Protocol, IP, por, NetProtocol, lon, lat, name, dllName,EntityList);
                            }
                            break;
                        }
                    }
                    #endregion
                    #region 旧代码
                    //if (dllName == "台州PGIS对接2")
                    //{
                    //    // xn1.InnerText = "DB_Version_1";
                    //    string newDllName = "DB_Version_3";
                    //    string tablename = node.SelectSingleNode("TableName").Attributes["value"].Value.Trim();
                    //    string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                    //    string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                    //    string maxcount = node.SelectSingleNode("MaxCount").Attributes["value"].Value.Trim();
                    //    createDBplugin(destPathForConfig, newDllName, lon, lat, maxcount, tablename, name);

                    //}
                    //if (dllName == "绍兴PGIS对接")
                    //{
                    //    // xn1.InnerText = "DB_Version_2";
                    //    string newDllName = "DB_Version_2";
                    //    string tablename = node.SelectSingleNode("TableName").Attributes["value"].Value.Trim();
                    //    string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                    //    string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                    //    string maxcount = node.SelectSingleNode("MaxCount").Attributes["value"].Value.Trim();
                    //    createDBplugin(destPathForConfig, newDllName, lon, lat, maxcount, tablename, name);

                    //}
                    //if (dllName == "内蒙PGIS对接")
                    //{
                    //    //xn1.InnerText = "UDP_Version_1";
                    //    string newDllName = "UDP_Version_1";
                    //    string Protocol = "PGIS";
                    //    string NetProtocol = "UDP";
                    //    string IP = node.SelectSingleNode("IP").Attributes["value"].Value.Trim();
                    //    string lon = node.SelectSingleNode("LonOffset").Attributes["value"].Value.Trim();
                    //    string lat = node.SelectSingleNode("LatOffset").Attributes["value"].Value.Trim();
                    //    string por = node.SelectSingleNode("Port").Attributes["value"].Value.Trim();
                    //    createIPPlugin(destPathForConfig, Protocol, IP, por, NetProtocol, lon, lat, name, newDllName);
                    //}
                    #endregion
                }
            }
            threadTag = 1;
        }
        //---------------------------------
       private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("configDataBase.ini") || textBox1.Text.Contains("Tran.config"))
            {
                ///modify 2015-07-03
                ///先关闭程序，后启动线程
                KillProcess();
                ThreadStart dealWithConfig_start = new ThreadStart(dealWithConfig);
                Thread dealWithConfig_thread = new Thread(dealWithConfig_start);
                dealWithConfig_thread.Start();

                string destDir = AppDomain.CurrentDomain.BaseDirectory + "\\Release";
                string sourDir_true = FileName.Substring(0, FileName.LastIndexOf("\\"));
                FIlesCopy fileCopy = new FIlesCopy(destDir, sourDir_true); //注意拷贝文件的 源和目的 文件夹
                ParameterizedThreadStart ParStart = new ParameterizedThreadStart(filesCopy);
                Thread filesCopyThread = new Thread(ParStart);
                        filesCopyThread.Start(fileCopy);
            }
            else
            {
                button2.Enabled = false;
                MessageBox.Show(@"路径名有误请重新选择！");
            }
  
        }

        void KillProcess()
        {
            var p = Process.GetProcesses();
            foreach (var p1 in p)
            {
                try
                {
                    var processName = p1.ProcessName.Trim();
                    
                    //判断是否包含阻碍更新的进程
                    if (processName.Equals("gpstran",StringComparison.CurrentCultureIgnoreCase))
                    {
                        p1.Kill();
                        break;
                    }
                }
                catch
                {
                    MessageBox.Show("请手动关闭程序，重新尝试！");
                    break;
                }
            }
        }

       private void button4_Click(object sender, EventArgs e)
       {
           Form form4 = new Form4();
           this.Close();
           // System.Diagnostics.Process.GetCurrentProcess().Close();
           form4.Show();
         
       }

       private void textBox1_TextChanged(object sender, EventArgs e)
       {
           if (textBox1.Text.Contains("configDataBase.ini") || textBox1.Text.Contains("Tran.config"))
           {
               button2.Enabled = true;
           }
           else
           {
               button2.Enabled = false;
               MessageBox.Show("路径名有误请重新选择！");
              

           }
       }

       private void Form2_Load(object sender, EventArgs e)
       {

       }
   
    
    }
}
