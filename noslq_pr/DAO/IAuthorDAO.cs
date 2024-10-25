﻿using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.DAO
{
    public interface  IAuthorDAO
    {

        void AddAuthor(Author p);
       
        Author GetAuthor(int id);
        Author GetAuthorByName(string name,string surname);
        List<Author> GetAuthorByPublicationId(long publId);
        void AddAuthorsToPublication(Publication p);
        void UpdateAuthor(Author a);

    }
}