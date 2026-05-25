using EZ.Redact.Lgpd.Json.NewtonsoftJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EZ.Redact.Lgpd.Json.Sample.NewtonsoftJson;

internal sealed class LGPDRedactNewtonsoftOptionsSetup : IConfigureOptions<MvcNewtonsoftJsonOptions>
{
    private readonly LGPDRedactContractResolver _resolver;

    public LGPDRedactNewtonsoftOptionsSetup(LGPDRedactContractResolver resolver)
        => _resolver = resolver;

    public void Configure(MvcNewtonsoftJsonOptions options)
    {
        options.SerializerSettings.ContractResolver = _resolver;
    }
}
