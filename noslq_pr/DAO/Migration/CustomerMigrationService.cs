using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO.Migration
{
    public class CustomerMigrationService : MigrationService
    {

        private readonly DAOFactory daoFactory;

        public CustomerMigrationService(DAOFactory dAOFactory)
        {
            daoFactory = dAOFactory;


        }

        public void Migration(DataBaseType sourse, DataBaseType destination)
        {
            try
            {
                ConnectionService.EstablishConnection(sourse);
                ICustomerDAO customerDAO = daoFactory.GetCustomerDAO();
                if (customerDAO == null) throw new Exception("Author is null");
                List<Customer> customers = customerDAO.GetAllCustomers();

                if (customers.Count == 0) throw new Exception("No customers in mysql database found");

                ConnectionService.EstablishConnection(destination);
                customerDAO = daoFactory.GetCustomerDAO();
                if (customerDAO == null) throw new Exception("Customer is null");
                customerDAO.AddCustomers(customers);

                Console.WriteLine($"Successfully migrated customer data from {sourse} to {destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
