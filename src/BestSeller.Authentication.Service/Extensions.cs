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

        public static BestSellerUserDto AsDto(this BestSellerUser factorySchedulerUser, string[] favoritesBookList) =>
                new BestSellerUserDto(Id: factorySchedulerUser.Id,
                                            Username: factorySchedulerUser.UserName,
                                            Email: factorySchedulerUser.Email,
                                            FirstName: factorySchedulerUser.FirstName,
                                            LastName: factorySchedulerUser.LastName,
                                            FavoritesBookList: favoritesBookList,
                                            CreatedDate: factorySchedulerUser.CreatedOn);

    }
}