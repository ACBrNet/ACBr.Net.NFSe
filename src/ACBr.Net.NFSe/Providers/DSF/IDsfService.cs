// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 10-08-2014
//
// Last Modified By : RFTD
// Last Modified On : 08-17-2016
// ***********************************************************************
// <copyright file="IDSFService.cs" company="ACBr.Net">
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

using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.DSF
{
	[ServiceContract(Namespace = "")]
	internal interface IDSFService
	{
		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		ConsultarSequencialRpsResponse consultarSequencialRps(ConsultarSequencialRpsRequest request);

		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		EnviarSincronoResponse enviarSincrono(EnviarSincronoRequest request);

		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		EnviarResponse enviar(EnviarRequest request);

		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		ConsultarLoteResponse consultarLote(ConsultarLoteRequest request);

		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		ConsultarNotaResponse consultarNota(ConsultarNotaRequest request);

		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		CancelarResponse cancelar(CancelarRequest request);

		[OperationContract(Action = "", ReplyAction = "*")]
		[XmlSerializerFormat(Style = OperationFormatStyle.Rpc, SupportFaults = true, Use = OperationFormatUse.Encoded)]
		// ReSharper disable once InconsistentNaming
		ConsultarNFSeRpsResponse consultarNFSeRps(ConsultarNFSeRpsRequest request);
	}
}