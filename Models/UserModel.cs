using System.Collections.ObjectModel;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace auth_backend.Models;

[Table("users")]
public class UserModel
{
    [Key]
    [Column(Order = 1)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [Column("username")]
    public string username { get; set; } = String.Empty;

    [Column("email")]
    public string email { get; set; } = String.Empty;

    [Column("password")]
    public byte[] passwordHash { get; set; }

    [Column("salt")]
    public byte[] passwordSalt { get; set; }

    [Column("role")]
    public Role role { get; set; } = Role.user;

    public Collection<Claim> GetClaims()
    {
        Collection<Claim> claims = new Collection<Claim>()
        {
            new Claim("Id", this.id.ToString()),
            new Claim("Username", this.username),
            new Claim("Email", this.email),
            new Claim("Role", this.role.ToString()),
        };
        return claims;
    }
}

public enum Role
{
    user,
    member,
    moderator,
    admin,
    owner,
}
