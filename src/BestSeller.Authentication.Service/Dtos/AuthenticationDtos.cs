using System;
using System.ComponentModel.DataAnnotations;
using BestSeller.Authentication.Service.Entities;

namespace BestSeller.Authentication.Service.Dtos
{
  public record BestSellerUserDto(
      Guid Id,
      string Username,
      string Email,
      string FirstName,
      string LastName,
      BestSellerBook[] FavoritesBookList,
      DateTimeOffset CreatedDate);
  public record AddUserDto([Required][EmailAddress] string Email, string FirstName, string LastName);
  public record UpdateUserDto([Required][EmailAddress] string Email, string FirstName, string LastName);

}