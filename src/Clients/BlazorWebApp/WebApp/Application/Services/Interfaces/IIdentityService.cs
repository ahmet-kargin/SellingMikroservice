namespace WebApp.Application.Services.Interfaces;

public interface IIdentityService
{
    string GetUserName();
    string GetUserToken();
    bool IsLoggedIn { get; }
    Task<bool> Loggin(string username, string password);
    void Logout();

}
