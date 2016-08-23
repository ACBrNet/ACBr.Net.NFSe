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
			Name = "Ginfes";
		}

		#endregion Constructors

		#region Methods

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
			MensagemErro(retornoWebservice, xmlRet);
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
			MensagemErro(retornoWebservice, xmlRet);
			if (retornoWebservice.Erros.Count > 0)
				return retornoWebservice;

			var cabeçalho = xmlRet.Element("ConsultarLoteRpsResposta");
			return retornoWebservice;
		}

		#region Private Methods

		private static void MensagemErro(RetornoWebservice retornoWs, XDocument xmlRet)
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