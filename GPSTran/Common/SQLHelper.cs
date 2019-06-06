using System;
using System.Data;
using System.Data.SqlClient;

namespace Common
{
    public class SqlHelper : SQLFactory
    {
        private readonly string strcon;
        //"User ID=scott;Password=tiger;" + "Data source=" +
        //"'(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.8.56.133)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=GPS)))'";
        //public INI ini=new INI()
        public SqlHelper(string str_con)
        {
            // strcon = str_con;
            // strcon = "User ID=scott;Password=tiger;Data source='(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.8.56.133)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=GPS)))'";
            strcon = str_con;
        }

        public override Boolean TestCon()
        {
            var conn = new SqlConnection(strcon);
            //SqlCommand command = new SqlCommand();
            //command.Connection = conn;
            //command.CommandType = CommandType.Text;
            //command.CommandText = "select count(0) from sysobjects where xtype='U'";
            try
            {
                conn.Open();
                //SqlDataReader reader=command.ExecuteReader();
                //reader.Dispose();
                //conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 0 testCon ,\n {0}", ex.ToString()));
               return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public override void ChangConn(params object[] obj)
        {
            //throw new NotImplementedException();
        }

        #region ExecuteNonQuery封装

        public override int ExecuteNonQuery(CommandType type, string sqlstr)
        {
            try
            {
                //System.Configuration.ConfigurationManager.AppSettings["m_connectionString"])
                using (var conn = new SqlConnection(strcon))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.CommandText = sqlstr;
                        cmd.CommandType = type;
                        cmd.Connection = conn;
                        cmd.Connection.Open();
                        cmd.Transaction = conn.BeginTransaction();
                        try
                        {
                            int result = cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                            return result;
                        }
                        catch (Exception ex)
                        {
                            cmd.Transaction.Rollback();
                            Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 0 Transaction ,\n {0}", ex));
                            Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 0 Transaction ,\n {0}", sqlstr));
                            //return 0;xzj--20190201--oracle-抛出错误，捕获后显示协议异常。返回0不会显示异常。
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 0 ExecuteNonQuery ,\n {0}", ex.ToString()));
                 //return 0;xzj--20190201--oracle-抛出错误，捕获后显示协议异常。返回0不会显示异常。
                throw ex;
            }
        }

        public override int ExecuteNonQueryNonTran(CommandType cmdtype, string cmdText)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strcon)) //System.Configuration.ConfigurationManager.AppSettings["m_connectionString"])
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.CommandType = cmdtype;
                        cmd.Connection = conn;

                        try
                        {
                            return cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            
                            Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: no Transaction ,\n {0}", ex));
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: no Transaction ,\n {0}", ex));
                return 0;
            }
        }

        #endregion

        #region ExecuteScalar封装
        //public override int ExecuteScalar(CommandType cmdtype, string cmdText, params OracleParameter[] parameters)
        public override int ExecuteScalar(CommandType type, string cmdText, params IDataParameter[] parameters)
        {
            try
            {
                using (var conn = new SqlConnection(strcon))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.CommandType = type;
                        cmd.Connection = conn;
                        cmd.Connection.Open();
                        if (parameters!=null)
                        {
                            foreach (var parameter in parameters)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        return (Int32)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 1 ExecuteScalar ,\n {0}", ex.ToString()));
                return 0;
            }
        }

        public override DataTable ExecuteDataReader(CommandType type, string sqlstr, params IDataParameter[] parameters)
        {
            DataTable dt = null;
            using (var conn = new SqlConnection(strcon))
            {
                using (var cmd=new SqlCommand(sqlstr,conn))
                {
                    if (parameters!=null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    cmd.Connection.Open();
                    using (var er = cmd.ExecuteReader())
                    {
                        if (!er.HasRows)
                        {
                            return dt;
                        }
                        dt = new DataTable();
                        var colcount=er.FieldCount;
                        for (var  i= 0;  i< colcount; i++)
                        {
                            dt.Columns.Add(er.GetName(i));
                        }
                        
                        while (er.Read())
                        {
                            var dr = dt.NewRow();
                            for (var j = 0; j < colcount; j++)
                            {
                                dr[j] = er[j];
                            }
                            dt.Rows.Add(dr);
                        }
                        return dt;
                    }
                }
            }
        }

        #endregion

        #region ExecuteRead封装
        public static DataTable ExecuteRead(CommandType cmdtype, string cmdText, int startRowIndex, int maximumRows, string tableName, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.Open();
                    var dr = new SqlDataAdapter(cmdText, conn);
                    var ds = new DataSet();
                    foreach (SqlParameter parameter in parameters)
                    {
                        dr.SelectCommand.Parameters.Add(parameter);
                    }
                    dr.Fill(ds, startRowIndex, maximumRows, tableName);
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 1 ExecuteRead ,\n {0}", ex.ToString()));
                return null;
            }

        }
        #endregion

        #region ExecuteRead封装
        public DataTable ExecuteRead(CommandType cmdtype, string cmdText, string tableName, params SqlParameter[] parameters)
        {
            try
            {
                using (var conn = new SqlConnection(strcon))
                {
                    conn.Open();
                    var dr = new SqlDataAdapter(cmdText, conn);
                    var ds = new DataSet();
                    if (parameters!=null)
                    {
                        foreach (var parameter in parameters)
                        {
                            dr.SelectCommand.Parameters.Add(parameter);
                        }
                    }
                    dr.Fill(ds, tableName);
                    return ds.Tables[0];
                }
            }
            catch (SqlException ex)
            {
                Logger.Error("连接字符串："+strcon);
                Logger.Error(cmdText);
                Logger.Error(string.Format("SQLHelper SQL CONNECTION ERROR: 2 ExecuteRead ,\n {0}", ex));
                return null;
            }
        }
        #endregion

    }
}
