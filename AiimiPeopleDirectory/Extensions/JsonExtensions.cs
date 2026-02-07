namespace AiimiPeopleDirectory.Extensions;

using System.Text.Json;

/**
 * Any extensions relating to JSON
 */
public static class JsonExtensions
{

    /**
     * Cache and reuse our JsonSerializerOptions as per
     * <see href="https://learn.microsoft.com/en-gb/dotnet/fundamentals/code-analysis/quality-rules/ca1869"/>
     */
    private static readonly JsonSerializerOptions DefaultReadOptions = new()
    {
        // Because we use lowerCamelCase for property names in our JSON file and UpperCamelCase for DTOs, we configure
        // our JSON serializer to be case-insensitive so we can stick to our respective JSON and C# case conventions
        PropertyNameCaseInsensitive = true
    };

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, DefaultReadOptions);
    }
}
