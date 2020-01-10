using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    internal sealed class ProviderBeloHorizonte : ProviderABRASF
    {
        #region Constructors

        public ProviderBeloHorizonte(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Porto Alegre";//Utilizar mesmos arquivos de schema de Porto Alegre
            //Name = "Belo Horizonte";
        }

        #endregion Constructors

        #region Methods

        private string gerarStringLoteRps(int lote, NotaFiscalCollection notas, bool assinar, bool salvarEmDisco)
        {
            string XmlEnvio = string.Empty;
            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = GetXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                if (salvarEmDisco)
                    GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //xmlLote.Append($"<GerarNfseEnvio {GetNamespace()}>");
            xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");//provider.GetNamespace()

            //xmlLote.Append($"<LoteRps Id=\"L{lote}\">");
            xmlLote.Append($"<LoteRps Id=\"lote\" versao=\"1.00\" {GetNamespace()}>");
            xmlLote.Append($"<NumeroLote>{lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            //xmlLote.Append("</GerarNfseEnvio>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            XmlEnvio = xmlLote.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
                XmlEnvio = XmlEnvio.RemoveAccent();

            if (assinar)
            {
                XmlEnvio = XmlSigning.AssinarXmlTodos(XmlEnvio, "Rps", "InfRps", Certificado);
                //XmlEnvio = XmlSigning.AssinarXml(XmlEnvio, "GerarNfseEnvio", "LoteRps", Certificado);
                XmlEnvio = XmlSigning.AssinarXml(XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
            }

            return XmlEnvio;
        }

        public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            //if (notas.Count > 3) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas 3 RPS podem ser enviados em modo Sincrono." });
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            retornoWebservice.XmlEnvio = gerarStringLoteRps(lote, notas, true, true);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-{lote}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.Enviar));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.Enviar))
                {
                    retornoWebservice.XmlRetorno = cliente.RecepcionarLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"lote-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "EnviarLoteRpsResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.DataLote = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.NumeroLote.IsEmpty();

            if (!retornoWebservice.Sucesso) return retornoWebservice;

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (NotaFiscal nota in notas)
            {
                nota.NumeroLote = retornoWebservice.NumeroLote;
            }
            
            return retornoWebservice;
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new BeloHorizonteServiceClient(this, tipo);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.abrasf.org.br/nfse.xsd\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_v20_08_2015.xsd";
        }

        public RetornoWebservice ValidarSchemaEnvio(int lote, NotaFiscalCollection notas, bool assinar)
        {
            var retorno = new RetornoWebservice();
            retorno.XmlEnvio = gerarStringLoteRps(lote, notas, assinar, false);

            ValidarSchema(retorno, GetSchema(TipoUrl.EnviarSincrono));
            return retorno;
        }


        #endregion Methods
    }
}