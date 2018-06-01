// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-22-2018
//
// Last Modified By : RFTD
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="ProviderBetha.cs" company="ACBr.Net">
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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers.Betha
{
    internal sealed class ProviderBetha : ProviderABRASF
    {
        #region Constructors

        public ProviderBetha(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Betha";
        }

        #endregion Constructors

        #region Methods

        public override RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            var url = GetUrl(tipo);
            return new BethaServiceClient(url, TimeOut, Certificado);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.betha.com.br/e-nota-contribuinte-ws\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar: return "servico_enviar_lote_rps_envio_v01.xsd";
                case TipoUrl.ConsultarSituacao: return "servico_consultar_situacao_lote_rps_envio_v01.xsd";
                case TipoUrl.ConsultarLoteRps: return "servico_consultar_lote_rps_envio_v01.xsd";
                case TipoUrl.ConsultaNFSeRps: return "servico_consultar_nfse_rps_envio_v01.xsd";
                case TipoUrl.ConsultaNFSe: return "servico_consultar_nfse_envio_v01.xsd";
                case TipoUrl.CancelaNFSe: return "servico_cancelar_nfse_envio_v01.xsd";
                default: throw new ArgumentOutOfRangeException(nameof(tipo), tipo, @"Valor incorreto ou serviço não suportado.");
            }
        }

        #endregion Methods
    }
}