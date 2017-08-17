// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 08-06-2017
// ***********************************************************************
// <copyright file="ACBrDANFSeBase.cs" company="ACBr.Net">
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
using ACBr.Net.Core;

namespace ACBr.Net.NFSe
{
    /// <summary>
    /// Classe base para impressão de DANFSe
    /// </summary>
    public abstract class ACBrDANFSeBase : ACBrComponent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [Browsable(false)]
        public ACBrNFSe Parent { get; internal set; }

        /// <summary>
        /// Gets or sets the logo empresa.
        /// </summary>
        /// <value>The logo empresa.</value>
        public string LogoEmpresa { get; set; }

        /// <summary>
        /// Gets or sets the logo empresa.
        /// </summary>
        /// <value>The logo empresa.</value>
        public string LogoPrefeitura { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Imprime as NFSe/RPS.
        /// </summary>
        public abstract void Imprimir();

        /// <summary>
        /// Imprimirs the PDF.
        /// </summary>
        public abstract void ImprimirPDF();

        #endregion Methods
    }
}