using noslq_pr.DAO.MongoDB;
using noslq_pr.DAO.MYSQL;
using noslq_pr.DAO.NoSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public class DAOFactory
    {
        private static readonly Lazy<DAOFactory> _instance = new Lazy<DAOFactory>(() => new DAOFactory());
        public static DAOFactory Instance => _instance.Value;

        private  DAOConfig config;
        private Dictionary<Type, object> data = new Dictionary<Type, object>();
        private DAOFactory()
        {
            config = DAOConfig.GetDAOConfig();
        }

        public IPublicationDAO? GetPublicationDAO()
        {
            if (config.DatabaseType == DataBaseType.MySQL.ToString())
            {
                if (data.TryGetValue(typeof(MYSQLPublicationDAO), out var publciationDAO))
                {
                    return (MYSQLPublicationDAO)publciationDAO;
                }
                else
                {
                    MYSQLPublicationDAO publicationDAO = new MYSQLPublicationDAO();
                    data.Add(typeof(MYSQLPublicationDAO), publicationDAO);
                    return publicationDAO;
                }
            }
            if (config.DatabaseType == DataBaseType.MongoDB.ToString())
            {
                if (data.TryGetValue(typeof(MongoDBPublicationDAO), out var publciationDAO))
                {
                    return (MongoDBPublicationDAO)publciationDAO;
                }
                else
                {
                    MongoDBPublicationDAO publicationDAO = new MongoDBPublicationDAO();
                    data.Add(typeof(MongoDBPublicationDAO), publicationDAO);
                    return publicationDAO;
                }
            }
            return null;
        }


        public ICustomerDAO? GetCustomerDAO()
        {
            config = DAOConfig.GetDAOConfig();
            if (config.DatabaseType == DataBaseType.MySQL.ToString())
            {
                if (data.TryGetValue(typeof(MySQLCustomerDAO), out var customerDAO))
                {
                    return (MySQLCustomerDAO)customerDAO;
                }
                else
                {
                    ICustomerDAO customerDAo = new MySQLCustomerDAO();
                    data.Add(typeof(MySQLCustomerDAO), customerDAo);
                    return customerDAo;
                }
            }

            if (config.DatabaseType == DataBaseType.MongoDB.ToString())
            {
                if (data.TryGetValue(typeof(MongoDBCustomerDAO), out var customerDAO))
                {
                    return (MongoDBCustomerDAO)customerDAO;
                }
                else
                {
                    ICustomerDAO customerDAo = new MongoDBCustomerDAO();
                    data.Add(typeof(MongoDBCustomerDAO), customerDAo);
                    return customerDAo;
                }
            }
            return null;
        }

        public IAuthorDAO? GetAuthorDAO()
        {
            config = DAOConfig.GetDAOConfig();
            if (config.DatabaseType == DataBaseType.MySQL.ToString())
            {
                if (data.TryGetValue(typeof(MySQLAuthorDAO), out var authorDAO))
                {
                    return (MySQLAuthorDAO)authorDAO;
                }
                else
                {
                    IAuthorDAO authorDao = new MySQLAuthorDAO();
                    data.Add(typeof(MySQLAuthorDAO), authorDao);
                    return authorDao;
                }
            }

            if (config.DatabaseType == DataBaseType.MongoDB.ToString())
            {
                if (data.TryGetValue(typeof(MongoDBAuthorDAO), out var authorDAO))
                {
                    return (MongoDBAuthorDAO)authorDAO;
                }
                else
                {
                    IAuthorDAO authorDao = new MongoDBAuthorDAO();
                    data.Add(typeof(MongoDBAuthorDAO), authorDao);
                    return authorDao;
                }
            }
            return null;
        }


        public IPrintingHouseDAO? GetPrintingHouseDAO()
        {
            config = DAOConfig.GetDAOConfig();
            if (config.DatabaseType == DataBaseType.MySQL.ToString())
            {
                if (data.TryGetValue(typeof(MySQLPrintingHouseDAO), out var printingHouseDAO))
                {
                    return (MySQLPrintingHouseDAO)printingHouseDAO;
                }
                else
                {
                    IPrintingHouseDAO printingHouseDAO1 = new MySQLPrintingHouseDAO();
                    data.Add(typeof(MySQLPrintingHouseDAO), printingHouseDAO1);
                    return printingHouseDAO1;
                }
            }
            if (config.DatabaseType == DataBaseType.MongoDB.ToString())
            {
                if (data.TryGetValue(typeof(MongoDBPrintingHouseDAO), out var printingHouseDAO))
                {
                    return (MongoDBPrintingHouseDAO)printingHouseDAO;
                }
                else
                {
                    IPrintingHouseDAO printingHouseDAO1 = new MongoDBPrintingHouseDAO();
                    data.Add(typeof(MongoDBPrintingHouseDAO), printingHouseDAO1);
                    return printingHouseDAO1;
                }
            }
            return null;
        }

        public IOrderDAO? GetOrderDAO()
        {
            config = DAOConfig.GetDAOConfig();
            if (config.DatabaseType == DataBaseType.MySQL.ToString())
            {
                if (data.TryGetValue(typeof(MySQLOrderDAO), out var orderDAO))
                {
                    return (MySQLOrderDAO)orderDAO;
                }
                else
                {
                    IOrderDAO orderDAO1 = new MySQLOrderDAO();
                    data.Add(typeof(MySQLOrderDAO), orderDAO1);
                    return orderDAO1;
                }



            }
            if (config.DatabaseType == DataBaseType.MongoDB.ToString())
            {
                if (data.TryGetValue(typeof(MongoDBOrderDAO), out var orderDAO))
                {
                    return (MongoDBOrderDAO)orderDAO;
                }
                else
                {
                    IOrderDAO orderDAO1 = new MongoDBOrderDAO();
                    data.Add(typeof(MongoDBOrderDAO), orderDAO1);
                    return orderDAO1;
                }
            }
            return null;

        }
    }
}
