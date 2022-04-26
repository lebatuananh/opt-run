namespace RunOtp.Driver;

public static class ClientConstant
{
    public static string ClientName = "RunOtp";

    public static class OtpTextNow
    {
        public static string ApiKey = "f594205fad24df601d88a0fe8eb14ad6";
        public static string Url = "http://otptextnow.com";
        public static string Endpoint = "/api";
    }
    
    public static class RentOtp
    {
        public static string ApiKey = "0eff02d8ec432d558c2aeb58d7620d764da44771";
        public static string Url = "http://thuecodetextnow.com";
        public static string Endpoint = "/api";
    }
    

    public static class RunOtp
    {
        public static string ApiKey = "3fa8443577cfae1bcf51954bd814d6a5";
        public static string Url = "http://runotp.com";
        public static string Endpoint = "/api.php";
        public static string Processing = "0";
        public static string Success = "1";
        public static string Cancel = "2";
    }
}