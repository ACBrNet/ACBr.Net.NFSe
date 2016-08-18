// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 10-08-2014
//
// Last Modified By : RFTD
// Last Modified On : 20-04-2015
// ***********************************************************************
// <copyright file="DsfServiceClient.cs" company="ACBr.Net">
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
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace ACBr.Net.NFSe.Providers.DSF
{
	/// <summary>
	/// Class DsfServiceClient.
	/// </summary>
	internal sealed class DsfServiceClient : ProviderServiceBase<IDsfService>, IDsfService
	{
		#region Constructor

		public DsfServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
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
			var retVal = ((IDsfService)this).consultarSequencialRps(mensagemXml);
			return retVal;
		}

		/// <summary>
		/// Enviars the sincrono.
		/// </summary>
		/// <param name="mensagemXml">The mensagem XML.</param>
		/// <returns>System.String.</returns>
		public string EnviarSincrono(string mensagemXml)
		{
			var retVal = ((IDsfService)this).enviarSincrono(mensagemXml);
			return retVal;
		}

		/// <summary>
		/// Enviars the specified mensagem XML.
		/// </summary>
		/// <param name="mensagemXml">The mensagem XML.</param>
		/// <returns>System.String.</returns>
		public string Enviar(string mensagemXml)
		{
			var request = new enviarRequest(mensagemXml);
			var retVal = ((IDsfService)this).enviar(request);
			return retVal.enviarReturn;
		}

		/// <summary>
		/// Consultars the lote.
		/// </summary>
		/// <param name="mensagemXml">The mensagem XML.</param>
		/// <returns>System.String.</returns>
		public string ConsultarLote(string mensagemXml)
		{
			var retVal = ((IDsfService)this).consultarLote(mensagemXml);
			return retVal;
		}

		/// <summary>
		/// Consultars the nota.
		/// </summary>
		/// <param name="mensagemXml">The mensagem XML.</param>
		/// <returns>System.String.</returns>
		public string ConsultarNota(string mensagemXml)
		{
			var retVal = ((IDsfService)this).consultarNota(mensagemXml);
			return retVal;
		}

		/// <summary>
		/// Cancelars the specified mensagem XML.
		/// </summary>
		/// <param name="mensagemXml">The mensagem XML.</param>
		/// <returns>System.String.</returns>
		public string Cancelar(string mensagemXml)
		{
			var retVal = ((IDsfService)this).cancelar(mensagemXml);
			return retVal;
		}

		/// <summary>
		/// Consultars the nf se RPS.
		/// </summary>
		/// <param name="mensagemXml">The mensagem XML.</param>
		/// <returns>System.String.</returns>
		public string ConsultarNFSeRps(string mensagemXml)
		{
			var retVal = ((IDsfService)this).consultarNFSeRps(mensagemXml);
			return retVal;
		}

		#endregion Methods

		#region Interface Methods

		/// <summary>
		/// Consultars the sequencial RPS.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>consultarSequencialRpsResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		string IDsfService.consultarSequencialRps(string request)
		{
			return Channel.consultarSequencialRps(request);
		}

		/// <summary>
		/// Enviars the sincrono.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>enviarSincronoResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		string IDsfService.enviarSincrono(string request)
		{
			return Channel.enviarSincrono(request);
		}

		/// <summary>
		/// Enviars the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>enviarResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		enviarResponse IDsfService.enviar(enviarRequest request)
		{
			return Channel.enviar(request);
		}

		/// <summary>
		/// Consultars the lote.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>consultarLoteResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		string IDsfService.consultarLote(string request)
		{
			return Channel.consultarLote(request);
		}

		/// <summary>
		/// Consultars the nota.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>consultarNotaResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		string IDsfService.consultarNota(string request)
		{
			return Channel.consultarNota(request);
		}

		/// <summary>
		/// Cancelars the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>cancelarResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		string IDsfService.cancelar(string request)
		{
			return Channel.cancelar(request);
		}

		/// <summary>
		/// Consultars the nf se RPS.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>consultarNFSeRpsResponse.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		string IDsfService.consultarNFSeRps(string request)
		{
			return Channel.consultarNFSeRps(request);
		}

		#endregion Interface Methods
	}
}