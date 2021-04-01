// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 03-29-2021
//
// ***********************************************************************
// <copyright file="ISSeServiceClient.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2021 Grupo ACBr.Net
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
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class ISSeServiceClient : NFSeSOAP12ServiceClient, IServiceClient
    {
        #region Constructors

        public ISSeServiceClient(ProviderISSe provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        public ISSeServiceClient(ProviderISSe provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
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
            //message.Append("<EnviarLoteRpsSincrono>");
            //message.Append("<xml>");
            //message.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            var message = new StringBuilder();
            message.Append("<v2:EnviarLoteRpsSincrono soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<xml xsi:type=\"xsd: string\">");
            message.AppendEnvio(msg);
            message.Append("</xml>");
            message.Append("</v2:EnviarLoteRpsSincrono>");

            const string SoapAction = "https://nfse-ws.ecity.maringa.pr.gov.br/v2.01#EnviarLoteRpsSincrono";

            return Execute(SoapAction, message.ToString(), "EnviarLoteRpsSincronoResponse");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string soapAction, string message, string responseTag)
        {
            var TagV2 = EhHomologação ? "xmlns:v2=\"https://nfse-ws.hom-ecity.maringa.pr.gov.br/v2.01\"" :
                                            "xmlns:v2=\"https://nfse-ws.ecity.maringa.pr.gov.br/v2.01\"";

            return Execute(soapAction, message, "", responseTag, TagV2, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new ACBrDFeCommunicationException(exMessage);
            }

            var reader = xmlDocument.ElementAnyNs(responseTag[0]).CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml().Replace("ns2:", string.Empty);
        }

        #endregion Methods
    }
}