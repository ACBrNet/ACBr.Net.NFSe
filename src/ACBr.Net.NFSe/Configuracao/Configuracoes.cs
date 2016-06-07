// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="Configuracoes.cs" company="ACBr.Net">
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

using System.ComponentModel;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Configuracao
{
    /// <summary>
    /// Class Configuracoes. This class cannot be inherited.
    /// </summary>
    public sealed class Configuracoes
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuracoes"/> class.
        /// </summary>
		internal Configuracoes()
		{
			Geral = new CfgGeral();
			Arquivos = new CfgArquivos(this);
			Certificados = new CfgCertificados();
			WebServices = new CfgWebServices();
			PrestadoPadrao = new DadosPrestador();
		}

		#endregion Constructor

		#region Properties

        /// <summary>
        /// Gets the geral.
        /// </summary>
        /// <value>The geral.</value>
		[Browsable(true)]
		public CfgGeral Geral { get; }

        /// <summary>
        /// Gets the arquivos.
        /// </summary>
        /// <value>The arquivos.</value>
		[Browsable(true)]
		public CfgArquivos Arquivos { get;}

        /// <summary>
        /// Gets the certificados.
        /// </summary>
        /// <value>The certificados.</value>
		[Browsable(true)]
		public CfgCertificados Certificados { get; }

        /// <summary>
        /// Gets the web services.
        /// </summary>
        /// <value>The web services.</value>
		[Browsable(true)]
		public CfgWebServices WebServices { get; }

        /// <summary>
        /// Gets the prestado padrão.
        /// </summary>
        /// <value>The prestado padrão.</value>
        public DadosPrestador PrestadoPadrao { get; set; }

        #endregion Properties

		#region Methods
		#endregion Methods
	}
}