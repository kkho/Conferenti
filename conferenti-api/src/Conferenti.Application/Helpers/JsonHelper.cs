using System.Text.Json;

namespace Conferenti.Application.Helpers;

internal static class JsonHelper
{
    public static readonly JsonSerializerOptions IndentedJsonSerializerOptions = new() { WriteIndented = true };

    public static readonly JsonSerializerOptions WebJsonSerializerOptions =
        new JsonSerializerOptions(JsonSerializerDefaults.Web);
}