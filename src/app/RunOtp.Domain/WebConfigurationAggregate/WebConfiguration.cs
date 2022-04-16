using Shared.SeedWork;

namespace RunOtp.Domain.WebConfigurationAggregate;

public class WebConfiguration : ModifierTrackingEntity, IAggregateRoot
{
    public string ApiSecret { get; set; }
    public string Url { get; set; }
    public string WebName { get; set; }
    public string Endpoint { get; set; }
    public WebStatus Status { get; set; }

    public WebConfiguration(string apiSecret, string url, string webName, string endpoint)
    {
        ApiSecret = apiSecret;
        Url = url;
        WebName = webName;
        Endpoint = endpoint;
        Status = WebStatus.Active;
    }

    public void Update(string apiSecret, string url, string webName, string endpoint, WebStatus status)
    {
        ApiSecret = apiSecret;
        Url = url;
        WebName = webName;
        Endpoint = endpoint;
        Status = status;
    }
}