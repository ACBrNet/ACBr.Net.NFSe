// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-13-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-11-2018
// ***********************************************************************
// <copyright file="ProviderWebIss.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class ProviderWebIss : ProviderABRASF
    {
        #region Constructors

        public ProviderWebIss(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "WebISS";
        }

        #endregion Constructors

        #region Methods

        public override RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new WebIssServiceClient(this, tipo);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://tempuri.org/\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar: return "servico_enviar_lote_rps_envio.xsd";
                case TipoUrl.EnviarSincrono: return "servico_gerar_nfse_envio.xsd";
                case TipoUrl.ConsultarSituacao: return "servico_consultar_situacao_lote_rps_envio.xsd";
                case TipoUrl.ConsultarLoteRps: return "servico_consultar_lote_rps_envio.xsd";
                case TipoUrl.ConsultaNFSeRps: return "servico_consultar_nfse_rps_envio.xsd";
                case TipoUrl.ConsultaNFSe: return "servico_consultar_nfse_envio.xsd";
                case TipoUrl.CancelaNFSe: return "servico_cancelar_nfse_envio.xsd";
                default: throw new ArgumentOutOfRangeException(nameof(tipo), tipo, @"Valor incorreto ou serviço não suportado.");
            }
        }

        #endregion Methods
    }
}