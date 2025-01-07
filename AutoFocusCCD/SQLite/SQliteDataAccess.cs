using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace AutoFocusCCD.SQLite
{
    public static class SQliteDataAccess
    {
        /// <summary>
        /// Get the connection string from the app.config file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string LoadConnectionString()
        {
            // Create file if not exist
            if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\" + "Database/ApplicationDB.db"))
            {
                if(!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\" + "Database"))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\" + "Database");
                }
                
                SQLiteConnection.CreateFile(System.IO.Directory.GetCurrentDirectory() + "\\" + "Database/ApplicationDB.db");
            }
            return "Data Source=" + System.IO.Directory.GetCurrentDirectory() + "\\" + "Database/ApplicationDB.db;Version=3;";            
        }

        /// <summary>
        /// Execute a query and return a list of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> Query<T>(string sql, DynamicParameters parameters = null) => ExecuteQuery<T>(sql, parameters);

        public static void Execute(string sql, DynamicParameters parameters)
        {
            using (IDbConnection con = new SQLiteConnection(LoadConnectionString()))
            {
                con.Execute(sql, parameters);
            }
        }

        /// <summary>
        /// Execute a query and return a list of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static List<T> ExecuteQuery<T>(string sql, DynamicParameters parameters = null)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return cnn.Query<T>(sql, parameters == null ? new DynamicParameters() : parameters).ToList();
            }
        }

        /// <summary>
        /// Get the current date and time
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static bool IsExist(string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query(sql, new DynamicParameters());
                return output.ToList().Count > 0;
            }
        }

        /// <summary>
        /// Execute a query and return a list of objects asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters = null)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryAsync<T>(sql, parameters);
            }
        }

        /// <summary>
        /// Execute a query and return a list of objects asynchronously
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task ExecuteAsync(string sql, DynamicParameters parameters)
        {
            using (IDbConnection con = new SQLiteConnection(LoadConnectionString()))
            {
                await con.ExecuteAsync(sql, parameters);
            }
        }
    }
}
