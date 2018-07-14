// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 10-08-2014
//
// Last Modified By : RFTD
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="DSFServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.DSF
{
    internal sealed class DSFServiceClient : NFSeServiceClient<IDSFService>, IDSFService
    {
        #region Constructor

        public DSFServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl, null)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Consultars the sequencial RPS.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarSequencialRps(string mensagemXml)
        {
            var request = new ConsultarSequencialRpsRequest(mensagemXml);
            var retVal = ((IDSFService)this).consultarSequencialRps(request);
            return retVal.Return;
        }

        /// <summary>
        /// Enviars the sincrono.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string EnviarSincrono(string mensagemXml)
        {
            var request = new EnviarSincronoRequest(mensagemXml);
            var retVal = ((IDSFService)this).enviarSincrono(request);
            return retVal.Return;
        }

        /// <summary>
        /// Enviars the specified mensagem XML.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string Enviar(string mensagemXml)
        {
            var request = new EnviarRequest(mensagemXml);
            var retVal = ((IDSFService)this).enviar(request);
            return retVal.Return;
        }

        /// <summary>
        /// Consultars the lote.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarLote(string mensagemXml)
        {
            var request = new ConsultarLoteRequest(mensagemXml);
            var retVal = ((IDSFService)this).consultarLote(request);
            return retVal.Return;
        }

        /// <summary>
        /// Consultars the nota.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarNFSe(string mensagemXml)
        {
            var request = new ConsultarNotaRequest(mensagemXml);
            var retVal = ((IDSFService)this).consultarNota(request);
            return retVal.Return;
        }

        /// <summary>
        /// Cancelars the specified mensagem XML.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string Cancelar(string mensagemXml)
        {
            var request = new CancelarRequest(mensagemXml);
            var retVal = ((IDSFService)this).cancelar(request);
            return retVal.Return;
        }

        /// <summary>
        /// Consultars the nf se RPS.
        /// </summary>
        /// <param name="mensagemXml">The mensagem XML.</param>
        /// <returns>System.String.</returns>
        public string ConsultarNFSeRps(string mensagemXml)
        {
            var request = new ConsultarNFSeRpsRequest(mensagemXml);
            var retVal = ((IDSFService)this).consultarNFSeRps(request);
            return retVal.Return;
        }

        #region Interface Methods

        ConsultarSequencialRpsResponse IDSFService.consultarSequencialRps(ConsultarSequencialRpsRequest request)
        {
            return Channel.consultarSequencialRps(request);
        }

        EnviarSincronoResponse IDSFService.enviarSincrono(EnviarSincronoRequest request)
        {
            return Channel.enviarSincrono(request);
        }

        EnviarResponse IDSFService.enviar(EnviarRequest request)
        {
            return Channel.enviar(request);
        }

        ConsultarLoteResponse IDSFService.consultarLote(ConsultarLoteRequest request)
        {
            return Channel.consultarLote(request);
        }

        ConsultarNotaResponse IDSFService.consultarNota(ConsultarNotaRequest request)
        {
            return Channel.consultarNota(request);
        }

        CancelarResponse IDSFService.cancelar(CancelarRequest request)
        {
            return Channel.cancelar(request);
        }

        ConsultarNFSeRpsResponse IDSFService.consultarNFSeRps(ConsultarNFSeRpsRequest request)
        {
            return Channel.consultarNFSeRps(request);
        }

        #endregion Interface Methods

        #endregion Methods
    }
}