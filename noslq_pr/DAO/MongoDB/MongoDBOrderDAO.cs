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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace noslq_pr.DAO.MongoDB
{
    public class MongoDBOrderDAO : IOrderDAO
    {
        DAOConfig config;
        public MongoDBOrderDAO()
        {
            config=DAOConfig.GetDAOConfig();
        }
        public void AddOrder(Order o)
        {
            try
            {
                var client = new MongoClient(config.Url);
                var database = client.GetDatabase(config.Database);
                var authorCollection = database.GetCollection<Author>("author");
                var customerCollection = database.GetCollection<Customer>("customer");
                var printingHouseCollection = database.GetCollection<PrintingHouse>("printing_house");
                var orderCollection = database.GetCollection<BsonDocument>("order");

                o.Publications.ForEach(x => x.ObjectId = ObjectId.GenerateNewId());
                
              
                var bsonOrder = o.ToBsonDocument(); // Convert the Order object to a BsonDocument

                // Add the CustomerEmail field to the BsonDocument
                bsonOrder.Add("CustomerEmail", BsonValue.Create(o.Customer?.Email ?? "Unknown"));
                bsonOrder.Add("PrintingHouseName", BsonValue.Create(o.PrintingHouse.Name ?? "Unknown"));

                // Insert the BsonDocument into the collection
                orderCollection.InsertOne(bsonOrder);

                foreach (var p in o.Publications)
                {
                    var authorNamesPubl = new BsonArray();
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
                        {"Surname",a.Surname},
                        {"Pseudonym",a.Pseudonym}

                        };

                        authorNamesPubl.Add(authorName);

                    }
                    var filter = Builders<BsonDocument>.Filter.Eq("Publications._id", p.ObjectId);
                    var updateValuesList = new List<UpdateDefinition<BsonDocument>>()
                    {
                        Builders<BsonDocument>.Update.Set("Publications.$.Authors", authorNamesPubl),
                       

                    };
                    var update =  Builders<BsonDocument>.Update.Combine(updateValuesList);
                    orderCollection.UpdateOne(filter, update);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("Error occured " + e);
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

        public Order GetOrder(object objectId)
        {
            if (objectId is ObjectId id)
            {
             
                Order o = new OrderBuilder().Build();
                try
                {
                    var client = new MongoClient(config.Url);
                    var database = client.GetDatabase(config.Database);
                    var orderCollection = database.GetCollection<BsonDocument>("order");

                    var filter = Builders<BsonDocument>.Filter.Eq("_id", id);

                    var doc = orderCollection.Find<BsonDocument>(filter).First();
                    o = GetOrderLogic(doc,database);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occurred during aggregation: " + e);

                }
                return o;
            }
            throw new Exception("Wrong id type");

        }

        public List<Order> GetAllOrder()
        {
            List<Order> o = new List<Order>();
            try
            {
                var client = new MongoClient(config.Url);
                var database = client.GetDatabase(config.Database);
                var orderCollection = database.GetCollection<BsonDocument>("order");


                var filter = Builders<BsonDocument>.Filter.Empty;

                var doc = orderCollection.Find<BsonDocument>(filter).ToList();
                //Console.WriteLine(doc.ToJson());
                foreach (var singlDoc in doc)
                {
                    o.Add(GetOrderLogic(singlDoc, database));
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred during aggregation: " + e);

            }
            return o;
        }

        public Order  GetOrderLogic(BsonDocument doc,IMongoDatabase database)
        {
            var customerCollection = database.GetCollection<Customer>("customer");
            var authorCollection = database.GetCollection<Author>("author");
            var printHouseCollection = database.GetCollection<PrintingHouse>("printing_house");
            OrderBuilder ob =  new OrderBuilder();
            if (doc["CustomerEmail"].IsBsonNull)
            {
                throw new Exception("Order does not have customer");
            }

            Customer customer = customerCollection.Find(f=> f.Email== doc["CustomerEmail"].ToString()).FirstOrDefault();
            doc.Remove("CustomerEmail");
            if (doc["PrintingHouseName"].IsBsonNull)
            {
                throw new Exception("Order does not have printing housr");
            }
            PrintingHouse printingHouse = printHouseCollection.Find(f=>f.Name ==doc["PrintingHouseName"].ToString()).FirstOrDefault();
            doc.Remove("PrintingHouseName");
            var authors = new Dictionary<string, BsonArray>();
            foreach (var publDoc in doc["Publications"].AsBsonArray)
            {
                var publication = publDoc.AsBsonDocument;
                authors.Add(publication["_id"].ToString(), publication["Authors"].AsBsonArray);
                publication.Remove("Authors");
            }

            ob = BsonSerializer.Deserialize<OrderBuilder>(doc);
            ob.SetCustomer(customer).SetPrintingHouse(printingHouse);

            foreach (var key in authors.Keys)
            {
                ob.Publications.Find(x => x.ObjectId.ToString().Equals(key)).Authors = new List<Author>();
                foreach (var name in authors[key])
                {
                    Author author = authorCollection.Find(f =>
                        f.Name == name["Name"].ToString() &&
                        f.Surname == name["Surname"].ToString() &&
                        f.Pseudonym == name["Pseudonym"].ToString()).FirstOrDefault();
                    ob.Publications.Find(x => x.ObjectId.ToString().Equals(key)).Authors.Add(author);
                }
            }
            return ob.Build();
        }

        public List<Order> GetOrderbyCustomer(string email)
        {
          
                List<Order> o = new List<Order>();
                try
                {
                    var client = new MongoClient(config.Url);
                    var database = client.GetDatabase(config.Database);
                    var orderCollection = database.GetCollection<BsonDocument>("order");
                    

                    var filter = Builders<BsonDocument>.Filter.Eq("CustomerEmail", email);

                    var doc = orderCollection.Find<BsonDocument>(filter).ToList();
                    //Console.WriteLine(doc.ToJson());
                    foreach (var singlDoc in doc)
                    {
                        o.Add(GetOrderLogic(singlDoc,database));
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occurred during aggregation: " + e);

                }
                return o;
            
        }

        public List<Order> GetOrderbyDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrderbyStatus(OrderStatus status)
        {
            throw new NotImplementedException();
        }

        public void Notify(string operation, object criteria, object result)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Order o)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderPublication(Order o)
        {
            throw new NotImplementedException();
        }

        

        public void AddOrders(List<Order> list)
        {
            try
            {
                var client = new MongoClient(config.Url);
                var database = client.GetDatabase(config.Database);
                var authorCollection = database.GetCollection<Author>("author");
                var customerCollection = database.GetCollection<Customer>("customer");
                var printingHouseCollection = database.GetCollection<PrintingHouse>("printing_house");
                var orderCollection = database.GetCollection<BsonDocument>("order");

                foreach (var o in list)
                {
                    o.Publications.ForEach(x => x.ObjectId = ObjectId.GenerateNewId());

                    Customer c  = customerCollection.Find(f => (f.Email == o.Customer.Email)).FirstOrDefault();
                    if (c is null)
                    {
                        c = o.Customer;
                        c.Address.ObjectId = ObjectId.GenerateNewId();
                        customerCollection.InsertOne(c);
                    }
                    PrintingHouse ph =printingHouseCollection.Find(f => (f.Name == o.PrintingHouse.Name)).FirstOrDefault();
                    if (ph is null)
                    {
                        ph = o.PrintingHouse;
                        ph.Address.ObjectId = ObjectId.GenerateNewId();
                        printingHouseCollection.InsertOne(ph);
                    }

                    var bsonOrder = o.ToBsonDocument(); // Convert the Order object to a BsonDocument

                    // Add the CustomerEmail field to the BsonDocument
                    bsonOrder.Add("CustomerEmail", BsonValue.Create(o.Customer?.Email ?? "Unknown"));
                    bsonOrder.Add("PrintingHouseName", BsonValue.Create(o.PrintingHouse.Name ?? "Unknown"));

                    // Insert the BsonDocument into the collection
                    orderCollection.InsertOne(bsonOrder);

                    foreach (var p in o.Publications)
                    {
                        var authorNamesPubl = new BsonArray();
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
                        {"Surname",a.Surname},
                        {"Pseudonym",a.Pseudonym}

                        };

                            authorNamesPubl.Add(authorName);

                        }
                        var filter = Builders<BsonDocument>.Filter.Eq("Publications._id", p.ObjectId);
                        var updateValuesList = new List<UpdateDefinition<BsonDocument>>()
                    {
                        Builders<BsonDocument>.Update.Set("Publications.$.Authors", authorNamesPubl),


                    };
                        var update = Builders<BsonDocument>.Update.Combine(updateValuesList);
                        orderCollection.UpdateOne(filter, update);
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Error occured " + e);
            }
        }
    }
}
