using System;
using System.Net.Http;
using Common;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World mohamed!");
            // doStuff1();
            doStuff2();
            Console.ReadKey();
        }


        public static void doStuff1()
        {
            var client = new HttpClient();
            var discoveryDoc = client.GetDiscoveryDocumentAsync(Constants.IdentityServerBaseAdress).Result;
            var tokenRes = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDoc.TokenEndpoint,
                ClientId = "consolApp_id",
                ClientSecret = "consolApp_secret",
                Scope = "ApiOne"
            }).Result;
            if (tokenRes.IsError)
            {
                Console.WriteLine("cannot get token");
                Console.WriteLine(tokenRes.Error);
            }
            else
            {
                

                var accessToken = tokenRes.AccessToken;
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

                var apiClient = new HttpClient();
                apiClient.SetBearerToken(accessToken);
                var dataRes = apiClient.GetAsync($"{Constants.APIOneBaseAdress}consolWeather").Result;
                var jsonData = dataRes.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<IEnumerable<string>>(jsonData);
                foreach (var item in data)
                {
                    Console.WriteLine(item);
                }

            }

        }
        public static void doStuff2()
        {
            var client = new HttpClient();
            var discoveryDoc = client.GetDiscoveryDocumentAsync(Constants.IdentityServerBaseAdress).Result;
            try
            {
                var tokenRes = new HttpClient()
                .RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = discoveryDoc.TokenEndpoint,
                    ClientId = "consolApp_id",
                    ClientSecret = "consolApp_secret"
                }).ConfigureAwait(true).GetAwaiter().GetResult();
                Console.WriteLine(tokenRes.RefreshToken);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
