using EZ.Redact.Lgpd.Core;
using EZ.Redact.Lgpd.Json;
using EZ.Redact.Lgpd.Json.Sample.SystemTextJson;
using EZ.Redact.Lgpd.Json.Sample.SystemTextJson.Models;
using EZ.Redact.Lgpd.Json.SystemTextJson;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLGPDRedaction()
                .AddSerialization();
builder.Logging.EnableRedaction(options => options.ApplyDiscriminator = false);

builder.Services.AddTransient<IConfigureOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>, LGPDRedactJsonOptionsSetup>();

var app = builder.Build();

// Manual: configuração direta no endpoint
app.MapGet("/stj-manual", (ILGPDRedactService redact) =>
{
    var options = new JsonSerializerOptions
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            .WithAddedModifier(LGPDRedactModifier.Create(redact)),
        WriteIndented = true
    };

    return Results.Content(
        JsonSerializer.Serialize(new Pessoa(), options),
        "application/json");
});

// Global: via IConfigureOptions<JsonOptions>
app.MapGet("/stj-global", (IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions> jsonOptions) =>
    Results.Content(
        JsonSerializer.Serialize(new Pessoa(), jsonOptions.Value.SerializerOptions),
        "application/json"));

// Sem redação (controle)
app.MapGet("/raw", () =>
    Results.Content(
        JsonSerializer.Serialize(new Pessoa(), new JsonSerializerOptions { WriteIndented = true }),
        "application/json"));

app.Run();
