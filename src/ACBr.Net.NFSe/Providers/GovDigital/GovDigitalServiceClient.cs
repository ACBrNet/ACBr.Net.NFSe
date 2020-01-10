// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-22-2018
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="GovDigitalServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.GovDigital
{
    internal sealed class GovDigitalServiceClient : NFSeServiceClient<IGovDigitalService>, IABRASF2Client
    {
        #region Constructors

        public GovDigitalServiceClient(ProviderGovDigital provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var request = new RecepcionarLoteRpsRequest(cabec, msg);
            var response = Channel.RecepcionarLoteRps(request);

            return response.Response;
        }

        public string RecepcionarLoteRpsSincrono(string cabec, string msg)
        {
            var request = new RecepcionarLoteRpsSincronoRequest(cabec, msg);

            var response = Channel.RecepcionarLoteRpsSincrono(request);
            return response.Response;
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var request = new ConsultarNFSePorRpsRequest(cabec, msg);
            var response = Channel.ConsultarNFSePorRps(request);
            return response.Response;
        }

        public string ConsultarNFSeFaixa(string cabec, string msg)
        {
            var request = new ConsultarNFSePorFaixaRequest(cabec, msg);
            var response = Channel.ConsultarNFSePorFaixa(request);

            return response.Response;
        }

        public string ConsultarNFSeServicoTomado(string cabec, string msg)
        {
            var request = new ConsultarNFSeServicoTomadoRequest(cabec, msg);
            var response = Channel.ConsultarNFSeServicoTomado(request);

            return response.Response;
        }

        public string ConsultarNFSeServicoPrestado(string cabec, string msg)
        {
            var request = new ConsultarNFSeServicoPrestadoRequest(cabec, msg);
            var response = Channel.ConsultarNFSeServicoPrestado(request);

            return response.Response;
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var request = new ConsultarLoteRpsRequest(cabec, msg);
            var response = Channel.ConsultarLoteRps(request);

            return response.Response;
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var request = new CancelarNFSeRequest(cabec, msg);
            var response = Channel.CancelarNFSe(request);

            return response.Response;
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var request = new SubstituirNFSeRequest(cabec, msg);
            var response = Channel.SubstituirNFSe(request);

            return response.Response;
        }

        #endregion Methods
    }
}