// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 10-02-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-02-2014
// ***********************************************************************
// <copyright file="ProviderDSF.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers.DSF
{
	/// <summary>
	/// Class ProviderDSF. This class cannot be inherited.
	/// </summary>
	internal sealed class ProviderDSF : ProviderBase
	{
		#region Internal Types

		private enum TipoEvento
		{
			Erros,
			Alertas,
			ListNFSeRps
		}

		#endregion Internal Types

		#region Fields

		private string situacao;

		private string recolhimento;

		private string tributacao;

		private string operacao;

		private string assinatura;

		#endregion Fields

		#region Constructors

		public ProviderDSF(Configuracoes config, MunicipioNFSe municipio) : base(config, municipio)
		{
		}

		#endregion Constructors

		#region Methods

		#region Public

		public override NotaFiscal LoadXml(XmlDocument xml)
		{
			var root = xml["Nota"] ?? xml["RPS"];
			Guard.Against<XmlException>(root == null, "Xml de Nota/RPS invalida.");

			var ret = new NotaFiscal();
			// Prestador
			ret.Prestador.InscricaoMunicipal = root["InscricaoMunicipalPrestador"].GetValue<string>();
			ret.Prestador.RazaoSocial = root["RazaoSocialPrestador"].GetValue<string>();
			ret.Prestador.Contato.DDD = root["DDDPrestador"].GetValue<string>();
			ret.Prestador.Contato.Telefone = root["TelefonePrestador"].GetValue<string>();
			ret.IntermediarioServico.CpfCnpj = root["CPFCNPJIntermediario"].GetValue<string>();

			// Tomador
			ret.Tomador.InscricaoMunicipal = root["InscricaoMunicipalTomador"].GetValue<string>();
			ret.Tomador.CpfCnpj = root["CPFCNPJTomador"].GetValue<string>();
			ret.Tomador.RazaoSocial = root["RazaoSocialTomador"].GetValue<string>();
			ret.Tomador.Endereco.TipoLogradouro = root["TipoLogradouroTomador"].GetValue<string>();
			ret.Tomador.Endereco.Logradouro = root["LogradouroTomador"].GetValue<string>();
			ret.Tomador.Endereco.Numero = root["NumeroEnderecoTomador"].GetValue<string>();
			ret.Tomador.Endereco.Complemento = root["ComplementoEnderecoTomador"].GetValue<string>();
			ret.Tomador.Endereco.TipoBairro = root["TipoBairroTomador"].GetValue<string>();
			ret.Tomador.Endereco.Bairro = root["BairroTomador"].GetValue<string>();
			ret.Tomador.Endereco.CodigoMunicipio = root["CidadeTomador"].GetValue<string>();
			ret.Tomador.Endereco.Municipio = root["CidadeTomadorDescricao"].GetValue<string>();
			ret.Tomador.Endereco.CEP = root["CEPTomador"].GetValue<string>();
			ret.Tomador.Contato.Email = root["EmailTomador"].GetValue<string>();
			ret.Tomador.Contato.DDD = root["DDDTomador"].GetValue<string>();
			ret.Tomador.Contato.Telefone = root["TelefoneTomador"].GetValue<string>();

			// Dados NFSe
			ret.Numero = root["NumeroNota"].GetValue<string>();
			ret.DhRecebimento = root["DataProcessamento"].GetValue<DateTime>();
			ret.NumeroLote = root["NumeroLote"].GetValue<string>();
			ret.ChaveNfse = root["CodigoVerificacao"].GetValue<string>();

			//RPS
			ret.IdentificacaoRps.Numero = root["NumeroRPS"].GetValue<string>();
			ret.IdentificacaoRps.DataEmissaoRps = root["DataEmissaoRPS"].GetValue<DateTime>();
			ret.IdentificacaoRps.SeriePrestacao = root["SeriePrestacao"].GetValue<string>();

			// RPS Substituido
			ret.RpsSubstituido.Serie = root["SerieRPSSubstituido"].GetValue<string>();
			ret.RpsSubstituido.NumeroRps = root["NumeroRPSSubstituido"].GetValue<string>();
			ret.RpsSubstituido.NumeroNfse = root["NumeroNFSeSubstituida"].GetValue<string>();
			ret.RpsSubstituido.DataEmissaoNfseSubstituida = root["DataEmissaoNFSeSubstituida"].GetValue<DateTime>();

			// Servico
			ret.Servico.CodigoCnae = root["CodigoAtividade"].GetValue<string>();
			ret.Servico.Valores.Aliquota = root["AliquotaAtividade"].GetValue<decimal>();
			ret.Servico.Valores.IssRetido = root["TipoRecolhimento"].GetValue<char>() == 'A' ? SituacaoTributaria.Normal : SituacaoTributaria.Retencao;
			ret.Servico.CodigoMunicipio = root["MunicipioPrestacao"].GetValue<string>();
			ret.Servico.Municipio = root["MunicipioPrestacaoDescricao"].GetValue<string>();

			switch (root["Operacao"].GetValue<char>())
			{
				case 'B':
					ret.NaturezaOperacao = NaturezaOperacao.NT02;
					break;

				case 'C':
					ret.NaturezaOperacao = NaturezaOperacao.NT03;
					break;

				case 'D':
					ret.NaturezaOperacao = NaturezaOperacao.NT04;
					break;

				case 'J':
					ret.NaturezaOperacao = NaturezaOperacao.NT05;
					break;

				default:
					ret.NaturezaOperacao = NaturezaOperacao.NT01;
					break;
			}

			switch (root["Tributacao"].GetValue<char>())
			{
				case 'C':
					ret.TipoTributacao = TipoTributacao.Isenta;
					break;

				case 'F':
					ret.TipoTributacao = TipoTributacao.Imune;
					break;

				case 'K':
					ret.TipoTributacao = TipoTributacao.DepositoEmJuizo;
					break;

				case 'E':
					ret.TipoTributacao = TipoTributacao.NaoIncide;
					break;

				case 'N':
					ret.TipoTributacao = TipoTributacao.NaoTributavel;
					break;

				case 'G':
					ret.TipoTributacao = TipoTributacao.TributavelFixo;
					break;

				case 'H':
					ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
					ret.TipoTributacao = TipoTributacao.Tributavel;
					break;

				case 'M':
					ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
					ret.TipoTributacao = TipoTributacao.Tributavel;
					break;

				//Tributavel
				default:
					ret.TipoTributacao = TipoTributacao.Tributavel;
					break;
			}

			ret.Servico.Valores.ValorPis = root["ValorPIS"].GetValue<decimal>();
			ret.Servico.Valores.ValorCofins = root["ValorCOFINS"].GetValue<decimal>();
			ret.Servico.Valores.ValorInss = root["ValorINSS"].GetValue<decimal>();
			ret.Servico.Valores.ValorIR = root["ValorIR"].GetValue<decimal>();
			ret.Servico.Valores.ValorCsll = root["ValorCSLL"].GetValue<decimal>();
			ret.Servico.Valores.AliquotaPis = root["AliquotaPIS"].GetValue<decimal>();
			ret.Servico.Valores.AliquotaCofins = root["AliquotaCOFINS"].GetValue<decimal>();
			ret.Servico.Valores.AliquotaInss = root["AliquotaINSS"].GetValue<decimal>();
			ret.Servico.Valores.AliquotaIR = root["AliquotaIR"].GetValue<decimal>();
			ret.Servico.Valores.AliquotaCsll = root["AliquotaCSLL"].GetValue<decimal>();
			ret.Servico.Descricao = root["DescricaoRPS"].GetValue<string>();

			//Outros
			ret.MotivoCancelamento = root["MotCancelamento"].GetValue<string>();

			//Deduções
			var deducoes = root["Deducoes"];
			if (deducoes != null && deducoes.HasChildNodes)
			{
				foreach (XmlNode node in deducoes.ChildNodes)
				{
					var deducaoRoot = node["Deducao"];
					var deducao = ret.Servico.Deducoes.AddNew();
					deducao.DeducaoPor = (DeducaoPor)Enum.Parse(typeof(DeducaoPor), deducaoRoot["DeducaoPor"].GetValue<string>());
					deducao.TipoDeducao = deducaoRoot["TipoDeducao"].GetValue<string>().ToEnum(
						new[] { "", "Despesas com Materiais", "Despesas com Mercadorias", "Despesas com Subempreitada",
								"Servicos de Veiculacao e Divulgacao", "Mapa de Const. Civil", "Servicos" },
						new[] { TipoDeducao.Nenhum, TipoDeducao.Materiais, TipoDeducao.Mercadorias, TipoDeducao.SubEmpreitada,
								TipoDeducao.VeiculacaoeDivulgacao, TipoDeducao.MapadeConstCivil, TipoDeducao.Servicos });
					deducao.CPFCNPJReferencia = deducaoRoot["CPFCNPJReferencia"].GetValue<string>();
					deducao.NumeroNFReferencia = deducaoRoot["NumeroNFReferencia"].GetValue<int?>();
					deducao.ValorTotalReferencia = deducaoRoot["ValorTotalReferencia"].GetValue<decimal>();
					deducao.PercentualDeduzir = deducaoRoot["PercentualDeduzir"].GetValue<decimal>();
					deducao.ValorDeduzir = deducaoRoot["ValorDeduzir"].GetValue<decimal>();
				}
			}

			//Serviços
			var servicos = root["Itens"];
			if (servicos != null && servicos.HasChildNodes)
			{
				foreach (XmlNode node in servicos.ChildNodes)
				{
					var servicoRoot = node["Item"];
					var servico = ret.Servico.ItensServico.AddNew();
					servico.Descricao = servicoRoot["DiscriminacaoServico"].GetValue<string>();
					servico.Quantidade = servicoRoot["Quantidade"].GetValue<decimal>();
					servico.ValorServicos = servicoRoot["ValorUnitario"].GetValue<decimal>();
					servico.Tributavel = servicoRoot["Tributavel"].GetValue<string>() == "S" ? NFSeSimNao.Sim : NFSeSimNao.Nao;
				}
			}

			return ret;
		}

		/// <summary>
		/// Gets the XML.
		/// </summary>
		/// <param name="nota">The nota.</param>
		/// <param name="identado">if set to <c>true</c> [identado].</param>
		/// <param name="showDeclaration">if set to <c>true</c> [show declaration].</param>
		/// <returns>System.String.</returns>
		public override string GetXmlRPS(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
		{
			Xmldoc.RemoveAll();
			var dec = Xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
			Xmldoc.AppendChild(dec);

			GerarCampos(nota);

			var rpsTag = Xmldoc.CreateElement("RPS");
			rpsTag.SetAttribute("Id", $"rps:{nota.InfId.Id}");

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, 1, assinatura));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalPrestador", 01, 11, 1, nota.Prestador.InscricaoMunicipal.OnlyNumbers()));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialPrestador", 1, 120, 1, RetirarAcentos ? nota.Prestador.RazaoSocial.RemoveAccent() : nota.Prestador.RazaoSocial));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 20, 1, "RPS"));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 01, 2, 1,
				nota.IdentificacaoRps.Serie.IsEmpty() ? "NF" : nota.IdentificacaoRps.Serie));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPS", 1, 12, 1, nota.IdentificacaoRps.Numero));
			rpsTag.AddTag(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRPS", 1, 21, 1, nota.IdentificacaoRps.DataEmissaoRps));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SituacaoRPS", 1, 1, 1, situacao));
			if (!nota.RpsSubstituido.NumeroRps.IsEmpty())
			{
				rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPSSubstituido", 0, 2, 1, "NF"));
				rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPSSubstituido", 0, 2, 1, nota.RpsSubstituido.NumeroRps));
				rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroNFSeSubstituida", 0, 2, 1, nota.RpsSubstituido.NumeroNfse));
				rpsTag.AddTag(AdicionarTag(TipoCampo.Dat, "", "DataEmissaoNFSeSubstituida", 0, 2, 1, nota.RpsSubstituido.DataEmissaoNfseSubstituida));
			}

			rpsTag.AddTag(AdicionarTag(TipoCampo.Int, "", "SeriePrestacao", 01, 02, 1,
				nota.IdentificacaoRps.SeriePrestacao.IsEmpty() ? "99" : nota.IdentificacaoRps.SeriePrestacao.OnlyNumbers()));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalTomador", 1, 11, 1, nota.Tomador.InscricaoMunicipal.OnlyNumbers()));
			rpsTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJTomador", "CPFCNPJTomador", nota.Tomador.CpfCnpj));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 120, 1, RetirarAcentos ? nota.Tomador.RazaoSocial.RemoveAccent() : nota.Tomador.RazaoSocial));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DocTomadorEstrangeiro", 0, 20, 1, nota.Tomador.DocTomadorEstrangeiro));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoLogradouroTomador", 0, 10, 1, nota.Tomador.Endereco.TipoLogradouro));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "LogradouroTomador", 1, 50, 1, RetirarAcentos ? nota.Tomador.Endereco.Logradouro.RemoveAccent() : nota.Tomador.Endereco.Logradouro));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroEnderecoTomador", 1, 9, 1, nota.Tomador.Endereco.Numero));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "ComplementoEnderecoTomador", 1, 30, 0, RetirarAcentos ? nota.Tomador.Endereco.Complemento.RemoveAccent() : nota.Tomador.Endereco.Complemento));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoBairroTomador", 0, 10, 1, nota.Tomador.Endereco.TipoBairro));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "BairroTomador", 1, 50, 1, RetirarAcentos ? nota.Tomador.Endereco.Bairro.RemoveAccent() : nota.Tomador.Endereco.Bairro));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomador", 1, 10, 1, nota.Tomador.Endereco.CodigoMunicipio));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomadorDescricao", 1, 50, 1, RetirarAcentos ? nota.Tomador.Endereco.Municipio.RemoveAccent() : nota.Tomador.Endereco.Municipio));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CEPTomador", 1, 8, 1, nota.Tomador.Endereco.CEP.OnlyNumbers()));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 60, 1, nota.Tomador.Contato.Email));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CodigoAtividade", 1, 9, 1, nota.Servico.CodigoCnae));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "AliquotaAtividade", 1, 11, 1, nota.Servico.Valores.Aliquota));

			//valores serviço
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRecolhimento", 01, 01, 1, recolhimento));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacao", 1, 10, 1, nota.Servico.CodigoMunicipio.ZeroFill(7)));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacaoDescricao", 01, 30, 1, RetirarAcentos ? nota.Servico.Municipio.RemoveAccent() : nota.Servico.Municipio));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Operacao", 01, 01, 1, operacao));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Tributacao", 01, 01, 1, tributacao));

			//Valores
			rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 2, 1, nota.Servico.Valores.ValorPis));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 2, 1, nota.Servico.Valores.ValorCofins));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 2, 1, nota.Servico.Valores.ValorInss));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 2, 1, nota.Servico.Valores.ValorIR));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 2, 1, nota.Servico.Valores.ValorCsll));

			//Aliquotas
			rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaPIS", 1, 2, 1, nota.Servico.Valores.AliquotaPis));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCOFINS", 1, 2, 1, nota.Servico.Valores.AliquotaCofins));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaINSS", 1, 2, 1, nota.Servico.Valores.AliquotaInss));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaIR", 1, 2, 1, nota.Servico.Valores.AliquotaIR));
			rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCSLL", 1, 2, 1, nota.Servico.Valores.AliquotaCsll));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DescricaoRPS", 1, 1500, 1, RetirarAcentos ? nota.Servico.Descricao.RemoveAccent() : nota.Servico.Descricao));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDPrestador", 0, 3, 1, nota.Prestador.Contato.DDD.OnlyNumbers()));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefonePrestador", 0, 8, 1, nota.Prestador.Contato.Telefone.OnlyNumbers()));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDTomador", 0, 03, 1, nota.Tomador.Contato.DDD.OnlyNumbers()));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefoneTomador", 0, 8, 1, nota.Tomador.Contato.Telefone.OnlyNumbers()));

			if (!nota.IntermediarioServico.CpfCnpj.IsEmpty())
				rpsTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJIntermediario", "CPFCNPJIntermediario", nota.IntermediarioServico.CpfCnpj));

			rpsTag.AddTag(GerarServicos(nota.Servico.ItensServico));
			if (nota.Servico.Deducoes.Count > 0)
				rpsTag.AddTag(GerarDeducoes(nota.Servico.Deducoes));

			Xmldoc.AddTag(rpsTag);
			return Xmldoc.AsString(identado, showDeclaration);
		}

		/// <summary>
		/// Gets the XML.
		/// </summary>
		/// <param name="nota">The nota.</param>
		/// <param name="identado">if set to <c>true</c> [identado].</param>
		/// <param name="showDeclaration">if set to <c>true</c> [show declaration].</param>
		/// <returns>System.String.</returns>
		public override string GetXmlNFSe(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
		{
			Xmldoc.RemoveAll();
			var dec = Xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
			Xmldoc.AppendChild(dec);

			GerarCampos(nota);

			var notaTag = Xmldoc.CreateElement("Nota");
			notaTag.AddTag(AdicionarTag(TipoCampo.Int, "", "NumeroNota", 1, 11, 1, nota.Numero));
			notaTag.AddTag(AdicionarTag(TipoCampo.DatHor, "", "DataProcessamento", 1, 21, 1, nota.DhRecebimento));
			notaTag.AddTag(AdicionarTag(TipoCampo.Int, "", "NumeroLote", 1, 11, 1, nota.NumeroLote));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CodigoVerificacao", 1, 200, 1, nota.ChaveNfse));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, 1, nota.Assinatura));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalPrestador", 01, 11, 1, nota.Prestador.InscricaoMunicipal.OnlyNumbers()));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialPrestador", 1, 120, 1, RetirarAcentos ? nota.Prestador.RazaoSocial.RemoveAccent() : nota.Prestador.RazaoSocial));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 20, 1, "RPS"));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 01, 02, 1,
				nota.IdentificacaoRps.Serie.IsEmpty() ? "NF" : nota.IdentificacaoRps.Serie));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPS", 1, 12, 1, nota.IdentificacaoRps.Serie));
			notaTag.AddTag(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRPS", 1, 21, 1, nota.IdentificacaoRps.DataEmissaoRps));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SituacaoRPS", 1, 1, 1, situacao));
			if (!nota.RpsSubstituido.NumeroRps.IsEmpty())
			{
				notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPSSubstituido", 0, 2, 1, "NF"));
				notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPSSubstituido", 0, 2, 1, nota.RpsSubstituido.NumeroRps));
				notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroNFSeSubstituida", 0, 2, 1, nota.RpsSubstituido.NumeroNfse));
				notaTag.AddTag(AdicionarTag(TipoCampo.Dat, "", "DataEmissaoNFSeSubstituida", 0, 2, 1, nota.RpsSubstituido.DataEmissaoNfseSubstituida));
			}

			notaTag.AddTag(AdicionarTag(TipoCampo.Int, "", "SeriePrestacao", 1, 2, 1,
				nota.IdentificacaoRps.SeriePrestacao.IsEmpty() ? "99" : nota.IdentificacaoRps.SeriePrestacao));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalTomador", 1, 11, 1, nota.Tomador.InscricaoMunicipal.OnlyNumbers()));
			notaTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJTomador", "CPFCNPJTomador", nota.Tomador.CpfCnpj));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 120, 1, RetirarAcentos ? nota.Tomador.RazaoSocial.RemoveAccent() : nota.Tomador.RazaoSocial));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DocTomadorEstrangeiro", 0, 20, 1, nota.Tomador.DocTomadorEstrangeiro));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoLogradouroTomador", 0, 10, 1, nota.Tomador.Endereco.TipoLogradouro));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "LogradouroTomador", 1, 50, 1, RetirarAcentos ? nota.Tomador.Endereco.Logradouro.RemoveAccent() : nota.Tomador.Endereco.Logradouro));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroEnderecoTomador", 1, 9, 1, nota.Tomador.Endereco.Numero));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "ComplementoEnderecoTomador", 1, 30, 0, RetirarAcentos ? nota.Tomador.Endereco.Complemento.RemoveAccent() : nota.Tomador.Endereco.Complemento));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoBairroTomador", 0, 10, 1, nota.Tomador.Endereco.TipoBairro));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "BairroTomador", 1, 50, 1, RetirarAcentos ? nota.Tomador.Endereco.Bairro.RemoveAccent() : nota.Tomador.Endereco.Bairro));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomador", 1, 10, 1, nota.Tomador.Endereco.CodigoMunicipio));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomadorDescricao", 1, 50, 1, RetirarAcentos ? nota.Tomador.Endereco.Municipio.RemoveAccent() : nota.Tomador.Endereco.Municipio));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CEPTomador", 1, 8, 1, nota.Tomador.Endereco.CEP.OnlyNumbers()));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 60, 1, nota.Tomador.Contato.Email));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CodigoAtividade", 1, 9, 1, nota.Servico.CodigoCnae));
			notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "AliquotaAtividade", 1, 11, 1, nota.Servico.Valores.Aliquota));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRecolhimento", 01, 01, 1, recolhimento));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacao", 01, 10, 1, nota.Servico.CodigoMunicipio.ZeroFill(7)));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacaoDescricao", 01, 30, 1, RetirarAcentos ? nota.Servico.Municipio.RemoveAccent() : nota.Servico.Municipio));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Operacao", 01, 01, 1, operacao));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Tributacao", 01, 01, 1, tributacao));

			//Valores
			notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 2, 1, nota.Servico.Valores.ValorPis));
			notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 2, 1, nota.Servico.Valores.ValorCofins));
			notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 2, 1, nota.Servico.Valores.ValorInss));
			notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 2, 1, nota.Servico.Valores.ValorIR));
			notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 2, 1, nota.Servico.Valores.ValorCsll));

			//Aliquotas criar propriedades
			notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaPIS", 1, 2, 1, nota.Servico.Valores.AliquotaPis));
			notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCOFINS", 1, 2, 1, nota.Servico.Valores.AliquotaCofins));
			notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaINSS", 1, 2, 1, nota.Servico.Valores.AliquotaInss));
			notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaIR", 1, 2, 1, nota.Servico.Valores.AliquotaIR));
			notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCSLL", 1, 2, 1, nota.Servico.Valores.AliquotaCsll));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DescricaoRPS", 1, 1500, 1, RetirarAcentos ? nota.Servico.Descricao.RemoveAccent() : nota.Servico.Descricao));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDPrestador", 0, 3, 1, nota.Prestador.Contato.DDD.OnlyNumbers()));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefonePrestador", 0, 8, 1, nota.Prestador.Contato.Telefone.OnlyNumbers()));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDTomador", 0, 03, 1, nota.Tomador.Contato.DDD.OnlyNumbers()));
			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefoneTomador", 0, 8, 1, nota.Tomador.Contato.Telefone.OnlyNumbers()));

			if (nota.Status == StatusRps.Cancelado)
				notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MotCancelamento", 1, 80, 1, RetirarAcentos ? nota.MotivoCancelamento.RemoveAccent() : nota.MotivoCancelamento));

			if (!nota.IntermediarioServico.CpfCnpj.IsEmpty())
				notaTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJIntermediario", "CPFCNPJIntermediario", nota.IntermediarioServico.CpfCnpj));

			notaTag.AddTag(GerarServicos(nota.Servico.ItensServico));
			if (nota.Servico.Deducoes.Count > 0)
				notaTag.AddTag(GerarDeducoes(nota.Servico.Deducoes));

			Xmldoc.AddTag(notaTag);
			return Xmldoc.AsString(identado, showDeclaration);
		}

		public override RetornoWebService Enviar(int lote, NotaFiscalCollection notas)
		{
			var rpsOrg = notas.OrderBy(x => x.IdentificacaoRps.DataEmissaoRps);
			var valorTotal = notas.Sum(nota => nota.Servico.Valores.ValorServicos);
			var deducaoTotal = notas.Sum(nota => nota.Servico.Valores.ValorDeducoes);

			var loteRps = GerarEnvEnvio();
			loteRps = loteRps.SafeReplace("%DTINICIO%", rpsOrg.First().IdentificacaoRps.DataEmissaoRps.ToString("yyyy-MM-dd"));
			loteRps = loteRps.SafeReplace("%DTFIM%", rpsOrg.Last().IdentificacaoRps.DataEmissaoRps.ToString("yyyy-MM-dd"));
			loteRps = loteRps.SafeReplace("%TOTALRPS%", notas.Count.ToString());
			loteRps = loteRps.SafeReplace("%TOTALVALOR%", $"{valorTotal:0.00}");
			loteRps = loteRps.SafeReplace("%TOTALDEDUCAO%", $"{deducaoTotal:0.00}");
			loteRps = loteRps.SafeReplace("%LOTE%", lote.ToString());

			var xmlNotas = new StringBuilder();
			foreach (var nota in notas)
			{
				xmlNotas.Append(GetXmlRPS(nota, false, false));
				if (!Config.Geral.Salvar)
					continue;

				var rpsFile = Path.Combine(Config.Arquivos.GetPathRps(nota.IdentificacaoRps.DataEmissaoRps),
					$"Rps-{nota.IdentificacaoRps.DataEmissaoRps:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml");
				var xml = GetXmlRPS(nota);
				File.WriteAllText(rpsFile, xml, Encoding.UTF8);
			}

			loteRps = loteRps.SafeReplace("%NOTAS%", xmlNotas.ToString());

			loteRps = CertificadoDigital.Assinar(loteRps, "", "ReqEnvioLoteRPS", Certificado, true);

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"lote-{lote}-env.xml");
				File.WriteAllText(loteFile, loteRps, Encoding.UTF8);
			}

			string[] errosSchema;
			string[] alertasSchema;
			var schema = Path.Combine(Config.Geral.PathSchemas, @"DSF\ReqEnvioLoteRPS.xsd");
			if (!CertificadoDigital.ValidarXml(loteRps, schema, out errosSchema, out alertasSchema))
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				foreach (var loteErro in errosSchema.Select(erro => new Evento { Codigo = "0", Descricao = erro }))
					retLote.Erros.Add(loteErro);

				foreach (var alerta in alertasSchema.Select(descricao => new Evento { Codigo = "0", Descricao = descricao }))
					retLote.Alertas.Add(alerta);

				return retLote;
			}

			var url = GetUrl(TipoUrl.Enviar);
			var cliente = new DSFServiceClient(url, TimeOut, Certificado);

			string retorno;
			try
			{
				retorno = cliente.Enviar(loteRps);
			}
			catch (Exception ex)
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				retLote.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retLote;
			}

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"lote-{lote}-ret.xml");
				File.WriteAllText(loteFile, retorno, Encoding.UTF8);
			}

			var ret = new RetornoWebService();
			var xmlRet = XDocument.Parse(retorno);
			var cabeçalho = xmlRet.Element("Cabecalho");

			ret.Sucesso = cabeçalho?.Element("Sucesso")?.GetValue<bool>() ?? false;
			ret.CodCidade = cabeçalho?.Element("CodCidade")?.GetValue<int>() ?? 0;
			ret.NumeroLote = cabeçalho?.Element("NumeroLote")?.GetValue<string>() ?? string.Empty;
			ret.CPFCNPJRemetente = cabeçalho?.Element("CPFCNPJRemetente")?.GetValue<string>() ?? string.Empty;
			ret.DataEnvioLote = cabeçalho?.Element("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

			var erros = xmlRet.Element("Erros");
			ret.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

			var alertas = xmlRet.Element("Alertas");
			ret.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

			if (!ret.Sucesso)
				return ret;

			foreach (var nota in notas)
				nota.NumeroLote = ret.NumeroLote;

			return ret;
		}

		public override RetornoWebService EnviarSincrono(int lote, NotaFiscalCollection notas)
		{
			var rpsOrg = notas.OrderBy(x => x.IdentificacaoRps.DataEmissaoRps);
			var valorTotal = notas.Sum(nota => nota.Servico.Valores.ValorServicos);
			var deducaoTotal = notas.Sum(nota => nota.Servico.Valores.ValorDeducoes);

			var loteRps = GerarEnvEnvio();
			loteRps = loteRps.SafeReplace("%DTINICIO%", rpsOrg.First().IdentificacaoRps.DataEmissaoRps.ToString("yyyy-MM-dd"));
			loteRps = loteRps.SafeReplace("%DTFIM%", rpsOrg.Last().IdentificacaoRps.DataEmissaoRps.ToString("yyyy-MM-dd"));
			loteRps = loteRps.SafeReplace("%TOTALRPS%", notas.Count.ToString());
			loteRps = loteRps.SafeReplace("%TOTALVALOR%", $"{valorTotal:0.00}");
			loteRps = loteRps.SafeReplace("%TOTALDEDUCAO%", $"{deducaoTotal:0.00}");
			loteRps = loteRps.SafeReplace("%LOTE%", lote.ToString());

			var xmlNotas = new StringBuilder();
			foreach (var nota in notas)
			{
				xmlNotas.Append(GetXmlRPS(nota, false, false));
				if (!Config.Geral.Salvar)
					continue;

				var rpsFile = Path.Combine(Config.Arquivos.GetPathRps(nota.IdentificacaoRps.DataEmissaoRps),
					$"Rps-{nota.IdentificacaoRps.DataEmissaoRps:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml");
				var xml = GetXmlRPS(nota);
				File.WriteAllText(rpsFile, xml, Encoding.UTF8);
			}

			loteRps = loteRps.SafeReplace("%NOTAS%", xmlNotas.ToString());

			loteRps = CertificadoDigital.Assinar(loteRps, "", "ReqEnvioLoteRPS", Certificado, true);

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"lote-{lote}-env.xml");
				File.WriteAllText(loteFile, loteRps, Encoding.UTF8);
			}

			string[] errosSchema;
			string[] alertasSchema;
			var schema = Path.Combine(Config.Geral.PathSchemas, @"DSF\ReqEnvioLoteRPS.xsd");
			if (!CertificadoDigital.ValidarXml(loteRps, schema, out errosSchema, out alertasSchema))
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				foreach (var loteErro in errosSchema.Select(erro => new Evento { Codigo = "0", Descricao = erro }))
					retLote.Erros.Add(loteErro);

				foreach (var alerta in alertasSchema.Select(descricao => new Evento { Codigo = "0", Descricao = descricao }))
					retLote.Alertas.Add(alerta);

				return retLote;
			}

			var url = GetUrl(TipoUrl.Enviar);
			var cliente = new DSFServiceClient(url, TimeOut, Certificado);

			string retorno;
			try
			{
				retorno = cliente.Enviar(loteRps);
			}
			catch (Exception ex)
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				retLote.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retLote;
			}

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"lote-{lote}-ret.xml");
				File.WriteAllText(loteFile, retorno, Encoding.UTF8);
			}

			var ret = new RetornoWebService();
			var xmlRet = XDocument.Parse(retorno);
			var cabeçalho = xmlRet.Element("Cabecalho");

			ret.Sucesso = cabeçalho?.Element("Sucesso")?.GetValue<bool>() ?? false;
			ret.CodCidade = cabeçalho?.Element("CodCidade")?.GetValue<int>() ?? 0;
			ret.NumeroLote = cabeçalho?.Element("NumeroLote")?.GetValue<string>() ?? string.Empty;
			ret.CPFCNPJRemetente = cabeçalho?.Element("CPFCNPJRemetente")?.GetValue<string>() ?? string.Empty;
			ret.DataEnvioLote = cabeçalho?.Element("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

			var erros = xmlRet.Element("Erros");
			ret.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

			var alertas = xmlRet.Element("Alertas");
			ret.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

			if (!ret.Sucesso) return ret;

			foreach (var nota in notas)
				nota.NumeroLote = ret.NumeroLote;

			var nfseRps = ProcessarEventos(TipoEvento.ListNFSeRps, xmlRet.Element("ChavesNFSeRPS"));
			if (nfseRps == null) return ret;

			foreach (var nfse in nfseRps)
			{
				var numeroRps = nfse.IdentificacaoRps.Numero;
				var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
				if (nota == null) continue;

				nota.Numero = nfse.IdentificacaoNfse.Numero;
				nota.ChaveNfse = nfse.IdentificacaoNfse.ChaveNFSe;

				var nfseFile = Path.Combine(Config.Arquivos.GetPathNFSe(nota.DhRecebimento),
					$"NFSe-{nota.ChaveNfse}-{nota.Numero}.xml");
				var xml = GetXmlNFSe(nota);
				File.WriteAllText(nfseFile, xml, Encoding.UTF8);
			}

			return ret;
		}

		public override RetornoWebService ConsultarSituacao(int lote, string protocolo, NotaFiscalCollection notas)
		{
			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ns1:ReqConsultaLote xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote  http://localhost:8080/WsNFe2/xsd/ReqConsultaLote.xsd\">");
			loteBuilder.Append("<Cabecalho>");
			loteBuilder.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
			loteBuilder.Append($"<CPFCNPJRemetente>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</CPFCNPJRemetente>");
			loteBuilder.Append("<Versao>1</Versao>");
			loteBuilder.Append($"<NumeroLote>{lote}</NumeroLote>");
			loteBuilder.Append("</Cabecalho>");
			loteBuilder.Append("</ns1:ReqConsultaLote>");

			var consultaLote = loteBuilder.ToString();
			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{lote}-env.xml");
				File.WriteAllText(loteFile, consultaLote, Encoding.UTF8);
			}

			string[] errosSchema;
			string[] alertasSchema;
			var schema = Path.Combine(Config.Geral.PathSchemas, @"DSF\ReqConsultaLote.xsd");
			if (!CertificadoDigital.ValidarXml(consultaLote, schema, out errosSchema, out alertasSchema))
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				foreach (var loteErro in errosSchema.Select(erro => new Evento { Codigo = "0", Descricao = erro }))
					retLote.Erros.Add(loteErro);

				foreach (var alerta in alertasSchema.Select(descricao => new Evento { Codigo = "0", Descricao = descricao }))
					retLote.Alertas.Add(alerta);

				return retLote;
			}

			string retorno;

			try
			{
				var url = GetUrl(TipoUrl.ConsultarLoteRps);
				var cliente = new DSFServiceClient(url, TimeOut, Certificado);

				retorno = cliente.ConsultarLote(consultaLote);
			}
			catch (Exception ex)
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				retLote.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retLote;
			}

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"Consultalote-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");
				File.WriteAllText(loteFile, retorno, Encoding.UTF8);
			}

			var retConsulta = new RetornoWebService();
			var xmlRet = XDocument.Parse(retorno);
			var cabeçalho = xmlRet.Element("Cabecalho");

			retConsulta.Sucesso = cabeçalho?.Element("Sucesso")?.GetValue<bool>() ?? false;
			retConsulta.CodCidade = cabeçalho?.Element("CodCidade")?.GetValue<int>() ?? 0;
			retConsulta.NumeroLote = cabeçalho?.Element("NumeroLote")?.GetValue<string>() ?? string.Empty;
			retConsulta.CPFCNPJRemetente = cabeçalho?.Element("CPFCNPJRemetente")?.GetValue<string>() ?? string.Empty;
			retConsulta.DataEnvioLote = cabeçalho?.Element("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

			var erros = xmlRet.Element("Erros");
			retConsulta.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

			var alertas = xmlRet.Element("Alertas");
			retConsulta.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

			var nfses = xmlRet.Element("ListaNFSe");
			if (nfses == null) return retConsulta;

			foreach (var nfse in nfses.Elements("ConsultaNFSe"))
			{
				var numeroRps = (nfse.Element("NumeroRPS")?.GetValue<string>() ?? string.Empty);
				var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
				if (nota == null) continue;

				nota.Numero = nfse.Element("NumeroNFe")?.GetValue<string>() ?? string.Empty;
				nota.ChaveNfse = nfse.Element("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

				var nfseFile = Path.Combine(Config.Arquivos.GetPathNFSe(nota.DhRecebimento),
					$"NFSe-{nota.ChaveNfse}-{nota.Numero}.xml");
				var xml = GetXmlNFSe(nota);
				File.WriteAllText(nfseFile, xml, Encoding.UTF8);
			}

			return retConsulta;
		}

		public override RetornoWebService ConsultarSequencialRps(string serie)
		{
			var lote = new StringBuilder();
			lote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			lote.Append("<ns1:ConsultaSeqRps xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ConsultaSeqRps.xsd\">");
			lote.Append("<Cabecalho>");
			lote.Append($"<CodCid>{Municipio.CodigoSiafi}</CodCid>");
			lote.Append($"<IMPrestador>{Config.PrestadoPadrao.InscricaoMunicipal.ZeroFill(11)}</IMPrestador>");
			lote.Append($"<CPFCNPJRemetente>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</CPFCNPJRemetente>");
			lote.Append($"<SeriePrestacao>{serie}</SeriePrestacao>");
			lote.Append("<Versao>1</Versao>");
			lote.Append("</Cabecalho>");
			lote.Append("</ns1:ConsultaSeqRps>");

			var consultaSequencia = lote.ToString();
			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathRps(DateTime.Now), $"ConSeqRPS-{DateTime.Now:yyyyMMMMdd}-env.xml");
				File.WriteAllText(loteFile, consultaSequencia, Encoding.UTF8);
			}

			string[] errosSchema;
			string[] alertasSchema;
			var schema = Path.Combine(Config.Geral.PathSchemas, @"DSF\ConsultaSeqRps.xsd");
			if (!CertificadoDigital.ValidarXml(consultaSequencia, schema, out errosSchema, out alertasSchema))
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				foreach (var loteErro in errosSchema.Select(erro => new Evento { Codigo = "0", Descricao = erro }))
					retLote.Erros.Add(loteErro);

				foreach (var alerta in alertasSchema.Select(descricao => new Evento { Codigo = "0", Descricao = descricao }))
					retLote.Alertas.Add(alerta);

				return retLote;
			}

			string retorno;
			try
			{
				var url = GetUrl(TipoUrl.ConsultarLoteRps);
				var cliente = new DSFServiceClient(url, TimeOut, Certificado);

				retorno = cliente.ConsultarLote(consultaSequencia);
			}
			catch (Exception ex)
			{
				var retLote = new RetornoWebService
				{
					Sucesso = false,
					CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
					CodCidade = Config.WebServices.CodMunicipio,
					DataEnvioLote = DateTime.Now,
					NumeroLote = "0",
					Assincrono = true
				};

				retLote.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retLote;
			}

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"Consultalote-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");
				File.WriteAllText(loteFile, retorno, Encoding.UTF8);
			}

			var retConsulta = new RetornoWebService();
			var xmlRet = XDocument.Parse(retorno);
			var cabeçalho = xmlRet.Element("Cabecalho");

			retConsulta.Sucesso = true;
			retConsulta.CodCidade = cabeçalho?.Element("CodCidade")?.GetValue<int>() ?? 0;
			retConsulta.NumeroUltimoRps = cabeçalho?.Element("NroUltimoRps")?.GetValue<string>() ?? string.Empty;
			retConsulta.CPFCNPJRemetente = cabeçalho?.Element("CPFCNPJRemetente")?.GetValue<string>() ?? string.Empty;

			var erros = xmlRet.Element("Erros");
			retConsulta.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

			var alertas = xmlRet.Element("Alertas");
			retConsulta.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

			return retConsulta;
		}

		#endregion Public

		#region Private

		private string GerarEnvEnvio()
		{
			var xmlLote = new StringBuilder();
			xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			xmlLote.Append("<ns1:ReqEnvioLoteRPS xmlns:ns1=\"http://localhost:8080/WsNFe2/lote\" xmlns:tipos=\"http://localhost:8080/WsNFe2/tp\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://localhost:8080/WsNFe2/lote http://localhost:8080/WsNFe2/xsd/ReqEnvioLoteRPS.xsd\">");
			xmlLote.Append("<Cabecalho>");
			xmlLote.Append($"<CodCidade>{Municipio.CodigoSiafi}</CodCidade>");
			xmlLote.Append($"<CPFCNPJRemetente>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</CPFCNPJRemetente>");
			xmlLote.Append($"<RazaoSocialRemetente>{Config.PrestadoPadrao.RazaoSocial}</RazaoSocialRemetente>");
			xmlLote.Append("<transacao/>");
			xmlLote.Append("<dtInicio>%DTINICIO%</dtInicio>");
			xmlLote.Append("<dtFim>%DTFIM%</dtFim>");
			xmlLote.Append("<QtdRPS>%TOTALRPS%</QtdRPS>");
			xmlLote.Append("<ValorTotalServicos>%TOTALVALOR%</ValorTotalServicos>");
			xmlLote.Append("<ValorTotalDeducoes>%TOTALDEDUCAO%</ValorTotalDeducoes>");
			xmlLote.Append("<Versao>1</Versao>");
			xmlLote.Append("<MetodoEnvio>WS</MetodoEnvio>");
			xmlLote.Append("</Cabecalho>");
			xmlLote.Append("<Lote Id=\"lote:%LOTE%\">");
			xmlLote.Append("%NOTAS%");
			xmlLote.Append("</Lote>");
			xmlLote.Append("</ns1:ReqEnvioLoteRPS>");

			return xmlLote.ToString();
		}

		private static IEnumerable<Evento> ProcessarEventos(TipoEvento tipo, XElement eventos)
		{
			var ret = new List<Evento>();
			if (eventos == null) return ret.ToArray();

			string nome;
			switch (tipo)
			{
				case TipoEvento.Erros:
					nome = "Erro";
					break;

				case TipoEvento.Alertas:
					nome = "Alerta";
					break;

				case TipoEvento.ListNFSeRps:
					nome = "ChaveNFSeRPS";
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
			}

			foreach (var evento in eventos.Elements(nome))
			{
				var item = new Evento();
				if (tipo.IsIn(TipoEvento.Erros, TipoEvento.Alertas))
				{
					item.Codigo = evento.Element("Codigo")?.GetValue<string>() ?? string.Empty;
					item.Descricao = evento.Element("Descricao")?.GetValue<string>() ?? string.Empty;
				}

				var ideRps = evento.Element("ChaveRPS");
				if (ideRps != null)
				{
					item.IdentificacaoRps.Numero = ideRps.Element("NumeroRPS")?.GetValue<string>() ?? string.Empty;
					item.IdentificacaoRps.Serie = ideRps.Element("SerieRPS")?.GetValue<string>() ?? string.Empty;
					item.IdentificacaoRps.DataEmissaoRps = ideRps.Element("DataEmissaoRPS")?.GetValue<DateTime>() ?? DateTime.MinValue;
				}

				var ideNFSe = evento.Element("ChaveNFe");
				if (ideNFSe != null)
				{
					item.IdentificacaoNfse.Numero = ideNFSe.Element("NumeroNFe")?.GetValue<string>() ?? string.Empty;
					item.IdentificacaoNfse.ChaveNFSe = ideNFSe.Element("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
					item.IdentificacaoNfse.InscricaoMunicipal = ideNFSe.Element("InscricaoPrestador")?.GetValue<string>() ?? string.Empty;
				}

				ret.Add(item);
			}

			return ret.ToArray();
		}

		private void GerarCampos(NotaFiscal nota)
		{
			recolhimento = nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal ? "A" : "R";
			situacao = nota.Status == StatusRps.Normal ? "N" : "C";

			switch (nota.NaturezaOperacao)
			{
				case NaturezaOperacao.NT02:
					operacao = "B";
					break;

				case NaturezaOperacao.NT03:
					operacao = "C";
					break;

				case NaturezaOperacao.NT04:
					operacao = "D";
					break;

				case NaturezaOperacao.NT05:
					operacao = "J";
					break;

				default:
					operacao = "A";
					break;
			}

			switch (nota.TipoTributacao)
			{
				case TipoTributacao.Isenta:
					tributacao = "C";
					break;

				case TipoTributacao.Imune:
					tributacao = "F";
					break;

				case TipoTributacao.DepositoEmJuizo:
					tributacao = "K";
					break;

				case TipoTributacao.NaoIncide:
					tributacao = "E";
					break;

				case TipoTributacao.NaoTributavel:
					tributacao = "N";
					break;

				case TipoTributacao.TributavelFixo:
					tributacao = "G";
					break;

				//Tributavel
				default:
					tributacao = "T";
					break;
			}

			if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
				tributacao = "H";

			if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.MicroEmpresarioIndividual)
				tributacao = "M";

			var valor = nota.Servico.Valores.ValorServicos - nota.Servico.Valores.ValorDeducoes;
			var rec = nota.Servico.Valores.IssRetido == SituacaoTributaria.Normal ? "N" : "S";
			var sign = $"{nota.Prestador.InscricaoMunicipal.ZeroFill(11)}{nota.IdentificacaoRps.Serie.FillLeft(5)}" + $"{nota.IdentificacaoRps.Numero.ZeroFill(12)}{nota.IdentificacaoRps.DataEmissaoRps:yyyyMMdd}{tributacao} " + $"{situacao}{rec}{Math.Round(valor * 100).ToString().ZeroFill(15)}" + $"{Math.Round(nota.Servico.Valores.ValorDeducoes * 100).ToString().ZeroFill(15)}" + $"{nota.Servico.CodigoCnae.ZeroFill(10)}{nota.Tomador.CpfCnpj.ZeroFill(14)}";

			assinatura = sign.ToSha1Hash().ToLowerInvariant();
		}

		private XmlElement GerarServicos(IEnumerable<Servico> servicos)
		{
			var itensTag = Xmldoc.CreateElement("Itens");

			foreach (var servico in servicos)
			{
				var itemTag = Xmldoc.CreateElement("Item");
				var sTributavel = servico.Tributavel == NFSeSimNao.Sim ? "S" : "N";
				itemTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DiscriminacaoServico", 1, 80, 1, RetirarAcentos ? servico.Descricao.RemoveAccent() : servico.Descricao));
				itemTag.AddTag(AdicionarTag(TipoCampo.De4, "", "Quantidade", 1, 15, 1, servico.Quantidade));
				itemTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorUnitario", 1, 20, 1, servico.ValorUnitario));
				itemTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorTotal", 1, 18, 1, servico.ValorTotal));
				itemTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Tributavel", 1, 1, 0, sTributavel));
				itensTag.AddTag(itemTag);
			}

			return itensTag;
		}

		private XmlElement GerarDeducoes(IEnumerable<Deducao> deducoes)
		{
			var deducoesTag = Xmldoc.CreateElement("Deducoes");
			foreach (var deducao in deducoes)
			{
				var deducaoTag = Xmldoc.CreateElement("Deducao");
				deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DeducaoPor", 1, 20, 1, deducao.DeducaoPor.ToString()));
				deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoDeducao", 0, 255, 1, deducao.TipoDeducao.GetStr(new[]
				{
					TipoDeducao.Nenhum, TipoDeducao.Materiais, TipoDeducao.Mercadorias, TipoDeducao.SubEmpreitada, TipoDeducao.VeiculacaoeDivulgacao, TipoDeducao.MapadeConstCivil, TipoDeducao.Servicos
				}, new[]
				{
					"", "Despesas com Materiais", "Despesas com Mercadorias", "Despesas com Subempreitada", "Servicos de Veiculacao e Divulgacao", "Mapa de Const. Civil", "Servicos"
				})));

				deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CPFCNPJReferencia", 0, 14, 1, deducao.CPFCNPJReferencia.OnlyNumbers()));
				deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroNFReferencia", 0, 10, 1, deducao.NumeroNFReferencia));
				deducaoTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorTotalReferencia", 0, 18, 1, deducao.ValorTotalReferencia));
				deducaoTag.AddTag(AdicionarTag(TipoCampo.De2, "", "PercentualDeduzir", 0, 8, 1, deducao.PercentualDeduzir));
				deducoesTag.AddTag(deducaoTag);
			}

			return deducoesTag;
		}

		#endregion Private

		#endregion Methods
	}
}