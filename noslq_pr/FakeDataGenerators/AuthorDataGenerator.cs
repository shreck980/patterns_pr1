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
    public class AuthorDataGenerator : IFakeDataGenerator<Author>
    {
        Faker<Author> authorModel;
        Faker<Address> addressModel;
        public Author GetFakeData()
        {
            authorModel = new Faker<Author>()
                      
            .RuleFor(u => u.Name, f => f.Name.FirstName())                
            .RuleFor(u => u.Surname, f => f.Name.LastName())              
            .RuleFor(u => u.Email, f => f.Internet.Email())              
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())      
                  
            .RuleFor(u => u.Pseudonym, f => f.Name.FirstName() + " " + f.Name.LastName());

            addressModel = new Faker<Address>()
     
           .RuleFor(u => u.Country, f => f.Address.Country())   
           .RuleFor(u => u.City, f => f.Address.City())         
           .RuleFor(u => u.Street, f => f.Address.StreetName())  
           .RuleFor(u => u.House, f => f.Random.Int(1, 500))    
           .RuleFor(u => u.Apartment, f => f.Random.Bool() ? f.Random.Int(1, 200) : null);
            
            var fakeAuthor = authorModel.Generate();
            var fakeAddress = addressModel.Generate();



            return new AuthorBuilder()
            .SetId(fakeAuthor.Id)
            .SetName(fakeAuthor.Name)
            .SetSurname(fakeAuthor.Surname)
            .SetEmail(fakeAuthor.Email)
            .SetPhoneNumber(fakeAuthor.PhoneNumber)
            .SetCountry(fakeAddress.Country)
            .SetCity(fakeAddress.City)
            .SetStreet(fakeAddress.Street)
            .SetHouse(fakeAddress.House)
            .SetAppartment(fakeAddress.Apartment.Value).Build();
        }
    }
}
