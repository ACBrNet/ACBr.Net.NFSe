// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 10-08-2014
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="DSFServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class DSFServiceClient : NFSeSOAP11ServiceClient
    {
        #region Constructor

        public DSFServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Consultars the sequencial RPS.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarSequencialRps(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:consultarSequencialRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarSequencialRps>");

            return Execute(message.ToString(), "consultarSequencialRpsResponse", "consultarSequencialRpsReturn");
        }

        /// <summary>
        /// Enviars the sincrono.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string EnviarSincrono(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:enviarSincrono soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:enviarSincrono>");

            return Execute(message.ToString(), "enviarSincronoResponse", "enviarSincronoReturn");
        }

        public string EnviarTeste(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:testeEnviar soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:testeEnviar>");

            return Execute(message.ToString(), "testeEnviarResponse", "testeEnviarReturn");
        }

        /// <summary>
        /// Enviars the specified mensagem XML.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string Enviar(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:enviar soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:enviar>");

            return Execute(message.ToString(), "enviarResponse", "enviarReturn");
        }

        /// <summary>
        /// Consultars the lote.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarLote(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:consultarLote soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarLote>");

            return Execute(message.ToString(), "consultarLoteResponse", "consultarLoteReturn");
        }

        /// <summary>
        /// Consultars the nota.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarNFSe(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:consultarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarNota>");

            return Execute(message.ToString(), "consultarNotaResponse", "consultarNotaReturn");
        }

        /// <summary>
        /// Cancelars the specified mensagem XML.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string Cancelar(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:cancelar soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:cancelar>");

            return Execute(message.ToString(), "cancelarResponse", "cancelarReturn");
        }

        /// <summary>
        /// Consultars the nf se RPS.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarNFSeRps(string mensagemXml)
        {
            var message = new StringBuilder();
            message.Append("<proc:consultarNFSeRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(mensagemXml);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarNFSeRps>");

            return Execute(message.ToString(), "consultarNFSeRpsResponse", "consultarNFSeRpsReturn");
        }

        private string Execute(string message, params string[] reponseTags)
        {
            return Execute("", message, reponseTags, "xmlns:proc=\"http://proces.wsnfe2.dsfnet.com.br\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, params string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs(responseTag[1]).Value;
        }

        #endregion Methods
    }
}