using System.Collections.Generic;

namespace TrackerRunner;

public sealed record ApplicationConfiguration
{
    public string AuthenticationCookie { get; init; }
    public IEnumerable<string> TrafficStationIds { get; init; }
    public int RepeatInSeconds { get; init; }
    public EmailSettings EmailSettings { get; init; }
}