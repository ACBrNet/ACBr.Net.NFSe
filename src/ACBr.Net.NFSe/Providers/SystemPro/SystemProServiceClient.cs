// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 18-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 18-08-2021
// ***********************************************************************
// <copyright file="SystemProServiceClient.cs" company="ACBr.Net">
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
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class SystemProServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public SystemProServiceClient(ProviderSystemPro provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        public SystemProServiceClient(ProviderSystemPro provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
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
            message.Append("<ns2:EnviarLoteRpsSincrono xmlns:ns2=\"http://NFSe.wsservices.systempro.com.br/\">");
            message.Append("<nfseCabecMsg>");
            message.AppendCData("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + cabec);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(msg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ns2:EnviarLoteRpsSincrono>");

            return Execute("EnviarLoteRpsSincrono", message.ToString(), "EnviarLoteRpsSincronoResponse");
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
            var baseUrl = Endpoint.Address.Uri.GetLeftPart(UriPartial.Authority);
            var soapNs = $"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                         $"xmlns:v2=\"{baseUrl}/v2.01\"";

            return Execute($"https://nfse-ws.hom-ecity.maringa.pr.gov.br/v2.01#{action}", message, responseTag, soapNs);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null)
                return xmlDocument.Root.ElementAnyNs("return")?.Value;

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new ACBrDFeCommunicationException(exMessage);
        }

        protected override Message WriteSoapEnvelope(string message, string soapAction, string soapHeader, string[] soapNamespaces)
        {
            var envelope = new StringBuilder();
            envelope.Append("<Soap:Envelope xmlns:Soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            envelope.Append("<Soap:Body>");
            envelope.Append(message);
            envelope.Append("</Soap:Body>");
            envelope.Append("</Soap:Envelope>");

            //envelope.Append("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            //envelope.Append(soapHeader.IsEmpty() ? "<SOAP-ENV:Header/>" : $"<SOAP-ENV:Header>{soapHeader}</SOAP-ENV:Header>");
            //envelope.Append("<S:Body>");
            //envelope.Append(message);
            //envelope.Append("</S:Body>");
            //envelope.Append("</S:Envelope>");

            string EnvelopeString = envelope.ToString();
            var request = Message.CreateMessage(XmlReader.Create(new StringReader(EnvelopeString)), int.MaxValue, Endpoint.Binding.MessageVersion);

            //Define a action no content type por ser SOAP 1.2
            var requestMessage = new HttpRequestMessageProperty();
            requestMessage.Headers["Content-Type"] = $"application/soap+xml;charset=UTF-8;action=\"{soapAction}\"";

            request.Properties[HttpRequestMessageProperty.Name] = requestMessage;

            return request;
        }

        #endregion Methods
    }
}