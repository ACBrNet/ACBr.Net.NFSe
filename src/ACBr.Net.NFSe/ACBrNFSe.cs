// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 04-20-2015
// ***********************************************************************
// <copyright file="ACBrNFSe.cs" company="ACBr.Net">
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
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using System;
using System.ComponentModel;

#region COM Interop Attributes

#if COM_INTEROP
using System.Runtime.InteropServices;
#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe
{
	#region COM Interop

	/* NOTAS para COM INTEROP
	 * Há um modo de compilação com a diretiva COM_INTEROP que inseri atributos e código específico
	 * para a DLL ser exportada para COM (ActiveX)
	 *
	 * O modelo COM possui alguma limitações/diferenças em relação ao modelo .NET
	 * Inserir os #if COM_INTEROP para prover implementações distintas nas modificações necessárias para COM:
	 *
	 * - Inserir atributos ComVisible(true), Guid("xxx") e ClassInterface(ClassInterfaceType.AutoDual) em todas as classes envolvidas
	 *
	 * - Propriedades/métodos que usam "Decimal" devem incluir o atributo MarshalAs(UnmanagedType.Currency)
	 *   usar [return: ...] para retornos de métodos e propriedades ou [param: ...] para o set de propriedades
	 *
	 * - Métodos que recebem array como parâmetros devem fazer como "ref".
	 *   Propriedades só podem retornar arrays, nunca receber.
	 *
	 * - Overload não é permitido. Métodos com mesmos nomes devem ser renomeados.
	 *   É possível usar parâmetros default, simplificando a necessidade de Overload
	 *
	 * - Generic não deve ser usado. Todas as classes Generic devem ser re-escritas como classes específicas
	 *
	 * - Eventos precisam de uma Interface com as declarações dos métodos (eventos) com o atributo [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	 *   A classe que declara os eventos precisa do atributo [ComSourceInterfaces(typeof(INomeDaInterface))]
	 *   Nenhum delegate deverá ser Generic, precisam ser re-escritos.
	 *
	 *   OBS: Por padrão o modelo .Net recebe os eventos com a assinatura void(object sender, EventArgs e)
	 *   O modelo COM não precisa desses parâmetros. Assim o delegate EventHandler foi redefinido para uma assinatura void()
	 *   Outros EventArgs devem seguir a assitarua COM void(MyEventArg e) ao invés da assinatura .NET void(object sender, MyEventArgs e)
	 * */

#if COM_INTEROP

	#region IDispatch Interface

	#region Documentation

	/// <summary>
	/// Interface contendo os eventos publicados pelo componente COM
	/// </summary>

	#endregion Documentation

	#endregion IDispatch Interface

	#region Delegates

	#region Comments

	///os componentes COM não suportam Generics
	///Estas são implementações específicas de delegates que no .Net são representados como EventHandler<T>

	#endregion Comments

	#endregion Delegates

#endif

	#endregion COM Interop

	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
    [Guid("0BE7D93A-0C14-4E14-B744-8653838197BA")]
	[ClassInterface(ClassInterfaceType.AutoDual)]
#endif

	#endregion COM Interop Attributes

	[ToolboxItem(typeof(ACBrNFSe))]
	// ReSharper disable once InconsistentNaming
	public class ACBrNFSe : ACBrComponent
	{
		#region Propriedades

		/// <summary>
		/// Configurações do Componente
		/// </summary>
		public Configuracoes Configuracoes { get; private set; }

		/// <summary>
		/// Componente de impressão
		/// </summary>
		public IDANFSe DaNfSe { get; set; }

		/// <summary>
		/// Coleção de NFSe para processar e/ou processadas
		/// </summary>
		public NotaFiscalCollection NotasFiscais { get; private set; }

		#endregion Propriedades

		#region Methods

		/// <summary>
		/// Envia as NFSe para o provedor da cidade de forma assincrona.
		/// </summary>
		/// <param name="lote">Numero do lote.</param>
		/// <param name="imprimir">Se for passado <c>true</c> imprime as RPS, se o envio foi executado com sucesso.</param>
		/// <returns>RetornoWebservice.</returns>
		public RetornoWebservice Enviar(int lote, bool imprimir)
		{
			Guard.Against<ArgumentException>(NotasFiscais.Count < 1, "ERRO: Nenhuma RPS adicionada ao Lote");
			Guard.Against<ArgumentException>(NotasFiscais.Count > 50, $"ERRO: Conjunto de RPS transmitidos (máximo de 50 RPS) excedido.{Environment.NewLine}" +
																	  $"Quantidade atual: {NotasFiscais.Count}");
			var provider = ProviderManager.GetProvider(Configuracoes);
			var ret = provider.Enviar(lote, NotasFiscais);

			if (ret.Sucesso && DaNfSe != null && imprimir)
				DaNfSe.Imprimir();

			return ret;
		}

		/// <summary>
		/// Envia as NFSe para o provedor da cidade de forma sincrona.
		/// Obs: Nem todos provedores suportar este metodo.
		/// </summary>
		/// <param name="lote">Numero do lote.</param>
		/// <param name="imprimir">Se for passado <c>true</c> imprime as NFSe, se o envio foi executado com sucesso.</param>
		/// <returns>RetornoWebservice.</returns>
		public RetornoWebservice EnviarSincrono(int lote, bool imprimir)
		{
			Guard.Against<ArgumentException>(NotasFiscais.Count < 1, "ERRO: Nenhuma RPS adicionada ao Lote");
			Guard.Against<ArgumentException>(NotasFiscais.Count > 50, $"ERRO: Conjunto de RPS transmitidos (máximo de 50 RPS) excedido.{Environment.NewLine}" +
																	  $"Quantidade atual: {NotasFiscais.Count}");
			var provider = ProviderManager.GetProvider(Configuracoes);
			var ret = provider.EnviarSincrono(lote, NotasFiscais);

			if (ret.Sucesso && DaNfSe != null && imprimir)
				DaNfSe.Imprimir();

			return ret;
		}

		/// <summary>
		/// Consulta a situação do lote de RPS.
		/// Obs: Nem todos provedores suportar este metodo.
		/// </summary>
		/// <param name="lote">The lote.</param>
		/// <param name="protocolo">The protocolo.</param>
		/// <returns>RetornoWebservice.</returns>
		public RetornoWebservice ConsultarSituacao(int lote, string protocolo = "")
		{
			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.ConsultarSituacao(lote, protocolo);
		}

		/// <summary>
		/// Consultars the lote RPS.
		/// </summary>
		/// <param name="protocolo">The protocolo.</param>
		/// <param name="lote">The lote.</param>
		/// <returns>RetornoWebservice.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public RetornoWebservice ConsultarLoteRps(string protocolo, int lote)
		{
			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.ConsultarLoteRps(protocolo, lote, NotasFiscais);
		}

		/// <summary>
		/// Consulta o numero de sequencia dos lotes de RPS.
		/// </summary>
		/// <param name="serie">The serie.</param>
		/// <returns>RetornoWebservice.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public RetornoWebservice ConsultarSequencialRps(string serie)
		{
			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.ConsultarSequencialRps(serie);
		}

		/// <summary>
		/// Consulta a NFSe/RPS que atende os filtros informados.
		/// Obs: Nem todos provedores suportar este metodo.
		/// </summary>
		/// <param name="numero">The numero.</param>
		/// <param name="serie">The serie.</param>
		/// <param name="tipo">The tipo.</param>
		/// <returns>RetornoWebservice.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public RetornoWebservice ConsultaNFSeRps(string numero, string serie, string tipo)
		{
			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.ConsultaNFSeRps(numero, serie, tipo, NotasFiscais);
		}

		/// <summary>
		/// Consulta as NFSe no periodo informado de acordo com os filtros.
		/// Obs: Nem todos provedores suportar este metodo.
		/// </summary>
		/// <param name="inicio">The inicio.</param>
		/// <param name="fim">The fim.</param>
		/// <param name="numeroNfse">The numero nfse.</param>
		/// <param name="pagina">The pagina.</param>
		/// <param name="cnpjTomador">The CNPJ tomador.</param>
		/// <param name="imTomador">The im tomador.</param>
		/// <param name="nomeInter">The nome inter.</param>
		/// <param name="cnpjInter">The CNPJ inter.</param>
		/// <param name="imInter">The im inter.</param>
		/// <param name="serie">The serie.</param>
		/// <returns>RetornoWebservice.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public RetornoWebservice ConsultaNFSe(DateTime inicio, DateTime fim, string numeroNfse = "", int pagina = 1,
			string cnpjTomador = "", string imTomador = "", string nomeInter = "", string cnpjInter = "", string imInter = "",
			string serie = "")
		{
			Guard.Against<ArgumentException>(inicio.Date > fim.Date, "A data inicial não pode ser maior que a data final.");

			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.ConsultaNFSe(inicio, fim, numeroNfse, pagina, cnpjTomador,
										 imTomador, nomeInter, cnpjInter, imInter, serie, NotasFiscais);
		}

		/// <summary>
		/// Cancela uma NFSe
		/// </summary>
		/// <param name="codigoCancelamento">O codigo de cancelamento.</param>
		/// <param name="numeroNFSe">O numero da NFSe.</param>
		/// <param name="motivo">O motivo.</param>
		/// <returns>RetornoWebservice.</returns>
		public RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo)
		{
			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.CancelaNFSe(codigoCancelamento, numeroNFSe, motivo);
		}

		/// <summary>
		/// Cancela as NFSe que estão carregadas na lista.
		///
		/// Obs.: Adicionar o motivo de cancelamento nas notas da lista.
		/// </summary>
		/// <param name="lote">Identificação do lote.</param>
		/// <returns>RetornoWebservice.</returns>
		public RetornoWebservice CancelaNFSe(int lote)
		{
			Guard.Against<ArgumentException>(NotasFiscais.Count < 1, "ERRO: Nenhuma NFS-e carregada ao componente");

			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.CancelaNFSe(lote, NotasFiscais);
		}

		/// <summary>
		/// Substitui uma NFSe
		/// </summary>
		/// <param name="codigoCancelamento">O codigo de cancelamento.</param>
		/// <param name="numeroNFSe">O numero da NFSe.</param>
		/// <param name="motivo">O motivo.</param>
		/// <returns>RetornoWebservice.</returns>
		public RetornoWebservice SubstituirNFSe(string codigoCancelamento, string numeroNFSe, string motivo)
		{
			Guard.Against<ArgumentException>(codigoCancelamento.IsEmpty(), "ERRO: Código de Cancelamento não informado");
			Guard.Against<ArgumentException>(numeroNFSe.IsEmpty(), "ERRO: Numero da NFS-e não informada");
			Guard.Against<ArgumentException>(NotasFiscais.Count < 1, "ERRO: Nenhuma RPS carregada ao componente");

			var provider = ProviderManager.GetProvider(Configuracoes);
			return provider.SubstituirNFSe(codigoCancelamento, numeroNFSe, motivo, NotasFiscais);
		}

		#endregion Methods

		#region Override Methods

		/// <summary>
		/// Função executada na inicialização do componente
		/// </summary>
		protected override void OnInitialize()
		{
			Configuracoes = new Configuracoes();
			NotasFiscais = new NotaFiscalCollection(this);
		}

		/// <summary>
		/// Função executada na desinicialização do componente
		/// </summary>
		protected override void OnDisposing()
		{
		}

		#endregion Override Methods
	}
}