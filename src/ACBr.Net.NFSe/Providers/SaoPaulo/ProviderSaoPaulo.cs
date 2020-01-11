// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderSaoPaulo.cs" company="ACBr.Net">
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

using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    internal sealed class ProviderSaoPaulo : ProviderBase
    {
        #region Constructors

        public ProviderSaoPaulo(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "São Paulo";
        }

        #endregion Constructors

        #region Methods

        public override NotaFiscal LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");

            var ret = new NotaFiscal();
            ret.Assinatura = xml.Root?.ElementAnyNs("Assinatura")?.GetValue<string>() ?? string.Empty;

            // Nota Fiscal
            ret.IdentificacaoNFSe.Numero = xml.ElementAnyNs("ChaveNFe")?.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoNFSe.Chave = xml.ElementAnyNs("ChaveNFe")?.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            ret.Prestador.InscricaoMunicipal = xml.ElementAnyNs("ChaveNFe")?.ElementAnyNs("InscricaoPrestador")?.GetValue<string>() ?? string.Empty;

            ret.IdentificacaoNFSe.DataEmissao = xml.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            ret.NumeroLote = xml.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? "";

            // RPS
            ret.IdentificacaoRps.Numero = xml.ElementAnyNs("ChaveRPS")?.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;
            ret.IdentificacaoRps.Serie = xml.ElementAnyNs("ChaveRPS")?.ElementAnyNs("SerieRPS")?.GetValue<string>() ?? string.Empty;
            if (ret.Prestador.InscricaoMunicipal == "")
                ret.Prestador.InscricaoMunicipal = xml.ElementAnyNs("ChaveRPS")?.ElementAnyNs("InscricaoPrestador")?.GetValue<string>() ?? string.Empty;

            switch (xml.ElementAnyNs("TipoRPS")?.GetValue<string>() ?? string.Empty)
            {
                case "RPS":
                    ret.IdentificacaoRps.Tipo = TipoRps.RPS;
                    break;

                case "RPS-M":
                    ret.IdentificacaoRps.Tipo = TipoRps.NFConjugada;
                    break;

                case "RPS-C":
                    ret.IdentificacaoRps.Tipo = TipoRps.Cupom;
                    break;
            }

            ret.IdentificacaoRps.DataEmissao = xml.ElementAnyNs("DataEmissaoRPS")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Tipo da Tributação
            switch (xml.ElementAnyNs("TributacaoNFe")?.GetValue<string>() ?? string.Empty)
            {
                case "T":
                    ret.TipoTributacao = TipoTributacao.Tributavel;
                    break;

                case "F":
                    ret.TipoTributacao = TipoTributacao.ForaMun;
                    break;

                case "A":
                    ret.TipoTributacao = TipoTributacao.Isenta;
                    break;

                case "B":
                    ret.TipoTributacao = TipoTributacao.ForaMunIsento;
                    break;

                case "M":
                    ret.TipoTributacao = TipoTributacao.Imune;
                    break;

                case "N":
                    ret.TipoTributacao = TipoTributacao.ForaMunImune;
                    break;

                case "X":
                    ret.TipoTributacao = TipoTributacao.Suspensa;
                    break;

                case "V":
                    ret.TipoTributacao = TipoTributacao.ForaMunSuspensa;
                    break;

                case "P":
                    ret.TipoTributacao = TipoTributacao.ExpServicos;
                    break;
            }

            switch (xml.ElementAnyNs("StatusNFe")?.GetValue<string>() ?? string.Empty)
            {
                case "N":
                    ret.Situacao = SituacaoNFSeRps.Normal;
                    break;

                case "F":
                    ret.Situacao = SituacaoNFSeRps.Cancelado;
                    break;
            }

            ret.Servico.Discriminacao = xml.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
            ret.Servico.Valores.ValorServicos = xml.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.Aliquota = xml.ElementAnyNs("AliquotaServicos")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.ValorIss = xml.ElementAnyNs("ValorISS")?.GetValue<decimal>() ?? 0;
            ret.Servico.ItemListaServico = xml.ElementAnyNs("CodigoServico")?.GetValue<string>() ?? string.Empty;
            ret.Servico.Valores.ValorCargaTributaria = xml.ElementAnyNs("ValorCargaTributaria")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.AliquotaCargaTributaria = xml.ElementAnyNs("PercentualCargaTributaria")?.GetValue<decimal>() ?? 0;
            ret.Servico.Valores.FonteCargaTributaria = xml.ElementAnyNs("FonteCargaTributaria")?.GetValue<string>() ?? string.Empty;
            ret.ValorCredito = xml.ElementAnyNs("ValorCredito")?.GetValue<decimal>() ?? 0;

            switch (xml.ElementAnyNs("ISSRetido")?.GetValue<string>() ?? string.Empty)
            {
                case "true":
                    ret.Servico.Valores.IssRetido = SituacaoTributaria.Retencao;
                    break;

                case "false":
                    ret.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
                    break;
            }

            ret.Prestador.CpfCnpj = xml.ElementAnyNs("CPFCNPJPrestador")?.ElementAnyNs("CNPJ")?.GetValue<string>() ?? string.Empty;
            if (ret.Prestador.CpfCnpj == "")
                ret.Prestador.CpfCnpj = xml.ElementAnyNs("CPFCNPJPrestador")?.ElementAnyNs("CPF")?.GetValue<string>() ?? string.Empty;
            ret.Prestador.RazaoSocial = xml.ElementAnyNs("RazaoSocialPrestador")?.GetValue<string>() ?? string.Empty;
            var endPrestador = xml.ElementAnyNs("EnderecoPrestador");
            if (endPrestador != null)
            {
                ret.Prestador.Endereco.TipoLogradouro = endPrestador.ElementAnyNs("TipoLogradouro")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Logradouro = endPrestador.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Numero = endPrestador.ElementAnyNs("NumeroEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Complemento = endPrestador.ElementAnyNs("ComplementoEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Bairro = endPrestador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.CodigoMunicipio = endPrestador.ElementAnyNs("Cidade")?.GetValue<int>() ?? 0;
                ret.Prestador.Endereco.Uf = endPrestador.ElementAnyNs("UF")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.Endereco.Cep = endPrestador.ElementAnyNs("CEP")?.GetValue<string>() ?? string.Empty;
            }

            ret.Tomador.CpfCnpj = xml.ElementAnyNs("CPFCNPJTomador")?.ElementAnyNs("CNPJ")?.GetValue<string>() ?? string.Empty;
            if (ret.Tomador.CpfCnpj == "")
                ret.Tomador.CpfCnpj = xml.ElementAnyNs("CPFCNPJTomador")?.ElementAnyNs("CPF")?.GetValue<string>() ?? string.Empty;
            ret.Tomador.RazaoSocial = xml.ElementAnyNs("RazaoSocialTomador")?.GetValue<string>() ?? string.Empty;
            var endTomador = xml.ElementAnyNs("EnderecoTomador");
            if (endTomador != null)
            {
                ret.Tomador.Endereco.TipoLogradouro = endTomador.ElementAnyNs("TipoLogradouro")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Logradouro = endTomador.ElementAnyNs("Logradouro")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Numero = endTomador.ElementAnyNs("NumeroEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Complemento = endTomador.ElementAnyNs("ComplementoEndereco")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Bairro = endTomador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.CodigoMunicipio = endTomador.ElementAnyNs("Cidade")?.GetValue<int>() ?? 0;
                ret.Tomador.Endereco.Uf = endTomador.ElementAnyNs("UF")?.GetValue<string>() ?? string.Empty;
                ret.Tomador.Endereco.Cep = endTomador.ElementAnyNs("CEP")?.GetValue<string>() ?? string.Empty;
            }

            return ret;
        }

        public override string GetXmlRps(NotaFiscal nota, bool identado, bool showDeclaration)
        {
            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "RPS";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "RPS-M";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "RPS-C";
                    break;

                default:
                    tipoRps = "";
                    break;
            }

            string tipoTributacao;
            switch (nota.TipoTributacao)
            {
                case TipoTributacao.Tributavel:
                    tipoTributacao = "T";
                    break;

                case TipoTributacao.ForaMun:
                    tipoTributacao = "F";
                    break;

                case TipoTributacao.Isenta:
                    tipoTributacao = "A";
                    break;

                case TipoTributacao.ForaMunIsento:
                    tipoTributacao = "B";
                    break;

                case TipoTributacao.Imune:
                    tipoTributacao = "M";
                    break;

                case TipoTributacao.ForaMunImune:
                    tipoTributacao = "N";
                    break;

                case TipoTributacao.Suspensa:
                    tipoTributacao = "X";
                    break;

                case TipoTributacao.ForaMunSuspensa:
                    tipoTributacao = "V";
                    break;

                case TipoTributacao.ExpServicos:
                    tipoTributacao = "P";
                    break;

                default:
                    tipoTributacao = "";
                    break;
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C";

            var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "true" : "false";

            // RPS
            XNamespace ns = "";

            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var rps = new XElement(ns + "RPS");
            xmlDoc.Add(rps);

            var hashRps = GetHashRps(nota);
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, Ocorrencia.Obrigatoria, hashRps));

            var chaveRPS = new XElement("ChaveRPS");
            rps.Add(chaveRPS);
            chaveRPS.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoPrestador", 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));
            chaveRPS.AddChild(AdicionarTag(TipoCampo.Int, "", "SerieRPS", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            chaveRPS.AddChild(AdicionarTag(TipoCampo.Int, "", "NumeroRPS", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 1, Ocorrencia.Obrigatoria, tipoRps));
            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "StatusRPS", 1, 1, Ocorrencia.Obrigatoria, situacao));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "TributacaoRPS", 1, 1, Ocorrencia.Obrigatoria, tipoTributacao));

            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorDeducoes));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorPis));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCofins));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorInss));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorIr));
            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCsll));

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
            rps.AddChild(AdicionarTag(TipoCampo.De4, "", "AliquotaServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota / 100));  // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "ISSRetido", 1, 4, Ocorrencia.Obrigatoria, issRetido));

            var tomadorCpfCnpj = new XElement("CPFCNPJTomador");
            rps.Add(tomadorCpfCnpj);
            tomadorCpfCnpj.AddChild(AdicionarTagCNPJCPF("", "CPF", "CNPJ", nota.Tomador.CpfCnpj));

            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipalTomador", 1, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));
            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoEstadualTomador", 1, 19, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
            {
                var endereco = new XElement("EnderecoTomador");
                rps.AddChild(endereco);
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "TipoLogradouro", 1, 3, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.TipoLogradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Logradouro", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroEndereco", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "ComplementoEndereco", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cidade", 1, 7, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "UF", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumberFill, "", "CEP", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 75, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));

            if (!nota.Intermediario.CpfCnpj.IsEmpty())
            {
                var intermediarioCpfCnpj = new XElement("CPFCNPJIntermediario");
                rps.Add(intermediarioCpfCnpj);
                intermediarioCpfCnpj.AddChild(AdicionarTagCNPJCPF("", "CPF", "CNPJ", nota.Intermediario.CpfCnpj));

                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalIntermediario", 1, 8, 0, nota.Intermediario.InscricaoMunicipal));
                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocialIntermediario", 1, 115, 0, nota.Intermediario.RazaoSocial));

                var issRetidoIntermediario = nota.Intermediario.IssRetido == SituacaoTributaria.Retencao ? "true" : "false";
                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "ISSRetidoIntermediario", 1, 4, Ocorrencia.Obrigatoria, issRetidoIntermediario));
                rps.AddChild(AdicionarTag(TipoCampo.Str, "", "EmailIntermediario", 1, 75, Ocorrencia.NaoObrigatoria, nota.Intermediario.EMail));
            }

            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));

            rps.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCargaTributaria", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.ValorCargaTributaria));
            rps.AddChild(AdicionarTag(TipoCampo.De4, "", "PercentualCargaTributaria", 1, 15, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.AliquotaCargaTributaria / 100));
            rps.AddChild(AdicionarTag(TipoCampo.Str, "", "FonteCargaTributaria", 1, 10, Ocorrencia.NaoObrigatoria, nota.Servico.Valores.FonteCargaTributaria));

            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCEI", 1, 12, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoCEI));
            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "MatriculaObra", 1, 12, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.Matricula));
            //rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "MunicipioPrestacao", 1, 7, Ocorrencia.MaiorQueZero, nota.Servico.CodigoMunicipio));
            rps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "NumeroEncapsulamento", 1, 7, Ocorrencia.NaoObrigatoria, nota.Material.NumeroEncapsulamento));

            return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0)
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });

            if (notas.Count == 0)
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });

            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;

            var xmlRPS = new StringBuilder();
            foreach (var nota in notas)
            {
                var xmlRps = GetXmlRps(nota, false, false);
                xmlRPS.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            xmlRPS.Replace("<RPS>", "<RPS xmlns=\"\">");

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoEnvioLoteRPS xmlns=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<transacao>true</transacao>");
            loteBuilder.Append($"<dtInicio>{notas.Min(x => x.IdentificacaoRps.DataEmissao):yyyy-MM-dd}</dtInicio>");
            loteBuilder.Append($"<dtFim>{notas.Max(x => x.IdentificacaoRps.DataEmissao):yyyy-MM-dd}</dtFim>");
            loteBuilder.Append($"<QtdRPS>{notas.Count}</QtdRPS>");
            loteBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<ValorTotalServicos>{0:0.00}</ValorTotalServicos>", notas.Sum(x => x.Servico.Valores.ValorServicos)));
            loteBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<ValorTotalDeducoes>{0:0.00}</ValorTotalDeducoes>", notas.Sum(x => x.Servico.Valores.ValorDeducoes)));
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append(xmlRPS);
            loteBuilder.Append("</PedidoEnvioLoteRPS>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoEnvioLoteRPS", "", Certificado);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-{lote}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "PedidoEnvioLoteRPS_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetCliente(TipoUrl.Enviar))
                {
                    retornoWebservice.XmlRetorno = Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao
                        ? cliente.TesteEnvioLoteRPS(retornoWebservice.XmlEnvio)
                        : cliente.EnvioLoteRPS(retornoWebservice.XmlEnvio);
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
            MensagemErro(retornoWebservice, xmlRet, "RetornoEnvioLoteRPS");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("InformacoesLote")?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.DataLote = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("InformacoesLote")?.ElementAnyNs("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

            if (!retornoWebservice.Sucesso)
                return retornoWebservice;

            foreach (NotaFiscal nota in notas)
            {
                nota.NumeroLote = retornoWebservice.NumeroLote;
            }

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoInformacoesLote xmlns=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<NumeroLote>{lote}</NumeroLote>");
            loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("</PedidoInformacoesLote>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoInformacoesLote", "", Certificado);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "PedidoInformacoesLote_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetCliente(TipoUrl.ConsultarLoteRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultaInformacoesLote(retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoInformacoesLote");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("InformacoesLote")?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarLoteRps(int lote, string protocolo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoConsultaLote xmlns=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<NumeroLote>{lote}</NumeroLote>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("</PedidoConsultaLote>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoConsultaLote", "", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "PedidoConsultaLote_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetCliente(TipoUrl.ConsultarSituacao))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultaLote(retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoConsulta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            foreach (var nfse in xmlRet.Root.ElementsAnyNs("NFe"))
            {
                var chaveNFSe = nfse.ElementAnyNs("ChaveNFe");
                var numeroNFSe = chaveNFSe?.ElementAnyNs("NumeroNFe")?.GetValue<string>() ?? string.Empty;
                var codigoVerificacao = chaveNFSe?.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var dataNFSe = nfse.ElementAnyNs("DataEmissaoNFe")?.GetValue<DateTime>() ?? DateTime.Now;

                var chaveRPS = nfse.ElementAnyNs("ChaveRPS");

                var numeroRps = chaveRPS?.ElementAnyNs("NumeroRPS")?.GetValue<string>() ?? string.Empty;

                GravarNFSeEmDisco(nfse.ToString(), $"NFSe-{numeroNFSe}-{codigoVerificacao}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    notas.Load(nfse.ToString());
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = codigoVerificacao;
                }
            }
            return retornoWebservice;
        }

        public override RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (string.IsNullOrWhiteSpace(numeroNFSe))
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
                return retornoWebservice;
            }

            // Hash Cancelamento
            var hash = Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8) + numeroNFSe.ZeroFill(12);

            var hashAssinado = "";
            var rsa = (RSACryptoServiceProvider)Certificado.PrivateKey;
            var hashBytes = Encoding.ASCII.GetBytes(hash);
            byte[] signData = rsa.SignData(hashBytes, new SHA1CryptoServiceProvider());
            hashAssinado = Convert.ToBase64String(signData);

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<PedidoCancelamentoNFe xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.prefeitura.sp.gov.br/nfe\">");
            loteBuilder.Append("<Cabecalho xmlns=\"\" Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append($"<transacao>true</transacao>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("<Detalhe xmlns=\"\">");
            loteBuilder.Append("<ChaveNFe>");
            loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
            loteBuilder.Append($"<NumeroNFe>{numeroNFSe}</NumeroNFe>");
            loteBuilder.Append("</ChaveNFe>");
            loteBuilder.Append($"<AssinaturaCancelamento>{hashAssinado}</AssinaturaCancelamento>");
            loteBuilder.Append("</Detalhe>");
            loteBuilder.Append("</PedidoCancelamentoNFe>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "PedidoCancelamentoNFe", "", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CanNFSe-{numeroNFSe}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "PedidoCancelamentoNFe_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetCliente(TipoUrl.CancelaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.CancelamentoNFe(retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"CanNFSe-{numeroNFSe}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoCancelamentoNFe");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
            if (nota != null)
            {
                nota.Situacao = SituacaoNFSeRps.Cancelado;
                nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;
                nota.Cancelamento.MotivoCancelamento = motivo;
                // No caso de São Paulo, não retorna o XML da NotaFiscal Cancelada.
                // Por este motivo, não grava o arquivo NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml
            }

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultaNFSeRps(string numeroRPS, string serieRPS, TipoRps tipo, NotaFiscalCollection notas)
        {
            return ConsultarRpsNfseSP(numeroRPS, serieRPS, "", notas);
        }

        public override RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNFSe, int pagina, string cnpjTomador,
            string imTomador, string nomeInter, string cnpjInter, string imInter, string serie, NotaFiscalCollection notas)
        {
            if (!string.IsNullOrWhiteSpace(numeroNFSe))
            {
                // Se informou o número da NFSe, utiliza o método que retorna apenas a NFSe desejada.
                return ConsultarRpsNfseSP("", "", numeroNFSe, notas);
            }

            var retornoWebservice = new RetornoWebservice();
            retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Falta implementar, no nosso componente, a Consulta de Lista da NFe Recebidas/Emitidas." });
            return retornoWebservice;
        }

        #region Private Methods

        private SaoPauloServiceClient GetCliente(TipoUrl tipo)
        {
            return new SaoPauloServiceClient(this, tipo);
        }

        private RetornoWebservice ConsultarRpsNfseSP(string numeroRPS, string serieRPS, string numeroNFSe, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (string.IsNullOrWhiteSpace(numeroRPS) & string.IsNullOrWhiteSpace(numeroNFSe))
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número do RPS/NFSe não informado para a consulta." });
                return retornoWebservice;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<p1:PedidoConsultaNFe xmlns:p1=\"http://www.prefeitura.sp.gov.br/nfe\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            loteBuilder.Append("<Cabecalho Versao=\"1\">");
            loteBuilder.Append($"<CPFCNPJRemetente><CNPJ>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</CNPJ></CPFCNPJRemetente>");
            loteBuilder.Append("</Cabecalho>");
            loteBuilder.Append("<Detalhe>");
            if (!string.IsNullOrWhiteSpace(numeroRPS))
            {
                // RPS
                loteBuilder.Append("<ChaveRPS>");
                loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
                loteBuilder.Append($"<SerieRPS>{serieRPS}</SerieRPS>");
                loteBuilder.Append($"<NumeroRPS>{numeroRPS}</NumeroRPS>");
                loteBuilder.Append("</ChaveRPS>");
            }
            else
            {
                // NFSe
                loteBuilder.Append("<ChaveNFe>");
                loteBuilder.Append($"<InscricaoPrestador>{Configuracoes.PrestadorPadrao.InscricaoMunicipal.ZeroFill(8)}</InscricaoPrestador>");
                loteBuilder.Append($"<NumeroNFe>{numeroNFSe}</NumeroNFe>");
                loteBuilder.Append("</ChaveNFe>");
            }
            loteBuilder.Append("</Detalhe>");
            loteBuilder.Append("</p1:PedidoConsultaNFe>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "p1:PedidoConsultaNFe", "", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNotaRps-{numeroRPS}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, "PedidoConsultaNFe_v01.xsd");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetCliente(TipoUrl.ConsultaNFSeRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultaNFe(retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNotaRps-{numeroRPS}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "RetornoConsulta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.Sucesso = xmlRet.Root?.ElementAnyNs("Cabecalho")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            foreach (var nfse in xmlRet.Root.ElementsAnyNs("NFe"))
            {
                notas.Load(nfse.ToString());
            }

            return retornoWebservice;
        }

        private string GetHashRps(NotaFiscal nota)
        {
            string tipoTributacao;
            switch (nota.TipoTributacao)
            {
                case TipoTributacao.Tributavel:
                    tipoTributacao = "T";
                    break;

                case TipoTributacao.ForaMun:
                    tipoTributacao = "F";
                    break;

                case TipoTributacao.Isenta:
                    tipoTributacao = "A";
                    break;

                case TipoTributacao.ForaMunIsento:
                    tipoTributacao = "B";
                    break;

                case TipoTributacao.Imune:
                    tipoTributacao = "M";
                    break;

                case TipoTributacao.ForaMunImune:
                    tipoTributacao = "N";
                    break;

                case TipoTributacao.Suspensa:
                    tipoTributacao = "X";
                    break;

                case TipoTributacao.ForaMunSuspensa:
                    tipoTributacao = "V";
                    break;

                case TipoTributacao.ExpServicos:
                    tipoTributacao = "P";
                    break;

                default:
                    tipoTributacao = "?";
                    break;
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "N" : "C";
            var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? "S" : "N";

            var indCpfCnpjTomador = "3";
            switch (nota.Tomador.CpfCnpj.Length)
            {
                case 11:
                    indCpfCnpjTomador = "1";
                    break;

                case 14:
                    indCpfCnpjTomador = "2";
                    break;
            }

            // Assinatura do RPS
            string hash = nota.Prestador.InscricaoMunicipal.PadLeft(8, '0') +
                          nota.IdentificacaoRps.Serie.PadRight(5, ' ') +
                          nota.IdentificacaoRps.Numero.PadLeft(12, '0') +
                          nota.IdentificacaoRps.DataEmissao.Year.ToString().PadLeft(4, '0') +
                          nota.IdentificacaoRps.DataEmissao.Month.ToString().PadLeft(2, '0') +
                          nota.IdentificacaoRps.DataEmissao.Day.ToString().PadLeft(2, '0') +
                          tipoTributacao +
                          situacao +
                          issRetido +
                          Convert.ToInt32(nota.Servico.Valores.ValorServicos * 100).ToString().PadLeft(15, '0') +
                          Convert.ToInt32(nota.Servico.Valores.ValorDeducoes * 100).ToString().PadLeft(15, '0') +
                          nota.Servico.ItemListaServico.PadLeft(5, '0') +
                          indCpfCnpjTomador +
                          nota.Tomador.CpfCnpj.PadLeft(14, '0');
            if (!nota.Intermediario.CpfCnpj.IsEmpty())
            {
                var indCpfCnpjIntermediario = "3";
                switch (nota.Intermediario.CpfCnpj.Length)
                {
                    case 11:
                        indCpfCnpjIntermediario = "1";
                        break;

                    case 14:
                        indCpfCnpjIntermediario = "2";
                        break;
                }

                var issRetidoIntermediario = nota.Intermediario.IssRetido == SituacaoTributaria.Retencao ? "S" : "N";
                hash = hash +
                       indCpfCnpjIntermediario +
                       nota.Intermediario.CpfCnpj.PadLeft(14, '0') +
                       issRetidoIntermediario;
            }

            var rsa = (RSACryptoServiceProvider)Certificado.PrivateKey;
            var hashBytes = Encoding.ASCII.GetBytes(hash);
            byte[] signData = rsa.SignData(hashBytes, new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signData);
        }

        private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
        {
            var mensagens = xmlRet?.ElementAnyNs(xmlTag);
            if (mensagens == null)
                return;

            foreach (var mensagem in mensagens.ElementsAnyNs("Alerta"))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Descricao")?.GetValue<string>() ?? string.Empty
                };
                var chave = mensagens?.ElementAnyNs("ChaveRPSNFe");
                if (chave != null)
                {
                    evento.IdentificacaoNfse.Chave = chave.ElementAnyNs("ChaveNFe")?.GetValue<string>() ?? string.Empty;
                    evento.IdentificacaoRps.Numero = chave.ElementAnyNs("ChaveRPS")?.GetValue<string>() ?? string.Empty;
                }
                retornoWs.Alertas.Add(evento);
            }

            foreach (var mensagem in mensagens.ElementsAnyNs("Erro"))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Descricao")?.GetValue<string>() ?? string.Empty
                };
                var chave = mensagens?.ElementAnyNs("ChaveRPSNFe");
                if (chave != null)
                {
                    evento.IdentificacaoNfse.Chave = chave.ElementAnyNs("ChaveNFe")?.GetValue<string>() ?? string.Empty;
                    evento.IdentificacaoRps.Numero = chave.ElementAnyNs("ChaveRPS")?.GetValue<string>() ?? string.Empty;
                }
                retornoWs.Erros.Add(evento);
            }
        }

        #endregion Private Methods

        #endregion Methods
    }
}