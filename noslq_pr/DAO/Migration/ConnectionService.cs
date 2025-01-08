using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace noslq_pr.DAO.Migration
{
    public class ConnectionService
    {
        public ConnectionService() { }

        public static void EstablishConnection(DataBaseType dataBaseType) { 
            
            switch (dataBaseType)
            {
                case DataBaseType.MySQL:
                    SetUpMySQLConnection();
                    break;
                case DataBaseType.MongoDB:
                    SetUpMongoDbConnection();
                    break;
                default:
                    throw new Exception("Unknown database type");
            }
        
        }
        private static void SetUpMySQLConnection()
        {
            ConnectionStrings connectionStrings = new ConnectionStrings
            {
                Server = "localhost",
                Port = "3306",             
                User = "root",
                Password = "shreck980",
                Database = "publshing_house_nosql",
                DatabaseType = "MySQL"    // Or "MongoDB"
            };
            var jsonObject = new
            {
                ConnectionStrings = connectionStrings
            };

            string json = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });

           

           
            File.WriteAllText("D:\\projects\\C#\\nosql_3\\noslq_pr\\appsettings.json",  json);

            Console.WriteLine("appsettings.json has been updated.");
        }

        private static void SetUpMongoDbConnection()
        {
            ConnectionStrings connectionStrings = new ConnectionStrings
            {
                Server = "localhost",
                Port = "27017",             
                User = "root",
                Database = "publshing_house_nosql",
                DatabaseType = "MongoDB"    
            };

            var jsonObject = new
            {
                ConnectionStrings = connectionStrings
            };

            string json = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });


            File.WriteAllText("D:\\projects\\C#\\nosql_3\\noslq_pr\\appsettings.json", json);

            Console.WriteLine("appsettings.json has been updated.");
        }

        public class ConnectionStrings
        {
            public string Server { get; set; }
            public string Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public string Database { get; set; }
            public string DatabaseType { get; set; }
        }
    }
        
}
