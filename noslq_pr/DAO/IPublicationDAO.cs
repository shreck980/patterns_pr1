using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface IPublicationDAO
    {
        void AddPublication(Publication p);
        Publication GetPublication(int id);
        List<Publication> GetPublicationByAuthorId(int authorId);
        List<Publication> GetPublicationByOrderId(int orderId);
        List<Publication> GetPublicationByCustomerId(int customerId);
        List<Publication> GetPublicationByTitle(string title);
        void UpdatePublication(Publication p);  
        

       
    }
}
