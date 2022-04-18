using Newtonsoft.Json;

namespace RunOtp.Driver.RunOtp;

public class RunOtpResponse
{
    [JsonProperty("status")] public int Status { get; set; }

    [JsonProperty("results")] public ListCreatedRequestResponse Results { get; set; }
}

public class ListCreatedRequestResponse
{
    [JsonProperty("data")] public List<CreatedRequestResponse> Data { get; set; }
}

public class CreatedRequestResponse
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("sdt")] public string Phone { get; set; }
    [JsonProperty("otp")] public string Otp { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("created_time")] public long CreateTime { get; set; }
    [JsonProperty("requestId")] public string RequestId { get; set; }
}

public class CheckedRequestResponse
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("sdt")] public string Phone { get; set; }
    [JsonProperty("otp")] public string Otp { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("created_time")] public int CreateTime { get; set; }
    [JsonProperty("finishtime")] public int FinishTime { get; set; }
    [JsonProperty("textSMS")] public int TextSms { get; set; }
    [JsonProperty("id")] public string RequestId { get; set; }
}