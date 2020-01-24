// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 12-26-2017
//
// Last Modified By : RFTD
// Last Modified On : 23-01-2020
// ***********************************************************************
// <copyright file="AbacoServiceClient.cs" company="ACBr.Net">
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
using System.IO;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Common;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class AbacoServiceClient : NFSeRequestServiceClient, IABRASFClient
    {
        #region Constructors

        public AbacoServiceClient(ProviderAbaco provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string RecepcionarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:RecepcionarLoteRPS.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:RecepcionarLoteRPS.Execute>");

            return Execute("http://www.e-nfs.com.braction/ARECEPCIONARLOTERPS.Execute", message.ToString(), "RecepcionarLoteRPS.ExecuteResponse");
        }

        public string ConsultarSituacaoLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarSituacaoLoteRPS.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarSituacaoLoteRPS.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARSITUACAOLOTERPS.Execute", message.ToString(), "ConsultarSituacaoLoteRPS.ExecuteResponse");
        }

        public string ConsultarNFSePorRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarNfsePorRps.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarNfsePorRps.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARNFSEPORRPS.Execute", message.ToString(), "ConsultarNfsePorRps.ExecuteResponse");
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarNfse.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarNfse.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARNFSE.Execute", message.ToString(), "ConsultarNfse.ExecuteResponse");
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:ConsultarLoteRps.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:ConsultarLoteRps.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACONSULTARLOTERPS.Execute", message.ToString(), "ConsultarLoteRps.ExecuteResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<e:CancelarNfse.Execute>");
            message.Append("<e:Nfsecabecmsg>");
            message.AppendCData(cabec);
            message.Append("</e:Nfsecabecmsg>");
            message.Append("<e:Nfsedadosmsg>");
            message.AppendCData(msg);
            message.Append("</e:Nfsedadosmsg>");
            message.Append("</e:CancelarNfse.Execute>");

            return Execute("http://www.e-nfs.com.braction/ACANCELARNFSE.Execute", message.ToString(), "CancelarNfse.ExecuteResponse");
        }

        public string GerarNfse(string nfseCabecMsg, string nfseDadosMsg)
        {
            throw new NotImplementedException();
        }

        private string Execute(string soapAction, string message, string responseTag)
        {
            var envelope = new StringBuilder();
            envelope.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:e=\"http://www.e-nfs.com.br\">");
            envelope.Append("<soapenv:Header/>");
            envelope.Append("<soapenv:Body>");
            envelope.Append(message);
            envelope.Append("</soapenv:Body>");
            envelope.Append("</soapenv:Envelope>");

            var msg = Message.CreateMessage(XmlReader.Create(new StringReader(envelope.ToString())), int.MaxValue, Endpoint.Binding.MessageVersion);

            RemoteCertificateValidationCallback validation = null;

            // o certificado do servidor de homologação é invalido, override na validação do certificado do servidor.
            if (Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao)
            {
                validation = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var ret = string.Empty;

            try
            {
                using (new OperationContextScope(InnerChannel))
                {
                    //Define a SOAPAction por ser SOAP 1.1
                    var requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers["SOAPAction"] = soapAction;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                    ret = Execute(msg);
                }
            }
            finally
            {
                if (Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao)
                    ServicePointManager.ServerCertificateValidationCallback = validation;
            }

            var xmlDocument = XDocument.Parse(ret);
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new ACBrDFeCommunicationException(exMessage);
            }

            return xmlDocument.ElementAnyNs(responseTag).ElementAnyNs("Outputxml").Value;
        }

        #endregion Methods
    }
}