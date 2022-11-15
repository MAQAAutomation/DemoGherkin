using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;

namespace Demo.DataUniverseFramework.SQL
{
    public class SqlConfig
    {
        public string Database { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseTimeout { get; set; }

        private string m_ConectionString = null;
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(m_ConectionString))
                {
                    if (string.IsNullOrEmpty(DatabaseUser))
                    {
                        m_ConectionString = Constants.SQL_CONNECTION_STRING_WITHOUT_USER;
                    }
                    else
                    {
                        m_ConectionString = Constants.SQL_CONNECTION_STRING_WITH_USER;
                    }

                    return m_ConectionString.Replace(Constants.DB_XML_ELEMENT, Database)
                                            .Replace(Constants.DB_SERVER_XML_ELEMENT, DatabaseServer)
                                            .Replace(Constants.DB_USER_XML_ELEMENT, DatabaseUser)
                                            .Replace(Constants.DB_PASS_XML_ELEMENT, DatabasePassword);
                }
                return m_ConectionString;
            }
            set
            {
                m_ConectionString = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConfig"/> class.
        /// </summary>
        public SqlConfig() { }

        /// <summary>
        /// Contructor 
        /// </summary>
        /// <param name="dataBase">Name os the schema of the data base </param>
        /// <param name="dataBaseServer">Name of the server</param>
        /// <param name="dataBaseUser">User of the Data Base</param>
        /// <param name="dataBasePassword">Password of the Data Base</param>
        public SqlConfig(string dataBase, string dataBaseServer, string dataBaseUser, string dataBasePassword)
        {
            Database = dataBase;
            DatabaseServer = dataBaseServer;
            DatabaseUser = dataBaseUser;
            DatabasePassword = dataBasePassword;
            DatabaseTimeout = "60";
        }

        /// <summary>
        /// Default Contructor, read de Data Base configuration parameters from a XML file
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="pathConfigFile">The path configuration file.</param>
        /// <exception cref="FrameworkException"></exception>
        public SqlConfig(string server, string pathConfigFile = null)
        {
            if (!string.IsNullOrEmpty(server))
            {
                ConnectionString = BaseFileUtils.ReadRemoteConnectionString(server, pathConfigFile);
                DatabaseTimeout = "60";
            }
            else
            {
                throw new FrameworkException(string.Format("Exception try to get a connection to the data Base. The server {0} to get the SQL parameter is no valid", server));
            }
        }        
    }
}
