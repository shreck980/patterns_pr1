using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Builder
{
    public class CustomerBuilder : PersonBuilder<CustomerBuilder>
    {

        public CustomerType CustomerType { get;  private set; }

        public CustomerBuilder()
        {

            Address = new Address();
            CustomerType = CustomerType.Other;
            Id = 0;
        }

        

        public CustomerBuilder SetCustomerType(CustomerType customerType)
        {
            this.CustomerType = customerType;
            return this;
        }


        public Customer Build()
        {
            return new Customer(this);
        }

        public void Reset()
        {
            this.Address = new Address();
            Id = 0;
            Name = null;
            Surname = String.Empty;

            Email = String.Empty;
            PhoneNumber = String.Empty;
            CustomerType =CustomerType.Other;
        }
    }
}
