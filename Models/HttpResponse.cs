namespace NetClient.Models
{
    public class HttpResponse<T>
    {
        public bool Result { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
