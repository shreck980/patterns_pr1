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
        private const string getLittleDate = "SELECT * FROM `order` WHERE DATE(acceptance_date) = @date;";
        private const string getLittleCustomer = "SELECT * FROM `order` WHERE customer = @customer;";
        private const string getLittleStatus = "SELECT * FROM `order` WHERE status = @status;";

        private readonly MYSQLPublicationDAO publicationDAO;

        private List<IObserver> _observers = new List<IObserver>();
        public MySQLOrderDAO()
        {

            this.daoConfig = DAOConfig.GetDAOConfig();
            GetLastID = "select max(id) from `order`;";

            DAOFactory factory = DAOFactory.Instance;
            publicationDAO = (MYSQLPublicationDAO)factory.GetPublicationDAO();

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
                        Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                           o, e.Message);
                    }
                    Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                           o, result.ToString());
                }
            }
        }

        public Order GetOrder(long id)
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

        public List<Order> GetOrderbyCustomer(long custoemrId)
        {

            List<Order> p = new List<Order>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getLittleCustomer, con))
                    {
                        cmd.Parameters.AddWithValue("@customer", custoemrId);

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
            p.SetPrintingHouse(new PrintingHouseBuilder().SetId(reader.GetInt32("printing_house")).Build());
            p.SetCustomer(new CustomerBuilder().SetId(reader.GetInt32("customer")).Build());
            p.SetPrice(reader.GetDecimal("total_price"));
            return p;

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
    }

}
