
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;


namespace ITDeskSignInResultNameSpace;

public  class CheckResultService : ControllerBase
{
    public IActionResult PasswordResult(Microsoft.AspNetCore.Identity.SignInResult result, DateTimeOffset? lockOutEnd)
    {
        if (result.IsLockedOut)
        {
            TimeSpan? timeSpan = lockOutEnd - DateTime.UtcNow;
            if (timeSpan is not null)
            {
                return BadRequest(new { Message = $"Kullanıcınız 3 kere yanlış şifre girişinden dolayı {Math.Ceiling(timeSpan.Value.TotalMinutes)} dakika kadar kilitlenmiştir." });
            }
            else
            {
                return BadRequest(new { Message = "Kullanıcınız 3 kere yanlış şifre girişinden dolayı 1 dakika kadar kilitlenmiştir." });
            }

        }

        if (result.IsNotAllowed)
        {
            return BadRequest(new { Message = "mail adresiniz onaylı değil." });
        }
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Şifreniz yanlış" });
            return BadRequest(new { Message = $"{DateTime.UtcNow}" });
        }

        return Ok();
    }
}
