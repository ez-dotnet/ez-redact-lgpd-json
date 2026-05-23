using LGPD.Redact.Core;
using EZ.Redact.Lgpd.Json.NewtonsoftJson;
using EZ.Redact.Lgpd.Json.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EZ.Redact.Lgpd.Json.Tests.NewtonsoftJson;

public class LGPDRedactContractResolverTest
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

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new LGPDRedactContractResolver(redactService)
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

        var json = JsonConvert.SerializeObject(pessoa, settings);
        var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        Assert.Equal("123.***.***-09", obj!["Documento"]);
        Assert.Equal("j***@email.com", obj["Email"]);
        Assert.Equal("(11) 9 ****-4444", obj["Telefone"]);
        Assert.Equal("J*** S****", obj["Nome"]);
        Assert.Equal("R** d** F*****, *** - S** P*******", obj["Endereco"]);
        Assert.Equal("1234****-****-****-****-********9abc", obj["Chave"]);
        Assert.Equal("1234****-****-****-****-****56789abc", obj["ChavePix"]);
        Assert.Equal("texto normal", obj["SemAtributo"]);
    }

    [Fact]
    public void Deve_Redatar_Propriedade_Nula_Sem_Erro()
    {
        var redactService = CreateRedactService();

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new LGPDRedactContractResolver(redactService),
            NullValueHandling = NullValueHandling.Include
        };

        var pessoa = new Pessoa
        {
            Documento = null,
            Nome = null,
        };

        var json = JsonConvert.SerializeObject(pessoa, settings);
        var obj = JsonConvert.DeserializeObject<Dictionary<string, object?>>(json);

        Assert.Null(obj!["Documento"]);
        Assert.Null(obj["Nome"]);
    }

    [Fact]
    public void Deve_Manter_Propriedades_Sem_Atributo()
    {
        var redactService = CreateRedactService();

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new LGPDRedactContractResolver(redactService)
        };

        var pessoa = new Pessoa
        {
            SemAtributo = "informação pública",
        };

        var json = JsonConvert.SerializeObject(pessoa, settings);
        var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        Assert.Equal("informação pública", obj!["SemAtributo"]);
    }
}
