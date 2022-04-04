namespace TrackerRunner;

public sealed record SmtpSettings
{
    public string Host { get; init; }
    public int Port { get; init; }
}