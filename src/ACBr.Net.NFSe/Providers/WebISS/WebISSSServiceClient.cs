// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-13-2017
//
// Last Modified By : RFTD
// Last Modified On : 01-13-2017
// ***********************************************************************
// <copyright file="WebISSSServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.WebISS
{
	// ReSharper disable once InconsistentNaming
	internal sealed class WebISSSServiceClient : DFeServiceClientBase<IWebISSServiceClient>, IWebISSServiceClient, IABRASFClient
	{
		#region Constructors

		public WebISSSServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
		{
		}

		#endregion Constructors

		#region Methods

		public string RecepcionarLoteRps(string cabec, string msg)
		{
			return ((IWebISSServiceClient)this).RecepcionarLoteRps(cabec, msg);
		}

		public string ConsultarSituacaoLoteRps(string cabec, string msg)
		{
			return ((IWebISSServiceClient)this).ConsultarSituacaoLoteRps(cabec, msg);
		}

		public string ConsultarNfsePorRps(string cabec, string msg)
		{
			return ((IWebISSServiceClient)this).ConsultarNfsePorRps(cabec, msg);
		}

		public string ConsultarNfse(string cabec, string msg)
		{
			return ((IWebISSServiceClient)this).ConsultarNfse(cabec, msg);
		}

		public string ConsultarLoteRps(string cabec, string msg)
		{
			return ((IWebISSServiceClient)this).ConsultarLoteRps(cabec, msg);
		}

		public string CancelarNfse(string cabec, string msg)
		{
			return ((IWebISSServiceClient)this).CancelarNfse(cabec, msg);
		}

		public string GerarNfse(string cabec, string msg)
		{
			throw new NotImplementedException();
		}

		#region Interface Methods

		string IWebISSServiceClient.RecepcionarLoteRps(string cabec, string msg)
		{
			return Channel.RecepcionarLoteRps(cabec, msg);
		}

		string IWebISSServiceClient.ConsultarSituacaoLoteRps(string cabec, string msg)
		{
			return Channel.ConsultarSituacaoLoteRps(cabec, msg);
		}

		string IWebISSServiceClient.ConsultarNfsePorRps(string cabec, string msg)
		{
			return Channel.ConsultarNfsePorRps(cabec, msg);
		}

		string IWebISSServiceClient.ConsultarNfse(string cabec, string msg)
		{
			return Channel.ConsultarNfse(cabec, msg);
		}

		string IWebISSServiceClient.ConsultarLoteRps(string cabec, string msg)
		{
			return Channel.ConsultarLoteRps(cabec, msg);
		}

		string IWebISSServiceClient.CancelarNfse(string cabec, string msg)
		{
			return Channel.CancelarNfse(cabec, msg);
		}

		#endregion Interface Methods

		#endregion Methods
	}
}