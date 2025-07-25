using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WonderDevTracker.Client.Helpers;

namespace WonderDevTracker.Client
{
    // This is a client-side AuthenticationStateProvider that determines the user's authentication state by
    // looking for data persisted in the page when it was rendered on the server. This authentication state will
    // be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
    // page reload is required.
    //
    // This only provides a user name and email for display purposes. It does not actually include any tokens
    // that authenticate to the server when making subsequent requests. That works separately using a
    // cookie that will be included on HttpClient requests to the server.
    internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly Task<AuthenticationState> _defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        private readonly Task<AuthenticationState> _authenticationStateTask = _defaultUnauthenticatedTask;

        public PersistentAuthenticationStateProvider(PersistentComponentState state)
        {
            if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
            {
                return;
            }

            //Use the extension method instead of manually defining claims
            var claims = userInfo.ToClaims();


            _authenticationStateTask = Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                    authenticationType: nameof(PersistentAuthenticationStateProvider)))));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
    }
}
