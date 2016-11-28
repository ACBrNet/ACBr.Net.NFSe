using ACBr.Net.Core.Extensions;
using ACBr.Net.Core.Logging;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ACBr.Net.NFSe.Demo
{
	public partial class FormMain : Form, IACBrLog
	{
		#region Fields

		private ACBrNFSe acbrNFSe;

		#endregion Fields

		#region Constructors

		public FormMain()
		{
			InitializeComponent();
		}

		#endregion Constructors

		#region Methods

		#region EventHandlers

		private void btnConsultarSituacao_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				var ret = acbrNFSe.ConsultarSituacao(0, "10");
				wbbDados.LoadXml(ret.XmlEnvio);
				wbbResposta.LoadXml(ret.XmlRetorno);
			});
		}

		private void btnConsultarNFSeRps_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				var ret = acbrNFSe.ConsultaNFSeRps("10", "0", TipoRps.RPS);
				wbbDados.LoadXml(ret.XmlEnvio);
				wbbResposta.LoadXml(ret.XmlRetorno);
			});
		}

		private void btnSelecionarSchema_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				txtSchemas.Text = Helpers.SelectFolder();
			});
		}

		private void btnSelecionarArquivo_Click(object sender, EventArgs e)
		{
			LoadMunicipios();
		}

		private void btnAdicionar_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				var municipio = new MunicipioNFSe();
				if (FormEdtMunicipio.Editar(municipio).Equals(DialogResult.Cancel)) return;

				AddMunicipio(municipio);
			});
		}

		private void btnDeletar_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				if (listView1.SelectedItems.Count < 1) return;

				if (MessageBox.Show(@"Você tem certeza?", @"ACBrNFSe Demo", MessageBoxButtons.YesNo).Equals(DialogResult.No)) return;

				var municipio = listView1.SelectedItems[0];
				listView1.Items.Remove(municipio);
				UpdateCidades();
			});
		}

		private void btnCarregar_Click(object sender, EventArgs e)
		{
			LoadMunicipios();
		}

		private void btnSalvar_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				if (listView1.Items.Count < 1) return;

				var path = Helpers.SelectFolder();
				if (path.IsEmpty()) return;

				var municipios = listView1.Items.Cast<ListViewItem>().Select(x => (MunicipioNFSe)x.Tag);
				ProviderManager.Municipios.Clear();
				ProviderManager.Municipios.AddRange(municipios);
				ProviderManager.Save(Path.Combine(path, "Municipios.nfse"));
			});
		}

		private void btnGetCertificate_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				var numeroSerie = acbrNFSe.Configuracoes.Certificados.SelecionarCertificado();
				txtNumeroSerie.Text = numeroSerie;
			});
		}

		private void btnFindCertificate_Click(object sender, EventArgs e)
		{
			ExecuteSafe(() =>
			{
				var file = Helpers.OpenFile("Certificate Files (*.pfx)|*.pfx|All Files (*.*)|*.*", "Selecione o certificado");
				txtCertificado.Text = file;
			});
		}

		private void cmbCidades_SelectedValueChanged(object sender, EventArgs e)
		{
			var municipio = (MunicipioNFSe)cmbCidades.SelectedItem;
			txtUf.Text = municipio?.UF;
			txtCodCidade.Text = municipio?.Codigo.ToString();
			txtCodSiafi.Text = municipio?.CodigoSiafi.ToString();
		}

		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ExecuteSafe(() =>
			{
				if (listView1.SelectedItems.Count < 1) return;

				var municipio = listView1.SelectedItems[0].Tag as MunicipioNFSe;
				if (FormEdtMunicipio.Editar(municipio).Equals(DialogResult.Cancel)) return;

				listView1.Refresh();
				UpdateCidades();
			});
		}

		#endregion EventHandlers

		protected override void OnLoad(EventArgs e)
		{
			acbrNFSe = new ACBrNFSe();
			AddMunicipio(ProviderManager.Municipios.ToArray());
			base.OnLoad(e);
		}

		protected override void OnShown(EventArgs e)
		{
			InitializeLog();
			this.Log().Debug("Log Iniciado");
			base.OnShown(e);
		}

		private void AddMunicipio(params MunicipioNFSe[] municipios)
		{
			foreach (var municipio in municipios)
			{
				var item = new ListViewItem(municipio.Nome);
				item.SubItems.Add(municipio.UF);
				item.SubItems.Add(municipio.Codigo.ToString());
				item.SubItems.Add(municipio.CodigoSiafi.ToString());
				item.SubItems.Add(municipio.Provedor);
				item.Tag = municipio;

				listView1.Items.Add(item);
			}

			UpdateCidades();
		}

		private void LoadMunicipios()
		{
			ExecuteSafe(() =>
			{
				var arquivo = Helpers.OpenFile("Arquivo de cidades NFSe (*.nfse) | *.nfse |Todos os arquivos | *.*", "Selecione o arquivo de cidades");
				if (arquivo.IsEmpty()) return;

				ProviderManager.Load(arquivo);

				txtArquivoCidades.Text = arquivo;

				listView1.BeginUpdate();

				listView1.Items.Clear();
				AddMunicipio(ProviderManager.Municipios.ToArray());

				listView1.EndUpdate();
			});
		}

		private void UpdateCidades()
		{
			cmbCidades.DataSource = null;
			cmbCidades.Items.Clear();
			cmbCidades.DisplayMember = "Nome";
			cmbCidades.ValueMember = "Codigo";
			cmbCidades.DataSource = ProviderManager.Municipios;
		}

		private void InitializeLog()
		{
			var config = new LoggingConfiguration();
			var target = new RichTextBoxTarget
			{
				UseDefaultRowColoringRules = true,
				Layout = @"${date:format=dd/MM/yyyy HH\:mm\:ss} - ${message}",
				FormName = Name,
				ControlName = rtbLog.Name,
				AutoScroll = true
			};

			config.AddTarget("RichTextBox", target);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

			var infoTarget = new FileTarget
			{
				FileName = "${basedir:dir=Logs:file=ACBrNFSe.log}",
				Layout = "${processid}|${longdate}|${level:uppercase=true}|" +
						 "${event-context:item=Context}|${logger}|${message}",
				CreateDirs = true,
				Encoding = Encoding.UTF8,
				MaxArchiveFiles = 93,
				ArchiveEvery = FileArchivePeriod.Day,
				ArchiveNumbering = ArchiveNumberingMode.Date,
				ArchiveFileName = "${basedir}/Logs/Archive/${date:format=yyyy}/${date:format=MM}/ACBrNFSe_{{#}}.log",
				ArchiveDateFormat = "dd.MM.yyyy"
			};

			config.AddTarget("infoFile", infoTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, infoTarget));
			LogManager.Configuration = config;
		}

		private void ExecuteSafe(Action action)
		{
			try
			{
				action();
			}
			catch (Exception exception)
			{
				lblStatus.Text = exception.Message;
			}
		}

		#endregion Methods
	}
}