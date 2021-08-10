using BestSeller.Authentication.Service.Interfaces;

namespace BestSeller.Authentication.Service.Models
{
    public class BestSellerSettings : IDatabaseSettings
    {
        public string ConnectionString { get; init; }
        public string DatabaseName { get; init; }
        public string UserBestSellerFavoritesCollectionName { get; init; }
        public string UsersCollectionName { get; init; }
    }
}