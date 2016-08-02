// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="CfgWebServices.cs" company="ACBr.Net">
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

using ACBr.Net.DFe.Core.Common;
using System.ComponentModel;

namespace ACBr.Net.NFSe.Configuracao
{
	/// <summary>
	/// Classe de configuração dos webservices
	/// </summary>
	public sealed class CfgWebServices : DFeWebServicesBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgWebServices"/> class.
		/// </summary>
		internal CfgWebServices()
		{
			Ambiente = TipoAmbiente.Homologacao;
			Visualizar = false;
			AjustaAguardaConsultaRet = false;
			AguardarConsultaRet = 1;
			Tentativas = 3;
			ProxyHost = string.Empty;
			ProxyPass = string.Empty;
			ProxyPort = string.Empty;
			ProxyUser = string.Empty;
		}

		#endregion Constructor

		#region Properties

		/// <summary>
		/// Uf do webservice em uso
		/// </summary>
		/// <value>The uf.</value>
		[Browsable(true)]
		public string Municipio { get; set; }

		/// <summary>
		/// Codigo do municipio do Webservices em uso
		/// </summary>
		/// <value>The uf codigo.</value>
		[Browsable(true)]
		public int CodMunicipio { get; set; }

		#endregion Properties
	}
}