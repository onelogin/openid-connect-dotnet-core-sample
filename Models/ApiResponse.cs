using System;
using Newtonsoft.Json;

namespace OidcSampleApp.Models
{
    public class ApiResponse<T>
    {
      [JsonProperty("data")]
      public T Data { get; set; }
    }
}