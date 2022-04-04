using System.Net;
using System.Net.Mail;

namespace TrackerRunner;

public interface INotificationDispatcher
{
    public void SendNotification(string notificationBody);
}

public class EmailNotificationService : INotificationDispatcher
{
    private readonly ApplicationConfiguration _applicationConfiguration;

    private readonly string _fromAddress;
    private readonly SmtpClient _smtpClient;
    private readonly string _subject;
    private readonly string _toAddress;

    public EmailNotificationService(ApplicationConfiguration applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;

        _fromAddress = applicationConfiguration.EmailSettings.FromAddress;
        _toAddress = applicationConfiguration.EmailSettings.ToAddress;
        _subject = "Vegvesen appointment availabilities";

        string fromPassword = applicationConfiguration.EmailSettings.FromAccountPassword;

        _smtpClient = new SmtpClient
        {
            Host = applicationConfiguration.EmailSettings.SmtpSettings.Host,
            Port = applicationConfiguration.EmailSettings.SmtpSettings.Port,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_fromAddress, fromPassword)
        };
    }

    public void SendNotification(string notificationBody)
    {
        SendEmail(notificationBody);
    }

    private void SendEmail(string notificationBody)
    {
        using var mailMessage = new MailMessage(_fromAddress, _toAddress)
        {
            Subject = _subject,
            Body = notificationBody
        };

        _smtpClient.Send(mailMessage);
    }
}