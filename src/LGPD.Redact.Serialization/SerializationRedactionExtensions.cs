using LGPD.Redact.Core;
using LGPD.Redact.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LGPD.Redact.Serialization;

public static class SerializationRedactionExtensions
{
    public static ILGPDRedactionBuilder AddSerialization(this ILGPDRedactionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddSingleton<LGPDRedactContractResolver>();

        return builder;
    }
}
