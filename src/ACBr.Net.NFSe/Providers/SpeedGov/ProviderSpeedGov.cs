// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 07-30-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 07-30-2021
// ***********************************************************************
// <copyright file="ProviderBetha2.cs" company="ACBr.Net">
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

using ACBr.Net.NFSe.Configuracao;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class ProviderSpeedGov : ProviderABRASF202
    {
        #region Constructors

        public ProviderSpeedGov(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "SpeedGov";
        }

        #endregion Constructors

        #region Methods

        #region Protected Methods

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new SpeedGovServiceClient(this, tipo);
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar: return "enviar_lote_rps_envio_v1.xsd";
                case TipoUrl.ConsultarSituacao: return "consultar_situacao_lote_rps_envio_v1.xsd";
                case TipoUrl.ConsultarLoteRps: return "consultar_lote_rps_envio_v1.xsd";
                case TipoUrl.ConsultarNFSeRps: return "consultar_nfse_rps_envio_v1.xsd";
                case TipoUrl.ConsultarNFSe: return "consultar_nfse_envio_v1.xsd";
                case TipoUrl.CancelarNFSe: return "cancelar_nfse_envio_v1.xsd";
                default: throw new System.ArgumentOutOfRangeException(nameof(tipo), tipo, @"Valor incorreto ou serviço não suportado.");
            }
        }

        protected override string GetNamespace()
        {
            return string.Empty;
        }

        #endregion Protected Methods

        #endregion Methods
    }
}