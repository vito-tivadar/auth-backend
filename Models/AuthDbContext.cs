using Microsoft.EntityFrameworkCore;

namespace auth_backend.Models;
public class AuthDbContext : DbContext {
  public DbSet<UserModel> userModels { get; set; }

  public AuthDbContext(DbContextOptions<AuthDbContext> options) : base (options) { }
}
