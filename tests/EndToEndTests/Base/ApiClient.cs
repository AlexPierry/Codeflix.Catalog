using System.Text;
using System.Text.Json;

namespace EndToEndTests.Base;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
        where TOutput : class
    {
        var response = await _httpClient
            .PostAsync(
                route,
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"));

        var outputString = await response.Content.ReadAsStringAsync();

        TOutput? output = null;
        if (!string.IsNullOrEmpty(outputString))
        {
            output = JsonSerializer.Deserialize<TOutput>(outputString, _jsonSerializerOptions);
        }

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route)
        where TOutput : class
    {
        var response = await _httpClient.GetAsync(route);

        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrEmpty(outputString))
        {
            output = JsonSerializer.Deserialize<TOutput>(outputString, _jsonSerializerOptions);
        }

        return (response, output);
    }
}