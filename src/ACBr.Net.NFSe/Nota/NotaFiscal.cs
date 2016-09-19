// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 10-01-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="NotaFiscal.cs" company="ACBr.Net">
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
using ACBr.Net.Core.Generics;
using ACBr.Net.DFe.Core.Document;
using ACBr.Net.NFSe.Configuracao;
using System;
using PropertyChanged;

#region COM Interop Attributes

#if COM_INTEROP

using System.Runtime.InteropServices;

#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe.Nota
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("6E7C0D03-9D0F-4C00-8940-E5329BB4F9D4")]
	[ClassInterface(ClassInterfaceType.None)]
#endif

	#endregion COM Interop Attributes

	[ImplementPropertyChanged]
	public sealed class NotaFiscal : GenericClone<NotaFiscal>
	{
		#region Fields

		private ConfiguracoesNFSe config;

		#endregion Fields

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="NotaFiscal"/> class.
		/// </summary>
		public NotaFiscal(ConfiguracoesNFSe config, bool prestadorPadrão = true)
		{
			Guard.Against<ACBrException>(config == null, "Configurações não podem ser nulas");

			this.config = config;

			Id = 0;
			IdentificacaoRps = new IdeRps();
			IdentificacaoNFSe = new IdeNFSe();
			RpsSubstituido = new IdeRpsSubtituida();
			Servico = new DadosServico();
			Prestador = prestadorPadrão ? config.PrestadorPadrao : new DadosPrestador();
			Tomador = new DadosTomador();
			Intermediario = new DadosIntermediario();
			ConstrucaoCivil = new DadosConstrucaoCivil();
			Pagamento = new DadosPagamento();
			OrgaoGerador = new IdeOrgaoGerador();
			Signature = new Signature();
			Cancelamento = new IdeCancelamento();
			Transportadora = new DadosTransportadora();
			Emails = new EmailCollection();
		}

		#endregion Constructor

		#region Propriedades

		public int Id { get; set; }

		public IdeRps IdentificacaoRps { get; }

		public IdeNFSe IdentificacaoNFSe { get; }

		public IdeRpsSubtituida RpsSubstituido { get; }

		public IdeCancelamento Cancelamento { get; }

		public IdeOrgaoGerador OrgaoGerador { get; }

		public DadosPrestador Prestador { get; internal set; }

		public DadosTomador Tomador { get; }

		public DadosIntermediario Intermediario { get; }

		public DadosServico Servico { get; }

		public DadosConstrucaoCivil ConstrucaoCivil { get; }

		public DadosTransportadora Transportadora { get; }

		public DadosPagamento Pagamento { get; }

		public EmailCollection Emails { get; }

		public NaturezaOperacao NaturezaOperacao { get; set; }

		public RegimeEspecialTributacao RegimeEspecialTributacao { get; set; }

		public DateTime DataOptanteSimplesNacional { get; set; }

		public NFSeSimNao IncentivadorCultural { get; set; }

		public NFSeSimNao Producao { get; set; }

		public SituacaoNFSeRps Situacao { get; set; }

		public NFSeSimNao DeducaoMateriais { get; set; }

		public TipoLocalServico LocalServico { get; set; }

		public string NumeroLote { get; set; }

		public string Protocolo { get; set; }

		public DateTime Competencia { get; set; }

		public string OutrasInformacoes { get; set; }

		public decimal ValorCredito
		{
			#region COM_INTEROP

#if COM_INTEROP
			[return: MarshalAs(UnmanagedType.Currency)]
#endif

			#endregion COM_INTEROP

			get;
			set;
		}

		public TipoEmissao TipoEmissao { get; set; }

		public TipoEmpreitadaGlobal EmpreitadaGlobal { get; set; }

		public TipoTributacao TipoTributacao { get; set; }

		public string Assinatura { get; set; }

		public Signature Signature { get; set; }

		#endregion Propriedades
	}
}