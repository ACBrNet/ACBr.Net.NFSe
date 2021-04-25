﻿// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 05-22-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="BethaServiceClient.cs" company="ACBr.Net">
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
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class BethaServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public BethaServiceClient(ProviderBetha provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        public BethaServiceClient(ProviderBetha provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            return Execute(msg.Replace("EnviarLoteRpsEnvio", "e:EnviarLoteRpsEnvio"));
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarSituacaoLoteRpsEnvio", "e:ConsultarSituacaoLoteRpsEnvio"));
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarLoteRpsEnvio", "e:ConsultarLoteRpsEnvio"));
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarNfsePorRpsEnvio", "e:ConsultarNfsePorRpsEnvio"));
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            return Execute(msg.Replace("ConsultarNfseEnvio", "e:ConsultarNfseEnvio"));
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            return Execute(msg.Replace("CancelarNfseEnvio", "e:CancelarNfseEnvio"));
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string message)
        {
            return Execute("", message, "", "", "xmlns:e=\"http://www.betha.com.br/e-nota-contribuinte-ws\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element == null) return xmlDocument.Root.FirstNode.ToString();

            var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
            throw new ACBrDFeCommunicationException(exMessage);
        }

        #endregion Methods
    }
}