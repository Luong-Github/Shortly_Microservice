using Duende.IdentityServer.Models;

namespace IdentityService.Configs
{
    public class IdentityServerConfig
    {
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope("urlshortent_api", "Url Shortener API"),
            new ApiScope("analytics_api", "Analytics API")
        };

        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "url_shortenere_client",
                AllowedGrantTypes = GrantTypes.Implicit,
                ClientSecrets = { new Secret("tQkM8cZXgXP1GK90841hBaoHIDoEwtud".Sha256())},
                AllowedScopes = { "urlshortent_api", "analytics_api", "openid", "profile" }
            },
            new Client
            {
                ClientId = "analytics_client",
                AllowedGrantTypes = GrantTypes.Implicit,
                ClientSecrets = { new Secret("tQkM8cZXgXP1GK90841hBaoHIDoEwtud".Sha256())},
                AllowedScopes = { "urlshortent_api", "analytics_api", "openid", "profile" }
            }
        };
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }


        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("urlshortent_api", "Url Shortener API"),
                new ApiResource("analytics_api", "Analytics API")
            };
        }
    }
}
