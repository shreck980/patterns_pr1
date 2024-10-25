using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public class DAOConfig
    {
        private static DAOConfig _daoConfig; 
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Url { get; set; }
        public string DatabaseType { get; set; }

        private DAOConfig()
        {
            InitConfig();

        }

        public static DAOConfig GetDAOConfig()
        {
            if(_daoConfig == null)
            {
                _daoConfig= new DAOConfig();
                
            }

            return _daoConfig;
        }


        private void InitConfig()
        {
            var databaseConfig = new ConfigurationBuilder().AddJsonFile("D:\\C#\\noslq_pr\\noslq_pr\\appsettings.json").Build().GetSection("ConnectionStrings");
            var _server = databaseConfig["Server"];
            if (_server != null)
            {
                Server = _server;
            }
            var _port = databaseConfig["Port"];
            if (_port != null)
            {
                Port = _port;
            }

            var _user = databaseConfig["User"];
            if (_user != null)
            {
                User = _user;
            }
            var _password = databaseConfig["Password"];

            if (_password != null)
            {
                Password = _password;
            }

            var _database = databaseConfig["Database"];

            if (_database != null)
            {
                Database = _database;
            }

            var _databaseType = databaseConfig["DatabaseType"];

            if (_databaseType != null)
            {
                DatabaseType = _databaseType;
            }

            Url = $"Server={Server};Port={Port};User ID={User};Password={Password};Database={Database}";
        }
    }
}
