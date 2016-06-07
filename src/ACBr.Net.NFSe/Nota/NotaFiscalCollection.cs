// ***********************************************************************
// Assembly         : G2i.NFSe
// Author           : RFTD
// Created          : 10-01-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="NotaFiscalCollection.cs" company="ACBr.Net">
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

using ACBr.Net.DFe.Core.Collection;
using ACBr.Net.NFSe.Util;

namespace ACBr.Net.NFSe.Nota
{
    /// <summary>
    /// Classe NotaFiscalCollection. Está classe não pode ser herdada.
    /// </summary>
    public sealed class NotaFiscalCollection : DFeCollection<NotaFiscal>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotaFiscalCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal NotaFiscalCollection(ACBrNFSe parent)
        {
            Parent = parent;
        }

        #endregion Constructor

        #region Propriedades

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public ACBrNFSe Parent { get; }

        #endregion Propriedades

        #region Methods

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns>T.</returns>
        public override NotaFiscal AddNew()
        {
            var nota = new NotaFiscal();
			Add(nota);
            return nota;
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="xml">The XML.</param>
        public void LoadFromFile(string xml)
        {
            var provider = ProviderHelper.GetProvider(Parent.Configuracoes.WebServices.CodMunicipio);
            var nota = provider.LoadXml(xml);
            Add(nota);
        }

        #endregion Methods
    }
}