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

using ACBr.Net.NFSe.Nota;
using System;
using System.Collections.Generic;

#if NETFULL

using System.Runtime.InteropServices;

#endif

namespace ACBr.Net.NFSe.Providers
{
#if NETFULL

    [ComVisible(true)]
    [ProgId(nameof(RetornoWebservice))]
    [ClassInterface(ClassInterfaceType.AutoDual)]
#endif

    public class RetornoWebservice
    {
        #region Propriedades

        /// <summary>
        /// Informa se a comunicação ocorreu com sucesso ou não.
        /// </summary>
        /// <value><c>true</c> se não teve erro, senão <c>false</c>.</value>
        public bool Sucesso { get; set; } = false;

        public string Situacao { get; set; } = "";

        public string Protocolo { get; set; } = "";

        public string NumeroLote { get; set; } = "";

        public string NumeroUltimoRps { get; set; } = "";

        public DateTime DataLote { get; set; } = DateTime.Now;

        public int ProximaPagina { get; set; } = 0;

        public bool Assincrono { get; set; } = false;

#if NETFULL

        [ComVisible(false)]
#endif
        public List<NotaFiscal> NotasFiscais { get; } = new List<NotaFiscal>();

#if NETFULL

        [ComVisible(false)]
#endif
        public List<Evento> Alertas { get; } = new List<Evento>();

#if NETFULL

        [ComVisible(false)]
#endif
        public List<Evento> Erros { get; } = new List<Evento>();

        public string XmlEnvio { get; set; } = "";

        public string XmlRetorno { get; set; } = "";

        public string EnvelopeEnvio { get; set; } = "";

        public string EnvelopeRetorno { get; set; } = "";

        #endregion Propriedades
    }
}