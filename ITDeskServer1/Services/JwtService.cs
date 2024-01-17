using ITDeskServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITDeskServer.Services
{
    public sealed class JwtService
    {
        public string CreateToken(AppUser appUser,List<string?>? roles,bool rememberMe)
        {
            string token = string.Empty;
            //Claim bizim taşınacak bodymiz. Bu sayede ben tokenda değer oluşturabiliyorum
            List<Claim> claims = new();
            claims.Add(new("UserId",appUser.Id.ToString()));
            claims.Add(new("Name", appUser.GetName()));
            claims.Add(new("Email", appUser.Email ?? string.Empty));
            claims.Add(new("UserName", appUser.UserName ?? string.Empty));
            if(roles is not null) claims.Add(new("Roles", string.Join(",", roles)));
           
            DateTime expires = rememberMe ? DateTime.Now.AddMonths(1) : DateTime.Now.AddDays(1);//Tokenın geçerli olacağı süre

            JwtSecurityToken jwtSecurityToken = new(
                issuer:"Serhan Kunt",
                audience:"IT Desk Angular App",
                claims:claims,
                notBefore:DateTime.Now,
                expires:expires,
                signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my secret key my secret key my secret key 1234 ...my secret key my secret key my secret key 1234 ...")),SecurityAlgorithms.HmacSha512));//GetBytes'daki şifreyi HmacSha512 şekliyle kilitler.
            
            JwtSecurityTokenHandler handler = new();//handler verileri aldıktan sonra token oluşturan class
            token  = handler.WriteToken(jwtSecurityToken);

            

            return token;
        }
    }
}
