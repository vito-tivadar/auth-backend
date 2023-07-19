using auth_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_backend.Utility;

public static class MySqlHelpers
{
    public static async Task<UserModel?> FindUserByEmail(AuthDbContext _dbContext, string email)
    {
        return await _dbContext.userModels
            .Where(u => u.email.ToLower() == email.ToLower())
            .FirstOrDefaultAsync();
    }

    public static async Task<UserModel?> FindUserByUsername(
        AuthDbContext _dbContext,
        string username
    )
    {
        return await _dbContext.userModels
            .Where(u => u.username.ToLower() == username.ToLower())
            .FirstOrDefaultAsync();
    }

    public static bool UsernameExitst(AuthDbContext _dbContext, string username)
    {
        return _dbContext.userModels.Any(u => u.username == username);
    }

    public static bool EmailExitst(AuthDbContext _dbContext, string email)
    {
        return _dbContext.userModels.Any(u => u.email == email);
    }

    public static string ConnectionBuilder(
        string[] servers,
        string username,
        string password,
        string database,
        int? port = null,
        bool? pooling = null,
        int? minPoolSize = null,
        int? maxPoolSize = null
    )
    {
        string output = "Server=";
        if (servers.Length == 1)
            output += servers[0];
        else
            for (int i = 0; i < servers.Count(); i++)
            {
                if (servers.Count() == i + 1)
                    output += servers[i] + ";";
                else
                    output += servers[i] + ", ";
            }

        output += " Uid=" + username + ";";
        output += " Pwd=" + password + ";";
        output += " Database=" + database + ";";

        if (port != null)
            output += " Port=" + port.ToString() + ";";
        if (pooling != null)
            output += " Pooling=" + pooling.ToString() + ";";
        if (minPoolSize != null)
            output += " MinimumPoolSize=" + minPoolSize.ToString() + ";";
        if (maxPoolSize != null)
            output += " MaximumPoolsize=" + maxPoolSize.ToString() + ";";

        return output;
    }
}
