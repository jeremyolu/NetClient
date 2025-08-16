using NetClient.Models;
using System.Text.Json;

namespace NetClient.Interfaces
{
    public interface INetHttpClient
    {
        Task<HttpResponse<T>> GetAsync<T>(string url, IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);

        Task<HttpResponse<T>> PostAsync<T>(string url, object? body = null, 
            IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null,
           CancellationToken cancellationToken = default);

        Task<HttpResponse<T>> PutAsync<T>(string url, object? body = null,
            IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default);

        Task<HttpResponse<T>> PatchAsync<T>(string url, object? body = null,
            IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default);
    }
}
