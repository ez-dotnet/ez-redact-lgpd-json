using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using LGPD.Redact.Core;
using LGPD.Redact.Serialization.SystemTextJson;
using LGPD.Redact.Serialization.UnitTests.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LGPD.Redact.Serialization.UnitTests.SystemTextJson;

public class LGPDRedactModifierTest
{
    private static ILGPDRedactService CreateRedactService()
    {
        var services = new ServiceCollection();
        services.AddLGPDRedaction();
        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<ILGPDRedactService>();
    }

    [Fact]
    public void Deve_Redatar_Propriedades_Com_Atributo()
    {
        var redactService = CreateRedactService();

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(
                LGPDRedactModifier.Create(redactService))
        };

        var pessoa = new Pessoa
        {
            Documento = "123.456.789-09",
            Email = "joao@email.com",
            Telefone = "(11) 9 8888-4444",
            Nome = "João Silva",
            Endereco = "Rua das Flores, 123 - São Paulo/SP",
            Chave = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
            ChavePix = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
            SemAtributo = "texto normal"
        };

        var json = JsonSerializer.Serialize(pessoa, options);
        var obj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        Assert.Equal("123.***.***-09", obj!["Documento"].GetString());
        Assert.Equal("j***@email.com", obj["Email"].GetString());
        Assert.Equal("(11) 9 ****-4444", obj["Telefone"].GetString());
        Assert.Equal("J*** S****", obj["Nome"].GetString());
        Assert.Equal("R** d** F*****, *** - S** P*******", obj["Endereco"].GetString());
        Assert.Equal("1234****-****-****-****-********9abc", obj["Chave"].GetString());
        Assert.Equal("1234****-****-****-****-****56789abc", obj["ChavePix"].GetString());
        Assert.Equal("texto normal", obj["SemAtributo"].GetString());
    }

    [Fact]
    public void Deve_Redatar_Propriedade_Nula_Sem_Erro()
    {
        var redactService = CreateRedactService();

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(
                LGPDRedactModifier.Create(redactService))
        };

        var pessoa = new Pessoa
        {
            Documento = null,
            Nome = null,
        };

        var json = JsonSerializer.Serialize(pessoa, options);
        var obj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        Assert.Equal(JsonValueKind.Null, obj!["Documento"].ValueKind);
        Assert.Equal(JsonValueKind.Null, obj["Nome"].ValueKind);
    }

    [Fact]
    public void Deve_Manter_Propriedades_Sem_Atributo()
    {
        var redactService = CreateRedactService();

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(
                LGPDRedactModifier.Create(redactService))
        };

        var pessoa = new Pessoa
        {
            SemAtributo = "informação pública",
        };

        var json = JsonSerializer.Serialize(pessoa, options);
        var obj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        Assert.Equal("informação pública", obj!["SemAtributo"].GetString());
    }
}
