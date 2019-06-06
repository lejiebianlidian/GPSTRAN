using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using Common;
using System.Text.RegularExpressions;
namespace GPSTran
{
    public partial class Form1 : Form
    {
        //threads
        public static UserInfoThread userInfoThread;
        public static GroupInfoThread groupInfoThread;
        public static CommonTableThread commonTableThread;
        public static IPTransferThread ipTransferThread;
        public static EntityAndIssiCacheThread entityAndIssiThread;
        public static UDPReceiveThread udpReceiveThread;
        public static CommonPluginThread PluginExecuteThread;
        public static TableMaintenanceThread tableMaintenanceThread;
        #region old code
        ////listening UDP port
        //public static int ListeningPort;
        ////queue for inserting GPSData,for DBTransfer
        //public static ConcurrentQueue<GPSData> GPSDataQueue = new ConcurrentQueue<GPSData>();
        ////queue for inserting GPSData,for Special
        //public static Dictionary<string, ConcurrentQueue<GPSData>> RegGPSDataQueue = new Dictionary<string, ConcurrentQueue<GPSData>>();
        ////declare lock of RegGPSDataQueue
        //public static readonly Object lckRegGPSDataQueue = new Object();

        ////dictionary of all  arguments from .config file
        //public static Dictionary<string, IPModel> IPList = new Dictionary<string, IPModel>();
        //public static Dictionary<string, DBModel> DBList = new Dictionary<string, DBModel>();
        //public static Dictionary<string, SpecialDBModel> SpecialList = new Dictionary<string, SpecialDBModel>();
        


        ////queue for inserting byte[] from UDP
        //public static ConcurrentQueue<byte[]> GPSByteQueue = new ConcurrentQueue<byte[]>();
        ////dictionary of enabled arguments from .config file
        //public static Dictionary<string, IPModel> IPInstanceList = new Dictionary<string, IPModel>();
        //public static Dictionary<string, DBModel> DBInstanceList = new Dictionary<string, DBModel>();
        //public static Dictionary<string, SpecialDBModel> SpecialDB = new Dictionary<string, SpecialDBModel>();
        ////lock for IPInstanceList
        //public static Object lckIPInstanceList = new Object();
        ////lock for DBInstanceList
        //public static Object lckDBInstanceList = new Object();
        ////lock for SpecialDB
        //public static Object lckSpecialDB = new Object();
        ///// <summary>
        ///// EGIS database server address
        ///// </summary>
        //public static string WebGisIp;
        //public static string WebGisDb;
        //public static string WebGisUser;
        //public static string WebGisPwd;
        //public static string WebGisConn;
        ///// <summary>
        ///// GPSTran database server address
        ///// </summary>
        //public static string TranIp;
        //public static string TranDb;
        //public static string TranUser;
        //public static string TranPwd;
        //public static string TranConn;

        ///// <summary>
        ///// Userinfo args
        ///// </summary>
        //public static int FlushInterval;
        //public static bool UserinfoEnabled;

        ////durable sections
        //public static string[] DurableSections=new string[]{"LISTENINGPORT","WEBGIS","USERINFO","GPSTRAN","SPECIAL"};
        //////UDP Address to send GPS data
        ////public static Dictionary<IPEndPoint, List<double>> UDPAddressList = new Dictionary<IPEndPoint, List<double>>();
        //////TCP Address to send GPS data ,this struct contains IPEndPoint、transfer protocol(TCP or UDP)、 entityId and GPS offset
        ////public static Dictionary<IPEndPoint, Dictionary<string, List<double>>> TCPAddressList = new Dictionary<IPEndPoint, Dictionary<string, List<double>>>();
        
      
        //// cache of entityid and device id
        //public static Dictionary<string, Dictionary<string, string>> ISSIOfEntity = new Dictionary<string, Dictionary<string, string>>();
        ////declare lock of ISSIOfEntity
        //public static readonly Object lckISSIOfEntity = new Object();

        //public static Dictionary<string, int> SendStatisticsList = new Dictionary<string, int>();

        //private static readonly Object lckSendStatistics = new Object();
        ////insert total statistics
        //public static Dictionary<string, int> InsertStatisticsList = new Dictionary<string, int>();
        ////lock for InsertStatisticsList
        //public static readonly Object lckInsertStatistics = new Object();
        ////general log enabled
        //public static bool GPSLogEnabled=false;

        ////debug log enabled
        //public static bool GPSDebugEnabled = false;

        ////thread list
        //public static Dictionary<string, IDBThread> DBThreadPool = new Dictionary<string, IDBThread>();

        ////show table or ip status
        //public static Dictionary<string, int> DBStatus = new Dictionary<string, int>();
      
        ////lock for DBStatus 
        //public static readonly Object lckDBStatus = new Object();
      

        //public static int MaxTranLimit = 9999;
        //public static int MinTranLimit = 1000;
        ///// <summary>
        ///// Init queue for every special table
        ///// </summary>
        //public static void RegistSpecialTable() 
        //{
        //    lock (lckRegGPSDataQueue)
        //    {

        //        //clear invalid key from RegGPSDataQueue
        //        foreach (string s in RegGPSDataQueue.Keys)
        //        {
        //            if (!SpecialDB.ContainsKey(s)) 
        //            {
        //                RegGPSDataQueue.Remove(s);
                    
        //            }
                
        //        }


        //        //add not existed keys to RegGPSDataQueue
        //        foreach (string s in SpecialDB.Keys)
        //        {
        //            if (SpecialDB[s].Enabled)
        //            { 
        //                if (!RegGPSDataQueue.ContainsKey(s))
        //                {
        //                    RegGPSDataQueue.Add(s, new ConcurrentQueue<GPSData>());
        //                }

        //            }

        //        } 
               
        //    }
        
        //}
        //public static void RegistStatistics() 
        //{

        //    lock (lckInsertStatistics) 
        //    {
        //        foreach (string s in InsertStatisticsList.Keys) 
        //        {
        //            if (SpecialDB.ContainsKey(s)) 
        //            {
        //                continue;
                    
        //            }

        //            if (DBInstanceList.ContainsKey(s)) 
        //            {
        //                continue;
        //            }

        //            InsertStatisticsList.Remove(s);
                
        //        }

        //        foreach (string s in DBInstanceList.Keys)
        //        {
        //            if (InsertStatisticsList.ContainsKey(s)) 
        //            {
        //                continue;
        //            }
        //            InsertStatisticsList.Add(s, 0);
                
        //        }
        //        foreach (string s in SpecialDB.Keys) 
        //        {
        //            if (InsertStatisticsList.ContainsKey(s))
        //            {
        //                continue;
        //            }
        //            InsertStatisticsList.Add(s, 0);
                
        //        }

        //    }

        //    lock (lckSendStatistics) 
        //    {
              
        //        foreach (string s in IPInstanceList.Keys) 
        //        {
        //            if (SendStatisticsList.ContainsKey(s))
        //            {
        //                continue;    
                    
        //            }
        //            SendStatisticsList.Add(s, 0);
                
        //        }

        //        foreach (string s in SendStatisticsList.Keys) 
        //        {
        //            if (IPInstanceList.ContainsKey(s))
        //            {
        //                continue;
        //            }
        //            SendStatisticsList.Remove(s);
        //        }

        //    }


        
        //}
        ////init table status for all tables
        //public static void RegistStatus() 
        //{
        //    lock (lckDBStatus) 
        //    {
        //        DBStatus.Clear();
        //        foreach (string s in SpecialDB.Keys) 
        //        {
        //            if (!DBStatus.ContainsKey(s)) 
        //            {
        //                DBStatus.Add(s, 1);
        //            }
        //        }
        //        foreach (string s in DBInstanceList.Keys) 
        //        {
        //            if (!DBStatus.ContainsKey(s)) 
        //            {
        //                DBStatus.Add(s, 1);
        //            }
                
        //        }

        //    }
        
        
        //}

        //public static void Register() 
        //{
        //    RegistSpecialTable();
        //    RegistStatistics();
        //    RegistStatus();
          
        
        //}
        //check database: Dagdb_Tran 

        //config form
#endregion
        ConfigForm configForm;

        /*check database connection*/
        public  int CheckDB() 
        {
            string conn = "Data Source=" + Resource.TranIp+ ","+Resource.TranPort+"\\"+Resource.TranInstance+ ";Initial Catalog=master;uid=" + Resource.TranUser + ";pwd=" + Resource.TranPwd;
            string sql="select count(0) as count from master.sys.sysdatabases where name =@name";
            var TestHelper = new SqlHelper(conn);

            string webGisConn = "Data Source=" + Resource.WebGisIp + "," + Resource.WebGisPort + "\\" + Resource.WebGisDb + ";Initial Catalog=master;uid=" + Resource.WebGisUser + ";pwd=" + Resource.WebGisPwd;
            //check webgis db
            try
            {
                var testHelper = new SqlHelper(webGisConn);

                bool result = testHelper.TestCon();
                if (!result) 
                {
                    Logger.Error("webgis database config error ,can't connect to webgis db");
                    return 0;
                
                }


            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when check webgis database,error message: " + ex.ToString());
                return 0;
            
            }


            try
            {
                DataTable dt = TestHelper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@name", Resource.TranDb) });
                DataRow[] dr = dt.Select();

                if (Convert.ToInt32(dr[0]["count"]) <= 0) 
                {
                    Logger.Warn("database "+Resource.TranDb+" does not exists,try to create...");
                    //try to create database Dagdb_Tran
                    sql = string.Format("create database {0} \n alter database {0} collate Chinese_PRC_CI_AS \n alter database {0} set recovery simple ", Resource.TranDb);
                    try
                    {
                        TestHelper.ExecuteNonQuery(CommandType.Text, sql);
                    }
                    catch (SqlException ex) 
                    {
                        Logger.Error("try to create database "+Resource.TranDb+" failed");
                        Logger.Error(ex.ToString());
                        return 0;
                    }
                    //try to add login eastcom,try to add user dbtran,try to grant db_owner to dbtran
                    sql = "use  "+Resource.TranDb+" \n" +
                            "if exists(select * from Dagdb_Tran.sys.sysusers where name='dbtran') \n" +
                            "begin \n" +
                            "exec sp_revokedbaccess 'dbtran' \n" +
                            "end \n" +
                            "if exists(select * from master.sys.syslogins where name='eastcom') \n" +
                            "begin \n" +
                            "exec sp_droplogin 'eastcom' \n" +
                            "end \n" +
                            "exec sp_addlogin 'eastcom','123456','Dagdb_Tran' \n" +
                            "exec sp_grantdbaccess 'eastcom','dbtran' \n" +
                            "exec sp_addrolemember 'db_owner','dbtran' \n"+
                            "alter user dbtran with default_schema =dbo \n";

                    try
                    {
                        TestHelper.ExecuteNonQuery(CommandType.Text, sql);

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("try to create login failed,try to create user failed,try to grant db_owner to dbtran failed,error message"+ex.ToString());
                        return 0;
                    }

                }

            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when check tran database,error message: "+ex.ToString());
                return 0;
            }
            return 1;

        }
        //check login:eastcom
        public int CheckLogin() 
        {
            //test login eastcom 
            string conn = "Data Source=" + Resource.TranIp + ";Initial Catalog="+Resource.TranDb+";uid=eastcom;pwd=123456";
            string conn2 = "Data Source=" + Resource.TranIp + ";Initial Catalog=master;uid="+Resource.TranUser+";pwd="+Resource.TranPwd;

            string sql= "use Dagdb_Tran \n" +
                            "if exists(select * from Dagdb_Tran.sys.sysusers where name='dbtran') \n" +
                            "begin \n" +
                            "exec sp_revokedbaccess 'dbtran' \n" +
                            "end \n" +
                            "if exists(select * from master.sys.syslogins where name='eastcom') \n" +
                            "begin \n" +
                            "exec sp_droplogin 'eastcom' \n" +
                            "end \n" +
                            "exec sp_addlogin 'eastcom','123456','Dagdb_Tran' \n" +
                            "exec sp_grantdbaccess 'eastcom','dbtran' \n" +
                           "exec sp_addrolemember 'db_owner','dbtran' \n" +
                            "alter user dbtran with default_schema =dbo \n";

            var TestHelper = new SqlHelper(conn);
            var TestHelper2 = new SqlHelper(conn2);
            try
            {
                bool result = TestHelper.TestCon();
                if (result)
                {
                    return 1;
                }
                else 
                {
                    //try to create login:eastcom
                    try
                    {
                        //if eastcom can't login,recreate login eastcom
                        return TestHelper2.ExecuteNonQuery(CommandType.Text, sql);

                    }
                    catch (Exception ex) 
                    {
                        Logger.Error("try to create login failed,try to create user failed,try to grant db_owner to dbtran failed，error message"+ex.ToString());
                        return 0;
                    
                    }
                    

                }


            }
            catch (Exception ex) 
            {
                Logger.Error("Error occur when test db connection,connectionString:"+conn+"\n error message"+ex.ToString());
                return 0;
            }

        }


        public Form1()
        {
           
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
         
            this.PortLabel.Text = Resource.lHelper.Key("n12")+"：     " + Resource.ListeningPort;
            this.timer1.Enabled = false;
            this.StartItem.Text = Resource.lHelper.Key("n2");
            this.StartBtn.Text = Resource.lHelper.Key("n2");
            this.ConfigItem.Text = Resource.lHelper.Key("n1");
            this.ConfigMenuItem.Text = Resource.lHelper.Key("n1");
            this.DebugItem.Text = Resource.lHelper.Key("n5");
            this.DebugCheck.Text=Resource.lHelper.Key("n15");
            this.GPSLogItem.Text = Resource.lHelper.Key("n4");
            this.GPSLogCheck.Text = Resource.lHelper.Key("n14");
            this.OperateMenuItem.Text = Resource.lHelper.Key("n3");
            this.Config_import.Text = Resource.lHelper.Key("m74");
            this.Config_Export.Text = Resource.lHelper.Key("m76");
            this.Exit.Text = Resource.lHelper.Key("n6");
            this.SafetyExit.Text = Resource.lHelper.Key("n7");
            this.HelpMenuItem.Text = Resource.lHelper.Key("n8");
            this.InstructionMenuItem.Text = Resource.lHelper.Key("n74");
            this.protolBaseMenuItem.Text = Resource.lHelper.Key("n75");
            this.AboutMenuItem.Text = Resource.lHelper.Key("n10");
            this.groupBox1.Text = Resource.lHelper.Key("n11");
            this.groupBox2.Text = Resource.lHelper.Key("n13");
            this.GPSStatisticsLabel.Text = Resource.lHelper.Key("n16")+"：  "+0;

            this.dataGridView1.Columns["TransferType"].HeaderText = Resource.lHelper.Key("n17");
            //this.dataGridView1.Columns["Protocol"].HeaderText = Resource.lHelper.Key("n18");
            this.dataGridView1.Columns["Destination"].HeaderText = Resource.lHelper.Key("n19");
            this.dataGridView1.Columns["Status"].HeaderText = Resource.lHelper.Key("n20");
            this.dataGridView1.Columns["EntityID"].HeaderText = Resource.lHelper.Key("n24");
            this.dataGridView1.Columns["Statistics"].HeaderText = Resource.lHelper.Key("n25");
            this.cacheStatus.Text = Resource.lHelper.Key("n65") + "：";
            this.cacheStatusValue.Text=Resource.lHelper.Key("n21");
            this.cacheStatusValue.ForeColor = System.Drawing.Color.Green;
            //start threads
            Start();
           

        }
        //method to start
        private void Start() 
        {
            StringBuilder sb=new StringBuilder("false");

            //Open a window to check database connection
            DBDetectForm dbForm = new DBDetectForm(ref sb);

            dbForm.ShowDialog();

            if (sb.ToString().Equals("false")) 
            {
                MessageBox.Show( Resource.lHelper.Key("m3"));
                StartBtn.Enabled = true;
                this.StartItem.Enabled = true;
                //variable to identify wether db connection is OK
                Resource.DBCheck = false;
                return;
            }
            //variable to identify wether db connection is OK
            Resource.DBCheck = true;
           

            if (StartThread() <= 0)
            {
                MessageBox.Show( Resource.lHelper.Key("m4"));
                StartBtn.Enabled = true;
                this.StartItem.Enabled = true;
                return;
            }
            StartBtn.Enabled = false;
            this.StartItem.Enabled = false;
            this.timer1.Enabled = true;
        
        }

        //start thread
        private int StartThread() 
        {
           //xzj--20190201--oracle--groupInfoThread，userInfoThread，tableMaintenanceThread 如果是oracle则无需开启线程
            if (Resource.TranInstance.Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
            {
                groupInfoThread = new GroupInfoThread();
                userInfoThread = new UserInfoThread();
                tableMaintenanceThread = new TableMaintenanceThread();
            }
            commonTableThread = new CommonTableThread();
            ipTransferThread = new IPTransferThread();
           
            entityAndIssiThread = new EntityAndIssiCacheThread();
            udpReceiveThread = new UDPReceiveThread();
            PluginExecuteThread = new CommonPluginThread();
              //xzj--20190201--oracle
            if (Resource.TranInstance.Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
            {
            if (groupInfoThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
            {
                if (groupInfoThread.ThreadStatus == System.Threading.ThreadState.Suspended)
                {
                    groupInfoThread.BeginResume();
                }
            }
            else
            {
                if (groupInfoThread.Start() <= 0)
                {
                    return 0;
                }
            }

            if (userInfoThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
           {
               if (userInfoThread.ThreadStatus == System.Threading.ThreadState.Suspended) 
               {
                   userInfoThread.BeginResume();
               }
           }
           else 
           {
               if (userInfoThread.Start() <= 0)
               {
                   return 0;

                }
                }
                if (tableMaintenanceThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
                {
                    if (tableMaintenanceThread.ThreadStatus == System.Threading.ThreadState.Suspended)
                    {
                        tableMaintenanceThread.BeginResume();
                    }
                }
                else
                {
                    if (tableMaintenanceThread.Start() <= 0)
                    {
                        return 0;

                    }               
                    }
           }
            if (commonTableThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
           {
               if (commonTableThread.ThreadStatus == System.Threading.ThreadState.Suspended)
               {
                   commonTableThread.BeginResume();
               }
           }
           else
           {
               if (commonTableThread.Start() <= 0)
               {
                   return 0;

               }

           }

            if (ipTransferThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
           {
               if (ipTransferThread.ThreadStatus == System.Threading.ThreadState.Suspended)
               {
                   ipTransferThread.BeginResume();
               }
           }
           else
           {
               if (ipTransferThread.Start() <= 0)
               {
                   return 0;

               }

           }
            if (entityAndIssiThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
           {
               if (entityAndIssiThread.ThreadStatus == System.Threading.ThreadState.Suspended)
               {
                   entityAndIssiThread.BeginResume();
               }
           }
           else
           {
               if (entityAndIssiThread.Start() <= 0)
               {
                   return 0;

               }

           }

            if (udpReceiveThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
           {
               if (udpReceiveThread.ThreadStatus == System.Threading.ThreadState.Suspended)
               {
                   udpReceiveThread.BeginResume();
               }
           }
           else
           {
               if (udpReceiveThread.Start() <= 0)
               {
                   return 0;

               }
           }
            if (PluginExecuteThread.ThreadStatus != System.Threading.ThreadState.Unstarted)
            {
                if (PluginExecuteThread.ThreadStatus == System.Threading.ThreadState.Suspended)
                {
                    PluginExecuteThread.BeginResume();
                }
            }
            else
            {
                if (PluginExecuteThread.Start() <= 0)
                {
                    return 0;

                }
            }


            LogWatcherStart();
            

            return 1;
        }

        //log file management,listen file creating,and delete old log file
        private void LogWatcherStart() 
        {
            //init file monitor
            FileSystemWatcher logWatcher = new System.IO.FileSystemWatcher();
            //watch path
            logWatcher.Path = Resource.logPath;

            //wether to watch on sub directory
            logWatcher.IncludeSubdirectories = false;

            logWatcher.NotifyFilter = NotifyFilters.FileName;

            logWatcher.Created += new System.IO.FileSystemEventHandler(fileSystemWatcher_EventHandle);

            logWatcher.EnableRaisingEvents = true;
        
        }
        //event handler when create file 
        private void fileSystemWatcher_EventHandle(object sender, FileSystemEventArgs e) 
        {
            //get split string between yyyy and MM,for example:yyyy-MM-dd ,then split1="-",split2="-"
            string split1 = Resource.logFilePattern.Substring(Resource.logFilePattern.LastIndexOf("y") + 1, Resource.logFilePattern.IndexOf("M") - Resource.logFilePattern.LastIndexOf("y") - 1);
            //get split string between MM and dd
            string split2 = Resource.logFilePattern.Substring(Resource.logFilePattern.LastIndexOf("M") + 1, Resource.logFilePattern.IndexOf("d") - Resource.logFilePattern.LastIndexOf("M") - 1);
            if (string.IsNullOrEmpty(split1)) 
            {
                split1 = "";
            }
            if (string.IsNullOrEmpty(split2)) 
            {
                split2 = "";
            }

            
            string mathPattern = @"^\d{4}"+split1+@"\d{2}"+split2+@"\d{2}" + Resource.logFileFormat;
            List<string> pathList = new List<string>();
            
            try
            {
                string[] fileName = Directory.GetFiles(Resource.logPath, "*" + Resource.logFileFormat);
                //find path witch match file name pattern
                foreach (string s in fileName)
                {
                    if (Regex.IsMatch(s.Substring(s.LastIndexOf("\\") + 1), mathPattern)&&(s.Substring(s.LastIndexOf("\\")+1).Length==(Resource.logFilePattern+Resource.logFileFormat).Length))
                    {
                        pathList.Add(s);
                    }

                }

            }
            catch (Exception ex) 
            {
                Logger.Error("error occur when deleting log file,error message:"+ex.ToString());
                return;
            
            }
          

            string delPath;
            if (pathList.Count <= Resource.logKeepDays) 
            {
                return;
            
            }
            
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

            dtFormat.ShortDatePattern = Resource.logFilePattern;
            List<DateTime> timeList = new List<DateTime>();
            try
            {

                foreach (string s in pathList)
                {
                    DateTime dt;
                    dt = Convert.ToDateTime(s.Substring(s.LastIndexOf("\\") + 1, s.LastIndexOf(".") - s.LastIndexOf("\\") - 1), dtFormat);
                    timeList.Add(dt);

                }

                timeList.Sort();
            }
            catch (Exception ex) 
            {
                Logger.Error("unknown log file pattern,deletion failed,error message:"+ex.ToString());
                return;
            }

            for (int i = 0; i < timeList.Count - Resource.logKeepDays; i++) 
            {
                delPath = Resource.logPath + timeList[i].ToString(Resource.logFilePattern)+Resource.logFileFormat;
                if (System.IO.File.Exists(delPath))
                {
                    System.IO.File.Delete(delPath);
                    Logger.Info("clear log file,path:" + delPath);
                }

            }

        }




        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.ShowInTaskbar == false)
                notifyIcon1.Visible = true;

            this.notifyIcon1.Visible = true;
            this.ShowInTaskbar = true; this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dlgResult = MessageBox.Show(Resource.lHelper.Key("m2"), Resource.lHelper.Key("m1"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
            if (dlgResult != DialogResult.Yes)
            {
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = true;
                this.Hide();
                
                this.ShowInTaskbar = false;
                e.Cancel = true;
            }
            else 
            {
                ExitForm exitForm = new ExitForm();
                exitForm.ShowDialog();
            
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExitToolStripMenuItem.Enabled = false;
            this.SafetyExit.Enabled = false;
            ExitForm exitForm = new ExitForm();
            exitForm.ShowDialog();

        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = true;
            this.ShowInTaskbar = true;
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }
        //show status in form,show thread healthy
        private void ShowStatus() 
        {
            
            this.dataGridView1.Rows.Clear();

            if ((Resource.IPInstanceList.Count + Resource.DBInstanceList.Count + Resource.PluginInstanceList.Count) <= 0) 
            {
                return;
            }

            dataGridView1.Rows.Add(Resource.IPInstanceList.Count + Resource.DBInstanceList.Count+Resource.PluginInstanceList.Count);
            int i = 0;
            foreach (IPManager ipManager in Resource.IPInstanceList.Values) 
            {
                dataGridView1.Rows[i].Cells["TransferType"].Value = Resource.lHelper.Key("n28");
                //dataGridView1.Rows[i].Cells["Protocol"].Value = ipManager.GetModel().Protocol;
                dataGridView1.Rows[i].Cells["Destination"].Value = ipManager.GetModel().Ip + ":" + ipManager.GetModel().Port.ToString();
                dataGridView1.Rows[i].Cells["EntityID"].Value = GetEntityName(ipManager.GetModel().EntityID);
                if (ipManager.GetStatus()==1)
                {
                    dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n21");
                    dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
                }
                else if (ipManager.GetStatus() == 0)
                {
                    dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n22");
                    dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Red;

                }
                else 
                {
                    dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n23");
                    dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Orange;
                
                }
                dataGridView1.Rows[i].Cells["Statistics"].Value = ipManager.GetStatistics().ToString();
                i++;
            }
            foreach (DBManager dbManager in Resource.DBInstanceList.Values) 
            {
                dataGridView1.Rows[i].Cells["TransferType"].Value = Resource.lHelper.Key("n29");
                dataGridView1.Rows[i].Cells["Protocol"].Value ="/";
                dataGridView1.Rows[i].Cells["Destination"].Value = dbManager.GetModel().TableName;
                dataGridView1.Rows[i].Cells["EntityID"].Value = GetEntityName(dbManager.GetModel().EntityID);
                if (dbManager.GetStatus()== 1)
                {
                    dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n21");
                    dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
                }
                else if (dbManager.GetStatus() == 0)
                {
                    dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n22");
                    dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Red;

                }
                else 
                {
                    dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n23");
                    dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Orange;

                }


                dataGridView1.Rows[i].Cells["Statistics"].Value = dbManager.GetStatistics().ToString();
                i++;
            
            }

            foreach (PluginManager pManager in Resource.PluginInstanceList.Values) 
            {
                PluginModel pModel = pManager.GetPluginModel();
                if (pModel.DllModel.PType == PluginType.RemoteIP) 
                {
                    dataGridView1.Rows[i].Cells["TransferType"].Value = Resource.lHelper.Key("n30");
                    //dataGridView1.Rows[i].Cells["Protocol"].Value = pModel.Protocol;
                    dataGridView1.Rows[i].Cells["Destination"].Value = pModel.Ip + ":" + pModel.Port.ToString();
                    dataGridView1.Rows[i].Cells["EntityID"].Value = GetEntityName(pModel.EntityID);
                    if (pManager.GetStatus()==1)
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n21");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (pManager.GetStatus() == 0)
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n22");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Red;

                    }
                    else 
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n23");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Orange;

                    }

                    dataGridView1.Rows[i].Cells["Statistics"].Value = pManager.GetStatistics();

                }
                else if (pModel.DllModel.PType == PluginType.RemoteDB) 
                {
                    dataGridView1.Rows[i].Cells["TransferType"].Value = Resource.lHelper.Key("n32");
                    dataGridView1.Rows[i].Cells["Protocol"].Value = "/";
                    dataGridView1.Rows[i].Cells["Destination"].Value = pModel.TableName;
                    dataGridView1.Rows[i].Cells["EntityID"].Value = GetEntityName(pModel.EntityID);
                    if (pManager.GetStatus() == 1)
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n21");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (pManager.GetStatus() == 0)
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n22");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Red;

                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n23");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Orange;

                    }
                    dataGridView1.Rows[i].Cells["Statistics"].Value = pManager.GetStatistics();
                   
                }
                else if (pModel.DllModel.PType == PluginType.TranDB) 
                {
                    dataGridView1.Rows[i].Cells["TransferType"].Value = Resource.lHelper.Key("n31");
                    dataGridView1.Rows[i].Cells["Protocol"].Value = "/";
                    dataGridView1.Rows[i].Cells["Destination"].Value = pModel.TableName;
                    dataGridView1.Rows[i].Cells["EntityID"].Value = GetEntityName(pModel.EntityID);
                    if (pManager.GetStatus()==1)
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n21");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (pManager.GetStatus() == 0)
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n22");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Red;

                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells["Status"].Value = Resource.lHelper.Key("n23");
                        dataGridView1.Rows[i].Cells["Status"].Style.ForeColor = System.Drawing.Color.Orange;

                    }
                    dataGridView1.Rows[i].Cells["Statistics"].Value = pManager.GetStatistics();
                   
                }

                i++;
            
            }

           
            if (Resource.TranDelay)
            {
                this.cacheStatus.Text = Resource.lHelper.Key("n65") + "：";
                this.cacheStatusValue.Text = Resource.lHelper.Key("n66");
                this.cacheStatusValue.ForeColor = System.Drawing.Color.Red;
                if (Resource.GPSDebugEnabled) 
                {
                    Logger.Warn("GPS cache queue overflow");
                }

            }
            else 
            {
                this.cacheStatus.Text = Resource.lHelper.Key("n65") + "：";
                this.cacheStatusValue.Text = Resource.lHelper.Key("n21");
                this.cacheStatusValue.ForeColor = System.Drawing.Color.Green;
            }
            this.GPSStatisticsLabel.Text = Resource.lHelper.Key("n16") + "： " + udpReceiveThread.Total.ToString();

        }
        //transfer entity ids to entity names
        private string GetEntityName(List<int> ls)
        {
            var sqlHelper = new SqlHelper(Resource.WebGisConn);
            string s="";
            string sql;
            DataTable dt;
            foreach (int i in ls) 
            {
                sql = "select top 1 Name from Entity where ID = @Entity_ID";
                try
                {
                    dt = sqlHelper.ExecuteDataReader(CommandType.Text, sql,new[] {new SqlParameter("@Entity_ID", i.ToString())});
                    //dt = sqlHelper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@Entity_ID", i.ToString()) });
                    if (dt!=null&&dt.Rows.Count > 0) 
                    {
                        s+= Convert.ToString(dt.Rows[0]["Name"])+" , ";
                    }
                }
                catch (Exception ex) 
                {
                    if (Resource.GPSDebugEnabled) 
                    {
                        Logger.Debug("error occur when select Name from Entity,error message:"+ex.ToString());
                    
                    }
                
                }

            }
            if(s.Length <=0) 
            {
                return "";
            }
           
            return s.Remove(s.LastIndexOf(","));
            
        }
        //transfer entity ids to entity names
        public static string GetEntityName(List<string> ls)
        {
            var sqlHelper = new SqlHelper(Resource.WebGisConn);
            string s = "";
            string sql;
            DataTable dt;
            foreach (string i in ls)
            {
                sql = "select Name from Entity where ID = @Entity_ID";
                try
                {
                    dt = sqlHelper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@Entity_ID", i) });
                    if (dt.Rows.Count > 0)
                    {
                        s += Convert.ToString(dt.Rows[0]["Name"]) + " , ";
                    }
                }
                catch (Exception ex)
                {
                    if (Resource.GPSDebugEnabled)
                    {
                        Logger.Debug("error occur when select Name from Entity,error message:" + ex.ToString());
                        return "";
                    }

                }

            }
            if (s.Length <=1) 
            {
                return "";
            }

            return s.Remove(s.LastIndexOf(","));

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowStatus();
        }

        private void ConfigItem_Click(object sender, EventArgs e)
        {
            if (Resource.configFormExisted)
            {
                configForm.Focus();
            }
            else 
            {
                configForm = new ConfigForm();
                configForm.Show();
                Resource.configFormExisted = true;
            }
           
        }

        private void GPSLogItem_Click(object sender, EventArgs e)
        {
            if (Resource.GPSLogEnabled)
            {
                Resource.GPSLogEnabled = false;
                this.GPSLogItem.Text = Resource.lHelper.Key("4");
                this.GPSLogCheck.Checked = false;
            }
            else 
            {
                Resource.GPSLogEnabled = true;
                this.GPSLogItem.Text = Resource.lHelper.Key("26");
                this.GPSLogCheck.Checked = true;
            }


        }

        private void DebugItem_Click(object sender, EventArgs e)
        {
            if (Resource.GPSDebugEnabled)
            {
                Resource.GPSDebugEnabled = false;
                this.DebugItem.Text = Resource.lHelper.Key("n5");
             

                this.DebugCheck.Checked = false;

            }
            else
            {
                Resource.GPSDebugEnabled = true;
                this.DebugItem.Text = Resource.lHelper.Key("27");
             
                this.DebugCheck.Checked = true;
            }
        }
        //force close
        private void Exit_Click(object sender, EventArgs e)
        {
            var dlgResult = MessageBox.Show(Resource.lHelper.Key("m9"), Resource.lHelper.Key("m1"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dlgResult != DialogResult.Yes)
            {
               
            }
            else
            {
                Logger.Info("Program closed");
                Process.GetCurrentProcess().Kill();
            }
        }
        //safe close: will wait util all thread return
        private void SafetyExit_Click(object sender, EventArgs e)
        {
            this.SafetyExit.Enabled = false;
            this.ExitToolStripMenuItem.Enabled = false;
            ExitForm exitForm = new ExitForm();
            exitForm.ShowDialog();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            Start();
        }
        //open or close gps log
        private void GPSLogCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.GPSLogCheck.Checked)
            {
                Resource.GPSLogEnabled = false;
                this.GPSLogItem.Text = Resource.lHelper.Key("n4");
              
            }
            else
            {
                Resource.GPSLogEnabled = true;
                this.GPSLogItem.Text = Resource.lHelper.Key("n26");
               
            }


        }
        //open or close debug
        private void DebugCheck_CheckedChanged(object sender, EventArgs e)
        {

            if (!this.DebugCheck.Checked)
            {
                Resource.GPSDebugEnabled = false;
                this.DebugItem.Text = Resource.lHelper.Key("n5");
            }
            else
            {
                Resource.GPSDebugEnabled = true;
                this.DebugItem.Text = Resource.lHelper.Key("n27");
            }

        }

        private void StartItem_Click(object sender, EventArgs e)
        {

            Start();

        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();

        }

        private void InstructionMenuItem_Click(object sender, EventArgs e)
        {
            //Help.ShowHelp(this,Path.Combine(CurrentPath));
            var process = new Process();
            var path = Resource.lHelper.Key("language");
            process.StartInfo.FileName = Application.StartupPath + "\\help_doc\\"+path+"\\GPS_HELP.chm";
            process.Start();
        }

        private void protolBaseMenuItem_Click(object sender, EventArgs e)
        {
            var process = new Process();
            var path = Resource.lHelper.Key("language");
            process.StartInfo.FileName =Application.StartupPath + "\\help_doc\\"+path+"\\PROTOCOL_HELP.chm";
            process.Start();
        }

        private void Config_Export_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                RestoreDirectory = true,
                InitialDirectory = @"c:\\",
                Title = Resource.lHelper.Key("m76"),
                FilterIndex = 1,
                Filter = @"Tran.config|*.config",
                FileName = "Tran.config"
            };
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;
            var wc = new WebClient();
            wc.DownloadFileAsync(new Uri(Path.Combine(Application.StartupPath, @"Tran.config")), Path.GetFullPath(sfd.FileName));
            MessageBox.Show(string.Format("{0}:{1}", Resource.lHelper.Key("m76"), Path.GetFullPath(sfd.FileName)), Resource.lHelper.Key("m77"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification, false);
        }

        private void Config_import_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title=Resource.lHelper.Key("m74"),
                InitialDirectory = @"c:\", 
                Filter = @"Tran.config|*.config",
                FileName=@"Tran.config",
            };
            if (ofd.ShowDialog()==DialogResult.Cancel)
            {
                return;
            }
            var xml = new XMLIni(ofd.FileName);
            xml.SaveFilePath();
            MessageBox.Show(Resource.lHelper.Key("m75"), Resource.lHelper.Key("m77"));
            Process.GetCurrentProcess().Kill();
        }

    }
}
