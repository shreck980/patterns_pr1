using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Entities
{
    public abstract class Person
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }

        

       
    }
}
