// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : Rafael Dias
// Created          : 10-01-2014
//
// Last Modified By : Rafael Dias
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

using ACBr.Net.Core.Generics;
using ACBr.Net.DFe.Core.Document;
using System;
using System.ComponentModel;

namespace ACBr.Net.NFSe.Nota
{
    public sealed class NotaFiscal : GenericClone<NotaFiscal>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Constructor

        public NotaFiscal()
        {
            Id = 0;
            IdentificacaoRps = new IdeRps();
            IdentificacaoNFSe = new IdeNFSe();
            RpsSubstituido = new IdeRpsSubtituida();
            Servico = new DadosServico();
            Prestador = new DadosPrestador();
            Tomador = new DadosTomador();
            Intermediario = new DadosIntermediario();
            ConstrucaoCivil = new DadosConstrucaoCivil();
            Material = new DadosMateriais();
            Pagamento = new DadosPagamento();
            OrgaoGerador = new IdeOrgaoGerador();
            Signature = new DFeSignature();
            Cancelamento = new IdeCancelamento();
            Transportadora = new DadosTransportadora();
            Emails = new EmailCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotaFiscal"/> class.
        /// </summary>
        public NotaFiscal(DadosPrestador prestador) : this()
        {
            Prestador = prestador;
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

        public DadosMateriais Material { get; set; }

        public DadosTransportadora Transportadora { get; }

        public DadosPagamento Pagamento { get; }

        public EmailCollection Emails { get; }

        public int NaturezaOperacao { get; set; }

        public RegimeEspecialTributacao RegimeEspecialTributacao { get; set; }

        public DateTime DataOptanteSimplesNacional { get; set; }

        public NFSeSimNao IncentivadorCultural { get; set; }

        public NFSeSimNao Producao { get; set; }

        public SituacaoNFSeRps Situacao { get; set; }

        public TipoLocalServico LocalServico { get; set; }

        public string NumeroLote { get; set; }

        public string Protocolo { get; set; }

        public DateTime Competencia { get; set; }

        public string OutrasInformacoes { get; set; }

        public string DiscriminacaoImpostos { get; set; }

        public string DescricaoCodigoTributacaoMunicipio { get; set; }

        public decimal ValorCredito { get; set; }

        public TipoEmissao TipoEmissao { get; set; }

        public TipoEmpreitadaGlobal EmpreitadaGlobal { get; set; }

        public TipoTributacao TipoTributacao { get; set; }

        public string Assinatura { get; set; }

        public DFeSignature Signature { get; set; }

        #endregion Propriedades
    }
}