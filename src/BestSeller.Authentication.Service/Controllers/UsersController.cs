using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Dtos;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Enums;
using BestSeller.Authentication.Service.Helpers;
using BestSeller.Authentication.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static IdentityServer4.IdentityServerConstants;

namespace BestSeller.Authentication.Service.Controllers
{
  [ApiController]
  [Route("Users")]
  public class UsersController : ControllerBase
  {
    private readonly UserManager<BestSellerUser> _userManager;
    private readonly IMongoBaseRepository<UserBestSellerFavorites> _userBestSellerFavoritesRepository;

    public UsersController(UserManager<BestSellerUser> userManager, IMongoBaseRepository<UserBestSellerFavorites> userBestSellerFavoritesRepository)
    {
      _userManager = userManager;
      _userBestSellerFavoritesRepository = userBestSellerFavoritesRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BestSellerUserDto>>> GetUsersAsync()
    {
      var usersFavoritesBookLists = await _userBestSellerFavoritesRepository.GetAllAsync();
      var users = _userManager.Users.ToList().Select(user =>
      {
        var userBookList = usersFavoritesBookLists.FirstOrDefault(p => p.UserId == user.Id)?.BestSellerFavorites;
        if (userBookList == null)
        {
          return user.AsDto(Array.Empty<BestSellerBook>());
        }
        return user.AsDto(userBookList);
      });
      return Ok(users);
    }

    //Adding this type of user requires no auth
    [HttpPost]
    public async Task<ActionResult<BestSellerUserDto>> AddUser([FromBody] AddUserDto addUserDto)
    {
      var existingUser = await _userManager.FindByEmailAsync(addUserDto.Email);
      if (existingUser != null)
      {
        //Need a better return error message (to explain that user with that email already exists)
        return Forbid();
      }
      var createdUser = new BestSellerUser
      {
        Email = addUserDto.Email,
        UserName = addUserDto.Email,
        FirstName = addUserDto.FirstName,
        LastName = addUserDto.LastName
      };
      await _userManager.CreateAsync(createdUser, addUserDto.Password);
      await _userManager.AddToRoleAsync(createdUser, Roles.BestSellerUser);

      await UserFavoritesHelpers.GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(createdUser, _userBestSellerFavoritesRepository);

      return CreatedAtAction(nameof(GetUserByIdAsync), new { id = createdUser.Id }, createdUser);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<BestSellerUserDto>> GetUserByIdAsync([FromRoute] Guid id)
    {
      var user = await _userManager.FindByIdAsync(id.ToString());
      if (user == null)
      {
        return NotFound();
      }
      var userBestSellerFavoritesObject = await UserFavoritesHelpers.GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(user, _userBestSellerFavoritesRepository);
      return user.AsDto(userBestSellerFavoritesObject.BestSellerFavorites);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid id, UpdateUserDto updateUserDto)
    {
      var user = await _userManager.FindByIdAsync(id.ToString());
      if (user == null)
      {
        return NotFound();
      }
      user.Email = updateUserDto.Email;
      if (!string.IsNullOrWhiteSpace(updateUserDto.FirstName))
      {
        user.FirstName = updateUserDto.FirstName;
      }
      if (!string.IsNullOrWhiteSpace(updateUserDto.LastName))
      {
        user.LastName = updateUserDto.LastName;
      }

      await _userManager.UpdateAsync(user);

      return NoContent();
    }

    [HttpPut("roles/{id}")]
    public async Task<ActionResult> UpdateUserRoleAsync([FromRoute] Guid id, [FromBody] BestSellerRoleType[] roles)
    {

      var existingUser = await _userManager.FindByIdAsync(id.ToString());
      if (existingUser == null)
      {
        return NotFound();
      }
      var currentRoles = existingUser.Roles;


      if (roles.Any(p => p == BestSellerRoleType.Admin))
      {
        await _userManager.AddToRoleAsync(existingUser, Roles.Admin);
      }

      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
      var user = await _userManager.FindByIdAsync(id.ToString());
      if (user == null)
      {
        return NotFound();
      }

      await _userManager.DeleteAsync(user);

      return NoContent();
    }

    [HttpGet("{id}/Favorites")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserFavoriteBookListAsync([FromRoute] Guid id)
    {
      var user = await _userManager.FindByIdAsync(id.ToString());
      var favoritesBookListObject = await UserFavoritesHelpers.GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(user, _userBestSellerFavoritesRepository);
      return Ok(favoritesBookListObject.BestSellerFavorites);
    }

    [HttpGet("Favorites/TopBooks")]
    public async Task<ActionResult<Dictionary<string, int>>> GetTopUserFavoriteBookTitlesAsync()
    {
      var bookList = (await _userBestSellerFavoritesRepository.GetAllAsync())?.SelectMany(p => p.BestSellerFavorites);

      var groups =
          from book in bookList
          group book by book into g
          select new
          {
            Book = g.Key,
            Count = g.Count()
          };
      var dictionary = groups.ToDictionary(g => g.Book.Title, g => g.Count);
      return Ok(dictionary);
    }

    [HttpPut("{id}/Favorites/AddBooks/")]
    public async Task<ActionResult> AddBooksToUserFavoritesAsync([FromRoute] Guid id, [FromBody] BestSellerBook[] booksToAdd)
    {
      if (booksToAdd == null)
      {
        throw new Exception("There are no books to add, book list is null");
      }
      var user = await _userManager.FindByIdAsync(id.ToString());
      var favoritesBookListObject = await UserFavoritesHelpers.GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(user, _userBestSellerFavoritesRepository);
      var favoritesList = favoritesBookListObject.BestSellerFavorites.ToList();
      favoritesList.AddRange(booksToAdd);
      favoritesBookListObject.BestSellerFavorites = favoritesList.Distinct().ToArray();
      await _userBestSellerFavoritesRepository.UpdateAsync(favoritesBookListObject);
      return Ok();
    }

    [HttpPut("{id}/Favorites/RemoveBooks/")]
    public async Task<ActionResult> RemoveBooksFromUserFavoritesAsync([FromRoute] Guid id, [FromBody] BestSellerBook[] booksToRemove)
    {
      if (booksToRemove == null)
      {
        throw new Exception("There are no books to remove, book list is null");
      }
      var user = await _userManager.FindByIdAsync(id.ToString());
      var favoritesBookListObject = await UserFavoritesHelpers.GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(user, _userBestSellerFavoritesRepository);
      var favoritesList = favoritesBookListObject.BestSellerFavorites;
      favoritesList = favoritesList.Where(p => !booksToRemove.Contains(p)).ToArray();
      favoritesBookListObject.BestSellerFavorites = favoritesList;
      await _userBestSellerFavoritesRepository.UpdateAsync(favoritesBookListObject);
      return Ok();
    }

    [HttpPut("{id}/Favorites/UpdateBooks/")]
    public async Task<ActionResult> UpdateBooksFromUserFavoritesAsync([FromRoute] Guid id, [FromBody] BestSellerBook[] favoriteBooks)
    {
      if (favoriteBooks == null)
      {
        throw new Exception("There are no books to update, book list is null");
      }
      var user = await _userManager.FindByIdAsync(id.ToString());
      var favoritesBookListObject = await UserFavoritesHelpers.GetOrCreateUserBestSellerFavoritesFromUserIfOneDoesNotExistAsync(user, _userBestSellerFavoritesRepository);
      favoritesBookListObject.BestSellerFavorites = favoriteBooks;
      await _userBestSellerFavoritesRepository.UpdateAsync(favoritesBookListObject);
      return Ok();
    }
  }
}