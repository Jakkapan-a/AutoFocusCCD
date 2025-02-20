using AutoFocusCCD.Utilities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.SQLite
{
    public class History : SQLiteBase, ISqliteEntity
    {
        public int Id { get; set; }
        public string employee { get; set; }
        public string qr_code { get; set; }
        public string path_folder { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; } = "";
        public int voltage_min { get; set; }
        public int voltage_max { get; set; }
        public int voltage { get; set; }
        public int current_min { get; set; }
        public int current_max { get; set; }
        public int current { get; set; }
        public string result { get; set; }
        public string re_judgment { get; set; } = "None";
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string TableName => "History";

        public DynamicParameters Columns { get; private set; }

        public DynamicParameters CreateParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id);
            parameters.Add("employee", employee);
            parameters.Add("qr_code", qr_code);
            parameters.Add("path_folder", path_folder);
            parameters.Add("product_id", product_id);
            parameters.Add("product_name", product_name);
            parameters.Add("voltage_min", voltage_min);
            parameters.Add("voltage_max", voltage_max);
            parameters.Add("voltage", voltage);
            parameters.Add("current_min", current_min);
            parameters.Add("current_max", current_max);
            parameters.Add("current", current);
            parameters.Add("result", result);
            parameters.Add("re_judgment", re_judgment);
            parameters.Add("CreatedAt", CreatedAt);
            parameters.Add("UpdatedAt", SQliteDataAccess.GetDateTimeNow());
            return parameters;
        }

        public History()
        {
            Columns = new DynamicParameters();
            Columns.Add("Id", "INTEGER NOT NULL");
            Columns.Add("employee", "TEXT NOT NULL");
            Columns.Add("qr_code", "TEXT NOT NULL");
            Columns.Add("path_folder", "TEXT NOT NULL");
            Columns.Add("product_id", "INTEGER NOT NULL");
            Columns.Add("product_name", "TEXT");
            Columns.Add("voltage_min", "INTEGER NOT NULL");
            Columns.Add("voltage_max", "INTEGER NOT NULL");
            Columns.Add("voltage", "INTEGER NOT NULL");
            Columns.Add("current_min", "INTEGER NOT NULL");
            Columns.Add("current_max", "INTEGER NOT NULL");
            Columns.Add("current", "INTEGER NOT NULL");
            Columns.Add("result", "TEXT NOT NULL");
            Columns.Add("re_judgment", "TEXT NOT NULL");
            Columns.Add("CreatedAt", "TEXT NOT NULL");
            Columns.Add("UpdatedAt", "TEXT NOT NULL");
            sqliteEntity = this;
        }

        public static History Get(int id)
        {
            string sql = $"SELECT * FROM History WHERE Id = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return SQliteDataAccess.Query<History>(sql, parameters).FirstOrDefault();
        }

        public static List<History> GetList()
        {
            string sql = $"SELECT * FROM History";
            return SQliteDataAccess.Query<History>(sql);
        }

        public static List<History> GetList(string search, string qr, string date , string result, int page = 1, int limit = 10)
        {
            string sql = $"SELECT * FROM History WHERE employee LIKE @search AND qr_code LIKE @qr AND result LIKE @result AND CreatedAt LIKE @date ORDER BY Id DESC LIMIT @limit OFFSET @offset";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("search", $"%{search}%");
            parameters.Add("qr", $"%{qr}%");
            parameters.Add("result", $"%{result}%");
            parameters.Add("date", $"%{date}%");

            parameters.Add("limit", limit);
            parameters.Add("offset", (page - 1) * limit);

            return SQliteDataAccess.Query<History>(sql, parameters);
        }

        public static int Count(string search, string qr, string date, string result)
        {
            string sql = $"SELECT COUNT(*) FROM History WHERE employee LIKE @search AND qr_code LIKE @qr AND result LIKE @result AND CreatedAt LIKE @date";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("search", $"%{search}%");
            parameters.Add("qr", $"%{qr}%");
            parameters.Add("result", $"%{result}%");
            parameters.Add("date", $"%{date}%");
            return SQliteDataAccess.Query<int>(sql, parameters).FirstOrDefault();
        }

        public static History GetLast()
        {
            string sql = $"SELECT * FROM History ORDER BY Id DESC LIMIT 1";
            return SQliteDataAccess.Query<History>(sql).FirstOrDefault();
        }
    }
}
