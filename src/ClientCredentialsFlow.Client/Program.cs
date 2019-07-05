using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ClientCredentialsFlow.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // discover endpoints from metadata
            DiscoveryResponse disco = await discoverIdentityAPI();

            TokenResponse tokenResponse = await askIdentityAPIForBearerToken(disco);

            await callTheProduceAPI(tokenResponse.AccessToken);
        }

        private static async Task<DiscoveryResponse> discoverIdentityAPI()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
            }

            Console.WriteLine("Discovered....\n" + disco.Json);
            return disco;
        }

        private static async Task<TokenResponse> askIdentityAPIForBearerToken(DiscoveryResponse disco)
        {
            var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "ProduceAPI"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            return tokenResponse;
        }

        private static async Task callTheProduceAPI(string accessToken)
        {
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("https://localhost:5005/api/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
            
            
            var values = await client.GetAsync("https://localhost:5005/api/values");
            if (!values.IsSuccessStatusCode)
            {
                Console.WriteLine(values.StatusCode);
            }
            else
            {
                var content = await values.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}