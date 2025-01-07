using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.SQLite
{
    public class Boxes : SQLiteBase, ISqliteEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // Label
        public int ProductId { get; set; }
        public int YoloModelId { get; set; } = 0;
        public string YoloModelName { get; set; }        
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Hight { get; set; }
        public int IndexT { get; set; } // Index of the box in the image
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public string TableName => "Boxes";

        public Boxes()
        {
            Columns = new DynamicParameters();
            Columns.Add("Id", "INTEGER NOT NULL");
            Columns.Add("Name", "TEXT NOT NULL");
            Columns.Add("ProductId", "INTEGER NOT NULL");
            Columns.Add("YoloModelId", "INTEGER NOT NULL");
            Columns.Add("YoloModelName", "TEXT NOT NULL");
            Columns.Add("X", "INTEGER NOT NULL");
            Columns.Add("Y", "INTEGER NOT NULL");
            Columns.Add("Width", "INTEGER NOT NULL");
            Columns.Add("Hight", "INTEGER NOT NULL");
            Columns.Add("IndexT", "INTEGER NOT NULL"); // Index of the box in the image
            Columns.Add("CreatedAt", "TEXT NOT NULL");
            Columns.Add("UpdatedAt", "TEXT NOT NULL");
            sqliteEntity = this;
        }

        public DynamicParameters Columns {get; private set; }
        public DynamicParameters CreateParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id);
            parameters.Add("Name", Name);
            parameters.Add("ProductId", ProductId);
            parameters.Add("YoloModelId", YoloModelId);
            parameters.Add("YoloModelName", YoloModelName);
            parameters.Add("X", X);
            parameters.Add("Y", Y);
            parameters.Add("Width", Width);
            parameters.Add("Hight", Hight);
            parameters.Add("IndexT", IndexT);
            parameters.Add("CreatedAt", CreatedAt);
            parameters.Add("UpdatedAt", UpdatedAt);
            return parameters;
        }
    }
}
