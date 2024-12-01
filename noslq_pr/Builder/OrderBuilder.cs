using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Builder
{
    public class OrderBuilder
    {
        public long Id { get; private set; }
        public DateTime AcceptanceDate { get; private set; }
        public PrintingHouse PrintingHouse { get; private set; }
        public decimal Price { get; private set; }
        public List<Publication> Publications { get; private set; }
        public Customer Customer { get; private set; }
        public OrderStatus Status { get; private set; }
        public OrderBuilder()
        {
            Publications = new List<Publication>();
            Status = OrderStatus.Other;
            Price = decimal.Zero;
            Id = 0;
            AcceptanceDate = DateTime.Now;
            Customer = new CustomerBuilder().Build();
            PrintingHouse = new PrintingHouseBuilder().Build();
        }

        public OrderBuilder SetId(long id)
        {
            Id = id;
            return this;
        }

        public OrderBuilder SetAcceptanceDate(DateTime AcceptanceDate)
        {
            this.AcceptanceDate = AcceptanceDate;
            return this;
        }

        public OrderBuilder SetPrintingHouse(PrintingHouse PrintingHouse)
        {
            this.PrintingHouse = PrintingHouse;
            return this;
        }


        public OrderBuilder SetPrice(decimal Price)
        {
            this.Price = Price;
            return this;
        }


        public OrderBuilder AddPublication(Publication p)
        {
            Publications.Add(p);
            return this;
        }

        public OrderBuilder SetCustomer(Customer Customer)
        {
            this.Customer = Customer;
            return this;
        }

        public OrderBuilder SetOrderStatus(OrderStatus OrderStatus)
        {
            this.Status = OrderStatus;
            return this;
        }


        public Order Build()
        {
            /*if (Publications.Count == 0 || String.IsNullOrEmpty(Customer.Name)
                || String.IsNullOrEmpty(PrintingHouse.Name)) 
                throw new Exception("Unable to create instance of Order, requiared fields are empty");*/
            return new Order(this);
        }

        public void Reset()
        {
            Publications = new List<Publication>();
            Status = OrderStatus.Other;
            Price = decimal.Zero;
            Id = 0;
            AcceptanceDate = DateTime.Now;
            Customer = new CustomerBuilder().Build();
            PrintingHouse = new PrintingHouseBuilder().Build();
        }
    }
}
