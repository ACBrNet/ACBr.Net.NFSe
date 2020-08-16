// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 06-17-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-17-2016
// ***********************************************************************
// <copyright file="RetornoWebservice.cs" company="ACBr.Net">
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
using System.Collections.Generic;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    public abstract class RetornoWebservice
    {
        #region Propriedades

        /// <summary>
        /// Informa se a comunicação ocorreu com sucesso ou não.
        /// </summary>
        /// <value><c>true</c> se não teve erro, senão <c>false</c>.</value>
        public bool Sucesso { get; set; }

        public List<Evento> Alertas { get; } = new List<Evento>();

        public List<Evento> Erros { get; } = new List<Evento>();

        public string XmlEnvio { get; set; } = "";

        public string XmlRetorno { get; set; } = "";

        public string EnvelopeEnvio { get; set; } = "";

        public string EnvelopeRetorno { get; set; } = "";

        #endregion Propriedades
    }

    public sealed class RetornoEnviar : RetornoWebservice
    {
        public string NumeroLote { get; set; }

        public DateTime Data { get; set; }

        public string Protocolo { get; set; }

        public bool Sincrono { get; set; }
    }

    public sealed class RetornoConsultarSituacao : RetornoWebservice
    {
        public int NumeroLote { get; set; }

        public string Situacao { get; set; }
    }

    public sealed class RetornoConsultarLoteRps : RetornoWebservice
    {
        public int Lote { get; set; }

        public string Protocolo { get; set; }

        public string Situacao { get; set; }

        public NotaServico[] Notas { get; set; }
    }

    public sealed class RetornoConsultarNFSeRps : RetornoWebservice
    {
        public int NumeroRps { get; set; }

        public string Serie { get; set; }

        public TipoRps Tipo { get; set; }

        public NotaServico Nota { get; set; }
    }

    public sealed class RetornoConsultarNFSe : RetornoWebservice
    {
        public DateTime? Inicio { get; set; }

        public DateTime? Fim { get; set; }

        public int NumeroNFse { get; set; }

        public int Pagina { get; set; }

        public int ProximaPagina { get; set; }

        public string CNPJTomador { get; set; }

        public string IMTomador { get; set; }

        public string NomeIntermediario { get; set; }

        public string CNPJIntermediario { get; set; }

        public string IMIntermediario { get; set; }

        public NotaServico[] Notas { get; set; }
    }

    public sealed class RetornoConsultarSequencialRps : RetornoWebservice
    {
        public string Serie { get; set; }

        public int UltimoNumero { get; set; }
    }

    public sealed class RetornoCancelar : RetornoWebservice
    {
        public DateTime Data { get; set; }

        public string NumeroNFSe { get; set; }

        public string CodigoCancelamento { get; set; }

        public string Motivo { get; set; }
    }

    public sealed class RetornoCancelarNFSeLote : RetornoWebservice
    {
        public int Lote { get; set; }
    }

    public sealed class RetornoSubstituirNFSe : RetornoWebservice
    {
        public string CodigoCancelamento { get; set; }

        public string NumeroNFSe { get; set; }

        public string Motivo { get; set; }

        public NotaServico Nota { get; set; }
    }
}