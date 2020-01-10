// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 07-30-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-30-2017
// ***********************************************************************
// <copyright file="IBetha2Service.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.Betha2
{
    internal sealed class Betha2ServiceClient : NFSeServiceClient<IBetha2Service>, IABRASF2Client
    {
        #region Constructors

        public Betha2ServiceClient(ProviderBetha2 provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string cabec, string msg)
        {
            var cancelarNfseRequest = new CancelarNfseRequest
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.CancelarNfse(cancelarNfseRequest);
            return ret.Response.Retorno;
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            var substituirNfseRequest = new SubstituirNfseRequest
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.SubstituirNfse(substituirNfseRequest);
            return ret.Response.Retorno;
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var consultarLoteRpsRequest = new ConsultarLoteRpsRequest
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.ConsultarLoteRps(consultarLoteRpsRequest);
            return ret.Response.Retorno;
        }

        public string ConsultarNFSeFaixa(string cabec, string msg)
        {
            var consultarNfseFaixaRequest = new ConsultarNfseFaixaRequest()
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.ConsultarNfseFaixa(consultarNfseFaixaRequest);
            return ret.Response.Retorno;
        }

        public string ConsultarNFSeServicoTomado(string cabec, string msg)
        {
            var consultarNfseServicoTomadoRequest = new ConsultarNfseServicoTomadoRequest()
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.ConsultarNfseServicoTomado(consultarNfseServicoTomadoRequest);
            return ret.Response.Retorno;
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var consultarNfsePorRpsRequest = new ConsultarNfsePorRpsRequest()
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.ConsultarNfsePorRps(consultarNfsePorRpsRequest);
            return ret.Response.Retorno;
        }

        public string ConsultarNFSeServicoPrestado(string cabec, string msg)
        {
            var consultarNfseServicoPrestadoRequest = new ConsultarNfseServicoPrestadoRequest()
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.ConsultarNfseServicoPrestado(consultarNfseServicoPrestadoRequest);
            return ret.Response.Retorno;
        }

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var recepcionarLoteRpsRequest = new RecepcionarLoteRpsRequest()
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.RecepcionarLoteRps(recepcionarLoteRpsRequest);
            return ret.Response.Retorno;
        }

        public string RecepcionarLoteRpsSincrono(string cabec, string msg)
        {
            var recepcionarLoteRpsSincronoRequest = new RecepcionarLoteRpsSincronoRequest()
            {
                Request = new RequestBase
                {
                    Cabecalho = cabec,
                    Mensagem = msg
                }
            };

            var ret = Channel.RecepcionarLoteRpsSincrono(recepcionarLoteRpsSincronoRequest);
            return ret.Response.Retorno;
        }

        #endregion Methods
    }
}