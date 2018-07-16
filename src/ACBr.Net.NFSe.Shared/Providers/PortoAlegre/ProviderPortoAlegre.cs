// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-16-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderPortoAlegre.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers.PortoAlegre
{
    internal sealed class ProviderPortoAlegre : ProviderABRASF
    {
        #region Constructors

        public ProviderPortoAlegre(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Porto Alegre";
        }

        #endregion Constructors

        #region Methods

        public override RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (notas.Count > 3) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas 3 RPS podem ser enviados em modo Sincrono." });
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = GetXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append($"<GerarNfseEnvio {GetNamespace()}>");
            xmlLote.Append($"<LoteRps Id=\"L{lote}\">");
            xmlLote.Append($"<NumeroLote>{lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</GerarNfseEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfRps", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "GerarNfseEnvio", "LoteRps", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-{lote}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, GetSchema(TipoUrl.EnviarSincrono));
            if (retSchema != null) return retSchema;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.EnviarSincrono))
                {
                    retornoWebservice.XmlRetorno = cliente.GerarNfse(GerarCabecalho(), retornoWebservice.XmlEnvio);
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
            MensagemErro(retornoWebservice, xmlRet, "GerarNfseResposta");
            MensagemErro(retornoWebservice, xmlRet, "GerarNfseResposta", "ListaMensagemRetornoLote");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.DataLote = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.NumeroLote.IsEmpty();

            if (!retornoWebservice.Sucesso) return retornoWebservice;

            var retornoLote = xmlRet.ElementAnyNs("GerarNfseResposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return retornoWebservice;
            }

            retornoWebservice.Sucesso = true;

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
                var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    notas.Load(compNfse.ToString());
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = chaveNFSe;
                }
            }

            return retornoWebservice;
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new PortoAlegreServiceClient(this, tipo);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.abrasf.org.br/nfse.xsd\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_v20_08_2015.xsd";
        }

        #endregion Methods
    }
}