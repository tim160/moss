using System;
using Newtonsoft.Json;


namespace EC.Models.SSO
{
  public class ApiResponse<T>
  {
    [JsonProperty("data")]
    public T Data { get; set; }
  }
}