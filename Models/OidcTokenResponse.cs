using System;
using Newtonsoft.Json;

namespace OidcSampleApp.Models
{
    public class OidcTokenResponse
    {
      [JsonProperty("access_token")]
      public string AccessToken { get; set; }
      [JsonProperty("refreshToken")]
      public string RefeshToken { get; set; }
      [JsonProperty("token_type")]
      public string TokenType { get; set; }
      [JsonProperty("expires_in")]
      public string ExpiresIn { get; set; }
    }
}