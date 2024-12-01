using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Builder
{
    public class PrintingHouseBuilder
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public Address Address { get; private set; }
        public PrintingHouseBuilder()
        {
            Address = new Address();
            Id = 0;
        }

        public PrintingHouseBuilder SetId(long id)
        {
            Id = id;
            return this;
        }

        public PrintingHouseBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

     
        public PrintingHouseBuilder SetPhoneNumber(string PhoneNumber)
        {
            this.PhoneNumber = PhoneNumber;
            return this;
        }

        public PrintingHouseBuilder SetAddressId(long AddressId)
        {
            Address.Id = AddressId;
            return this;
        }

        public PrintingHouseBuilder SetCountry(string Country)
        {
            Address.Country = Country;
            return this;
        }

        public PrintingHouseBuilder SetCity(string City)
        {
            Address.City = City;
            return this;
        }

        public PrintingHouseBuilder SetStreet(string Street)
        {
            Address.Street = Street;
            return this;
        }

        public PrintingHouseBuilder SetHouse(int House)
        {
            Address.House = House;
            return this;
        }

        public PrintingHouseBuilder SetAppartment(int Appartment)
        {
            Address.Apartment = Appartment;
            return this;
        }


        public PrintingHouse Build()
        {
            return new PrintingHouse(this);
        }
    }
}
