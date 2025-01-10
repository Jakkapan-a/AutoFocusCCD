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
        public int ProductId { get; set; } // Product Id
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
            Columns.Add("YoloModelId", "INTEGER");
            Columns.Add("YoloModelName", "TEXT");
            Columns.Add("X", "INTEGER NOT NULL");
            Columns.Add("Y", "INTEGER NOT NULL");
            Columns.Add("Width", "INTEGER NOT NULL");
            Columns.Add("Hight", "INTEGER NOT NULL");
            Columns.Add("IndexT", "INTEGER"); // Index of the box in the image
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
            parameters.Add("UpdatedAt", SQliteDataAccess.GetDateTimeNow());
            return parameters;
        }

        public static Boxes Get(int id)
        {
            string sql = $"SELECT * FROM Boxes WHERE Id = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return SQliteDataAccess.Query<Boxes>(sql, parameters).FirstOrDefault();
        }

        public static List<Boxes> Get()
        {
            string sql = $"SELECT * FROM Boxes order by Id desc limit 1000";
            return SQliteDataAccess.Query<Boxes>(sql);
        }


        public static List<Boxes> GetByProductId(int productId)
        {
            string sql = $"SELECT * FROM Boxes WHERE ProductId = @ProductId order by Id desc limit 1000";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProductId", productId);
            return SQliteDataAccess.Query<Boxes>(sql, parameters);
        }

        public static int CountByProductId(int productId)
        {
            string sql = $"SELECT count(*) FROM Boxes WHERE ProductId = @ProductId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProductId", productId);
            return SQliteDataAccess.Query<int>(sql, parameters).FirstOrDefault();
        }

        public static void Delete(int id)
        {
            string sql = $"DELETE FROM Boxes WHERE Id = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            SQliteDataAccess.Execute(sql, parameters);
        }

        /***
         * Check if the box name is exist
         * @param name string | The name of the box
         */
        public static bool IsNameExit(string name, int productId)
        {
            string sql = $"SELECT count(*) FROM Boxes WHERE Name = @Name AND ProductId = @ProductId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Name", name);
            parameters.Add("ProductId", productId);
            return SQliteDataAccess.Query<int>(sql, parameters).FirstOrDefault() > 0;
        }

        public static bool IsNameExit(string name, int productId, int id)
        {
            string sql = $"SELECT count(*) FROM Boxes WHERE Name = @Name AND ProductId = @ProductId AND Id != @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Name", name);
            parameters.Add("ProductId", productId);
            parameters.Add("Id", id);
            return SQliteDataAccess.Query<int>(sql, parameters).FirstOrDefault() > 0;
        }
    }
}
