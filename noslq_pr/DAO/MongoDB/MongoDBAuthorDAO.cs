using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MongoDB.Bson;
using noslq_pr.Builder;
using System.ComponentModel.DataAnnotations;
using Bogus.DataSets;


namespace noslq_pr.DAO.NoSQL
{
    public class MongoDBAuthorDAO : IAuthorDAO
    {
        DAOConfig daoConfig;
        public MongoDBAuthorDAO()
        {
            daoConfig = DAOConfig.GetDAOConfig();
           
        }

        public void AddAuthor(Author a)
        {
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Author>("author");
                a.Address.ObjectId = ObjectId.GenerateNewId();
                collection.InsertOne(a);

                Console.WriteLine($"Author {a.Name} {a.Surname} added successfully!");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

        }

        public void AddAuthors(List<Author> list)
        {
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Author>("author");
               
                list.ForEach(a =>a.Address.ObjectId = ObjectId.GenerateNewId());
                collection.InsertMany(list);


            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

        }

        public void AddAuthorsToPublication(Publication p)
        {
            throw new NotImplementedException();
        }

        public void Attach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public void Detach(IObserver observer)
        {
            throw new NotImplementedException();
        }

        public List<Author> GetAllAuthors()
        {
           
            List<Author> list = new List<Author>();
                try
                {
                    var client = new MongoClient(daoConfig.Url);
                    var database = client.GetDatabase(daoConfig.Database);
                    var collection = database.GetCollection<Author>("author");
                    var filter = Builders<Author>.Filter.Empty;
                    list = collection.Find(filter).ToList();


                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            return list;
            
        }

        public Author GetAuthor(object objectId)
        {
           if(objectId is ObjectId id)
           {

                try
                {
                    var client = new MongoClient(daoConfig.Url);
                    var database = client.GetDatabase(daoConfig.Database);
                    var collection = database.GetCollection<Author>("author");
                    var filter =  Builders<Author>.Filter.Eq("ObjectId",id);
                    Author a = collection.Find(f=>f.ObjectId == id).FirstOrDefault();

                    return a;

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
           }
           throw new Exception("Id value wrong type");
        }

        public Author GetAuthorByName(string name, string surname)
        {
            Author a = new AuthorBuilder().Build();
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Author>("author");
                var filter = Builders<Author>.Filter.And(
                    Builders<Author>.Filter.Eq("Name", name),
                    Builders<Author>.Filter.Eq("Surname", surname)
                    );
                 a = collection.Find(filter).FirstOrDefault();

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            
            return a;
            
            
        }

        public List<Author> GetAuthorByPublicationId(long publId)
        {
            throw new NotImplementedException();
        }

        public void Notify(string operation, object criteria, object result)
        {
            throw new NotImplementedException();
        }

        public void UpdateAuthor(Author a)
        {
            try
            {
                var client = new MongoClient(daoConfig.Url);
                var database = client.GetDatabase(daoConfig.Database);
                var collection = database.GetCollection<Author>("author");
                var filter = Builders<Author>.Filter.Eq("ObjectId", a.ObjectId);

                var updateDef = Builders<Author>.Update;
                var updateValuesList = new List<UpdateDefinition<Author>>();

                if (!System.String.IsNullOrEmpty(a.Email)) 
                    updateValuesList.Add(updateDef.Set("Email", a.Email));
                if (!String.IsNullOrEmpty(a.PhoneNumber)) 
                    updateValuesList.Add(updateDef.Set("PhoneNumber", a.PhoneNumber));
                if (!String.IsNullOrEmpty(a.Name)) 
                    updateValuesList.Add(updateDef.Set("Name", a.Name));
                if (!String.IsNullOrEmpty(a.Surname)) 
                    updateValuesList.Add(updateDef.Set("Surname", a.Surname));
                if (!String.IsNullOrEmpty(a.Address.Country)) 
                    updateValuesList.Add(updateDef.Set("Address.Country", a.Address.Country));
                if (!String.IsNullOrEmpty(a.Address.City)) 
                    updateValuesList.Add(updateDef.Set("Address.City", a.Address.City));
                if (!String.IsNullOrEmpty(a.Address.Street)) 
                    updateValuesList.Add(updateDef.Set("Address.Street", a.Address.Street));
                if (a.Address.House != 0) 
                    updateValuesList.Add(updateDef.Set("Address.House", a.Address.House));
                if (a.Address.Apartment.HasValue) 
                    updateValuesList.Add(updateDef.Set("Address.Apartment", a.Address.Apartment));

                var update = Builders<Author>.Update.Combine(updateValuesList);
                var result  = collection.UpdateOne(filter, update);
                if(result.ModifiedCount > 0)
                {
                    Console.WriteLine("Author updated");
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

        }
    }
}
