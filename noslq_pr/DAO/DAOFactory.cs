using noslq_pr.DAO.MYSQL;
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

        private readonly DAOConfig config;
        private Dictionary<Type, object> data = new Dictionary<Type, object>();
        private DAOFactory() { 
            config = DAOConfig.GetDAOConfig();
        }

        public IPublicationDAO? GetPublicationDAO()
        {
            if (config.DatabaseType == "MySQL")
            {
                if(data.TryGetValue(typeof(MYSQLPublicationDAO), out var publciationDAO)){
                    return (MYSQLPublicationDAO)publciationDAO;
                }
                else
                {
                    MYSQLPublicationDAO publicationDAO = new MYSQLPublicationDAO();
                    data.Add(typeof(MYSQLPublicationDAO), publicationDAO);
                    return publicationDAO;
                }
            }
            return null;
        }


        public ICustomerDAO? GetCustomerDAO()
        {
            if (config.DatabaseType == "MySQL")
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
            return null;
        }

        public IAuthorDAO? GetAuthorDAO()
        {
            if (config.DatabaseType == "MySQL")
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
            return null;
        }


        public IPrintingHouseDAO? GetPrintingHouseDAO()
        {
            if (config.DatabaseType == "MySQL")
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
            return null;
        }

        public IOrderDAO? GetOrderDAO()
        {
            if (config.DatabaseType == "MySQL")
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
            return null;
        }

    }
}
