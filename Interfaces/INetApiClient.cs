using NetClient.Models;
using System.Text.Json;

namespace NetClient.Interfaces
{
    public interface INetApiClient
    {
        Task<HttpResponse<T>> GetAsync<T>(string url, IDictionary<string, string?>? headers = null,
            JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    }
}
