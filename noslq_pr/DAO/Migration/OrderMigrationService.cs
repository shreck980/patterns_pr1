using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO.Migration
{
    public class OrderMigrationService : MigrationService
    {
        private readonly DAOFactory daoFactory;

        public OrderMigrationService(DAOFactory dAOFactory)
        {
            daoFactory = dAOFactory;

        }

        public void Migration(DataBaseType sourse, DataBaseType destination)
        {
            try
            {
                ConnectionService.EstablishConnection(sourse);
                IOrderDAO orderDAO = daoFactory.GetOrderDAO();
                if (orderDAO == null) throw new Exception("Order is null");
                List<Order> orders = orderDAO.GetAllOrder();

                if (orders.Count == 0) throw new Exception("No customers in mysql database found");

                ConnectionService.EstablishConnection(destination);
                orderDAO = daoFactory.GetOrderDAO();
                if (orderDAO == null) throw new Exception("Customer is null");
                orderDAO.AddOrders(orders);

                Console.WriteLine($"Successfully migrated data from {sourse} to {destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
