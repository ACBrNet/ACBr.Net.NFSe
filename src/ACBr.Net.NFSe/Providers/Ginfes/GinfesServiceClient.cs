// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 07-28-2016
//
// Last Modified By : RFTD
// Last Modified On : 07-28-2016
// ***********************************************************************
// <copyright file="GinfesServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	internal sealed class GinfesServiceClient : ProviderServiceBase<IGinfesService>, IGinfesService
	{
		#region Constructors

		public GinfesServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
		{
		}

		#endregion Constructors

		#region Methods

		public string ConsultarSituacao(string cabecalho, string dados)
		{
			var request = new ConsultarSituacaoRequest(cabecalho, dados);
			return ((IGinfesService)this).ConsultarSituacaoLoteRpsV3(request);
		}

		public string ConsultarLoteRps(string cabecalho, string dados)
		{
			var request = new ConsultarLoteRequest(cabecalho, dados);
			return ((IGinfesService)this).ConsultarLoteRpsV3(request);
		}

		public string ConsultarNfsePorRps(string cabecalho, string dados)
		{
			var request = new ConsultarNfsePorRpsRequest(cabecalho, dados);
			return ((IGinfesService)this).ConsultarNfsePorRpsV3(request);
		}

		public string ConsultarNfse(string cabecalho, string dados)
		{
			var request = new ConsultarNfseRequest(cabecalho, dados);
			return ((IGinfesService)this).ConsultarNfseV3(request);
		}

		public string CancelarNfse(string cabecalho, string dados)
		{
			var request = new CancelarNfseRequest(cabecalho, dados);
			return ((IGinfesService)this).CancelarNfseV3(request);
		}

		public string RecepcionarLoteRps(string cabecalho, string dados)
		{
			var request = new RecepcionarLoteRpsRequest(cabecalho, dados);
			return ((IGinfesService)this).RecepcionarLoteRpsV3(request);
		}

		#region Interface Methods

		string IGinfesService.ConsultarSituacaoLoteRpsV3(ConsultarSituacaoRequest request)
		{
			return Channel.ConsultarSituacaoLoteRpsV3(request);
		}

		string IGinfesService.ConsultarLoteRpsV3(ConsultarLoteRequest request)
		{
			return Channel.ConsultarLoteRpsV3(request);
		}

		string IGinfesService.ConsultarNfsePorRpsV3(ConsultarNfsePorRpsRequest request)
		{
			return Channel.ConsultarNfsePorRpsV3(request);
		}

		string IGinfesService.ConsultarNfseV3(ConsultarNfseRequest request)
		{
			return Channel.ConsultarNfseV3(request);
		}

		string IGinfesService.CancelarNfseV3(CancelarNfseRequest request)
		{
			return Channel.CancelarNfseV3(request);
		}

		string IGinfesService.RecepcionarLoteRpsV3(RecepcionarLoteRpsRequest request)
		{
			return Channel.RecepcionarLoteRpsV3(request);
		}

		#endregion Interface Methods

		#endregion Methods
	}
}