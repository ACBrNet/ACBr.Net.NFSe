// ***********************************************************************
// Assembly         : ACBr.Net.NFSe.Shared
// Author           : RFTD
// Created          : 06-02-2018
//
// Last Modified By : RFTD
// Last Modified On : 06-02-2018
// ***********************************************************************
// <copyright file="FissLexServiceClient.cs" company="ACBr.Net">
//		        	   The MIT License (MIT)
//	     		Copyright (c) 2016-2018 Grupo ACBr.Net
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
using ACBr.Net.DFe.Core.Service;
using System;
using System.Security.Cryptography.X509Certificates;

namespace ACBr.Net.NFSe.Providers.FissLex
{
    internal sealed class FissLexConsultarLoteRpsServiceClient : DFeServiceClientBase<IFissLexConsultarLoteRpsServiceClient>, IABRASFClient
    {
        #region Constructors

        public FissLexConsultarLoteRpsServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacaoLoteRps(string cabec, string msg)
        {
            return Channel.ConsultarLoteRps(msg);
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string GerarNfse(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}