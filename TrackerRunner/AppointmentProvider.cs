using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Statens_Vegvesen.Api.Responses;

namespace TrackerRunner;

public interface IAppointmentProvider
{
    IEnumerable<Appointment> FetchAppointments();
}

public class AppointmentProvider : IAppointmentProvider
{
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IHttpClientWrapper _client;
    private readonly IRetrieveAppointmentsRequestBuilder _retrieveAppointmentsRequestBuilder;

    public AppointmentProvider(IHttpClientWrapper client, ApplicationConfiguration applicationConfiguration, IRetrieveAppointmentsRequestBuilder retrieveAppointmentsRequestBuilder)
    {
        _client = client;
        _applicationConfiguration = applicationConfiguration;
        _retrieveAppointmentsRequestBuilder = retrieveAppointmentsRequestBuilder;
    }

    public IEnumerable<Appointment> FetchAppointments()
    {
        var allAvailabilities = new List<Appointment>();

        foreach (string trafficStationId in _applicationConfiguration.TrafficStationIds)
        {
            HttpRequestMessage request = _retrieveAppointmentsRequestBuilder.CreateHttpRequestMessage(trafficStationId);
            HttpResponseMessage response = _client.SendRequest(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Responded with {response.StatusCode}: {response.ReasonPhrase}");

            IEnumerable<Appointment> appointments = response.Content.ReadFromJsonAsync<IEnumerable<Appointment>>().Result;

            if (appointments != null && appointments.Any())
                allAvailabilities.AddRange(appointments);
        }

        return allAvailabilities;
    }
}