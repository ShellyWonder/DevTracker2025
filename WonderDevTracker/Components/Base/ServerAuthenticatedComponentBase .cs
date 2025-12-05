using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace WonderDevTracker.Components.Base
{
    public abstract class ServerAuthenticatedComponentBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        protected ClaimsPrincipal? AuthUser { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateTask;
            AuthUser = authState.User;
            await OnInitializedWithAuthAsync();
        }

        /// <summary>
        /// Override this instead of OnInitializedAsync in derived components.
        /// </summary>
        protected virtual Task OnInitializedWithAuthAsync() => Task.CompletedTask;

        protected bool IsAuthenticated => AuthUser?.Identity?.IsAuthenticated == true;
    }
}
