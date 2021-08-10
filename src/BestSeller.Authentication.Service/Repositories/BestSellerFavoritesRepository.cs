using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Interfaces;
using MongoDB.Driver;

namespace BestSeller.Authentication.Service.Repositories
{
    public class BestSellerFavoritesRepository : MongoBaseRepository<UserBestSellerFavorites>
    {
        public BestSellerFavoritesRepository(
            IMongoCollection<UserBestSellerFavorites> collection) : base(collection)
        {

        }
    }
}