using ACBr.Net.Core.Logging;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe.Providers;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using System;
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
			ExecuteSafe(() => acbrNFSe.ConsultarSituacao(0, "10"));
		}

		private void btnSalvar_Click(object sender, EventArgs e)
		{
			/*
			Exemplo de como adicionar Cidade no arquivo de cidades

			var municipio = new MunicipioNFSe
			{
				Nome = "Ribeirão Preto",
				UF = "SP",
				Codigo = 3543402,
				CodigoSiafi = 0,
				Provedor = "GINFES"
			};

			municipio.UrlProducao.Add(TipoUrl.Enviar, "https://producao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlProducao.Add(TipoUrl.EnviarSincrono, "");
			municipio.UrlProducao.Add(TipoUrl.CancelaNFSe, "https://producao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultaNFSe, "https://producao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultaNFSeRps, "https://producao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultarLoteRps, "https://producao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultarSituacao, "https://producao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultarSequencialRps, "");
			municipio.UrlProducao.Add(TipoUrl.SubstituirNFSe, "");

			municipio.UrlHomologacao.Add(TipoUrl.Enviar, "https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.EnviarSincrono, "");
			municipio.UrlHomologacao.Add(TipoUrl.CancelaNFSe, "https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultaNFSe, "https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultaNFSeRps, "https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultarLoteRps, "https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultarSituacao, "https://homologacao.ginfes.com.br/ServiceGinfesImpl?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultarSequencialRps, "");
			municipio.UrlHomologacao.Add(TipoUrl.SubstituirNFSe, "");

			AddMunicipio(municipio);

			*/

			ExecuteSafe(() =>
			{
				if (listView1.Items.Count < 1) return;

				var municipios = listView1.Items.Cast<ListViewItem>().Select(x => (MunicipioNFSe)x.Tag);
				ProviderManager.Municipios.Clear();
				ProviderManager.Municipios.AddRange(municipios);
				ProviderManager.Serialize();
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
				var file = Helpers.OpenFiles("Certificate Files (*.pfx)|*.pfx|All Files (*.*)|*.*", "Selecione o certificado");
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

		#endregion EventHandlers

		protected override void OnLoad(EventArgs e)
		{
			acbrNFSe = new ACBrNFSe();

			acbrNFSe.Configuracoes.WebServices.CodMunicipio = 3543402;
			acbrNFSe.Configuracoes.WebServices.Ambiente = TipoAmbiente.Homologacao;

			acbrNFSe.Configuracoes.PrestadoPadrao.InscricaoMunicipal = "0000000000";

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

		private void UpdateCidades()
		{
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