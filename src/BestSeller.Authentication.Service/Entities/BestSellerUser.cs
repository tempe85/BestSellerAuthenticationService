using System;
using AspNetCore.Identity.MongoDbCore.Models;
using BestSeller.Authentication.Service.Interfaces;
using MongoDbGenericRepository.Attributes;

namespace BestSeller.Authentication.Service.Entities
{

    [CollectionName("Users")]
    public class BestSellerUser : MongoIdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}