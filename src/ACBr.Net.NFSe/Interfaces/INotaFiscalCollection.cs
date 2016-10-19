using ACBr.Net.NFSe.Nota;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ACBr.Net.NFSe.Interfaces
{

	public interface INotaFiscalCollection
	{
		NotaFiscal this[int index] { get; set; }

		NotaFiscal AddNew();

		IEnumerator<NotaFiscal> GetEnumerator();

		string GetXml(NotaFiscal nota);

		NotaFiscal Load(XDocument xml);

		NotaFiscal Load(Stream stream);

		NotaFiscal Load(string xml, Encoding encoding = null);

		void Save(NotaFiscal nota, string path);

		void Save(NotaFiscal nota, Stream stream);
	}
}