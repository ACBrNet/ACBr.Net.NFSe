﻿// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="ACBr.Net">
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

using ACBr.Net.Core.Extensions;
using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers
{
    public class SigissWebServiceClient : NFSeRestServiceClient, IServiceClient
    {
        private TipoUrl _tipoUrl;
        public SigissWebServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
            _tipoUrl = tipoUrl;
        }

        #region Methods

        #region nao implementados
        public string Enviar(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }
        #endregion nao implementados

        public string EnviarSincrono(string cabec, string msg)
        {
            return Execute("/nfes", msg, "POST");
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            var xml = XDocument.Parse(msg);
            var numerorps = xml.Root?.ElementAnyNs("NumeroRPS")?.GetValue<string>();
            var serierps = xml.Root?.ElementAnyNs("SerieRPS")?.GetValue<string>();
            return Execute($"/nfes/pegaxml/{numerorps}/serierps/{serierps}");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var xml = XDocument.Parse(msg);
            var numeronf = xml.Root?.ElementAnyNs("NumeroNFSe")?.GetValue<string>();
            var serie = xml.Root?.ElementAnyNs("SerieNFSe")?.GetValue<string>();
            var motivo = xml.Root?.ElementAnyNs("Motivo")?.GetValue<string>();
            return Execute($"/nfes/cancela/{numeronf}/serie/{serie}/motivo/{motivo}");
        }

        protected override WebRequest CreateWebRequest(string action, string method)
        {
            string autenticacao = string.Empty;
            string json = "{ \"login\": \"" + Provider.Configuracoes.WebServices.Usuario + "\"  , \"senha\":\"" + Provider.Configuracoes.WebServices.Senha + "\"}";
            var webrequest = WebRequest.Create(Provider.GetUrl(TipoUrl.Autenticacao) + "/login");
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json; charset=utf-8";

            using (var streamWriter = new StreamWriter(webrequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            using (var response = webrequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    autenticacao = reader.ReadToEnd();
                }
                response.Close();
            }

            var url = Provider.GetUrl(_tipoUrl);
            if (!url.EndsWith("/"))
                url += "/";

            webrequest = WebRequest.Create(url + action);
            webrequest.Headers.Add("AUTHORIZATION", autenticacao);
            webrequest.Method = method;
            webrequest.ContentType = "application/xml";

            return webrequest;
        }

        public void Dispose()
        {

        }

        #endregion Methods

    }
}
