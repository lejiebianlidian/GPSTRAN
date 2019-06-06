using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

///加入事物方式处理数据
namespace Common
{
    public class OracleHelper : SQLFactory
    {
        private readonly String consql = @"data source={0}:{1}/{2};user id={3};password={4};Pooling=true;Min pool size=5;Max pool size=10;";

        private String con = string.Empty;

        public OracleHelper(params object[] obj)
        {
            ChangConn(obj);
        }

        public override Boolean TestCon()
        {
            var conn = new OracleConnection(con);
            try
            {
                conn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public override void ChangConn(params object[] obj)
        {
            con = string.Format(consql, obj[0], obj[1], obj[2], obj[3], obj[4]);
        }

        public override int ExecuteNonQuery(CommandType type, string sqlstr)
        {
            //throw new NotImplementedException();
            using (var orlcon = new OracleConnection(con))
            {
                using (var cmd = new OracleCommand(sqlstr, orlcon))
                {
                    try
                    {
                        cmd.Connection.Open();
                        cmd.CommandType = type;
                        cmd.CommandText = sqlstr;
                        var i = cmd.ExecuteNonQuery();
                        return i;
                    }
                    catch
                    {
                        Logger.Error(string.Format("oracle 无法写入数据:{0}", sqlstr)); ;
                        return 0;
                    }
                }
            }
        }

        public override int ExecuteNonQueryNonTran(CommandType cmdtype, string sqlstr)
        {
            throw new NotImplementedException();
        }

        public override int ExecuteScalar(CommandType type, string sqlstr, params IDataParameter[] parameters)
        {
            using (var orlcon = new OracleConnection(con))
            {
                using (var cmd = new OracleCommand(sqlstr, orlcon))
                {
                    //cmd.Parameters.Add("gpsid",OracleDbType.Varchar2).Value="24006";
                    //cmd.Parameters.Add("x",OracleDbType.Double).Value=120.0;
                    //cmd.Parameters.Add("y",OracleDbType.Double).Value=10.0;
                    //cmd.Parameters.Add("speed",OracleDbType.Double).Value=50.0;
                    //cmd.Parameters.Add("dir",OracleDbType.Varchar2).Value="300";
                    //cmd.Parameters.Add("state",OracleDbType.Varchar2).Value="200";
                    //cmd.Parameters.Add("gpstime",OracleDbType.Date).Value=DateTime.Now;
                    cmd.CommandType = type;
                    //cmd.CommandText = sqlstr;
                    cmd.Connection.Open();
                    return (Int32)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 新增实现 xzj--20190130--oracle
        /// </summary>>
        /// <returns></returns>
        public override DataTable ExecuteDataReader(CommandType type, string sqlstr, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}