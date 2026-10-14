namespace Peace_BankingAPI.WebAPI.Models
{
    public sealed class ApiErrorResponse
    {
        public string Type { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public int StatusCode { get; init; }
        public string? TraceId { get; init; }
        public IDictionary<string, string[]>? Errors { get; init; }
    }
}
