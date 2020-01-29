// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 07-28-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="GinfesServiceClient.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Common;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class GinfesServiceClient : NFSeSOAP11ServiceClient
    {
        #region Constructors

        public GinfesServiceClient(ProviderGinfes provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string ConsultarSituacao(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarSituacaoLoteRpsV3>");
            message.Append("<arg0>");
            message.AppendCData(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendCData(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarSituacaoLoteRpsV3>");

            return Execute(message.ToString(), "ConsultarSituacaoLoteRpsV3Response");
        }

        public string ConsultarLoteRps(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarLoteRpsV3>");
            message.Append("<arg0>");
            message.AppendCData(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendCData(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarLoteRpsV3>");

            return Execute(message.ToString(), "ConsultarLoteRpsV3Response");
        }

        public string ConsultarNfsePorRps(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarNfsePorRpsV3>");
            message.Append("<arg0>");
            message.AppendCData(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendCData(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarNfsePorRpsV3>");

            return Execute(message.ToString(), "ConsultarNfsePorRpsV3Response");
        }

        public string ConsultarNfse(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarNfseV3>");
            message.Append("<arg0>");
            message.AppendCData(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendCData(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarNfseV3Response>");

            return Execute(message.ToString(), "RecepcionarLoteRpsV3Response");
        }

        public string CancelarNfse(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:CancelarNfseV3>");
            message.Append("<arg0>");
            message.AppendCData(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendCData(dados);
            message.Append("</arg1>");
            message.Append("</gin:CancelarNfseV3>");

            return Execute(message.ToString(), "CancelarNfseV3Response");
        }

        public string RecepcionarLoteRps(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:RecepcionarLoteRpsV3>");
            message.Append("<arg0>");
            message.AppendCData(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendCData(dados);
            message.Append("</arg1>");
            message.Append("</gin:RecepcionarLoteRpsV3>");

            return Execute(message.ToString(), "RecepcionarLoteRpsV3Response");
        }

        private string Execute(string message, string responseTag)
        {
            var ns = Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao ?
                            "xmlns:gin=\"http://homologacao.ginfes.com.br\"" : "xmlns:gin=\"http://producao.ginfes.com.br\"";

            return Execute("", message, responseTag, ns);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;
        }

        #endregion Methods
    }
}