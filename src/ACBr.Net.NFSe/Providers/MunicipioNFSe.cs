// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 06-19-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-28-2016
// ***********************************************************************
// <copyright file="MunicipioNFSe.cs" company="ACBr.Net">
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

namespace ACBr.Net.NFSe.Providers
{
	/// <summary>
	/// Class MunicipioNFSe. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class MunicipioNFSe
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MunicipioNFSe"/> class.
		/// </summary>
		public MunicipioNFSe()
		{
			UrlHomologacao = new Dictionary<TipoUrl, string>(9)
			{
				{ TipoUrl.Enviar, string.Empty },
				{ TipoUrl.EnviarSincrono, string.Empty },
				{ TipoUrl.CancelaNFSe, string.Empty },
				{ TipoUrl.ConsultaNFSe, string.Empty },
				{ TipoUrl.ConsultaNFSeRps, string.Empty },
				{ TipoUrl.ConsultarLoteRps, string.Empty },
				{ TipoUrl.ConsultarSituacao, string.Empty },
				{ TipoUrl.ConsultarSequencialRps, string.Empty },
				{ TipoUrl.SubstituirNFSe, string.Empty}
			};

			UrlProducao = new Dictionary<TipoUrl, string>(9)
			{
				{ TipoUrl.Enviar, string.Empty },
				{ TipoUrl.EnviarSincrono, string.Empty },
				{ TipoUrl.CancelaNFSe, string.Empty },
				{ TipoUrl.ConsultaNFSe, string.Empty },
				{ TipoUrl.ConsultaNFSeRps, string.Empty },
				{ TipoUrl.ConsultarLoteRps, string.Empty },
				{ TipoUrl.ConsultarSituacao, string.Empty },
				{ TipoUrl.ConsultarSequencialRps, string.Empty },
				{ TipoUrl.SubstituirNFSe, string.Empty }
			};
		}

		#endregion Constructors

		#region Propriedades

		/// <summary>
		/// Define ou retorna o codigo IBGE do municipio
		/// </summary>
		/// <value>The codigo.</value>
		public int Codigo { get; set; }

		/// <summary>
		/// Define ou retorna o codigo Siafi do municipio
		/// Obrigatorio para municipios com provedor DSF/ISSDSF
		/// </summary>
		/// <value>The codigo siafi.</value>
		public int CodigoSiafi { get; set; }

		/// <summary>
		/// Define ou retorna o nome do municipio
		/// </summary>
		/// <value>The nome.</value>
		public string Nome { get; set; }

		/// <summary>
		/// Define ou retorna a UF do municipio
		/// </summary>
		/// <value>The uf.</value>
		public string UF { get; set; }

		/// <summary>
		/// Define ou retorna o nome do provedor
		/// </summary>
		/// <value>The provedor.</value>
		public string Provedor { get; set; }

		/// <summary>
		/// Define ou retorna o tamanho da inscrição municipal
		/// Para validação em alguns provedores
		/// </summary>
		/// <value>The tamanho im.</value>
		public int TamanhoIM { get; set; }

		/// <summary>
		/// Lista de url de homologação dos serviços.
		/// </summary>
		/// <value>The URL homologacao.</value>
		public Dictionary<TipoUrl, string> UrlHomologacao { get; }

		/// <summary>
		/// Lista de url de produção dos serviços.
		/// </summary>
		/// <value>The URL producao.</value>
		public Dictionary<TipoUrl, string> UrlProducao { get; }

		#endregion Propriedades
	}
}