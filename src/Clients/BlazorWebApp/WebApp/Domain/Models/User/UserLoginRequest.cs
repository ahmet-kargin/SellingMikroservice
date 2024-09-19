namespace WebApp.Domain.Models.User;

public class UserLoginRequest
{
    public UserLoginRequest(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }

    public string UserName { get;private set; }
    public string Password { get;private set; }
}
