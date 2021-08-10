using System;

namespace BestSeller.Authentication.Service.Interfaces
{
    public interface IMongoEntity
    {
        Guid Id { get; init; }
    }
}