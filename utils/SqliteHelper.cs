﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA_Window
{
    using System;
    using System.Data;
    using System.Data.SQLite;

    public class SqliteHelper
    {
        private readonly string connectionString;

        public SqliteHelper()
        {
            connectionString = $"Data Source=lyr_rpa.db;Version=3;";
            string createTableSql = "CREATE TABLE IF NOT EXISTS MyTable (Id INTEGER PRIMARY KEY, Name TEXT, Age INTEGER);";
            ExecuteNonQuery(createTableSql);
        }

        public void ExecuteNonQuery(string sql)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public DataTable ExecuteQuery(string sql)
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
                        return dataTable;
                    }
                }
            }
        }
    }

}