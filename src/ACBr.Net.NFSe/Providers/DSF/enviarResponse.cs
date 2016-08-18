// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 08-17-2016
//
// Last Modified By : RFTD
// Last Modified On : 08-17-2016
// ***********************************************************************
// <copyright file="enviarResponse.cs" company="ACBr.Net">
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
using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.DSF
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "enviarResponse", WrapperNamespace = "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws", IsWrapped = true)]
	// ReSharper disable once InconsistentNaming
	internal class enviarResponse
	{
		/// <summary>
		/// The enviar return
		/// </summary>
		[MessageBodyMember(Namespace = "", Order = 0)]
		// ReSharper disable once InconsistentNaming
		public string enviarReturn;

		/// <summary>
		/// Initializes a new instance of the <see cref="enviarResponse"/> class.
		/// </summary>
		public enviarResponse()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="enviarResponse"/> class.
		/// </summary>
		/// <param name="enviarReturn">The enviar return.</param>
		public enviarResponse(string enviarReturn)
		{
			this.enviarReturn = enviarReturn;
		}
	}
}