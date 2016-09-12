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
		#region Internal Types

		private enum LoadXmlFormato
		{
			Indefinido,
			NFSe,
			Rps
		}

		#endregion Internal Types

		#region Constructors

		public ProviderGinfes(ConfiguracoesNFSe config, MunicipioNFSe municipio) : base(config, municipio)
		{
			Name = "Ginfes";
		}

		#endregion Constructors

		#region Methods

		public override NotaFiscal LoadXml(XDocument xml)
		{
			Guard.Against<XmlException>(xml == null, "Xml invalido.");

			XElement rootDoc;
			XElement rootCanc = null;
			var formatoXml = LoadXmlFormato.Indefinido;

			var rootGrupo = xml.ElementAnyNs("CompNfse");
			if (rootGrupo != null)
			{
				formatoXml = LoadXmlFormato.NFSe;
				rootDoc = rootGrupo.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
				rootCanc = rootGrupo.ElementAnyNs("NfseCancelamento")?.ElementAnyNs("Confirmacao");
			}
			else
			{
				rootDoc = xml.ElementAnyNs("Rps");
				if (rootDoc != null)
				{
					formatoXml = LoadXmlFormato.Rps;
					rootDoc = rootDoc.ElementAnyNs("InfRps");
				}
			}

			Guard.Against<XmlException>(rootDoc == null, "Xml de RPS ou NFSe invalido.");

			var ret = new NotaFiscal(Config, false);

			if (formatoXml == LoadXmlFormato.NFSe)
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

			if (formatoXml == LoadXmlFormato.NFSe)
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

			// Regime Especial de Tributaçao
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
			if (formatoXml == LoadXmlFormato.Rps)
			{
				ret.Situacao = (rootDoc.ElementAnyNs("Competencia")?.GetValue<string>() ?? string.Empty) == "2" ? SituacaoNFSeRps.Cancelado : SituacaoNFSeRps.Normal;
				// RPS Substituido
				var rootRpsSubstituido = rootDoc.ElementAnyNs("RpsSubstituido");
				if (rootRpsSubstituido != null)
				{
					ret.RpsSubstituido.NumeroRps = rootRpsSubstituido.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
					ret.RpsSubstituido.Serie = rootRpsSubstituido.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
					ret.RpsSubstituido.Tipo = rootRpsSubstituido.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
				}
			}

			if (formatoXml == LoadXmlFormato.NFSe)
			{
				ret.Competencia = rootDoc.ElementAnyNs("Competencia")?.GetValue<string>() ?? string.Empty;
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
				ret.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
			}
			if (formatoXml == LoadXmlFormato.NFSe)
				ret.ValorCredito = rootDoc.ElementAnyNs("ValorCredito")?.GetValue<Decimal>() ?? 0;

			if (formatoXml == LoadXmlFormato.NFSe)
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
						ret.Prestador.Endereco.CodigoMunicipio = rootPrestadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
						ret.Prestador.Endereco.Uf = rootPrestadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
						ret.Prestador.Endereco.Cep = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
					}
					var rootPrestadorContato = rootPrestador.ElementAnyNs("Contato");
					if (rootPrestadorContato != null)
					{
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
			var rootTomador = rootDoc.ElementAnyNs(formatoXml == LoadXmlFormato.NFSe ? "TomadorServico" : "Tomador");
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
					if (String.IsNullOrWhiteSpace(ret.Intermediario.CpfCnpj))
						ret.Intermediario.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
				}
				ret.Intermediario.InscricaoMunicipal = rootIntermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
			}

			if (formatoXml == LoadXmlFormato.NFSe)
			{
				// Orgão Gerador
				var rootOrgaoGerador = rootDoc.ElementAnyNs("OrgaoGerador");
				if (rootOrgaoGerador != null)
				{
					ret.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
					ret.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
				}
			}

			// Construção Civil
			var rootConstrucaoCivil = rootDoc.ElementAnyNs("ConstrucaoCivil");
			if (rootConstrucaoCivil != null)
			{
				ret.ConstrucaoCivil.CodigoObra = rootConstrucaoCivil.ElementAnyNs("CodigoObra")?.GetValue<string>() ?? string.Empty;
				ret.ConstrucaoCivil.Art = rootConstrucaoCivil.ElementAnyNs("Art")?.GetValue<string>() ?? string.Empty;
			}

			// Verifica se a NFSe está cancelada
			if (rootCanc != null)
			{
				if (rootCanc.ElementAnyNs("InfConfirmacaoCancelamento")?.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false == true) ;
				{
					ret.Situacao = SituacaoNFSeRps.Cancelado;
					ret.Cancelamento.Pedido.CodigoCancelamento = rootCanc.ElementAnyNs("InfPedidoCancelamento")?.ElementAnyNs("CodigoCancelamento")?.GetValue<string>() ?? string.Empty;
					ret.Cancelamento.DataHora = rootCanc.ElementAnyNs("InfConfirmacaoCancelamento")?.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
				}
			}

			return ret;
		}

		public override string GetXmlRps(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
		{
			return GetXmlRps(nota, identado, showDeclaration, false);
		}

		public override string GetXmlNFSe(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
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

			var situacao = nota.Situacao == SituacaoNFSeRps.Normal ? "1" : "2";
			var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2;

			var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

			XNamespace ns = "http://www.ginfes.com.br/tipos_v03.xsd";
			var compNfse = new XElement("CompNfse", new XAttribute(XNamespace.Xmlns + "ns3", ns));
			xmlDoc.Add(compNfse);

			var nfse = new XElement(ns + "Nfse", new XAttribute(XNamespace.Xmlns + "ns4", ns));
			compNfse.Add(nfse);

			var infNfse = new XElement(ns + "InfNfse", new XAttribute("Id", nota.IdentificacaoNFSe.Numero));
			nfse.Add(infNfse);

			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", ns, 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Numero));
			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoVerificacao", ns, 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Chave));
			infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", ns, 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.DataEmissao));

			var infRps = new XElement(ns + "IdentificacaoRps");
			infNfse.Add(infRps);

			infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", ns, 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
			infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", ns, 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
			infRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", ns, 1, 1, Ocorrencia.Obrigatoria, tipoRps));

			infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRps", ns, 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", ns, 1, 1, Ocorrencia.Obrigatoria, naturezaOperacao));
			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", ns, 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", ns, 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", ns, 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
			infNfse.AddChild(AdicionarTag(TipoCampo.Int, "", "Competencia", ns, 6, 6, Ocorrencia.Obrigatoria, nota.Competencia));

			if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
			{
				var rpsSubstituido = new XElement(ns + "RpsSubstituido");

				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", ns, 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", ns, 1, 5, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", ns, 1, 1, Ocorrencia.Obrigatoria, tipoRpsSubstituido));

				nfse.AddChild(rpsSubstituido);
			}

			var servico = new XElement(ns + "Servico");
			nfse.AddChild(servico);

			var valores = new XElement(ns + "Valores");
			servico.AddChild(valores);

			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", ns, 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorDeducoes));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorPis));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorCofins));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorInss));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorIr));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorCsll));

			valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", ns, 1, 1, Ocorrencia.Obrigatoria, issRetido));

			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorIss));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorIssRetido));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.OutrasRetencoes));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.BaseCalculo));
			valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.Aliquota / 100)); // Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorLiquidoNfse));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.DescontoIncondicionado));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.DescontoCondicionado));

			servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", ns, 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

			servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", ns, 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

			servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", ns, 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
			servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", ns, 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
			servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", ns, 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

			var prestador = new XElement(ns + "Prestador");
			nfse.AddChild(prestador);

			prestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj.ZeroFill(14), ns));
			prestador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", ns, 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));

			var tomador = new XElement(ns + "Tomador");
			nfse.AddChild(tomador);

			var ideTomador = new XElement(ns + "IdentificacaoTomador");
			tomador.AddChild(ideTomador);

			var cpfCnpj = new XElement(ns + "CpfCnpj");
			ideTomador.AddChild(cpfCnpj);

			cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj, ns));
			if (!string.IsNullOrWhiteSpace(nota.Tomador.InscricaoMunicipal))
				ideTomador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", ns, 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

			tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", ns, 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));
			if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
			{
				var endereco = new XElement(ns + "Endereco");
				tomador.AddChild(endereco);

				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", ns, 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", ns, 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", ns, 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", ns, 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
				endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", ns, 1, 7, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", ns, 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
				endereco.AddChild(AdicionarTag(TipoCampo.StrNumberFill, "", "Cep", ns, 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
			}

			var contato = new XElement(ns + "Contato");
			tomador.AddChild(contato);

			contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", ns, 1, 11, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Telefone));
			contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", ns, 1, 80, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.Email));

			if (!nota.Intermediario.RazaoSocial.IsEmpty())
			{
				var intServico = new XElement(ns + "IntermediarioServico");
				nfse.AddChild(intServico);

				intServico.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", ns, 1, 115, 0, nota.Intermediario.RazaoSocial));

				var intServicoCpfCnpj = new XElement(ns + "CpfCnpj");
				intServico.AddChild(intServicoCpfCnpj);

				cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj, ns));

				intServico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", ns, 1, 15, 0, nota.Intermediario.InscricaoMunicipal));
			}

			if (!nota.ConstrucaoCivil.CodigoObra.IsEmpty())
			{
				var conCivil = new XElement(ns + "ConstrucaoCivil");
				nfse.AddChild(conCivil);

				conCivil.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", ns, 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.CodigoObra));
				conCivil.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", ns, 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.Art));
			}

			return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
		}

		public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)
		{
			var retornoWebservice = new RetornoWebservice
			{
				Sucesso = false,
				CpfCnpjRemetente = Config.PrestadorPadrao.CpfCnpj,
				CodCidade = Config.WebServices.CodMunicipio,
				DataLote = DateTime.Now,
				NumeroLote = "0",
				Protocolo = "",
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
				var xmlRps = GetXmlRps(nota, false, false, true);
				xmlLoteRps.Append(xmlRps);
				GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
			}

			var loteRps = GerarEnvelopeEnvio(lote, notas.Count, xmlLoteRps.ToString());
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
			retornoWebservice.DataLote = xmlRet.Root?.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
			retornoWebservice.Protocolo = xmlRet.Root?.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
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
				CpfCnpjRemetente = Config.PrestadorPadrao.CpfCnpj,
				CodCidade = Config.WebServices.CodMunicipio,
				DataLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = true
			};

			// Monta mensagem de envio
			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ConsultarSituacaoLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_situacao_lote_rps_envio_v03.xsd\">");
			loteBuilder.Append("<Prestador>");
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
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

			retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
			retornoWebservice.Situacao = xmlRet.Root?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
			switch (retornoWebservice.Situacao)
			{
				case "1":
					retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Situação de lote de RPS: 1 – Não Recebido" });
					break;

				case "2":
					retornoWebservice.Alertas.Add(new Evento { Codigo = "0", Descricao = "Situação de lote de RPS: 2 – Não Processado" });
					retornoWebservice.Sucesso = true;
					break;

				case "3":
					retornoWebservice.Alertas.Add(new Evento { Codigo = "0", Descricao = "Situação de lote de RPS: 3 – Processado com Erro" });
					retornoWebservice.Sucesso = true;
					break;

				case "4":
					retornoWebservice.Sucesso = true;
					break;

				default:
					retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Situação de lote de RPS indefinida." });
					break;
			}
			return retornoWebservice;
		}

		public override RetornoWebservice ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas)
		{
			var retornoWebservice = new RetornoWebservice()
			{
				Sucesso = false,
				CpfCnpjRemetente = Config.PrestadorPadrao.CpfCnpj,
				CodCidade = Config.WebServices.CodMunicipio,
				DataLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = true
			};

			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ConsultarLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_lote_rps_envio_v03.xsd\">");
			loteBuilder.Append("<Prestador>");
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
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

			retornoWebservice.Sucesso = true;

			foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
			{
				var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
				var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
				var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
				var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
				var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
				GravarNFSeEmDisco(compNfse.ToString(), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

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

		public override RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas)
		{
			var retornoWebservice = new RetornoWebservice
			{
				Sucesso = false,
				CpfCnpjRemetente = Config.PrestadorPadrao.CpfCnpj,
				CodCidade = Config.WebServices.CodMunicipio,
				DataLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = false
			};

			if (string.IsNullOrWhiteSpace(numeroNFSe))
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
				return retornoWebservice;
			}

			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<CancelarNfseEnvio xmlns=\"http://www.ginfes.com.br/servico_cancelar_nfse_envio_v03.xsd\">");
			loteBuilder.Append("<Pedido xmlns=\"\">");
			loteBuilder.Append("<tipos:InfPedidoCancelamento Id=\"\" xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\">");
			loteBuilder.Append("<tipos:IdentificacaoNfse>");
			loteBuilder.Append($"<tipos:Numero>{numeroNFSe}</tipos:Numero>");
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
			loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
			loteBuilder.Append($"<tipos:CodigoMunicipio>{Config.PrestadorPadrao.Endereco.CodigoMunicipio}</tipos:CodigoMunicipio>");
			loteBuilder.Append("</tipos:IdentificacaoNfse>");
			loteBuilder.Append($"<tipos:CodigoCancelamento>{codigoCancelamento}</tipos:CodigoCancelamento>");
			loteBuilder.Append("</tipos:InfPedidoCancelamento>");
			loteBuilder.Append("</Pedido>");
			loteBuilder.Append("</CancelarNfseEnvio>");

			retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "Pedido", Certificado);

			GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CanNFSe-{numeroNFSe}-env.xml");

			// Verifica Schema
			var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_cancelar_nfse_envio_v03.xsd");
			if (retSchema != null)
				return retSchema;

			// Recebe mensagem de retorno
			try
			{
				var cliente = GetCliente(TipoUrl.CancelaNFSe);
				var cabecalho = GerarCabecalho();
				retornoWebservice.XmlRetorno = cliente.CancelarNfse(cabecalho, retornoWebservice.XmlEnvio);
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
			if (retornoWebservice.Erros.Count > 0)
				return retornoWebservice;

			var confirmacaoCancelamento = xmlRet.ElementAnyNs("CancelarNfseResposta")?.ElementAnyNs("Cancelamento")?.ElementAnyNs("Confirmacao")?.ElementAnyNs("InfConfirmacaoCancelamento");
			if (confirmacaoCancelamento == null)
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Confirmação do cancelamento não encontrada! (InfConfirmacaoCancelamento)" });
				return retornoWebservice;
			}

			retornoWebservice.Sucesso = confirmacaoCancelamento.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;
			retornoWebservice.DataLote = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;

			// Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
			var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
			if (nota != null)
			{
				nota.Situacao = SituacaoNFSeRps.Cancelado;
				nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;
				nota.Cancelamento.DataHora = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
				nota.Cancelamento.MotivoCancelamento = motivo;

				// No caso do Ginfes, não retorna o XML da NotaFiscal Cancelada.
				// Por este motivo, não grava o arquivo NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml
			}

			return retornoWebservice;
		}

		public override RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo, NotaFiscalCollection notas)
		{
			var retornoWebservice = new RetornoWebservice()
			{
				Sucesso = false,
				CpfCnpjRemetente = Config.PrestadorPadrao.CpfCnpj,
				CodCidade = Config.WebServices.CodMunicipio,
				DataLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = false
			};

			if (string.IsNullOrWhiteSpace(numero))
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para cancelamento." });
				return retornoWebservice;
			}

			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ConsultarNfseRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_nfse_rps_envio_v03.xsd\">");
			loteBuilder.Append("<IdentificacaoRps>");
			loteBuilder.Append($"<tipos:Numero>{numero}</tipos:Numero>");
			loteBuilder.Append($"<tipos:Serie>{serie}</tipos:Serie>");
			loteBuilder.Append($"<tipos:Tipo>{(int)tipo + 1}</tipos:Tipo>");
			loteBuilder.Append("</IdentificacaoRps>");
			loteBuilder.Append("<Prestador>");
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
			loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
			loteBuilder.Append("</Prestador>");
			loteBuilder.Append("</ConsultarNfseRpsEnvio>");
			retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarNfseRpsEnvio", Certificado);

			GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNotaRps-{numero}-env.xml");

			// Verifica Schema
			var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_consultar_nfse_rps_envio_v03.xsd");
			if (retSchema != null)
				return retSchema;

			// Recebe mensagem de retorno
			try
			{
				var cliente = GetCliente(TipoUrl.ConsultaNFSeRps);
				var cabecalho = GerarCabecalho();
				retornoWebservice.XmlRetorno = cliente.ConsultarNfsePorRps(cabecalho, retornoWebservice.XmlEnvio);
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
			if (retornoWebservice.Erros.Count > 0)
				return retornoWebservice;

			var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");
			if (compNfse == null)
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
				return retornoWebservice;
			}

			// Carrega a nota fiscal na coleção de Notas Fiscais
			notas.Load(compNfse.ToString());

			retornoWebservice.Sucesso = true;
			return retornoWebservice;
		}

		public override RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNfse, int pagina, string cnpjTomador,
			string imTomador, string nomeInter, string cnpjInter, string imInter, string serie, NotaFiscalCollection notas)
		{
			var retornoWebservice = new RetornoWebservice()
			{
				Sucesso = false,
				CpfCnpjRemetente = Config.PrestadorPadrao.CpfCnpj,
				CodCidade = Config.WebServices.CodMunicipio,
				DataLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = false
			};

			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ConsultarNfseEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_nfse_rps_envio_v03.xsd\">");
			loteBuilder.Append("<Prestador>");
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
			loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
			loteBuilder.Append("</Prestador>");

			if (!numeroNfse.IsEmpty())
				loteBuilder.Append($"<tipos:NumeroNfse>{numeroNfse}</tipos:NumeroNfse>");

			if (inicio.HasValue & fim.HasValue)
			{
				loteBuilder.Append("<PeriodoEmissao>");
				loteBuilder.Append($"<tipos:DataInicial>{inicio:yyyy-MM-dd}</tipos:DataInicial>");
				loteBuilder.Append($"<tipos:DataFinal>{fim:yyyy-MM-dd}</tipos:DataFinal>");
				loteBuilder.Append("</PeriodoEmissao>");
			}

			if (!cnpjTomador.IsEmpty() & !imTomador.IsEmpty())
			{
				loteBuilder.Append("<Tomador>");
				loteBuilder.Append($"<tipos:CpfCnpj>{cnpjTomador.ZeroFill(14)}</tipos:CpfCnpj>");
				loteBuilder.Append($"<tipos:InscricaoMunicipal>{imTomador}</tipos:InscricaoMunicipal>");
				loteBuilder.Append("</Tomador>");
			}

			if (!nomeInter.IsEmpty() & !cnpjInter.IsEmpty())
			{
				loteBuilder.Append("<IntermediarioServico>");
				loteBuilder.Append($"<tipos:RazaoSocial>{nomeInter}</tipos:RazaoSocial>");
				loteBuilder.Append($"<tipos:CpfCnpj>{cnpjInter.ZeroFill(14)}</tipos:CpfCnpj>");
				if (!string.IsNullOrWhiteSpace(imInter))
					loteBuilder.Append($"<tipos:InscricaoMunicipal>{imInter}</tipos:InscricaoMunicipal>");
				loteBuilder.Append("</IntermediarioServico>");
			}

			loteBuilder.Append("</ConsultarNfseEnvio>");
			retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarNfseEnvio", Certificado);

			GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNota-{DateTime.Now:yyyyMMddHHmmss}-{numeroNfse}-env.xml");

			// Verifica Schema
			var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_consultar_nfse_envio_v03.xsd");
			if (retSchema != null)
				return retSchema;

			// Recebe mensagem de retorno
			try
			{
				var cliente = GetCliente(TipoUrl.ConsultaNFSeRps);
				var cabecalho = GerarCabecalho();
				retornoWebservice.XmlRetorno = cliente.ConsultarNfse(cabecalho, retornoWebservice.XmlEnvio);
			}
			catch (Exception ex)
			{
				retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retornoWebservice;
			}
			GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNota-{DateTime.Now:yyyyMMddHHmmss}-{numeroNfse}-ret.xml");

			// Analisa mensagem de retorno
			var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
			MensagemErro(retornoWebservice, xmlRet, "ConsultarNfseResposta");
			if (retornoWebservice.Erros.Count > 0)
				return retornoWebservice;

			//var compNfse = xmlRet.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");
			//if (compNfse == null)
			//{
			//    retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
			//    return retornoWebservice;
			//}

			//// Carrega a nota fiscal na coleção de Notas Fiscais
			//notas.Load(compNfse.ToString());

			//retornoWebservice.Sucesso = true;
			return retornoWebservice;
		}

		#endregion Methods

		#region Private Methods

		private string GetXmlRps(NotaFiscal nota, bool identado, bool showDeclaration, bool withPrefix)
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
			var issRetido = nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2;

			var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

			XNamespace ns = "http://www.ginfes.com.br/tipos_v03.xsd";
			var rps = withPrefix ? new XElement(ns + "Rps", new XAttribute(XNamespace.Xmlns + "tipos", ns)) :
								   new XElement("Rps", new XAttribute(XNamespace.Xmlns + "tipos", ns));
			xmlDoc.Add(rps);

			var infoRps = new XElement(ns + "InfRps", new XAttribute("Id", nota.IdentificacaoRps.Numero));
			rps.Add(infoRps);

			var ideRps = new XElement(ns + "IdentificacaoRps");
			infoRps.Add(ideRps);

			ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", ns, 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
			ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", ns, 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
			ideRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", ns, 1, 1, Ocorrencia.Obrigatoria, tipoRps));

			infoRps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", ns, 20, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "NaturezaOperacao", ns, 1, 1, Ocorrencia.Obrigatoria, naturezaOperacao));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", ns, 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", ns, 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivadorCultural", ns, 1, 1, Ocorrencia.Obrigatoria, incentivadorCultural));
			infoRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", ns, 1, 1, Ocorrencia.Obrigatoria, situacao));

			if (!string.IsNullOrWhiteSpace(nota.RpsSubstituido.NumeroRps))
			{
				var rpsSubstituido = new XElement(ns + "RpsSubstituido");

				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Numero", ns, 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NumeroRps));
				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Serie", ns, 1, 5, Ocorrencia.Obrigatoria, nota.RpsSubstituido.Serie));
				rpsSubstituido.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", ns, 1, 1, Ocorrencia.Obrigatoria, tipoRpsSubstituido));

				infoRps.AddChild(rpsSubstituido);
			}

			var servico = new XElement(ns + "Servico");
			infoRps.AddChild(servico);

			var valores = new XElement(ns + "Valores");
			servico.AddChild(valores);

			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", ns, 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));

			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorDeducoes));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorPis));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorCofins));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorInss));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorIr));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorCsll));

			valores.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", ns, 1, 1, Ocorrencia.Obrigatoria, issRetido));

			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorIss));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIssRetido", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorIssRetido));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.OutrasRetencoes));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.BaseCalculo));
			// Valor Percentual - Exemplos: 1% => 0.01   /   25,5% => 0.255   /   100% => 1
			valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.Aliquota));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.ValorLiquidoNfse));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.DescontoIncondicionado));
			valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", ns, 1, 15, Ocorrencia.SeDiferenteDeZero, nota.Servico.Valores.DescontoCondicionado));

			servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", ns, 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));

			servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCnae", ns, 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

			servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", ns, 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
			servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", ns, 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
			servico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", ns, 1, 7, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));

			var prestador = new XElement(ns + "Prestador");
			infoRps.AddChild(prestador);

			prestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj.ZeroFill(14), ns));
			prestador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", ns, 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));

			var tomador = new XElement(ns + "Tomador");
			infoRps.AddChild(tomador);

			var ideTomador = new XElement(ns + "IdentificacaoTomador");
			tomador.AddChild(ideTomador);

			var cpfCnpj = new XElement(ns + "CpfCnpj");
			ideTomador.AddChild(cpfCnpj);

			cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj, ns));
			if (!string.IsNullOrWhiteSpace(nota.Tomador.InscricaoMunicipal))
				ideTomador.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", ns, 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

			tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", ns, 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));
			if (!nota.Tomador.Endereco.Logradouro.IsEmpty())
			{
				var endereco = new XElement(ns + "Endereco");
				tomador.AddChild(endereco);

				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", ns, 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", ns, 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", ns, 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", ns, 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Bairro));
				endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", ns, 1, 7, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
				endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", ns, 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
				endereco.AddChild(AdicionarTag(TipoCampo.StrNumberFill, "", "Cep", ns, 8, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep));
			}

			if (!nota.Tomador.DadosContato.Telefone.IsEmpty() || !nota.Tomador.DadosContato.Email.IsEmpty())
			{
				var contato = new XElement(ns + "Contato");
				tomador.AddChild(contato);

				contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", ns, 1, 11, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Telefone));
				contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", ns, 1, 80, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
			}

			if (!nota.Intermediario.RazaoSocial.IsEmpty())
			{
				var intServico = new XElement(ns + "IntermediarioServico");
				infoRps.AddChild(intServico);

				intServico.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", ns, 1, 115, 0, nota.Intermediario.RazaoSocial));

				var intServicoCpfCnpj = new XElement(ns + "CpfCnpj");
				intServico.AddChild(intServicoCpfCnpj);

				cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj, ns));

				intServico.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", ns, 1, 15, 0, nota.Intermediario.InscricaoMunicipal));
			}

			if (!nota.ConstrucaoCivil.CodigoObra.IsEmpty())
			{
				var conCivil = new XElement(ns + "ConstrucaoCivil");
				infoRps.AddChild(conCivil);

				conCivil.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", ns, 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.CodigoObra));
				conCivil.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", ns, 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.Art));
			}

			return xmlDoc.AsString(identado, showDeclaration, Encoding.UTF8);
		}

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
			xmlLote.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</tipos:Cnpj>");
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