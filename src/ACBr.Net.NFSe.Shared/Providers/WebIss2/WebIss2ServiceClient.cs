// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 12-24-2017
//
// Last Modified By : RFTD
// Last Modified On : 12-24-2017
// ***********************************************************************
// <copyright file="WebIss2ServiceClient.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Service;

namespace ACBr.Net.NFSe.Providers.WebISS2
{
    internal sealed class WebIss2ServiceClient : DFeServiceClientBase<IWebIss2ServiceClient>, IWebIss2ServiceClient, IABRASF2Client
    {
        #region Constructors

        public WebIss2ServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var request = new RecepcionarLoteRpsRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).RecepcionarLoteRps(request);
            return ret.outputXML;
        }

        public string RecepcionarLoteRpsSincrono(string cabec, string msg)
        {
            var request = new RecepcionarLoteRpsSincronoRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).RecepcionarLoteRpsSincrono(request);
            return ret.outputXML;
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var request = new ConsultarNfsePorRpsRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).ConsultarNfsePorRps(request);
            return ret.outputXML;
        }

        public string ConsultarNFSeFaixa(string cabec, string msg)
        {
            var request = new ConsultarNfsePorFaixaRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).ConsultarNfsePorFaixa(request);
            return ret.outputXML;
        }

        public string ConsultarNFSeServicoTomado(string cabec, string msg)
        {
            var request = new ConsultarNfseServicoTomadoRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).ConsultarNfseServicoTomado(request);
            return ret.outputXML;
        }

        public string ConsultarNFSeServicoPrestado(string cabec, string msg)
        {
            var request = new ConsultarNfseServicoPrestadoRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).ConsultarNfseServicoPrestado(request);
            return ret.outputXML;
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var request = new ConsultarLoteRpsRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).ConsultarLoteRps(request);
            return ret.outputXML;
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var request = new CancelarNfseRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).CancelarNfse(request);
            return ret.outputXML;
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var request = new SubstituirNfseRequest(cabec, msg);
            var ret = ((IWebIss2ServiceClient)this).SubstituirNfse(request);
            return ret.outputXML;
        }

        #endregion Methods

        #region Interface Methods

        CancelarNfseResponse IWebIss2ServiceClient.CancelarNfse(CancelarNfseRequest request)
        {
            return Channel.CancelarNfse(request);
        }

        ConsultarLoteRpsResponse IWebIss2ServiceClient.ConsultarLoteRps(ConsultarLoteRpsRequest request)
        {
            return Channel.ConsultarLoteRps(request);
        }

        ConsultarNfseServicoPrestadoResponse IWebIss2ServiceClient.ConsultarNfseServicoPrestado(ConsultarNfseServicoPrestadoRequest request)
        {
            return Channel.ConsultarNfseServicoPrestado(request);
        }

        ConsultarNfseServicoTomadoResponse IWebIss2ServiceClient.ConsultarNfseServicoTomado(ConsultarNfseServicoTomadoRequest request)
        {
            return Channel.ConsultarNfseServicoTomado(request);
        }

        ConsultarNfsePorFaixaResponse IWebIss2ServiceClient.ConsultarNfsePorFaixa(ConsultarNfsePorFaixaRequest request)
        {
            return Channel.ConsultarNfsePorFaixa(request);
        }

        ConsultarNfsePorRpsResponse IWebIss2ServiceClient.ConsultarNfsePorRps(ConsultarNfsePorRpsRequest request)
        {
            return Channel.ConsultarNfsePorRps(request);
        }

        RecepcionarLoteRpsResponse IWebIss2ServiceClient.RecepcionarLoteRps(RecepcionarLoteRpsRequest request)
        {
            return Channel.RecepcionarLoteRps(request);
        }

        GerarNfseResponse IWebIss2ServiceClient.GerarNfse(GerarNfseRequest request)
        {
            return Channel.GerarNfse(request);
        }

        SubstituirNfseResponse IWebIss2ServiceClient.SubstituirNfse(SubstituirNfseRequest request)
        {
            return Channel.SubstituirNfse(request);
        }

        RecepcionarLoteRpsSincronoResponse IWebIss2ServiceClient.RecepcionarLoteRpsSincrono(RecepcionarLoteRpsSincronoRequest request)
        {
            return Channel.RecepcionarLoteRpsSincrono(request);
        }

        #endregion Interface Methods
    }
}