using System;
using Newtonsoft.Json;

namespace OidcSampleApp.Models
{
    public class User
    {
      [JsonProperty("sub")]
      public int Id { get; set; }

      [JsonProperty("email")]
      public string Email { get; set; }

      [JsonProperty("preferred_username")]
      public string Username { get; set; }

      [JsonProperty("name")]
      public string FullName { get; set; }

      [JsonProperty("given_name")]
      public string FirstName { get; set; }

      [JsonProperty("family_name")]
      public string LastName { get; set; }

      [JsonProperty("updated_at")]
      public DateTime UpdatedAt { get; set; }
    }
}