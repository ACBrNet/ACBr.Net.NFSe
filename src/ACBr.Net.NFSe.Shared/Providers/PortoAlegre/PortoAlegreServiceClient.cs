// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-16-2017
//
// Last Modified By : RFTD
// Last Modified On : 01-16-2017
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

using System;
using System.Security.Cryptography.X509Certificates;
using ACBr.Net.DFe.Core.Service;

namespace ACBr.Net.NFSe.Providers.PortoAlegre
{
	internal sealed class PortoAlegreServiceClient : DFeServiceClientBase<IPortoAlegreServiceClient>, IABRASFClient, IPortoAlegreServiceClient
	{
		#region Constructors

		public PortoAlegreServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
		{
		}

		#endregion Constructors

		#region Methods

		public string CancelarNFSe(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new CancelarNfseRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).CancelarNfse(inValue);
			return retVal.outputXML;
		}

		public string ConsultarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new ConsultarLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).ConsultarLoteRps(inValue);
			return retVal.outputXML;
		}

		public string ConsultarNFSe(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new ConsultarNfseRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).ConsultarNfse(inValue);
			return retVal.outputXML;
		}

		public string ConsultarNfsePorFaixa(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new ConsultarNfsePorFaixaRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).ConsultarNfsePorFaixa(inValue);
			return retVal.outputXML;
		}

		public string ConsultarNFSePorRps(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new ConsultarNfsePorRpsRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).ConsultarNfsePorRps(inValue);
			return retVal.outputXML;
		}

		public string ConsultarSituacaoLoteRps(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new ConsultarSituacaoLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).ConsultarSituacaoLoteRps(inValue);
			return retVal.outputXML;
		}

		public string RecepcionarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new RecepcionarLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).RecepcionarLoteRps(inValue);
			return retVal.outputXML;
		}

		public string GerarNfse(string nfseCabecMsg, string nfseDadosMsg)
		{
			var inValue = new GerarNfseRequest(nfseCabecMsg, nfseDadosMsg);
			var retVal = ((IPortoAlegreServiceClient)this).GerarNfse(inValue);
			return retVal.outputXML;
		}

		#region Interface Methods

		CancelarNfseResponse IPortoAlegreServiceClient.CancelarNfse(CancelarNfseRequest request)
		{
			return Channel.CancelarNfse(request);
		}

		ConsultarLoteRpsResponse IPortoAlegreServiceClient.ConsultarLoteRps(ConsultarLoteRpsRequest request)
		{
			return Channel.ConsultarLoteRps(request);
		}

		ConsultarNfseResponse IPortoAlegreServiceClient.ConsultarNfse(ConsultarNfseRequest request)
		{
			return Channel.ConsultarNfse(request);
		}

		ConsultarNfsePorFaixaResponse IPortoAlegreServiceClient.ConsultarNfsePorFaixa(ConsultarNfsePorFaixaRequest request)
		{
			return Channel.ConsultarNfsePorFaixa(request);
		}

		ConsultarNfsePorRpsResponse IPortoAlegreServiceClient.ConsultarNfsePorRps(ConsultarNfsePorRpsRequest request)
		{
			return Channel.ConsultarNfsePorRps(request);
		}

		ConsultarSituacaoLoteRpsResponse IPortoAlegreServiceClient.ConsultarSituacaoLoteRps(ConsultarSituacaoLoteRpsRequest request)
		{
			return Channel.ConsultarSituacaoLoteRps(request);
		}

		RecepcionarLoteRpsResponse IPortoAlegreServiceClient.RecepcionarLoteRps(RecepcionarLoteRpsRequest request)
		{
			return Channel.RecepcionarLoteRps(request);
		}

		GerarNfseResponse IPortoAlegreServiceClient.GerarNfse(GerarNfseRequest request)
		{
			return Channel.GerarNfse(request);
		}

		#endregion Interface Methods

		#endregion Methods
	}
}