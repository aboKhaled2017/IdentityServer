using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> WeatherStates()
        {
            var apiOneClient = httpClientFactory.CreateClient(Constants.AppsNames.APIOneApp);
            var access_token = await HttpContext.GetTokenAsync("access_token");
            var id_token = await HttpContext.GetTokenAsync("id_token");
            var refresh_token = await HttpContext.GetTokenAsync("refresh_token");

            var accesstoken = new JwtSecurityTokenHandler().ReadJwtToken(access_token);
            var idtoken = new JwtSecurityTokenHandler().ReadJwtToken(id_token);
           // var refreshtoken = new JwtSecurityTokenHandler().ReadJwtToken(refresh_token);

            apiOneClient.SetBearerToken(access_token);
            var dataRes = await apiOneClient.GetAsync($"weatherCases");
            if (!dataRes.IsSuccessStatusCode) return NotFound("cannot access api");
            var data = JsonConvert.DeserializeObject<IEnumerable<string>>(dataRes.Content.ReadAsStringAsync().Result);
            return View(data);
        }

        [Authorize]
        public async Task<IActionResult> addWeather()
        {
            var apiOneClient = httpClientFactory.CreateClient(Constants.AppsNames.APIOneApp);
            var access_token = await HttpContext.GetTokenAsync("access_token");


            apiOneClient.SetBearerToken(access_token);
            var dataRes = await apiOneClient.GetAsync($"addweather?weather=mohamedcool");
            if (!dataRes.IsSuccessStatusCode) return NotFound("cannot access api");
            var data = JsonConvert.DeserializeObject<IEnumerable<string>>(dataRes.Content.ReadAsStringAsync().Result);
            return View(nameof(WeatherStates),data);
        }

        [Authorize(Policy ="get")]
        public string get()
        {
            return "you can get data";
        }

        [Authorize(Policy = "add")]
        public string add()
        {
            return "you can add data";
        }
        [Authorize(Policy = "update")]
        public string update()
        {
            return "you can update data";
        }

        [Authorize(Policy = "delete")]
        public string delete()
        {
            return "you can delete data";
        }

        [Authorize(Policy = "get_and_add")]
        public string both()
        {
            return "you can add and get data";
        }

        [Authorize(Policy = "admin")]
        public string admin()
        {
            return "only admin role can go here";
        }
    }
}
