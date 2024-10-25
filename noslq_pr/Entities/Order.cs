using noslq_pr.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public DateTime AcceptanceDate { get; set; }
        public PrintingHouse PrintingHouse { get; set; }
        public decimal Price { get; set; }
        public List<Publication> Publications { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus Status { get; set; }
        public Order(OrderBuilder builder) {

            Id= builder.Id; 
            AcceptanceDate= builder.AcceptanceDate;
            PrintingHouse= builder.PrintingHouse;   
            Price= builder.Price;
            Publications= builder.Publications;
            Customer= builder.Customer;
            Status= builder.Status;
        }

        public override string ToString()
        {
            var publicationCount = Publications.Count;
            var publicationDetails = publicationCount > 0
                ? string.Join(", ", Publications.Select(p => p.Title)) 
                : "No publications";

            return $"Order ID: {Id}, Acceptance Date: {AcceptanceDate}, " +
                   $"Price: {Price:C}, Status: {Status}, " +
                   $"Customer: {Customer?.Id}, PrintingHouse: {PrintingHouse?.Id}, Publications: {publicationDetails}";
        }
    }


    public enum OrderStatus
    {
        Pending=1,      // order has been placed and is awaiting processing
        InProduction, // order is being prepared (e.g., books are being printed)
        Shipped,      // order has been shipped to the customer
        Delivered,    // order has been delivered to the customer
        Canceled,      // order has been canceled
        Other
    }

}
