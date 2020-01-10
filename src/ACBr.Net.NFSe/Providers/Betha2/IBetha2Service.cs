// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 07-30-2017
//
// Last Modified By : RFTD
// Last Modified On : 07-30-2017
// ***********************************************************************
// <copyright file="IBetha2Service.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers.Betha2
{
    [ServiceContract(Namespace = "http://www.betha.com.br/e-nota-contribuinte-ws")]
    internal interface IBetha2Service
    {
        [OperationContract(Action = "CancelarNfseEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        CancelarNfseResponse CancelarNfse(CancelarNfseRequest request);

        [OperationContract(Action = "ConsultarLoteRpsEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        ConsultarLoteRpsResponse ConsultarLoteRps(ConsultarLoteRpsRequest request);

        [OperationContract(Action = "ConsultarNfseFaixaEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        ConsultarNfseFaixaResponse ConsultarNfseFaixa(ConsultarNfseFaixaRequest request);

        [OperationContract(Action = "ConsultarNfseRpsEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        ConsultarNfsePorRpsResponse ConsultarNfsePorRps(ConsultarNfsePorRpsRequest request);

        [OperationContract(Action = "ConsultarNfseServicoPrestadoEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        ConsultarNfseServicoPrestadoResponse ConsultarNfseServicoPrestado(ConsultarNfseServicoPrestadoRequest request);

        [OperationContract(Action = "ConsultarNfseServicoTomadoEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        ConsultarNfseServicoTomadoResponse ConsultarNfseServicoTomado(ConsultarNfseServicoTomadoRequest request);

        [OperationContract(Action = "GerarNfseEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        GerarNfseResponse GerarNfse(GerarNfseRequest request);

        [OperationContract(Action = "RecepcionarLoteRpsEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        RecepcionarLoteRpsResponse RecepcionarLoteRps(RecepcionarLoteRpsRequest request);

        [OperationContract(Action = "RecepcionarLoteRpsSincronoEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        RecepcionarLoteRpsSincronoResponse RecepcionarLoteRpsSincrono(RecepcionarLoteRpsSincronoRequest request);

        [OperationContract(Action = "SubstituirNfseEnvio", ReplyAction = "*")]
        [XmlSerializerFormat(SupportFaults = true)]
        SubstituirNfseResponse SubstituirNfse(SubstituirNfseRequest request);
    }
}