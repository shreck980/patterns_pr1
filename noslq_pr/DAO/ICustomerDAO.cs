using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface ICustomerDAO : ISubject
    {
        void AddCustomer(Customer p);
        void AddCustomers(List<Customer> list);
        Customer GetCustomer(object id);
        List<Customer> GetAllCustomers();
        Customer GetCustomerByName(string name,string surname);
        List<Customer> GetCustomerByCountry(string country);
        List<Customer> GetAllCustomers(int limit);
        void UpdateCustomer(Customer a);
    }
}
