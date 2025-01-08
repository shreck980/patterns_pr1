using Bogus.DataSets;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using noslq_pr.Builder;
using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO.MongoDB
{
    public class MongoDBPublicationDAO : IPublicationDAO
    {

        DAOConfig config;
        public MongoDBPublicationDAO()
        {
            config =  DAOConfig.GetDAOConfig();
        }
        public void AddPublication(Publication p)
        {
            try
            {
                var client = new MongoClient(config.Url);
                var database = client.GetDatabase(config.Database);
                var publCollection = database.GetCollection<Publication>("publication");
                var authorCollection = database.GetCollection<Author>("author");

                var authorNames = new BsonArray();
                foreach (Author a in p.Authors)
                {
                    Author au = authorCollection.Find(f => (f.Name == a.Name && f.Surname == a.Surname)).FirstOrDefault();
                    if (au is null)
                    {
                        a.Address.ObjectId = ObjectId.GenerateNewId();
                        authorCollection.InsertOne(a);
                    }
                    var authorName = new BsonDocument {
                        {"Name", a.Name },
                        {"Surname",a.Surname}
                        };

                    authorNames.Add(authorName);
                   
                }

                publCollection.InsertOne(p);
                var update = Builders<Publication>.Update.Set("Authors", authorNames);
                publCollection.UpdateOne((f => (f.ObjectId == p.ObjectId)),update);
                Console.WriteLine($"Publication {p.Title} added successfully!");
            }
            catch(Exception e) {

                Console.WriteLine("Error occured "+e);
            }
        }

        public void Attach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Detach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public Publication GetPublication(object objectId)
        {
            if (objectId is ObjectId id)
            {
                Publication p = new PublicationBuilder().Build();
                try
                {
                    var client = new MongoClient(config.Url);
                    var database = client.GetDatabase(config.Database);
                    var publCollection = database.GetCollection<BsonDocument>("publication");
                    var authorCollection = database.GetCollection<Author>("author");
                    //var projection = Builders<BsonDocument>.Projection.Exclude("Authors");
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", id);

                    var doc = publCollection.Find<BsonDocument>(filter).First();//Project(projection).ToList();
                    var authorsNames = doc["Authors"].AsBsonArray;
                    doc.Remove("Authors");
                    p = BsonSerializer.Deserialize<Publication>(doc);
                    p.Authors = new List<Author>();
                    if (authorsNames.Any())
                    {

                        foreach (var name in authorsNames)
                        {
                            p.Authors.Add(new AuthorBuilder().SetName(name["Name"].ToString())
                                .SetSurname(name["Surname"].ToString()).Build());
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("error " + e);
                }
                return p;
            }
            throw new Exception("Wrong id type");
        }

        public List<Publication> GetPublicationByAuthorId(int authorId)
        {
            throw new NotImplementedException();
        }

        public List<Publication> GetPublicationByCustomerId(int customerId)
        {
            throw new NotImplementedException();
        }

        public List<Publication> GetPublicationByOrderId(int orderId)
        {
            throw new NotImplementedException();
        }

        public List<Publication> GetPublicationByTitle(string title)
        {
            throw new NotImplementedException();
        }

        public void Notify(string operation, object criteria, object result)
        {
            throw new NotImplementedException();
        }

        public void UpdatePublication(Publication p)
        {
            throw new NotImplementedException();
        }
    }
}
