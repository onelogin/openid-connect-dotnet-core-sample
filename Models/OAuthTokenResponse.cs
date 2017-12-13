using System;
using Newtonsoft.Json;

namespace OidcSampleApp.Models
{
    public class OAuthTokenResponse
    {
      [JsonProperty("access_token")]
      public string AccessToken { get; set; }
      [JsonProperty("refresh_token")]
      public string RefeshToken { get; set; }
      [JsonProperty("token_type")]
      public string TokenType { get; set; }
      [JsonProperty("expires_in")]
      public string ExpiresIn { get; set; }
      [JsonProperty("created_at")]
      public DateTime CreatedAt { get; set; }
    }
}