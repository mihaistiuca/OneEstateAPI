using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace OneEstate.Domain.Entities.Base
{
    public class EntityBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
