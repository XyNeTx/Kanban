using HINOSystem.Context;
using KANBAN.Models.KB3.Login;
using KANBAN.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace KANBAN.Controllers.API.Login
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly KB3Context _kb3Context;

        public LoginController(IConfiguration configuration, KB3Context kb3Context)
        {
            _configuration = configuration;
            _kb3Context = kb3Context;
        }



        [HttpPost]
        public async Task<IActionResult> GenerateJWTToken(LoginVM loginVM)
        {
            try
            {
                var user = await _kb3Context.User
                    .FirstOrDefaultAsync(x => x.Code!.Trim() == loginVM.UserCode.Trim());

                string query = $@"	SELECT
                    DISTINCT
                    M.i18n AS Value
                    FROM [erp].[UserAuthorize] UA
                    INNER JOIN [erp].[User] U
                    ON U._ID = UA.User_ID
                    INNER JOIN [erp].[Menu] M
                    ON M._ID = UA.Menu_ID
                    WHERE U.Code = '{loginVM.UserCode}'
                    AND M.i18n NOT LIKE 'ERP%'";

                var userAuth = await _kb3Context.Database.SqlQueryRaw<string>(query).ToListAsync();

                if (userAuth == null || userAuth.Count == 0)
                {
                    throw new CustomHttpException(404, "User Code Not found Please Send a DP Request to access this Program");
                }

                if (user == null)
                {
                    throw new CustomHttpException(404, "User Code Not found Please Send a DP Request to access this Program");
                }

                var secret = _configuration["ApplicationSettings:JWT_Secret"];
                var keyBytes = Convert.FromBase64String(secret);

                var signinKey = new SymmetricSecurityKey(keyBytes);

                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user._ID.ToString()),
                    new Claim(ClaimTypes.Name, user.Name + " " + user.Surname),
                    new Claim(ClaimTypes.Locality, loginVM.Factory),
                    new Claim(ClaimTypes.Email, user.Email?? ""),
                    new Claim(ClaimTypes.UserData, user.Code!),
                    new Claim(ClaimTypes.WindowsDeviceClaim, loginVM.Device),
                    new Claim(ClaimTypes.Dns, loginVM.IpAddress),
                };
                foreach (var menu in userAuth)
                {
                    claim.Add(new Claim("Menu", menu));
                }


                var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
                });

                //claim = new List<Claim>
                //{
                //    new Claim(ClaimTypes.NameIdentifier, user._ID.ToString()),
                //};

                var jwtToken = new JwtSecurityToken
                    (
                        claims: claim,
                        notBefore: DateTime.UtcNow,
                        expires: DateTime.UtcNow.AddHours(12),
                        //expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: new SigningCredentials(signinKey,SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                });
            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> getUserDetail()
        {
            try
            {

                var handler = new JwtSecurityTokenHandler();
                var keyBytes = Convert.FromBase64String(_configuration["ApplicationSettings:JWT_Secret"]);
                string tokenString = Request.Headers["Authorization"].FirstOrDefault();

                if(string.IsNullOrWhiteSpace(tokenString))
                {
                    throw new CustomHttpException(401, "Please Login First");
                }

                var authHead = AuthenticationHeaderValue.Parse(tokenString);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };

                var principal = handler.ValidateToken(tokenString.Replace("Bearer ","").Trim(), validationParameters, out _);

                var _ID = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var factory = principal.FindFirst(ClaimTypes.Locality)?.Value;
                var user = await _kb3Context.User
                    .FirstOrDefaultAsync(x => x._ID.ToString().Trim() == _ID);
                if (user == null)
                {
                    throw new CustomHttpException(404, "User Code Not found Please Send a DP Request to access this Program");
                }

                return Ok(new
                {
                    UserCode = user.Code,
                    UserName = user.Name + " " + user.Surname,
                    UserEmail = user.Email,
                    Factory = factory,
                });

            }
            catch (Exception ex)
            {
                if (ex is CustomHttpException) throw;
                throw new CustomHttpException(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
