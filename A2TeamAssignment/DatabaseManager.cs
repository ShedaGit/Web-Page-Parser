﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace A2TeamAssignment
{
    public class DatabaseManager : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
                _connection.Dispose();
            }
        }

        public SqlConnection GetConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        public void CloseConnection()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
            else
            {
                Console.WriteLine("The connection is already closed.");
            }
        }
    }
}