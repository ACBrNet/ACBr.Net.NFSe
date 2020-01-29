// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Rafael Dias
// Created          : 12-26-2017
//
// Last Modified By : Rafael Dias
// Last Modified On : 12-26-2017
// ***********************************************************************
// <copyright file="ProviderAbaco.cs" company="ACBr.Net">
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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class ProviderAbaco : ProviderABRASF
    {
        #region Constructors

        public ProviderAbaco(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Abaco";
        }

        #endregion Constructors

        #region Methods

        public override RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override string GetNamespace()
        {
            return string.Empty;
        }

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new AbacoServiceClient(this, tipo);
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_v2010.xsd";
        }

        #endregion Methods
    }
}