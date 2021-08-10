using System;
using BestSeller.Authentication.Service.Enums;
using BestSeller.Authentication.Service.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BestSeller.Authentication.Service.Entities
{
    public class User_Certification : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; init; }
        //Unique key constraint on WorkStation Id + User Id
        public Guid WorkStationId { get; init; }
        public Guid UserId { get; init; }
        public StationCertification StationCertification { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public bool HasExperience => StationCertification != StationCertification.None;
    }
}