// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-16-2017
//
// Last Modified By : RFTD
// Last Modified On : 01-16-2017
// ***********************************************************************
// <copyright file="ProviderPortoAlegre.cs" company="ACBr.Net">
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers.PortoAlegre
{
	internal sealed class ProviderPortoAlegre : ProviderABRASF
	{
		#region Constructors

		public ProviderPortoAlegre(ConfiguracoesNFSe config, MunicipioNFSe municipio) : base(config, municipio)
		{
			Name = "Porto Alegre";
		}

		#endregion Constructors

		#region Methods

		protected override IABRASFClient GetClient(TipoUrl tipo)
		{
			var url = GetUrl(tipo);
			return new PortoAlegreServiceClient(url, TimeOut, Certificado);
		}

		protected override string GetNamespace()
		{
			return "http://www.abrasf.org.br/nfse.xsd";
		}

		protected override string GetSchema(TipoUrl tipo)
		{
			return "nfse_v20_08_2015.xsd";
		}

		#endregion Methods
	}
}