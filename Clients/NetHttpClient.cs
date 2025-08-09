using NetClient.Interfaces;
using NetClient.Models;
using System.Text.Json;

namespace NetClient.Clients
{
    public class NetHttpClient : INetHttpClient
    {
        private readonly HttpClient _httpClient;

        public NetHttpClient(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL with optional headers, json options and cancellation token."/>.
        /// </summary>
        /// <returns>
        /// A task of type HttpResponse that represents the asynchronous GET operation.  
        /// </returns>
        public async Task<HttpResponse<T>> GetAsync<T>(string url, IDictionary<string, string?>? headers = null,
             JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default)
        {
            if (jsonOptions is null)
            {
                jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (headers is not null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                using var response = await _httpClient.SendAsync(request, cancellationToken);

                var result = new HttpResponse<T>
                {
                    StatusCode = (int)response.StatusCode,
                    Result = response.IsSuccessStatusCode,
                    Message = response.ReasonPhrase
                };

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result.Data = await JsonSerializer.DeserializeAsync<T>(stream, jsonOptions, cancellationToken: cancellationToken);
                }
                else
                {
                    result.Data = default;
                }

                return result;
            }
            catch (Exception)
            {
                return new HttpResponse<T>
                {
                    Result = false,
                    StatusCode = 500,
                    Message = "An unexpected error occurred.",
                    Data = default
                };
            }
        }
    }
}
