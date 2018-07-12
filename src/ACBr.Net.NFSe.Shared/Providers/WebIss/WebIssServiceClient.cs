// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-13-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="WebIssServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.WebISS
{
    // ReSharper disable once InconsistentNaming
    internal sealed class WebIssServiceClient : NFSeServiceClient<IWebIssServiceClient>, IABRASFClient
    {
        #region Constructors

        public WebIssServiceClient(ProviderWebIss provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            return Channel.RecepcionarLoteRps(cabec, msg);
        }

        public string ConsultarSituacaoLoteRps(string cabec, string msg)
        {
            return Channel.ConsultarSituacaoLoteRps(cabec, msg);
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            return Channel.ConsultarNfsePorRps(cabec, msg);
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            return Channel.ConsultarNfse(cabec, msg);
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            return Channel.ConsultarLoteRps(cabec, msg);
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            return Channel.CancelarNfse(cabec, msg);
        }

        public string GerarNfse(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}