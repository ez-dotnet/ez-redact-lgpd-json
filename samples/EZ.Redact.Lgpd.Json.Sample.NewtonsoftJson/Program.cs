using LGPD.Redact.Core;
using EZ.Redact.Lgpd.Json;
using EZ.Redact.Lgpd.Json.NewtonsoftJson;
using EZ.Redact.Lgpd.Json.Sample.NewtonsoftJson;
using EZ.Redact.Lgpd.Json.Sample.NewtonsoftJson.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLGPDRedaction()
                .AddSerialization();
builder.Logging.EnableRedaction(options => options.ApplyDiscriminator = false);

builder.Services.AddTransient<IConfigureOptions<MvcNewtonsoftJsonOptions>, LGPDRedactNewtonsoftOptionsSetup>();

var app = builder.Build();

// Manual: configuração direta no endpoint
app.MapGet("/newtonsoft-manual", (LGPDRedactContractResolver resolver) =>
{
    var settings = new JsonSerializerSettings
    {
        ContractResolver = resolver,
        Formatting = Formatting.Indented
    };

    return Results.Content(
        JsonConvert.SerializeObject(new Pessoa(), settings),
        "application/json");
});

// Global: via IConfigureOptions<MvcNewtonsoftJsonOptions>
app.MapGet("/newtonsoft-global", (IOptions<MvcNewtonsoftJsonOptions> options) =>
    Results.Content(
        JsonConvert.SerializeObject(new Pessoa(), new JsonSerializerSettings
        {
            ContractResolver = options.Value.SerializerSettings.ContractResolver,
            Formatting = Formatting.Indented
        }),
        "application/json"));

// Sem redação (controle)
app.MapGet("/raw", () =>
    Results.Content(
        JsonConvert.SerializeObject(new Pessoa(), new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        }),
        "application/json"));

app.Run();
