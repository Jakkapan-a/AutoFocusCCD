using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.SQLite
{
    public class Product : SQLiteBase, ISqliteEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // Name of the product
        public int Type { get; set; } // 0 = NONE(6V), 1 = PVM(4.7V)
        public int Voltage_min { get; set; }
        public int Voltage_max { get; set; }
        public int Current_min { get; set; }
        public int Current_max { get; set; }
        public string ImageFile { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public string TableName => "ProductCcdPvm";
        public DynamicParameters Columns { get; private set; }

        public Product()
        {
            Columns = new DynamicParameters();
            Columns.Add("Id", "INTEGER NOT NULL");
            Columns.Add("Name", "TEXT NOT NULL");
            Columns.Add("Type", "INTEGER NOT NULL");
            Columns.Add("Voltage_min", "INTEGER NOT NULL");
            Columns.Add("Voltage_max", "INTEGER NOT NULL");
            Columns.Add("Current_min", "INTEGER NOT NULL");
            Columns.Add("Current_max", "INTEGER NOT NULL");
            Columns.Add("ImageFile", "TEXT NULL");
            Columns.Add("CreatedAt", "TEXT NOT NULL");
            Columns.Add("UpdatedAt", "TEXT NOT NULL");
            sqliteEntity = this;
        }
        
        public DynamicParameters CreateParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id);
            parameters.Add("Name", Name); 
            parameters.Add("Type", Type);
            parameters.Add("Voltage_min", Voltage_min);
            parameters.Add("Voltage_max", Voltage_max);
            parameters.Add("Current_min", Current_min);
            parameters.Add("Current_max", Current_max);
            parameters.Add("ImageFile", ImageFile);
            parameters.Add("CreatedAt", CreatedAt);
            parameters.Add("UpdatedAt", SQLite.SQliteDataAccess.GetDateTimeNow());
            return parameters;
        }

        public static List<Product> Get()
        {
            string sql = $"SELECT * FROM ProductCcdPvm order by Id desc limit 1000";
            return SQLite.SQliteDataAccess.Query<Product>(sql);
        }

        public static Product Get(int Id)
        {
            string sql = "SELECT * FROM ProductCcdPvm WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id);
            return SQLite.SQliteDataAccess.Query<Product>(sql, parameters).FirstOrDefault();
        }
        public static Product Get(string name)
        {
            string sql = "SELECT * FROM ProductCcdPvm WHERE Name = @Name";
            var parameters = new DynamicParameters();
            parameters.Add("Name", name);
            return SQLite.SQliteDataAccess.Query<Product>(sql, parameters).FirstOrDefault();
        }

        public static List<Product> GetList(string search, int pageSize, int currentPage)
        {
            string sql = "SELECT * FROM ProductCcdPvm WHERE Name LIKE @search order by Id desc limit @pageSize offset @offset";
            var parameters = new DynamicParameters();
            parameters.Add("search", $"%{search}%");
            parameters.Add("pageSize", pageSize);
            parameters.Add("offset", (currentPage - 1) * pageSize);
            return SQLite.SQliteDataAccess.Query<Product>(sql, parameters);
        }

        public static int Count(string search)
        {
            string sql = "SELECT count(*) FROM ProductCcdPvm WHERE Name LIKE @search";
            var parameters = new DynamicParameters();
            parameters.Add("search", $"%{search}%");
            return SQLite.SQliteDataAccess.Query<int>(sql, parameters).FirstOrDefault();
        }

        public static bool IsNameExist(string name, int Id = -1)
        { 
            string sql = "SELECT count(*) FROM ProductCcdPvm WHERE Name = @name";
            var parameters = new DynamicParameters();
            parameters.Add("name", name);
            if (Id > 0)
            {
                sql += " AND Id != @Id";
                parameters.Add("Id", Id);
            }
            return SQLite.SQliteDataAccess.Query<int>(sql, parameters).FirstOrDefault() > 0;
        }
    }
}