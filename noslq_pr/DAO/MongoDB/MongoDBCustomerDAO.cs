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
    public class MongoDBCustomerDAO : ICustomerDAO
    {
        DAOConfig daoConfig;
        public MongoDBCustomerDAO()
        {
            daoConfig = DAOConfig.GetDAOConfig();

        }

        public void AddCustomer(Customer p)
        {

            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Customer>("customer");
                p.Address.ObjectId = ObjectId.GenerateNewId();
                collection.InsertOne(p);


            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void AddCustomers(List<Customer> list)
        {

            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Customer>("customer");
                list.ForEach(p => p.Address.ObjectId = ObjectId.GenerateNewId());
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

        public List<Customer> GetAllCustomers(int limit)
        {
            List<Customer> all = new List<Customer>();
            try
            {
              
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Customer>("customer");
                var filter = Builders<Customer>.Filter.Empty;
                return collection.Find(filter).Limit(limit).ToList();

                

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return all;
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> all = new List<Customer>();
            try
            {

                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Customer>("customer");
                var filter = Builders<Customer>.Filter.Empty;
                return collection.Find(filter).ToList();



            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return all;
        }

        public Customer GetCustomer(object objectId)
        {
            if (objectId is ObjectId id)
            {

                try
                {
                    var client = new MongoClient(daoConfig.Url);
                    var database = client.GetDatabase(daoConfig.Database);
                    var collection = database.GetCollection<Customer>("customer");
                    var filter = Builders<Customer>.Filter.Eq("_id", id);
                    Customer c = collection.Find(f => f.ObjectId == id).FirstOrDefault();

                    return c;

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            throw new Exception("Id value wrong type");
        }

        public List<Customer> GetCustomerByCountry(string country)
        {
            throw new NotImplementedException();
        }

        public Customer GetCustomerByName(string name, string surname)
        {
            Customer c = new CustomerBuilder().Build();
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Customer>("customer");
                var filter = Builders<Customer>.Filter.And(
                    Builders<Customer>.Filter.Eq("Name", name),
                    Builders<Customer>.Filter.Eq("Surname", surname)
                    );
                c = collection.Find(filter).FirstOrDefault();

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

        public void UpdateCustomer(Customer a)
        {

            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Customer>("customer");
                var filter = Builders<Customer>.Filter.Eq("_id", a.ObjectId);

                var updateDef = Builders<Customer>.Update;
                var updateValuesList = new List<UpdateDefinition<Customer>>();

                if (!System.String.IsNullOrEmpty(a.Email))
                    updateValuesList.Add(updateDef.Set("Email", a.Email));
                if (!System.String.IsNullOrEmpty(a.PhoneNumber))
                    updateValuesList.Add(updateDef.Set("PhoneNumber", a.PhoneNumber));
                if (!System.String.IsNullOrEmpty(a.Name))
                    updateValuesList.Add(updateDef.Set("Name", a.Name));
                if (!System.String.IsNullOrEmpty(a.Surname))
                    updateValuesList.Add(updateDef.Set("Surname", a.Surname));
                if (a.CustomerType!= CustomerType.Other)
                    updateValuesList.Add(updateDef.Set("CustomerType", a.CustomerType));
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

                var update = Builders<Customer>.Update.Combine(updateValuesList);
                var result = collection.UpdateOne(filter, update);
                if (result.ModifiedCount > 0)
                {
                    Console.WriteLine("Customer updated");
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

        }
    }
}
