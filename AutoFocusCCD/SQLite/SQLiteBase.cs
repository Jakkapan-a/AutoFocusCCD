using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System;





namespace AutoFocusCCD.SQLite
{
    public interface ISqliteEntity
    {
        int Id { get; set; }
        string TableName { get; }
        DynamicParameters Columns { get; }
        DynamicParameters CreateParameters();
    }

    public class SQLiteBase : IDisposable
    {
        protected ISqliteEntity sqliteEntity;
        private bool _disposed = false;

        public SQLiteBase(ISqliteEntity sqlite = null)
        {
            this.sqliteEntity = sqlite;
        }

        /// <summary>--
        /// Create a table in the database
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CreateTable()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }
            string sql = $"CREATE TABLE IF NOT EXISTS {sqliteEntity.TableName} (";
            foreach (var column in sqliteEntity.Columns.ParameterNames)
            {
                sql += $"{column} {sqliteEntity.Columns.Get<string>(column)},";
            }

            if (sqliteEntity.Columns.ParameterNames.Contains("Id"))
            {
                sql += "PRIMARY KEY(`Id` AUTOINCREMENT));";
            }
            else if (sqliteEntity.Columns.ParameterNames.Contains("id"))
            {
                sql += "PRIMARY KEY(`id` AUTOINCREMENT));";
            }
            else
            {
                sql = sql.TrimEnd(',') + ");";
            }
            SQLite.SQliteDataAccess.Execute(sql, null);
        }

        public void SyncTable()
        {
            // 1. check if the table exists
            string sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{sqliteEntity.TableName}';";
            var result = SQLite.SQliteDataAccess.Query<string>(sql, null);
            if (result.Count == 0)
            {
                CreateTable();

                return;
            }
            // 2. if the table exists, check if the columns are the same
            sql = $"PRAGMA table_info({sqliteEntity.TableName});";
            var dbColumns = SQLite.SQliteDataAccess.Query<dynamic>(sql, null).Select(x => (string)x.name).ToList();

            // 3. if the columns are not the same, update the table
            var entityColumns = sqliteEntity.Columns.ParameterNames.ToList();

            // 4. Add new columns if not exist
            foreach(var column in entityColumns)
            {
                if (!dbColumns.Contains(column))
                {
                    sql = $"ALTER TABLE {sqliteEntity.TableName} ADD COLUMN {column} {sqliteEntity.Columns.Get<string>(column)};";
                    SQLite.SQliteDataAccess.Execute(sql, null);
                }
            }

            // 5. Remove columns if not exist
            foreach(var column in dbColumns)
            {
                if (!entityColumns.Contains(column))
                {
                    sql = $"ALTER TABLE {sqliteEntity.TableName} DROP COLUMN {column};";
                    SQLite.SQliteDataAccess.Execute(sql, null);
                }
            }

        }

        public void DropTable()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }
            string sql = $"DROP TABLE IF EXISTS {sqliteEntity.TableName};";
            SQLite.SQliteDataAccess.Execute(sql, null);
        }

        /// <summary>
        /// Save the object to the database
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Save()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }

            string sql = $"INSERT INTO {sqliteEntity.TableName} (";
            DynamicParameters parameters = sqliteEntity.CreateParameters();
            parameters.Add("CreatedAt", SQLite.SQliteDataAccess.GetDateTimeNow());
            foreach (var column in parameters.ParameterNames)
            {
                if (column == "Id")
                {
                    continue;
                }
                sql += $"{column},";
            }
            sql = sql.TrimEnd(',') + ") VALUES (";
            foreach (var column in parameters.ParameterNames)
            {
                if (column == "Id")
                {
                    continue;
                }
                sql += $"@{column},";
            }
            sql = sql.TrimEnd(',') + ");";
            SQLite.SQliteDataAccess.Execute(sql, parameters);
        }

        /// <summary>
        /// Save the object to the database asynchronously
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SaveSaync()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }

            string sql = $"INSERT INTO {sqliteEntity.TableName} (";
            DynamicParameters parameters = sqliteEntity.CreateParameters();
            parameters.Add("CreatedAt", SQLite.SQliteDataAccess.GetDateTimeNow());
            foreach (var column in parameters.ParameterNames)
            {
                if (column == "Id")
                {
                    continue;
                }
                sql += $"{column},";
            }
            sql = sql.TrimEnd(',') + ") VALUES (";
            foreach (var column in parameters.ParameterNames)
            {
                if (column == "Id")
                {
                    continue;
                }
                sql += $"@{column},";
            }
            sql = sql.TrimEnd(',') + ");";
            await SQLite.SQliteDataAccess.ExecuteAsync(sql, parameters);
        }


        /// <summary>
        /// Update the object in the database
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Update()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }

            string sql = $"UPDATE {sqliteEntity.TableName} SET ";
            DynamicParameters parameters = sqliteEntity.CreateParameters();
            foreach (var column in parameters.ParameterNames)
            {
                if (column == "Id")
                {
                    continue;
                }
                sql += $"{column} = @{column},";
            }
            sql = sql.TrimEnd(',') + $" WHERE Id = @Id;";
            SQLite.SQliteDataAccess.Execute(sql, parameters);
        }

        /// <summary>
        /// Update the object in the database asynchronously
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateAsync()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }

            string sql = $"UPDATE {sqliteEntity.TableName} SET ";
            DynamicParameters parameters = sqliteEntity.CreateParameters();
            foreach (var column in parameters.ParameterNames)
            {
                if (column == "Id")
                {
                    continue;
                }
                sql += $"{column} = @{column},";
            }
            sql = sql.TrimEnd(',') + $" WHERE Id = @Id;";
            await SQLite.SQliteDataAccess.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Delete the object from the database
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Delete()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }
            string sql = $"DELETE FROM {sqliteEntity.TableName} WHERE Id = @Id;";
            SQLite.SQliteDataAccess.Execute(sql, sqliteEntity.CreateParameters());
        }

        /// <summary>
        /// Delete the object from the database asynchronously
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteAsync()
        {
            if (sqliteEntity == null)
            {
                throw new Exception("sqliteEntity is null");
            }
            string sql = $"DELETE FROM {sqliteEntity.TableName} WHERE Id = @Id;";
            await SQLite.SQliteDataAccess.ExecuteAsync(sql, sqliteEntity.CreateParameters());
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release any managed resources here if necessary
                    // For example, closing database connections
                }

                // Release any unmanaged resources here if necessary
                _disposed = true;
            }
        }
    }
}
