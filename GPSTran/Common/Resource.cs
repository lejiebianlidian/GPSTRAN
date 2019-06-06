using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
    public static  class Resource
    {
        //listening UDP port
        public static int ListeningPort;
        //queue for inserting GPSData,for DBTransfer
        public static ConcurrentQueue<GPSData> GPSDataQueue = new ConcurrentQueue<GPSData>();
        //21
        public static ConcurrentQueue<GPSDataFile> GPSDataQueueFile = new ConcurrentQueue<GPSDataFile>();

        //dictionary of all  arguments from .config file
        public static Dictionary<string, IPManager> IPList = new Dictionary<string, IPManager>();
        public static Dictionary<string, DBManager> DBList = new Dictionary<string, DBManager>();
        //queue for inserting GPSData bytes,for Plugin
        public static ConcurrentQueue<byte[]> PluginDataQueue = new ConcurrentQueue<byte[]>();
        //define protocol length
        public static readonly int ProtocolLength = 44;

        public static int Ptyp;
        //define maxisum queue length
        public static readonly int CacheLength = 1000;

        public static bool TranDelay = false;

        //queue for inserting byte[] from UDP
        public static ConcurrentQueue<byte[]> GPSByteQueue = new ConcurrentQueue<byte[]>();
        //dictionary of enabled arguments from .config file
        public static Dictionary<string, IPManager> IPInstanceList = new Dictionary<string, IPManager>();
        public static Dictionary<string, DBManager> DBInstanceList = new Dictionary<string, DBManager>();
        
        //lock for IPInstanceList
        public static Object lckIPInstanceList = new Object();
        //lock for DBInstanceList
        public static Object lckDBInstanceList = new Object();

        /// <summary>
        /// EGIS database server address
        /// </summary>
        public static string WebGisIp;
        public static string WebGisDb;
        public static string WebGisUser;
        public static string WebGisPwd;
        public static string WebGisConn;
        public static string WebGisInstance;
        public static int WebGisPort;
        /// <summary>
        /// GPSTran database server address
        /// </summary>
        public static string TranIp;
        public static string TranDb;
        public static string TranUser;
        public static string TranPwd;
        public static string TranConn;
        public static string TranInstance;
        public static int TranPort;

        /// <summary>
        /// Userinfo args
        /// </summary>
        public static int FlushInterval;
        public static bool UserinfoEnabled;

        //durable sections
        public static string[] DurableSections = new string[] { "LISTENINGPORT", "WEBGIS", "USERINFO", "GPSTRAN","Plugin","PluginList" };
        ////UDP Address to send GPS data
        //public static Dictionary<IPEndPoint, List<double>> UDPAddressList = new Dictionary<IPEndPoint, List<double>>();
        ////TCP Address to send GPS data ,this struct contains IPEndPoint、transfer protocol(TCP or UDP)、 entityId and GPS offset
        //public static Dictionary<IPEndPoint, Dictionary<string, List<double>>> TCPAddressList = new Dictionary<IPEndPoint, Dictionary<string, List<double>>>();


        // cache of entityid and device id
        public static Dictionary<string, Dictionary<string, string>> ISSIOfEntity = new Dictionary<string, Dictionary<string, string>>();
        //declare lock of ISSIOfEntity
        public static readonly Object lckISSIOfEntity = new Object();

        public static Dictionary<string, long> SendStatisticsList = new Dictionary<string, long>();

        public static readonly Object lckSendStatistics = new Object();
        //insert total statistics
        public static Dictionary<string, long> InsertStatisticsList = new Dictionary<string, long>();
        //lock for InsertStatisticsList
        public static readonly Object lckInsertStatistics = new Object();
        //general log enabled
        public static bool GPSLogEnabled = false;

        //debug log enabled
        public static bool GPSDebugEnabled = false;

        //thread list
        public static Dictionary<string, IThread> DBThreadPool = new Dictionary<string, IThread>();

        //show table or ip status
        public static Dictionary<string, int> DBStatus = new Dictionary<string, int>();

        //lock for DBStatus 
        public static readonly Object lckDBStatus = new Object();

        //plugin list
        public static Dictionary<string, DllModel> DllList = new Dictionary<string, DllModel>();

        //plugin instance list which enabled=true
        public static Dictionary<string, PluginManager> PluginInstanceList = new Dictionary<string, PluginManager>();
        //plugin instance list which enabled=false
        public static Dictionary<string, PluginManager> PluginList = new Dictionary<string, PluginManager>();
        
        //lock for PluginInstanceList
        public static readonly Object lckPluginInstanceList = new Object();
        //lock for PluginList
        public static readonly Object lckPluginList = new Object();


        public static int MaxTranLimit = 9999;
        public static int MinTranLimit = 1000;

        public static LanguageHelper lHelper;

        //supported language
        public static string[] LanguageList = new string[] { "English", "简体中文" };
        
        public static bool IsAutoRun;

        public static bool DBCheck;

        //wether config form is existed
        public static bool configFormExisted=false;

        //log file path
        public static string logPath;

        //log file keep days
        public static int logKeepDays;

        //log file format
        public static string logFileFormat;

        //log file pattern
        public static string logFilePattern;

        //public static void RegistStatistics()
        //{

        //    lock (lckInsertStatistics)
        //    {
              
        //        foreach (string s in DBInstanceList.Keys)
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

        //        for (int n = 0; n < SendStatisticsList.Count; n++)
        //        {
        //            int flag = 0;
        //            for (int i = 0; i < IPInstanceList.Count; i++)
        //            {
        //                if (IPInstanceList.ElementAt(i).Key.Equals(SendStatisticsList.ElementAt(n).Key))
        //                {
        //                    flag = 1;
        //                    break;
        //                }
        //            }
        //            if (flag ==0)
        //            {
        //                SendStatisticsList.Remove(SendStatisticsList.ElementAt(n).Key);
        //            }
        //        }
        //    }



        //}
        ////init table status for all tables
        //public static void RegistStatus()
        //{
        //    lock (lckDBStatus)
        //    {
        //        DBStatus.Clear();
               
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
            
        //    RegistStatistics();
        //    RegistStatus();

        //}
    }
}
