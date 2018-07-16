// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-13-2017
//
// Last Modified By : RFTD
// Last Modified On : 01-13-2017
// ***********************************************************************
// <copyright file="ProviderABRASF.cs" company="ACBr.Net">
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
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Classe base para trabalhar com provedores que usam o padrão ABRASF V1
    /// </summary>
    public abstract class ProviderABRASF : ProviderBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderABRASF"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="municipio">The municipio.</param>
        protected ProviderABRASF(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "ABRASFv1";
        }

        #endregion Constructors

        #region Methods

        #region LoadXml

        public override NotaFiscal LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");

            XElement rootDoc;
            XElement rootCanc = null;
            XElement rootSub = null;

            var isRps = false;
            var isNFSe = false;

            var rootGrupo = xml.ElementAnyNs("CompNfse");
            if (rootGrupo != null)
            {
                isNFSe = true;
                rootDoc = rootGrupo.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
                rootCanc = rootGrupo.ElementAnyNs("NfseCancelamento");
                rootSub = rootGrupo.ElementAnyNs("NfseSubstituicao");
            }
            else
            {
                rootDoc = xml.ElementAnyNs("Rps");
                if (rootDoc != null)
                {
                    isRps = true;
                    rootDoc = rootDoc.ElementAnyNs("InfRps");
                }
            }

            Guard.Against<XmlException>(rootDoc == null, "Xml de RPS ou NFSe invalido.");

            var ret = new NotaFiscal();
            if (isNFSe)
            {
                // Nota Fiscal
                ret.IdentificacaoNFSe.Numero = rootDoc.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoNFSe.Chave = rootDoc.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoNFSe.DataEmissao = rootDoc.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            }

            // RPS
            var rootRps = rootDoc.ElementAnyNs("IdentificacaoRps");
            if (rootRps != null)
            {
                ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Serie = rootRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Tipo = rootRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
            }

            if (isNFSe)
                ret.IdentificacaoRps.DataEmissao = rootDoc.ElementAnyNs("DataEmissaoRps")?.GetValue<DateTime>() ?? DateTime.MinValue;
            else
                ret.IdentificacaoRps.DataEmissao = rootDoc.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Natureza da Operação
            switch (rootDoc.ElementAnyNs("NaturezaOperacao")?.GetValue<int>())
            {
                case 0:
                    ret.NaturezaOperacao = NaturezaOperacao.NT00;
                    break;

                case 1:
                    ret.NaturezaOperacao = NaturezaOperacao.NT01;
                    break;

                case 2:
                    ret.NaturezaOperacao = NaturezaOperacao.NT02;
                    break;

                case 3:
                    ret.NaturezaOperacao = NaturezaOperacao.NT03;
                    break;

                case 4:
                    ret.NaturezaOperacao = NaturezaOperacao.NT04;
                    break;

                case 5:
                    ret.NaturezaOperacao = NaturezaOperacao.NT05;
                    break;

                case 6:
                    ret.NaturezaOperacao = NaturezaOperacao.NT06;
                    break;
            }

            // Simples Nacional
            if (rootDoc.ElementAnyNs("OptanteSimplesNacional")?.GetValue<int>() == 1)
            {
                ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            }
            else
            {
                // Regime Especial de Tributaçao
                switch (rootDoc.ElementAnyNs("RegimeEspecialTributacao")?.GetValue<int>())
                {
                    case 1:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
                        break;

                    case 2:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Estimativa;
                        break;

                    case 3:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SociedadeProfissionais;
                        break;

                    case 4:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Cooperativa;
                        break;

                    case 5:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioIndividual;
                        break;

                    case 6:
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioEmpresaPP;
                        break;
                }
            }

            // Incentivador Culturalstr
            switch (rootDoc.ElementAnyNs("IncentivadorCultural")?.GetValue<int>())
            {
                case 1:
                    ret.IncentivadorCultural = NFSeSimNao.Sim;
                    break;

                case 2:
                    ret.IncentivadorCultural = NFSeSimNao.Nao;
                    break;
            }

            // Situação do RPS
            if (isRps)
            {
                ret.Situacao = (rootDoc.ElementAnyNs("Status")?.GetValue<string>() ?? string.Empty) == "2" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;
                // RPS Substituido
                var rootRpsSubstituido = rootDoc.ElementAnyNs("RpsSubstituido");
                if (rootRpsSubstituido != null)
                {
                    ret.RpsSubstituido.NumeroRps = rootRpsSubstituido.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.RpsSubstituido.Serie = rootRpsSubstituido.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                    ret.RpsSubstituido.Tipo = rootRpsSubstituido.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
                }
            }

            if (isNFSe)
            {
                ret.Competencia = rootDoc.ElementAnyNs("Competencia")?.GetValue<DateTime>() ?? DateTime.MinValue;
                ret.RpsSubstituido.NumeroNfse = rootDoc.ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
                ret.OutrasInformacoes = rootDoc.ElementAnyNs("OutrasInformacoes")?.GetValue<string>() ?? string.Empty;
            }

            // Serviços e Valores
            var rootServico = rootDoc.ElementAnyNs("Servico");
            if (rootServico != null)
            {
                var rootServicoValores = rootServico.ElementAnyNs("Valores");
                if (rootServicoValores != null)
                {
                    ret.Servico.Valores.ValorServicos = rootServicoValores.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorDeducoes = rootServicoValores.ElementAnyNs("ValorDeducoes")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorPis = rootServicoValores.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorCofins = rootServicoValores.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorInss = rootServicoValores.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorIr = rootServicoValores.ElementAnyNs("ValorIr")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorCsll = rootServicoValores.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.IssRetido = (rootServicoValores.ElementAnyNs("IssRetido")?.GetValue<int>() ?? 0) == 1 ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;
                    ret.Servico.Valores.ValorIss = rootServicoValores.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorOutrasRetencoes = rootServicoValores.ElementAnyNs("OutrasRetencoes")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.BaseCalculo = rootServicoValores.ElementAnyNs("BaseCalculo")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.Aliquota = rootServicoValores.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorLiquidoNfse = rootServicoValores.ElementAnyNs("ValorLiquidoNfse")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorIssRetido = rootServicoValores.ElementAnyNs("ValorIssRetido")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.DescontoCondicionado = rootServicoValores.ElementAnyNs("DescontoCondicionado")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.DescontoIncondicionado = rootServicoValores.ElementAnyNs("DescontoIncondicionado")?.GetValue<decimal>() ?? 0;
                }
                ret.Servico.ItemListaServico = rootServico.ElementAnyNs("ItemListaServico")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoCnae = rootServico.ElementAnyNs("CodigoCnae")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoTributacaoMunicipio = rootServico.ElementAnyNs("CodigoTributacaoMunicipio")?.GetValue<string>() ?? string.Empty;
                ret.Servico.Discriminacao = rootServico.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            }
            if (isNFSe)
            {
                ret.ValorCredito = rootDoc.ElementAnyNs("ValorCredito")?.GetValue<decimal>() ?? 0;
            }

            if (isNFSe)
            {
                // Prestador (Nota Fiscal)
                var rootPrestador = rootDoc.ElementAnyNs("PrestadorServico");
                if (rootPrestador != null)
                {
                    var rootPretadorIdentificacao = rootPrestador.ElementAnyNs("IdentificacaoPrestador");
                    if (rootPretadorIdentificacao != null)
                    {
                        ret.Prestador.CpfCnpj = rootPretadorIdentificacao.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.InscricaoMunicipal = rootPretadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                    }
                    ret.Prestador.RazaoSocial = rootPrestador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.NomeFantasia = rootPrestador.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;
                    var rootPrestadorEndereco = rootPrestador.ElementAnyNs("Endereco");
                    if (rootPrestadorEndereco != null)
                    {
                        ret.Prestador.Endereco.Logradouro = rootPrestadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Numero = rootPrestadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Complemento = rootPrestadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Bairro = rootPrestadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.CodigoMunicipio = rootPrestadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                        ret.Prestador.Endereco.Uf = rootPrestadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.Cep = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                    }
                    var rootPrestadorContato = rootPrestador.ElementAnyNs("Contato");
                    if (rootPrestadorContato != null)
                    {
                        ret.Prestador.DadosContato.DDD = "";
                        ret.Prestador.DadosContato.Telefone = rootPrestadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.DadosContato.Email = rootPrestadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                    }
                }
            }
            else
            {
                // Prestador (RPS)
                var rootPrestador = rootDoc.ElementAnyNs("Prestador");
                if (rootPrestador != null)
                {
                    ret.Prestador.CpfCnpj = rootPrestador.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.InscricaoMunicipal = rootPrestador.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Tomador
            var rootTomador = rootDoc.ElementAnyNs(isNFSe ? "TomadorServico" : "Tomador");
            if (rootTomador != null)
            {
                var rootTomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
                if (rootTomadorIdentificacao != null)
                {
                    var rootTomadorIdentificacaoCpfCnpj = rootTomadorIdentificacao.ElementAnyNs("CpfCnpj");
                    if (rootTomadorIdentificacaoCpfCnpj != null)
                    {
                        ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                        if (ret.Tomador.CpfCnpj.IsEmpty())
                        {
                            ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                        }
                    }
                    ret.Tomador.InscricaoMunicipal = rootTomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
                ret.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootTomadorEndereco = rootTomador.ElementAnyNs("Endereco");
                if (rootTomadorEndereco != null)
                {
                    ret.Tomador.Endereco.Logradouro = rootTomadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Numero = rootTomadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Complemento = rootTomadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Bairro = rootTomadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CodigoMunicipio = rootTomadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                    ret.Tomador.Endereco.Uf = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Cep = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
                if (rootTomadorContato != null)
                {
                    ret.Tomador.DadosContato.DDD = "";
                    ret.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Intermediario
            var rootIntermediarioIdentificacao = rootDoc.ElementAnyNs("IntermediarioServico");
            if (rootIntermediarioIdentificacao != null)
            {
                ret.Intermediario.RazaoSocial = rootIntermediarioIdentificacao.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootIntermediarioIdentificacaoCpfCnpj = rootIntermediarioIdentificacao.ElementAnyNs("CpfCnpj");
                if (rootIntermediarioIdentificacaoCpfCnpj != null)
                {
                    ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                    if (ret.Intermediario.CpfCnpj.IsEmpty())
                        ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                }
                ret.Intermediario.InscricaoMunicipal = rootIntermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            if (isNFSe)
            {
                // Orgão Gerador
                var rootOrgaoGerador = rootDoc.ElementAnyNs("OrgaoGerador");
                if (rootOrgaoGerador != null)
                {
                    ret.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                    ret.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Construção Civil
            var rootConstrucaoCivil = rootDoc.ElementAnyNs("ConstrucaoCivil");
            if (rootConstrucaoCivil != null)
            {
                ret.ConstrucaoCivil.CodigoObra = rootConstrucaoCivil.ElementAnyNs("CodigoObra")?.GetValue<string>() ?? string.Empty;
                ret.ConstrucaoCivil.ArtObra = rootConstrucaoCivil.ElementAnyNs("Art")?.GetValue<string>() ?? string.Empty;
            }

            // Verifica se a NFSe está cancelada
            if (rootCanc != null)
            {
                ret.Situacao = SituacaoNFSeRps.Cancelado;
                ret.Cancelamento.Pedido.CodigoCancelamento = rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?.ElementAnyNs("CodigoCancelamento")?.GetValue<string>() ?? string.Empty;
                ret.Cancelamento.DataHora = rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("DataHoraCancelamento")?.GetValue<DateTime>() ?? DateTime.MinValue;
                ret.Cancelamento.Signature = LoadSignature(rootCanc.ElementAnyNs("Signature"));
                ret.Cancelamento.Pedido.Signature = LoadSignature(rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("Pedido").ElementAnyNs("Signature"));
            }

            if (rootSub != null)
            {
                ret.RpsSubstituido.NFSeSubstituidora = rootSub.ElementAnyNs("SubstituicaoNfse").ElementAnyNs("NfseSubstituidora").GetValue<string>();
                ret.RpsSubstituido.Signature = LoadSignature(rootSub.ElementAnyNs("SubstituicaoNfse").ElementAnyNs("Signature"));
            }

            return ret;
        }

        #endregion LoadXml

        #region RPS

        public override string GetXmlRps(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xmlDoc.Add(GenerateRps(nota));
            return xmlDoc.AsString(identado, showDeclaration);
        }

        protected virtual XElement GenerateRps(NotaFiscal nota)
        {
            var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "3";
                    break;

                default:
                    tipoRps = "0";
                    break;
            }

            string tipoRpsSubstituido;
            switch (nota.RpsSubstituido.Tipo)
            {
                case TipoRps.RPS:
                    tipoRpsSubstituido = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRpsSubstituido = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRpsSubstituido = "3";
                    break;

                default:
                    tipoRpsSubstituido = "0";
                    break;
            }

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
                regimeEspecialTributacao = "6";
                optanteSimplesNacional = "1";
            }
            else
            {
                var regime = (int)nota.RegimeEspecialTributacao;
                regimeEspecialTributacao = regime == 0 ? string.Empty : regime.ToString();
                optanteSimplesNacional = "2";
            }

            var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";

            var rps = new XElement("Rps");
            var infoRps = new XElement("InfRps", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero}"));
            rps.Add(infoRps);

            var ideRps = new XElement("IdentificacaoRps");
            infoRps.Add(ideRps);

            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

            infoRps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, naturezaOperacao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
            infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, situacao));

            if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
            {
                var rpsSubstituido = new XElement("RpsSubstituido");

                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRpsSubstituido));

                infoRps.AddChild(rpsSubstituido);
            }

            var servico = GenerateServicosValoresRps(nota);
            infoRps.AddChild(servico);

            var prestador = GeneratePrestadorRps(nota);
            infoRps.AddChild(prestador);

            var tomador = GenerateTomadorRps(nota);
            infoRps.AddChild(tomador);

            if (!nota.Intermediario.RazaoSocial.IsEmpty())
            {
                var intServico = GenerateIntermediarioRps(nota);
                infoRps.AddChild(intServico);
            }

            if (!nota.ConstrucaoCivil.CodigoObra.IsEmpty())
            {
                var conCivil = GenerateConstrucaoCivilRps(nota);
                infoRps.AddChild(conCivil);
            }

            return rps;
        }

        protected virtual XElement GenerateServicosValoresRps(NotaFiscal nota)
        {
            var servico = new XElement("Servico");
            var valores = new XElement("Valores");
            servico.AddChild(valores);

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

            valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));

            // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
            valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota / 100));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

            return servico;
        }

        protected virtual XElement GeneratePrestadorRps(NotaFiscal nota)
        {
            var prestador = new XElement("Prestador");
            prestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));
            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            return prestador;
        }

        protected virtual XElement GenerateTomadorRps(NotaFiscal nota)
        {
            var tomador = new XElement("Tomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
                !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
                !nota.Tomador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                !nota.Tomador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("Contato");
                tomador.Add(contato);

                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
            }

            return tomador;
        }

        protected virtual XElement GenerateIntermediarioRps(NotaFiscal nota)
        {
            var intermediario = new XElement("Intermediario");

            var ideIntermediario = new XElement("IdentificacaoIntermediario");
            intermediario.Add(ideIntermediario);

            var cpfCnpj = new XElement("CpfCnpj");
            ideIntermediario.Add(cpfCnpj);

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

            ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.InscricaoMunicipal));

            intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.RazaoSocial));

            return intermediario;
        }

        protected virtual XElement GenerateConstrucaoCivilRps(NotaFiscal nota)
        {
            var construcao = new XElement("ConstrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        #endregion RPS

        #region NFSe

        public override string GetXmlNFSe(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var compNfse = new XElement("CompNfse");

            compNfse.AddChild(GenerateNFSe(nota));
            compNfse.AddChild(GenerateNFSeCancelamento(nota));
            compNfse.AddChild(GenerateNFSeSubstituicao(nota));

            xmlDoc.AddChild(compNfse);
            return xmlDoc.AsString(identado, showDeclaration);
        }

        protected virtual XElement GenerateNFSe(NotaFiscal nota)
        {
            var incentivadorCultural = nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2;

            string tipoRps;
            switch (nota.IdentificacaoRps.Tipo)
            {
                case TipoRps.RPS:
                    tipoRps = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRps = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRps = "3";
                    break;

                default:
                    tipoRps = "0";
                    break;
            }

            string tipoRpsSubstituido;
            switch (nota.RpsSubstituido.Tipo)
            {
                case TipoRps.RPS:
                    tipoRpsSubstituido = "1";
                    break;

                case TipoRps.NFConjugada:
                    tipoRpsSubstituido = "2";
                    break;

                case TipoRps.Cupom:
                    tipoRpsSubstituido = "3";
                    break;

                default:
                    tipoRpsSubstituido = "0";
                    break;
            }

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
                regimeEspecialTributacao = "6";
                optanteSimplesNacional = "1";
            }
            else
            {
                regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
                optanteSimplesNacional = "2";
            }

            var nfse = new XElement("Nfse");

            var infNfse = new XElement("InfNfse", new XAttribute("Id", nota.IdentificacaoNFSe.Numero));
            nfse.Add(infNfse);

            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Numero));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoVerificacao", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Chave));
            infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.DataEmissao));

            var infRps = new XElement("IdentificacaoRps");
            infNfse.Add(infRps);

            infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRps));

            infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRps", 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", 1, 1, Ocorrencia.Obrigatoria, naturezaOperacao));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
            infNfse.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

            if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
            {
                var rpsSubstituido = new XElement("RpsSubstituido");

                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
                rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, tipoRpsSubstituido));

                infNfse.AddChild(rpsSubstituido);
            }

            var servico = GenerateServicosValoresNFSe(nota);
            infNfse.AddChild(servico);

            var prestador = GeneratePrestadorNFSe(nota);
            infNfse.AddChild(prestador);

            var tomador = GenerateTomadorNFSe(nota);
            infNfse.AddChild(tomador);

            if (!nota.Intermediario.RazaoSocial.IsEmpty())
            {
                var intServico = GenerateIntermediarioNFSe(nota);
                infNfse.AddChild(intServico);
            }

            if (!nota.ConstrucaoCivil.CodigoObra.IsEmpty())
            {
                var conCivil = GenerateConstrucaoCivilNFSe(nota);
                infNfse.AddChild(conCivil);
            }

            if (nota.OrgaoGerador.CodigoMunicipio != 0)
            {
                var orgGerador = new XElement("OrgaoGerador");
                infNfse.AddChild(orgGerador);

                orgGerador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.NaoObrigatoria, nota.OrgaoGerador.CodigoMunicipio));
                orgGerador.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.OrgaoGerador.Uf));
            }

            nfse.AddChild(WriteSignature(nota.Signature));

            return nfse;
        }

        protected virtual XElement GenerateNFSeCancelamento(NotaFiscal nota)
        {
            if (nota.Situacao != SituacaoNFSeRps.Cancelado) return null;

            var nfSeCancelamento = new XElement("NfseCancelamento");

            var confirmacao = new XElement("Confirmacao");
            nfSeCancelamento.AddChild(confirmacao);

            confirmacao.AddChild(WriteSignature(nota.Cancelamento.Signature));

            var pedido = new XElement("Pedido", new XAttribute("Id", nota.Cancelamento.Id));
            confirmacao.AddChild(pedido);

            var infPedidoCancelamento = new XElement("InfPedidoCancelamento", new XAttribute("Id", nota.Cancelamento.Id));
            pedido.AddChild(infPedidoCancelamento);

            var identificacaoNfse = new XElement("IdentificacaoNfse");
            infPedidoCancelamento.AddChild(identificacaoNfse);

            identificacaoNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.Cancelamento.Pedido.IdentificacaoNFSe.Numero));
            identificacaoNfse.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj.ZeroFill(14)));
            identificacaoNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));
            identificacaoNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.CodigoMunicipio));

            infPedidoCancelamento.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCancelamento", 1, 4, Ocorrencia.Obrigatoria, nota.Cancelamento.Pedido.CodigoCancelamento));

            pedido.AddChild(WriteSignature(nota.Cancelamento.Pedido.Signature));

            confirmacao.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataHoraCancelamento", 20, 20, Ocorrencia.Obrigatoria, nota.Cancelamento.DataHora));

            return nfSeCancelamento;
        }

        protected virtual XElement GenerateNFSeSubstituicao(NotaFiscal nota)
        {
            if (nota.RpsSubstituido.NFSeSubstituidora.IsEmpty()) return null;

            var substituidora = new XElement("NfseSubstituicao");
            var substituicaoNfse = new XElement("SubstituicaoNfse", new XAttribute("Id", nota.RpsSubstituido.Id));
            substituidora.AddChild(substituicaoNfse);

            substituicaoNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "NfseSubstituidora", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NFSeSubstituidora));
            substituicaoNfse.AddChild(WriteSignature(nota.RpsSubstituido.Signature));

            return substituidora;
        }

        protected virtual XElement GenerateServicosValoresNFSe(NotaFiscal nota)
        {
            var servico = new XElement("Servico");
            var valores = new XElement("Valores");
            servico.AddChild(valores);

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));

            valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIssRetido));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.BaseCalculo));
            valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota / 100)); // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

            return servico;
        }

        protected virtual XElement GeneratePrestadorNFSe(NotaFiscal nota)
        {
            var prestador = new XElement("Prestador");

            var cpfCnpjPrestador = new XElement("CpfCnpj");
            prestador.Add(cpfCnpjPrestador);

            cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));

            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            return prestador;
        }

        protected virtual XElement GenerateTomadorNFSe(NotaFiscal nota)
        {
            var tomador = new XElement("Tomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty() || !nota.Tomador.Endereco.Numero.IsEmpty() ||
                !nota.Tomador.Endereco.Complemento.IsEmpty() || !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                nota.Tomador.Endereco.CodigoMunicipio > 0 || !nota.Tomador.Endereco.Uf.IsEmpty() ||
                !nota.Tomador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
            }

            if (!nota.Tomador.DadosContato.DDD.IsEmpty() || !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                !nota.Tomador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("Contato");
                tomador.Add(contato);

                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
            }

            return tomador;
        }

        protected virtual XElement GenerateIntermediarioNFSe(NotaFiscal nota)
        {
            var intermediario = new XElement("Intermediario");

            var ideIntermediario = new XElement("IdentificacaoIntermediario");
            intermediario.Add(ideIntermediario);

            var cpfCnpj = new XElement("CpfCnpj");
            ideIntermediario.Add(cpfCnpj);

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

            ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.InscricaoMunicipal));

            intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.RazaoSocial));

            return intermediario;
        }

        protected virtual XElement GenerateConstrucaoCivilNFSe(NotaFiscal nota)
        {
            var construcao = new XElement("ConstrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        #endregion NFSe

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
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
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
            retornoWebservice.XmlEnvio = xmlLote.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfRps", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);

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

        public override RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (numeroNFSe.IsEmpty() || codigoCancelamento.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
                return retornoWebservice;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append("<Pedido>");
            loteBuilder.Append($"<InfPedidoCancelamento Id=\"N{numeroNFSe}\">");
            loteBuilder.Append("<IdentificacaoNfse>");
            loteBuilder.Append($"<Numero>{numeroNFSe}</Numero>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
            loteBuilder.Append("</IdentificacaoNfse>");
            loteBuilder.Append($"<CodigoCancelamento>{codigoCancelamento}</CodigoCancelamento>");
            loteBuilder.Append("</InfPedidoCancelamento>");
            loteBuilder.Append("</Pedido>");
            loteBuilder.Append("</CancelarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Pedido", "InfPedidoCancelamento", Certificado);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CanNFSe-{numeroNFSe}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.CancelaNFSe));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.CancelaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.CancelarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
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
            MensagemErro(retornoWebservice, xmlRet, "CancelarNfseResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var confirmacaoCancelamento = xmlRet.ElementAnyNs("CancelarNfseResposta")?.ElementAnyNs("RetCancelamento")?.ElementAnyNs("NfseCancelamento")?.ElementAnyNs("Confirmacao");
            if (confirmacaoCancelamento == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada!" });
                return retornoWebservice;
            }

            retornoWebservice.DataLote = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = retornoWebservice.DataLote != DateTime.MinValue;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
            if (nota == null) return retornoWebservice;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;
            nota.Cancelamento.DataHora = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Cancelamento.MotivoCancelamento = motivo;

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWebservice = new RetornoWebservice();

            // Monta mensagem de envio
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarSituacaoLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarSituacao));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultarSituacao))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarSituacaoLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
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
            MensagemErro(retornoWebservice, xmlRet, "ConsultarSituacaoLoteRpsResposta");

            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Situacao = xmlRet.Root?.ElementAnyNs("Situacao")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarLoteRps(int lote, string protocolo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append($"<ConsultarLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarLoteRps));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultarLoteRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
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
            MensagemErro(retornoWebservice, xmlRet, "ConsultarLoteRpsResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResposta");
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

        public override RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (numero.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
                return retornoWebservice;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarNfseRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<IdentificacaoRps>");
            loteBuilder.Append($"<Numero>{numero}</Numero>");
            loteBuilder.Append($"<Serie>{serie}</Serie>");
            loteBuilder.Append($"<Tipo>{(int)tipo + 1}</Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfseRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNotaRps-{numero}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultaNFSeRps));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultaNFSeRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSePorRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNotaRps-{numero}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseRpsResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");
            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
                return retornoWebservice;
            }

            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);

            retornoWebservice.Sucesso = true;
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNfse, int pagina, string cnpjTomador,
            string imTomador, string nomeInter, string cnpjInter, string imInter, string serie, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");

            if (!numeroNfse.IsEmpty())
                loteBuilder.Append($"<NumeroNfse>{numeroNfse}</NumeroNfse>");

            if (inicio.HasValue && fim.HasValue)
            {
                loteBuilder.Append("<PeriodoEmissao>");
                loteBuilder.Append($"<DataInicial>{inicio:yyyy-MM-dd}</DataInicial>");
                loteBuilder.Append($"<DataFinal>{fim:yyyy-MM-dd}</DataFinal>");
                loteBuilder.Append("</PeriodoEmissao>");
            }

            if (!cnpjTomador.IsEmpty() || !imTomador.IsEmpty())
            {
                loteBuilder.Append("<Tomador>");
                loteBuilder.Append("<CpfCnpj>");
                loteBuilder.Append(cnpjTomador.IsCNPJ()
                    ? $"<Cnpj>{cnpjTomador.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{cnpjTomador.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!imTomador.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{imTomador}</InscricaoMunicipal>");
                loteBuilder.Append("</Tomador>");
            }

            if (!nomeInter.IsEmpty() && !cnpjInter.IsEmpty())
            {
                loteBuilder.Append("<IntermediarioServico>");
                loteBuilder.Append($"<RazaoSocial>{nomeInter}</RazaoSocial>");
                loteBuilder.Append(cnpjInter.IsCNPJ()
                    ? $"<Cnpj>{cnpjInter.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{cnpjInter.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!imInter.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{imInter}</InscricaoMunicipal>");
                loteBuilder.Append("</IntermediarioServico>");
            }

            loteBuilder.Append("</ConsultarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNota-{DateTime.Now:yyyyMMddssfff}-{numeroNfse}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultaNFSe));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNota-{DateTime.Now:yyyyMMddssfff}-{numeroNfse}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarNfseResposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");
            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return retornoWebservice;
            }

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                // Carrega a nota fiscal na coleção de Notas Fiscais
                var nota = LoadXml(compNfse.AsString());
                notas.Add(nota);
            }

            retornoWebservice.Sucesso = true;

            return retornoWebservice;
        }

        #endregion Services

        #region Protected Methods

        protected abstract IABRASFClient GetClient(TipoUrl tipo);

        protected virtual string GetNamespace()
        {
            return "xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"";
        }

        protected virtual string GetSchema(TipoUrl tipo)
        {
            return "nfse.xsd";
        }

        protected virtual string GerarCabecalho()
        {
            return $"<cabecalho versao=\"1.00\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\">{Environment.NewLine}<versaoDados>1.00</versaoDados>{Environment.NewLine}</cabecalho>";
        }

        protected virtual void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag,
            string elementName = "ListaMensagemRetorno", string messageElement = "MensagemRetorno")
        {
            var listaMenssagens = xmlRet?.ElementAnyNs(xmlTag)?.ElementAnyNs(elementName);
            if (listaMenssagens == null) return;

            foreach (var mensagem in listaMenssagens.ElementsAnyNs(messageElement))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                    Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
                };

                retornoWs.Erros.Add(evento);
            }
        }

        #endregion Protected Methods

        #endregion Methods
    }
}