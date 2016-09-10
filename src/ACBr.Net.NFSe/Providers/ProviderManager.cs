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
using ACBr.Net.NFSe.Providers.DSF;
using ACBr.Net.NFSe.Providers.Ginfes;
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
	public static class ProviderManager
	{
		#region Constructors

		static ProviderManager()
		{
			Municipios = new List<MunicipioNFSe>();
			Providers = new Dictionary<string, Type>
			{
				{"DSF", typeof(ProviderDSF)},
				{"ISSDSF", typeof(ProviderDSF)},
				{"GINFES", typeof(ProviderGinfes)}
			};

			Load();
		}

		#endregion Constructors

		#region Propriedades

		/// <summary>
		/// Municipios cadastrados no ACBrNFSe
		/// </summary>
		/// <value>Os municipios</value>
		public static List<MunicipioNFSe> Municipios { get; }

		/// <summary>
		/// Provedores cadastrados no ACBrNFSe
		/// </summary>
		/// <value>Os provedores</value>
		public static Dictionary<string, Type> Providers { get; }

		#endregion Propriedades

		#region Methods

		#region Public

		/// <summary>
		/// Salva o arquivo de cidades.
		/// </summary>
		/// <param name="path">Caminho para salvar o arquivo</param>
		public static void Save(string path = "Municipios.nfse")
		{
			Guard.Against<ArgumentNullException>(path == null, "Path invalido.");

			if (File.Exists(path))
				File.Delete(path);

			using (var fileStream = new FileStream(path, FileMode.CreateNew))
			{
				Save(fileStream);
			}
		}

		/// <summary>
		/// Salva o arquivo de cidades.
		/// </summary>
		/// <param name="stream">O stream.</param>
		public static void Save(Stream stream)
		{
			using (var zip = new GZipStream(stream, CompressionMode.Compress))
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(zip, Municipios.ToArray());
			}
		}

		/// <summary>
		/// Carrega o arquivo de cidades.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="clean">if set to <c>true</c> [clean].</param>
		public static void Load(string path = "", bool clean = true)
		{
			byte[] buffer = null;
			if (path.IsEmpty())
			{
				buffer = Properties.Resources.Municipios;
			}
			else if (File.Exists(path))
			{
				buffer = File.ReadAllBytes(path);
			}

			Guard.Against<ArgumentException>(buffer == null, "Arquivo de cidades não encontrado");

			using (var stream = new MemoryStream(buffer))
			{
				Load(stream, clean);
			}
		}

		/// <summary>
		/// Carrega o arquivo de cidades.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="clean">if set to <c>true</c> [clean].</param>
		public static void Load(Stream stream, bool clean = true)
		{
			Guard.Against<ArgumentException>(stream == null, "Arquivo de cidades não encontrado");

			using (var zip = new GZipStream(stream, CompressionMode.Decompress))
			{
				var formatter = new BinaryFormatter();
				var cidades = (MunicipioNFSe[])formatter.Deserialize(zip);

				if (clean) Municipios.Clear();
				Municipios.AddRange(cidades);
			}
		}

		#endregion Public

		#region Internal

		/// <summary>
		/// Retorna o provedor para o municipio nas configurações informadas.
		/// </summary>
		/// <param name="config">A configuração.</param>
		/// <returns>Provedor NFSe.</returns>
		public static ProviderBase GetProvider(ConfiguracoesNFSe config)
		{
			var municipio = Municipios.SingleOrDefault(x => x.Codigo == config.WebServices.CodMunicipio);
			Guard.Against<ACBrException>(municipio == null, "Provedor para esta cidade não implementado ou não especificado!");

			// ReSharper disable once PossibleNullReferenceException
			var providerType = Providers[municipio.Provedor.ToUpper()];
			Guard.Against<ACBrException>(providerType == null, "Provedor não encontrado!");
			Guard.Against<ACBrException>(!typeof(ProviderBase).IsAssignableFrom(providerType), "Classe base do provedor incorreta!");

			// ReSharper disable once AssignNullToNotNullAttribute
			return (ProviderBase)Activator.CreateInstance(providerType, config, municipio);
		}

		#endregion Internal

		#endregion Methods
	}
}