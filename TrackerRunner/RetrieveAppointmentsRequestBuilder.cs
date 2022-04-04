using System;
using System.Net.Http;
using Statens_Vegvesen.Api;

namespace TrackerRunner;

public interface IRetrieveAppointmentsRequestBuilder
{
    public HttpRequestMessage CreateHttpRequestMessage(string trafficStationId);
}

public class RetrieveAppointmentsRequestBuilder : IRetrieveAppointmentsRequestBuilder
{
    private readonly ApplicationConfiguration _applicationConfiguration;

    public RetrieveAppointmentsRequestBuilder(ApplicationConfiguration applicationConfiguration)
    {
        _applicationConfiguration = applicationConfiguration;
    }

    public HttpRequestMessage CreateHttpRequestMessage(string trafficStationId)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"{Endpoints.SearchAppointments}{trafficStationId}"),
            Method = HttpMethod.Get
        };

        request.Headers.Add("Cookie", _applicationConfiguration.AuthenticationCookie);

        return request;
    }
}