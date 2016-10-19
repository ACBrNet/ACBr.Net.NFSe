// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-26-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="ValoresServico.cs" company="ACBr.Net">
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
using PropertyChanged;

namespace ACBr.Net.NFSe.Nota
{
	[ImplementPropertyChanged]
	public sealed class ValoresServico : GenericClone<ValoresServico>
	{
		#region Constructors

		internal ValoresServico()
		{
			IssRetido = SituacaoTributaria.Normal;
		}

		#endregion Constructors

		#region Propriedades

		public decimal ValorServicos { get; set; }

        public decimal ValorDeducoes { get; set; }

        public decimal ValorPis { get; set; }

        public decimal ValorCofins { get; set; }

        public decimal ValorInss { get; set; }

        public decimal ValorIr { get; set; }

        public decimal ValorCsll { get; set; }

        public SituacaoTributaria IssRetido { get; set; }

		public decimal ValorIss { get; set; }

        public decimal OutrasRetencoes { get; set; }

        public decimal BaseCalculo { get; set; }

        public decimal Aliquota { get; set; }

        public decimal AliquotaPis { get; set; }

        public decimal AliquotaCofins { get; set; }

        public decimal AliquotaInss { get; set; }

        public decimal AliquotaIR { get; set; }

        public decimal AliquotaCsll { get; set; }

        public decimal ValorLiquidoNfse { get; set; }

        public decimal ValorIssRetido { get; set; }

        public decimal DescontoCondicionado { get; set; }

        public decimal DescontoIncondicionado { get; set; }

        public string JustificativaDeducao { get; set; }

		public decimal ValorOutrasRetencoes { get; set; }

        public string DescricaoOutrasRetencoes { get; set; }

		#endregion Propriedades
	}
}