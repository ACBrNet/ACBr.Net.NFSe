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
				Nome = "Campo Grande",
				UF = "MS",
				Codigo = 5002704,
				CodigoSiafi = 9051,
				Provedor = "DSF"
			};

			municipio.UrlProducao.Add(TipoUrl.Enviar, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.EnviarSincrono, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.CancelaNFSe, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultaNFSe, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultaNFSeRps, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultarLoteRps, "");
			municipio.UrlProducao.Add(TipoUrl.ConsultarSituacao, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.ConsultarSequencialRps, "http://issdigital.pmcg.ms.gov.br/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlProducao.Add(TipoUrl.SubstituirNFSe, "");

			municipio.UrlHomologacao.Add(TipoUrl.Enviar, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.EnviarSincrono, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.CancelaNFSe, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultaNFSe, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultaNFSeRps, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultarLoteRps, "");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultarSituacao, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.ConsultarSequencialRps, "http://200.201.194.78/WsNFe2/LoteRps.jws?wsdl");
			municipio.UrlHomologacao.Add(TipoUrl.SubstituirNFSe, "");
			
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
			foreach (var municipio in ProviderManager.Municipios)
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