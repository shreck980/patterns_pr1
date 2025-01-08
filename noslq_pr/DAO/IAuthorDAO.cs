using noslq_pr.Entities;
using noslq_pr.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface  IAuthorDAO: ISubject
    {

        void AddAuthor(Author p);
        void AddAuthors(List<Author> list);
       
        Author GetAuthor(Object id);
        Author GetAuthorByName(string name,string surname);
        List<Author> GetAuthorByPublicationId(long publId);
        List<Author> GetAllAuthors();
        void AddAuthorsToPublication(Publication p);
        void UpdateAuthor(Author a);

    }
}
