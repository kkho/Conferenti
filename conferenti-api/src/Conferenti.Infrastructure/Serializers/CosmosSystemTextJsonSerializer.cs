using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace Conferenti.Infrastructure.Serializers;

public class CosmosSystemTextJsonSerializer(JsonSerializerOptions jsonSerializerOptions) : CosmosSerializer
{

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream is { CanSeek: true, Length: 0 })
            {
                return default!;
            }

            if (typeof(Stream).IsAssignableTo(typeof(T)))
            {
                return (T)(object)stream;
            }

            return JsonSerializer.Deserialize<T>(stream, jsonSerializerOptions);
        }
    }

    public override Stream ToStream<T>(T input)
    {
        var streamPayload = new MemoryStream();
        JsonSerializer.Serialize(streamPayload, input, jsonSerializerOptions);
        streamPayload.Position = 0;
        return streamPayload;
    }
}