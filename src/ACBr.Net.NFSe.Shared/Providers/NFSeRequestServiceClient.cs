// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 07-11-2018
//
// Last Modified By : RFTD
// Last Modified On : 09-11-2018
// ***********************************************************************
// <copyright file="NFSeRequestServiceClient.cs" company="ACBr.Net">
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

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Xml;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    public abstract class NFSeRequestServiceClient : NFSeServiceClient<IRequestChannel>
    {
        #region Constructors

        protected NFSeRequestServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado = null) :
            base(provider, tipoUrl, certificado)
        {
        }

        protected NFSeRequestServiceClient(ProviderBase provider, TipoUrl tipoUrl) :
            base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Executa uma requisição ao webservice.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="msg"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        protected string Execute(string action, string msg, params MessageHeader[] headers)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(msg);

            var wcfMsg = Message.CreateMessage(Endpoint.Binding.MessageVersion, action, xmlDoc.DocumentElement);
            if (headers.Any())
            {
                foreach (var header in headers)
                {
                    wcfMsg.Headers.Add(header);
                }
            }

            var ret = Channel.Request(wcfMsg);
            Guard.Against<ACBrDFeException>(ret == null, "Nenhum retorno do webservice.");
            var reader = ret.GetReaderAtBodyContents();
            return reader.ReadOuterXml();
        }

        /// <summary>
        /// Cria um Header para ser adicionado ao request.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameSpace"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        protected MessageHeader CreateHeader(string name, string nameSpace, string header)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(header);

            return MessageHeader.CreateHeader(name, nameSpace, xmlDoc.DocumentElement);
        }

        #endregion Methods
    }
}