// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 08-16-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-16-2018
// ***********************************************************************
// <copyright file="ProviderNotaCarioca.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class ProviderNotaCarioca : ProviderABRASF
    {
        #region Constructors

        public ProviderNotaCarioca(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Nota Carioca";
        }

        #endregion Constructors

        #region Methods

        protected override XElement WriteInfoRPS(NotaFiscal nota)
        {
            var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

            string naturezaOperacao;
            switch (nota.NaturezaOperacao)
            {
                case NaturezaOperacao.NT01:
                    naturezaOperacao = "1";
                    break;

                case NaturezaOperacao.NT02:
                    naturezaOperacao = "2";
                    break;

                case NaturezaOperacao.NT03:
                    naturezaOperacao = "3";
                    break;

                case NaturezaOperacao.NT04:
                    naturezaOperacao = "4";
                    break;

                case NaturezaOperacao.NT05:
                    naturezaOperacao = "5";
                    break;

                case NaturezaOperacao.NT06:
                    naturezaOperacao = "6";
                    break;

                default:
                    naturezaOperacao = "0";
                    break;
            }

            string regimeEspecialTributacao;
            string optanteSimplesNacional;
            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            {
                regimeEspecialTributacao = "";
                optanteSimplesNacional = "1";
            }
            else
            {
                var regime = (int)nota.RegimeEspecialTributacao;
                regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
                optanteSimplesNacional = "2";
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

            var infoRps = new XElement("InfRps", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero}"));

            infoRps.Add(WriteIdentificacao(nota));
            infoRps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, naturezaOperacao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

            return infoRps;
        }

        public override RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nenhuma RPS informada." });
            if (notas.Count > 1) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas uma RPS pode ser enviada em modo Sincrono." });
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append("<GerarNfseEnvio xmlns=\"http://notacarioca.rio.gov.br/WSNacional/XSD/1/nfse_pcrj_v01.xsd\">");

            var xmlRps = WriteXmlRps(notas[0], false, false);
            XmlSigning.AssinarXml(xmlRps, "Rps", "InfRps", Certificado);
            GravarRpsEmDisco(xmlRps, $"Rps-{notas[0].IdentificacaoRps.DataEmissao:yyyyMMdd}-{notas[0].IdentificacaoRps.Numero}.xml", notas[0].IdentificacaoRps.DataEmissao);

            xmlLote.Append(xmlRps);
            xmlLote.Append("</GerarNfseEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-sinc-{lote}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.EnviarSincrono));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.EnviarSincrono))
                {
                    retornoWebservice.XmlRetorno = cliente.GerarNfse(GerarCabecalho(), retornoWebservice.XmlEnvio);
                    retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                    retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"lote-sinc-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "GerarNfseResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var compNfse = xmlRet.ElementAnyNs("GerarNfseResposta")?.ElementAnyNs("CompNfse");
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

            retornoWebservice.Sucesso = true;
            return retornoWebservice;
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new NotaCariocaServiceClient(this, tipo);
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_pcrj_v01.xsd";
        }

        #endregion Methods
    }
}