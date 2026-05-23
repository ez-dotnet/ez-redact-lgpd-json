using LGPD.Redact.Core;
using EZ.Redact.Lgpd.Json.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EZ.Redact.Lgpd.Json;

public static class SerializationRedactionExtensions
{
    public static ILGPDRedactionBuilder AddSerialization(this ILGPDRedactionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddSingleton<LGPDRedactContractResolver>();

        return builder;
    }
}
