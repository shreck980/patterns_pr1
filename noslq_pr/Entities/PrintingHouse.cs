using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using noslq_pr.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Entities
{
    public class PrintingHouse
    {
        [BsonIgnore]
        public long Id { get; set; }
        [BsonId]
        public ObjectId ObjectId { get; set; }
        public string Name { get; set; }    
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        public PrintingHouse(PrintingHouseBuilder builder)
        {
            Id = builder.Id;
            ObjectId = builder.ObjectId;
            Name = builder.Name;
            PhoneNumber = builder.PhoneNumber;
            Address = builder.Address;
        }

        public override string ToString()
        {
            return $"Id: {Id}, ObjectId: {ObjectId},\nName: {Name},\nPhoneNumber: {PhoneNumber},\nAddress: {Address}";
        }

    }
}
