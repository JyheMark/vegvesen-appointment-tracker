using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using Statens_Vegvesen.Api.Responses;
using TrackerRunner;
using Xunit;

namespace Tests;

public class AppointmentProviderTests
{
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly Mock<IHttpClientWrapper> _client;
    private readonly Mock<IRetrieveAppointmentsRequestBuilder> _retrieveAppointmentsRequestBuilder;
    private readonly AppointmentProvider _sut;
    private readonly HttpResponseMessage _testHttpResponseMessage;

    public AppointmentProviderTests()
    {
        _client = new Mock<IHttpClientWrapper>();
        _retrieveAppointmentsRequestBuilder = new Mock<IRetrieveAppointmentsRequestBuilder>();

        _applicationConfiguration = new ApplicationConfiguration
        {
            TrafficStationIds = new[] { "1" }
        };

        _testHttpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(new List<Appointment>
                {
                    new()
                    {
                        Location = "TestLocation",
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddHours(1),
                        Language = "Test",
                        TrafficStationId = 1
                    },
                    new()
                    {
                        Location = "TestLocation",
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddHours(1),
                        Language = "Test",
                        TrafficStationId = 1
                    }
                }
            )
        };

        _client.Setup(p => p.SendRequest(It.IsAny<HttpRequestMessage>())).Returns(_testHttpResponseMessage);

        _sut = new AppointmentProvider(_client.Object, _applicationConfiguration, _retrieveAppointmentsRequestBuilder.Object);
    }

    [Fact]
    public void FetchAppointments_should_return_collection_of_appointments()
    {
        IEnumerable<Appointment>? results = _sut.FetchAppointments();

        results.Should().BeAssignableTo<IEnumerable<Appointment>>();
        results.Should().HaveCount(2);
        results.First().Location.Should().Be("TestLocation");
        results.First().StartDate.Should().Be(DateTime.Today);
        results.First().EndDate.Should().Be(DateTime.Today.AddHours(1));
        results.First().TrafficStationId.Should().Be(1);
    }

    [Fact]
    public void FetchAppointsments_should_return_if_SendRequest_returns_unsuccessful_response()
    {
        _client.Setup(p => p.SendRequest(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        IEnumerable<Appointment>? results = _sut.FetchAppointments();

        results.Should().BeNull();
    }

    [Fact]
    public void FetchAppointments_should_return_empty_if_no_availabilities()
    {
        _testHttpResponseMessage.Content = JsonContent.Create(Array.Empty<Appointment>());

        IEnumerable<Appointment>? results = _sut.FetchAppointments();

        results.Should().BeAssignableTo<IEnumerable<Appointment>>();
        results.Should().BeEmpty();
    }

    [Fact]
    public void FetchAppointments_should_call_RetrieveAppointmentsRequestBuilder_for_each_traffic_station()
    {
        _sut.FetchAppointments();

        _retrieveAppointmentsRequestBuilder.Verify(p => p.CreateHttpRequestMessage(It.IsAny<string>()), Times.Exactly(_applicationConfiguration.TrafficStationIds.Count()));
    }

    [Fact]
    public void FetchAppointments_should_pass_parameters_to_RetrieveAppointmentsRequestBuilder()
    {
        _sut.FetchAppointments();

        foreach (string trafficStationId in _applicationConfiguration.TrafficStationIds)
            _retrieveAppointmentsRequestBuilder.Verify(p => p.CreateHttpRequestMessage(It.Is<string>(i => i == trafficStationId)), Times.AtLeastOnce);
    }

    [Fact]
    public void FetchAppointments_should_call_httpclient()
    {
        var testRequestMessage = new HttpRequestMessage(HttpMethod.Get, "testuri");

        _retrieveAppointmentsRequestBuilder.Setup(p => p.CreateHttpRequestMessage(It.IsAny<string>())).Returns(testRequestMessage);

        _sut.FetchAppointments();

        _client.Verify(p => p.SendRequest(It.Is<HttpRequestMessage>(hrm => hrm.Equals(testRequestMessage))), Times.Exactly(_applicationConfiguration.TrafficStationIds.Count()));
    }
}