using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Compliance.Classification;
using LGPD.Redact.Core;

namespace EZ.Redact.Lgpd.Json.SystemTextJson;

public static class LGPDRedactModifier
{
    public static Action<JsonTypeInfo> Create(ILGPDRedactService redactService)
    {
        ArgumentNullException.ThrowIfNull(redactService);

        return jsonTypeInfo =>
        {
            if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
                return;

            foreach (var property in jsonTypeInfo.Properties)
            {
                var provider = property.AttributeProvider;
                if (provider == null)
                    continue;

                var attrs = provider.GetCustomAttributes(typeof(DataClassificationAttribute), true);
                if (attrs.Length == 0)
                    continue;

                var attr = (DataClassificationAttribute)attrs[0];

                if (!DadoPessoalMapping.TryGet(attr.Classification, out var dadoPessoal))
                    continue;

                if (property.PropertyType == typeof(string))
                {
                    var originalGet = property.Get;
                    if (originalGet == null) continue;

                    property.Get = obj =>
                    {
                        var value = (string?)originalGet(obj);
                        return value != null ? redactService.Redact(dadoPessoal, value) : null;
                    };
                }
                else
                {
                    var genericConverter = typeof(RedactingConverter<>)
                        .MakeGenericType(property.PropertyType);

                    property.CustomConverter = (JsonConverter)Activator.CreateInstance(
                        genericConverter, dadoPessoal, redactService)!;
                }
            }
        };
    }
}
