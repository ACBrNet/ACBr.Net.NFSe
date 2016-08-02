// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="CfgGeral.cs" company="ACBr.Net">
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
using System.IO;
using System.Reflection;

namespace ACBr.Net.NFSe.Configuracao
{
	/// <summary>
	/// Class CfgGeral. This class cannot be inherited.
	/// </summary>
	public sealed class CfgGeral : DFeGeralBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgGeral"/> class.
		/// </summary>
		internal CfgGeral()
		{
			Salvar = false;
			AtualizarXmlCancelado = false;
			PathSchemas = Path.Combine(Assembly.GetExecutingAssembly().CodeBase, "Schemas");
			PathSalvar = Path.Combine(Assembly.GetExecutingAssembly().CodeBase, "XmlNFSe");
			ExibirErroSchema = false;
			RetirarAcentos = false;
			FormatoAlerta = "TAG:%TAG% ID:%ID%/%TAG%(%DESCRICAO%) - %MSG%.";
		}

		#endregion Constructor

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether [atualizar XML cancelado].
		/// </summary>
		/// <value><c>true</c> if [atualizar XML cancelado]; otherwise, <c>false</c>.</value>
		[Browsable(true)]
		[DefaultValue(false)]
		public bool AtualizarXmlCancelado { get; set; }

		#endregion Properties
	}
}