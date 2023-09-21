using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIdDictIdentityServer.Data.Seed
{
    public class UserSeed : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserSeed> _logger;

        public UserSeed(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public UserSeed(IServiceProvider serviceProvider, ILogger<UserSeed> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
             
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRoleSystemAdmin(roleManager);
            await SeedRoleBasicUser(roleManager);
            await AddUser(userManager); 

        }

        private async Task AddUser(UserManager<IdentityUser> userManager)
        {
            var sysUser = new IdentityUser
            {
                Email = "suhut.wadiyo@outlook.com",
                UserName = "suhut.wadiyo@outlook.com",
                EmailConfirmed = true, 
            }
            ;

            var superUserInDb =  await userManager.FindByEmailAsync(sysUser.Email);
            if(superUserInDb ==null)
            {
                var rs = await userManager.CreateAsync(sysUser, DefaultContants.DefaultPassword);
                if(!rs.Succeeded)
                {
                    foreach(var error in rs.Errors)
                    {
                        _logger.LogError(error.Description);
                    }
                }
                var result = await userManager.AddToRoleAsync(sysUser, DefaultRole.SystemAdmin);
                result = await userManager.AddToRoleAsync(sysUser, DefaultRole.TenantAdmin );
                if(result.Succeeded)
                {
                    _logger.LogInformation("Seeded Default Sysadmin User");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError(error.Description);
                    }
                }

            }
        } 


        private async Task SeedRoleBasicUser(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(DefaultRole.TenantAdmin);
            if (adminRole == null)
            {
                var role = new IdentityRole
                {
                    Name = DefaultRole.TenantAdmin
                }
                ;
                await roleManager.CreateAsync(role);
                adminRole = role;
            }
            var allClaims = await roleManager.GetClaimsAsync(adminRole);

            var defaultPermission = new List<string>()
            {
                DefaultPermission.Create,
                DefaultPermission.View,
                DefaultPermission.Edit,  

            }
            ;

            foreach (var permission in defaultPermission)
            {
                if (!allClaims.Any(a => a.Type == "Permissiom" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
                }
            }
        }

        private async Task SeedRoleSystemAdmin(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(DefaultRole.SystemAdmin);
            if (adminRole == null)
            {
                var role = new IdentityRole
                {
                    Name = DefaultRole.SystemAdmin
                }
                ;
                await roleManager.CreateAsync(role);
                adminRole = role;
            }
            var allClaims = await roleManager.GetClaimsAsync(adminRole);

            var defaultPermission = new List<string>()
            {
                DefaultPermission.Create,
                DefaultPermission.View,
                DefaultPermission.Edit,
                DefaultPermission.Delete,

            }
            ;

            foreach (var permission in defaultPermission)
            {
                if (!allClaims.Any(a => a.Type == "Permissiom" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal class DefaultPermission
    {
        internal static readonly string View = "View";
        internal static readonly string Edit = "Edit";
        internal static readonly string Delete = "Delete";
        internal static readonly string Create = "Create";
    }
    internal class DefaultRole
    {
        internal static readonly string SystemAdmin = "SystemAdministrator";
        internal static readonly string TenantAdmin = "TenantAdministrator"; 
    }
    internal class DefaultContants
    {
        internal static readonly string SysAdminRole = "View";
        internal static readonly string SysAdminDesc = "Edit";
        internal static readonly string BasicRole = "Delete";
        internal static readonly string UserRole = "Create"; 
        internal static readonly string DefaultPassword = "NotMyPassword";
        internal static readonly string DefaultTenantAdminPassword = "NotMyPassword";
    }

}
