using System.Net.Http;

namespace TrackerRunner;

public interface IHttpClientWrapper
{
    HttpResponseMessage SendRequest(HttpRequestMessage httpRequestMessage);
}

public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _client;

    public HttpClientWrapper()
    {
        _client = new HttpClient();
    }

    public HttpResponseMessage SendRequest(HttpRequestMessage httpRequestMessage)
    {
        return _client.Send(httpRequestMessage);
    }
}