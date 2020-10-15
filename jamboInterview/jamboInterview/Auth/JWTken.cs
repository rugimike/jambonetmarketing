using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace jamboInterview.Auth
{
    public class JWTken
    {

        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }

        public bool validateToken(string token,string mySecret)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
            var myIssuer = "http://localhost:18454";
            var myAudience = "http://localhost:18454";

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey,
                    LifetimeValidator = CustomLifetimeValidator,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public JToken decodeJwt(string jwttoken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadJwtToken(jwttoken);
            /*
            var headers = token.Header;
            var jwtHeader = "{";
            foreach (var h in headers)
            {
                jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
            }*/

            var claims = token.Claims;
            var jwtPayload = "{";
            foreach (Claim c in claims)
            {
                jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
            }
            jwtPayload = jwtPayload.Substring(0, jwtPayload.Length - 1);//remove last comma
            jwtPayload += "}";

            JToken jtoken = JToken.Parse(jwtPayload);
            //string userid = (string)jtoken["nameid"];//working like charm
            return jtoken;
        }
        
    }
}