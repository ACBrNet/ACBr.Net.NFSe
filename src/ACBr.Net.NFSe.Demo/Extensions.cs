using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using ACBr.Net.Core.Extensions;
using ACBr.Net.NFSe.Providers;

namespace ACBr.Net.NFSe.Demo
{
    public static class Extensions
    {
        public static void LoadXml(this WebBrowser browser, string xml)
        {
            if (xml.IsEmpty()) return;

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

        public static void AppendLine(this RichTextBox rtb, string text)
        {
            rtb.AppendText(text + Environment.NewLine);
        }

        public static void JumpLine(this RichTextBox rtb)
        {
            rtb.AppendText(Environment.NewLine);
        }

        public static void EnumDataSource<T>(this ComboBox cmb) where T : struct
        {
            cmb.DataSource = (from T value in Enum.GetValues(typeof(T)) select new ItemData<T>(value)).ToArray();
        }

        public static void EnumDataSource<T>(this ComboBox cmb, T valorPadrao) where T : struct
        {
            var list = (from T value in Enum.GetValues(typeof(T)) select new ItemData<T>(value.ToString(), value)).ToArray();
            cmb.DataSource = list;
            cmb.SelectedItem = list.SingleOrDefault(x => x.Content.Equals(valorPadrao));
        }

        public static T GetSelectedValue<T>(this ComboBox cmb)
        {
            return ((ItemData<T>)cmb.SelectedItem).Content;
        }

        public static void SetSelectedValue<T>(this ComboBox cmb, T valor) where T : struct
        {
            var dataSource = (ItemData<T>[])cmb.DataSource;
            cmb.SelectedItem = dataSource.SingleOrDefault(x => x.Content.Equals(valor));
        }

        public static void MunicipiosDataSource(this ComboBox cmb)
        {
            cmb.DataSource = (from ACBrMunicipioNFSe value in ProviderManager.Municipios
                              select new ItemData<ACBrMunicipioNFSe>($"{value.Nome} - {value.UF}", value)).ToArray();
        }

        public static void SetSelectedValue(this ComboBox cmb, ACBrMunicipioNFSe valor)
        {
            var dataSource = (ItemData<ACBrMunicipioNFSe>[])cmb.DataSource;
            cmb.SelectedItem = dataSource.SingleOrDefault(x => x.Content.Codigo == valor.Codigo);
        }
    }
}