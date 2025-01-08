using MongoDB.Bson;
using MySqlConnector;
using noslq_pr.Builder;
using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace noslq_pr.DAO.MYSQL
{
    public class MySQLOrderDAO : LastIdGetter, IOrderDAO
    {

        private DAOConfig daoConfig;
        private const string insertOrder = "insert into `order` (id,acceptance_date,status,printing_house,customer,total_price) values(@id,@acceptance_date,@status,@printing_house,@customer,@total_price)";

        //private const string insertPubl = "INSERT INTO publication (id,title, page_count, circulation, genre_id, price) VALUE (@id, @title, @pageCount, @circulation, @genreId, @price);";
        //private const string getFull = "SELECT Id, title, page_count, circulation, genre_id, price FROM publication WHERE id = @id";
        //private const string findAuthor = "select p.id from author a join person p on p.id = a.id where p.name = @name and p.surname=@surname;";
        //private const string insertAuthor = "insert into author (id, name,surname,email, phone_number, customer_id, address_id) values(@id, @name,@surname,@email,@phone_number,@customer_id, @address_id)";
        //private const string insertPerson = "insert into person (id, name,surname,email, phone_number, address_book_address_id) values(@id, @name,@surname, @email,@phone_number,@address_book_address_id)";
        //private const string insertAuthor = "insert into author (id, pseudonym) values(@id, @pseudonym)";


        //const string insertAuthorsToPubl = "insert into publication_author(author_ID,publications_id) values(@author_ID, @publications_id);";
        
        
        const string deletePublToOrder = "DELETE FROM order_publication WHERE `order` = @id;";
        const string insertPublToOrder = "insert into order_publication (`order`,punlication,print_quality,quantity) values(@order,@punlication,@print_quality,@quantity);";
        //private const string insertAddress = "insert into address_book (address_id, country, city, street, house,apartment) values(@address_id, @country, @city, @street, @house,@appartment)";

        private const string getLittle = "select * from `order` where id=@id;";
        private const string getAllOrder = "SELECT \r\n    o.id AS order_id,\r\n    o.acceptance_date AS order_date,\r\n    o.status AS order_status,\r\n    o.total_price AS order_total_price,\r\n    o.customer AS order_customer,\r\n    o.printing_house AS order_printing_house,\r\n    p.name AS name,\r\n    p.surname AS surname,\r\n    p.email AS person_email,\r\n    p.phone_number AS person_phone,\r\n    c.customer_type_id AS customer_type,\r\n    ph.id AS printing_house_id,\r\n    ph.name AS printing_house_name,\r\n    ph.phone_number AS printing_house_contact_phone,\r\n     pa.country AS person_country,\r\n\tpa.address_id as person_address_id,\r\n    pa.city AS person_city,\r\n    pa.street AS person_street,\r\n    pa.house AS person_house,\r\n    pa.apartment AS person_apartment,\r\n    pha.address_id as pha_address_id,\r\n\tpha.country AS printing_house_country,\r\n    pha.city AS printing_house_city,\r\n    pha.street AS printing_house_street,\r\n    pha.house AS printing_house_house,\r\n    pha.apartment AS printing_house_apartment\r\nFROM `order` o\r\nJOIN person p ON p.id = o.customer\r\nJOIN customer c ON c.id = p.id\r\nJOIN printing_house ph ON ph.id = o.printing_house\r\nJOIN address_book pa ON pa.address_id = p.address_book_address_id\r\nJOIN address_book pha ON pha.address_id = ph.address;";
        private const string getLittleDate = "SELECT * FROM `order` WHERE DATE(acceptance_date) = @date;";
        private const string getLittleCustomer = "SELECT * FROM `order` o\r\njoin person p on o.customer =p.id\r\nWHERE p.email = @email;";
        private const string getLittleStatus = "SELECT * FROM `order` WHERE status = @status;";

        private readonly MYSQLPublicationDAO publicationDAO;
        private readonly MySQLCustomerDAO customerDAO;
        private readonly MySQLPrintingHouseDAO printingHouseDAO;

        private List<IObserver> _observers = new List<IObserver>();
        public MySQLOrderDAO()
        {

            this.daoConfig = DAOConfig.GetDAOConfig();
            GetLastID = "select max(id) from `order`;";

            DAOFactory factory = DAOFactory.Instance;
            customerDAO = (MySQLCustomerDAO)factory.GetCustomerDAO();
            publicationDAO = (MYSQLPublicationDAO)factory.GetPublicationDAO();
            printingHouseDAO = (MySQLPrintingHouseDAO)factory.GetPrintingHouseDAO();

        }

        public void AddOrder(Order o)
        {
            if (publicationDAO == null)
            {
                throw new Exception("publication dao is null");
            }

            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {



                    try
                    {
                        o.Id = GetLastId(con, transaction) + 1;
                        using (var c = new MySqlCommand(insertOrder, con))
                        {
                            c.Transaction = transaction;
                            c.Parameters.AddWithValue("@id", o.Id);
                            c.Parameters.AddWithValue("@acceptance_date", o.AcceptanceDate);
                            c.Parameters.AddWithValue("@status", o.Status);
                            c.Parameters.AddWithValue("@customer", o.Customer.Id);
                            c.Parameters.AddWithValue("@total_price", o.Price);
                            c.Parameters.AddWithValue("@printing_house", o.PrintingHouse.Id);

                            int rowsAffected = c.ExecuteNonQuery();
                            result.Append($"Insert Order: {rowsAffected} row(s) inserted;\n");

                        }

                        foreach (var p in o.Publications)
                        {
                            publicationDAO.AddPublication(p,transaction,con,result);


                            using (var com = new MySqlCommand(insertPublToOrder, con))
                            {
                                com.Transaction = transaction;

                                com.Parameters.AddWithValue("@order", o.Id);
                                com.Parameters.AddWithValue("@punlication", p.Id);
                                com.Parameters.AddWithValue("@print_quality", p.PrintQuality);
                                com.Parameters.AddWithValue("@quantity", p.Quantity);
                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"Add publication to order: {rowsAffected} row(s) inserted;\n");
                               
                            }

                        }



                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.Message);
                        //Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                          // o, e.Message);
                    }
                    //Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                         //  o, result.ToString());
                }
            }
        }

        public Order GetOrder(object objectId)
        {
            if (objectId is long id)
            {
                OrderBuilder p = new OrderBuilder();
                try
                {
                    using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                    {
                        con.Open();


                        using (var cmd = new MySqlCommand(getLittle, con))
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
                                    p = MapOrder(reader);


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
            throw new Exception("Wrong id type.");
        }


        public List<Order> GetOrderbyDate(DateTime date)
        {

            List<Order> p = new List<Order>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getLittleDate, con))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapOrder(reader).Build());


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

        public List<Order> GetOrderbyCustomer(string email)
        {
           

                List<Order> p = new List<Order>();
                try
                {
                    using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                    {
                        con.Open();


                        using (var cmd = new MySqlCommand(getLittleCustomer, con))
                        {
                            cmd.Parameters.AddWithValue("@email", email);

                            using (var reader = cmd.ExecuteReader())
                            {

                                if (!reader.HasRows)
                                {
                                    throw new Exception("No data found for the query.");

                                }
                                while (reader.Read())
                                {
                                    p.Add(MapOrder(reader).Build());


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

        public List<Order> GetOrderbyStatus(OrderStatus status)
        {
            List<Order> p = new List<Order>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getLittleStatus, con))
                    {
                        cmd.Parameters.AddWithValue("@status", status);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapOrder(reader).Build());


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


        OrderBuilder MapOrder(MySqlDataReader reader)
        {
            OrderBuilder p = new OrderBuilder();
            p.SetId(reader.GetInt64("id"));
            p.SetAcceptanceDate( reader.GetDateTime("acceptance_date"));
            p.SetOrderStatus((OrderStatus)reader.GetInt32("status"));
            p.SetPrintingHouse(new PrintingHouseBuilder().SetId(reader.GetInt32("printing_house")).SetName(reader.GetString("name")).Build());
            p.SetCustomer(new CustomerBuilder().SetId(reader.GetInt32("customer")).SetEmail(reader.GetString("email")).Build());
            p.SetPrice(reader.GetDecimal("total_price"));
            return p;

        }
        OrderBuilder MapFullOrder(MySqlDataReader reader)
        {
            OrderBuilder order = new OrderBuilder();


            order.SetId(reader.GetInt64("order_id"));
            order.SetAcceptanceDate(reader.GetDateTime("order_date"));
            order.SetOrderStatus((OrderStatus)reader.GetInt32("order_status"));
            order.SetPrice(reader.GetDecimal("order_total_price"));
           

            // Set customer details
            var customerBuilder = new CustomerBuilder()
                .SetId(reader.GetInt32("order_customer"))
                .SetEmail(reader.GetString("person_email"))
                .SetCustomerType((CustomerType)reader.GetInt32("customer_type"))
                .SetPhoneNumber(reader.GetString("person_phone"))
                .SetName(reader.GetString("name"))
                .SetSurname(reader.GetString("surname"))
                    .SetAddressId(reader.GetInt32("person_address_id"))
                    .SetCountry(reader.GetString("person_country"))
                    .SetCity(reader.GetString("person_city"))
                    .SetStreet(reader.GetString("person_street"))
                    .SetHouse(reader.GetInt32("person_house"));
                    
            var apartment = reader.GetInt32("person_apartment");
            customerBuilder.SetAppartment(reader.IsDBNull(reader.GetOrdinal("person_apartment")) ? 0 : apartment);

            order.SetCustomer(customerBuilder.Build());

            // Set printing house details
            var printingHouseBuilder = new PrintingHouseBuilder()
                .SetId(reader.GetInt32("printing_house_id"))
                .SetName(reader.GetString("printing_house_name"))
                .SetPhoneNumber(reader.GetString("printing_house_contact_phone"))

                    .SetAddressId(reader.GetInt32("pha_address_id"))
                    .SetCountry(reader.GetString("printing_house_country"))
                    .SetCity(reader.GetString("printing_house_city"))
                    .SetStreet(reader.GetString("printing_house_street"))
                    .SetHouse(reader.GetInt32("printing_house_house"));
            apartment = reader.GetInt32("printing_house_apartment");
            printingHouseBuilder.SetAppartment(reader.IsDBNull(reader.GetOrdinal("printing_house_apartment")) ? 0 : apartment);

            order.SetPrintingHouse(printingHouseBuilder.Build());

            return order;
        }

        public void UpdateOrder(Order a)
        {
            var updatedValuesOrder = new Dictionary<string, object>();
            StringBuilder result = new StringBuilder();

            if (a.Status != OrderStatus.Other) updatedValuesOrder.Add("status", a.Status);
            if (a.PrintingHouse.Id != 0) updatedValuesOrder.Add("printing_house", a.PrintingHouse.Id);
            if (a.Price != decimal.Zero) updatedValuesOrder.Add("total_price", a.Price);


            List<MySqlParameter>? updateOrderParams = null;
            string updateOrderQuery = "";

            if (updatedValuesOrder.Count > 0)
            {
                updateOrderParams = QueryBilder.Update("`order`", updatedValuesOrder, "id=@id", out updateOrderQuery);
            }



            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {

                        if (!System.String.IsNullOrEmpty(updateOrderQuery) && updateOrderParams is not null)
                        {

                            using (var com = new MySqlCommand(updateOrderQuery, con))
                            {
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updateOrderParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"{rowsAffected} row(s) inserted;\n");

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
                          a, result);
                }
            }
        }

        public void UpdateOrderPublication(Order o)
        {
            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {


                        using (var com = new MySqlCommand(deletePublToOrder, con))
                        {
                            com.Transaction= transaction;
                            com.Parameters.AddWithValue("@id", o.Id);
                            int rows =  com.ExecuteNonQuery();
                            result.Append($"{rows} row(s) deleted;\n");
                        }


                        foreach (var p in o.Publications)
                        {
                            publicationDAO.AddPublication(p, transaction, con, result);


                            using (var com = new MySqlCommand(insertPublToOrder, con))
                            {
                                com.Transaction = transaction;

                                com.Parameters.AddWithValue("@order", o.Id);
                                com.Parameters.AddWithValue("@punlication", p.Id);
                                com.Parameters.AddWithValue("@print_quality", p.PrintQuality);
                                com.Parameters.AddWithValue("@quantity", p.Quantity);
                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"Add  publication to order: {rowsAffected} row(s) inserted;\n");

                            }

                        }

                            transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.Message);
                        Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                          o, e.Message);
                    }
                    Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                          o, result.ToString());
                }
            }
        }

        public void Attach(IObserver observer)
        {
            Console.WriteLine($"Attached observer {observer.GetType()} to MySQLPrintingHouseDAO");
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Console.WriteLine($"Detached observer {observer.GetType()} to MySQLPrintingHouseDAO");
            _observers.Remove(observer);
        }

        public void Notify(string operation, object criteria, object result)
        {
            Console.WriteLine($"Notified observers of MySQLOrderDAO");
            foreach (var o in _observers)
            {
                o.Update(operation, criteria, result);
            }
        }

        public List<Order> GetAllOrder()
        {

            List<Order> p = new List<Order>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getAllOrder, con))
                    {
                       

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                // TODO: тут помилка у мапі
                                p.Add(MapFullOrder(reader).Build());


                            }

                        }
                    }

                    foreach (var o in p)
                    {
                        o.Publications =  publicationDAO.GetPublicationByOrderId(con,o.Id);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return p;
        }

        public void AddOrders(List<Order> list)
        {
            if (publicationDAO == null)
            {
                throw new Exception("publication dao is null");
            }

            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {
                        foreach (var o in list)
                        {
                            printingHouseDAO.AddPrintingHouseOrder(o.PrintingHouse,con,transaction);
                            customerDAO.AddCustomerOrder(o.Customer,con, transaction);
                          

                            o.Id = GetLastId(con, transaction) + 1;
                            using (var c = new MySqlCommand(insertOrder, con))
                            {
                                c.Transaction = transaction;
                                c.Parameters.AddWithValue("@id", o.Id);
                                c.Parameters.AddWithValue("@acceptance_date", o.AcceptanceDate);
                                c.Parameters.AddWithValue("@status", o.Status);
                                c.Parameters.AddWithValue("@customer", o.Customer.Id);
                                c.Parameters.AddWithValue("@total_price", o.Price);
                                c.Parameters.AddWithValue("@printing_house", o.PrintingHouse.Id);

                                int rowsAffected = c.ExecuteNonQuery();
                                result.Append($"Insert Order: {rowsAffected} row(s) inserted;\n");

                            }

                            foreach (var p in o.Publications)
                            {
                                publicationDAO.AddPublication(p, transaction, con, result);


                                using (var com = new MySqlCommand(insertPublToOrder, con))
                                {
                                    com.Transaction = transaction;

                                    com.Parameters.AddWithValue("@order", o.Id);
                                    com.Parameters.AddWithValue("@punlication", p.Id);
                                    com.Parameters.AddWithValue("@print_quality", PrintQuality.High);
                                    com.Parameters.AddWithValue("@quantity", p.Quantity);
                                    int rowsAffected = com.ExecuteNonQuery();
                                    result.Append($"Add publication to order: {rowsAffected} row(s) inserted;\n");

                                }

                            }
                           
                        }
                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.Message);
                        //Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        // o, e.Message);
                    }
                    //Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    //  o, result.ToString());
                }
            }
        }
    }

}
