namespace auth_backend.Models;

public class UserInputModel
{
    public string email { get; set; } = String.Empty;
    public string username { get; set; } = String.Empty;
    public string password { get; set; } = String.Empty;
}
