// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-22-2018
//
// Last Modified By : RFTD
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="IBethaService.cs" company="ACBr.Net">
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
using System.Xml;

namespace ACBr.Net.NFSe.Providers.Betha
{
    [ServiceContract(Namespace = "http://www.betha.com.br/e-nota-contribuinte-ws")]
    internal interface IBethaService
    {
        [XmlSerializerFormat(SupportFaults = true)]
        [OperationContract(Action = "", ReplyAction = "*")]
        EnviarLoteRpsEnvioResponse EnviarLoteRpsEnvio(EnviarLoteRpsEnvioRequest request);
    }

    [MessageContract(IsWrapped = false)]
    internal sealed class EnviarLoteRpsEnvioRequest
    {
        #region Constructors

        public EnviarLoteRpsEnvioRequest()
        {
        }

        public EnviarLoteRpsEnvioRequest(XmlNode request)
        {
            this.Request = request;
        }

        #endregion Constructors

        #region Properties

        [MessageBodyMember(Name = "EnviarLoteRpsEnvio", Namespace = "http://www.betha.com.br/e-nota-contribuinte-ws", Order = 0)]
        public XmlNode Request { get; set; }

        #endregion Properties
    }

    [MessageContract(IsWrapped = false)]
    internal sealed class EnviarLoteRpsEnvioResponse
    {
        #region Constructors

        public EnviarLoteRpsEnvioResponse()
        {
        }

        public EnviarLoteRpsEnvioResponse(string result)
        {
            this.Result = result;
        }

        #endregion Constructors

        #region Properties

        [MessageBodyMember(Name = "EnviarLoteRpsEnvioResponse", Namespace = "http://www.betha.com.br/e-nota-contribuinte-ws", Order = 0)]
        public string Result { get; set; }

        #endregion Properties
    }
}