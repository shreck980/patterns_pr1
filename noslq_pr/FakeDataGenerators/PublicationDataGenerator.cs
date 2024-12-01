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
    public class PublicationDataGenerator : IFakeDataGenerator<Publication>
    {
        Faker<PublicationBuilder> faker;
        AuthorDataGenerator authorDataGenerator;

    

        public PublicationDataGenerator()
        {
            
            authorDataGenerator = new AuthorDataGenerator();
           
        }
        public Publication GetFakeData()
        {
            faker = new Faker<PublicationBuilder>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(p => p.PageCount, f => f.Random.Int(50, 1000))
            .RuleFor(p => p.Circulation, f => f.Random.Int(1000, 50000))
            .RuleFor(p => p.Price, f => f.Finance.Amount(400, 1000))
            .RuleFor(p => p.Authors, f => Enumerable.Range(1, f.Random.Int(1,4))
                                     .Select(_=>authorDataGenerator.GetFakeData())
                                     .ToList())
            .RuleFor(p => p.Genre, f => (Genre)f.Random.Int(1, 10))
            .RuleFor(p => p.PrintQuality, f => (PrintQuality)f.Random.Int(1, 4))
            .RuleFor(p => p.Quantity, f => f.Random.Int(300, 7000));
            return faker.Generate().Build();
        }
    }
}
