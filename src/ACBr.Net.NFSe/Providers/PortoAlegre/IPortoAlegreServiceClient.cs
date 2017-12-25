// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-16-2017
//
// Last Modified By : RFTD
// Last Modified On : 01-16-2017
// ***********************************************************************
// <copyright file="IPortoAlegreServiceClient.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.PortoAlegre
{
    [ServiceContract(Namespace = "http://ws.bhiss.pbh.gov.br")]
    internal interface IPortoAlegreServiceClient
    {
        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/CancelarNfse", ReplyAction = "*")]
        CancelarNfseResponse CancelarNfse(CancelarNfseRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarLoteRps", ReplyAction = "*")]
        ConsultarLoteRpsResponse ConsultarLoteRps(ConsultarLoteRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarNfse", ReplyAction = "*")]
        ConsultarNfseResponse ConsultarNfse(ConsultarNfseRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarNfsePorFaixa", ReplyAction = "*")]
        ConsultarNfsePorFaixaResponse ConsultarNfsePorFaixa(ConsultarNfsePorFaixaRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarNfsePorRps", ReplyAction = "*")]
        ConsultarNfsePorRpsResponse ConsultarNfsePorRps(ConsultarNfsePorRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarSituacaoLoteRps", ReplyAction = "*")]
        ConsultarSituacaoLoteRpsResponse ConsultarSituacaoLoteRps(ConsultarSituacaoLoteRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/RecepcionarLoteRps", ReplyAction = "*")]
        RecepcionarLoteRpsResponse RecepcionarLoteRps(RecepcionarLoteRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/GerarNfse", ReplyAction = "*")]
        GerarNfseResponse GerarNfse(GerarNfseRequest request);
    }
}