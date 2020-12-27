using Common;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer
{
    public static class IDPConfiguration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(), 
                new IdentityResource
                {
                    Name="privilages.scope",
                    UserClaims =
                    {
                        "get",
                        "add",
                        "update",
                        "delete",
                        ClaimTypes.Role
                    }
                },
                
            };
        public static IEnumerable<ApiResource> GetApiResources()
            => new List<ApiResource>
            {
                new ApiResource(name:"ApiOne",displayName:"API One resource"),
                new ApiResource(name:"ApiTwo",displayName:"API Two resource")
            };
        public static IEnumerable<ApiScope> GetApiScopes()
            => new List<ApiScope>
            {
                new ApiScope("ApiOne","Api One Scope",new string []{ "get","add"}),
                new ApiScope("ApiTwo","Api One Scope"),
            };

        
        public static IEnumerable<Client> GetClients()
            => new List<Client>
            {
                new Client
                {
                    ClientId="ApiTwoClient_id",
                    ClientSecrets={new Secret("ApiTwoSecret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={ "ApiOne" }
                },
                new Client
                {
                    ClientId="MvcClient_id",
                    ClientSecrets={new Secret("MvcClient_Secret".Sha256()) },
                    AllowedGrantTypes=GrantTypes.Code,
                    AllowedScopes={
                        "ApiOne","ApiTwo" ,
                        StandardScopes.Profile,
                        StandardScopes.OpenId,
                       
                        "privilages.scope"
                    },
                   
                    RedirectUris={$"{Constants.MVCAppBaseAdress}signin-oidc" },
                    RequireConsent=true,
                    AllowOfflineAccess=true,
                    //AlwaysIncludeUserClaimsInIdToken=true
                },
                new Client
                {
                    ClientId="consolApp_id",
                    ClientSecrets={new Secret("consolApp_secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={"ApiOne","ApiTwo"},
                    AllowOfflineAccess=true
                }
            };
    }
}
