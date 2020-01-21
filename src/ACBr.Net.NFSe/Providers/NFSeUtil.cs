// ***********************************************************************
// Assembly         : ACBr.Net.NFSe.Shared
// Author           : RFTD
// Created          : 06-02-2018
//
// Last Modified By : RFTD
// Last Modified On : 06-02-2018
// ***********************************************************************
// <copyright file="NFSeUtil.cs" company="ACBr.Net">
//		        	   The MIT License (MIT)
//	     		Copyright (c) 2016-2018 Grupo ACBr.Net
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
using ACBr.Net.Core.Extensions;

namespace ACBr.Net.NFSe.Providers
{
    internal static class NFSeUtil
    {
        #region Fields

        private static readonly string[] escapedCharacters = { "&amp;", "&lt;", "&gt;" };
        private static readonly string[] unescapedCharacters = { "&", "<", ">", };

        #endregion Fields

        #region Methods

        public static string RemoverDeclaracaoXml(this string xml)
        {
            if (xml.IsEmpty()) return xml;

            var posIni = xml.IndexOf("<?", StringComparison.Ordinal);
            if (posIni < 0) return xml;

            var posFinal = xml.IndexOf("?>", StringComparison.Ordinal);
            return posFinal < 0 ? xml : xml.Remove(posIni, (posFinal + 2) - posIni);
        }

        public static string AjustarEnvio(this string envio)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                envio = envio.Replace(unescapedCharacters[i], escapedCharacters[i]);
            }
            return envio;
        }

        public static string AjustarRetorno(this string retorno)
        {
            for (var i = 0; i < escapedCharacters.Length; i++)
            {
                retorno = retorno.Replace(escapedCharacters[i], unescapedCharacters[i]);
            }
            retorno = retorno.Replace("xmlns=\"\"", "");
            retorno = retorno.Replace("xmlns=\"FISS-LEX\"", "");
            return retorno;
        }

        #endregion Methods
    }
}