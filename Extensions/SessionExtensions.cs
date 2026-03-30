using System.Text.Json;

namespace WebStore.Extensions;

/// <summary>
/// Helper methods to store cart objects in ASP.NET Core session as JSON.
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    /// Serializes and stores a value in session.
    /// </summary>
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        var json = JsonSerializer.Serialize(value);// switch Data to JSON string
        session.SetString(key, json);// store JSON string in session
    }

    /// <summary>
    /// Reads and deserializes a value from session.
    /// Returns default when key does not exist.
    /// </summary>
    public static T? GetObject<T>(this ISession session, string key)
    {
        var json = session.GetString(key);// read JSON string from session
        return json is null ? default : JsonSerializer.Deserialize<T>(json);// switch JSON string back to Data
    }
}
