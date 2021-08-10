using System;
using System.ComponentModel.DataAnnotations;

namespace BestSeller.Authentication.Service.Dtos
{
    public record BestSellerUserDto(
        Guid Id,
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string[] FavoritesBookList,
        DateTimeOffset CreatedDate);
    public record AddUserDto([Required][EmailAddress] string Email, [Required] string Password, string FirstName, string LastName);
    public record UpdateUserDto([Required][EmailAddress] string Email, string FirstName, string LastName);

}