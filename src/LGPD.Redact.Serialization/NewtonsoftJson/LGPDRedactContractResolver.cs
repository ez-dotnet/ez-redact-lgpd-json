using System.Reflection;
using Microsoft.Extensions.Compliance.Classification;
using LGPD.Redact.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LGPD.Redact.Serialization.NewtonsoftJson;

public class LGPDRedactContractResolver : DefaultContractResolver
{
    private readonly ILGPDRedactService _redactService;

    public LGPDRedactContractResolver(ILGPDRedactService redactService)
    {
        _redactService = redactService ?? throw new ArgumentNullException(nameof(redactService));
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        var attr = member.GetCustomAttribute<DataClassificationAttribute>();
        if (attr == null)
            return property;

        if (!DadoPessoalMapping.TryGet(attr.Classification, out var dadoPessoal))
            return property;

        if (property.PropertyType == typeof(string))
        {
            var originalProvider = property.ValueProvider;
            property.ValueProvider = new StringRedactingValueProvider(
                originalProvider, _redactService, dadoPessoal);
        }
        else
        {
            property.Converter = new ToStringRedactingConverter(dadoPessoal, _redactService);
        }

        return property;
    }

    private sealed class StringRedactingValueProvider : IValueProvider
    {
        private readonly IValueProvider _inner;
        private readonly ILGPDRedactService _redactService;
        private readonly DadoPessoal _dadoPessoal;

        public StringRedactingValueProvider(IValueProvider? inner, ILGPDRedactService redactService, DadoPessoal dadoPessoal)
        {
            _inner = inner!;
            _redactService = redactService;
            _dadoPessoal = dadoPessoal;
        }

        public object? GetValue(object target)
        {
            var value = _inner.GetValue(target);
            if (value is string str)
                return _redactService.Redact(_dadoPessoal, str);
            return value;
        }

        public void SetValue(object target, object? value)
        {
            _inner.SetValue(target, value);
        }
    }

    private sealed class ToStringRedactingConverter : JsonConverter
    {
        private readonly DadoPessoal _dadoPessoal;
        private readonly ILGPDRedactService _redactService;

        public ToStringRedactingConverter(DadoPessoal dadoPessoal, ILGPDRedactService redactService)
        {
            _dadoPessoal = dadoPessoal;
            _redactService = redactService;
        }

        public override bool CanConvert(Type objectType) => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var str = value.ToString();
            if (str != null)
            {
                var redacted = _redactService.Redact(_dadoPessoal, str);
                writer.WriteValue(redacted);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }
    }
}
