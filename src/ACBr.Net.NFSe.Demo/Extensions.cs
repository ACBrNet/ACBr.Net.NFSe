using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ACBr.Net.NFSe.Demo
{
	public static class Extensions
	{
		public static void LoadXml(this WebBrowser browser, string xml)
		{
			if (xml == null)
				return;

			var path = Path.GetTempPath();
			var fileName = Guid.NewGuid() + ".xml";
			var fullFileName = Path.Combine(path, fileName);
			var xmlDoc = new XmlDocument();
			if (File.Exists(xml))
				xmlDoc.Load(xml);
			else
				xmlDoc.LoadXml(xml);
			xmlDoc.Save(fullFileName);
			browser.Navigate(fullFileName);
		}

		public static void EnumDataSource<T>(this ComboBox cmb, T? valorPadrao = null) where T : struct
		{
			cmb.DataSource = Enum.GetValues(typeof(T));
			if (valorPadrao.HasValue)
				cmb.SelectedItem = valorPadrao.Value;
		}

		public static void AddValue(this KeyValueConfigurationCollection col, string key, string value)
		{
			if (col[key]?.Value != null)
				col[key].Value = value;
			else
				col.Add(key, value);
		}
	}
}