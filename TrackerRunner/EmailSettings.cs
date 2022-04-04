namespace TrackerRunner;

public sealed record EmailSettings
{
    public string FromAddress { get; init; }
    public string ToAddress { get; init; }
    public string FromAccountPassword { get; init; }
    public SmtpSettings SmtpSettings { get; init; }
}