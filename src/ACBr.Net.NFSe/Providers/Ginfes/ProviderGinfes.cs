// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 28-07-2016
//
// Last Modified By : RFTD
// Last Modified On : 19-08-2016
// ***********************************************************************
// <copyright file="ProviderGinfes.cs" company="ACBr.Net">
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using ACBr.Net.Core.Exceptions;
using System.Globalization;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
    internal sealed class ProviderGinfes : ProviderBase
    {
        #region Constructors

        public ProviderGinfes(Configuracoes config, MunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Ginfes";
        }

        #endregion Constructors

        #region Methods

        public override NotaFiscal LoadXml(XmlDocument xml)
        {
            var xmlX = xml.ToXDocument();
            Guard.Against<XmlException>(xmlX == null, "Xml de Nota invalida.");

            //Verifica qual XML deverá ser processado:
            XElement root;
            var FormatoXmlNFSe = true;

            root = xmlX.ElementAnyNs("EnviarLoteRpsEnvio");
            if (root != null)
            {
                // XML para Enviar Lote Rps
                FormatoXmlNFSe = false;
                root = root?.ElementAnyNs("LoteRps")?.ElementAnyNs("ListaRps")?.ElementAnyNs("Rps")?.ElementAnyNs("InfRps");
            }
            else
            {
                root = xmlX.ElementAnyNs("ConsultarNfseResposta") ?? xmlX.ElementAnyNs("ConsultarLoteRpsResposta");
                if (root != null)
                {
                    // XML de Consultar NFSe
                    // XML de Consultar Lote Rps
                    // Ambos retornam uma lista de NFSe, mas consideramos apenas a primeira NFSe será importada.
                    // O certo seria importar a coleção de Notas Fiscais.
                    root = root.ElementAnyNs("ListaNfse");
                }
                else
                {
                    // XML de Consultar NFSe através de um RPS
                    root = xmlX.ElementAnyNs("ConsultarNfseRpsResposta");
                }
                root = root?.ElementAnyNs("CompNfse")?.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
            }
            Guard.Against<XmlException>(root == null, "Xml de Nota invalida.");

            var ret = new NotaFiscal();

            if (FormatoXmlNFSe == true)
            {
                // Nota Fiscal
                ret.Numero = root.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.ChaveNfse = root.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                ret.DhRecebimento = root.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            }

            // RPS
            var rootRps = root.ElementAnyNs("IdentificacaoRps");
            if (rootRps != null)
            {
                ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Serie = rootRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Tipo = rootRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
            }
            if (FormatoXmlNFSe == true)
                ret.IdentificacaoRps.DataEmissaoRps = root.ElementAnyNs("DataEmissaoRps")?.GetValue<DateTime>() ?? DateTime.MinValue;
            else
                ret.IdentificacaoRps.DataEmissaoRps = root.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Natureza da Operação
            switch (root.ElementAnyNs("NaturezaOperacao")?.GetValue<char>())
            {
                case '1':
                    ret.NaturezaOperacao = NaturezaOperacao.NT01;
                    break;
                case '2':
                    ret.NaturezaOperacao = NaturezaOperacao.NT02;
                    break;
                case '3':
                    ret.NaturezaOperacao = NaturezaOperacao.NT03;
                    break;
                case '4':
                    ret.NaturezaOperacao = NaturezaOperacao.NT04;
                    break;
                case '5':
                    ret.NaturezaOperacao = NaturezaOperacao.NT05;
                    break;
                case '6':
                    ret.NaturezaOperacao = NaturezaOperacao.NT06;
                    break;
            }

            // Simples Nacional
            if (root.ElementAnyNs("OptanteSimplesNacional")?.GetValue<char>() == '1')
            {
                ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            }
            else
            {
                // Regime Especial de Tributaçao
                switch (root.ElementAnyNs("RegimeEspecialTributacao")?.GetValue<char>())
                {
                    case '1':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
                        break;
                    case '2':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Estimativa;
                        break;
                    case '3':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SociedadeProfissionais;
                        break;
                    case '4':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Cooperativa;
                        break;
                    case '5':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioIndividual;
                        break;
                    case '6':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioEmpresaPP;
                        break;
                }
            }

            // Regime Especial de Tributaçao
            switch (root.ElementAnyNs("IncentivadorCultural")?.GetValue<char>())
            {
                case '1':
                    ret.IncentivadorCultural = NFSeSimNao.Sim;
                    break;
                case '2':
                    ret.IncentivadorCultural = NFSeSimNao.Nao;
                    break;
            }

            // Situação do RPS
            if (FormatoXmlNFSe == false)
            {
                ret.Situacao = (root.ElementAnyNs("Competencia")?.GetValue<string>() ?? string.Empty) == "2" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;
                // RPS Substituido
                var rootRpsSubstituido = root.ElementAnyNs("RpsSubstituido");
                if (rootRpsSubstituido != null)
                {
                    ret.RpsSubstituido.NumeroRps = rootRpsSubstituido.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.RpsSubstituido.Serie = rootRpsSubstituido.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                }
            }

            if (FormatoXmlNFSe == true)
            {
                ret.Competencia = root.ElementAnyNs("Competencia")?.GetValue<string>() ?? string.Empty;
                ret.RpsSubstituido.NumeroNfse = root.ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
                ret.OutrasInformacoes = root.ElementAnyNs("OutrasInformacoes")?.GetValue<string>() ?? string.Empty;
            }


            // Serviços e Valores
            var rootServico = root.ElementAnyNs("Servico");
            if (rootServico != null)
            {
                var rootServicoValores = rootServico.ElementAnyNs("Valores");
                if (rootServicoValores != null)
                {
                    ret.Servico.Valores.ValorServicos = Convert.ToDecimal((rootServicoValores.ElementAnyNs("ValorServicos")?.GetValue<string>() ?? "0"), CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorDeducoes = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorDeducoes")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorPis = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorCofins = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorInss = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorIR = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorIR")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorCsll = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
                    ret.Servico.Valores.ValorIss = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorOutrasRetencoes = Convert.ToDecimal(rootServicoValores.ElementAnyNs("OutrasRetencoes")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.BaseCalculo = Convert.ToDecimal(rootServicoValores.ElementAnyNs("BaseCalculo")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.Aliquota = Convert.ToDecimal(rootServicoValores.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorLiquidoNfse = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorLiquidoNfse")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.ValorIssRetido = Convert.ToDecimal(rootServicoValores.ElementAnyNs("ValorIssRetido")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.DescontoCondicionado = Convert.ToDecimal(rootServicoValores.ElementAnyNs("DescontoCondicionado")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                    ret.Servico.Valores.DescontoIncondicionado = Convert.ToDecimal(rootServicoValores.ElementAnyNs("DescontoIncondicionado")?.GetValue<decimal>() ?? 0, CultureInfo.InvariantCulture);
                }
                ret.Servico.xItemListaServico = rootServico.ElementAnyNs("ItemListaServico")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoCnae = rootServico.ElementAnyNs("CodigoCnae")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoTributacaoMunicipio = rootServico.ElementAnyNs("CodigoTributacaoMunicipio")?.GetValue<string>() ?? string.Empty;
                ret.Servico.Discriminacao = rootServico.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
            }
            if (FormatoXmlNFSe == true)
                ret.ValorCredito = Convert.ToDecimal(root.ElementAnyNs("ValorCredito")?.GetValue<Decimal>() ?? 0, CultureInfo.InvariantCulture);

            if (FormatoXmlNFSe == true)
            {
                // Prestador (Nota Fiscal)
                var rootPrestador = root.ElementAnyNs("PrestadorServico");
                if (rootPrestador != null)
                {
                    var rootPretadorIdentificacao = rootPrestador.ElementAnyNs("IdentificacaoPrestador");
                    if (rootPretadorIdentificacao != null)
                    {
                        ret.Prestador.CPFCNPJ = rootPretadorIdentificacao.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
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
                        ret.Prestador.Endereco.CodigoMunicipio = rootPrestadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.UF = rootPrestadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Endereco.CEP = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                    }
                    var rootPrestadorContato = rootPrestador.ElementAnyNs("Contato");
                    if (rootPrestadorContato != null)
                    {
                        ret.Prestador.Contato.Telefone = rootPrestadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                        ret.Prestador.Contato.Email = rootPrestadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                    }
                }
            }
            else
            {
                // Prestador (RPS)
                var rootPrestador = root.ElementAnyNs("Prestador");
                if (rootPrestador != null)
                {
                    ret.Prestador.CPFCNPJ = rootPrestador.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.InscricaoMunicipal = rootPrestador.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Tomador
            var rootTomador = root.ElementAnyNs(FormatoXmlNFSe == true ? "TomadorServico" : "Tomador");
            if (rootTomador != null)
            {
                var rootTomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
                if (rootTomadorIdentificacao != null)
                {
                    var rootTomadorIdentificacaoCpfCnpj = rootTomadorIdentificacao.ElementAnyNs("CpfCnpj");
                    if (rootTomadorIdentificacaoCpfCnpj != null)
                    {
                        ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                        if (String.IsNullOrWhiteSpace(ret.Tomador.CpfCnpj))
                            ret.Tomador.CpfCnpj = rootTomadorIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
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
                    ret.Tomador.Endereco.CodigoMunicipio = rootTomadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.UF = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CEP = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
                if (rootTomadorContato != null)
                {
                    ret.Tomador.Contato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Contato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Intermediario
            var rootIntermediarioIdentificacao = root.ElementAnyNs("IntermediarioServico");
            if (rootIntermediarioIdentificacao != null)
            {
                ret.IntermediarioServico.RazaoSocial = rootIntermediarioIdentificacao.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootIntermediarioIdentificacaoCpfCnpj = rootIntermediarioIdentificacao.ElementAnyNs("CpfCnpj");
                if (rootIntermediarioIdentificacaoCpfCnpj != null)
                {
                    ret.IntermediarioServico.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                    if (String.IsNullOrWhiteSpace(ret.IntermediarioServico.CpfCnpj))
                        ret.IntermediarioServico.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                }
                ret.IntermediarioServico.InscricaoMunicipal = rootIntermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            if (FormatoXmlNFSe == true)
            {
                // Orgão Gerador
                var rootOrgaoGerador = root.ElementAnyNs("OrgaoGerador");
                if (rootOrgaoGerador != null)
                {
                    ret.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
                    ret.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Construção Civil
            var rootConstrucaoCivil = root.ElementAnyNs("ConstrucaoCivil");
            if (rootConstrucaoCivil != null)
            {
                ret.ConstrucaoCivil.CodigoObra = rootConstrucaoCivil.ElementAnyNs("CodigoObra")?.GetValue<string>() ?? string.Empty;
                ret.ConstrucaoCivil.Art = rootConstrucaoCivil.ElementAnyNs("Art")?.GetValue<string>() ?? string.Empty;
            }

            return ret;
        }

        public override string GetXmlRPS(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            var numberFormat = CultureInfo.InvariantCulture.NumberFormat;

            string tipoRPS, naturezaOperacao, regimeEspecialTributacao, optanteSimplesNacional, situacao, tomadorCpfCnpj;
            int issRetido;

            tipoRPS = nota.IdentificacaoRps.Tipo == TipoRps.RPS ? "1"
                : nota.IdentificacaoRps.Tipo == TipoRps.NFConjugada ? "2"
                : nota.IdentificacaoRps.Tipo == TipoRps.Cupom ? "3"
                : "0";

            naturezaOperacao = nota.NaturezaOperacao == NaturezaOperacao.NT01 ? "1"
                : nota.NaturezaOperacao == NaturezaOperacao.NT02 ? "2"
                : nota.NaturezaOperacao == NaturezaOperacao.NT03 ? "3"
                : nota.NaturezaOperacao == NaturezaOperacao.NT04 ? "4"
                : nota.NaturezaOperacao == NaturezaOperacao.NT05 ? "5"
                : nota.NaturezaOperacao == NaturezaOperacao.NT06 ? "6"
                : "0";

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
            situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";
            issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2;
            tomadorCpfCnpj = nota.Tomador.CpfCnpj.Trim().OnlyNumbers();

            var xmlRPS = new StringBuilder();
            xmlRPS.Append("<tipos:Rps>");
            {
                xmlRPS.Append("<tipos:InfRps>");
                {
                    xmlRPS.Append("<tipos:IdentificacaoRps>");
                    {
                        xmlRPS.Append($"<tipos:Numero>{nota.IdentificacaoRps.Numero}</tipos:Numero>");
                        xmlRPS.Append($"<tipos:Serie>{nota.IdentificacaoRps.Serie}</tipos:Serie>");
                        xmlRPS.Append($"<tipos:Tipo>{tipoRPS}</tipos:Tipo>");
                    }
                    xmlRPS.Append("</tipos:IdentificacaoRps>");
                    xmlRPS.Append($"<tipos:DataEmissao>{nota.IdentificacaoRps.DataEmissaoRps.ToString("yyyy-MM-ddTHH:MM:ss")}</tipos:DataEmissao>");
                    xmlRPS.Append($"<tipos:NaturezaOperacao>{naturezaOperacao}</tipos:NaturezaOperacao>");
                    xmlRPS.Append($"<tipos:RegimeEspecialTributacao>{regimeEspecialTributacao}</tipos:RegimeEspecialTributacao>");
                    xmlRPS.Append($"<tipos:OptanteSimplesNacional>{optanteSimplesNacional}</tipos:OptanteSimplesNacional>");
                    xmlRPS.Append($"<tipos:IncentivadorCultural>{optanteSimplesNacional}</tipos:IncentivadorCultural>");
                    xmlRPS.Append($"<tipos:Status>{situacao}</tipos:Status>");
                    if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
                    {
                        xmlRPS.Append("<tipos:RpsSubstituido>");
                        {
                            xmlRPS.Append($"<tipos:Numero>{nota.RpsSubstituido.NumeroRps}</tipos:Numero>");
                            xmlRPS.Append($"<tipos:Serie>{nota.RpsSubstituido.Serie}</tipos:Serie>");
                            xmlRPS.Append($"<tipos:Tipo>1</tipos:Tipo>");  //  RPSSubstituido não contém a propriedade Tipo (para manter o padrão do IdentificacaoRPS - não sei se neste caso se aplica, pois podemos mandar um "RPS" do tipo "cupom" ou "Nota Fiscal Conjugada".Talvez seja impossível "substituir" um RPS do tipo "cupom".Se for o caso, desconsiderar.)
                        }
                        xmlRPS.Append("</tipos:RpsSubstituido>");
                    }
                    xmlRPS.Append("<tipos:Servico>");
                    {
                        xmlRPS.Append("<tipos:Valores>");
                        {
                            xmlRPS.Append($"<tipos:ValorServicos>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorServicos)}</tipos:ValorServicos>");
                            if (nota.Servico.Valores.ValorDeducoes != 0)
                                xmlRPS.Append($"<tipos:ValorDeducoes>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorDeducoes)}</tipos:ValorDeducoes>");
                            if (nota.Servico.Valores.ValorPis != 0)
                                xmlRPS.Append($"<tipos:ValorPis>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorPis)}</tipos:ValorPis>");
                            if (nota.Servico.Valores.ValorCofins != 0)
                                xmlRPS.Append($"<tipos:ValorCofins>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorCofins)}</tipos:ValorCofins>");
                            if (nota.Servico.Valores.ValorInss != 0)
                                xmlRPS.Append($"<tipos:ValorInss>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorInss)}</tipos:ValorInss>");
                            if (nota.Servico.Valores.ValorIR != 0)
                                xmlRPS.Append($"<tipos:ValorIr>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorIR)}</tipos:ValorIr>");
                            if (nota.Servico.Valores.ValorCsll != 0)
                                xmlRPS.Append($"<tipos:ValorCsll>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorCsll)}</tipos:ValorCsll>");
                            xmlRPS.Append($"<tipos:IssRetido>{issRetido}</tipos:IssRetido>");
                            if (nota.Servico.Valores.ValorIss != 0)
                                xmlRPS.Append($"<tipos:ValorIss>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorIss)}</tipos:ValorIss>");
                            if (nota.Servico.Valores.ValorIssRetido != 0)
                                xmlRPS.Append($"<tipos:ValorIssRetido>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorIssRetido)}</tipos:ValorIssRetido>");
                            if (nota.Servico.Valores.ValorOutrasRetencoes != 0)
                                xmlRPS.Append($"<tipos:OutrasRetencoes>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorOutrasRetencoes)}</tipos:OutrasRetencoes>");
                            if (nota.Servico.Valores.BaseCalculo != 0)
                                xmlRPS.Append($"<tipos:BaseCalculo>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.BaseCalculo)}</tipos:BaseCalculo>");
                            if (nota.Servico.Valores.Aliquota != 0)
                                xmlRPS.Append($"<tipos:Aliquota>{string.Format(numberFormat, "{0:0.0000}", nota.Servico.Valores.Aliquota)}</tipos:Aliquota>");
                            if (nota.Servico.Valores.ValorLiquidoNfse != 0)
                                xmlRPS.Append($"<tipos:ValorLiquidoNfse>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.ValorLiquidoNfse)}</tipos:ValorLiquidoNfse>");
                            if (nota.Servico.Valores.DescontoIncondicionado != 0)
                                xmlRPS.Append($"<tipos:DescontoIncondicionado>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.DescontoIncondicionado)}</tipos:DescontoIncondicionado>");
                            if (nota.Servico.Valores.DescontoCondicionado != 0)
                                xmlRPS.Append($"<tipos:DescontoCondicionado>{string.Format(numberFormat, "{0:0.00}", nota.Servico.Valores.DescontoCondicionado)}</tipos:DescontoCondicionado>");
                        }
                        xmlRPS.Append("</tipos:Valores>");
                        xmlRPS.Append($"<tipos:ItemListaServico>{nota.Servico.xItemListaServico}</tipos:ItemListaServico>");
                        if (!string.IsNullOrWhiteSpace(nota.Servico.CodigoCnae))
                            xmlRPS.Append($"<tipos:CodigoCnae>{nota.Servico.CodigoCnae}</tipos:CodigoCnae>");
                        xmlRPS.Append($"<tipos:CodigoTributacaoMunicipio>{nota.Servico.CodigoTributacaoMunicipio}</tipos:CodigoTributacaoMunicipio>");
                        xmlRPS.Append($"<tipos:Discriminacao>{nota.Servico.Discriminacao}</tipos:Discriminacao>");
                        xmlRPS.Append($"<tipos:CodigoMunicipio>{nota.Servico.CodigoMunicipio}</tipos:CodigoMunicipio>");
                    }
                    xmlRPS.Append("</tipos:Servico>");
                    xmlRPS.Append("<tipos:Prestador>");
                    {
                        xmlRPS.Append($"<tipos:Cnpj>{nota.Prestador.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
                        xmlRPS.Append($"<tipos:InscricaoMunicipal>{nota.Prestador.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
                    }
                    xmlRPS.Append("</tipos:Prestador>");
                    xmlRPS.Append("<tipos:Tomador>");
                    {
                        xmlRPS.Append("<tipos:IdentificacaoTomador>");
                        {
                            xmlRPS.Append("<tipos:CpfCnpj>");
                            {
                                if (tomadorCpfCnpj.Length == 11)
                                    xmlRPS.Append($"<tipos:Cpf>{tomadorCpfCnpj}</tipos:Cpf>");
                                else
                                    xmlRPS.Append($"<tipos:Cnpj>{tomadorCpfCnpj}</tipos:Cnpj>");
                            }
                            xmlRPS.Append("</tipos:CpfCnpj>");
                        }
                        xmlRPS.Append("</tipos:IdentificacaoTomador>");
                        xmlRPS.Append($"<tipos:RazaoSocial>{nota.Tomador.RazaoSocial}</tipos:RazaoSocial>");
                        xmlRPS.Append("<tipos:Endereco>");
                        {
                            xmlRPS.Append($"<tipos:Endereco>{nota.Tomador.Endereco.Logradouro}</tipos:Endereco>");
                            xmlRPS.Append($"<tipos:Numero>{nota.Tomador.Endereco.Numero}</tipos:Numero>");
                            if (!string.IsNullOrWhiteSpace(nota.Tomador.Endereco.Complemento))
                                xmlRPS.Append($"<tipos:Complemento>{nota.Tomador.Endereco.Complemento}</tipos:Complemento>");
                            xmlRPS.Append($"<tipos:Bairro>{nota.Tomador.Endereco.Bairro}</tipos:Bairro>");
                            xmlRPS.Append($"<tipos:CodigoMunicipio>{nota.Tomador.Endereco.CodigoMunicipio}</tipos:CodigoMunicipio>");
                            xmlRPS.Append($"<tipos:Uf>{nota.Tomador.Endereco.UF}</tipos:Uf>");
                            xmlRPS.Append($"<tipos:Cep>{nota.Tomador.Endereco.CEP}</tipos:Cep>");
                        }
                        xmlRPS.Append("</tipos:Endereco>");
                        xmlRPS.Append("<tipos:Contato>");
                        {
                            xmlRPS.Append($"<tipos:Telefone>{nota.Tomador.Contato.Telefone}</tipos:Telefone>");
                            xmlRPS.Append($"<tipos:Email>{nota.Tomador.Contato.Email}</tipos:Email>");
                        }
                        xmlRPS.Append("</tipos:Contato>");
                    }
                    xmlRPS.Append("</tipos:Tomador>");

                }
                xmlRPS.Append("</tipos:InfRps>");
            }
            xmlRPS.Append("</tipos:Rps>");
            return xmlRPS.ToString();
        }

        public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadorPadrao.CPFCNPJ,
                CodCidade = Config.WebServices.CodMunicipio,
                DataEnvioLote = DateTime.Now,
                NumeroLote = "0",
                Assincrono = true
            };

            if (lote == 0)
                retornoWebservice.Erros.Add(new Evento() { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0)
                retornoWebservice.Erros.Add(new Evento() { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;

            var xmlLoteRps = new StringBuilder();
            foreach (var nota in notas)
            {
                var xmlRps = GetXmlRPS(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissaoRps:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissaoRps);
            }

            var loteRps = GerarEnvelopeEnvio(lote, notas.Count, xmlLoteRps.ToString());

            retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteRps, "", "EnviarLoteRpsEnvio", Certificado, true);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-{lote}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_enviar_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.Enviar);
                retornoWebservice.XmlRetorno = cliente.RecepcionarLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
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
            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;


            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.DataEnvioLote = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = (!string.IsNullOrWhiteSpace(retornoWebservice.NumeroLote));

            if (!retornoWebservice.Sucesso)
                return retornoWebservice;

            foreach (var nota in notas)
                nota.NumeroLote = retornoWebservice.NumeroLote;

            return retornoWebservice;
        }


        public override RetornoWebservice ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWebservice = new RetornoWebservice()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadorPadrao.CPFCNPJ,
                CodCidade = Config.WebServices.CodMunicipio,
                DataEnvioLote = DateTime.Now,
                NumeroLote = "0",
                Assincrono = true
            };

            // Monta mensagem de envio
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultarSituacaoLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_situacao_lote_rps_envio_v03.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarSituacaoLoteRpsEnvio", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_consultar_situacao_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.ConsultarSituacao);
                var cabecalho = GerarCabecalho();
                retornoWebservice.XmlRetorno = cliente.ConsultarSituacao(cabecalho, retornoWebservice.XmlEnvio);
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "ConsultarSituacaoLoteRpsResposta");
            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;

            retornoWebservice.Situacao = xmlRet.Root?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
            retornoWebservice.Sucesso = (retornoWebservice.Situacao == "4");
            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;

            return retornoWebservice;
        }
        public override RetornoWebservice ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadorPadrao.CPFCNPJ,
                CodCidade = Config.WebServices.CodMunicipio,
                DataEnvioLote = DateTime.Now,
                NumeroLote = "0",
                Assincrono = true
            };

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultarLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_lote_rps_envio_v03.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");

            retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarLoteRpsEnvio", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_consultar_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.ConsultarLoteRps);
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
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet, "ConsultarLoteRpsResposta");
            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");
            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return retornoWebservice;
            }

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                    continue;

                nota.Numero = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                nota.ChaveNfse = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var xml = GetXmlNFSe(nota);
                GravarNFSeEmDisco(xml, $"NFSe-{nota.ChaveNfse}-{nota.Numero}.xml", nota.DhRecebimento);
            }
            return retornoWebservice;
        }

        #endregion Methods

        #region Private Methods
        private IGinfesServiceClient GetCliente(TipoUrl tipo)
        {
            var url = GetUrl(tipo);
            if (Config.WebServices.Ambiente == TipoAmbiente.Homologacao)
            {
                return new GinfesHomServiceClient(url, TimeOut, Certificado);
            }

            return new GinfesProdServiceClient(url, TimeOut, Certificado);
        }

        private static string GerarCabecalho()
        {
            var cabecalho = new StringBuilder();
            cabecalho.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            cabecalho.Append("<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\">");
            cabecalho.Append("<versaoDados>3</versaoDados>");
            cabecalho.Append("</ns2:cabecalho>");
            return cabecalho.ToString();
        }

        private string GerarEnvelopeEnvio(int numeroLote, int quantidadeRps, string xmlLoteRps)
        {
            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append("<EnviarLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_enviar_lote_rps_envio_v03.xsd\">");
            xmlLote.Append("<LoteRps>");
            xmlLote.Append($"<tipos:NumeroLote>{numeroLote}</tipos:NumeroLote>");
            xmlLote.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            xmlLote.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            xmlLote.Append($"<tipos:QuantidadeRps>{quantidadeRps}</tipos:QuantidadeRps>");
            xmlLote.Append("<tipos:ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</tipos:ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            return xmlLote.ToString();
        }

        private static void MensagemErro(RetornoWebservice retornoWs, XDocument xmlRet, string xmlTag)
        {
            XElement mensagens = xmlRet?.ElementAnyNs(xmlTag);
            mensagens = mensagens?.ElementAnyNs("ListaMensagemRetorno");
            if (mensagens == null)
                return;

            foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
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

        #endregion Private Methods

    }
}