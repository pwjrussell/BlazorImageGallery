using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrudFunctions.Services
{
    public class AADJwtService
    {
        // Reference: https://github.com/Azure-Samples/ms-identity-dotnet-webapi-azurefunctions

        #region Constants
        private string Audience { get; }
        private string ClientId { get; }
        private string Tenant { get; }
        private string TenantId { get; }

        private string Authority { get { return $"https://login.microsoftonline.com/{Tenant}/v2.0"; } }
        private List<string> ValidIssuers
        {
            get
            {
                return new List<string>()
            {
                $"https://login.microsoftonline.com/{Tenant}/",
                $"https://login.microsoftonline.com/{Tenant}/v2.0",
                $"https://login.windows.net/{Tenant}/",
                $"https://login.microsoft.com/{Tenant}/",
                $"https://sts.windows.net/{TenantId}/"
            };
            }
        }
        #endregion

        public AADJwtService()
        {
            Audience = "https://dzigalleryfunctions.azurewebsites.net";
            ClientId = "fca594e3-3b1d-460a-b21a-425b32473479";
            Tenant = "pwjrussellgmail.onmicrosoft.com";
            TenantId = "19671f89-d1c1-42f1-9fdb-d543f400f2ea";
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(HttpRequest request)
        {
            return new JwtSecurityTokenHandler().ValidateToken(
                GetBearerAccessToken(request),
                new TokenValidationParameters()
                {
                    ValidAudiences = new[] { Audience, ClientId },
                    ValidIssuers = ValidIssuers,
                    IssuerSigningKeys = (await GetConfigAsync()).SigningKeys
                },
                out SecurityToken _);
        }

        private string GetBearerAccessToken(HttpRequest request)
        {
            string[] parts = request.Headers["Authorization"].ToString().Split(null);
            if ((parts.Length != 2) || (parts[0] != "Bearer"))
            {
                throw new ArgumentException();
            }
            return parts[1];
        }

        private async Task<OpenIdConnectConfiguration> GetConfigAsync()
        {
            return await new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{Authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever()
                ).GetConfigurationAsync();
        }
    }
}
