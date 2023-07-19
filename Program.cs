using Microsoft.EntityFrameworkCore;
using auth_backend.Models;
using auth_backend.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using auth_backend.Services;

var builder = WebApplication.CreateBuilder(args);

string? JwtSecret = Environment.GetEnvironmentVariable("AUTH_API_JWT_SECRET");
int dbPort = Convert.ToInt16(Environment.GetEnvironmentVariable("MYSQL_DB_PORT"));
string? dbPassword = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");
string? dbName = Environment.GetEnvironmentVariable("MYSQL_DATABASE_NAME");

if (JwtSecret == null)
    JwtSecret = "";

// string authDbConnectionString = MySqlHelpers.ConnectionBuilder(
//     new string[] { "localhost", "host.docker.internal" },
//     "root",
//     dbPassword!,
//     dbName!,
//     dbPort
// );

string authDbConnectionString = MySqlHelpers.ConnectionBuilder(
    new string[] { "localhost", "host.docker.internal" },
    "root",
    "root",
    "portfolio",
    3306
);

System.Console.WriteLine(authDbConnectionString);

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseMySQL(authDbConnectionString);
});

builder.Services.AddSingleton<string>(JwtSecret!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = AuthenticationHelpers.GetTokenValidationParameters(
            JwtSecret!
        );
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ClaimsService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AuthDbContext>();
    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT Authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
