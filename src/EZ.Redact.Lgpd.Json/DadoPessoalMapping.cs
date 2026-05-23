using Microsoft.Extensions.Compliance.Classification;
using EZ.Redact.Lgpd.Core;

namespace EZ.Redact.Lgpd.Json;

internal static class DadoPessoalMapping
{
    // Keys devem coincidir com os valores usados em LGPDTaxonomy
    private static readonly Dictionary<string, DadoPessoal> s_map = new()
    {
        ["CPF"] = DadoPessoal.CPF,
        ["CNPJ"] = DadoPessoal.CNPJ,
        ["Nome"] = DadoPessoal.Nome,
        ["Endereco"] = DadoPessoal.Endereco,
        ["Telefone"] = DadoPessoal.Telefone,
        ["Email"] = DadoPessoal.Email,
        ["CartaoCredito"] = DadoPessoal.CartaoCredito,
        ["CEP"] = DadoPessoal.CEP,
        ["Guid"] = DadoPessoal.Guid,
        ["PIX"] = DadoPessoal.Pix,
        ["EnderecoIP"] = DadoPessoal.EnderecoIP,
        ["MacAddress"] = DadoPessoal.MacAddress,
        ["Geolocalizacao"] = DadoPessoal.Geolocalizacao,
        ["CNH"] = DadoPessoal.CNH,
        ["TituloEleitor"] = DadoPessoal.TituloEleitor,
        ["Placa"] = DadoPessoal.Placa,
        ["Renavam"] = DadoPessoal.Renavam,
        ["PIS"] = DadoPessoal.PIS,
        ["CNS"] = DadoPessoal.CNS,
        ["CTPS"] = DadoPessoal.CTPS,
        ["Certidao"] = DadoPessoal.Certidao,
        ["DataGenerica"] = DadoPessoal.DataGenerica,
        ["ContaBancaria"] = DadoPessoal.ContaBancaria,
        ["Passaporte"] = DadoPessoal.Passaporte,
        ["RNE"] = DadoPessoal.RNE,
    };

    public static bool TryGet(DataClassification classification, out DadoPessoal result)
    {
        if (classification.TaxonomyName == "LGPD")
            return s_map.TryGetValue(classification.Value, out result);

        result = default;
        return false;
    }
}
