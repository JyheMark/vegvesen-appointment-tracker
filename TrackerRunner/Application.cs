using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Statens_Vegvesen.Api.Responses;

namespace TrackerRunner;

public class Application
{
    private readonly IAppointmentProvider _appointmentProvider;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly INotificationMessageBuilder _notificationMessageBuilder;
    private readonly ILogger<Application> _logger;

    public Application(ILogger<Application> logger, IAppointmentProvider appointmentProvider, INotificationMessageBuilder notificationMessageBuilder,
        INotificationDispatcher notificationDispatcher)
    {
        _logger = logger;
        _appointmentProvider = appointmentProvider;
        _notificationMessageBuilder = notificationMessageBuilder;
        _notificationDispatcher = notificationDispatcher;
    }

    public void Run()
    {
        try
        {
            IEnumerable<Appointment> availableAppointments = _appointmentProvider.FetchAppointments();

            ArgumentNullException.ThrowIfNull(availableAppointments);

            string notificationBody = _notificationMessageBuilder.CreateNotificationMessage(availableAppointments);

            if (string.IsNullOrWhiteSpace(notificationBody))
                throw new ArgumentNullException(nameof(notificationBody));

            _notificationDispatcher.SendNotification(notificationBody);
        }
        catch (Exception ex)
        {
            _notificationDispatcher.SendNotification($"Fatal error: {ex.Message}");
            _logger.LogError(ex, "Fatal error");
        }
    }
}