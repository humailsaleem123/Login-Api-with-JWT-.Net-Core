using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;

namespace Login.Models
{
    public class JwtService
    {

        public string SecretKey { get; set; }

        public int tokenDuration { get; set; }

        private readonly IConfiguration _config;



        public JwtService(IConfiguration configuration)
        {
            _config = configuration;

            this.SecretKey = _config.GetSection("jwtConfig").GetSection("KEY").Value;

            this.tokenDuration = Int32.Parse(_config.GetSection("jwtConfig").GetSection("DURATION").Value);


        }


        public string GenerateToken(string ID , string username) 
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.SecretKey));

            var signature = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var payload = new[]
            {
                new Claim("id", ID),
                new Claim("username", username),
            };

            var jwtToken = new JwtSecurityToken(

                issuer: "localhost",
                audience: "localhost",
                claims: payload,
                expires: DateTime.Now.AddMinutes(tokenDuration),
                signingCredentials: signature


                );


            return new JwtSecurityTokenHandler().WriteToken(jwtToken);


        }


    }
}
