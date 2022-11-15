using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Extensions;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Demo.DataUniverseFramework.SQL
{
    public class SqlAccess : ISqlAccess
    {
        public SqlConfig Config { get; private set; }
        private SqlConnection Connection = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccess" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public SqlAccess(SqlConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        private void CloseConnection()
        {
            if(Connection != null)
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataAdapter">The data adapter.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException"></exception>
        public DataTable ExecuteQuery(IDbDataAdapter dataAdapter)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                DataTable resultSet = dataSet.Tables["Table"];
                return resultSet;
            }
            catch (Exception e)
            {
                string message = string.Format("Exception in Demo.DataUniverseFramework.SQL.SQLAccess.ExecuteQuery. "
                                        + "Found this error {1} in the following query {0}", dataAdapter.SelectCommand.CommandText, e.ToString());
                throw new FrameworkException(message, e);
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Gets the data adapter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public IDbDataAdapter GetDataAdapter(IDbCommand command)
        {
            return new SqlDataAdapter(command.CommandText, command.Connection.ConnectionString);
        }

        /// <summary>
        /// Executes the query string result.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException"></exception>
        public string ExecuteQueryStringResult(IDbCommand command)
        {
            string result = string.Empty;

            using (IDataReader dataReader = command.ExecuteReader())
            {
                try
                {
                    while (dataReader.Read())
                    {
                        result += dataReader.GetValue(0).ToString();
                    }

                    return result;
                }
                catch (Exception e)
                {
                    string message = string.Format("Exception in Demo.DataUniverseFramework.SQL.SQLAccess.ExecuteQueryStringResult. "
                                        + "Found this error {1} in the following query {0}", command.CommandText, e.ToString());
                    throw new FrameworkException(message, e);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Executes the statement.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException"></exception>
        public int ExecuteStatement(IDbCommand command)
        {
            using (IDataReader dataReader = command.ExecuteReader())
            {
                try
                {
                    return command.CommandText.IsQuery() ? Convert.ToInt32(dataReader.Read()) : dataReader.RecordsAffected;
                }
                catch (Exception e)
                {
                    string message = string.Format("Exception in Demo.DataUniverseFramework.SQL.SQLAccess.ExecuteStatement. "
                                                + "Found this error {1} in the following statement {0}", command.CommandText, e.ToString());
                    throw new FrameworkException(message, e);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Puts a file into the Database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException"></exception>
        public int DatabaseFilePut(IDbCommand command, byte[] file)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@FILE", SqlDbType.VarBinary, file.Length)
                {
                    Value = file
                };
                command.Parameters.Add(sqlParameter);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string message = string.Format("Exception in Demo.DataUniverseFramework.SQL.SQLAccess.DatabaseFilePut. "
                                            + "Found this error {1} in the following statement {0}", command.CommandText, e.ToString());
                throw new FrameworkException(message, e);
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">SqlConfig cannot be null
        /// or
        /// ConnectionString from remote application config file not found in the server: " + Config.DataBaseServer
        /// or</exception>
        public IDbCommand CreateConnection(string statement)
        {
            if (Config == null || string.IsNullOrEmpty(Config.ConnectionString))
            {
                throw new FrameworkException("ConnectionString from remote application config file not found in the server: " + Config?.DatabaseServer);
            }

            try
            {
                Connection = new SqlConnection(Config.ConnectionString);
                IDbCommand selectCommand = new SqlCommand(statement, Connection);
                Connection.Open();
                if (!string.IsNullOrEmpty(Config.DatabaseTimeout))
                {
                    selectCommand.CommandTimeout = int.Parse(Config.DatabaseTimeout);
                }

                return selectCommand;
            }
            catch (Exception e)
            {
                string message = string.Format("Exception in Demo.DataUniverseFramework.SQL.SQLAccess.GetSqlCommand. The connection String is: '"
                    + Connection.ConnectionString + "'. Found this error {1} in the following query/statement {0}", statement, e.ToString());
                throw new FrameworkException(message, e);
            }
        }
    }
}
