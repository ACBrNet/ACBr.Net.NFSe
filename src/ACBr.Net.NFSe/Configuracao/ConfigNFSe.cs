// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="ConfigNFSe.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Configuracao
{
    [TypeConverter(typeof(ACBrExpandableObjectConverter))]
    public sealed class ConfigNFSe : DFeConfigBase<ACBrNFSe, ConfigGeralNFSe, ConfigWebServicesNFSe, ConfigCertificadosNFSe, ConfigArquivosNFSe>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigNFSe"/> class.
        /// </summary>
        internal ConfigNFSe(ACBrNFSe parent) : base(parent)
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the prestado padr�o.
        /// </summary>
        /// <value>The prestado padr�o.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DadosPrestador PrestadorPadrao { get; set; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        protected override void CreateConfigs()
        {
            Geral = new ConfigGeralNFSe(Parent);
            WebServices = new ConfigWebServicesNFSe(Parent);
            Certificados = new ConfigCertificadosNFSe(Parent);
            Arquivos = new ConfigArquivosNFSe(Parent);
            PrestadorPadrao = new DadosPrestador();
        }

        #endregion Methods
    }
}