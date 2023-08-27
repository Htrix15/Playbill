using System.Text.Json;

namespace Models.Billboards.Common.Extension;

public static class ParseJsonExtension
{
    private static readonly JsonSerializerOptions _options = new () { PropertyNameCaseInsensitive = true };
    public static IList<TResponse?> ParseJsons<TResponse>(this IList<string> responses,
        JsonSerializerOptions? options = null) 
        => responses.Select(respons => JsonSerializer.Deserialize<TResponse>(respons, options ?? _options)).ToList();

    public static TResponse ParseJson<TResponse>(this string respons, JsonSerializerOptions? options = null)
    =>  JsonSerializer.Deserialize<TResponse>(respons, options ?? _options);
}
