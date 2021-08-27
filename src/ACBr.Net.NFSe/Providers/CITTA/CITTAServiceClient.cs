// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 26-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 26-08-2021
// ***********************************************************************
// <copyright file="ISSeServiceClient.cs" company="ACBr.Net">
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

using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class CITTAServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public CITTAServiceClient(ProviderCITTA provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        public CITTAServiceClient(ProviderCITTA provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string EnviarSincrono(string cabec, string msg)
        {

            var message = new StringBuilder();
            message.Append("<nfse:RecepcionarLoteRpsSincronoEnvio>");
            message.AppendCData(msg); //.Replace("EnviarLoteRpsSincronoEnvio", "nfse:RecepcionarLoteRpsSincronoEnvio")
            message.Append("</nfse:RecepcionarLoteRpsSincronoEnvio>");

            return Execute("RecepcionarLoteRpsSincronoEnvio", message.ToString(), "RecepcionarLoteRpsSincronoResposta");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<v2:ConsultarLoteRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<xml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</xml>");
            message.Append("</v2:ConsultarLoteRps>");

            return Execute("ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<v2:ConsultarNfseRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<xml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</xml>");
            message.Append("</v2:ConsultarNfseRps>");

            return Execute("ConsultarNfseRps", message.ToString(), "ConsultarNfseRpsResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<v2:ConsultarNfseFaixa soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<xml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</xml>");
            message.Append("</v2:ConsultarNfseFaixa>");

            return Execute("ConsultarNfseFaixa", message.ToString(), "ConsultarNfseFaixaResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<v2:CancelarNfse soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<xml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</xml>");
            message.Append("</v2:CancelarNfse>");

            return Execute("CancelarNfse", message.ToString(), "CancelarNfseResponse");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<v2:SubstituirNfse soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<xml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</xml>");
            message.Append("</v2:SubstituirNfse>");

            return Execute("SubstituirNfse", message.ToString(), "SubstituirNfseResponse");
        }

        private string Execute(string action, string message, params string[] responseTag)
        {
            var ns = "xmlns:nfse=\"http://nfse.abrasf.org.br\" xmlns:nfs=\"http://localhost:8080/nfse/services/nfseSOAP?wsdl\"";
            if (action == "RecepcionarLoteRpsSincronoEnvio") ns += " xmlns:nfse1=\"http://nfse.citta.com.br\"";

            return Execute(action, message, responseTag, ns);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null)
                return xmlDocument.Root.ElementAnyNs("return")?.Value;

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new ACBrDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}