// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="CfgArquivos.cs" company="ACBr.Net">
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
using System;
using System.ComponentModel;
using System.IO;

#region COM Interop Attributes

#if COM_INTEROP

using System.Runtime.InteropServices;

#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe.Configuracao
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("B1BD7344-5878-41D0-BFEC-5220BE61AF0F")]
#endif

	#endregion COM Interop Attributes

	public sealed class CfgArquivos : DFeArquivosConfigBase
	{
		#region Constructor

		/// <summary>
		/// Inicializa uma nova instancia da classe <see cref="CfgArquivos"/>.
		/// </summary>
		internal CfgArquivos(ConfiguracoesNFSe parent)
		{
			Parent = parent;
			Salvar = false;
			AdicionarLiteral = false;
			EmissaoPathNFSe = false;
			PastaMensal = false;
			PathNFSe = string.Empty;
			PathLote = string.Empty;
			PathRps = string.Empty;
		}

		#endregion Constructor

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether [emissao path n fe].
		/// </summary>
		/// <value><c>true</c> if [emissao path n fe]; otherwise, <c>false</c>.</value>
		[Browsable(true)]
		[DefaultValue(false)]
		public bool EmissaoPathNFSe { get; set; }

		/// <summary>
		/// Gets or sets the path n fe.
		/// </summary>
		/// <value>The path n fe.</value>
		[Browsable(true)]
		public string PathNFSe { get; set; }

		/// <summary>
		/// Gets or sets the path lote.
		/// </summary>
		/// <value>The path lote.</value>
		[Browsable(true)]
		public string PathLote { get; set; }

		/// <summary>
		/// Gets or sets the path lote.
		/// </summary>
		/// <value>The path lote.</value>
		[Browsable(true)]
		public string PathRps { get; set; }

		private ConfiguracoesNFSe Parent { get; }

		#endregion Properties

		#region Methods

		public string GetPathNFSe(DateTime data)
		{
			var dir = string.IsNullOrEmpty(PathNFSe.Trim()) ? Parent.Geral.PathSalvar : PathNFSe;

			if (PastaMensal)
				dir = Path.Combine(dir, data.ToString("yyyyMM"));

			if (AdicionarLiteral && !dir.EndsWith("NFSe"))
				dir = Path.Combine(dir, "NFSe");

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			return dir;
		}

		public string GetPathLote()
		{
			var dir = string.IsNullOrEmpty(PathLote.Trim()) ? Parent.Geral.PathSalvar : PathLote;

			if (PastaMensal)
				dir = Path.Combine(dir, DateTime.Now.ToString("yyyyMM"));

			if (AdicionarLiteral && !dir.EndsWith("Lote"))
				dir = Path.Combine(dir, "Lote");

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			return dir;
		}

		public string GetPathRps(DateTime data)
		{
			var dir = string.IsNullOrEmpty(PathRps.Trim()) ? Parent.Geral.PathSalvar : PathRps;

			if (PastaMensal)
				dir = Path.Combine(dir, data.ToString("yyyyMM"));

			if (AdicionarLiteral && !dir.EndsWith("RPS"))
				dir = Path.Combine(dir, "RPS");

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			return dir;
		}

		#endregion Methods
	}
}