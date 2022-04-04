using System;
using System.Collections.Generic;
using System.Linq;
using Statens_Vegvesen.Api.Responses;

namespace TrackerRunner;

public interface INotificationMessageBuilder
{
    string CreateNotificationMessage(IEnumerable<Appointment> appointments);
}

public class NotificationMessageBuilder : INotificationMessageBuilder
{
    private readonly ApplicationConfiguration _configuration;


    public NotificationMessageBuilder(ApplicationConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateNotificationMessage(IEnumerable<Appointment> appointments)
    {
        ArgumentNullException.ThrowIfNull(appointments);

        var message = string.Empty;

        message += $"Message generated: {DateTime.Now}\n\n";
        message += "For traffic stations:\n";

        foreach (string trafficStation in _configuration.TrafficStationIds) message += $"{trafficStation}\n";

        message += "\n\nAvailabilities: \n\n";

        if (!appointments.Any())
        {
            message += "No availabilities";
            return message;
        }

        var appointmentsByLocation = appointments.GroupBy(p => p.TrafficStationId,
            (trafficStationId, groupedAppointments) => new { TrafficStationId = trafficStationId, Appointments = groupedAppointments });

        foreach (var location in appointmentsByLocation)
        {
            message += $"{location.Appointments.First().Location}\n";
            foreach (Appointment availability in location.Appointments) message += $"-> {availability.StartDate}\n";
            message += "\n\n";
        }

        return message;
    }
}