
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace RPA_Window
{
    

    public class SqliteHelper
    {
        private readonly string connectionString;

        public SqliteHelper()
        {
            String dbPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\RPA";
            Directory.CreateDirectory(dbPath);
            connectionString = $"Data Source={dbPath}\\lyr_rpa.db;Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // 在初始化时创建数据库表
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "CREATE TABLE IF NOT EXISTS folders (folder_path TEXT);";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS remove_projects (folder_path TEXT);";
                    command.ExecuteNonQuery();
                }
            }
        }

        public int InsertData(string sql)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected; // 返回受影响的行数
                }
            }
        }

        public int UpdateData(string sql)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected; // 返回受影响的行数
                }
            }
        }

        public int DeleteData(string sql)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected; // 返回受影响的行数
                }
            }
        }

        public DataTable QueryData(string sql)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable; // 返回查询结果
                    }
                }
            }
        }
    }
}
