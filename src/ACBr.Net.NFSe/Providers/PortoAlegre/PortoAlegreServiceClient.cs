// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-16-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="PortoAlegreServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.PortoAlegre
{
    internal sealed class PortoAlegreServiceClient : NFSeServiceClient<IPortoAlegreServiceClient>, IABRASFClient
    {
        #region Constructors

        public PortoAlegreServiceClient(ProviderPortoAlegre provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new CancelarNfseRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.CancelarNfse(inValue);
            return retVal.outputXML;
        }

        public string ConsultarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarLoteRps(inValue);
            return retVal.outputXML;
        }

        public string ConsultarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarNfseRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarNfse(inValue);
            return retVal.outputXML;
        }

        public string ConsultarNfsePorFaixa(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarNfsePorFaixaRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarNfsePorFaixa(inValue);
            return retVal.outputXML;
        }

        public string ConsultarNFSePorRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarNfsePorRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarNfsePorRps(inValue);
            return retVal.outputXML;
        }

        public string ConsultarSituacaoLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarSituacaoLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarSituacaoLoteRps(inValue);
            return retVal.outputXML;
        }

        public string RecepcionarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new RecepcionarLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.RecepcionarLoteRps(inValue);
            return retVal.outputXML;
        }

        public string GerarNfse(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new GerarNfseRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.GerarNfse(inValue);
            return retVal.outputXML;
        }

        #endregion Methods
    }
}