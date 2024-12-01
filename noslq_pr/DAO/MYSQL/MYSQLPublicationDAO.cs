using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;
using noslq_pr.Builder;
using noslq_pr.Observer;

namespace noslq_pr.DAO.MYSQL
{
    public class MYSQLPublicationDAO : LastIdGetter, IPublicationDAO
    {
        private readonly DAOConfig daoConfig;
        private const string insert = "INSERT INTO publication (id,title, page_count, circulation, genre_id, price) VALUE (@id, @title, @pageCount, @circulation, @genreId, @price);";

        private const string getFull = "SELECT Id, title, page_count, circulation, genre_id, price FROM publication WHERE id = @id";
        private const string getFullByTitle = "SELECT Id, title, page_count, circulation, genre_id, price FROM publication WHERE title = @title";
        private const string getPublByAuthor = "select  p.Id, p.title, p.page_count, p.circulation, p.genre_id, p.price\r\nfrom publication_author pa\r\njoin publication p on pa.publications_id  = p.id\r\nwhere pa.author_id = @id;";
        private const string getPublByOrder = "select  p.Id, p.title, p.page_count, p.circulation, p.genre_id, p.price\r\nfrom order_publication op\r\njoin publication p on op.punlication  = p.id\r\nwhere op.order = 1;";
        private const string getPublByCustomer = "select  p.Id, p.title, p.page_count, p.circulation, p.genre_id, p.price\r\nfrom `order` o\r\njoin order_publication op  on o.id = op.order\r\njoin publication p on op.punlication  = p.id\r\nwhere o.customer = @id;";
        private const string getAuthors = "select  p.id, p.name,p.surname, p.email, p.phone_number, a.pseudonym,\r\np.address_book_address_id,ad.country, ad.city, ad.street,ad.house,ad.apartment \r\nfrom publication_author pa\r\njoin author a on pa.author_ID  = a.id\r\njoin person p on p.id = a.id\r\njoin  address_book ad  ON p.address_book_address_id = ad.address_id\r\nwhere pa.publications_id = @id;";
        private const string findAuthor = "select p.id from author a join person p on p.id = a.id where p.name = @name and p.surname=@surname;";
        //private const string insertAuthor = "insert into author (id, name,surname,email, phone_number, customer_id, address_id) values(@id, @name,@surname,@email,@phone_number,@customer_id, @address_id)";
        //private const string insertPerson = "insert into person (id, name,surname,email, phone_number, address_book_address_id) values(@id, @name,@surname, @email,@phone_number,@address_book_address_id)";
        //private const string insertAuthor = "insert into author (id, pseudonym) values(@id, @pseudonym)";

        private const string insertAuthorsToPubl = "insert into publication_author(author_ID,publications_id) values(@author_ID, @publications_id);";
       
       //private const string insertAddress = "insert into address_book (address_id, country, city, street, house,apartment) values(@address_id, @country, @city, @street, @house,@appartment)";
        private readonly MySQLAuthorDAO authorDAO;


        private List<IObserver> _observers = new List<IObserver>();

        public MYSQLPublicationDAO()
        {
            daoConfig = DAOConfig.GetDAOConfig();
            GetLastID = "SELECT MAX(id) FROM publication";
            DAOFactory factory = DAOFactory.Instance;
            authorDAO = (MySQLAuthorDAO)factory.GetAuthorDAO();
        }



        public void AddPublication(Publication p, MySqlTransaction transaction, MySqlConnection con,StringBuilder result)
        {
            
            p.Id = GetLastId(con, transaction) + 1;
            using (var c = new MySqlCommand(insert, con))
            {
                c.Transaction = transaction;
                c.Parameters.AddWithValue("@id", p.Id);
                c.Parameters.AddWithValue("@title", p.Title);
                c.Parameters.AddWithValue("@pageCount", p.PageCount);
                c.Parameters.AddWithValue("@circulation", p.Circulation);
                c.Parameters.AddWithValue("@genreId", p.Genre);
                c.Parameters.AddWithValue("@price", p.Price);

                int rowsAffected = c.ExecuteNonQuery();
                result.Append($"Add publication: {rowsAffected} row(s) inserted;\n");

            }

            foreach (Author a in p.Authors)
            {
                using (var c = new MySqlCommand(findAuthor, con))
                {
                    c.Transaction = transaction;
                    c.Parameters.AddWithValue("@name", a.Name);
                    c.Parameters.AddWithValue("@surname", a.Surname);

                    using (var reader = c.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read()) 
                            {
                                a.Id = reader.GetInt32("id");
                                Console.WriteLine(a);
                            }
                        }


                    }

                }

                if (a.Id == 0)
                {
                    authorDAO.AddAuthor(a, transaction, con, result);
                
                }

            }


            using (var com = new MySqlCommand(insertAuthorsToPubl, con))
            {
                com.Transaction = transaction;
                foreach (Author a in p.Authors)
                {
                    com.Parameters.AddWithValue("author_ID", a.Id);
                    com.Parameters.AddWithValue("publications_id", p.Id);
                    int rowsAffected = com.ExecuteNonQuery();
                    result.Append($"Added authors to publication: {rowsAffected} row(s) inserted;\n");
                    com.Parameters.Clear();
                }
            }
        }


        public void AddPublication(Publication p)
        {

            if (authorDAO == null)
            {
                throw new Exception("Author dao is null");
            }
            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        AddPublication(p, transaction, con,result);

                        transaction.Commit();
                    }
                    catch (MySqlException e)
                    {
                        transaction.Rollback();
                        Console.Error.WriteLine(e.Message);
                        Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                            p, e.Message);
                    }
                    Notify(System.Reflection.MethodBase.GetCurrentMethod().Name,
                            p, result.ToString());
                }
            }
        }




        public Publication GetPublication(int id)
        {
            if (authorDAO == null)
            {
                throw new Exception("Author dao is null");
            }
            PublicationBuilder p = new PublicationBuilder();
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
                                p = MapPublication(reader);
                            }

                        }
                    }

                    using (var cmd = new MySqlCommand(getAuthors, con))
                    {
                        cmd.Parameters.AddWithValue("@id", p.Id);


                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {

                              
                                p.Authors.Add(authorDAO.MapAuthor(reader).Build());

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

    

        public List<Publication> GetPublicationByAuthorId(int authorId)
        {
            if (authorDAO == null)
            {
                throw new Exception("Author dao is null");
            }

            List<Publication> p = new List<Publication>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getPublByAuthor, con))
                    {
                        cmd.Parameters.AddWithValue("@id", authorId);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapPublication(reader).Build());
                            }

                        }
                    }
                    foreach (var publ in p)
                    {
                        using (var cmd = new MySqlCommand(getAuthors, con))
                        {
                            cmd.Parameters.AddWithValue("@id", publ.Id);


                            using (var reader = cmd.ExecuteReader())
                            {

                                if (!reader.HasRows)
                                {
                                    throw new Exception("No data found for the query.");

                                }
                                while (reader.Read())
                                {


                                    publ.Authors.Add(authorDAO.MapAuthor(reader).Build());

                                }

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

        public List<Publication> GetPublicationByOrderId(int orderId)
        {
            if (authorDAO == null)
            {
                throw new Exception("Author dao is null");
            }

            List<Publication> p = new List<Publication>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getPublByOrder, con))
                    {
                        cmd.Parameters.AddWithValue("@id", orderId);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapPublication(reader).Build());
                            }

                        }
                    }
                    foreach (var publ in p)
                    {
                        using (var cmd = new MySqlCommand(getAuthors, con))
                        {
                            cmd.Parameters.AddWithValue("@id", publ.Id);


                            using (var reader = cmd.ExecuteReader())
                            {

                                if (!reader.HasRows)
                                {
                                    throw new Exception("No data found for the query.");

                                }
                                while (reader.Read())
                                {


                                    publ.Authors.Add(authorDAO.MapAuthor(reader).Build());

                                }

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

        public List<Publication> GetPublicationByCustomerId(int customerId)
        {
            if (authorDAO == null)
            {
                throw new Exception("Author dao is null");
            }

            List<Publication> p = new List<Publication>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getPublByCustomer, con))
                    {
                        cmd.Parameters.AddWithValue("@id", customerId);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapPublication(reader).Build());
                            }

                        }
                    }
                    foreach (var publ in p)
                    {
                        using (var cmd = new MySqlCommand(getAuthors, con))
                        {
                            cmd.Parameters.AddWithValue("@id", publ.Id);


                            using (var reader = cmd.ExecuteReader())
                            {

                                if (!reader.HasRows)
                                {
                                    throw new Exception("No data found for the query.");

                                }
                                while (reader.Read())
                                {


                                    publ.Authors.Add(authorDAO.MapAuthor(reader).Build());

                                }

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

        public List<Publication> GetPublicationByTitle(string title)
        {
            if (authorDAO == null)
            {
                throw new Exception("Author dao is null");
            }

            List<Publication> p = new List<Publication>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
                {
                    con.Open();


                    using (var cmd = new MySqlCommand(getFullByTitle, con))
                    {
                        cmd.Parameters.AddWithValue("@title", title);

                        using (var reader = cmd.ExecuteReader())
                        {

                            if (!reader.HasRows)
                            {
                                throw new Exception("No data found for the query.");

                            }
                            while (reader.Read())
                            {
                                p.Add(MapPublication(reader).Build());
                            }

                        }
                    }
                    foreach (var publ in p)
                    {
                        using (var cmd = new MySqlCommand(getAuthors, con))
                        {
                            cmd.Parameters.AddWithValue("@id", publ.Id);


                            using (var reader = cmd.ExecuteReader())
                            {

                                if (!reader.HasRows)
                                {
                                    throw new Exception("No data found for the query.");

                                }
                                while (reader.Read())
                                {


                                    publ.Authors.Add(authorDAO.MapAuthor(reader).Build());

                                }

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

        PublicationBuilder MapPublication(MySqlDataReader reader)
        {
            PublicationBuilder p = new PublicationBuilder();
            p.SetId(reader.GetInt64("id"));
            p.SetTitle(reader.GetString("title"));
            p.SetPageCount(reader.GetInt32("page_count"));
            p.SetCirculation(reader.GetInt32("circulation"));
            p.SetGenre((Genre)reader.GetInt32("genre_id"));
            p.SetPrice( reader.GetDecimal("price"));
            return p;
        }

        public void UpdatePublication(Publication a)
        {
            var updatedValuesPublication = new Dictionary<string, object>();

            if (!String.IsNullOrEmpty(a.Title)) updatedValuesPublication.Add("title", a.Title);
            if (a.PageCount!=0) updatedValuesPublication.Add("page_count", a.PageCount);
            if (a.Circulation!=0) updatedValuesPublication.Add("circulation", a.Circulation);
            if (a.Genre!=Genre.Other) updatedValuesPublication.Add("genre_id", a.Genre);
            if (a.Price!=decimal.Zero) updatedValuesPublication.Add("price", a.Price);


            List<MySqlParameter>? updatePublicationParams = null;
            string updatePublicationQuery = "";

            if (updatedValuesPublication.Count > 0)
            {
                updatePublicationParams = QueryBilder.Update("publication", updatedValuesPublication, "id=@id", out updatePublicationQuery);
            }
            

            StringBuilder result = new StringBuilder();
            using (MySqlConnection con = new MySqlConnection(daoConfig.Url))
            {
                con.Open();
                using (var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {

                        if (!String.IsNullOrEmpty(updatePublicationQuery) && updatePublicationParams is not null)
                        {

                            using (var com = new MySqlCommand(updatePublicationQuery, con))
                            {
                                com.Transaction = transaction;
                                com.Parameters.AddWithValue("@id", a.Id);
                                com.Parameters.AddRange(updatePublicationParams.ToArray());
                                int rowsAffected = com.ExecuteNonQuery();
                                result.Append($"{rowsAffected} row(s) updated.");

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
            Console.WriteLine($"Attached observer {observer.GetType()} to MySQLPublicationDAO");
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Console.WriteLine($"Detached observer {observer.GetType()} from MySQLPublicationDAO");
            _observers.Add(observer);
        }

        public void Notify(string operation, object criteria, object result)
        {
            Console.WriteLine($"Notified observers of MySQLPublicationDAO");
            foreach (var obs in _observers)
            {
                obs.Update(operation, criteria, result);
            }
        }
    }
}
