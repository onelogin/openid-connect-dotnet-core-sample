using System;
using Newtonsoft.Json;

namespace OidcSampleApp.Models
{
    public class UserApp
    {
      [JsonProperty("id")]
      public int Id { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("icon")]
      public string Icon { get; set; }

      [JsonProperty("provisioned")]
      public int? Provisioned { get; set; }

      [JsonProperty("extension")]
      public bool Extension { get; set; }

      [JsonProperty("login_id")]
      public string LoginId { get; set; }

      [JsonProperty("personal")]
      public bool Personal { get; set; }
    }
}