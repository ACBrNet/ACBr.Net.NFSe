// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="SaoPauloServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    internal sealed class SaoPauloServiceClient : NFSeServiceClient<ISaoPauloServiceClient>, ISaoPauloServiceClient
    {
        #region Constructors

        public SaoPauloServiceClient(ProviderSaoPaulo provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelamentoNFe(string request)
        {
            var loteRequest = new CancelamentoNFeRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).CancelamentoNFe(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaCNPJ(string request)
        {
            var loteRequest = new ConsultaCNPJRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaCNPJ(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaInformacoesLote(string request)
        {
            var loteRequest = new ConsultaInformacoesLoteRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaInformacoesLote(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaLote(string request)
        {
            var loteRequest = new ConsultaLoteRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaLote(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaNFe(string request)
        {
            var loteRequest = new ConsultaNFeRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaNFe(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaNFeEmitidas(string request)
        {
            var loteRequest = new ConsultaNFeEmitidasRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaNFeEmitidas(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaNFeRecebidas(string request)
        {
            var loteRequest = new ConsultaNFeRecebidasRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaNFeRecebidas(loteRequest);
            return ret.RetornoXML;
        }

        public string EnvioLoteRPS(string request)
        {
            var loteRequest = new EnvioLoteRPSRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).EnvioLoteRPS(loteRequest);
            return ret.RetornoXML;
        }

        public string EnvioRPS(string request)
        {
            var loteRequest = new EnvioRPSRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).EnvioRPS(loteRequest);
            return ret.RetornoXML;
        }

        public string TesteEnvioLoteRPS(string request)
        {
            var loteRequest = new TesteEnvioLoteRPSRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).TesteEnvioLoteRPS(loteRequest);
            return ret.RetornoXML;
        }

        #region Interface Methods

        CancelamentoNFeResponse ISaoPauloServiceClient.CancelamentoNFe(CancelamentoNFeRequest request)
        {
            return Channel.CancelamentoNFe(request);
        }

        ConsultaCNPJResponse ISaoPauloServiceClient.ConsultaCNPJ(ConsultaCNPJRequest request)
        {
            return Channel.ConsultaCNPJ(request);
        }

        ConsultaInformacoesLoteResponse ISaoPauloServiceClient.ConsultaInformacoesLote(ConsultaInformacoesLoteRequest request)
        {
            return Channel.ConsultaInformacoesLote(request);
        }

        ConsultaLoteResponse ISaoPauloServiceClient.ConsultaLote(ConsultaLoteRequest request)
        {
            return Channel.ConsultaLote(request);
        }

        ConsultaNFeResponse ISaoPauloServiceClient.ConsultaNFe(ConsultaNFeRequest request)
        {
            return Channel.ConsultaNFe(request);
        }

        ConsultaNFeEmitidasResponse ISaoPauloServiceClient.ConsultaNFeEmitidas(ConsultaNFeEmitidasRequest request)
        {
            return Channel.ConsultaNFeEmitidas(request);
        }

        ConsultaNFeRecebidasResponse ISaoPauloServiceClient.ConsultaNFeRecebidas(ConsultaNFeRecebidasRequest request)
        {
            return Channel.ConsultaNFeRecebidas(request);
        }

        EnvioLoteRPSResponse ISaoPauloServiceClient.EnvioLoteRPS(EnvioLoteRPSRequest request)
        {
            return Channel.EnvioLoteRPS(request);
        }

        EnvioRPSResponse ISaoPauloServiceClient.EnvioRPS(EnvioRPSRequest request)
        {
            return Channel.EnvioRPS(request);
        }

        TesteEnvioLoteRPSResponse ISaoPauloServiceClient.TesteEnvioLoteRPS(TesteEnvioLoteRPSRequest request)
        {
            return Channel.TesteEnvioLoteRPS(request);
        }

        #endregion Interface Methods

        #endregion Methods
    }
}