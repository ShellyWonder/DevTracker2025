using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Components.BaseComponents
{
    public abstract class AuthenticatedComponentBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        protected UserInfo? UserInfo { get; set; }
        protected ClaimsPrincipal? AuthUser { get; private set; }
        //guard UI until auth is loaded
        protected bool AuthReady { get; set; } 
        protected override async Task OnInitializedAsync()
        {
            //load the auth state and set AuthUser
            var authState = await AuthStateTask;
            AuthUser = authState.User;

            UserInfo ??= await UserInfoHelper.GetUserInfoAsync(AuthStateTask);
            AuthReady = true;
            await OnInitializedWithAuthAsync();
        }

        /// <summary>
        /// Override this method instead of OnInitializedAsync when inheriting.
        /// </summary>
        protected virtual Task OnInitializedWithAuthAsync() => Task.CompletedTask;

        //use for a single role scenario(e.g. show/hide in the navMenu)
        protected bool UserIsInRole(Role role) => AuthUser?.IsInRole(role.ToString()) ?? false;

        //use for multi-role scenario
        protected bool UserIsAnyRole(params Role[] roles) =>
                       roles.Any(r => AuthUser?.IsInRole(r.ToString()) == true);
    }
}
