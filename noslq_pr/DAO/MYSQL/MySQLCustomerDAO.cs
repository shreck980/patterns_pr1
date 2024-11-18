using MySqlConnector;
using noslq_pr.Builder;
using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IObserver = noslq_pr.Observer.IObserver;


namespace noslq_pr.DAO.MYSQL
{
    public class MySQLCustomerDAO : LastIdGetter, ICustomerDAO
    {
        private readonly DAOConfig daoConfig;
        private const string insertPerson = "insert into person (id, name,surname,email, phone_number, address_book_address_id) values(@id, @name,@surname, @email,@phone_number,@address_book_address_id)";
        private const string insertCustomer = "insert into customer (id, customer_type_id) values(@id,@customer_type_id)";
        private const string insertAddress = "insert into address_book (address_id, country, city, street, house,apartment) values(@address_id, @country, @city, @street, @house,@appartment)";
        
        private const string getFull = "SELECT p.id, p.name,p.surname, p.email, p.phone_number, c.customer_type_id,\r\n       p.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment\r\nFROM customer c\r\n  join  person p on c.id = p.id\r\n  join  address_book ad  ON p.address_book_address_id = ad.address_id\r\nWHERE c.id = @id;";
        private const string getFullByName = "SELECT p.id, p.name,p.surname, p.email, p.phone_number, c.customer_type_id,\r\n       p.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment\r\nFROM customer c\r\n  join  person p on c.id = p.id\r\n  join  address_book ad  ON p.address_book_address_id = ad.address_id\r\nWHERE p.name = @name and p.surname=@surname;";
        private const string getFullByCountry = "select  p.id, p.name,p.surname, p.email, p.phone_number, c.customer_type_id,\r\np.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment \r\nfrom customer c\r\njoin person p on p.id = c.id\r\njoin  address_book ad  ON p.address_book_address_id = ad.address_id\r\nwhere ad.country = @country;";



        private List<IObserver> _observers = new List<IObserver>();
        public MySQLCustomerDAO()
        {
            daoConfig = DAOConfig.GetDAOConfig();
            GetLastID = "SELECT MAX(id) FROM person";
        }
        public void AddCustomer(Customer c)
        {
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    
                    try
                    {
                        c.Address.Id = GetLastId(con, transaction,"select max(address_id) from address_book") +1;
                        using (var com = new MySqlCommand(insertAddress, con))
                        {
                            com.Transaction = transaction;
                            com.Parameters.AddWithValue("@address_id", c.Address.Id);
                            com.Parameters.AddWithValue("@country", c.Address.Country);
                            com.Parameters.AddWithValue("@city", c.Address.City);
                            com.Parameters.AddWithValue("@street", c.Address.Street);
                            com.Parameters.AddWithValue("@house", c.Address.House);
                            com.Parameters.AddWithValue("@appartment", 
                                c.Address.Apartment.HasValue ? c.Address.Apartment: 0 );

                            com.ExecuteNonQuery();

                            
                        }

                        c.Id = GetLastId(con,transaction) +1;
                        using (var com = new MySqlCommand(insertPerson, con))
                        {
                            com.Transaction = transaction;
                            com.Parameters.AddWithValue("@name", c.Name);
                            com.Parameters.AddWithValue("@surname", c.Surname);
                            com.Parameters.AddWithValue("@id", c.Id);
                            com.Parameters.AddWithValue("@email", c.Email);
                            com.Parameters.AddWithValue("@phone_number", c.PhoneNumber);
                            
                            com.Parameters.AddWithValue("@address_book_address_id", c.Address.Id);

                            com.ExecuteNonQuery();

                        }

                        using (var com = new MySqlCommand(insertCustomer, con))
                        {
                            com.Transaction = transaction;
                           
                            com.Parameters.AddWithValue("@id", c.Id);
                            com.Parameters.AddWithValue("@customer_type_id", c.CustomerType);
                            

                            com.ExecuteNonQuery();

                        }
                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.Message);
                    }
                }
            }
        }

        public Customer GetCustomer(int id)
        {
            CustomerBuilder cb = new CustomerBuilder();
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
                               cb = MapCustomer(reader);
                                
                                  
                            }

                        }
                    }
                    catch (MySqlException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return cb.Build();
                }
            }

        }

        public Customer GetCustomerByName(string name, string surname)
        {
            CustomerBuilder p = new CustomerBuilder();
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
                                p = MapCustomer(reader);

                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        Notify(
                            System.Reflection.MethodBase.GetCurrentMethod().Name,
                            $"customer name = {name} {surname}",
                            p.Build()
                            );
                    }
                    return p.Build();
                }
            }
        }

        public List<Customer> GetCustomerByCountry(string country)
        {
            List<Customer> p = new List<Customer>();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();


                using (var cmd = new MySqlCommand(getFullByCountry, con))
                {
                    cmd.Parameters.AddWithValue("@country", country);
                   
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
                                p.Add( MapCustomer(reader).Build());

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

        CustomerBuilder MapCustomer(MySqlDataReader reader)
        {
            CustomerBuilder cb = new CustomerBuilder();
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

        public void UpdateCustomer(Customer a)
        {
            var updatedValuesPerson = new Dictionary<string, object>();

            if (!String.IsNullOrEmpty(a.Email)) updatedValuesPerson.Add("email", a.Email);
            if (!String.IsNullOrEmpty(a.PhoneNumber)) updatedValuesPerson.Add("phone_number", a.PhoneNumber);
            if (!String.IsNullOrEmpty(a.Name)) updatedValuesPerson.Add("name", a.Name);
            if (!String.IsNullOrEmpty(a.Surname)) updatedValuesPerson.Add("surname", a.Surname);


            List<MySqlParameter>? updatePersonParams = null;
            string updatePersonQuery = "";

            if (updatedValuesPerson.Count > 0)
            {
                updatePersonParams = QueryBilder.Update("person", updatedValuesPerson, "id=@id", out updatePersonQuery);
            }
            var updatedValuesCustomer = new Dictionary<string, object>();
            if(a.CustomerType!=CustomerType.Other) updatedValuesCustomer.Add("customer_type_id", a.CustomerType);

            List<MySqlParameter>? updateCustomerParams = null;
            string updateCustomerQuery = "";

            if (updatedValuesCustomer.Count > 0)
            {
                updateCustomerParams = QueryBilder.Update("customer", updatedValuesCustomer, "id=@id", out updateCustomerQuery);
            }

            var updatedValuesAddress = new Dictionary<string, object>();
            if (!String.IsNullOrEmpty(a.Address.Country)) updatedValuesAddress.Add("country", a.Address.Country);
            if (!String.IsNullOrEmpty(a.Address.City)) updatedValuesAddress.Add("city", a.Address.City);
            if (!String.IsNullOrEmpty(a.Address.Street)) updatedValuesAddress.Add("street", a.Address.Street);
            if (a.Address.House != 0) updatedValuesAddress.Add("house", a.Address.House);
            if (a.Address.Apartment.HasValue) updatedValuesAddress.Add("apartment", a.Address.Apartment);


            List<MySqlParameter>? updateAddressParams = null;
            string updateAddressQuery = "";
            if (updatedValuesAddress.Count > 0)
            {
                updateAddressParams = QueryBilder.Update("address_book", updatedValuesAddress, "address_id=@id", out updateAddressQuery);
            }



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
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@id", a.Address.Id);
                                com.Parameters.AddRange(updateAddressParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                Console.WriteLine($"{rowsAffected} row(s) updated.");

                            }
                        }



                        if (!String.IsNullOrEmpty(updatePersonQuery) && updatePersonParams is not null)
                        {

                            using (var com = new MySqlCommand(updatePersonQuery, con))
                            {
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updatePersonParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                Console.WriteLine($"{rowsAffected} row(s) updated.");

                            }
                        }

                        if (!String.IsNullOrEmpty(updateAddressQuery) && updateCustomerParams is not null)
                        {

                            using (var com = new MySqlCommand(updateCustomerQuery, con))
                            {
                                com.Transaction = transaction;

                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updateCustomerParams.ToArray());

                                int rowsAffected = com.ExecuteNonQuery();
                                Console.WriteLine($"{rowsAffected} row(s) updated.");

                            }
                        }

                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.WriteLine(e.Message);
                    }
                }
            }


        }

        public void Attach(IObserver observer)
        {
            Console.WriteLine("Observer attached to MySQLCustomerDAO");
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Console.WriteLine("Observer detached to MySQLCustomerDAO");
            _observers.Remove(observer);
        }

        public void Notify(string operation, object criteria, object result)
        {
            Console.WriteLine("Observer attached to MySQLCustomerDAO");
            foreach(var observer in _observers) {
                observer.Update(operation,criteria,result);
            }
        }
    }
}
