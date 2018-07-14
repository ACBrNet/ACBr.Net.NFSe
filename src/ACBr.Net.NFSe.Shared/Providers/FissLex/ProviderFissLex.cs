// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 12-27-2017
//
// Last Modified By : RFTD
// Last Modified On : 12-27-2017
// ***********************************************************************
// <copyright file="ProviderFissLex.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.FissLex
{
    // ReSharper disable once InconsistentNaming
    internal sealed class ProviderFissLex : ProviderABRASF
    {
        #region Fields

        private static readonly string[] escapedCharacters = { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };
        private static readonly string[] unescapedCharacters = { "&", "<", ">", "\"", "\'" };

        #endregion Fields

        #region Constructors

        public ProviderFissLex(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "FissLex";
        }

        #endregion Constructors

        #region Services

        public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = GetXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<EnviarLoteRpsEnvio>");
            xmlLote.Append($"<LoteRps Id=\"L{lote}\">");
            xmlLote.Append($"<NumeroLote>{lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            var xmlEnvio = xmlLote.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                xmlEnvio = xmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(xmlEnvio, "Rps", "InfRps", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, GetSchema(TipoUrl.Enviar));
            if (retSchema != null) return retSchema;

            retornoWebservice.XmlEnvio = "<WS_RecepcionarLoteRps.Execute xmlns:fiss=\"FISS-LEX\">" +
                                        $"<fiss:Enviarloterpsenvio>{AjustarEnvio(retornoWebservice.XmlEnvio)}</fiss:Enviarloterpsenvio>" +
                                         "</WS_RecepcionarLoteRps.Execute>";

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-{lote}-env.xml");

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
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));

            var errorElement = xmlRet.ElementAnyNs("WS_RecepcionarLoteRps.ExecuteResponse")
                ?.ElementAnyNs("Enviarloterpsresposta");
            MensagemErro(retornoWebservice, errorElement, "EnviarLoteRpsResposta");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

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

        public override RetornoWebservice ConsultarLoteRps(int lote, string protocolo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<WS_ConsultaLoteRps.Execute xmlns:fiss=\"FISS-LEX\">");
            loteBuilder.Append("<fiss:Consultarloterpsenvio>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</fiss:Consultarloterpsenvio>");
            loteBuilder.Append("</WS_ConsultaLoteRps.Execute>");
            var xmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                xmlEnvio = xmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = xmlEnvio;
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, GetSchema(TipoUrl.ConsultarLoteRps));
            if (retSchema != null) return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetClient(TipoUrl.ConsultarLoteRps);
                var cabecalho = GerarCabecalho();
                retornoWebservice.XmlRetorno = cliente.ConsultarLoteRps(cabecalho, retornoWebservice.XmlEnvio);
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(AjustarRetorno(retornoWebservice.XmlRetorno));
            MensagemErro(retornoWebservice, xmlRet, "WS_ConsultaLoteRps.ExecuteResponse");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var retornoLote = xmlRet.ElementAnyNs("WS_ConsultaLoteRps.ExecuteResponse").ElementAnyNs("Consultarloterpsresposta");
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

        #endregion Services

        #region Methods

        private static string AjustarEnvio(string envio)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                envio = envio.Replace(unescapedCharacters[i], escapedCharacters[i]);
            }
            return envio;
        }

        private static string AjustarRetorno(string retorno)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                retorno = retorno.Replace(escapedCharacters[i], unescapedCharacters[i]);
            }
            retorno = retorno.Replace("xmlns=\"\"", "");
            retorno = retorno.Replace("xmlns=\"FISS-LEX\"", "");
            return retorno;
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new FissLexServiceClient(this, tipo);
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse.xsd";
        }

        #endregion Methods
    }
}