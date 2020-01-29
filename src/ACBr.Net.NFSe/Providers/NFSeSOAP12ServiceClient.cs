// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 21-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 21-01-2020
// ***********************************************************************
// <copyright file="NFSeSOAP12ServiceClient.cs" company="ACBr.Net">
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

using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal abstract class NFSeSOAP12ServiceClient : NFSeServiceClient<IRequestChannel>
    {
        #region Constructors

        protected NFSeSOAP12ServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
            var custom = new CustomBinding(Endpoint.Binding);
            var version = custom.Elements.Find<TextMessageEncodingBindingElement>();
            version.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);

            Endpoint.Binding = custom;
        }

        protected NFSeSOAP12ServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
            var custom = new CustomBinding(Endpoint.Binding);
            var version = custom.Elements.Find<TextMessageEncodingBindingElement>();
            version.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);

            Endpoint.Binding = custom;
        }

        #endregion Constructors

        #region Methods

        protected virtual string Execute(string soapAction, string message, string responseTag, params string[] soapNamespaces)
        {
            return Execute(soapAction, message, new[] { responseTag }, soapNamespaces);
        }

        protected virtual string Execute(string soapAction, string message, string[] responseTag, params string[] soapNamespaces)
        {
            var request = WriteSoapEnvelope(message, soapNamespaces);

            RemoteCertificateValidationCallback validation = null;
            var naoValidarCertificado = !ValidarCertificadoServidor();

            if (naoValidarCertificado)
            {
                validation = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var ret = string.Empty;

            try
            {
                //Define a action no content type por ser SOAP 1.2
                var requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["Content-Type"] = $"application/soap+xml;charset=UTF-8;action=\"{soapAction}\"";

                request.Properties[HttpRequestMessageProperty.Name] = requestMessage;

                lock (serviceLock)
                {
                    var response = Channel.Request(request);
                    Guard.Against<ACBrDFeException>(response == null, "Nenhum retorno do webservice.");
                    var reader = response.GetReaderAtBodyContents();
                    ret = reader.ReadOuterXml();
                }
            }
            finally
            {
                if (naoValidarCertificado)
                    ServicePointManager.ServerCertificateValidationCallback = validation;
            }

            var xmlDocument = XDocument.Parse(ret);
            return TratarRetorno(xmlDocument, responseTag);
        }

        protected virtual Message WriteSoapEnvelope(string message, string[] soapNamespaces)
        {
            var envelope = new StringBuilder();
            envelope.Append("<soapenv:Envelope xmlns:soapenv=\"http://www.w3.org/2003/05/soap-envelope\"");

            foreach (var ns in soapNamespaces)
            {
                envelope.Append($" {ns}");
            }

            envelope.Append(">");
            envelope.Append("<soapenv:Header/>");
            envelope.Append("<soapenv:Body>");
            envelope.Append(message);
            envelope.Append("</soapenv:Body>");
            envelope.Append("</soapenv:Envelope>");

            return Message.CreateMessage(XmlReader.Create(new StringReader(envelope.ToString())), int.MaxValue, Endpoint.Binding.MessageVersion);
        }

        protected virtual bool ValidarCertificadoServidor()
        {
            return true;
        }

        protected abstract string TratarRetorno(XDocument xmlDocument, string[] responseTag);

        #endregion Methods
    }
}