using EZ.Redact.Lgpd.Core.Attributes;

namespace EZ.Redact.Lgpd.Json.Tests.Models;

public class Pessoa
{
    [CPFData]
    public string? Documento { get; set; }

    [EmailData]
    public string? Email { get; set; }

    [TelefoneData]
    public string? Telefone { get; set; }

    [NomeData]
    public string? Nome { get; set; }

    [EnderecoData]
    public string? Endereco { get; set; }

    [GuidData]
    public Guid Chave { get; set; }

    [PixData]
    public Guid ChavePix { get; set; }

    public string? SemAtributo { get; set; }
}
