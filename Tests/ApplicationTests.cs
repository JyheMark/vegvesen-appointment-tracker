using System;
using System.Collections.Generic;
using Moq;
using Statens_Vegvesen.Api.Responses;
using TrackerRunner;
using Xunit;

namespace Tests;

public class ApplicationTests
{
    private readonly Mock<IAppointmentProvider> _appointmentProvider;
    private readonly Mock<INotificationDispatcher> _notificationDispatcher;
    private readonly Mock<INotificationMessageBuilder> _notificationMessageBuilder;
    private readonly Application _sut;
    private readonly List<Appointment> _testAppointmentResponse;
    private readonly string _testNotificationMessageResponse;

    public ApplicationTests()
    {
        _appointmentProvider = new Mock<IAppointmentProvider>();
        _notificationMessageBuilder = new Mock<INotificationMessageBuilder>();
        _notificationDispatcher = new Mock<INotificationDispatcher>();

        _testAppointmentResponse = new List<Appointment>
        {
            new()
            {
                Location = "TestLocation",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddHours(1),
                Language = "TestLang",
                TrafficStationId = 123
            }
        };

        _testNotificationMessageResponse = "This is a test notification";

        _notificationMessageBuilder.Setup(p => p.CreateNotificationMessage(It.IsAny<IEnumerable<Appointment>>())).Returns(_testNotificationMessageResponse);
        _appointmentProvider.Setup(p => p.FetchAppointments()).Returns(_testAppointmentResponse);

        _sut = new Application(_appointmentProvider.Object, _notificationMessageBuilder.Object, _notificationDispatcher.Object);
    }

    [Fact]
    public void Should_request_available_appointments()
    {
        _sut.Run();

        _appointmentProvider.Verify(p => p.FetchAppointments());
    }

    [Fact]
    public void Should_build_notification_message()
    {
        _sut.Run();

        _notificationMessageBuilder.Verify(p => p.CreateNotificationMessage(It.Is<IEnumerable<Appointment>>(a => Equals(a, _testAppointmentResponse))));
    }

    [Fact]
    public void Should_send_notification()
    {
        _sut.Run();

        _notificationDispatcher.Verify(p => p.SendNotification(It.Is<string>(n => n.Equals(_testNotificationMessageResponse))));
    }

    [Fact]
    public void Should_return_if_no_appointments()
    {
        _appointmentProvider.Setup(p => p.FetchAppointments()).Returns(Array.Empty<Appointment>());

        _sut.Run();

        _notificationMessageBuilder.Verify(p => p.CreateNotificationMessage(It.IsAny<IEnumerable<Appointment>>()), Times.Never);
        _notificationDispatcher.Verify(p => p.SendNotification(It.IsAny<string>()), Times.Never);
    }
}