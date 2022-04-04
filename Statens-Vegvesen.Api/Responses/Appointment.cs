using System.Text.Json.Serialization;

namespace Statens_Vegvesen.Api.Responses;

public sealed record Appointment
{
    [JsonPropertyName("oppmotested")] public string Location { get; init; }

    [JsonPropertyName("start")] public DateTime StartDate { get; init; }

    [JsonPropertyName("ferdig")] public DateTime EndDate { get; init; }

    [JsonPropertyName("sprakkode")] public string Language { get; init; }

    [JsonPropertyName("trafikkstasjonId")] public int TrafficStationId { get; init; }
}