// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
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

using System;
using System.ComponentModel;
using ACBr.Net.Core;
using ACBr.Net.Core.Logging;
using ACBr.Net.DFe.Core.Common;

#if !NETSTANDARD2_0

using System.Drawing;

#endif

namespace ACBr.Net.NFSe
{
    /// <summary>
    /// Classe base para impressão de DANFSe
    /// </summary>
    [TypeConverter(typeof(ACBrExpandableObjectConverter))]
    public abstract class ACBrDANFSeBase : DFeReportClass<ACBrNFSe>
    {
        #region Fields

        private ACBrNFSe parent;

        #endregion Fields

        #region Properties

#if NETFULL
        public Image LogoPrefeitura { get; set; }
#else
        public byte[] LogoPrefeitura { get; set; }
#endif

        public LayoutImpressao Layout { get; set; }

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

        #region Overrides

        /// <inheritdoc />
        protected override void ParentChanged(ACBrNFSe oldParent, ACBrNFSe newParent)
        {
            if (oldParent != null && oldParent.DANFSe == this) oldParent.DANFSe = null;
            if (newParent != null && newParent.DANFSe != this) newParent.DANFSe = this;
        }

        #endregion Overrides
    }
}