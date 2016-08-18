// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 07-28-2016
//
// Last Modified By : RFTD
// Last Modified On : 07-28-2016
// ***********************************************************************
// <copyright file="IGinfesService.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[ServiceContract]
	internal interface IGinfesService
	{
		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string CancelarNfse(string arg0);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarLoteRps(string arg0);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarLoteRpsV3(string arg0, string arg1);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarNfse(string arg0);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarNfsePorRps(string arg0);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarNfsePorRpsV3(string arg0, string arg1);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarNfseV3(string arg0, string arg1);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarSituacaoLoteRps(string arg0);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string ConsultarSituacaoLoteRpsV3(string arg0, string arg1);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string RecepcionarLoteRps(string arg0);

		[OperationContract(Action = "", ReplyAction = "*")]
		[DataContractFormat(Style = OperationFormatStyle.Rpc)]
		[return: MessageParameter(Name = "return")]
		string RecepcionarLoteRpsV3(string arg0, string arg1);
	}
}