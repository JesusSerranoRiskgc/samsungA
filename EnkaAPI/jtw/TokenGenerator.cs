using CapaDTO.Peticiones;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Claims;

namespace EnkaAPI.jtw
{
    public class TokenGenerator
    {
        private readonly IConfiguration _configuration;
        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public object GenerateTokenJwt(LoginRequest Userinfo, DataTable userinfo)
        {
            // appsetting for Token JWT


            var secretKey = _configuration["Jwt:Key"];
            var audienceToken = _configuration["Jwt:Audience"];
            var issuerToken = _configuration["Jwt:Issuer"];


            var expireTime = _configuration["Jwt:JWT_EXPIRE_MINUTES"];

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, Userinfo.email)});

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var identity = GenerateClaimsIdentity(Userinfo.email);
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);

            var token = new { access_token = jwtTokenString, expiration = expireTime, user = Userinfo.email, userId = Convert.ToInt32(userinfo.Rows[0]["Id"]) };
            return token;
        }

        public static ClaimsIdentity GenerateClaimsIdentity(string user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user),
                new Claim(ClaimTypes.Email, user),
                new Claim("role","admin"),

            };
            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
