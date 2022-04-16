using Newtonsoft.Json;

namespace RunOtp.Driver.OtpTextNow;

public class NumberResponse
{
    [JsonProperty("number")] public string Number { get; set; }
    [JsonProperty("request_id")] public string RequestId { get; set; }
}

public class OtpCodeResponse
{
    [JsonProperty("otp_code")] public string OtpCode { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
}