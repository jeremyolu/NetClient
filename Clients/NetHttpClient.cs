using NetClient.Interfaces;
using NetClient.Models;
using System.Reflection.PortableExecutable;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;

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
            jsonOptions = SetSerializerOptions(jsonOptions);

            try
            {
                using var response = await SendRequest(HttpMethod.Get, url, null, headers, jsonOptions, cancellationToken);

                var result = SetHttpResponse<T>(response);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    result.Data = await JsonSerializer.DeserializeAsync<T>(stream, jsonOptions, cancellationToken: cancellationToken);
                }

                return result;
            }
            catch (Exception)
            {
                return SetHttpExceptionResponse<T>();
            }
        }

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with optional headers, json options and cancellation token."/>.
        /// </summary>
        /// <returns>
        /// A task of type HttpResponse that represents the asynchronous POST operation.  
        /// </returns>
        public async Task<HttpResponse<T>> PostAsync<T>(string url, object? body = null, 
            IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default)
        {
            jsonOptions = SetSerializerOptions(jsonOptions);

            try
            {
                using var response = await SendRequest(HttpMethod.Post, url, body, headers, jsonOptions, cancellationToken);

                var result = SetHttpResponse<T>(response);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        result.Data = JsonSerializer.Deserialize<T>(jsonString, jsonOptions);
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return SetHttpExceptionResponse<T>();
            }
        }

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with optional headers, json options and cancellation token."/>.
        /// </summary>
        /// <returns>
        /// A task of type HttpResponse that represents the asynchronous PUT operation.  
        /// </returns>
        public async Task<HttpResponse<T>> PutAsync<T>(string url, object? body = null,
            IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default)
        {
            jsonOptions = SetSerializerOptions(jsonOptions);

            try
            {
                using var response = await SendRequest(HttpMethod.Put, url, body, headers, jsonOptions, cancellationToken);

                var result = SetHttpResponse<T>(response);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        result.Data = JsonSerializer.Deserialize<T>(jsonString, jsonOptions);
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return SetHttpExceptionResponse<T>();
            }
        }

        private JsonSerializerOptions SetSerializerOptions(JsonSerializerOptions? jsonOptions)
        {
            if (jsonOptions is null)
            {
                jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
            }
            return jsonOptions;
        }

        public HttpResponse<T> SetHttpResponse<T>(HttpResponseMessage response)
        {
            return new HttpResponse<T>
            {
                StatusCode = (int)response.StatusCode,
                Result = response.IsSuccessStatusCode,
                Message = response.ReasonPhrase
            };
        }

        public HttpResponse<T> SetHttpExceptionResponse<T>()
        {
            return new HttpResponse<T>
            {
                Result = false,
                StatusCode = 500,
                Message = "An unexpected error occurred.",
                Data = default
            };
        }

        private async Task<HttpResponseMessage> SendRequest(HttpMethod method, string? url, 
            object? body,IDictionary<string, string?>? headers, 
            JsonSerializerOptions? jsonOptions, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, url);

            if (headers is not null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (body is not null)
            {
                if (body is IDictionary<string, string> dict)
                {
                    request.Content = new FormUrlEncodedContent(dict);
                }
                else
                {
                    var json = JsonSerializer.Serialize(body, jsonOptions);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
