using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DapperExtensions;
using Dapper;



namespace DBUtility
{
    public  class DataFactory : BaseClass
    {
        private static IDbConnection con;

         //private IDbConnection _conn;

        private static string _conStr;

        public static IDbConnection Conn
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_conStr))
                {
                    return con = new SqlConnection(_conStr);
                }
                return con = new SqlConnection(GetConnectionStr()); 
            }
        }

        public DataFactory(){}

        public DataFactory(string connectionstr) {
            _conStr = connectionstr;
        }

        //private DbTransaction transaction;
        public static int? _commandTimeout = 15;//链接时长,

        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="id">id参数值</param>
        /// <returns></returns>
        public static T Get<T>(object id)where T :class ,new ()
        {
            using (Conn)
            {
                T t = Conn.Get<T>(id, null, _commandTimeout);
           
                return t;
            }
        }

        /// <summary>
        /// 返回一个IEnumerable集合
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public static IEnumerable<T> GetList<T>(object predicate = null) where T : class ,new()
        {
            using (Conn)
            {
    
                IEnumerable<T> t = Conn.GetList<T>(predicate, null, null, _commandTimeout, false);
                return t;
            }
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(string sql, object par) where T : class, new()
        {
            using (Conn)
            {
                IEnumerable<T> t = Conn.Query<T>(sql, par, null, true, _commandTimeout, null);
                return t;
            }
        }

        public static int GetCount(string tableName, object param=null,string where="")
        {
            using (Conn)
            {
                string sql = "SELECT COUNT(1) FROM " + tableName;
                if (!string.IsNullOrWhiteSpace(where))
                {
                    sql += " WHERE " + where;
                }
                int count = Conn.ExecuteScalar<int>(sql, param, null, _commandTimeout, null);
                return count;
            }
        }

        public static int GetCount(string sql)
        {
            using (Conn)
            {
              return  Conn.ExecuteScalar<int>(sql, null, null, _commandTimeout, null);
            }
        }


        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public static object Insert<T>(T entity) where T : class, new()
        {
            using (Conn)
            {
                return Conn.Insert(entity, null, null);
            }
        }

        /*批量添加暂不实现*/
        //public static void Insert<T>(IEnumerable<T> entities) where T : class, new()
        //{
        //    using (IDbConnection con = new SqlConnection(GetConnectionStr()))
        //    {
        //        con.Insert(entities, null, null);
        //    }
        //}

        //public static object Insert<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = new int?()) where T : class;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Delete<T>(T entity) where T : class, new()
        {
            using (Conn)
            {
                return Conn.Delete(entity, null, null);
            }
        }



        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public static bool DeleteByWhere<T>(object predicate) where T : class, new()
        //{
        //    bool returnValue = false;
        //    using (IDbConnection con = new SqlConnection(GetConnectionStr()))
        //    {
        //        con.Open();
        //        returnValue= con.Delete(predicate, null, null);
        //        con.Close();
        //    }
        //    return returnValue;
        //}

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Update<T>(T entity) where T : class, new()
        {
            using (Conn)
            {
                return Conn.Update(entity, null, null);
            }
        }

        public static void ExecuteNonQueryProc(string storedProcedure, object par = null)
        {
            using (Conn)
            {
                Conn.Execute(storedProcedure, par, null, null, CommandType.StoredProcedure);
                //con.Execute(storedProcedure, par, null, null, CommandType.StoredProcedure);
              
            }
        }

        /// <summary>
        /// 执行存储过程返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedure"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteNonQueryProc<T>(string storedProcedure, object par = null) where T : class, new()
        {
            using (Conn)
            {
                return Conn.Query<T>(storedProcedure, par, null, true, _commandTimeout, CommandType.StoredProcedure);
            }
        }


        public static bool Execute(string sql)
        {
            using (Conn)
            {
                return Conn.Execute(sql, "", null, null, CommandType.Text) > 0;
            }
        }

        /// <summary>
        /// 组装插入语句
        /// </summary>
        /// <param name="tbName">表名</param>
        /// <param name="colums"></param>
        /// <returns></returns>
        private static string CreateInertSql(string tbName, IList<TableColumnModel> colums, string ParamPrefix)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(string.Format("INSERT INTO {0}(", tbName));
            for (int i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0 ? colums[i].ColumnName : string.Format(",{0}", colums[i].ColumnName));
            }
            sql.Append(")");
            sql.Append(" VALUES(");
            for (int i = 0; i < colums.Count; i++)
            {
                sql.Append(i == 0
                    ? string.Format("{0}{1}", ParamPrefix, colums[i].FieldName)
                    : string.Format(",{0}{1}", ParamPrefix, colums[i].FieldName));
            }
            sql.Append(") ");
            return sql.ToString();
        }



    }
}
