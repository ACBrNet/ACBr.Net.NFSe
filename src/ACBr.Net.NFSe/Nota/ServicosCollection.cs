// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 10-01-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright file="ServicosCollection.cs" company="ACBr.Net">
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
using System.Collections;
using System.Collections.Generic;

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
	[Guid("251F7C51-69EE-47ED-9E98-E39550C4DE08")]
	[ClassInterface(ClassInterfaceType.AutoDual)]
#endif

	#endregion COM Interop Attributes

	public sealed class ServicosCollection : DFeCollection<Servico>, IEnumerable<Servico>
	{
		#region Constructors

		internal ServicosCollection()
		{
		}

		#endregion Constructors

		#region Methods

		public override Servico AddNew()
		{
			var ret = new Servico();
			Add(ret);
			return ret;
		}

		#endregion Methods

		#region IEnumerable<Servico>

#if COM_INTEROP

		[DispId(-4)]
		public IDictionaryEnumerator GetEnumerator()
#else

		public IEnumerator<Servico> GetEnumerator()
#endif
		{
#if COM_INTEROP
			return (IDictionaryEnumerator)(List.GetEnumerator() as IEnumerator);
#else
			return List.GetEnumerator();
#endif
		}

		IEnumerator<Servico> IEnumerable<Servico>.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		#endregion IEnumerable<Servico>
	}
}