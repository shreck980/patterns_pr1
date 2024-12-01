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
    public class OrderDataGenerator : IFakeDataGenerator<OrderBuilder>
    {
        Faker<OrderBuilder> faker;
        PublicationDataGenerator publicationGenerator =  new PublicationDataGenerator();

        
        public OrderBuilder GetFakeData()
        {
             faker = new Faker<OrderBuilder>()
            .RuleFor(o => o.AcceptanceDate, f => f.Date.Past(1))
            .RuleFor(o => o.Price, f => f.Finance.Amount(100, 5000))
            .RuleFor(o => o.Publications, f => Enumerable.Range(0, f.Random.Int(1, 5))
                                                         .Select(_ => publicationGenerator.GetFakeData())
                                                         .ToList())

            .RuleFor(o => o.Status, f => (OrderStatus)f.Random.Int(1,5));
            return faker.Generate();
        }
    }
}
