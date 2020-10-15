using jamboInterview.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace jamboInterview.Auth
{
    public class CreatetokenController : Controller
    {
        [HttpPost]
        public string GenerateToken(AUser iuser)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(iuser.secret.ToString()));
            var myIssuer = "http://localhost:18454";
            var myAudience = "http://localhost:18454";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                  new Claim(ClaimTypes.NameIdentifier,iuser.userid.ToString()),
                }),
                //Expires = DateTime.UtcNow.AddSeconds(4000),
                Expires = DateTime.UtcNow.AddDays(10),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}