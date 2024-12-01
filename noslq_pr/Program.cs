using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using noslq_pr.Builder;
using noslq_pr.DAO;
using noslq_pr.DAO.MYSQL;
using noslq_pr.Entities;
using noslq_pr.FakeDataGenerators;
using noslq_pr.Observer;

namespace noslq_pr
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DAOFactory factory = DAOFactory.Instance;

                //InsertMethods(factory);
                //Console.WriteLine("All information inserted without exceptions");
                //GetMethods(factory);
                //UpdateMethods(factory);
                //FakerTest();
                ObserverTest(factory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

     
        private static void ObserverTest(DAOFactory factory)
        {
            try
            {
                Console.WriteLine("=== Author test ===");
                var authorDAO = factory.GetAuthorDAO();
                if (authorDAO == null) throw new Exception("Author DAO is null");
                string fileNameAuthor = "author_log";
                authorDAO.Attach(new LoggingListenerTXT(fileNameAuthor));
                authorDAO.Attach(new LoggingListenerJSON(fileNameAuthor));
                IFakeDataGenerator<Author> generatorAuthor = new AuthorDataGenerator();
                Author testAuthor = generatorAuthor.GetFakeData();
                authorDAO.AddAuthor(testAuthor);
                Author testAuthor1 = generatorAuthor.GetFakeData();
                authorDAO.UpdateAuthor(new AuthorBuilder().SetId(testAuthor.Id).SetSurname(testAuthor1.Surname).SetAddressId(testAuthor1.Address.Id).
                    SetCountry(testAuthor1.Address.Country).SetPhoneNumber(testAuthor1.PhoneNumber).Build());


                Console.WriteLine("=== Publication test ===");
                string fileNamePublication = "publication_log";
                var PublicationDAO = factory.GetPublicationDAO();
                if (PublicationDAO == null) throw new Exception("PublicationDAO is null");
                PublicationDAO.Attach(new LoggingListenerTXT(fileNamePublication));
                PublicationDAO.Attach(new LoggingListenerJSON(fileNamePublication));
                IFakeDataGenerator<Publication> generatorPublication = new PublicationDataGenerator();
                Publication testPublication = generatorPublication.GetFakeData();
            
                PublicationDAO.AddPublication(testPublication);
              
                PublicationDAO.UpdatePublication(new PublicationBuilder().SetId(testPublication.Id).SetGenre(Genre.Fantasy).SetPrintQuality(PrintQuality.Low).Build());

                Console.WriteLine("=== Order test ===");
                string fileName = "order_log";
                var OrderDAO = factory.GetOrderDAO();
                if (OrderDAO == null) throw new Exception("OrderDAO is null");
                OrderDAO.Attach(new LoggingListenerTXT(fileName));
                OrderDAO.Attach(new LoggingListenerJSON(fileName));
                IFakeDataGenerator<OrderBuilder> generator = new OrderDataGenerator();
                OrderBuilder test0 = generator.GetFakeData();

                Console.WriteLine("=== Customer test ===");
                string fileNameCustomer = "customer_log";
                var customerDAO = factory.GetCustomerDAO();
                if (customerDAO == null) throw new Exception("customer DAO is null");
                customerDAO.Attach(new LoggingListenerTXT(fileNameCustomer));
                customerDAO.Attach(new LoggingListenerJSON(fileNameCustomer));
                IFakeDataGenerator<Customer> generatorCustomer = new CustomerDataGenerator();
                Customer c = generatorCustomer.GetFakeData();
                customerDAO.AddCustomer(c);

                Console.WriteLine("=== Printing House Test ===");
                string fileNamePrintHouse = "printhouse_log";
                var PrintingHouseDAO = factory.GetPrintingHouseDAO();
                if (PrintingHouseDAO == null) throw new Exception("PrintingHouse DAO is null");
                PrintingHouseDAO.Attach(new LoggingListenerTXT(fileNamePrintHouse));
                PrintingHouseDAO.Attach(new LoggingListenerJSON(fileNamePrintHouse));
                IFakeDataGenerator<PrintingHouse> generatorPrintingHouse = new PrintingHouseDataGenerator();
                PrintingHouse pr = generatorPrintingHouse.GetFakeData();
                PrintingHouseDAO.AddPrintingHouse(pr);
                
                test0.SetCustomer(c).SetPrintingHouse(pr);
                Console.WriteLine("=== Add order House Test ===");
                Order otest0 = test0.Build();
                OrderDAO.AddOrder(otest0);
                Console.WriteLine("=== Update order Test ===");
                IFakeDataGenerator<Publication> publGenerator = new PublicationDataGenerator();
                OrderDAO.UpdateOrder(new OrderBuilder().SetId(otest0.Id).SetOrderStatus(OrderStatus.Pending)
                    .SetAcceptanceDate(DateTime.Now).Build());

                OrderDAO.UpdateOrderPublication(new OrderBuilder().SetId(otest0.Id)
                    .AddPublication(publGenerator.GetFakeData())
                    .AddPublication(publGenerator.GetFakeData())
                    .AddPublication(publGenerator.GetFakeData()).Build());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void UpdateMethods(DAOFactory factory)
        {
            Console.WriteLine("\n==Updating==\n");
            IAuthorDAO? authorDAO = factory.GetAuthorDAO();
            if (authorDAO == null) throw new Exception("Author DAO is null");
            ICustomerDAO? CustomerDAO = factory.GetCustomerDAO();
            if (CustomerDAO == null) throw new Exception("Customer DAO is null");
            IPublicationDAO? PublicationDAO = factory.GetPublicationDAO();
            if (PublicationDAO == null) throw new Exception("Publication DAO is null");
            IPrintingHouseDAO? PrintingHouseDAO = factory.GetPrintingHouseDAO();
            if (PrintingHouseDAO == null) throw new Exception("PrintingHouse DAO is null");
            IOrderDAO? OrderDAO = factory.GetOrderDAO();
            if (OrderDAO == null) throw new Exception("Order DAO is null");
            Console.WriteLine("Update order status");
            Order o1 = OrderDAO.GetOrder(2);
            Console.WriteLine("Order before status changed");
            Console.WriteLine(o1);
            OrderBuilder orderBuilder = new OrderBuilder();
            orderBuilder.Reset();
            Order toUpdate = new OrderBuilder().SetId(o1.Id).SetOrderStatus(OrderStatus.Delivered).Build();
            OrderDAO.UpdateOrder(toUpdate);
            OrderStatus status = OrderStatus.Delivered;
            List<Order> orders = OrderDAO.GetOrderbyStatus(status);
            Console.WriteLine("\nOrders with Status " + status.ToString() + ":");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.Id}, Customer ID: {order.Customer.Id}, Date: {order.AcceptanceDate}");
            }
        }
        private static void GetMethods(DAOFactory factory)
        {
            Console.WriteLine("\n==Retrieving==\n");
            IAuthorDAO? authorDAO = factory.GetAuthorDAO();
            if (authorDAO == null) throw new Exception("Author DAO is null");
            ICustomerDAO? CustomerDAO = factory.GetCustomerDAO();
            if (CustomerDAO == null) throw new Exception("Customer DAO is null");
            IPublicationDAO? PublicationDAO = factory.GetPublicationDAO();
            if (PublicationDAO == null) throw new Exception("Publication DAO is null");
            IPrintingHouseDAO? PrintingHouseDAO = factory.GetPrintingHouseDAO();
            if (PrintingHouseDAO == null) throw new Exception("PrintingHouse DAO is null");
            IOrderDAO? OrderDAO = factory.GetOrderDAO();
            if (OrderDAO == null) throw new Exception("Order DAO is null");
            long publicationId = 11; // Example publication ID
            List<Author> authors = authorDAO.GetAuthorByPublicationId(publicationId);
            Console.WriteLine("Authors for Publication ID " + publicationId + ":");
            foreach (var author in authors)
            {
                Console.WriteLine($"Name: {author.Name}, Surname: {author.Surname}");
            }
            // Test 2: Get Customers by Country
            string country = "China"; // Example country
            List<Customer> customers = CustomerDAO.GetCustomerByCountry(country);
            Console.WriteLine("\nCustomers in " + country + ":");
            foreach (var customer in customers)
            {
                Console.WriteLine($"Name: {customer.Name}, Email: {customer.Email}");
            }
            // Test 3: Get All Printing Houses
            List<PrintingHouse> printingHouses = PrintingHouseDAO.GetAllPrintingHouse();
            Console.WriteLine("\nAll Printing Houses:");
            foreach (var printingHouse in printingHouses)
            {
                Console.WriteLine($"Name: {printingHouse.Name}, Address: {printingHouse.Address}");
            }
            // Test 4: Get Publications by Customer ID
            int customerId = 7;
            List<Publication> publications = PublicationDAO.GetPublicationByCustomerId(customerId);
            Console.WriteLine("\nPublications for Customer ID " + customerId + ":");
            foreach (var publication in publications)
            {
                Console.WriteLine($"Title: {publication.Title}, Genre: {publication.Genre}");
            }
            // Test 5: Get Orders by Status
            OrderStatus status = OrderStatus.Pending; // Example status
            List<Order> orders = OrderDAO.GetOrderbyStatus(status);
            Console.WriteLine("\nOrders with Status " + status.ToString() + ":");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.Id}, Customer ID: {order.Customer.Id}, Date: {order.AcceptanceDate}");
            }
        }
        static void InsertMethods(DAOFactory factory)
        {
            Console.WriteLine("\n==Inserting==\n");
            IAuthorDAO? authorDAO = factory.GetAuthorDAO();
            if (authorDAO == null) throw new Exception("Author DAO is null");
            ICustomerDAO? CustomerDAO = factory.GetCustomerDAO();
            if (CustomerDAO == null) throw new Exception("Customer DAO is null");
            IPublicationDAO? PublicationDAO = factory.GetPublicationDAO();
            if (PublicationDAO == null) throw new Exception("Publication DAO is null");
            IPrintingHouseDAO? PrintingHouseDAO = factory.GetPrintingHouseDAO();
            if (PrintingHouseDAO == null) throw new Exception("PrintingHouse DAO is null");
            IOrderDAO? OrderDAO = factory.GetOrderDAO();
            if (OrderDAO == null) throw new Exception("Order DAO is null");
            AuthorBuilder authorBuilder = new AuthorBuilder();
            Author a1 = authorBuilder.SetName("Kate").SetSurname("Middelton").SetEmail("katelove@gmail.com")
            .SetPhoneNumber("7593953").SetCountry("UK").SetCity("London")
            .SetStreet("First Street").SetHouse(12).Build();
            authorBuilder.Reset();
            Author a2 = authorBuilder.SetName("Michael").SetSurname("Johnson").SetEmail("michaelj@outlook.com")
            .SetPhoneNumber("555123456").SetCountry("Australia").SetCity("Sydney")
            .SetStreet("Harbour Road").SetHouse(88).Build();
            authorBuilder.Reset();
            Author a3 = authorBuilder.SetName("Sara").SetSurname("Connor").SetEmail("sara.connor@gmail.com")
            .SetPhoneNumber("321654987").SetCountry("UK").SetCity("Manchester")
            .SetStreet("Rose Street").SetPseudonym("KittyEater").SetHouse(22).Build();
            authorBuilder.Reset();
            Author a4 = authorBuilder.SetName("David").SetSurname("Brown").SetEmail("david.brown@hotmail.com")
            .SetPhoneNumber("678912345").SetCountry("Germany").SetCity("Berlin")
            .SetStreet("Green Lane").SetHouse(30).Build();
            authorBuilder.Reset();
            authorDAO.AddAuthor(a1);
            CustomerBuilder customerBuilder = new CustomerBuilder();
            Customer c1 = customerBuilder.SetId(8).SetName("Oliver").SetSurname("Garcia").SetEmail("oliver.garcia@yahoo.com")
            .SetPhoneNumber("654321789").SetCountry("Spain").SetCity("Madrid")
            .SetStreet("Calle Gran Via").SetHouse(17).SetCustomerType(CustomerType.DirectCustomer).Build();
            customerBuilder.Reset();
            Customer c2 = customerBuilder.SetName("Sophia").SetSurname("Li").SetEmail("sophia.li@outlook.com")
            .SetPhoneNumber("321456987").SetCountry("China").SetCity("Beijing")
            .SetStreet("Tiananmen Square").SetCustomerType(CustomerType.Retailer).SetHouse(3).Build();
            customerBuilder.Reset();
            Customer c3 = customerBuilder.SetName("Liam").SetSurname("Nguyen").SetEmail("liam.nguyen@gmail.com")
            .SetPhoneNumber("147258369").SetCountry("Vietnam").SetCity("Hanoi")
            .SetStreet("Lakeside Street").SetHouse(12).SetCustomerType(CustomerType.Distributor).Build();
            customerBuilder.Reset();
            Customer c4 = customerBuilder.SetName("My Books Store").SetEmail("isabella.miller@mail.com")
            .SetPhoneNumber("963852741").SetCountry("UK").SetCity("Bristol")
            .SetStreet("Maple Avenue").SetHouse(48).SetCustomerType(CustomerType.OnlineStore).Build();
            customerBuilder.Reset();
            Customer c6 = customerBuilder.SetName("Book Haven").SetEmail("contact@bookhaven.com").SetPhoneNumber("123456789")
            .SetCountry("USA").SetCity("New York")
            .SetStreet("Broadway").SetHouse(200).SetCustomerType(CustomerType.OnlineStore).Build();
            CustomerDAO.AddCustomer(c6);
            CustomerDAO.AddCustomer(c1);
            CustomerDAO.AddCustomer(c3);
            CustomerDAO.AddCustomer(c4);
            CustomerDAO.AddCustomer(c2);
            PublicationBuilder publicationBuilder = new PublicationBuilder();
            Publication publication1 = publicationBuilder.SetTitle("Understanding C#").SetPageCount(300)
            .SetCirculation(1000).SetPrice(29.99m)
            .AddAuthor(a1).AddAuthor(a2)
            .SetGenre(Genre.Fiction).SetPrintQuality(PrintQuality.High).SetQuantity(50).Build();
            publicationBuilder.Reset();
            Publication publication2 = publicationBuilder.SetTitle("Exploring the Universe").SetPageCount(250)
            .SetCirculation(500).SetPrice(19.99m)
            .AddAuthor(a3).AddAuthor(a4)
            .SetGenre(Genre.ScienceFiction).SetPrintQuality(PrintQuality.Medium).SetQuantity(30).Build();
            publicationBuilder.Reset();
            Publication publication3 = publicationBuilder.SetTitle("Mastering Python").SetPageCount(400)
            .SetCirculation(1500).SetPrice(39.99m)
            .AddAuthor(a1).AddAuthor(a3)
            .SetGenre(Genre.Fantasy).SetPrintQuality(PrintQuality.High).SetQuantity(75).Build();
            publicationBuilder.Reset();
            Publication publication4 = publicationBuilder.SetTitle("The Art of Cooking").SetPageCount(220)
            .SetCirculation(2000).SetPrice(24.99m)
            .AddAuthor(a2).AddAuthor(a4)
            .SetGenre(Genre.NonFiction).SetPrintQuality(PrintQuality.Low).SetQuantity(100).Build();
            publicationBuilder.Reset();
            Publication publication6 = publicationBuilder.SetTitle("Timeless Literature").SetPageCount(180)
            .SetCirculation(900).SetPrice(15.99m)
            .AddAuthor(a3).AddAuthor(a2)
            .SetGenre(Genre.GraphicNovels).SetPrintQuality(PrintQuality.High).SetQuantity(60).Build();
            publicationBuilder.Reset();
            Publication publication7 = publicationBuilder.SetTitle("The Future of AI").SetPageCount(500)
            .SetCirculation(200).SetPrice(49.99m)
            .AddAuthors(new List<Author> { a1, a2, a3 })
            .SetGenre(Genre.ScienceFiction).SetPrintQuality(PrintQuality.High).SetQuantity(20).Build();
            publicationBuilder.Reset();
            Publication publication8 = publicationBuilder.SetTitle("Wanderlust Adventures").SetPageCount(240)
            .SetCirculation(1200).SetPrice(27.99m)
            .AddAuthor(a2).AddAuthor(a4)
            .SetGenre(Genre.Poetry).SetPrintQuality(PrintQuality.Medium).SetQuantity(70).Build();
            publicationBuilder.Reset();
            Publication publication9 = publicationBuilder.SetTitle("Wanderlust Adventures").SetPageCount(240)
            .SetCirculation(2500).SetPrice(29.99m)
            .AddAuthor(a2).AddAuthor(a4)
            .SetGenre(Genre.Poetry).SetPrintQuality(PrintQuality.Medium).SetQuantity(70).Build();
            publicationBuilder.Reset();
            Author a5 = authorBuilder.SetName("Emma")
            .SetSurname("Johnson")
            .SetEmail("emma.johnson@gmail.com")
            .SetPhoneNumber("789456123")
            .SetCountry("France")
            .SetCity("Paris")
            .SetStreet("Rue de Rivoli")
            .SetHouse(102)
            .Build();
            Publication publication10 = publicationBuilder.SetTitle("Mysteries of the Deep")
            .SetPageCount(320)
            .SetCirculation(1500)
            .SetPrice(34.50m)
            .AddAuthor(a5)
            .SetGenre(Genre.NonFiction)
            .SetPrintQuality(PrintQuality.High)
            .SetQuantity(120)
            .Build();
            PublicationDAO.AddPublication(publication10);
            PrintingHouse printingHouse = new PrintingHouseBuilder()
            .SetName("Quality Print Co.")
            .SetPhoneNumber("123-456-7890")
            .SetAddressId(101)
            .SetCountry("USA")
            .SetCity("San Francisco")
            .SetStreet("Market Street")
            .SetHouse(45)
            .SetAppartment(10)
            .Build();
            var allPrintingHouses = new List<PrintingHouse>();
            PrintingHouseDAO.AddPrintingHouse(printingHouse);
            allPrintingHouses = PrintingHouseDAO.GetAllPrintingHouse();
            authorBuilder.Reset();
            Author a6 = authorBuilder.SetName("Liam")
            .SetSurname("Smith")
            .SetEmail("liam.smith@example.com")
            .SetPhoneNumber("123456789")
            .SetCountry("Canada")
            .SetCity("Toronto")
            .SetStreet("Queen Street")
            .SetHouse(45)
            .Build();
            authorBuilder.Reset();
            Author a7 = authorBuilder.SetName("Sophia")
            .SetSurname("Davis")
            .SetEmail("sophia.davis@example.com")
            .SetPhoneNumber("987654321")
            .SetCountry("Australia")
            .SetCity("Sydney")
            .SetStreet("Bondi Road")
            .SetHouse(23)
            .Build();
            authorBuilder.Reset();
            publicationBuilder.Reset();
            Publication publication11 = publicationBuilder.SetTitle("Journey Through the Stars")
            .SetPageCount(400)
            .SetCirculation(2000)
            .SetPrice(45.99m)
            .AddAuthor(a6)
            .AddAuthor(a7)
            .SetGenre(Genre.ScienceFiction)
            .SetPrintQuality(PrintQuality.Premium)
            .SetQuantity(150)
            .Build();
            OrderBuilder orderBuilder = new OrderBuilder();
            Order order1 = orderBuilder.SetId(101)
            .SetPrice(199.99m)
            .SetOrderStatus(OrderStatus.Pending)
            .SetAcceptanceDate(DateTime.Now.AddDays(-2))
            .SetCustomer(c1)
            .AddPublication(publication8)
            .AddPublication(publication7)
            .SetPrintingHouse(allPrintingHouses[3])
            .Build();
            orderBuilder.Reset();
            Order order2 = orderBuilder.SetId(102)
            .SetPrice(299.99m)
            .SetOrderStatus(OrderStatus.Shipped)
            .SetAcceptanceDate(DateTime.Now.AddDays(-1))
            .SetCustomer(c2)
            .AddPublication(publication2)
            .AddPublication(publication3)
            .SetPrintingHouse(allPrintingHouses[2])
            .Build();
            orderBuilder.Reset();
            Order order3 = orderBuilder.SetId(103)
            .SetPrice(149.50m)
            .SetOrderStatus(OrderStatus.Pending)
            .SetAcceptanceDate(DateTime.Now)
            .SetCustomer(c3)
            .AddPublication(publication4)
            .AddPublication(publication6)
            .SetPrintingHouse(allPrintingHouses[1])
            .Build();
            orderBuilder.Reset();
            Order order4 = orderBuilder.SetId(104)
            .SetPrice(89.99m)
            .SetOrderStatus(OrderStatus.Delivered)
            .SetAcceptanceDate(DateTime.Now.AddDays(1))
            .AddPublication(publication9)
            .SetCustomer(c6)
            .SetPrintingHouse(allPrintingHouses[0])
            .Build();
            orderBuilder.Reset();
            Order order5 = orderBuilder
            .SetPrice(500.99m)
            .SetOrderStatus(OrderStatus.Canceled)
            .SetAcceptanceDate(DateTime.Now.AddDays(-10))
            .SetCustomer(c1)
            .AddPublication(publication11)
            .SetPrintingHouse(allPrintingHouses[1])
            .Build();
            orderBuilder.Reset();
            OrderDAO.AddOrder(order5);
            OrderDAO.AddOrder(order2);
            OrderDAO.AddOrder(order3);
            OrderDAO.AddOrder(order4);
            OrderDAO.AddOrder(order1);
        }
    }

}


