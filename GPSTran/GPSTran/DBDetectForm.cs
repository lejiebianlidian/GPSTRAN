using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GPSTran
{
    public partial class DBDetectForm : Form
    {
        private StringBuilder sb;

        public DBDetectForm(ref StringBuilder sb)
        {
            InitializeComponent();
            this.sb = sb;
            this.dbTitle.Text = Resource.lHelper.Key("m13");

        }
        delegate void GetValue(int i);
        private void ValueChange(int i) 
        {
            this.progressBar1.Value = i;

            if (i == 100) 
            {
                this.Close();
            }
        
        }

        private void DBDetect() 
        {
            GetValue gv = ValueChange;
            //check database and login
            this.Invoke(gv, 10);
            if (CheckDB() <= 0)
            {
                this.Invoke(gv, 30);
                Thread.Sleep(200);
                sb.Clear();
                sb.Append("false");
                this.Invoke(gv, 100);
                return;
               
            }
            this.Invoke(gv, 30);
            Thread.Sleep(200);
            if (CheckLogin() <= 0)
            {
                this.Invoke(gv, 50);
                Thread.Sleep(200);
                sb.Clear();
                sb.Append("false");
                this.Invoke(gv, 100);
                
                return;
            }
            sb.Clear();
            sb.Append("true");
            this.Invoke(gv, 80);
            Thread.Sleep(200);

            this.Invoke(gv, 100);
        }

        public int CheckDB()
        {
            string conn = "Data Source=" + Resource.TranIp + ","+Resource.TranPort+"\\"+Resource.TranInstance+";Initial Catalog=master;uid=" + Resource.TranUser + ";pwd=" + Resource.TranPwd;
            string sql = "select count(0) as count from master.sys.sysdatabases where name =@name";
            SqlHelper TestHelper = new SqlHelper(conn);

            string webGisConn = "Data Source=" + Resource.WebGisIp + "," + Resource.WebGisPort + "\\" + Resource.WebGisInstance + ";Initial Catalog=master;uid=" + Resource.WebGisUser + ";pwd=" + Resource.WebGisPwd;
            //check webgis db
            try
            {
                var testHelper = new SqlHelper(webGisConn);

                var result=testHelper.TestCon();
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
            //xzj--20190201--排除oracle判断
            if (Resource.TranInstance.Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
            {
            try
            {
                DataTable dt = TestHelper.ExecuteRead(CommandType.Text, sql, "test", new SqlParameter[] { new SqlParameter("@name", Resource.TranDb) });
                DataRow[] dr = dt.Select();

                if (Convert.ToInt32(dr[0]["count"]) <= 0)
                {
                    Logger.Warn("database " + Resource.TranDb + " does not exists,try to create...");
                    //try to create database Dagdb_Tran
                    sql = string.Format("create database {0} \n alter database {0} collate Chinese_PRC_CI_AS \n alter database {0} set recovery simple ", Resource.TranDb);
                    try
                    {
                        TestHelper.ExecuteNonQueryNonTran(CommandType.Text, sql);
                    }
                    catch (SqlException ex)
                    {
                        Logger.Error("try to create database " + Resource.TranDb + " failed");
                        Logger.Error(ex.ToString());
                        return 0;
                    }
                    //try to add login eastcom,try to add user dbtran,try to grant db_owner to dbtran
                    sql = "use  " + Resource.TranDb + " \n" +
                            "if exists(select * from sys.sysusers where name='dbtran') \n" +
                            "begin \n" +
                            "exec sp_revokedbaccess 'dbtran' \n" +
                            "end \n" +
                            "if exists(select * from master.sys.syslogins where name='eastcom') \n" +
                            "begin \n" +
                            "exec sp_droplogin 'eastcom' \n" +
                            "end \n" +
                            "exec sp_addlogin 'eastcom','123456','" + Resource.TranDb + "' \n" +
                            "exec sp_grantdbaccess 'eastcom','dbtran' \n" +
                            "exec sp_addrolemember 'db_owner','dbtran' \n" +
                            "alter user dbtran with default_schema =dbo \n";

                    try
                    {
                        TestHelper.ExecuteNonQueryNonTran(CommandType.Text, sql);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("try to create login failed,try to create user failed,try to grant db_owner to dbtran failed,error message" + ex.ToString());
                        return 0;
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when check tran database,error message: " + ex.ToString());
                return 0;
            }
            }
            return 1;

        }
        //check login:eastcom
        public int CheckLogin()
        {
            //xzj--20190201--排除oracle判断
            if (Resource.TranInstance.Equals("mssqlserver", StringComparison.CurrentCultureIgnoreCase))
            {
            //test login eastcom 
            string conn = "Data Source=" + Resource.TranIp + "," + Resource.TranPort + "\\" + Resource.TranInstance + ";Initial Catalog=" + Resource.TranDb + ";uid=eastcom;pwd=123456";
            string conn2 = "Data Source=" + Resource.TranIp + "," + Resource.TranPort + "\\" + Resource.TranInstance + ";Initial Catalog=master;uid=" + Resource.TranUser + ";pwd=" + Resource.TranPwd;
            
            string sql = "use Dagdb_Tran \n" +
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
                        TestHelper2.ExecuteNonQueryNonTran(CommandType.Text, sql);
                        return 1;

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("try to create login failed,try to create user failed,try to grant db_owner to dbtran failed，error message" + ex.ToString());
                        return 0;

                    }


                }


            }
            catch (Exception ex)
            {
                Logger.Error("Error occur when test db connection,connectionString:" + conn + "\n error message" + ex.ToString());
                return 0;
            }
            }
            return 1;
        }

        private void DBDetectForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            this.Text = Resource.lHelper.Key("m13");
           
            Thread detect = new Thread(new ThreadStart(DBDetect));
            detect.Start();
        }



    }
}
