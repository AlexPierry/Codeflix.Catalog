using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

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

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route, object? parameterObject = null)
        where TOutput : class
    {
        var query = PrepareQuery(route, parameterObject);
        var response = await _httpClient.GetAsync(query);

        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrEmpty(outputString))
        {
            output = JsonSerializer.Deserialize<TOutput>(outputString, _jsonSerializerOptions);
        }

        return (response, output);
    }

    private string? PrepareQuery(string route, object? parameterObject)
    {
        if (parameterObject is null)
        {
            return route;
        }

        var parametersJson = JsonSerializer.Serialize(parameterObject);
        var parametersDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(parametersJson);

        return QueryHelpers.AddQueryString(route, parametersDictionary!);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route)
        where TOutput : class
    {
        var response = await _httpClient.DeleteAsync(route);

        var outputString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;
        if (!string.IsNullOrEmpty(outputString))
        {
            output = JsonSerializer.Deserialize<TOutput>(outputString, _jsonSerializerOptions);
        }

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload)
        where TOutput : class
    {
        var response = await _httpClient
            .PutAsync(
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
}