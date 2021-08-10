using System;
using BestSeller.Authentication.Service.Interfaces;

namespace BestSeller.Authentication.Service.Entities
{
    public class WorkArea : IMongoEntity
    {
        public Guid Id { get; init; }
        public Guid WorkBuildingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
