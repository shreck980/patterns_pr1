using Microsoft.Extensions.Configuration;
using MySqlConnector;
using noslq_pr.Builder;
using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace noslq_pr.DAO.MYSQL
{
    public class MySQLPrintingHouseDAO : LastIdGetter, IPrintingHouseDAO
    {
        private readonly DAOConfig daoConfig;
        private const string getFull = "SELECT ph.id, ph.name, ph.phone_number,\n    ph.address,ad.country, ad.city, ad.street,ad.house,ad.apartment\n    FROM printing_house ph \n    join  address_book ad  ON ph.address = ad.address_id\n    WHERE ph.id = @id;";
        private const string getAllFull = "SELECT ph.id, ph.name, ph.phone_number,\n    ph.address,ad.country, ad.city, ad.street,ad.house,ad.apartment\n    FROM printing_house ph \n    join  address_book ad  ON ph.address = ad.address_id;";
        private const string getFullByName = "SELECT ph.id, ph.name, ph.phone_number,\n    ph.address,ad.country, ad.city, ad.street,ad.house,ad.apartment\n    FROM printing_house ph \n    join  address_book ad  ON ph.address = ad.address_id\n    WHERE ph.name = @name;";
        private const string getFullByCountry = "SELECT ph.id, ph.name, ph.phone_number,\n    ph.address,ad.country, ad.city, ad.street,ad.house,ad.apartment\n    FROM  address_book ad\n    join  printing_house ph  ON ph.address = ad.address_id\n    WHERE ad.country = @country;\n    ";

        private const string insertAddress = "insert into address_book (address_id, country, city, street, house,apartment) values(@address_id, @country, @city, @street, @house,@appartment)";

        private const string insertPrintingHouse = "insert into printing_house (id,name,phone_number,address) values(@id, @name,@phone_number, @address)";
        public MySQLPrintingHouseDAO()
        {
            daoConfig = DAOConfig.GetDAOConfig();
            GetLastID = "select max(id) from printing_house; ";
        }
        public void AddPrintingHouse(PrintingHouse c)
        {

            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
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

                            com.ExecuteNonQuery();


                        }

                        c.Id = GetLastId(con, transaction) + 1;
                        using (var com = new MySqlCommand(insertPrintingHouse, con))
                        {
                            com.Transaction = transaction;
                            com.Parameters.AddWithValue("@name", c.Name);
                            com.Parameters.AddWithValue("@id", c.Id);
                            com.Parameters.AddWithValue("@phone_number", c.PhoneNumber);
                            com.Parameters.AddWithValue("@address", c.Address.Id);

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

        public PrintingHouse GetPrintingHouse(int id)
        {
            PrintingHouseBuilder p = new PrintingHouseBuilder();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();


                using (var cmd = new MySqlCommand(getFull, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    
                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p = MapPrintingHouse(reader);

                            }

                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return p.Build();
        }


        PrintingHouseBuilder MapPrintingHouse(MySqlDataReader reader)
        {
            PrintingHouseBuilder cb = new PrintingHouseBuilder();
            cb.SetId(reader.GetInt64("id"));
            cb.SetName(reader.GetString("name"));
            cb.SetPhoneNumber(reader.GetString("phone_number"));
            cb.SetAddressId(reader.GetInt32("address"));
            cb.SetCountry(reader.GetString("country"));
            cb.SetCity(reader.GetString("city"));
            cb.SetStreet(reader.GetString("street"));
            cb.SetHouse(reader.GetInt32("house"));
            var apartment = reader.GetInt32("apartment");
            cb.SetAppartment(reader.IsDBNull(reader.GetOrdinal("apartment")) ? 0 : apartment);
            return cb;
          
        }

        public PrintingHouse GetPrintingHouseByName(string name)
        {
            PrintingHouseBuilder p = new PrintingHouseBuilder();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getFullByName, con))
                    {
                        cmd.Parameters.AddWithValue("@name", name);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p = MapPrintingHouse(reader);

                            }

                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return p.Build();
        }

        public List<PrintingHouse> GetPrintingHouseByCountry(string country)
        {
            List<PrintingHouse> p = new List<PrintingHouse>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getFullByCountry, con))
                    {
                        cmd.Parameters.AddWithValue("@country", country);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add( MapPrintingHouse(reader).Build());

                            }

                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return p;
        }

        public void UpdatePrintingHouse(PrintingHouse a)
        {
            var updatedValuesPrintingHouse = new Dictionary<string, object>();

            if (!String.IsNullOrEmpty(a.Name)) updatedValuesPrintingHouse.Add("name", a.Name);
            if (!String.IsNullOrEmpty(a.PhoneNumber)) updatedValuesPrintingHouse.Add("phone_number", a.PhoneNumber);
           

            List<MySqlParameter>? updatePrintingHouseParams = null;
            string updatePrintingHouseQuery = "";

            if (updatedValuesPrintingHouse.Count > 0)
            {
                updatePrintingHouseParams = QueryBilder.Update("printing_house", updatedValuesPrintingHouse, "id=@id", out updatePrintingHouseQuery);
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



                        if (!String.IsNullOrEmpty(updatePrintingHouseQuery) && updatePrintingHouseParams is not null)
                        {

                            using (var com = new MySqlCommand(updatePrintingHouseQuery, con))
                            {
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updatePrintingHouseParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                Console.WriteLine($"{rowsAffected} row(s) updated.");

                            }
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

        public List<PrintingHouse> GetAllPrintingHouse()
        {
            List<PrintingHouse> p = new List<PrintingHouse>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getAllFull, con))
                    {
                       
                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapPrintingHouse(reader).Build());

                            }

                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return p;
        }
    }
    
}
