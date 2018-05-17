// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 12-26-2017
//
// Last Modified By : RFTD
// Last Modified On : 12-26-2017
// ***********************************************************************
// <copyright file="AbacoServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.Abaco
{
    internal sealed class AbacoServiceClient : DFeServiceClientBase<IAbacoServiceClient>, IABRASFClient, IAbacoServiceClient
    {
        #region Constructors

        public AbacoServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var request = new RecepcionarLoteRequest(cabec, msg);
            var ret = ((IAbacoServiceClient)this).RecepcionarLote(request);
            return ret.Outputxml;
        }

        public string ConsultarSituacaoLoteRps(string cabec, string msg)
        {
            var request = new ConsultarSituacaoLoteRequest(cabec, msg);
            var ret = ((IAbacoServiceClient)this).ConsultarSituacaoLote(request);
            return ret.Outputxml;
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var request = new ConsultarNfsePorRpsRequest(cabec, msg);
            var ret = ((IAbacoServiceClient)this).ConsultarNfsePorRps(request);
            return ret.Outputxml;
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var request = new ConsultarNfseRequest(cabec, msg);
            var ret = ((IAbacoServiceClient)this).ConsultarNfse(request);
            return ret.Outputxml;
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var request = new ConsultarLoteRequest(cabec, msg);
            var ret = ((IAbacoServiceClient)this).ConsultarLote(request);
            return ret.Outputxml;
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var request = new CancelarNfseRequest(cabec, msg);
            var ret = ((IAbacoServiceClient)this).CancelarNfse(request);
            return ret.Outputxml;
        }

        public string GerarNfse(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        #region Interface Methods

        RecepcionarLoteResponse IAbacoServiceClient.RecepcionarLote(RecepcionarLoteRequest request)
        {
            return Channel.RecepcionarLote(request);
        }

        ConsultarSituacaoLoteResponse IAbacoServiceClient.ConsultarSituacaoLote(ConsultarSituacaoLoteRequest request)
        {
            return Channel.ConsultarSituacaoLote(request);
        }

        ConsultarLoteResponse IAbacoServiceClient.ConsultarLote(ConsultarLoteRequest request)
        {
            return Channel.ConsultarLote(request);
        }

        ConsultarNfsePorRpsResponse IAbacoServiceClient.ConsultarNfsePorRps(ConsultarNfsePorRpsRequest request)
        {
            return Channel.ConsultarNfsePorRps(request);
        }

        ConsultarNfseResponse IAbacoServiceClient.ConsultarNfse(ConsultarNfseRequest request)
        {
            return Channel.ConsultarNfse(request);
        }

        CancelarNfseResponse IAbacoServiceClient.CancelarNfse(CancelarNfseRequest request)
        {
            return Channel.CancelarNfse(request);
        }

        #endregion Interface Methods

        #endregion Methods
    }
}