// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-06-2015
//
// Last Modified By : RFTD
// Last Modified On : 01-06-2015
// ***********************************************************************
// <copyright file="Contato.cs" company="ACBr.Net">
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
using ACBr.Net.Core.Generics;

namespace ACBr.Net.NFSe.Nota
{
	/// <summary>
	/// Classe Contato. Está classe não pode ser herdada.
	/// </summary>
	public sealed class DadosContato : GenericClone<DadosContato>
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DadosContato"/> class.
		/// </summary>
		internal DadosContato()
		{
			Telefone = string.Empty;
			Email = string.Empty;
		}

		#endregion Constructor

		#region Propriedades

		/// <summary>
		/// Gets or sets the DDD.
		/// </summary>
		/// <value>The DDD.</value>
		public string DDD { get; set; }

		/// <summary>
		/// Gets or sets the telefone.
		/// </summary>
		/// <value>The telefone.</value>
		public string Telefone { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public string Email { get; set; }

		#endregion Propriedades
	}
}