using System;
using System.ComponentModel.DataAnnotations;
using BestSeller.Authentication.Service.Enums;

namespace BestSeller.Authentication.Service.Dtos
{
    //General Favorites Dto
    public record UserBestSellerFavoritesDto(Guid FavoritesId, Guid UserId, string[] FavoritesBookList, DateTimeOffset CreatedDate);
    public record CreateUserFavoritesDto([Required] string[] FavoritesBookList);
    public record AddBooksToUserFavoritesDto([Required] string[] BookList);
    public record RemoveBooksFromUserFavoritesDto([Required] string[] BookList);
    public record UpdateUserFavoriteBooksDto([Required] string[] BookList);


}