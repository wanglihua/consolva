using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace shunda.Core.Json;

public class ShortDateTimeConverter : DateTimeConverterBase
{
    private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        dtConverter.WriteJson(writer, value, serializer);
    }
}