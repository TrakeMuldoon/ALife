using System.Text.Json;
using System.Text.Json.Serialization;

namespace ALife.Core.Utility;

/// <summary>
/// Helpers for dealing with JSON objects
/// </summary>
public static class JsonHelpers
{
    /// <summary>
    /// The default json options to use during serialization and deserialization.
    /// </summary>
    public static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, IncludeFields = true };

    /// <summary>
    /// Deserializes the contents.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="contents">The contents.</param>
    /// <param name="serializerOptions">The serializer options to use.</param>
    /// <returns>The deserialized object, or null if deserialization fails.</returns>
    /// <exception cref="System.IO.InvalidDataException">Failed to deserialize contents: {contents}.</exception>
    public static T? DeserializeContents<T>(string contents, JsonSerializerOptions? serializerOptions = null)
    {
        if(string.IsNullOrWhiteSpace(contents))
        {
            return default;
        }

        JsonSerializerOptions options = serializerOptions ?? DefaultJsonOptions;
        T? output = JsonSerializer.Deserialize<T>(contents, options);
        return output;
    }

    /// <summary>
    /// Deserializes the file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="serializerOptions">The serializer options to use.</param>
    /// <returns>The deserialized object</returns>
    public static T? DeserializeFile<T>(string file, JsonSerializerOptions? serializerOptions = null)
    {
        string contents = File.ReadAllText(file);
        return DeserializeContents<T>(contents, serializerOptions);
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="serializerOptions">The serializer options.</param>
    /// <returns>The serialized object.</returns>
    public static string SerializeObject(object obj, JsonSerializerOptions? serializerOptions = null)
    {
        JsonSerializerOptions options = serializerOptions ?? DefaultJsonOptions;
        string contents = JsonSerializer.Serialize(obj, options);
        return contents;
    }

    /// <summary>
    /// Writes the object to file.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="serializerOptions">The serializer options to use.</param>
    public static void WriteObjectToFile<T>(T obj, string filePath, JsonSerializerOptions? serializerOptions = null)
    {
        JsonSerializerOptions options = serializerOptions ?? DefaultJsonOptions;
        string contents = JsonSerializer.Serialize(obj, options);
        File.WriteAllText(filePath, contents);
    }
}
