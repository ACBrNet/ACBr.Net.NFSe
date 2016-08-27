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

using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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

		public override NotaFiscal LoadXml(XDocument xml)
		{
			Guard.Against<XmlException>(xml == null, "Xml de Nota invalida.");

			//Verifica qual XML deverá ser processado:
			var formatoXmlNFSe = true;

			var root = xml.ElementAnyNs("EnviarLoteRpsEnvio");
			if (root != null)
			{
				// XML para Enviar Lote Rps
				formatoXmlNFSe = false;
				root = root.ElementAnyNs("LoteRps")?.ElementAnyNs("ListaRps")?.ElementAnyNs("Rps")?.ElementAnyNs("InfRps");
			}
			else
			{
				root = xml.ElementAnyNs("ConsultarNfseResposta") ?? xml.ElementAnyNs("ConsultarLoteRpsResposta");
				root = root != null ? root.ElementAnyNs("ListaNfse") : xml.ElementAnyNs("ConsultarNfseRpsResposta");
				root = root?.ElementAnyNs("CompNfse")?.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
			}

			Guard.Against<XmlException>(root == null, "Xml de Nota invalida.");

			var ret = new NotaFiscal();

			if (formatoXmlNFSe)
			{
				// Nota Fiscal
				ret.IdentificacaoNFSe.Numero = root.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
				ret.IdentificacaoNFSe.Chave = root.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
				ret.IdentificacaoNFSe.DataEmissao = root.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
			}

			// RPS
			var rootRps = root.ElementAnyNs("IdentificacaoRps");
			if (rootRps != null)
			{
				ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
				ret.IdentificacaoRps.Serie = rootRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
				ret.IdentificacaoRps.Tipo = rootRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
			}

			if (formatoXmlNFSe)
				ret.IdentificacaoRps.DataEmissao = root.ElementAnyNs("DataEmissaoRps")?.GetValue<DateTime>() ?? DateTime.MinValue;
			else
				ret.IdentificacaoRps.DataEmissao = root.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

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
			if (formatoXmlNFSe == false)
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

			if (formatoXmlNFSe == true)
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
					ret.Servico.Valores.ValorServicos = rootServicoValores.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.ValorDeducoes = rootServicoValores.ElementAnyNs("ValorDeducoes")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.ValorPis = rootServicoValores.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.ValorCofins = rootServicoValores.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.ValorInss = rootServicoValores.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.ValorIr = rootServicoValores.ElementAnyNs("ValorIr")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.ValorCsll = rootServicoValores.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
					ret.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
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
				ret.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
			}
			if (formatoXmlNFSe == true)
				ret.ValorCredito = root.ElementAnyNs("ValorCredito")?.GetValue<Decimal>() ?? 0;

			if (formatoXmlNFSe == true)
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
						ret.Prestador.Endereco.Uf = rootPrestadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
						ret.Prestador.Endereco.Cep = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
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
			var rootTomador = root.ElementAnyNs(formatoXmlNFSe == true ? "TomadorServico" : "Tomador");
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
					ret.Tomador.Endereco.Uf = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
					ret.Tomador.Endereco.Cep = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
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

			if (formatoXmlNFSe == true)
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

			var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";
			var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2;

			var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

			XNamespace ns = "http://www.ginfes.com.br/tipos_v03.xsd";
			var rps = new XElement(ns + "Rps", new XAttribute(XNamespace.Xmlns + "tipos", ns));
			xmlDoc.Add(rps);

			var infoRps = new XElement(ns + "InfRps");
			rps.Add(infoRps);

			var ideRps = new XElement(ns + "IdentificacaoRps");
			infoRps.Add(ideRps);

			ideRps.AddChild(AdicionarTag(TipoCampo.Int, "ID1", "Numero", ns, 1, 15, 1, nota.IdentificacaoRps.Numero));
			ideRps.AddChild(AdicionarTag(TipoCampo.Int, "ID2", "Serie", ns, 1, 5, 1, nota.IdentificacaoRps.Serie));
			ideRps.AddChild(AdicionarTag(TipoCampo.Int, "ID3", "Tipo", ns, 1, 1, 1, tipoRps));

			infoRps.AddChild(AdicionarTag(TipoCampo.DatHor, "IR1", "DataEmissao", ns, 20, 20, 1, nota.IdentificacaoRps.DataEmissao));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "IR2", "NaturezaOperacao", ns, 1, 1, 1, naturezaOperacao));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "IR3", "RegimeEspecialTributacao", ns, 1, 1, 0, regimeEspecialTributacao));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "IR4", "OptanteSimplesNacional", ns, 1, 1, 1, optanteSimplesNacional));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "IR5", "IncentivadorCultural", ns, 1, 1, 1, incentivadorCultural));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "IR6", "Status", ns, 1, 1, 1, situacao));

			if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
			{
				var rpsSubstituido = new XElement(ns + "RpsSubstituido");

				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "RS1", "Numero", ns, 1, 15, 1, nota.RpsSubstituido.NumeroRps));
				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "RS2", "Serie", ns, 1, 5, 1, nota.RpsSubstituido.Serie));
				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "RS3", "Tipo", ns, 1, 1, 1, 1));

				infoRps.AddChild(rpsSubstituido);
			}

			var servico = new XElement(ns + "Servico");
			infoRps.AddChild(servico);

			var valores = new XElement(ns + "Valores");
			servico.AddChild(valores);

			valores.AddChild(AdicionarTag(TipoCampo.De2, "VL01", "ValorServicos", ns, 1, 15, 1, nota.Servico.Valores.ValorServicos));

			if (nota.Servico.Valores.ValorDeducoes != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL02", "ValorDeducoes", ns, 1, 15, 0, nota.Servico.Valores.ValorDeducoes));

			if (nota.Servico.Valores.ValorPis != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL03", "ValorPis", ns, 1, 15, 0, nota.Servico.Valores.ValorPis));

			if (nota.Servico.Valores.ValorCofins != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL04", "ValorCofins", ns, 1, 15, 0, nota.Servico.Valores.ValorCofins));

			if (nota.Servico.Valores.ValorInss != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL05", "ValorInss", ns, 1, 15, 0, nota.Servico.Valores.ValorInss));

			if (nota.Servico.Valores.ValorIr != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL06", "ValorIr", ns, 1, 15, 0, nota.Servico.Valores.ValorIr));

			if (nota.Servico.Valores.ValorCsll != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL07", "ValorCsll", ns, 1, 15, 0, nota.Servico.Valores.ValorCsll));

			valores.AddChild(AdicionarTag(TipoCampo.Int, "VL08", "IssRetido", ns, 1, 1, 1, issRetido));

			if (nota.Servico.Valores.ValorIss != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL09", "ValorIss", ns, 1, 15, 0, nota.Servico.Valores.ValorIss));

			if (nota.Servico.Valores.ValorIssRetido != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL10", "ValorIssRetido", ns, 1, 15, 0, nota.Servico.Valores.ValorIssRetido));

			if (nota.Servico.Valores.OutrasRetencoes != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL11", "OutrasRetencoes", ns, 1, 15, 0, nota.Servico.Valores.OutrasRetencoes));

			if (nota.Servico.Valores.BaseCalculo != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL12", "BaseCalculo", ns, 1, 15, 0, nota.Servico.Valores.BaseCalculo));

			if (nota.Servico.Valores.Aliquota != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL13", "Aliquota", ns, 1, 15, 0, nota.Servico.Valores.Aliquota));

			if (nota.Servico.Valores.ValorLiquidoNfse != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL14", "ValorLiquidoNfse", ns, 1, 15, 0, nota.Servico.Valores.ValorLiquidoNfse));

			if (nota.Servico.Valores.DescontoIncondicionado != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL15", "DescontoIncondicionado", ns, 1, 15, 0, nota.Servico.Valores.DescontoIncondicionado));

			if (nota.Servico.Valores.DescontoCondicionado != 0)
				valores.AddChild(AdicionarTag(TipoCampo.De2, "VL16", "DescontoCondicionado", ns, 1, 15, 0, nota.Servico.Valores.DescontoCondicionado));

			servico.AddChild(AdicionarTag(TipoCampo.Str, "SE1", "ItemListaServico", ns, 1, 5, 1, nota.Servico.ItemListaServico));
			servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "SE2", "CodigoCnae", ns, 1, 7, 0, nota.Servico.CodigoCnae));
			servico.AddChild(AdicionarTag(TipoCampo.Str, "SE3", "CodigoTributacaoMunicipio", ns, 1, 20, 0, nota.Servico.CodigoTributacaoMunicipio));
			servico.AddChild(AdicionarTag(TipoCampo.Str, "SE4", "Discriminacao", ns, 1, 2000, 1, nota.Servico.Discriminacao));
			servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "SE5", "CodigoMunicipio", ns, 1, 7, 1, nota.Servico.CodigoMunicipio));

			var prestador = new XElement(ns + "Prestador");
			infoRps.AddChild(prestador);

			prestador.AddChild(AdicionarTagCNPJCPF("Cpf", "Cnpj", nota.Prestador.CPFCNPJ.ZeroFill(14), ns));
			prestador.AddChild(AdicionarTag(TipoCampo.StrNumber, "SE5", "InscricaoMunicipal", ns, 1, 15, 1, nota.Prestador.InscricaoMunicipal));

			var tomador = new XElement(ns + "Tomador");
			infoRps.AddChild(tomador);

			var ideTomador = new XElement(ns + "IdentificacaoTomador");
			tomador.AddChild(ideTomador);

			var cpfCnpj = new XElement(ns + "CpfCnpj");
			ideTomador.AddChild(cpfCnpj);

			cpfCnpj.AddChild(AdicionarTagCNPJCPF("Cpf", "Cnpj", nota.Tomador.CpfCnpj, ns));
			ideTomador.AddChild(AdicionarTag(TipoCampo.StrNumber, "IT2", "InscricaoMunicipal", ns, 1, 15, 0, nota.Tomador.InscricaoMunicipal));

			tomador.AddChild(AdicionarTag(TipoCampo.Str, "IT2", "RazaoSocial", ns, 1, 115, 0, nota.Tomador.RazaoSocial));
			if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
			{
				var endereco = new XElement(ns + "Endereco");
				tomador.AddChild(endereco);

				endereco.AddChild(AdicionarTag(TipoCampo.Str, "TE1", "Endereco", ns, 1, 125, 1, nota.Tomador.Endereco.Logradouro));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "TE2", "Numero", ns, 1, 10, 1, nota.Tomador.Endereco.Numero));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "TE3", "Complemento", ns, 1, 10, 0, nota.Tomador.Endereco.Complemento));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "TE4", "Bairro", ns, 1, 60, 0, nota.Tomador.Endereco.Bairro));
				endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "TE5", "CodigoMunicipio", ns, 1, 7, 0, nota.Tomador.Endereco.CodigoMunicipio));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "TE6", "Uf", ns, 2, 2, 0, nota.Tomador.Endereco.Uf));
				endereco.AddChild(AdicionarTag(TipoCampo.StrNumberFill, "TE7", "Cep", ns, 8, 8, 0, nota.Tomador.Endereco.Cep));
			}

			var contato = new XElement(ns + "Contato");
			tomador.AddChild(contato);

			contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "CT1", "Telefone", ns, 1, 11, 1, nota.Tomador.Contato.Telefone));
			contato.AddChild(AdicionarTag(TipoCampo.Str, "CT2", "Email", ns, 1, 80, 1, nota.Tomador.Contato.Email));

			if (!nota.IntermediarioServico.RazaoSocial.IsEmpty())
			{
				var intServico = new XElement(ns + "IntermediarioServico");
				infoRps.AddChild(intServico);

				intServico.AddChild(AdicionarTag(TipoCampo.Str, "IS1", "RazaoSocial", ns, 1, 115, 0, nota.IntermediarioServico.RazaoSocial));

				var intServicoCpfCnpj = new XElement(ns + "CpfCnpj");
				intServico.AddChild(intServicoCpfCnpj);

				cpfCnpj.AddChild(AdicionarTagCNPJCPF("Cpf", "Cnpj", nota.IntermediarioServico.CpfCnpj, ns));

				intServico.AddChild(AdicionarTag(TipoCampo.StrNumber, "IS3", "InscricaoMunicipal", ns, 1, 15, 0, nota.IntermediarioServico.InscricaoMunicipal));
			}

			if (!nota.ConstrucaoCivil.CodigoObra.IsEmpty())
			{
				var conCivil = new XElement(ns + "ConstrucaoCivil");
				infoRps.AddChild(conCivil);

				conCivil.AddChild(AdicionarTag(TipoCampo.Str, "CC1", "CodigoObra", ns, 1, 15, 1, nota.ConstrucaoCivil.CodigoObra));
				conCivil.AddChild(AdicionarTag(TipoCampo.Str, "CC2", "Art", ns, 1, 15, 1, nota.ConstrucaoCivil.Art));
			}

			return xmlDoc.AsString(false, false);
		}

		public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
		{
			var retornoWebservice = new RetornoWebservice
			{
				Sucesso = false,
				CPFCNPJRemetente = Config.PrestadorPadrao.CPFCNPJ,
				CodCidade = Config.WebServices.CodMunicipio,
				DataEnvioLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = true
			};

			if (lote == 0)
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
			}

			if (notas.Count == 0)
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
			}

			if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

			var xmlLoteRps = new StringBuilder();
			foreach (var nota in notas)
			{
				var xmlRps = GetXmlRPS(nota, false, false);
				xmlLoteRps.Append(xmlRps);
				GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
			}

			var loteRps = GerarEnvelopeEnvio(lote, notas.Count, xmlLoteRps.ToString());
			loteRps = CertificadoDigital.AssinarXmlTodos(loteRps, "", "tipos:Rps", Certificado);

			retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteRps, "", "EnviarLoteRpsEnvio", Certificado);

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

				nota.IdentificacaoNFSe.Numero = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
				nota.IdentificacaoNFSe.Chave = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

				var xml = GetXmlNFSe(nota);
				GravarNFSeEmDisco(xml, $"NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml", nota.IdentificacaoNFSe.DataEmissao);
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

		private static void MensagemErro(RetornoWebservice retornoWs, XContainer xmlRet, string xmlTag)
		{
			var mensagens = xmlRet?.ElementAnyNs(xmlTag);
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