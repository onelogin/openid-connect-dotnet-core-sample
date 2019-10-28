using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OidcSampleApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace OidcSampleApp.Controllers
{    public class AccountController : Controller
    {
        private IOptions<OidcOptions> options;

      public AccountController(IOptions<OidcOptions> options)
      {
        this.options = options;
      }

      [HttpGet]
      [AllowAnonymous]
      public async Task<IActionResult> Login(string returnUrl = null)
      {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync();

        ViewData["ReturnUrl"] = returnUrl;
        return View();
      }

      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Login(LoginViewModel loginModel, string returnUrl = "/home/dashboard")
      {
        ViewData["ReturnUrl"] = returnUrl;

        var token = await LoginUser(loginModel.Username, loginModel.Password);

        if(!String.IsNullOrEmpty(token.AccessToken))
        {
          // We need to call OIDC again to get the user claims
          var user = await GetUserInfo(token.AccessToken);

          var claims = new List<Claim>
          {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("onelogin-access-token", token.AccessToken)
          };

          var userIdentity = new ClaimsIdentity(claims, "login");

          ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
          await HttpContext.SignInAsync(principal);

          //Just redirect to our index after logging in.
          return Redirect(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Login Failed");
        return View(loginModel);
      }

      [HttpGet]
      public async Task<IActionResult> Logout()
      {
        await LogoutUser(User.FindFirstValue("onelogin-access-token"));
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
      }

      private async Task<OidcTokenResponse> LoginUser(string username, string password)
      {
        using(var client = new HttpClient()){

          // The Token Endpoint Authentication Method must be set to POST if you
          // want to send the client_secret in the POST body.
          // If Token Endpoint Authentication Method then client_secret must be
          // combined with client_id and provided as a base64 encoded string
          // in a basic authorization header.
          // e.g. Authorization: basic <base64 encoded ("client_id:client_secret")>
          var formData = new FormUrlEncodedContent(new[]
          {
              new KeyValuePair<string, string>("username", username),
              new KeyValuePair<string, string>("password", password),
              new KeyValuePair<string, string>("client_id", options.Value.ClientId),
              new KeyValuePair<string, string>("client_secret", options.Value.ClientSecret),
              new KeyValuePair<string, string>("grant_type", "password"),
              new KeyValuePair<string, string>("scope", "openid profile email")
          });

          var uri = String.Format("https://{0}.onelogin.com/oidc/token", options.Value.Region);

          var res = await client.PostAsync(uri, formData);

          var json = await res.Content.ReadAsStringAsync();

          var tokenReponse = JsonConvert.DeserializeObject<OidcTokenResponse>(json);

          return tokenReponse;
        }
      }

      private async Task<bool> LogoutUser(string accessToken)
      {
        using(var client = new HttpClient()){

          // The Token Endpoint Authentication Method must be set to POST if you
          // want to send the client_secret in the POST body.
          // If Token Endpoint Authentication Method then client_secret must be
          // combined with client_id and provided as a base64 encoded string
          // in a basic authorization header.
          // e.g. Authorization: basic <base64 encoded ("client_id:client_secret")>
          var formData = new FormUrlEncodedContent(new[]
          {
              new KeyValuePair<string, string>("token", accessToken),
              new KeyValuePair<string, string>("token_type_hint", "access_token"),
              new KeyValuePair<string, string>("client_id", options.Value.ClientId),
              new KeyValuePair<string, string>("client_secret", options.Value.ClientSecret)
          });

          var uri = String.Format("https://{0}.onelogin.com/oidc/token/revocation", options.Value.Region);

          var res = await client.PostAsync(uri, formData);

          return res.IsSuccessStatusCode;
        }
      }

      private async Task<User> GetUserInfo(string accessToken)
      {
        using(var client = new HttpClient()){

          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

          var uri = String.Format("https://{0}.onelogin.com/oidc/me", options.Value.Region);

          var res = await client.GetAsync(uri);

          var json = await res.Content.ReadAsStringAsync();

          return JsonConvert.DeserializeObject<User>(json);
        }
      }
    }
}
