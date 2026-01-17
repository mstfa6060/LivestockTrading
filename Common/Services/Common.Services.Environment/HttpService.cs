using System.Net;
using System.Text.Json;


namespace Common.Services.Environment;

public enum HttpVerbs
{
    GET = 0,
    POST = 1,
    PUT = 2,
    DELETE = 3,
}

public class HttpService
{
    // private readonly string _jwt;
    private readonly JsonSerializerOptions jsonSeriliazerOptions;
    public HttpService()
    {
        // _jwt = jwt;
        jsonSeriliazerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public async Task<T> Post<T>(string url, object requestPayload = null)
    {
        T response;
        var json = JsonSerializer.Serialize(requestPayload != null ? requestPayload : new object(), jsonSeriliazerOptions);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        using (var client = new HttpClient())
        {
            // client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(jwt);
            HttpResponseMessage httpResponse = await client.PostAsync(url, data);

            string result = await httpResponse.Content.ReadAsStringAsync();
            response = (T)JsonSerializer.Deserialize(result, typeof(T), jsonSeriliazerOptions);
            // TODO: Add try catch
        }
        return response;
    }

    public async Task<T> Get<T>(string url, JsonSerializerOptions options = null)
    {
        options ??= jsonSeriliazerOptions;
        
        T response;
        using var client = new HttpClient();
        HttpResponseMessage httpResponse = await client.GetAsync(url);
        string result = await httpResponse.Content.ReadAsStringAsync();
        response = (T)JsonSerializer.Deserialize(result, typeof(T), options);
        return response;
    }

    // public async Task<T> Call<T>(string url, object payload) where T : class
    // {
    //     // System.Console.WriteLine("===HttpService/Call===");
    //     var client = new HttpClient()
    //     {
    //         Timeout = TimeSpan.FromMinutes(1),
    //     };

    //     // System.Console.WriteLine($"URL : {url}");

    //     // client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);
    //     var json = JsonSerializer.Serialize(payload, jsonSeriliazerOptions);
    //     var data = new StringContent(json, Encoding.UTF8, "application/json");
    //     var response = await client.PostAsync(url, data);
    //     // System.Console.WriteLine($"===StatusCode: {response.StatusCode}");
    //     if (response.StatusCode != HttpStatusCode.OK)
    //         throw new Exception($"Request Failed : {response.StatusCode}");

    //     var result = await response.Content.ReadAsStringAsync();

    //     // System.Console.WriteLine(result);

    //     var blockResponse = JsonSerializer.Deserialize<Arfware.ArfBlocks.Core.ArfBlocksRequestResult>(result, jsonSeriliazerOptions);
    //     if (blockResponse == null || blockResponse.HasError)
    //     {
    //         throw new Exception(blockResponse.Error.Message);
    //     }

    //     return JsonSerializer.Deserialize<T>(blockResponse.Payload.ToString(), jsonSeriliazerOptions);
    // }
}