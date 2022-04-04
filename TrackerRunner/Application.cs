using System.Collections.Generic;
using System.Linq;
using Statens_Vegvesen.Api.Responses;

namespace TrackerRunner;

public class Application
{
    private readonly IAppointmentProvider _appointmentProvider;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly INotificationMessageBuilder _notificationMessageBuilder;

    public Application(IAppointmentProvider appointmentProvider, INotificationMessageBuilder notificationMessageBuilder,
        INotificationDispatcher notificationDispatcher)
    {
        _appointmentProvider = appointmentProvider;
        _notificationMessageBuilder = notificationMessageBuilder;
        _notificationDispatcher = notificationDispatcher;
    }

    public void Run()
    {
        IEnumerable<Appointment> availableAppointments = _appointmentProvider.FetchAppointments();

        if (availableAppointments == null)
            return;

        string notificationBody = _notificationMessageBuilder.CreateNotificationMessage(availableAppointments);

        _notificationDispatcher.SendNotification(notificationBody);
    }
}