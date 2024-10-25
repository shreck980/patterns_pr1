using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Entities
{
    public class Address
    {
        public long Id { get; set; }   
        public string Country { get; set; }  
        public string City { get; set; } 
        public string Street { get; set; } 
        public int House { get; set; }      
        public int? Apartment { get; set; } 

     
        public Address() { 
            House = 0;
            Apartment = 0;
        }

        public override string ToString()
        {
            return $"{Street} {House}" +
                   (Apartment.HasValue ? $", Apt {Apartment.Value}" : "") +
                   $", {City}, {Country}";
        }
    }
}
