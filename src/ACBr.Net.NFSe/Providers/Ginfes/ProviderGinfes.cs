// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 07-28-2016
//
// Last Modified By : RFTD
// Last Modified On : 07-28-2016
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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	internal sealed class ProviderGinfes : ProviderBase
	{
		#region Constructors

		public ProviderGinfes(Configuracoes config, MunicipioNFSe municipio) : base(config, municipio)
		{
		}

		#endregion Constructors

		#region Methods

		public override RetornoWebService ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas)
		{
			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ConsultarLoteRpsEnvio>");
			loteBuilder.Append("<Prestador>");
			loteBuilder.Append($"<Cnpj>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</Cnpj>");
			loteBuilder.Append($"<InscricaoMunicipal>{Config.PrestadoPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
			loteBuilder.Append("</Prestador>");
			loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
			loteBuilder.Append("</ConsultarLoteRpsEnvio>");

			var consultaLote = loteBuilder.ToString();
			consultaLote = CertificadoDigital.Assinar(consultaLote, "", "ConsultarLoteRpsEnvio", Certificado, true);

			if (Config.Geral.Salvar)
			{
				var loteFile = Path.Combine(Config.Arquivos.GetPathLote(), $"Consultalote-{DateTime.Now:yyyyMMdd}-{lote}-env.xml");
				File.WriteAllText(loteFile, consultaLote, Encoding.UTF8);
			}

			string[] errosSchema;
			string[] alertasSchema;
			var schema = Path.Combine(Config.Geral.PathSchemas, @"GINFES\servico_consultar_lote_rps_envio_v03.xsd");
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

				foreach (var erro in errosSchema.Select(descricao => new Evento { Codigo = "0", Descricao = descricao }))
					retLote.Erros.Add(erro);

				foreach (var alerta in alertasSchema.Select(descricao => new Evento { Codigo = "0", Descricao = descricao }))
					retLote.Alertas.Add(alerta);

				return retLote;
			}

			string retorno;

			try
			{
				var url = GetUrl(TipoUrl.ConsultarLoteRps);
				var cliente = new GinfesServiceClient(url, TimeOut, Certificado);

				var cabecalho = GerarCabecalho();
				retorno = cliente.ConsultarLoteRpsV3(cabecalho, consultaLote);
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

			return retConsulta;
		}

		private static string GerarCabecalho()
		{
			var cabecalho = new StringBuilder();
			cabecalho.Append("<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\">");
			cabecalho.Append("<versaoDados>3</versaoDados>");
			cabecalho.Append("</ns2:cabecalho>");

			return cabecalho.ToString();
		}

		#endregion Methods
	}
}