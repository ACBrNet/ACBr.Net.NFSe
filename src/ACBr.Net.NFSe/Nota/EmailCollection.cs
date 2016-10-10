// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 06-05-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-05-2016
// ***********************************************************************
// <copyright file="EmailCollection.cs" company="ACBr.Net">
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

#region COM Interop Attributes

#if COM_INTEROP

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe.Nota
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("C4B9AF40-B4EC-48ED-A64A-D6F64F22C6D0")]
	[ClassInterface(ClassInterfaceType.None)]
#endif

	#endregion COM Interop Attributes

	[ImplementPropertyChanged]
	public sealed class EmailCollection : DFeCollection<EmailItem>, IEnumerable<EmailItem>
	{
		#region Contructors

		internal EmailCollection()
		{
		}

		#endregion Contructors

		#region Propriedades

		#region COM Interop Attributes

#if COM_INTEROP

		[IndexerName("GetItem")]
#endif

		#endregion COM Interop Attributes

		public new EmailItem this[int index]
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

		public override EmailItem AddNew()
		{
			var ret = new EmailItem();
			Add(ret);
			return ret;
		}

		#endregion Methods

		#region IEnumerable<EmailItem>

#if COM_INTEROP

		[DispId(-4)]
		public IDictionaryEnumerator GetEnumerator()
#else

		public IEnumerator<EmailItem> GetEnumerator()
#endif
		{
#if COM_INTEROP
			return (IDictionaryEnumerator)(GetEnumerator() as IEnumerator);
#else
			return GetEnumerator();
#endif
		}

		IEnumerator<EmailItem> IEnumerable<EmailItem>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IEnumerable<EmailItem>
	}
}