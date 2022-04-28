namespace RunOtp.Driver;

public static class ClientConstant
{
    public static string ClientName = "RunOtp";

    public static class OtpTextNow
    {
        public static readonly string ApiKey = "f594205fad24df601d88a0fe8eb14ad6";
        public static readonly string Url = "http://otptextnow.com";
        public static readonly string Endpoint = "/api";
    }

    public static class RentOtp
    {
        public static readonly string ApiKey = "0eff02d8ec432d558c2aeb58d7620d764da44771";
        public static readonly string Url = "http://thuecodetextnow.com";
        public static readonly string Endpoint = "/api";
    }


    public static class RunOtp
    {
        public static readonly string ApiKey = "3fa8443577cfae1bcf51954bd814d6a5";
        public static readonly string Url = "http://runotp.com";
        public static readonly string Endpoint = "/api.php";
        public static readonly string Processing = "0";
        public static readonly string ProcessingMessage = "Đang chờ OTP";
        public static readonly string Timeout = "4";
        public static readonly string TimeoutMessage = "Hết thời gian";
        public static readonly string Success = "1";
        public static readonly string SuccessMessage = "Thành công";
        public static readonly string Cancel = "2";
        public static readonly string CancelMessage = "Đã hủy";
    }
}