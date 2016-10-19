// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 10-01-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="DeducoesCollection.cs" company="ACBr.Net">
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

using ACBr.Net.DFe.Core.Collection;
using PropertyChanged;
using System.Collections;
using System.Collections.Generic;

namespace ACBr.Net.NFSe.Nota
{
	[ImplementPropertyChanged]
	public sealed class DeducoesCollection : DFeCollection<Deducao>, IEnumerable<Deducao>
	{
		#region Contructors

		internal DeducoesCollection()
		{
		}

		#endregion Contructors

		#region Propriedades

		public new Deducao this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		#endregion Propriedades

		#region Methods

		public override Deducao AddNew()
		{
			var ret = new Deducao();
			Add(ret);
			return ret;
		}

		#endregion Methods

		#region IEnumerable<Deducao>
		public IEnumerator<Deducao> GetEnumerator()
        {
			return GetEnumerator();
		}

		IEnumerator<Deducao> IEnumerable<Deducao>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IEnumerable<Deducao>
	}
}