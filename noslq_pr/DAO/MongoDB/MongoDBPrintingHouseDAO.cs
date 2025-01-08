using Bogus.DataSets;
using MongoDB.Bson;
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
    public class MongoDBPrintingHouseDAO : IPrintingHouseDAO
    {
        DAOConfig daoConfig;
        public MongoDBPrintingHouseDAO()
        {
            daoConfig =  DAOConfig.GetDAOConfig();
        }
        public void AddPrintingHouse(PrintingHouse p)
        {
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<PrintingHouse>("printing_house");
                p.Address.ObjectId = ObjectId.GenerateNewId();
                collection.InsertOne(p);

                Console.WriteLine($"Printing House {p.Name} added successfully!");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void AddPrintingHouses(List<PrintingHouse> list)
        {
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<PrintingHouse>("printing_house");
                list.ForEach(p=>p.Address.ObjectId = ObjectId.GenerateNewId());
                collection.InsertMany(list);

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
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

        public List<PrintingHouse> GetAllPrintingHouse()
        {

            List<PrintingHouse> c = new List<PrintingHouse>();
            try
            {

                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<PrintingHouse>("printing_house");
                c = collection.Find(_=>true).ToList();

                

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return c;

        }

        public PrintingHouse GetPrintingHouse(object objectId)
        {
            if (objectId is ObjectId id)
            {

                try
                {
                    var client = new MongoClient(daoConfig.Url);
                    var database = client.GetDatabase(daoConfig.Database);
                    var collection = database.GetCollection<PrintingHouse>("printing_house");
                    var filter = Builders<PrintingHouse>.Filter.Eq("_id", id);
                    PrintingHouse c = collection.Find(f => f.ObjectId == id).FirstOrDefault();

                    return c;

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            throw new Exception("Id value wrong type");
        }

        public List<PrintingHouse> GetPrintingHouseByCountry(string country)
        {
            List<PrintingHouse> c = new List<PrintingHouse>();

            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<PrintingHouse>("printing_house");

                c = collection.Find(f => f.Address.Country.Equals(country)).ToList();

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return c;
        }

        public PrintingHouse GetPrintingHouseByName(string name)
        {
            PrintingHouse c = new PrintingHouseBuilder().Build();
             
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<PrintingHouse>("printing_house");

                c = collection.Find(f => f.Name.Equals(name)).FirstOrDefault();

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return c;
        }

        public void Notify(string operation, object criteria, object result)
        {
            throw new NotImplementedException();
        }

        public void UpdatePrintingHouse(PrintingHouse a)
        {
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<PrintingHouse>("printing_house");
                var filter = Builders<PrintingHouse>.Filter.Eq("_id", a.ObjectId);

                var updateDef = Builders<PrintingHouse>.Update;
                var updateValuesList = new List<UpdateDefinition<PrintingHouse>>();

              
                if (!System.String.IsNullOrEmpty(a.PhoneNumber))
                    updateValuesList.Add(updateDef.Set("PhoneNumber", a.PhoneNumber));
                if (!System.String.IsNullOrEmpty(a.Name))
                    updateValuesList.Add(updateDef.Set("Name", a.Name));
                if (!System.String.IsNullOrEmpty(a.Address.Country))
                    updateValuesList.Add(updateDef.Set("Address.Country", a.Address.Country));
                if (!System.String.IsNullOrEmpty(a.Address.City))
                    updateValuesList.Add(updateDef.Set("Address.City", a.Address.City));
                if (!System.String.IsNullOrEmpty(a.Address.Street))
                    updateValuesList.Add(updateDef.Set("Address.Street", a.Address.Street));
                if (a.Address.House != 0)
                    updateValuesList.Add(updateDef.Set("Address.House", a.Address.House));
                if (a.Address.Apartment.HasValue)
                    updateValuesList.Add(updateDef.Set("Address.Apartment", a.Address.Apartment));

                var update = Builders<PrintingHouse>.Update.Combine(updateValuesList);
                var result = collection.UpdateOne(filter, update);
                if (result.ModifiedCount > 0)
                {
                    Console.WriteLine("Printing House updated.");
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
