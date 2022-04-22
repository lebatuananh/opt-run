using Newtonsoft.Json;

namespace RunOtp.Driver.RentOtp;

public class RentCodeNumberResponse
{
    [JsonProperty("phone")] public string Phone { get; set; }
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("balance")] public int Balance { get; set; }
    [JsonProperty("otp")] public string Otp { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
}