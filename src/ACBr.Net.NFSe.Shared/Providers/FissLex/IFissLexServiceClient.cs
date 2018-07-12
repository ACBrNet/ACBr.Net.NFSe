// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 12-27-2017
//
// Last Modified By : RFTD
// Last Modified On : 12-27-2017
// ***********************************************************************
// <copyright file="ProviderFissLex.cs" company="ACBr.Net">
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

using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;

namespace ACBr.Net.NFSe.Providers.FissLex
{
    [ServiceContract(Namespace = "FISS-LEX")]
    internal interface IFissLexRecepcionarLoteServiceClient
    {
        [OperationContract(Name = "Execute", Action = "FISS-LEXaction/AWS_RECEPCIONARLOTERPS.Execute", ReplyAction = "*")]
        string RecepcionarLote(string request);
    }

    [ServiceContract(Namespace = "FISS-LEX")]
    internal interface IFissLexConsultarLoteRpsServiceClient
    {
        [OperationContract(Name = "Execute", Action = "FISS-LEXaction/AWS_CONSULTALOTERPS.Execute", ReplyAction = "*")]
        ExecuteResponse ConsultarLoteRps(ConsultarLoteRpsRequest request);
    }

    [MessageContract(WrapperName = "WS_ConsultaLoteRps.Execute", WrapperNamespace = "FISS-LEX", IsWrapped = true)]
    internal sealed class ConsultarLoteRpsRequest
    {
        public ConsultarLoteRpsRequest()
        {
        }

        public ConsultarLoteRpsRequest(XmlElement request)
        {
            Request = request;
        }

        [MessageBodyMember(Name = "Consultarloterpsenvio", Namespace = "FISS-LEX", Order = 0)]
        public XmlElement Request;
    }

    [MessageContract(WrapperName = "WS_ConsultaLoteRps.ExecuteResponse", WrapperNamespace = "FISS-LEX", IsWrapped = true)]
    internal sealed class ExecuteResponse
    {
        [MessageBodyMember(Name = "Consultarloterpsresposta", Namespace = "FISS-LEX", Order = 0)]
        public string Consultarloterpsresposta;

        [MessageBodyMember(Name = "Listamensagemretorno", Namespace = "FISS-LEX", Order = 1)]
        public XmlNode[] Listamensagemretorno;

        public ExecuteResponse()
        {
        }

        public ExecuteResponse(string Consultarloterpsresposta, XmlNode[] Listamensagemretorno)
        {
            this.Consultarloterpsresposta = Consultarloterpsresposta;
            this.Listamensagemretorno = Listamensagemretorno;
        }
    }
}