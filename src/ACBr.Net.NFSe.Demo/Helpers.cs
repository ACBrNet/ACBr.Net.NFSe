using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ACBr.Net.NFSe.Demo
{
	public static class Helpers
	{
		public static string OpenFiles(string filters, string title = "Abrir")
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.CheckPathExists = true;
				ofd.CheckFileExists = true;
				ofd.Multiselect = false;
				ofd.Filter = filters;
				ofd.Title = title;

				if (ofd.ShowDialog().Equals(DialogResult.Cancel))
					return null;

				return ofd.FileName;
			}
		}

		public static Configuration GetConfiguration()
		{
			var configFile = Path.Combine(Application.StartupPath, "nfse.config");
			if (!File.Exists(configFile))
			{
				var sb = new StringBuilder();
				sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				sb.AppendLine("<configuration>");
				sb.AppendLine("	<appSettings>");
				sb.AppendLine("	</appSettings>");
				sb.AppendLine("</configuration>");
				File.WriteAllText(configFile, sb.ToString());
			}

			var configFileMap = new ExeConfigurationFileMap
			{
				ExeConfigFilename = Path.Combine(Application.StartupPath, "sat.config")
			};

			return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
		}
	}
}