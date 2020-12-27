using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using IdentityModel.Client;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ApiTwo.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class HomeController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<HomeController> _logger;
        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        //[Authorize]
        public async Task<IActionResult> states()
        {
            //retrieve access token
            var idpClient = httpClientFactory.CreateClient(Constants.AppsNames.IdentityServerApp);
            var discoveryDoc = await idpClient.GetDiscoveryDocumentAsync();
            var tokenRes = await idpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address=discoveryDoc.TokenEndpoint,
                ClientId = "ApiTwoClient_id",
                ClientSecret = "ApiTwoSecret",
                Scope="ApiOne"
            });
            if (tokenRes.IsError)
            {
                return Ok(new
                {
                    status = "erro cannot get token",
                    message = tokenRes.Error
                });
            }
            //retrieve secret weather data
            var apiOneClient = httpClientFactory.CreateClient(Constants.AppsNames.APIOneApp);
            apiOneClient.SetBearerToken(tokenRes.AccessToken);
            var weatherCases = await apiOneClient.GetAsync($"{apiOneClient.BaseAddress}weathercases");
            var data = JsonConvert.DeserializeObject<IEnumerable<string>>(weatherCases.Content.ReadAsStringAsync().Result);
            return Ok(new { 
            weathercases=data,
            accessToken=tokenRes.AccessToken
            });
        }
    }
}
