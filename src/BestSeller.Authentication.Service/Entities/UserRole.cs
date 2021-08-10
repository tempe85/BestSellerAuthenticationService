using System;
using AspNetCore.Identity.MongoDbCore.Models;
using BestSeller.Authentication.Service.Interfaces;
using MongoDbGenericRepository.Attributes;

namespace BestSeller.Authentication.Service.Entities
{

    [CollectionName("UserRoles")]
    public class UserRole : MongoIdentityRole<Guid>
    {

    }
}