using ACBr.Net.Core.Exceptions;
using ACBr.Net.NFSe.Providers;
using System;
using System.Windows.Forms;

namespace ACBr.Net.NFSe.Demo
{
	public partial class FormEdtMunicipio : Form
	{
		#region Fields

		private MunicipioNFSe target;

		#endregion Fields

		#region Constructors

		public FormEdtMunicipio()
		{
			InitializeComponent();
		}

		#endregion Constructors

		#region Methods

		#region Event Handlers

		private void btnCancelar_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void btnSalvar_Click(object sender, EventArgs e)
		{
			Salvar();
			DialogResult = DialogResult.OK;
		}

		#endregion Event Handlers

		public static DialogResult Editar(MunicipioNFSe municipio)
		{
			Guard.Against<ArgumentNullException>(municipio == null, nameof(municipio));

			using (var form = new FormEdtMunicipio())
			{
				form.target = municipio;
				form.LoadTarget();

				return form.ShowDialog();
			}
		}

		private void LoadTarget()
		{
			txtMunicipio.Text = target.Nome;
			txtUF.Text = target.UF;
			nudTamIM.Value = target.TamanhoIM;
			nudCodIBGE.Value = target.Codigo;
			nudCodSiafi.Value = target.CodigoSiafi;
			cmbProvedor.Text = target.Provedor;

			txtPEnviar.Text = target.UrlProducao[TipoUrl.Enviar];
			txtPEnviarSincrono.Text = target.UrlProducao[TipoUrl.EnviarSincrono];
			txtPCancelaNFSe.Text = target.UrlProducao[TipoUrl.CancelaNFSe];
			txtPConsultaNFSe.Text = target.UrlProducao[TipoUrl.ConsultaNFSe];
			txtPConsultaNFSeRps.Text = target.UrlProducao[TipoUrl.ConsultaNFSeRps];
			txtPConsultrLoteRps.Text = target.UrlProducao[TipoUrl.ConsultarLoteRps];
			txtPConsultarSituacao.Text = target.UrlProducao[TipoUrl.ConsultarSituacao];
			txtPConsultarSequencialRps.Text = target.UrlProducao[TipoUrl.ConsultarSequencialRps];
			txtPSubstituirNFSe.Text = target.UrlProducao[TipoUrl.SubstituirNFSe];

			txtHEnviar.Text = target.UrlHomologacao[TipoUrl.Enviar];
			txtHEnviarSincrono.Text = target.UrlHomologacao[TipoUrl.EnviarSincrono];
			txtHCancelaNFSe.Text = target.UrlHomologacao[TipoUrl.CancelaNFSe];
			txtHConsultaNFSe.Text = target.UrlHomologacao[TipoUrl.ConsultaNFSe];
			txtHConsultaNFSeRps.Text = target.UrlHomologacao[TipoUrl.ConsultaNFSeRps];
			txtHConsultrLoteRps.Text = target.UrlHomologacao[TipoUrl.ConsultarLoteRps];
			txtHConsultarSituacao.Text = target.UrlHomologacao[TipoUrl.ConsultarSituacao];
			txtHConsultarSequencialRps.Text = target.UrlHomologacao[TipoUrl.ConsultarSequencialRps];
			txtHSubstituirNFSe.Text = target.UrlHomologacao[TipoUrl.SubstituirNFSe];
		}

		private void Salvar()
		{
			target.Nome = txtMunicipio.Text;
			target.UF = txtUF.Text;
			target.TamanhoIM = (int)nudTamIM.Value;
			target.Codigo = (int)nudCodIBGE.Value;
			target.CodigoSiafi = (int)nudCodSiafi.Value;
			target.Provedor = cmbProvedor.Text;

			target.UrlProducao[TipoUrl.Enviar] = txtPEnviar.Text;
			target.UrlProducao[TipoUrl.EnviarSincrono] = txtPEnviarSincrono.Text;
			target.UrlProducao[TipoUrl.CancelaNFSe] = txtPCancelaNFSe.Text;
			target.UrlProducao[TipoUrl.ConsultaNFSe] = txtPConsultaNFSe.Text;
			target.UrlProducao[TipoUrl.ConsultaNFSeRps] = txtPConsultaNFSeRps.Text;
			target.UrlProducao[TipoUrl.ConsultarLoteRps] = txtPConsultrLoteRps.Text;
			target.UrlProducao[TipoUrl.ConsultarSituacao] = txtPConsultarSituacao.Text;
			target.UrlProducao[TipoUrl.ConsultarSequencialRps] = txtPConsultarSequencialRps.Text;
			target.UrlProducao[TipoUrl.SubstituirNFSe] = txtPSubstituirNFSe.Text;

			target.UrlHomologacao[TipoUrl.Enviar] = txtHEnviar.Text;
			target.UrlHomologacao[TipoUrl.EnviarSincrono] = txtHEnviarSincrono.Text;
			target.UrlHomologacao[TipoUrl.CancelaNFSe] = txtHCancelaNFSe.Text;
			target.UrlHomologacao[TipoUrl.ConsultaNFSe] = txtHConsultaNFSe.Text;
			target.UrlHomologacao[TipoUrl.ConsultaNFSeRps] = txtHConsultaNFSeRps.Text;
			target.UrlHomologacao[TipoUrl.ConsultarLoteRps] = txtHConsultrLoteRps.Text;
			target.UrlHomologacao[TipoUrl.ConsultarSituacao] = txtHConsultarSituacao.Text;
			target.UrlHomologacao[TipoUrl.ConsultarSequencialRps] = txtHConsultarSequencialRps.Text;
			target.UrlHomologacao[TipoUrl.SubstituirNFSe] = txtHSubstituirNFSe.Text;
		}

		#endregion Methods
	}
}