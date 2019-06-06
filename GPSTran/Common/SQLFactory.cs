using System;
using System.Data;

namespace Common
{
    public abstract class SQLFactory
    {
        public abstract Boolean TestCon();

        public abstract void ChangConn(params Object[] obj);

        public abstract int ExecuteNonQuery(CommandType type,String sqlstr);

        public abstract int ExecuteNonQueryNonTran(CommandType cmdtype, string sqlstr);

        public abstract int ExecuteScalar(CommandType type,String sqlstr, params IDataParameter[] parameters);

        /// <summary>
        /// 新增方式 2015-10-20
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sqlstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract DataTable ExecuteDataReader(CommandType type, string sqlstr, params IDataParameter[] parameters);
    }
}
