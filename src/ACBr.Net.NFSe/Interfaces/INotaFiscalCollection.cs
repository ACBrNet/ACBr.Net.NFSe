using ACBr.Net.NFSe.Nota;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

#region COM Interop Attributes

#if COM_INTEROP

using System.Runtime.InteropServices;
using System.Collections;

#else

using System.Collections.Generic;

#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe.Interfaces
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("6EC859D4-FF8A-47EA-873D-4E898BBF4EB3")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
#endif

	#endregion COM Interop Attributes

	public interface INotaFiscalCollection
	{
		#region COM Interop Attributes

#if COM_INTEROP

		[IndexerName("GetItem")]
#endif

		#endregion COM Interop Attributes

		NotaFiscal this[int index] { get; set; }

		NotaFiscal AddNew();

#if COM_INTEROP

		[DispId(-4)]
		IEnumerator GetEnumerator();

#else

		IEnumerator<NotaFiscal> GetEnumerator();

#endif

		string GetXml(NotaFiscal nota);

		NotaFiscal Load(XDocument xml);

		NotaFiscal Load(Stream stream);

		NotaFiscal Load(string xml, Encoding encoding = null);

		void Save(NotaFiscal nota, string path);

		void Save(NotaFiscal nota, Stream stream);
	}
}