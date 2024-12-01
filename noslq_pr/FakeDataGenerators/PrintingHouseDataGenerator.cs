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
    public class PrintingHouseDataGenerator : IFakeDataGenerator<PrintingHouse>
    {
        Faker<PrintingHouseBuilder> printingHouseModel;
        Faker<Address> addressModel;
        public PrintingHouse GetFakeData()
        {
            printingHouseModel = new Faker<PrintingHouseBuilder>()
            .RuleFor(u => u.Name, f => f.Company.CompanyName())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumberFormat());

          
                  


            addressModel = new Faker<Address>()
     
           .RuleFor(u => u.Country, f => f.Address.Country())   
           .RuleFor(u => u.City, f => f.Address.City())         
           .RuleFor(u => u.Street, f => f.Address.StreetName())  
           .RuleFor(u => u.House, f => f.Random.Int(1, 500))    
           .RuleFor(u => u.Apartment, f => f.Random.Bool() ? f.Random.Int(1, 200) : 0);
            
            var fakePrintingHouse = printingHouseModel.Generate();
            var fakeAddress = addressModel.Generate();



            return fakePrintingHouse
            .SetCountry(fakeAddress.Country)
            .SetCity(fakeAddress.City)
            .SetStreet(fakeAddress.Street)
            .SetHouse(fakeAddress.House)
            .SetAppartment(fakeAddress.Apartment.Value).Build();
        }
    }
}
