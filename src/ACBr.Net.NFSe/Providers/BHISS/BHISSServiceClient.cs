// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 25-01-2020
//
// Last Modified By : RFTD
// Last Modified On : 25-01-2020
// ***********************************************************************
// <copyright file="BHISSServiceClient.cs" company="ACBr.Net">
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
    internal sealed class BHISSServiceClient : NFSeSOAP11ServiceClient, IABRASFClient
    {
        #region Constructors

        public BHISSServiceClient(ProviderBHISS provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:CancelarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:CancelarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
        }

        public string ConsultarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfse", message.ToString(), "ConsultarNfseResponse");
        }

        public string ConsultarNfsePorFaixa(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfsePorFaixaRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfsePorFaixaRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfsePorFaixa", message.ToString(), "ConsultarNfsePorFaixaResponse");
        }

        public string ConsultarNFSePorRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfsePorRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfsePorRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarSituacaoLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarSituacaoLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarSituacaoLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
        }

        public string RecepcionarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:RecepcionarLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string GerarNfse(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:GerarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:GerarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/GerarNfse", message.ToString(), "GerarNfseResponse");
        }

        private string Execute(string action, string message, string responseTag)
        {
            return Execute(action, message, responseTag, "xmlns:ws=\"http://ws.bhiss.pbh.gov.br\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new ACBrDFeCommunicationException(exMessage);
            }

            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("outputXML").Value;
        }

        #endregion Methods
    }
}