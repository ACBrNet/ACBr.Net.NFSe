// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-22-2018
//
// Last Modified By : RFTD
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="IGovDigitalService.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.GovDigital
{
    [ServiceContract(Namespace = "http://nfse.abrasf.org.br")]
    internal interface IGovDigitalService
    {
        [OperationContract(Action = "http://nfse.abrasf.org.br/ConsultarLoteRps", ReplyAction = "http://nfse.abrasf.org.br/nfse/ConsultarLoteRpsResponse")]
        ConsultarLoteRpsResponse ConsultarLoteRps(ConsultarLoteRpsRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/ConsultarNfseServicoPrestado", ReplyAction = "http://nfse.abrasf.org.br/nfse/ConsultarNfseServicoPrestadoResponse")]
        ConsultarNFSeServicoPrestadoResponse ConsultarNFSeServicoPrestado(ConsultarNFSeServicoPrestadoRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/ConsultarNfseServicoTomado", ReplyAction = "http://nfse.abrasf.org.br/nfse/ConsultarNfseServicoTomadoResponse")]
        ConsultarNFSeServicoTomadoResponse ConsultarNFSeServicoTomado(ConsultarNFSeServicoTomadoRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/ConsultarNfsePorFaixa", ReplyAction = "http://nfse.abrasf.org.br/nfse/ConsultarNfsePorFaixaResponse")]
        ConsultarNFSePorFaixaResponse ConsultarNFSePorFaixa(ConsultarNFSePorFaixaRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/ConsultarNfsePorRps", ReplyAction = "http://nfse.abrasf.org.br/nfse/ConsultarNfsePorRpsResponse")]
        ConsultarNFSePorRpsResponse ConsultarNFSePorRps(ConsultarNFSePorRpsRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/RecepcionarLoteRps", ReplyAction = "http://nfse.abrasf.org.br/nfse/RecepcionarLoteRpsResponse")]
        RecepcionarLoteRpsResponse RecepcionarLoteRps(RecepcionarLoteRpsRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/GerarNfse", ReplyAction = "http://nfse.abrasf.org.br/nfse/GerarNfseResponse")]
        GerarNFSeResponse GerarNFSe(GerarNFSeRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/SubstituirNfse", ReplyAction = "http://nfse.abrasf.org.br/nfse/SubstituirNfseResponse")]
        SubstituirNFSeResponse SubstituirNFSe(SubstituirNFSeRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/RecepcionarLoteRpsSincrono", ReplyAction = "http://nfse.abrasf.org.br/nfse/RecepcionarLoteRpsSincronoResponse")]
        RecepcionarLoteRpsSincronoResponse RecepcionarLoteRpsSincrono(RecepcionarLoteRpsSincronoRequest request);

        [OperationContract(Action = "http://nfse.abrasf.org.br/CancelarNfse", ReplyAction = "http://nfse.abrasf.org.br/nfse/CancelarNfseResponse")]
        CancelarNFSeResponse CancelarNFSe(CancelarNFSeRequest request);
    }
}