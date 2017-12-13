using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OidcSampleApp.Models;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace OidcSampleApp.Controllers
{    public class HomeController : Controller
    {
        // These variables are only here for visibility in the sample
        // In production you should move this to a configuration file
        const string ONELOGIN_CLIENT_ID = "your-onelogin-api-client-id";
        const string ONELOGIN_CLIENT_SECRET = "your-onelogin-api-client-secret";
        const string ONELOGIN_REGION = "us or eu";

        const string ONELOGIN_SUBDOMAIN = "your-onelogin-instance-subdomain";

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Message"] = "This is a secured dashboard";

            // Get the OneLogin user id for the current user
            var oneLoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get a list of apps for this user
            var apps = await GetAppsForUser(oneLoginUserId);
            ViewData["Apps"] = apps;

            // Used for launching apps from the dashboard
            ViewData["Subdomain"] = ONELOGIN_SUBDOMAIN;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Private

        private async Task<List<UserApp>> GetAppsForUser(string userId){
            using(var client = new HttpClient()){

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetAccessToken());

                var res = await client.GetAsync(String.Format("https://api.{0}.onelogin.com/api/1/users/{1}/apps", ONELOGIN_REGION, userId));

                var json = await res.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<UserApp>>>(json);

                return apiResponse.Data;
            }
        }

        private string GetAccessToken(){
            using(var client = new HttpClient()){

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", String.Format(
                    "client_id:{0}, client_secret:{1}", ONELOGIN_CLIENT_ID, ONELOGIN_CLIENT_SECRET));

                var body = JsonConvert.SerializeObject(new {
                    grant_type = "client_credentials"
                });

                var req = new HttpRequestMessage(){
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(String.Format("https://api.{0}.onelogin.com/auth/oauth2/v2/token",ONELOGIN_REGION)),
                    Content = new StringContent(body)
                };

                // We add the Content-Type Header like this because otherwise dotnet
                // adds the utf-8 charset extension to it which is not compatible with OneLogin
                req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var res = client.SendAsync(req).Result;

                var json = res.Content.ReadAsStringAsync().Result;

                var tokenReponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

                return tokenReponse.AccessToken;
            }
        }

        #endregion
    }
}
