// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 10-01-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="Deducao.cs" company="ACBr.Net">
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
using PropertyChanged;

#region COM Interop Attributes

#if COM_INTEROP

using System.Runtime.InteropServices;

#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe.Nota
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("9BFC382F-1F94-42E9-8682-17334451F769")]
	[ClassInterface(ClassInterfaceType.None)]
#endif

	#endregion COM Interop Attributes

	[ImplementPropertyChanged]
	public sealed class Deducao : GenericClone<Deducao>
	{
		#region Constructors

		internal Deducao()
		{
		}

		#endregion Constructors

		#region Propriedades

		/// <summary>
		/// Gets or sets the deducao por.
		/// </summary>
		/// <value>The deducao por.</value>
		public DeducaoPor DeducaoPor { get; set; }

		/// <summary>
		/// Gets or sets the tipo deducao.
		/// </summary>
		/// <value>The tipo deducao.</value>
		public TipoDeducao TipoDeducao { get; set; }

		/// <summary>
		/// Gets or sets the CPFCNPJ referencia.
		/// </summary>
		/// <value>The CPFCNPJ referencia.</value>
		public string CPFCNPJReferencia { get; set; }

		/// <summary>
		/// Gets or sets the numero nf referencia.
		/// </summary>
		/// <value>The numero nf referencia.</value>
		public int? NumeroNFReferencia { get; set; }

		/// <summary>
		/// Gets or sets the valor total referencia.
		/// </summary>
		/// <value>The valor total referencia.</value>
		public decimal ValorTotalReferencia
		{
			#region COM_INTEROP

#if COM_INTEROP
			[return: MarshalAs(UnmanagedType.Currency)]
#endif

			#endregion COM_INTEROP

			get;
			set;
		}

		/// <summary>
		/// Gets or sets the percentual deduzir.
		/// </summary>
		/// <value>The percentual deduzir.</value>
		public decimal PercentualDeduzir
		{
			#region COM_INTEROP

#if COM_INTEROP
			[return: MarshalAs(UnmanagedType.Currency)]
#endif

			#endregion COM_INTEROP

			get;
			set;
		}

		/// <summary>
		/// Gets or sets the valor deduzir.
		/// </summary>
		/// <value>The valor deduzir.</value>
		public decimal ValorDeduzir
		{
			#region COM_INTEROP

#if COM_INTEROP
			[return: MarshalAs(UnmanagedType.Currency)]
#endif

			#endregion COM_INTEROP

			get;
			set;
		}

		#endregion Propriedades
	}
}