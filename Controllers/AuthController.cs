using Microsoft.AspNetCore.Mvc;
using auth_backend.Models;
using auth_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using auth_backend.Services;

namespace auth_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _database;
        private readonly ClaimsService _claimsService;
        private readonly string _jwtSecret;

        public AuthController(AuthDbContext database, ClaimsService claimsService, string JwtSecret)
        {
            this._database = database;
            this._claimsService = claimsService;
            this._jwtSecret = JwtSecret;
        }

        [HttpGet]
        public ActionResult<string> Login(
            string email,
            string password,
            int loginDurationInHours = 2
        )
        {
            UserModel? user = MySqlHelpers.FindUserByEmail(_database, email).Result;

            if (user == null)
                return BadRequest("User with provided email does not exist.");

            bool authPassed = AuthorizationHelpers.VerifyPasswordHash(
                password,
                user!.passwordHash,
                user!.passwordSalt
            );
            if (authPassed)
                return Ok(AuthenticationHelpers.CreateJWT(user, _jwtSecret, loginDurationInHours));
            else
                return BadRequest("Provided password is wrong.");
        }

        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody] UserInputModel userInput)
        {
            if (MySqlHelpers.EmailExitst(_database, userInput.email))
                return BadRequest("Email exists.");
            if (MySqlHelpers.UsernameExitst(_database, userInput.username))
                return BadRequest("Username exists.");

            byte[] passwordHash = new byte[] { };
            byte[] passwordSalt = new byte[] { };
            AuthorizationHelpers.HashPassword(
                userInput.password,
                out passwordHash,
                out passwordSalt
            );

            UserModel user = new UserModel()
            {
                username = userInput.username,
                email = userInput.email,
                passwordHash = passwordHash,
                passwordSalt = passwordSalt,
            };

            await _database.userModels.AddAsync(user);
            _database.SaveChanges();

            return Ok(AuthenticationHelpers.CreateJWT(user, _jwtSecret));
        }

        [Authorize]
        [HttpPut]
        public ActionResult<string> UpdateUserData(
            string? newUsername = null,
            string? newEmail = null,
            string? newPassword = null
        )
        {
            bool updateDB = false;

            string? email = _claimsService.GetClaim("Email");

            if (email == null)
                return Unauthorized("Bad JWT Token.");

            UserModel? user = MySqlHelpers.FindUserByEmail(_database, email).Result;
            if (user == null)
                return BadRequest("User does not exist.");

            if (newUsername != null && newUsername != string.Empty && user.username != newUsername)
            {
                if (MySqlHelpers.UsernameExitst(_database, newUsername!))
                    return BadRequest($"User with username '{newUsername}' already exists.");

                user.username = newUsername;
                updateDB = true;
            }

            if (newEmail != null && newEmail != string.Empty && user.email != newEmail)
            {
                if (MySqlHelpers.EmailExitst(_database, newEmail!))
                    return BadRequest($"User with email '{newEmail}' already exists.");

                user.email = newEmail;
                updateDB = true;
            }

            if (newPassword != null && newPassword != string.Empty)
            {
                if (
                    AuthorizationHelpers.VerifyPasswordHash(
                        newPassword,
                        user.passwordHash,
                        user.passwordSalt
                    ) == false
                )
                {
                    byte[] newPasswordHash = new byte[] { };
                    byte[] newPasswordSalt = new byte[] { };
                    AuthorizationHelpers.HashPassword(
                        newPassword!,
                        out newPasswordHash,
                        out newPasswordSalt
                    );

                    user.passwordHash = newPasswordHash;
                    user.passwordSalt = newPasswordSalt;
                    updateDB = true;
                }
            }

            if (updateDB)
            {
                _database.userModels.Update(user);
                _database.SaveChanges();
                System.Console.WriteLine("DB Updated!");
                return Ok(AuthenticationHelpers.CreateJWT(user, _jwtSecret));
            }

            return BadRequest("No credentials were changed!");
        }

        [Authorize]
        [HttpDelete()]
        public ActionResult<UserModel> DeleteUserData()
        {
            string? email = _claimsService.GetClaim("Email");
            if (email == null)
                return Unauthorized("Bad JWT Token.");

            UserModel? user = MySqlHelpers.FindUserByEmail(_database, email).Result;

            if (user == null)
                return BadRequest("User does not exist.");
            else
            {
                _database.userModels.Remove(user);
                _database.SaveChanges();

                return Ok("User deleted successfully.");
            }
        }
    }
}
