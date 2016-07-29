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

		public string CancelarNfse(string arg0)
		{
			return base.Channel.CancelarNfse(arg0);
		}

		public string ConsultarLoteRps(string arg0)
		{
			return Channel.ConsultarLoteRps(arg0);
		}

		public string ConsultarLoteRpsV3(string arg0, string arg1)
		{
			return Channel.ConsultarLoteRpsV3(arg0, arg1);
		}

		public string ConsultarNfse(string arg0)
		{
			return Channel.ConsultarNfse(arg0);
		}

		public string ConsultarNfsePorRps(string arg0)
		{
			return Channel.ConsultarNfsePorRps(arg0);
		}

		public string ConsultarNfsePorRpsV3(string arg0, string arg1)
		{
			return Channel.ConsultarNfsePorRpsV3(arg0, arg1);
		}

		public string ConsultarNfseV3(string arg0, string arg1)
		{
			return Channel.ConsultarNfseV3(arg0, arg1);
		}

		public string ConsultarSituacaoLoteRps(string arg0)
		{
			return Channel.ConsultarSituacaoLoteRps(arg0);
		}

		public string ConsultarSituacaoLoteRpsV3(string arg0, string arg1)
		{
			return Channel.ConsultarSituacaoLoteRpsV3(arg0, arg1);
		}

		public string RecepcionarLoteRps(string arg0)
		{
			return Channel.RecepcionarLoteRps(arg0);
		}

		public string RecepcionarLoteRpsV3(string arg0, string arg1)
		{
			return Channel.RecepcionarLoteRpsV3(arg0, arg1);
		}

		#endregion Methods
	}
}