using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe.Nota;
using System;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Test
{
    public static class ExemploSigiss
    {
        public static void Inicializacao()
        {
            ACBrNFSe acbrNFSe = new ACBrNFSe();

            //Salvar os arquivos
            acbrNFSe.Configuracoes.Geral.Salvar = false;
            acbrNFSe.Configuracoes.Arquivos.Salvar = false;

            //prestador
            acbrNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "37761587000161";

            //webservices
            //Configure os dados da cidade e do Certificado aqui
            acbrNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Producao;
            acbrNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;

            acbrNFSe.Configuracoes.WebServices.Usuario = "888888";//USUARIO
            acbrNFSe.Configuracoes.WebServices.Senha = "123456";//SENHA

            //acoes (habilite as opcoes abaixo para testar)
            //EmissaoNota(acbrNFSe);

            //CancelarNota(acbrNFSe);
        }


        private static void CancelarNota(ACBrNFSe acbrNFSe)
        {
            try
            {
                //enviando requisicao de cancelamento
                var retorno = acbrNFSe.CancelarNFSe("a@a.com", "7125", "motivo teste testetestetesteteste");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void EmissaoNota(ACBrNFSe acbrNFSe)
        {
            try
            {
                //adicionado rps
                var nota = acbrNFSe.NotasServico.AddNew();
                nota.Prestador.CpfCnpj = "37761587000161";
                nota.RegimeEspecialTributacao = ACBr.Net.NFSe.Nota.RegimeEspecialTributacao.SimplesNacional;
                nota.Servico.Valores.Aliquota = 2;
                nota.Servico.CodigoTributacaoMunicipio = "802";
                nota.NaturezaOperacao = NaturezaOperacao.Sigiss.TributadaNoPrestador;
                nota.Servico.Valores.ValorServicos = 29.91M;
                nota.Servico.Valores.BaseCalculo = 29.91M;
                nota.Servico.Descricao = "serviço teste";
                nota.Tomador.Tipo = TipoTomador.Sigiss.PFNI;
                nota.Tomador.DadosContato.Email = "a@a.com";

                //enviando
                var retorno = acbrNFSe.Enviar(0);

                Console.Clear();
                if (retorno.Sucesso == true)
                {
                    string numeroNota = XDocument.Parse(retorno.XmlRetorno)?.ElementAnyNs("GerarNotaResponse")?.ElementAnyNs("RetornoNota")?.ElementAnyNs("Nota")?.GetValue<string>() ?? string.Empty;
                    string linkNFSe = XDocument.Parse(retorno.XmlRetorno)?.ElementAnyNs("GerarNotaResponse")?.ElementAnyNs("RetornoNota")?.ElementAnyNs("LinkImpressao")?.GetValue<string>() ?? string.Empty;
                    Console.WriteLine("Nota Emitida! | prot:" + retorno.Protocolo + " | n: " + numeroNota);
                    Console.WriteLine(linkNFSe);
                }
                else
                {
                    Console.WriteLine("Erros:");
                    foreach (var item in retorno.Erros)
                    {
                        Console.WriteLine($"{item.Codigo} - {item.Correcao} - {item.Descricao}");
                    }
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
