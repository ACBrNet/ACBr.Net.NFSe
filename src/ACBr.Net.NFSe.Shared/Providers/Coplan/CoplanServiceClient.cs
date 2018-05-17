// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-16-2018
//
// Last Modified By : RFTD
// Last Modified On : 05-16-2018
// ***********************************************************************
// <copyright file="CoplanServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.Coplan
{
    internal sealed class CoplanServiceClient : DFeServiceClientBase<ICoplanService>, IABRASF2Client
    {
        #region Constructors

        public CoplanServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(
            url, timeOut, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var request = new RecepcionarLoteRpsRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.RecepcionarLoteRps(request);

            return response.Response.OutputXml;
        }

        public string RecepcionarLoteRpsSincrono(string cabec, string msg)
        {
            var request = new RecepcionarLoteRpsSincronoRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.RecepcionarLoteRpsSincrono(request);

            return response.Response.OutputXml;
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var request = new ConsultarNFSePorRpsRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.ConsultarNFSePorRps(request);

            return response.Response.OutputXml;
        }

        public string ConsultarNFSeFaixa(string cabec, string msg)
        {
            var request = new ConsultarNFSeFaixaRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.ConsultarNFSeFaixa(request);

            return response.Response.OutputXml;
        }

        public string ConsultarNFSeServicoTomado(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeServicoPrestado(string cabec, string msg)
        {
            var request = new ConsultarNFSeServicoPrestadoRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.ConsultarNFSeServicoPrestado(request);

            return response.Response.OutputXml;
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var request = new ConsultarLoteRpsRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.ConsultarLoteRps(request);

            return response.Response.OutputXml;
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var request = new CancelarNFSeRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.CancelarNFSe(request);

            return response.Response.OutputXml;
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var request = new SubstituirNFSeRequest(new RequestBase()
            {
                Cabecalho = cabec,
                Mensagem = msg
            });

            var response = Channel.SubstituirNFSe(request);

            return response.Response.OutputXml;
        }

        #endregion Methods
    }
}