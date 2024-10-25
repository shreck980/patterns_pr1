using noslq_pr.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Entities
{
    public class Customer :Person
    {
        
        public CustomerType CustomerType { get; set; }


        public Customer(CustomerBuilder c)
        {
            this.Id = c.Id;
            this.Name = c.Name;
            this.Email = c.Email;
            this.PhoneNumber = c.PhoneNumber;
            this.CustomerType = c.CustomerType;
            this.Surname = c.Surname;
            this.Address = c.Address;


        }
        public override string ToString()
        {
            return $"Customer ID: {Id}\n" +
                   $"Name: {Name}\n" +
                   $"Surname: {Surname}\n" +
                   $"Customer Type: {CustomerType}\n" +
                   $"Email: {Email}\n" +
                   $"Phone Number: {PhoneNumber}\n" +
                   $"Address: {Address}\n";
        }
    }
    public enum CustomerType
    {
        Retailer = 1,
        Distributor,
        DirectCustomer,
        OnlineStore,
        Author,
        Other

    }
}
