// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 10-08-2014
//
// Last Modified By : Rafael Dias
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

using System;
using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Common;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class DSFServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructor

        public DSFServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null)
        {
        }

        #endregion Constructor

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var servico = EhHomologa��o ? "testeEnviar" : "enviar";

            var message = new StringBuilder();
            message.Append($"<proc:{servico} soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append($"</proc:{servico}>");

            var response = EhHomologa��o ? "testeEnviarResponse" : "enviarResponse";
            var responseReturn = EhHomologa��o ? "testeEnviarReturn" : "enviarReturn";

            return Execute(message.ToString(), response, responseReturn);
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            if (EhHomologa��o) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:enviarSincrono soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:enviarSincrono>");

            return Execute(message.ToString(), "enviarSincronoResponse", "enviarSincronoReturn");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            if (EhHomologa��o) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarLote soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarLote>");

            return Execute(message.ToString(), "consultarLoteResponse", "consultarLoteReturn");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            if (EhHomologa��o) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarSequencialRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarSequencialRps>");

            return Execute(message.ToString(), "consultarSequencialRpsResponse", "consultarSequencialRpsReturn");
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            if (EhHomologa��o) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarNFSeRps soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarNFSeRps>");

            return Execute(message.ToString(), "consultarNFSeRpsResponse", "consultarNFSeRpsReturn");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            if (EhHomologa��o) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:consultarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:consultarNota>");

            return Execute(message.ToString(), "consultarNotaResponse", "consultarNotaReturn");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            if (EhHomologa��o) throw new NotImplementedException();

            var message = new StringBuilder();
            message.Append("<proc:cancelar soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            message.Append("<mensagemXml xsi:type=\"xsd:string\">");
            message.AppendEnvio(msg);
            message.Append("</mensagemXml>");
            message.Append("</proc:cancelar>");

            return Execute(message.ToString(), "cancelarResponse", "cancelarReturn");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string message, params string[] reponseTags)
        {
            return Execute("", message, reponseTags, "xmlns:proc=\"http://proces.wsnfe2.dsfnet.com.br\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs(responseTag[1]).Value;
        }

        #endregion Methods
    }
}