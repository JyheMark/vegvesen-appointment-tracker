using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Statens_Vegvesen.Api;
using TrackerRunner;
using Xunit;

namespace Tests;

public class RetrieveAppointmentsRequestBuilderTests
{
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly RetrieveAppointmentsRequestBuilder _sut;

    public RetrieveAppointmentsRequestBuilderTests()
    {
        _applicationConfiguration = new ApplicationConfiguration
        {
            AuthenticationCookie = "testauth"
        };

        _sut = new RetrieveAppointmentsRequestBuilder(_applicationConfiguration);
    }

    [Fact]
    public void Should_return_HttpRequestMessage()
    {
        const string trafficStationId = "123";
        HttpRequestMessage? httpRequestMessage = _sut.CreateHttpRequestMessage(trafficStationId);

        httpRequestMessage.Should().BeOfType<HttpRequestMessage>();
        httpRequestMessage.RequestUri?.AbsoluteUri.Should().Be($"{Endpoints.SearchAppointments}{trafficStationId}");
        httpRequestMessage.Method.Should().Be(HttpMethod.Get);
        httpRequestMessage.Headers.Should().ContainKey("Cookie");
        httpRequestMessage.Headers.GetValues("Cookie").FirstOrDefault().Should().Be("testauth");
    }
}