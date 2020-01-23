// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : angelomachado
// Created          : 01-23-2020
//
// Last Modified By : angelomachado
// Last Modified On : 07-11-2020
// ***********************************************************************
// <copyright file="EquiplanoServiceClient.cs" company="ACBr.Net">
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

using System.Net;

namespace ACBr.Net.NFSe.Providers.Equiplano
{
    internal sealed class EquiplanoServiceClient : NFSeRequestServiceClient, IABRASFClient
    {
        #region Constructors

        public EquiplanoServiceClient(ProviderEquiplano provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string cabec, string msg)
        {
            return Execute("esCancelarNfse", msg);
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            return Execute("esConsultarLoteRps", msg);
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            return Execute("esConsultarNfse", msg);
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            return Execute("esConsultarNfsePorRps", msg);
        }

        public string ConsultarSituacaoLoteRps(string cabec, string msg)
        {
            return Execute("esConsultarSituacaoLoteRps", msg);
        }

        public string GerarNfse(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            return Execute("esRecepcionarLoteRps", msg);
        }

        #endregion Methods
    }
}
