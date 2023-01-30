using System.Text.Json;

namespace DependenSee.Core.ResultTransformers;

/// <summary>
/// Transforms a <see cref="DiscoveryResult"/> into JSON
/// </summary>
public static class JsonTransformer
{
    private static readonly JsonSerializerOptions _JsonSerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Transforms a <see cref="DiscoveryResult"/> into JSON
    /// </summary>
    public static string Transform(DiscoveryResult result) 
        => JsonSerializer.Serialize(result, _JsonSerializerOptions);
}
