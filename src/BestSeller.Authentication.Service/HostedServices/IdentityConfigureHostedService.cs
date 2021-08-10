using System.Threading;
using System.Threading.Tasks;
using BestSeller.Authentication.Service.Entities;
using BestSeller.Authentication.Service.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BestSeller.Authentication.Service.HostedServices
{
    public class IdentityConfigureHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IdentitySettings _identitySettings;

        public IdentityConfigureHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<IdentitySettings> identityOptions)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _identitySettings = identityOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
            //Use this logic to create endpoints for directly creating a user
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BestSellerUser>>();

            await CreateRoleIfDoesNotExistAsync(Roles.Admin, roleManager);
            await CreateRoleIfDoesNotExistAsync(Roles.BestSellerUser, roleManager);

            var adminUser = await userManager.FindByEmailAsync(_identitySettings.AdminEmail);
            if (adminUser == null)
            {
                adminUser = new BestSellerUser
                {
                    UserName = _identitySettings.AdminEmail,
                    Email = _identitySettings.AdminEmail,
                    FirstName = "Admin",
                    LastName = "Admin",
                };
                await userManager.CreateAsync(adminUser, _identitySettings.AdminPassword);
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task CreateRoleIfDoesNotExistAsync(string role, RoleManager<UserRole> roleManager)
        {
            var exists = await roleManager.RoleExistsAsync(role);

            if (!exists)
            {
                await roleManager.CreateAsync(new UserRole { Name = role });
            }
        }
    }
}