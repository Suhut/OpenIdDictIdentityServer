using OpenIddict.Abstractions;
using System.Globalization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIdDictIdentityServer.Data.Seed
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            #region Postman
            string postmanClient = "PostManClient";

            var postman = await manager.FindByClientIdAsync(postmanClient);
            if (postman != null)
            { 
                await manager.DeleteAsync(postman);
            }

            if (await manager.FindByClientIdAsync(postmanClient) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = postmanClient,
                    ClientSecret = "PostMan-Secret",
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "Postman UI Aplication",
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + "apibff",
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
            #endregion

            #region BFF
            string resource_Bff = "Resource_Bff";
            if (await manager.FindByClientIdAsync(resource_Bff) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = resource_Bff,
                    ClientSecret = "Resource-Bff-Secret",
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection,
                    },
                });
            }

            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            string scopeName = "apibff";
            if (await scopeManager.FindByNameAsync(scopeName) is null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "BFF API Access",
                    DisplayNames = { [CultureInfo.GetCultureInfo("id-ID")] = " BFF API" },
                    Name = scopeName,
                    Resources = { "Resource_Bff" }
                });
            }
            #endregion


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
