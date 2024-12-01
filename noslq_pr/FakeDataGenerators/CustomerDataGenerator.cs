using Bogus;
using noslq_pr.Builder;
using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.FakeDataGenerators
{
    public class CustomerDataGenerator : IFakeDataGenerator<Customer>
    {
        Faker<CustomerBuilder> customerModel;
        Faker<Address> addressModel;
        public Customer GetFakeData()
        {
            customerModel = new Faker<CustomerBuilder>()

            .RuleFor(u => u.Name, f => f.Name.FirstName())
            .RuleFor(u => u.Surname, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumberFormat())
            .RuleFor(u => u.CustomerType, f => (CustomerType)f.Random.Int(1, 5));
                  


            addressModel = new Faker<Address>()
     
           .RuleFor(u => u.Country, f => f.Address.Country())   
           .RuleFor(u => u.City, f => f.Address.City())         
           .RuleFor(u => u.Street, f => f.Address.StreetName())  
           .RuleFor(u => u.House, f => f.Random.Int(1, 500))    
           .RuleFor(u => u.Apartment, f => f.Random.Bool() ? f.Random.Int(1, 200) : 0);
            
            var fakeCustomer = customerModel.Generate();
            var fakeAddress = addressModel.Generate();



            return fakeCustomer
            .SetCountry(fakeAddress.Country)
            .SetCity(fakeAddress.City)
            .SetStreet(fakeAddress.Street)
            .SetHouse(fakeAddress.House)
            .SetAppartment(fakeAddress.Apartment.Value).Build();
        }
    }
}
