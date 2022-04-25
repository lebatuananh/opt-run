using Newtonsoft.Json;

namespace RunOtp.Driver.RentOtp;

public class RentCodeNumberResponse
{
    [JsonProperty("phone")] public string Phone { get; set; }
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("balance")] public int Balance { get; set; }
    [JsonProperty("otp")] public string Otp { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("description")] public string Description { get; set; }

    public const string StatusSuccess = "Thành công";
    public const string StatusError = "Thất bại";
    public const string StatusProcessing = "Đang chờ OTP";
    public const string NoPhoneMessage = "Không tìm thấy số này  !";
}