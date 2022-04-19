using System;

namespace Shared.Extensions;

public static class EpochTimeExtensions
{
    public static DateTime UnixSecondsToDateTime(this long timestamp, bool local = false)
    {
        var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return local ? offset.LocalDateTime : offset.UtcDateTime;
    }
    public static DateTime UnixMillisecondsToDateTime(this long timestamp, bool local = false)
    {
        var offset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        return local ? offset.LocalDateTime : offset.UtcDateTime;
    }
}