// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 10-01-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-11-2014
// ***********************************************************************
// <copyright file="INFSeProvider.cs" company="ACBr.Net">
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

using ACBr.Net.Core.Logging;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ACBr.Net.NFSe.Interfaces
{
	/// <summary>
	/// Interface INFSeProvider
	/// </summary>
	public interface INFSeProvider : IACBrLog
	{
		#region Propriedades

		/// <summary>
		/// Gets the lista de alertas.
		/// </summary>
		/// <value>The lista de alertas.</value>
		List<string> ListaDeAlertas { get; }

		/// <summary>
		/// Gets or sets the formato alerta.
		/// </summary>
		/// <value>The formato alerta.</value>
		string FormatoAlerta { get; set; }

		#endregion Propriedades

		#region Methods

		/// <summary>
		/// Gets the XML RPS.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="identado">if set to <c>true</c> [identado].</param>
		/// <param name="showDeclaration">if set to <c>true</c> [show declaration].</param>
		/// <returns>System.String.</returns>
		string GetXmlRPS(Nota.NotaFiscal item, bool identado = true, bool showDeclaration = true);

		/// <summary>
		/// Gets the XML nf se.
		/// </summary>
		/// <param name="nota">The nota.</param>
		/// <param name="identado">if set to <c>true</c> [identado].</param>
		/// <param name="showDeclaration">if set to <c>true</c> [show declaration].</param>
		/// <returns>System.String.</returns>
		string GetXmlNFSe(Nota.NotaFiscal nota, bool identado = true, bool showDeclaration = true);

		/// <summary>
		/// Carrega XML da NFSe no Componente a partir do caminho do XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		NotaFiscal LoadXml(string xml, Encoding encoding);

		/// <summary>
		/// Carrega XML da NFSe no Componente a parte da classe XmlDocument.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		NotaFiscal LoadXml(XmlDocument xml);

		/// <summary>
		/// Carrega XML da NFSe no Componente a partir do Stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		NotaFiscal LoadXml(Stream stream);

		RetornoWebservice Enviar(int lote, NotaFiscalCollection notas);

		RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas);

		RetornoWebservice ConsultarSituacao(int lote, string protocolo);

		RetornoWebservice ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas);

		RetornoWebservice ConsultarSequencialRps(string serie);

		RetornoWebservice ConsultaNFSeRps(string numero, string serie, string tipo, NotaFiscalCollection notas);

		RetornoWebservice ConsultaNFSe(DateTime inicio, DateTime fim, string numeroNfse, int pagina,
			string cnpjTomador, string imTomador, string nomeInter, string cnpjInter, string imInter,
			string serie, NotaFiscalCollection notas);

		RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas);

		RetornoWebservice CancelaNFSe(int lote, NotaFiscalCollection notas);

		RetornoWebservice SubstituirNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas);

		#endregion Methods
	}
}