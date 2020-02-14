﻿// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Felipe Silveira/Transis
// Created          : 14-02-2020
//
// ***********************************************************************
// <copyright file="Pronim2ServiceClient.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2020 Grupo ACBr.Net
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
    internal sealed class Pronim2ServiceClient : NFSeSOAP11ServiceClient, IABRASF2Client
    {
        #region Constructors

        public Pronim2ServiceClient(ProviderPronim2 provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        public Pronim2ServiceClient(ProviderPronim2 provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string cabec, string msg)
        {
            // Dados de homologação
            // CNPJ=01001001000113, IM:15000, Login=01001001000113, Senha=123456;
            var message = new StringBuilder();
            message.Append("<ws:cancelarNfse>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:cancelarNfse>");

            return Execute("cancelarNfse", message.ToString(), "cancelarNfseResponse");
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:substituirNfse>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:substituirNfse>");

            return Execute("substituirNfse", message.ToString(), "substituirNfseResponse");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:consultarLoteRps>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:consultarLoteRps>");

            return Execute("consultarLoteRps", message.ToString(), "consultarLoteRpsResponse");
        }

        public string ConsultarNFSeFaixa(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:consultarNfsePorFaixa>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:consultarNfsePorFaixa>");

            return Execute("consultarNfsePorFaixa", message.ToString(), "consultarNfsePorFaixaResponse");
        }

        public string ConsultarNFSeServicoTomado(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:consultarNfseServicoTomado>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:consultarNfseServicoTomado>");

            return Execute("consultarNfseServicoTomado", message.ToString(), "consultarNfseServicoTomadoResponse");
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:consultarNfsePorRps>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:consultarNfsePorRps>");

            return Execute("consultarNfsePorRps", message.ToString(), "consultarNfsePorRpsResponse");
        }

        public string ConsultarNFSeServicoPrestado(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:consultarNfseServicoPrestado>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:consultarNfseServicoPrestado>");

            return Execute("consultarNfseServicoPrestado", message.ToString(), "consultarNfseServicoPrestadoResponse");
        }

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:recepcionarLoteRps>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:recepcionarLoteRps>");

            return Execute("recepcionarLoteRps", message.ToString(), "recepcionarLoteRpsResponse");
        }

        public string RecepcionarLoteRpsSincrono(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<ws:recepcionarLoteRpsSincrono>");
            message.Append(msg);
            message.Append("<username>");
            message.Append(Provider.Configuracoes.WebServices.Usuario);
            message.Append("</username>");
            message.Append("<password>");
            message.Append(Provider.Configuracoes.WebServices.Senha);
            message.Append("</password>");
            message.Append("</ws:recepcionarLoteRpsSincrono>");

            return Execute("recepcionarLoteRpsSincrono", message.ToString(), "recepcionarLoteRpsSincronoResponse");
        }

        private string Execute(string soapAction, string message, string responseTag)
        {
            return Execute(soapAction, message, responseTag, "xmlns:ws=\"http://ws.issweb.fiorilli.com.br/\"");
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