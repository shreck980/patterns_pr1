using noslq_pr.DAO.MYSQL;
using noslq_pr.DAO.NoSQL;
using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO.Migration
{
    public class AuthorMigrationService : MigrationService
    {

      
        private readonly DAOFactory daoFactory;
       
        public AuthorMigrationService(DAOFactory dAOFactory)
        {
            daoFactory = dAOFactory;
          

        }

        public void Migration(DataBaseType sourse, DataBaseType destination)
        {
            try
            {
                ConnectionService.EstablishConnection(sourse);
                IAuthorDAO authorDAO = daoFactory.GetAuthorDAO();
                if (authorDAO == null) throw new Exception("Author is null");
                List<Author> authors = authorDAO.GetAllAuthors();

                if (authors.Count == 0) throw new Exception("No authors in mysql database found");

                ConnectionService.EstablishConnection(destination);
                authorDAO = daoFactory.GetAuthorDAO();
                if(authorDAO == null) throw new Exception("Author is null");
                authorDAO.AddAuthors(authors);

                Console.WriteLine($"Successfully migrated customer data from {sourse} to {destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

       
    }
}
