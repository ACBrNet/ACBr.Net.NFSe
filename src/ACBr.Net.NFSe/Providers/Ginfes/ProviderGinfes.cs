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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
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

		public override RetornoWebService ConsultarSituacao(int lote, string protocolo)
		{
			var retornoWS = new RetornoWebService()
			{
				Sucesso = false,
				CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
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
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
			loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadoPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
			loteBuilder.Append("</Prestador>");
			loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
			loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
			retornoWS.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarSituacaoLoteRpsEnvio", Certificado);

			GravarArquivoEmDisco(retornoWS.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

			// Verifica Schema
			var retSchema = ValidarSchema(retornoWS.XmlEnvio, "GINFES", "servico_consultar_situacao_lote_rps_envio_v03.xsd");
			if (retSchema != null)
				return retSchema;

			// Recebe mensagem de retorno
			try
			{
				var cliente = GetCliente(TipoUrl.ConsultarSituacao);
				var cabecalho = GerarCabecalho();
				retornoWS.XmlRetorno = cliente.ConsultarSituacao(cabecalho, retornoWS.XmlEnvio);
			}
			catch (Exception ex)
			{
				retornoWS.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retornoWS;
			}
			GravarArquivoEmDisco(retornoWS.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

			// Analisa mensagem de retorno
			var xmlRet = XDocument.Parse(retornoWS.XmlRetorno);
			MensagemErro(retornoWS, xmlRet);
			if (retornoWS.Erros.Count > 0)
				return retornoWS;

			retornoWS.Situacao = xmlRet.Root?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
			retornoWS.Sucesso = (retornoWS.Situacao == "4");
			retornoWS.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;

			return retornoWS;
		}

		public override RetornoWebService ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas)
		{
			var retornoWebService = new RetornoWebService()
			{
				Sucesso = false,
				CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
				CodCidade = Config.WebServices.CodMunicipio,
				DataEnvioLote = DateTime.Now,
				NumeroLote = "0",
				Assincrono = true
			};

			var loteBuilder = new StringBuilder();
			loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			loteBuilder.Append("<ConsultarLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_lote_rps_envio_v03.xsd\">");
			loteBuilder.Append("<Prestador>");
			loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
			loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadoPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
			loteBuilder.Append("</Prestador>");
			loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
			loteBuilder.Append("</ConsultarLoteRpsEnvio>");

			retornoWebService.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarLoteRpsEnvio", Certificado);

			GravarArquivoEmDisco(retornoWebService.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

			// Verifica Schema
			var retSchema = ValidarSchema(retornoWebService.XmlEnvio, "GINFES", "servico_consultar_lote_rps_envio_v03.xsd");
			if (retSchema != null)
				return retSchema;

			// Recebe mensagem de retorno
			try
			{
				var cliente = GetCliente(TipoUrl.ConsultarLoteRps);
				var cabecalho = GerarCabecalho();
				retornoWebService.XmlRetorno = cliente.ConsultarLoteRps(cabecalho, retornoWebService.XmlEnvio);
			}
			catch (Exception ex)
			{
				retornoWebService.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
				return retornoWebService;
			}
			GravarArquivoEmDisco(retornoWebService.XmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

			// Analisa mensagem de retorno
			var xmlRet = XDocument.Parse(retornoWebService.XmlRetorno);
			MensagemErro(retornoWebService, xmlRet);
			if (retornoWebService.Erros.Count > 0)
				return retornoWebService;

			var cabeçalho = xmlRet.Element("ConsultarLoteRpsResposta");
			return retornoWebService;
		}

		#region Private Methods

		private static void MensagemErro(RetornoWebService retornoWs, XDocument xmlRet)
		{
			var mensagens = xmlRet.ElementAnyNs("ListaMensagemRetorno");
			if (mensagens == null) return;

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

		private static string GerarCabecalho()
		{
			var cabecalho = new StringBuilder();
			cabecalho.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			cabecalho.Append("<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\">");
			cabecalho.Append("<versaoDados>3</versaoDados>");
			cabecalho.Append("</ns2:cabecalho>");
			return cabecalho.ToString();
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

		#endregion Private Methods

		#endregion Methods
	}
}