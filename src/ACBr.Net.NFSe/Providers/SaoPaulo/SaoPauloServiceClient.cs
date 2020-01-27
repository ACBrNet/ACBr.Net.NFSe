// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="SaoPauloServiceClient.cs" company="ACBr.Net">
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

using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class SaoPauloServiceClient : NFSeSOAP12ServiceClient
    {
        #region Constructors

        public SaoPauloServiceClient(ProviderSaoPaulo provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelamentoNFe(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:CancelamentoNFeRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:CancelamentoNFeRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe", message.ToString(), "CancelamentoNFeResponse");
        }

        public string ConsultaCNPJ(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultaCNPJRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:ConsultaCNPJRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaCNPJ", message.ToString(), "ConsultaCNPJResponse");
        }

        public string ConsultaInformacoesLote(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultaInformacoesLoteRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:ConsultaInformacoesLoteRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote", message.ToString(), "ConsultaInformacoesLoteResponse");
        }

        public string ConsultaLote(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultaLoteRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:ConsultaLoteRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote", message.ToString(), "ConsultaLoteResponse");
        }

        public string ConsultaNFe(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultaNFeRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:ConsultaNFeRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe", message.ToString(), "ConsultaNFeResponse");
        }

        public string ConsultaNFeEmitidas(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultaNFeEmitidasRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:ConsultaNFeEmitidasRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeEmitidas", message.ToString(), "ConsultaNFeEmitidasResponse");
        }

        public string ConsultaNFeRecebidas(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:ConsultaNFeRecebidasRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:ConsultaNFeRecebidasRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeRecebidas", message.ToString(), "ConsultaNFeRecebidasResponse");
        }

        public string EnvioLoteRPS(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:EnvioLoteRPSRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:EnvioLoteRPSRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS", message.ToString(), "EnvioLoteRPSResponse");
        }

        public string EnvioRPS(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:EnvioRPSRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:EnvioRPSRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS", message.ToString(), "EnvioRPSResponse");
        }

        public string TesteEnvioLoteRPS(string request)
        {
            var message = new StringBuilder();
            message.Append("<nfe:TesteEnvioLoteRPSRequest>");
            message.Append("<nfe:VersaoSchema>1</nfe:VersaoSchema>");
            message.Append("<nfe:MensagemXML>");
            message.AppendCData(request);
            message.Append("</nfe:MensagemXML>");
            message.Append("</nfe:TesteEnvioLoteRPSRequest>");

            return Execute("http://www.prefeitura.sp.gov.br/nfe/ws/testeenvio", message.ToString(), "TesteEnvioLoteRPSResponse");
        }

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, responseTag, "xmlns:nfe=\"http://www.prefeitura.sp.gov.br/nfe\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("Code")?.ElementAnyNs("Value")?.GetValue<string>()} - " +
                                $"{element.ElementAnyNs("Reason")?.ElementAnyNs("Text")?.GetValue<string>()}";
                throw new ACBrDFeCommunicationException(exMessage);
            }

            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("RetornoXML").Value;
        }

        #endregion Methods
    }
}