using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Dtos;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Interfaces;

namespace BestSeller.Authentication.Service
{
    public static class Extensions
    {

        public static BestSellerUserDto AsDto(this BestSellerUser user, string[] favoritesBookList) =>
                new BestSellerUserDto(Id: user.Id,
                                            Username: user.UserName,
                                            Email: user.Email,
                                            FirstName: user.FirstName,
                                            LastName: user.LastName,
                                            FavoritesBookList: favoritesBookList,
                                            CreatedDate: user.CreatedOn);

    }
}