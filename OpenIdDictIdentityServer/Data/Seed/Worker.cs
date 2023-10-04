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


            #region 
            string authenticationSchemaClient = "AuthenticationSchemaClient";

            //var obj = await manager.FindByClientIdAsync(authenticationSchemaClient);
            //if (obj != null)
            //{
            //    await manager.DeleteAsync(obj);
            //}

            if (await manager.FindByClientIdAsync(authenticationSchemaClient) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = authenticationSchemaClient,
                    ClientSecret = "AuthenticationSchema-Secret",
                    ConsentType = ConsentTypes.Implicit, 
                    DisplayName = "AuthenticationSchema UI Aplication",
                    RedirectUris = { new Uri("https://localhost:7246/cb-patreon") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
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

            #region Flutter
            string flutterClient = "FlutterClient";

            //var obj = await manager.FindByClientIdAsync(flutterClient);
            //if (obj != null)
            //{ 
            //    await manager.DeleteAsync(obj);
            //}

            if (await manager.FindByClientIdAsync(flutterClient) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = flutterClient, 
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "Flutter UI Aplication",
                    Type = OpenIddictConstants.ClientTypes.Public, 
                    RedirectUris = { new Uri("http://localhost:3000/") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
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

            #region NextJs
            string nextJsClient = "NextJsClient";

            //var obj = await manager.FindByClientIdAsync(nextJsClient);
            //await manager.DeleteAsync(obj);

            if (await manager.FindByClientIdAsync(nextJsClient) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = nextJsClient,
                    ClientSecret = "NextJs-Secret",
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "NextJs UI Aplication",
                    RedirectUris = { new Uri("http://192.168.8.109:3000/api/auth/callback/openiddict") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
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

            #region Postman
            string postmanClient = "PostManClient";

            //var obj1 = await manager.FindByClientIdAsync(postmanClient);
            //await manager.DeleteAsync(obj1);

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
