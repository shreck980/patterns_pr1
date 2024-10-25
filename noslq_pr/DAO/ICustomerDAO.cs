using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface ICustomerDAO
    {
        void AddCustomer(Customer p);
        Customer GetCustomer(int id);
        Customer GetCustomerByName(string name,string surname);
        List<Customer> GetCustomerByCountry(string country);
        void UpdateCustomer(Customer a);
    }
}
