using System.Text.Json;
using System.Text.Json.Serialization;
using LGPD.Redact.Core;

namespace EZ.Redact.Lgpd.Json.SystemTextJson;

internal sealed class RedactingConverter<T> : JsonConverter<T>
{
    private readonly DadoPessoal _dadoPessoal;
    private readonly ILGPDRedactService _redactService;

    public RedactingConverter(DadoPessoal dadoPessoal, ILGPDRedactService redactService)
    {
        _dadoPessoal = dadoPessoal;
        _redactService = redactService;
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var str = value.ToString();
        if (str != null)
        {
            var redacted = _redactService.Redact(_dadoPessoal, str);
            writer.WriteStringValue(redacted);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
