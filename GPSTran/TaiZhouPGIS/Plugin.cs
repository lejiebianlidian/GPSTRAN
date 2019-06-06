using System;
using Common;
using System.Data;
using System.Data.SqlClient;
namespace TaiZhouPGIS
{
   public class Plugin:IPlugin
    {
       private readonly PluginType pType = PluginType.TranDB;
       private PluginModel pModel;
       private readonly Object lckPModel = new Object();
       private int status;
       private long statistics;
       private bool enabled;
       private bool debugEnabled;
       private Protocol pro;
       private SqlHelper helper;
       private int flag = 0;
       private readonly Object lckHelper = new Object();

       public bool IsStop()
       {
           return true;
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

           this.pModel = pModel;
           status = 1;
           statistics = 0;
           enabled = pModel.Enabled;
           debugEnabled = false;
           pro = new Protocol();
           string s = "Data Source=" + pModel.DesIP + "," + pModel.DesPort + "\\" + pModel.DesInstance + ";Initial Catalog=" + pModel.DesCatalog + ";uid=" + pModel.DesUser + ";pwd=" + pModel.DesPwd;
           
           helper = new SqlHelper(s);

           if (enabled)
           {
               if (CheckTable() <= 0)
               {
                   status = 0;
               }
           }

       
       }

       public void SetPluginModel(PluginModel pModel)
       {
           if (pModel == null) 
           {
               throw new NullReferenceException("null argument");
           
           }

            this.pModel = pModel;
            enabled = pModel.Enabled;
            string s = "Data Source=" + pModel.DesIP + "," + pModel.DesPort + "\\" + pModel.DesInstance + ";Initial Catalog=" + pModel.DesCatalog + ";uid=" + pModel.DesUser + ";pwd=" + pModel.DesPwd;
            lock (lckHelper)
            {
                helper = new SqlHelper(s);
            }
           if(enabled)
           {
                if (CheckTable() <= 0) 
                {
                    status = 0;
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
                   dt = helper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@name", pModel.TableName) });
                   count = Convert.ToInt32(dt.Rows[0]["count"]);

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
                   Logger.Info("error occur when check table in InsertCommonThread thread,error message:" + ex.ToString());
                   return 0;
               }



           return 1;
       }
       private int RepairTable(string name)
       {
           string sql = "CREATE TABLE "+pModel.TableName+"("+
                        "[DBID] [decimal](23, 0) NOT NULL,"+
                        "[DB33BM] [nvarchar](20) NOT NULL,"+
                        "[SIMNO] [nvarchar](20) NULL,"+
                        "[STARNUM] [int] NULL,"+
                        "[GTIME] [datetime] NULL,"+
                        "[ATIME] [datetime] NULL,"+
                        "[LONGITUDE] [int] NULL,"+
                        "[LATITUDE] [int] NULL,"+
                        "[SPEED] [int] NULL,"+
                        "[DIR] [int] NULL,"+
                        "CONSTRAINT [PK_"+pModel.TableName+"] PRIMARY KEY CLUSTERED"+ 
                        "("+
                        "[DBID] ASC"+
                        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]"+
                        ") ON [PRIMARY]";


           try
           {
               helper.ExecuteNoQuery(CommandType.Text, sql);

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
           if (!enabled) 
           {
               return;
           }

          //todo:
           GPSData gpsData = pro.ToGPSData(data);
           GPSData temp = pro.GPSDataClone(gpsData);
           decimal DBID;
           string strInfo;
           string DB33BM;
           if (flag > 999) 
           {
               flag = 0;
           }


           try
           {
               DBID = Convert.ToDecimal(DateTime.Now.ToString("yyyyMMddHHmmssfff") + flag.ToString());

               if (gpsData.id.Length < 5)
               {
                   string s = "";
                   for (int i = 0; i < 5 - gpsData.id.Length; i++)
                   {
                       s = "0" + s;
                   }
                   DB33BM = "331000220000" + "3" + s + gpsData.id;

               }
               else if (gpsData.id.Length == 5)
               {
                   DB33BM = "331000220000" + "3" + gpsData.id;
               }
               else
               {
                   string s;
                   s = gpsData.id.Substring(gpsData.id.Length - 5, 5);

                   DB33BM = "331000220000" + "3" + s;
               }
              
               foreach (int i in pModel.EntityID)
               {

                   if (Resource.ISSIOfEntity.ContainsKey(i.ToString()))
                   {
                       lock (Resource.lckISSIOfEntity)
                       {
                           if (Resource.ISSIOfEntity[i.ToString()].ContainsKey(gpsData.id))
                           {
                               temp = pro.ModifyLaLon(ref temp, pModel.LonOffset, pModel.LatOffset);
                               //Logger.Debug(pModel.LonOffset.ToString());
                               //Logger.Debug(temp.lon.ToString());

                               strInfo = "insert into " + pModel.TableName + "(DBID,DB33BM,SIMNO ,STARNUM,GTIME,ATIME,LONGITUDE ,LATITUDE,SPEED,DIR )  values" + "(" + DBID + ",'" + DB33BM + "'," + gpsData.id + "," + "0" + ",'" + gpsData.time + "','" + gpsData.time + "'," + ((Int32)(temp.lon * 1000000)).ToString() + "," + ((Int32)(temp.lat * 1000000)).ToString() + "," + gpsData.speed + "," + gpsData.dir + ")";
                               lock (lckHelper)
                               {
                                   helper.ExecuteNoQuery(CommandType.Text, strInfo);
                                   flag++;
                                   statistics++;

                               }
                               if (Resource.GPSLogEnabled)
                               {
                                   Logger.Info(String.Format("实例：{0} 号码:{1}", pModel.Name, temp.id.ToString()));
                               }
                               break;
                           }
                       }
                   }
               }
               status = 1;

           }
           catch (Exception ex) 
           {
               status = 0;
               if (debugEnabled) 
               {
                   Logger.Error("error occur when execute plugin(TaiZhouPGIS),error message:"+ex.ToString());
               
               }
           
           }






       }



    }
}
