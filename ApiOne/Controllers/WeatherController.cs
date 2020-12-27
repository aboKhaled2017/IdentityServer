using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiOne.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class WeatherController : ControllerBase
    {
        private static  string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherController> _logger;

        public WeatherController(ILogger<WeatherController> logger)
        {
            _logger = logger;
        }
        public IActionResult welcom()
        {
            return Ok("welcom to weather");
        }
        [Authorize]
        public IActionResult weatherCases([FromServices] IAuthorizationService authorizationService)
        {
            var claims = User.Claims.ToList();
            var policy = new AuthorizationPolicyBuilder("authWeather")
                .RequireClaim("get", "true")
                .Build();
            var res = authorizationService.AuthorizeAsync(User, policy).Result;
            if (res.Succeeded)
                return Ok(Summaries);
            else return Unauthorized("you don't have the clam type [get] with [true] value");
        }
        [Authorize]
        public IActionResult addweather([FromServices] IAuthorizationService authorizationService)
        {
            var claims = User.Claims.ToList();
            var policy = new AuthorizationPolicyBuilder("authWeather")
                .RequireClaim("add", "true")
                .Build();
            var res = authorizationService.AuthorizeAsync(User, policy).Result;
            if (res.Succeeded)
            {
                var val = Request.Query["weather"].ToString();
                if (!string.IsNullOrEmpty(val))
                    Summaries=Summaries.Append(val).ToArray();
                return Ok(Summaries);
            }
            else return Unauthorized("you don't have the clam type [get] with [true] value");
        }

        [Authorize]
        public IActionResult consolWeather()
        {
           
                return Ok(Summaries);
        }
    }
}
