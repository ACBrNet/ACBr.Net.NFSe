// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rodolfo Duarte
// Created          : 05-15-2017
//
// Last Modified By : RFTD
// Last Modified On : 05-15-2017
// ***********************************************************************
// <copyright file="ISaoPauloServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [ServiceContract(Namespace = "http://www.prefeitura.sp.gov.br/nfe", ConfigurationName = "Providers.SaoPaulo.LoteNFeSoap")]
    internal interface ISaoPauloServiceClient
    {
        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS", ReplyAction = "*")]
        EnvioRPSResponse EnvioRPS(EnvioRPSRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS", ReplyAction = "*")]
        EnvioLoteRPSResponse EnvioLoteRPS(EnvioLoteRPSRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/testeenvio", ReplyAction = "*")]
        TesteEnvioLoteRPSResponse TesteEnvioLoteRPS(TesteEnvioLoteRPSRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe", ReplyAction = "*")]
        CancelamentoNFeResponse CancelamentoNFe(CancelamentoNFeRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe", ReplyAction = "*")]
        ConsultaNFeResponse ConsultaNFe(ConsultaNFeRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeRecebidas", ReplyAction = "*")]
        ConsultaNFeRecebidasResponse ConsultaNFeRecebidas(ConsultaNFeRecebidasRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeEmitidas", ReplyAction = "*")]
        ConsultaNFeEmitidasResponse ConsultaNFeEmitidas(ConsultaNFeEmitidasRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote", ReplyAction = "*")]
        ConsultaLoteResponse ConsultaLote(ConsultaLoteRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote", ReplyAction = "*")]
        ConsultaInformacoesLoteResponse ConsultaInformacoesLote(ConsultaInformacoesLoteRequest request);

        [OperationContract(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaCNPJ", ReplyAction = "*")]
        ConsultaCNPJResponse ConsultaCNPJ(ConsultaCNPJRequest request);
    }
}