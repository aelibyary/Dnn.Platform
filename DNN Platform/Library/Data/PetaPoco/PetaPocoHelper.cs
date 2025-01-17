﻿#region Copyright

// 
// DotNetNuke® - https://www.dnnsoftware.com
// Copyright (c) 2002-2018
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;
using PetaPoco;

namespace DotNetNuke.Data.PetaPoco
{
    public static class PetaPocoHelper
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(PetaPocoHelper));

        private const string SqlProviderName = "System.Data.SqlClient";

        #region Public Methods

        public static void ExecuteNonQuery(string connectionString, CommandType type, string sql, params object[] args)
        {
            ExecuteNonQuery(connectionString, type, Null.NullInteger, sql, args);
        }

        public static void ExecuteNonQuery(string connectionString, CommandType type, int timeoutSec, string sql, params object[] args)
        {
            using (var database = new Database(connectionString, SqlProviderName) { EnableAutoSelect = false })
            {
                if (type == CommandType.StoredProcedure)
                {
                    sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
                }

                if (timeoutSec > 0)
                {
                    database.CommandTimeout = timeoutSec;
                }

                try
                {
                    database.Execute(sql, args);
                }
                catch (Exception ex)
                {
                    Logger.Error("[1] Error executing SQL: " + sql + Environment.NewLine + ex.Message);
                    throw;
                }
            }
        }

        public static void BulkInsert(string connectionString, string procedureName, string tableParameterName, DataTable dataTable)
        {
            BulkInsert(connectionString, Null.NullInteger, procedureName, tableParameterName, dataTable);
        }

        public static void BulkInsert(string connectionString, int timeoutSec, string procedureName, string tableParameterName, DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(procedureName, con))
                {
                    if (!tableParameterName.StartsWith("@"))
                        tableParameterName = "@" + tableParameterName;

                    if (timeoutSec > 0)
                        cmd.CommandTimeout = timeoutSec;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(tableParameterName, dataTable);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("[2] Error executing SQL: " + cmd.CommandText + Environment.NewLine + ex.Message);
                        throw;
                    }
                    con.Close();
                }
            }
        }
        public static void BulkInsert(string connectionString, string procedureName, string tableParameterName, DataTable dataTable, Dictionary<string, object> args)
        {
            BulkInsert(connectionString, procedureName, tableParameterName, dataTable, Null.NullInteger, args);
        }

        public static void BulkInsert(string connectionString, string procedureName, string tableParameterName, DataTable dataTable, int timeoutSec, Dictionary<string, object> args)
        {
            if (dataTable.Rows.Count > 0)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(procedureName, con))
                {
                    if (!tableParameterName.StartsWith("@"))
                        tableParameterName = "@" + tableParameterName;

                    if (timeoutSec > 0)
                        cmd.CommandTimeout = timeoutSec;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(tableParameterName, dataTable);
                    foreach (var arg in args)
                    {
                        cmd.Parameters.AddWithValue(arg.Key, arg.Value);
                    }
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        Logger.Error("[2] Error executing SQL: " + cmd.CommandText);
                        throw;
                    }
                    con.Close();
                }
            }
        }

        public static IDataReader ExecuteReader(string connectionString, CommandType type, string sql, params object[] args)
        {
            return ExecuteReader(connectionString, type, Null.NullInteger, sql, args);
        }

        public static IDataReader ExecuteReader(string connectionString, CommandType type, int timeoutSec, string sql, params object[] args)
        {
            var database = new Database(connectionString, SqlProviderName) { EnableAutoSelect = false };

            if (type == CommandType.StoredProcedure)
            {
                sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
            }

            if (timeoutSec > 0)
            {
                database.CommandTimeout = timeoutSec;
            }

            try
            {
                return database.ExecuteReader(sql, args);
            }
            catch (Exception ex)
            {
                // very special case for installation
                if (!sql.EndsWith("GetDatabaseVersion"))
                {
                    Logger.Error("[3] Error executing SQL: " + sql + Environment.NewLine + ex.Message);
                }
                throw;
            }
        }

        public static T ExecuteScalar<T>(string connectionString, CommandType type, string sql, params object[] args)
        {
            return ExecuteScalar<T>(connectionString, type, Null.NullInteger, sql, args);
        }

        public static T ExecuteScalar<T>(string connectionString, CommandType type, int timeoutSec, string sql, params object[] args)
        {
            using (var database = new Database(connectionString, SqlProviderName) { EnableAutoSelect = false })
            {
                if (type == CommandType.StoredProcedure)
                {
                    sql = DataUtil.GenerateExecuteStoredProcedureSql(sql, args);
                }

                if (timeoutSec > 0)
                {
                    database.CommandTimeout = timeoutSec;
                }

                try
                {
                    return database.ExecuteScalar<T>(sql, args);
                }
                catch (Exception ex)
                {
                    Logger.Error("[4] Error executing SQL: " + sql + Environment.NewLine + ex.Message);
                    throw;
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public static void ExecuteSQL(string connectionString, string sql)
        {
            ExecuteSQL(connectionString, sql, Null.NullInteger);
        }

        // ReSharper disable once InconsistentNaming
        public static void ExecuteSQL(string connectionString, string sql, int timeoutSec)
        {
            using (var database = new Database(connectionString, SqlProviderName) { EnableAutoSelect = false })
            {
                if (timeoutSec > 0)
                {
                    database.CommandTimeout = timeoutSec;
                }

                try
                {
                    database.Execute(sql);
                }
                catch (Exception ex)
                {
                    Logger.Error("[5] Error executing SQL: " + sql + Environment.NewLine + ex.Message);
                    throw;
                }
            }
        }

        #endregion
    }
}
