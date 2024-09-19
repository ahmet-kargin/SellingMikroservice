using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using WebApp.Extentions;

namespace WebApp.Utils;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationState _authenticationState;

    public AuthStateProvider(ILocalStorageService localStorageService, HttpClient httpClient, AuthenticationState authenticationState)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
        _authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        String apiToken = await _localStorageService.GetToken();
        if (String.IsNullOrEmpty(apiToken))
        {
            return _authenticationState;
        }
        String userName = await _localStorageService.GetUserName();

        var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userName),
        }, "jwtAuthType"));

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        return new AuthenticationState(cp);
    }

    public void NotifyUseLogin(String userName)
    {
        var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name,userName),
        }, "jwtAuthType"));
        var authState = Task.FromResult(_authenticationState);
        NotifyAuthenticationStateChanged(authState);

    }
    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_authenticationState);
        NotifyAuthenticationStateChanged(authState);
    }
}
