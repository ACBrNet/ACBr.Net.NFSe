// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 06-17-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-17-2016
// ***********************************************************************
// <copyright file="RetornoWebService.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers
{
	public class RetornoWebService
	{
		#region Constructor

		internal RetornoWebService()
		{
			DataEnvioLote = DateTime.Now;
			NotasFiscais = new List<NotaFiscal>();
			Erros = new List<Evento>();
			Alertas = new List<Evento>();
		}

		#endregion Constructor

		#region Propriedades

		public int CodCidade { get; set; }

		public bool Sucesso { get; set; }

		public string Situacao { get; set; }

		public string NumeroLote { get; set; }

		public string NumeroUltimoRps { get; set; }

		public string CPFCNPJRemetente { get; set; }

		public DateTime DataEnvioLote { get; set; }

		public long Versao { get; set; }

		public bool Assincrono { get; set; }

		public List<NotaFiscal> NotasFiscais { get; }

		public List<Evento> Alertas { get; }

		public List<Evento> Erros { get; }

		public string XmlEnvio { get; set; }

		public string XmlRetorno { get; set; }

		#endregion Propriedades
	}
}