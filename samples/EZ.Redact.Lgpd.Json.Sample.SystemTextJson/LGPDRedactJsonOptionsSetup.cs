using LGPD.Redact.Core;
using EZ.Redact.Lgpd.Json.SystemTextJson;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;

namespace EZ.Redact.Lgpd.Json.Sample.SystemTextJson;

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
