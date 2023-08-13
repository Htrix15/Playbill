using System.Text.Json;

namespace Playbill.Billboards.Common.Extension;

public static class ParseJsonExtension
{
    private static readonly JsonSerializerOptions _options = new () { PropertyNameCaseInsensitive = true };
    public static IList<TResponse?> ParseJsons<TResponse>(this IList<string> responses,
        JsonSerializerOptions? options = null) 
        => responses.Select(respons => JsonSerializer.Deserialize<TResponse>(respons, options ?? _options)).ToList();
}
