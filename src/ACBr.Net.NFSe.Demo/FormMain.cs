using ACBr.Net.NFSe.Providers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ACBr.Net.NFSe.Demo
{
	public partial class FormMain : Form
	{
		#region Constructors

		public FormMain()
		{
			InitializeComponent();
		}

		#endregion Constructors

		#region Methods

		#region EventHandlers

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

			if (listView1.Items.Count < 1) return;

			var municipios = listView1.Items.Cast<ListViewItem>().Select(x => (MunicipioNFSe)x.Tag);
			ProviderManager.Municipios.Clear();
			ProviderManager.Municipios.AddRange(municipios);
			ProviderManager.Serialize();
		}

		#endregion EventHandlers

		protected override void OnLoad(EventArgs e)
		{
			PopulateMunicipios();
			base.OnLoad(e);
		}

		private void PopulateMunicipios()
		{
			AddMunicipio(ProviderManager.Municipios.ToArray());
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
		}

		#endregion Methods
	}
}