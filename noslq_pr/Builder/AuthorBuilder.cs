using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Builder
{
    public class AuthorBuilder : PersonBuilder<AuthorBuilder>
    {

       
        public string Pseudonym { get; private set; }

        public AuthorBuilder()
        {
            Id = 0;
            Address = new Address();
        }

        

        public AuthorBuilder SetPseudonym(string pseudonym)
        {
            this.Pseudonym = pseudonym;
            return this;
        }


        public Author Build()
        {
            return new Author(this);
        }

        public void Reset()
        {
            this.Address = new Address();
            Id = 0;
            Name = null;
            Surname=String.Empty;

            Email = String.Empty;
            PhoneNumber = String.Empty;
            Pseudonym = String.Empty;
        }
    }
    
}
