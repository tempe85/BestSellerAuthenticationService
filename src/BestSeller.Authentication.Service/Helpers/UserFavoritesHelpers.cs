using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Interfaces;

namespace BestSeller.Authentication.Service.Helpers
{
    public static class UserFavoritesHelpers
    {
        public static async Task<UserBestSellerFavorites> GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(BestSellerUser user, IMongoBaseRepository<UserBestSellerFavorites> userBestSellerFavoritesRepository)
        {
            var userBestSellerFavorites = await userBestSellerFavoritesRepository.GetAllAsync(p => p.UserId == user.Id);
            if (userBestSellerFavorites == null)
            {
                var bestSellerUserFavoritesObject = new UserBestSellerFavorites
                {
                    BestSellerFavorites = Array.Empty<string>(),
                    CreatedDate = DateTimeOffset.Now,
                    Id = Guid.NewGuid(),
                    UserId = user.Id
                };
                await userBestSellerFavoritesRepository.CreateAsync(bestSellerUserFavoritesObject);
                return bestSellerUserFavoritesObject;
            }
            if (userBestSellerFavorites.Count() > 1)
            {
                throw new InvalidOperationException($"User {user.Id} has multiple best seller favorites object entries in the database");
            }
            return userBestSellerFavorites.FirstOrDefault();
        }
    }
}