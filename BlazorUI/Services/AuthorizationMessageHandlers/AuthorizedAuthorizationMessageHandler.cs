using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Services.AuthorizationMessageHandlers
{
    public class AuthorizedAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public AuthorizedAuthorizationMessageHandler(IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { "https://dzigalleryfunctions.azurewebsites.net" },
                scopes: new[] 
                {
                    "fca594e3-3b1d-460a-b21a-425b32473479/Annotations.Edit",
                    "fca594e3-3b1d-460a-b21a-425b32473479/Images.Edit"
                });
        }
    }
}
