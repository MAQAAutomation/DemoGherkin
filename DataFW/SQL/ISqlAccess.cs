using System.Data;

namespace Demo.DataUniverseFramework.SQL
{
    public interface ISqlAccess
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        SqlConfig Config { get; }
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="dataAdapter">The data adapter.</param>
        /// <returns></returns>
        DataTable ExecuteQuery(IDbDataAdapter dataAdapter);
        /// <summary>
        /// Executes the query string result.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        string ExecuteQueryStringResult(IDbCommand command);
        /// <summary>
        /// Executes the statement.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        int ExecuteStatement(IDbCommand command);
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns></returns>
        IDbCommand CreateConnection(string statement);
        /// <summary>
        /// Gets the data adapter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        IDbDataAdapter GetDataAdapter(IDbCommand command);
    }
}
