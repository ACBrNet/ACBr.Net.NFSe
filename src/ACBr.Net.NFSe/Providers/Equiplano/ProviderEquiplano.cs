// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : angelomachado
// Created          : 01-23-2020
//
// Last Modified By : angelomachado
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="ProviderEquiplano.cs" company="ACBr.Net">
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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class ProviderEquiplano : ProviderBase
    {
        #region Constructors

        public ProviderEquiplano(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Equiplano";
        }

        #endregion Constructors

        #region Methods

        #region RPS

        public override string WriteXmlRps(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xmlDoc.Add(WriteRps(nota));
            return xmlDoc.AsString(identado, showDeclaration);
        }

        private XElement WriteRps(NotaFiscal nota)
        {
            var rps = new XElement("rps");
            rps.AddChild(new XElement("nrRps", nota.IdentificacaoRps.Numero));
            rps.AddChild(new XElement("nrEmissorRps", 1));

            rps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "dtEmissaoRps", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            var stRps = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

            rps.AddChild(new XElement("stRps", stRps));

            var tpTributacao = "1";
            switch (nota.TipoTributacao)
            {
                case TipoTributacao.Tributavel:
                    tpTributacao = "1";
                    break;

                case TipoTributacao.ForaMun:
                    tpTributacao = "2";
                    break;

                case TipoTributacao.Imune:
                    tpTributacao = "3";
                    break;

                case TipoTributacao.Isenta:
                    tpTributacao = "3";
                    break;

                case TipoTributacao.Suspensa:
                    tpTributacao = "4";
                    break;
            }
            rps.AddChild(new XElement("tpTributacao", tpTributacao));

            var issRetido = nota.Servico.ResponsavelRetencao == ResponsavelRetencao.Prestador ? "1" : "2";
            rps.AddChild(new XElement("isIssRetido", issRetido));

            rps.AddChild(WriteTomadorRps(nota));

            rps.AddChild(WriteValoresServicos(nota));

            rps.AddChild(new XElement("vlTotalRps", nota.Servico.Valores.ValorServicos));
            rps.AddChild(new XElement("vlLiquidoRps", nota.Servico.Valores.ValorLiquidoNfse));

            rps.AddChild(WriteRetencoes(nota));

            if (!nota.DiscriminacaoImpostos.IsEmpty())
                rps.AddChild(new XElement("dsImpostos", nota.DiscriminacaoImpostos));

            return rps;
        }

        private XElement WriteValoresServicos(NotaFiscal nota)
        {
            var listaServicos = new XElement("listaServicos");
            if (nota.Servico.ItensServico.Count > 0)
            {
                foreach (var servicoItem in nota.Servico.ItensServico)
                {
                    var iSerItem = 0;
                    var iSerSubItem = 0;
                    var iAux = int.Parse(Regex.Replace(servicoItem.ItemListaServico, "[^0-9]", "")); //Ex.: 1402, 901

                    if (iAux > 999)
                    {
                        iSerItem = int.Parse(iAux.ToString().Substring(0, 2)); //14
                        iSerSubItem = int.Parse(iAux.ToString().Substring(2, 2)); //2
                    }
                    else
                    {
                        iSerItem = int.Parse(iAux.ToString().Substring(0, 1)); //9
                        iSerSubItem = int.Parse(iAux.ToString().Substring(1, 2)); //1
                    }

                    var servico = new XElement("servico");
                    servico.AddChild(new XElement("nrServicoItem", iSerItem));
                    servico.AddChild(new XElement("nrServicoSubItem", iSerSubItem));
                    servico.AddChild(new XElement("vlServico", servicoItem.ValorUnitario));
                    servico.AddChild(new XElement("vlAliquota", servicoItem.Aliquota));

                    if (nota.Servico.Valores.ValorDeducoes > 0)
                    {
                        var deducao = new XElement("deducao");
                        deducao.AddChild(AdicionarTag(TipoCampo.De2, "", "vlDeducao", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
                        deducao.AddChild(AdicionarTag(TipoCampo.Str, "", "dsJustificativaDeducao", 1, 115, Ocorrencia.Obrigatoria, nota.Servico.Valores.JustificativaDeducao));
                        servico.AddChild(deducao);
                    }

                    servico.AddChild(AdicionarTag(TipoCampo.De2, "", "vlBaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, servicoItem.BaseCalculo));
                    servico.AddChild(AdicionarTag(TipoCampo.De2, "", "vlIssServico", 1, 15, Ocorrencia.MaiorQueZero, servicoItem.ValorIss));
                    servico.AddChild(AdicionarTag(TipoCampo.Str, "", "dsDiscriminacaoServico", 1, 15, Ocorrencia.MaiorQueZero, servicoItem.Discriminacao));
                    listaServicos.Add(servico);
                }
            }
            else
            {
                var iSerItem = 0;
                var iSerSubItem = 0;
                var iAux = int.Parse(Regex.Replace(nota.Servico.ItemListaServico, "[^0-9]", "")); //Ex.: 1402, 901

                if (iAux > 999)
                {
                    iSerItem = int.Parse(iAux.ToString().Substring(0, 2)); //14
                    iSerSubItem = int.Parse(iAux.ToString().Substring(2, 2)); //2
                }
                else
                {
                    iSerItem = int.Parse(iAux.ToString().Substring(0, 1)); //9
                    iSerSubItem = int.Parse(iAux.ToString().Substring(1, 2)); //1
                }

                var servico = new XElement("servico");
                servico.AddChild(new XElement("nrServicoItem", iSerItem));
                servico.AddChild(new XElement("nrServicoSubItem", iSerSubItem));
                servico.AddChild(new XElement("vlServico", nota.Servico.Valores.ValorServicos));
                servico.AddChild(new XElement("vlAliquota", nota.Servico.Valores.Aliquota));

                if (nota.Servico.Valores.ValorDeducoes > 0)
                {
                    var deducao = new XElement("deducao");
                    deducao.AddChild(AdicionarTag(TipoCampo.De2, "", "vlDeducao", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
                    deducao.AddChild(AdicionarTag(TipoCampo.Str, "", "dsJustificativaDeducao", 1, 115, Ocorrencia.Obrigatoria, nota.Servico.Valores.JustificativaDeducao));
                    servico.AddChild(deducao);
                }

                servico.AddChild(AdicionarTag(TipoCampo.De2, "", "vlBaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));
                servico.AddChild(AdicionarTag(TipoCampo.De2, "", "vlIssServico", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
                servico.AddChild(AdicionarTag(TipoCampo.Str, "", "dsDiscriminacaoServico", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Discriminacao));

                listaServicos.Add(servico);
            }

            return listaServicos;
        }

        private XElement WriteRetencoes(NotaFiscal nota)
        {
            var retencoes = new XElement("retencoes");

            if (nota.Servico.Valores.ValorCofins > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlCofins", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCofins));

            if (nota.Servico.Valores.ValorCsll > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "VlCsll", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCsll));

            if (nota.Servico.Valores.ValorInss > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlInss", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorInss));

            if (nota.Servico.Valores.ValorIr > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlIrrf", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorIr));

            if (nota.Servico.Valores.ValorPis > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlPis", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorPis));

            if (nota.Servico.Valores.ValorIssRetido > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));

            if (nota.Servico.Valores.AliquotaCofins > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlAliquotaCofins", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaCofins));

            if (nota.Servico.Valores.AliquotaCsll > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlAliquotaCsll", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaCsll));

            if (nota.Servico.Valores.AliquotaInss > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlAliquotaInss", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaInss));

            if (nota.Servico.Valores.AliquotaIR > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlAliquotaIrrf", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaIR));

            if (nota.Servico.Valores.AliquotaPis > 0)
                retencoes.AddChild(AdicionarTag(TipoCampo.De2, "", "vlAliquotaPis", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaPis));

            return retencoes;
        }

        private XElement WriteTomadorRps(NotaFiscal nota)
        {
            string sTpDoc;
            if (!string.IsNullOrEmpty(nota.Tomador.DocTomadorEstrangeiro))
                sTpDoc = "3"; // Estrangeiro
            else if (nota.Tomador.CpfCnpj.IsCNPJ())
                sTpDoc = "2"; // CNPJ
            else
                sTpDoc = "1"; // CPF

            var tomador = new XElement("tomador");
            var documento = new XElement("documento");
            documento.AddChild(AdicionarTagCNPJCPF("", "nrDocumento", "nrDocumento", nota.Tomador.CpfCnpj));
            documento.AddChild(AdicionarTag(TipoCampo.Int, "", "tpDocumento", 1, 1, Ocorrencia.Obrigatoria, sTpDoc));
            documento.AddChild(AdicionarTag(TipoCampo.Str, "", "dsDocumentoEstrangeiro", 1, 115, Ocorrencia.Obrigatoria, nota.Tomador.DocTomadorEstrangeiro));
            tomador.Add(documento);

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nmTomador", 1, 80, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "dsEmail", 0, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nrInscricaoEstadual", 1, 20, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual));
            // tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nrInscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "dsEndereco", 0, 40, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
            if (!nota.Tomador.Endereco.Numero.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "nrEndereco", 0, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
            if (!nota.Tomador.Endereco.Complemento.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "dsComplemento", 0, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
            if (!nota.Tomador.Endereco.Bairro.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nmBairro", 0, 25, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
            if (nota.Tomador.Endereco.CodigoMunicipio > 0)
                tomador.AddChild(AdicionarTag(TipoCampo.Int, "", "nrCidadeIbge", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
            if (!nota.Tomador.Endereco.Uf.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nmUf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));

            if (!nota.Tomador.DocTomadorEstrangeiro.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nmCidadeEstrangeira", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Municipio));

            if (!nota.Tomador.Endereco.Pais.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "nmPais", 1, 100, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Pais));

            if (!nota.Tomador.Endereco.Cep.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "nrCep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));

            if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty())
                tomador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "nrTelefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));

            return tomador;
        }

        #endregion RPS

        #region Services

        public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var xmlListaRps = new StringBuilder();
            xmlListaRps.Append("<listaRps>");
            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                xmlListaRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }
            xmlListaRps.Append("</listaRps>");

            var optanteSimplesNacional = notas.First().RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional ? "1" : "2";

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<es:enviarLoteRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esRecepcionarLoteRpsEnvio_01.xsd\">")
                .Append($"<lote>")
                .Append($"<nrLote>{ lote }</nrLote>")
                .Append($"<qtRps>{ notas.Count }</qtRps>")
                .Append($"<nrVersaoXml>1</nrVersaoXml>")
                .Append("<prestador>")
                .Append($"<nrCnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</nrCnpj>")
                .Append($"<nrInscricaoMunicipal>{ Configuracoes.PrestadorPadrao.InscricaoMunicipal }</nrInscricaoMunicipal>")
                .Append($"<isOptanteSimplesNacional>{ optanteSimplesNacional }</isOptanteSimplesNacional>")
                .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
                .Append("</prestador>")
                .Append(xmlListaRps.ToString())
                .Append("</lote>")
                .Append("</es:enviarLoteRpsEnvio>")
                .ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:enviarLoteRpsEnvio", "", Certificado);

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<esRecepcionarLoteRps xmlns=\"http://services.enfsws.es\">")
                .Append("<nrVersaoXml>1</nrVersaoXml>")
                .Append("<xml>")
                .AppendEnvio(retornoWebservice.XmlEnvio)
                .Append("</xml>")
                .Append("</esRecepcionarLoteRps>")
                .ToString();

            // Verifica Schema
            ValidarSchema(retornoWebservice, "esRecepcionarLoteRpsEnvio_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio.AjustarString(), $"lote-{lote}-env.xml");

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.Enviar))
                {
                    retornoWebservice.XmlRetorno = cliente.RecepcionarLoteRps(null, retornoWebservice.XmlEnvio);
                    retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno.AjustarString();
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"lote-{lote}-ret.xml");

            retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno
                .Replace("<ns:esRecepcionarLoteRpsResponse xmlns:ns=\"http://services.enfsws.es\">", "")
                .Replace("<ns:return>", "")
                .Replace("</ns:return>", "")
                .Replace("</ns:esRecepcionarLoteRpsResponse>", "");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var rootElement = xmlRet.ElementAnyNs("esEnviarLoteRpsResposta");
            MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var protocoloElement = rootElement?.ElementAnyNs("protocolo");

            retornoWebservice.NumeroLote = protocoloElement?.ElementAnyNs("nrLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.DataLote = protocoloElement?.ElementAnyNs("dtRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = protocoloElement?.ElementAnyNs("nrProtocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.NumeroLote.IsEmpty();

            if (!retornoWebservice.Sucesso) return retornoWebservice;

            foreach (var nota in notas)
            {
                nota.NumeroLote = retornoWebservice.NumeroLote;
            }

            return retornoWebservice;
        }

        public override RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (numeroNFSe.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
                return retornoWebservice;
            }

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
                .Append("<es:esCancelarNfseEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esCancelarNfseEnvio_v01.xsd\">")
                .Append("<prestador>")
                .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
                .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
                .Append("</prestador>")
                .Append($"<nrNfse>{ numeroNFSe }</nrNfse>")
                .Append($"<dsMotivoCancelamento>{ motivo }</dsMotivoCancelamento>")
                .Append("</es:esCancelarNfseEnvio>")
                .ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esCancelarNfseEnvio", "", Certificado);

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<esCancelarNfse xmlns=\"http://services.enfsws.es\">")
                .Append("<nrVersaoXml>1</nrVersaoXml>")
                .Append("<xml>")
                .AppendEnvio(retornoWebservice.XmlEnvio)
                .Append("</xml>")
                .Append("</esCancelarNfse>")
                .ToString();

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CanNFSe-{numeroNFSe}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "esCancelarNfseEnvio_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.CancelaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.CancelarNFSe(null, retornoWebservice.XmlEnvio);
                    retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno.AjustarString();
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"CanNFSe-{numeroNFSe}-ret.xml");

            retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno
                .Replace("<ns:esCancelarNfseResponse xmlns:ns=\"http://services.enfsws.es\">", "")
                .Replace("<ns:return>", "")
                .Replace("</ns:return>", "")
                .Replace("</ns:esCancelarNfseResponse>", "");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            var rootElement = xmlRet.ElementAnyNs("esCancelarNfseResposta");
            MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var sucesso = rootElement.ElementAnyNs("sucesso");
            if (sucesso == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
                return retornoWebservice;
            }

            retornoWebservice.DataLote = rootElement.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = retornoWebservice.DataLote != DateTime.MinValue;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
            if (nota == null) return retornoWebservice;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;
            nota.Cancelamento.DataHora = rootElement.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Cancelamento.MotivoCancelamento = motivo;

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (numero.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número do RPS/NFSe não informado para a consulta." });
                return retornoWebservice;
            }

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
                .Append("<es:esConsultarNfsePorRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarNfsePorRpsEnvio_v01.xsd\">")
                .Append("<rps>")
                .Append($"<nrRps>{ numero }</nrRps>")
                .Append("<nrEmissorRps>1</nrEmissorRps>")
                .Append("</rps>")
                .Append("<prestador>")
                .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
                .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
                .Append("</prestador>")
                .Append("</es:esConsultarNfsePorRpsEnvio>")
                .ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarNfsePorRpsEnvio", "", Certificado);

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<esConsultarNfsePorRps xmlns=\"http://services.enfsws.es\">")
                .Append("<nrVersaoXml>1</nrVersaoXml>")
                .Append("<xml>")
                .AppendEnvio(retornoWebservice.XmlEnvio)
                .Append("</xml>")
                .Append("</esConsultarNfsePorRps>")
                .ToString();

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNotaRps-{ numero }-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "esConsultarNfsePorRpsEnvio_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultaNFSeRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSePorRps(null, retornoWebservice.XmlEnvio);
                    retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno.AjustarString();
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNotaRps-{ numero }-ret.xml");

            retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno
                .Replace("<ns:esConsultarNfsePorRpsResponse xmlns:ns=\"http://services.enfsws.es\">", "")
                .Replace("<ns:return>", "")
                .Replace("</ns:return>", "")
                .Replace("</ns:esConsultarNfsePorRpsResponse>", "");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);

            MensagemErro(retornoWebservice, xmlRet.ElementAnyNs("esConsultarNfsePorRpsResposta"), "mensagemRetorno");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var elementRoot = xmlRet.ElementAnyNs("esConsultarNfsePorRpsResposta");

            var nfse = elementRoot.ElementAnyNs("nfse");

            if (nfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (nfse)" });
                return retornoWebservice;
            }

            var nota = new NotaFiscal();
            nota.IdentificacaoNFSe.Chave = nfse?.ElementAnyNs("cdAutenticacao")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.Numero = nfse?.ElementAnyNs("nrNfse")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Numero = nfse?.ElementAnyNs("nrRps")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.DataEmissao = nfse.ElementAnyNs("dtEmissaoNfs")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Situacao = SituacaoNFSeRps.Normal;

            var infoCancelamento = nfse.ElementAnyNs("cancelamento");
            if (infoCancelamento != null)
            {
                nota.Cancelamento.DataHora = infoCancelamento.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
                nota.Cancelamento.MotivoCancelamento = infoCancelamento?.ElementAnyNs("dsCancelamento")?.GetValue<string>() ?? string.Empty;
                nota.Situacao = SituacaoNFSeRps.Cancelado;
            }

            notas.Add(nota);

            retornoWebservice.Sucesso = true;

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNfse, int pagina, string cnpjPrestador, string imPrestador, string nomeInter, string cnpjInter, string imInter, string serie, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();
            var xml = new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
                .Append("<es:esConsultarNfseEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarNfsePorRpsEnvio_v01.xsd\">")
                .Append("<prestador>")
                .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
                .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
                .Append("</prestador>");

            if (!numeroNfse.IsEmpty())
            {
                xml = xml.Append($"<nrNfse>{ numeroNfse }</nrNfse>");
            }
            else
            {
                if (inicio == null || fim == null)
                {
                    retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Data de início ou fim não informada para a consulta." });
                    return retornoWebservice;
                }

                xml = xml
                    .Append("<periodoEmissao>")
                    .Append($"<dtInicial>{inicio.Value:yyyy'-'MM'-'dd'T'HH':'mm':'ss}</dtInicial>")
                    .Append($"<dtFinal>{fim.Value:yyyy'-'MM'-'dd'T'HH':'mm':'ss}</dtFinal>")
                    .Append("</periodoEmissao>");
            }
            xml = xml.Append("</es:esConsultarNfseEnvio>");

            retornoWebservice.XmlEnvio = xml.ToString(); ;

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            var xmlAssinado = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarNfseEnvio", "", Certificado);

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<esConsultarNfse xmlns=\"http://services.enfsws.es\">")
                .Append("<nrVersaoXml>1</nrVersaoXml>")
                .Append("<xml>")
                .AppendEnvio(xmlAssinado)
                .Append("</xml>")
                .Append("</esConsultarNfse>")
                .ToString();

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNota-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "esConsultarNfseEnvio_v01.xsd");
            if (retornoWebservice.Erros.Any())
                return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSe(null, retornoWebservice.XmlEnvio);
                    retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno.AjustarString();
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNotaRps-ret.xml");

            retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno
                .Replace("<ns:esConsultarNfseResponse xmlns:ns=\"http://services.enfsws.es\">", "")
                .Replace("<ns:return>", "")
                .Replace("</ns:return>", "")
                .Replace("</ns:esConsultarNfseResponse>", "");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.ElementAnyNs("esConsultarNfseResposta"), "mensagemRetorno");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var elementRoot = xmlRet.ElementAnyNs("esConsultarNfseResposta");

            var listaNfse = elementRoot.ElementAnyNs("listaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (listaNfse)" });
                return retornoWebservice;
            }

            foreach (var nfse in listaNfse.ElementsAnyNs("nfse"))
            {
                var nota = new NotaFiscal();
                nota.IdentificacaoNFSe.Chave = nfse?.ElementAnyNs("cdAutenticacao")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.Numero = nfse?.ElementAnyNs("nrNfse")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoRps.Numero = nfse?.ElementAnyNs("nrRps")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.DataEmissao = nfse.ElementAnyNs("dtEmissaoNfs")?.GetValue<DateTime>() ?? DateTime.MinValue;
                nota.Situacao = SituacaoNFSeRps.Normal;

                var infoCancelamento = nfse.ElementAnyNs("cancelamento");
                if (infoCancelamento != null)
                {
                    nota.Cancelamento.DataHora = infoCancelamento.ElementAnyNs("dtCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
                    nota.Cancelamento.MotivoCancelamento = infoCancelamento?.ElementAnyNs("dsCancelamento")?.GetValue<string>() ?? string.Empty;
                    nota.Situacao = SituacaoNFSeRps.Cancelado;
                }

                notas.Add(nota);
            }

            retornoWebservice.Sucesso = true;
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarLoteRps(int lote, string protocolo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append($"<es:esConsultarLoteRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarLoteRpsEnvio_v01.xsd\">")
                .Append("<prestador>")
                .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
                .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
                .Append("</prestador>")
                .Append($"<nrLoteRps>{ lote }</nrLoteRps>")
                .Append("</es:esConsultarLoteRpsEnvio>")
                .ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarLoteRpsEnvio", "", Certificado);

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<esConsultarLoteNfse xmlns=\"http://services.enfsws.es\">")
                .Append("<nrVersaoXml>1</nrVersaoXml>")
                .Append("<xml>")
                .AppendEnvio(retornoWebservice.XmlEnvio)
                .Append("</xml>")
                .Append("</esConsultarLoteNfse>")
                .ToString();

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{""}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "esConsultarLoteRpsEnvio_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultarSituacao))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarLoteRps(null, retornoWebservice.XmlEnvio);
                    retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno.AjustarString();
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarLoteRps-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");

            retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno
                .Replace("<ns:esConsultarLoteRpsResponse xmlns:ns=\"http://services.enfsws.es\">", "")
                .Replace("<ns:return>", "")
                .Replace("</ns:return>", "")
                .Replace("</ns:esConsultarLoteRpsResponse>", "");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var rootElement = xmlRet.ElementAnyNs("esConsultarLoteRpsResposta");
            MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            var elementRoot = xmlRet.ElementAnyNs("esConsultarLoteRpsResposta");

            var listaNfse = elementRoot.ElementAnyNs("listaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (listaNfse)" });
                return retornoWebservice;
            }

            foreach (var nfse in listaNfse.ElementsAnyNs("nfse"))
            {
                var nota = new NotaFiscal();
                nota.IdentificacaoNFSe.Chave = nfse?.ElementAnyNs("cdAutenticacao")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.Numero = nfse?.ElementAnyNs("nrNfse")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoRps.Numero = nfse?.ElementAnyNs("nrRps")?.GetValue<string>() ?? string.Empty;
                nota.IdentificacaoNFSe.DataEmissao = nfse.ElementAnyNs("dtEmissaoNfs")?.GetValue<DateTime>() ?? DateTime.MinValue;

                notas.Add(nota);
            }

            retornoWebservice.Sucesso = true;
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWebservice = new RetornoWebservice();

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append($"<es:esConsultarSituacaoLoteRpsEnvio xmlns:es=\"http://www.equiplano.com.br/esnfs\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.equiplano.com.br/enfs esConsultarLoteRpsEnvio_v01.xsd\">")
                .Append("<prestador>")
                .Append($"<cnpj>{ Configuracoes.PrestadorPadrao.CpfCnpj }</cnpj>")
                .Append($"<idEntidade>{ Municipio.IdEntidade }</idEntidade>")
                .Append("</prestador>")
                .Append($"<nrLoteRps>{ lote }</nrLoteRps>")
                .Append("</es:esConsultarSituacaoLoteRpsEnvio>")
                .ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            var xmlAssinado = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "es:esConsultarSituacaoLoteRpsEnvio", "", Certificado);

            retornoWebservice.XmlEnvio = new StringBuilder()
                .Append("<esConsultarSituacaoLoteRps xmlns=\"http://services.enfsws.es\">")
                .Append("<nrVersaoXml>1</nrVersaoXml>")
                .Append("<xml>")
                .AppendEnvio(xmlAssinado)
                .Append("</xml>")
                .Append("</esConsultarSituacaoLoteRps>")
                .ToString();

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{""}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "esConsultarSituacaoLoteRpsEnvio_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultarSituacao))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarSituacaoLoteRps(null, retornoWebservice.XmlEnvio);
                    retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno.AjustarString();
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");

            retornoWebservice.XmlRetorno = retornoWebservice.XmlRetorno
                .Replace("<ns:esConsultarSituacaoLoteRpsResponse xmlns:ns=\"http://services.enfsws.es\">", "")
                .Replace("<ns:return>", "")
                .Replace("</ns:return>", "")
                .Replace("</ns:esConsultarSituacaoLoteRpsResponse>", "");

            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var rootElement = xmlRet.ElementAnyNs("esConsultarSituacaoLoteRpsResposta");
            MensagemErro(retornoWebservice, rootElement, "mensagemRetorno");

            retornoWebservice.NumeroLote = rootElement?.ElementAnyNs("nrLoteRps")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Situacao = rootElement?.ElementAnyNs("stLote")?.GetValue<string>() ?? "0";
            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
            return retornoWebservice;
        }

        #endregion Services

        #endregion Methods

        #region Private Methods

        private EquiplanoServiceClient GetClient(TipoUrl tipo)
        {
            return new EquiplanoServiceClient(this, tipo);
        }

        private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag, string elementName = "listaErros", string messageElement = "erro")
        {
            var listaErros = xmlRet?.ElementAnyNs(xmlTag)?.ElementAnyNs(elementName);
            if (listaErros == null) return;

            foreach (var erro in listaErros.ElementsAnyNs(messageElement))
            {
                var evento = new Evento
                {
                    Codigo = erro?.ElementAnyNs("cdMensagem")?.GetValue<string>() ?? string.Empty,
                    Descricao = erro?.ElementAnyNs("dsMensagem")?.GetValue<string>() ?? string.Empty,
                };

                retornoWs.Erros.Add(evento);
            }
        }

        #endregion Private Methods
    }
}