using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface IOrderDAO : ISubject
    {

        void AddOrder(Order o);
        void AddOrders(List<Order> list);
        void UpdateOrder(Order o);
        void UpdateOrderPublication(Order o);
        Order GetOrder(object ObjectId);
        List<Order> GetAllOrder();
        List<Order> GetOrderbyDate(DateTime date);
        List<Order> GetOrderbyCustomer(string email);
        List<Order> GetOrderbyStatus(OrderStatus status);
    }
}
