using noslq_pr.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Builder
{
    public abstract class PersonBuilder<T> where T :  PersonBuilder<T>, new()
    {
        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public string Surname { get; protected set; }

        public string Email { get; protected set; }
        public string PhoneNumber { get; protected set; }
        public Address Address { get; protected set; }

        public T SetId(long id)
        {
            Id = id;
            return (T)this;
        }

        public T SetName(string name)
        {
            Name = name;
            return (T)this;
        }

        public T SetSurname(string surname)
        {
            Surname = surname;
            return (T)this;
        }

        public T SetEmail(string email)
        {
            Email = email;
            return (T)this;
        }

        public T SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            return (T)this;
        }
        public T SetAddressId(long AddressId)
        {
            Address.Id = AddressId;
            return (T)this;
        }

        public T SetCountry(string Country)
        {
            Address.Country = Country;
            return (T)this;
        }

        public T SetCity(string City)
        {
            Address.City = City;
            return (T)this;
        }

        public T SetStreet(string Street)
        {
            Address.Street = Street;
            return (T)this;
        }

        public T SetHouse(int House)
        {
            Address.House = House;
            return (T)this;
        }

        public T SetAppartment(int Appartment)
        {
            Address.Apartment = Appartment;
            return (T)this;
        }


    }
}
