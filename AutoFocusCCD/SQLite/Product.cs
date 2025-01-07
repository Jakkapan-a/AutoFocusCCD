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
        public string Name { get; set; }
        public int Type { get; set; } // 0 = NONE, 1 = PVM
        public int Voltage_min { get; set; }
        public int Voltage_max { get; set; }
        public int Current_min { get; set; }
        public int Current_max { get; set; }
        public string ImagePath { get; set; }
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
            Columns.Add("ImagePath", "TEXT NULL");
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
            parameters.Add("ImagePath", ImagePath);
            parameters.Add("CreatedAt", CreatedAt);
            parameters.Add("UpdatedAt", UpdatedAt);
            return parameters;
        }
    }
}