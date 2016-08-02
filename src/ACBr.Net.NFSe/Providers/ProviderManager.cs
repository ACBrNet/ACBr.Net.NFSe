// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="ProviderManager.cs" company="ACBr.Net">
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

using ACBr.Net.Core;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Interfaces;
using ACBr.Net.NFSe.Providers.DSF;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ACBr.Net.NFSe.Providers
{
	/// <summary>
	/// Classe ProviderManager.
	/// </summary>
	public class ProviderManager
	{
		#region Constructors

		static ProviderManager()
		{
			Municipios = new List<MunicipioNFSe>();
		}

		#endregion Constructors

		#region Propriedades

		public static List<MunicipioNFSe> Municipios { get; }

		#endregion Propriedades

		#region Methods

		#region Public

		public static void Serialize(Stream stream)
		{
			using (var zip = new GZipStream(stream, CompressionMode.Compress))
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(zip, Municipios);
			}
		}

		public static void Deserialize(string path = "", bool clean = true)
		{
			byte[] buffer = null;
			if (path.IsEmpty())
			{
				//todo: colocar um resouce com as configuracoes de cidades
				buffer = new byte[0];
			}
			else if (File.Exists(path))
			{
				buffer = File.ReadAllBytes(path);
			}

			Guard.Against<ArgumentException>(buffer == null, "Arquivo de cidades não encontrado");
			using (var stream = new MemoryStream(buffer))
			{
				using (var zip = new GZipStream(stream, CompressionMode.Decompress))
				{
					var formatter = new BinaryFormatter();
					var cidades = (MunicipioNFSe[])formatter.Deserialize(zip);

					if (clean) Municipios.Clear();
					Municipios.AddRange(cidades);
				}
			}
		}

		#endregion Public

		#region Internal

		internal static INFSeProvider GetProvider(Configuracoes config)
		{
			var municipio = Municipios.SingleOrDefault(x => x.Codigo == config.WebServices.CodMunicipio);
			Guard.Against<ACBrException>(municipio == null, "Provedor para esta cidade não implementado ou não especificado ! ");

			switch (municipio.Provedor.ToUpper())
			{
				case "DSF":
				case "ISSDSF":
					return new ProviderDSF(config, municipio);

				default:
					throw new ACBrException("Provedor não encontrado");
			}
		}

		#endregion Internal

		#endregion Methods
	}
}