using MySqlConnector;
using noslq_pr.Builder;
using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace noslq_pr.DAO.MYSQL
{
    public class MySQLAuthorDAO : LastIdGetter, IAuthorDAO
    {
        private readonly DAOConfig daoConfig;
       
        private const string insertAddress = "insert into address_book (address_id, country, city, street, house,apartment) values(@address_id, @country, @city, @street, @house,@appartment)";
        private const string insertPerson = "insert into person (id, name,surname,email, phone_number, address_book_address_id) values(@id, @name,@surname, @email,@phone_number,@address_book_address_id)";
        private const string insertAuthor = "insert into author (id, pseudonym) values(@id, @pseudonym)";

        //private const string getFull = "select a.id, a.name,a.surname, a.customer_id, a.email, a.phone_number, a.address_id,\r\nad.country, ad.city, ad.street,ad.house,ad.apartment\r\nfrom author a \r\njoin address_book ad on a.address_id = ad.address_id \r\nwhere a.id = @id;";
        private const string getFull = "SELECT p.id, p.name,p.surname, p.email, p.phone_number, c.pseudonym,\r\n       p.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment\r\nFROM author c\r\n  join  person p on c.id = p.id\r\n  join  address_book ad  ON p.address_book_address_id = ad.address_id\r\nWHERE p.id = @id;";
        private const string getFullByName = "SELECT p.id, p.name,p.surname, p.email, p.phone_number, c.pseudonym,\r\n       p.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment\r\nFROM author c\r\n  join  person p on c.id = p.id\r\n  join  address_book ad  ON p.address_book_address_id = ad.address_id\r\nWHERE p.name = @name and p.surname=@surname;";

        private const string insertAuthorsToPubl = "insert into publication_author(author_ID,publications_id) values(@author_ID, @publications_id);";
        private const string getAuthortByPubl = "select  p.id, p.name,p.surname, p.email, p.phone_number, a.pseudonym,\r\np.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment \r\nfrom publication_author pa\r\njoin author a on pa.author_ID  = a.id\r\njoin person p on p.id = a.id\r\njoin  address_book ad  ON p.address_book_address_id = ad.address_id\r\nwhere pa.publications_id = @id;";


        private List<IObserver> _observers = new List<IObserver> ();
        public MySQLAuthorDAO()
        {
            daoConfig = DAOConfig.GetDAOConfig();
            GetLastID = "SELECT MAX(id) FROM person";
        }

        public void AddAuthor(Author c,MySqlTransaction transaction,MySqlConnection con, StringBuilder result)
        {
            c.Address.Id = GetLastId(con, transaction, "select max(address_id) from address_book") + 1;
            using (var com = new MySqlCommand(insertAddress, con))
            {
                com.Transaction = transaction;
                com.Parameters.AddWithValue("@address_id", c.Address.Id);
                com.Parameters.AddWithValue("@country", c.Address.Country);
                com.Parameters.AddWithValue("@city", c.Address.City);
                com.Parameters.AddWithValue("@street", c.Address.Street);
                com.Parameters.AddWithValue("@house", c.Address.House);
                com.Parameters.AddWithValue("@appartment",
                    c.Address.Apartment.HasValue ? c.Address.Apartment : 0);

               int rowsAffected =  com.ExecuteNonQuery();
               result.Append($"Insert adress of author: {rowsAffected} row(s) inserted;\n");


            }


            c.Id = GetLastId(con, transaction) + 1;
            using (var com = new MySqlCommand(insertPerson, con))
            {
                com.Transaction = transaction;
                com.Parameters.AddWithValue("@name", c.Name);
                com.Parameters.AddWithValue("@surname", c.Surname);
                com.Parameters.AddWithValue("@id", c.Id);
                com.Parameters.AddWithValue("@email", c.Email);
                com.Parameters.AddWithValue("@phone_number", c.PhoneNumber);

                com.Parameters.AddWithValue("@address_book_address_id", c.Address.Id);

                int rowsAffected = com.ExecuteNonQuery();
                result.Append($"Insert person as author of publication: {rowsAffected} row(s) inserted;\n");


            }

            using (var com = new MySqlCommand(insertAuthor, con))
            {
                com.Transaction = transaction;

                com.Parameters.AddWithValue("@id", c.Id);
                com.Parameters.AddWithValue("@pseudonym", "ppseudonym");


                int rowsAffected = com.ExecuteNonQuery();
                result.Append($"Insert author of publication: {rowsAffected} row(s) inserted;\n");

            }
           
        }


        public void AddAuthor(Author c)
        {
            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {
                       
                        AddAuthor(c,transaction,con,result);
                         
                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.StackTrace);
                        Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                            c, e.Message);
                    }

                    Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                            c,result.ToString());
                }
            }
        }

        public Author GetAuthor(int id)
        {
            AuthorBuilder p = new AuthorBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();


                using (var cmd = new MySqlCommand(getFull, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p = MapAuthor(reader);

                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    return p.Build();
                }
            }

        }

        public void AddAuthorsToPublication(Publication p)
        {
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {

                        using (var com = new MySqlCommand(insertAuthorsToPubl, con))
                        {
                            com.Transaction = transaction;
                            foreach (Author a in p.Authors)
                            {
                                com.Parameters.AddWithValue("author_ID", a.Id);
                                com.Parameters.AddWithValue("publications_id", p.Id);
                                com.ExecuteNonQuery();
                                com.Parameters.Clear(); 
                            }

                            


                        }
                       
                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        public Author GetAuthorByName(string name, string surname)
        {
            AuthorBuilder p = new AuthorBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();


                using (var cmd = new MySqlCommand(getFullByName, con))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@surname", surname);
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p = MapAuthor(reader);

                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    return p.Build();
                }
            }
        }

        public List<Author> GetAuthorByPublicationId(long publId)
        {
            List<Author> p = new List<Author>();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();


                using (var cmd = new MySqlCommand(getAuthortByPubl, con))
                {
                    cmd.Parameters.AddWithValue("@id", publId);
                    
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapAuthor(reader).Build());

                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    return p;
                }
            }
        }


        public AuthorBuilder MapAuthor(MySqlDataReader reader)
        {
            AuthorBuilder cb =new AuthorBuilder();
            cb.SetId(reader.GetInt64("id"));
            cb.SetName(reader.GetString("name"));
            cb.SetName(reader.GetString("name"));
            cb.SetSurname(reader.GetString("surname"));
            cb.SetEmail(reader.GetString("email"));
            cb.SetPhoneNumber(reader.GetString("phone_number"));
            cb.SetAddressId(reader.GetInt32("address_book_address_id"));
            cb.SetCountry(reader.GetString("country"));
            cb.SetCity(reader.GetString("city"));
            cb.SetStreet(reader.GetString("street"));
            cb.SetHouse(reader.GetInt32("house"));

            //
            var apartment = reader.GetInt32("apartment");
            cb.SetAppartment(reader.IsDBNull(reader.GetOrdinal("apartment")) ? 0 : apartment);
            return cb;
        }

        public void UpdateAuthor(Author a)
        {
            var updatedValuesPerson = new Dictionary<string, object>();

            if(!String.IsNullOrEmpty(a.Email)) updatedValuesPerson.Add("email", a.Email);
            if(!String.IsNullOrEmpty(a.PhoneNumber)) updatedValuesPerson.Add("phone_number", a.PhoneNumber);
            if(!String.IsNullOrEmpty(a.Name)) updatedValuesPerson.Add("name", a.Name);
            if(!String.IsNullOrEmpty(a.Surname)) updatedValuesPerson.Add("surname", a.Surname);


            List<MySqlParameter>? updatePersonParams = null;
            string updatePersonQuery = "";

            if (updatedValuesPerson.Count > 0)
            {
                updatePersonParams = QueryBilder.Update("person", updatedValuesPerson, "id=@id", out updatePersonQuery);
            }
            var updatedValuesAuthor = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(a.Pseudonym)) updatedValuesAuthor.Add("pseudonym", a.Pseudonym);

            List<MySqlParameter>? updateAuthorParams = null;
            string updateAuthorQuery = "";

            if (updatedValuesAuthor.Count > 0)
            {
                updateAuthorParams = QueryBilder.Update("author", updatedValuesAuthor, "id=@id", out updateAuthorQuery);
            }

            var updatedValuesAddress = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(a.Address.Country)) updatedValuesAddress.Add("country", a.Address.Country);
            if (!String.IsNullOrEmpty(a.Address.City)) updatedValuesAddress.Add("city", a.Address.City);
            if (!String.IsNullOrEmpty(a.Address.Street)) updatedValuesAddress.Add("street", a.Address.Street);
            if (a.Address.House!=0) updatedValuesAddress.Add("house", a.Address.House);
            if (a.Address.Apartment.HasValue) updatedValuesAddress.Add("apartment", a.Address.Apartment);
            
            
            List<MySqlParameter>? updateAddressParams = null;
            string updateAddressQuery = "";
            if (updatedValuesAddress.Count > 0)
            {
                updateAddressParams = QueryBilder.Update("address_book", updatedValuesAddress, "address_id=@id", out updateAddressQuery);
            }


            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {

                        if (!String.IsNullOrEmpty(updateAddressQuery) && updateAddressParams is not null)
                        {

                            using (var com = new MySqlCommand(updateAddressQuery, con))
                            {
                                com.Transaction= transaction;
                                com.Parameters.AddWithValue("@id", a.Address.Id);
                                com.Parameters.AddRange(updateAddressParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"Update Address: {rowsAffected} row(s) updated; ");

                            }
                        }



                        if (!String.IsNullOrEmpty(updatePersonQuery) && updatePersonParams is not null) { 

                            using (var com = new MySqlCommand(updatePersonQuery, con))
                            {
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updatePersonParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"Update Person: {rowsAffected} row(s) updated; ");

                            }
                        }

                        if (!String.IsNullOrEmpty(updateAuthorQuery) && updateAuthorParams is not null)
                        {

                            using (var com = new MySqlCommand(updateAuthorQuery, con))
                            {
                                com.Transaction = transaction;

                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updateAuthorParams.ToArray());

                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"Update Author: {rowsAffected} row(s) updated; ");

                            }
                        }

                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.Message);
                        Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                            a, e.Message);
                    }

                    Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                           a, result.ToString());
                }
            }


        }

        public void Attach(IObserver observer)
        {
            Console.WriteLine($"Attached observer {observer.GetType()} to MySQLAuthorDAO");
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Console.WriteLine($"detached observer {observer.GetType()} from MySQLAuthorDAO");
            _observers.Add(observer);
        }

        public void Notify(string operation, object criteria, object result)
        {
            Console.WriteLine($"Notified observers of MySQLAuthorDAO");
            foreach(var obs in _observers)
            {
                obs.Update(operation, criteria, result);
            }
        }
    }
}

