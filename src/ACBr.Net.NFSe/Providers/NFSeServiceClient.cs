// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : Rafael Dias
// Created          : 07-11-2018
//
// Last Modified By : Rafael Dias
// Last Modified On : 09-11-2018
// ***********************************************************************
// <copyright file="NFSeServiceClient.cs" company="ACBr.Net">
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ACBr.Net.DFe.Core.Service;

namespace ACBr.Net.NFSe.Providers
{
    public abstract class NFSeServiceClient<T> : DFeServiceClientBase<T> where T : class
    {
        #region Fields

        protected readonly object serviceLock;

        #endregion Fields

        #region Constructors

        protected NFSeServiceClient(ProviderBase provider, TipoUrl tipoUrl) : this(provider, tipoUrl, provider.Certificado)
        {
        }

        protected NFSeServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado) :
            base(provider.GetUrl(tipoUrl), provider.TimeOut, certificado)
        {
            serviceLock = new object();
            Provider = provider;

            switch (tipoUrl)
            {
                case TipoUrl.Enviar:
                    PrefixoEnvio = "lot";
                    PrefixoResposta = "lot";
                    break;

                case TipoUrl.EnviarSincrono:
                    PrefixoEnvio = "lot-sinc";
                    PrefixoResposta = "lot-sinc";
                    break;

                case TipoUrl.ConsultarSituacao:
                    PrefixoEnvio = "env-sit-lot";
                    PrefixoResposta = "rec-sit-lot";
                    break;

                case TipoUrl.ConsultarLoteRps:
                    PrefixoEnvio = "con-lot";
                    PrefixoResposta = "con-lot";
                    break;

                case TipoUrl.ConsultarSequencialRps:
                    PrefixoEnvio = "seq-rps";
                    PrefixoResposta = "seq-rps";
                    break;

                case TipoUrl.ConsultaNFSeRps:
                    PrefixoEnvio = "con-rps-nfse";
                    PrefixoResposta = "con-rps-nfse";
                    break;

                case TipoUrl.ConsultaNFSe:
                    PrefixoEnvio = "con-nfse";
                    PrefixoResposta = "con-nfse";
                    break;

                case TipoUrl.CancelaNFSe:
                    PrefixoEnvio = "canc-nfse";
                    PrefixoResposta = "canc-nfse";
                    break;

                case TipoUrl.SubstituirNFSe:
                    PrefixoEnvio = "sub-nfse";
                    PrefixoResposta = "sub-nfse";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipoUrl), tipoUrl, null);
            }
        }

        #endregion Constructors

        #region Properties

        public ProviderBase Provider { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string PrefixoEnvio { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public string PrefixoResposta { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public string EnvelopeEnvio { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        public string EnvelopeRetorno { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Salvar o arquivo xml no disco de acordo com as propriedades.
        /// </summary>
        /// <param name="conteudoArquivo"></param>
        /// <param name="nomeArquivo"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void GravarSoap(string conteudoArquivo, string nomeArquivo)
        {
            if (Provider.Configuracoes.WebServices.Salvar == false) return;

            var path = Provider.Configuracoes.Arquivos.GetPathSoap(DateTime.Now, Provider.Configuracoes.PrestadorPadrao.CpfCnpj);
            nomeArquivo = Path.Combine(path, nomeArquivo);
            File.WriteAllText(nomeArquivo, conteudoArquivo, Encoding.UTF8);
        }

        /// <inheritdoc />
        protected override void BeforeSendDFeRequest(string message)
        {
            EnvelopeEnvio = message;
            GravarSoap(EnvelopeEnvio, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoEnvio}_soap_env.xml");
        }

        /// <inheritdoc />
        protected override void AfterReceiveDFeReply(string message)
        {
            EnvelopeRetorno = message;
            GravarSoap(EnvelopeRetorno, $"{DateTime.Now:yyyyMMddssfff}_{PrefixoResposta}_soap_ret.xml");
        }

        #endregion Methods
    }
}