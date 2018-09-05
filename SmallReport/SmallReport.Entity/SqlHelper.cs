using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace SmallReport.Entity
{
    public static class SqlHelper
    {
        private static void AddParamToSQLCmd(SqlCommand command, string paramName, SqlDbType sqlType, int paramSize, ParameterDirection paramDirection, object paramValue)
        {
            if (command == null)
                throw new ArgumentNullException("SqlCommand");
            if (string.IsNullOrWhiteSpace(paramName))
                throw new ArgumentOutOfRangeException("paramName");
            var sqlParameter = new SqlParameter
            {
                ParameterName = paramName,
                SqlDbType = sqlType,
                Direction = paramDirection
            };
            if (paramSize > 0)
                sqlParameter.Size = paramSize;
            sqlParameter.Value = paramValue == null ? DBNull.Value : RuntimeHelpers.GetObjectValue(paramValue);
            command.Parameters.Add(sqlParameter);
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, List<SqlParameter> commandParams)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
                command.Transaction = transaction;
            command.CommandType = commandType;
            if (commandParams == null)
                return;
            var enumerator = new List<SqlParameter>.Enumerator();
            try
            {
                enumerator = commandParams.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    command.Parameters.Add(current);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static SqlParameter CreateParam(string paramName, SqlDbType sqlType, int paramSize, ParameterDirection paramDirection, object paramValue)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                throw new ArgumentOutOfRangeException("paramName");
            var sqlParameter = new SqlParameter
            {
                ParameterName = paramName,
                SqlDbType = sqlType,
                Direction = paramDirection
            };
            if (paramSize > 0)
                sqlParameter.Size = paramSize;
            sqlParameter.Value = paramValue == null ? DBNull.Value : RuntimeHelpers.GetObjectValue(paramValue);
            return sqlParameter;
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, List<SqlParameter> commandParms, ref int returnValue)
        {
            var command = new SqlCommand();
            int num;
            using (var connection = new SqlConnection(connectionString))
            {
                PrepareCommand(command, connection, null, commandType, commandText, commandParms);
                command.Parameters.Add(SqlHelper.CreateParam("@ReturnValue", SqlDbType.Int, 0, ParameterDirection.ReturnValue, (object)null));
                num = command.ExecuteNonQuery();
                if (int.TryParse(command.Parameters["@ReturnValue"].Value.ToString(), out int value))
                    returnValue = (int)command.Parameters["@ReturnValue"].Value;
                command.Parameters.Clear();
            }
            return num;
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, List<SqlParameter> commandParms)
        {
            var command = new SqlCommand();
            int num;
            using (var connection = new SqlConnection(connectionString))
            {
                PrepareCommand(command, connection, null, commandType, commandText, commandParms);
                num = command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            return num;
        }


        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, List<SqlParameter> commandParms)
        {
            var command = new SqlCommand();
            var connection = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(command, connection, null, commandType, commandText, commandParms);
                var sqlDataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                command.Parameters.Clear();
                return sqlDataReader;
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, List<SqlParameter> commandParms)
        {
            var command = new SqlCommand();
            object objectValue;
            using (var connection = new SqlConnection(connectionString))
            {
                PrepareCommand(command, connection, null, commandType, commandText, commandParms);
                objectValue = RuntimeHelpers.GetObjectValue(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            return objectValue;
        }

        public static DataTable ExecuteTable(string connectionString, CommandType commandType, string commandText, List<SqlParameter> commandParms)
        {
            var dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var selectCommand = new SqlCommand(commandText, connection)
                {
                    CommandType = commandType
                };
                var enumerator = new List<SqlParameter>.Enumerator();
                if (commandParms != null)
                {
                    try
                    {
                        enumerator = commandParms.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            selectCommand.Parameters.Add(current);
                        }
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }
                }
                new SqlDataAdapter(selectCommand).Fill(dataTable);
                selectCommand.Parameters.Clear();
            }
            return dataTable;
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, List<SqlParameter> commandParms)
        {
            var dataSet = new DataSet();
            using (var connection = new SqlConnection(connectionString))
            {
                var selectCommand = new SqlCommand(commandText, connection)
                {
                    CommandType = commandType
                };
                var enumerator = new List<SqlParameter>.Enumerator();
                if (commandParms != null)
                {
                    try
                    {
                        enumerator = commandParms.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            selectCommand.Parameters.Add(current);
                        }
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }
                }
                new SqlDataAdapter(selectCommand).Fill(dataSet);
                selectCommand.Parameters.Clear();
            }
            return dataSet;
        }
    }
}
