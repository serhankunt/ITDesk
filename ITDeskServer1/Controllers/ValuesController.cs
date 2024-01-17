using ITDeskServer.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITDeskServer.Controllers
{
   
    
    public sealed class ValuesController : ApiController
    {
        [HttpGet]
        //[AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new { Message = "Api çalışıyor!" });
        }
    }
}
