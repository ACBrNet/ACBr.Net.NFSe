// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 21-01-2020
//
// Last Modified By : RFTD
// Last Modified On : 21-01-2020
// ***********************************************************************
// <copyright file="VitoriaServiceClient.cs" company="ACBr.Net">
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
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class VitoriaServiceClient : NFSeRequestServiceClient, IABRASF2Client
    {
        #region Constructors

        public VitoriaServiceClient(ProviderVitoria provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
            var custom = new CustomBinding(Endpoint.Binding);
            var version = custom.Elements.Find<TextMessageEncodingBindingElement>();
            version.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);

            Endpoint.Binding = custom;
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<CancelarNfse xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</CancelarNfse>");

            return Execute("CancelarNfse", message.ToString());
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<SubstituirNfse xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</SubstituirNfse>");

            return Execute("SubstituirNfse", message.ToString());
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarLoteRps xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</ConsultarLoteRps>");

            return Execute("ConsultarLoteRps", message.ToString());
        }

        public string ConsultarNFSeFaixa(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfseFaixa xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</ConsultarNfseFaixa>");

            return Execute("ConsultarNfseFaixa", message.ToString());
        }

        public string ConsultarNFSeServicoTomado(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfseServicoTomado xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</ConsultarNfseServicoTomado>");

            return Execute("ConsultarNfseServicoTomado", message.ToString());
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfsePorRps xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</ConsultarNfsePorRps>");

            return Execute("ConsultarNfsePorRps", message.ToString());
        }

        public string ConsultarNFSeServicoPrestado(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ConsultarNfseServicoPrestado xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</ConsultarNfseServicoPrestado>");

            return Execute("ConsultarNfseServicoPrestado", message.ToString());
        }

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRps xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</RecepcionarLoteRps>");

            return Execute("RecepcionarLoteRps", message.ToString());
        }

        public string RecepcionarLoteRpsSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<RecepcionarLoteRpsSincrono xmlns=\"http://www.abrasf.org.br/nfse.xsd\">");
            message.Append($"<mensagemXML>{msg.AjustarEnvio()}</mensagemXML>");
            message.Append("</RecepcionarLoteRpsSincrono>");

            return Execute("RecepcionarLoteRpsSincrono", message.ToString());
        }

        private string Execute(string responseTag, string message)
        {
            var envelope = new StringBuilder();
            envelope.Append("<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">");
            envelope.Append("<soap:Body>");
            envelope.Append(message);
            envelope.Append("</soap:Body>");
            envelope.Append("</soap:Envelope>");

            var msg = Message.CreateMessage(XmlReader.Create(new StringReader(envelope.ToString())), int.MaxValue, Endpoint.Binding.MessageVersion);
            var ret = Execute(msg);

            var xmlDocument = XDocument.Parse(ret);
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
                throw new ACBrDFeCommunicationException(element.ElementAnyNs("Reason").GetValue<string>());

            return xmlDocument.ElementAnyNs(responseTag + "Response").ElementAnyNs(responseTag + "Result").Value;
        }

        #endregion Methods
    }
}