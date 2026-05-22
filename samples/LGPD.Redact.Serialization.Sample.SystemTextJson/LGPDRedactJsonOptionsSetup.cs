using LGPD.Redact.Core;
using LGPD.Redact.Serialization.SystemTextJson;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;

namespace LGPD.Redact.Serialization.Sample.SystemTextJson;

internal sealed class LGPDRedactJsonOptionsSetup : IConfigureOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>
{
    private readonly ILGPDRedactService _redactService;

    public LGPDRedactJsonOptionsSetup(ILGPDRedactService redactService)
        => _redactService = redactService;

    public void Configure(Microsoft.AspNetCore.Http.Json.JsonOptions options)
    {
        options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            .WithAddedModifier(LGPDRedactModifier.Create(_redactService));
    }
}
