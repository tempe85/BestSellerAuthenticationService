using System;
using BestSeller.Authentication.Service.Interfaces;

namespace BestSeller.Authentication.Service.Entities
{
  public class UserBestSellerFavorites : IMongoEntity
  {
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public BestSellerBook[] BestSellerFavorites { get; set; }
    public DateTimeOffset CreatedDate { get; init; }
  }
}
