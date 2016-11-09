using ACBr.Net.DFe.Core.Common;

namespace ACBr.Net.NFSe.Test
{
	public class SetupACBrNFSe
	{
		#region Setup

		private static ACBrNFSe instance;
		public static ACBrNFSe Instance => instance ?? (instance = GetACBrNFSe());

		// ReSharper disable once InconsistentNaming
		private static ACBrNFSe GetACBrNFSe()
		{
			var acbrNFSe = new ACBrNFSe();

			//Salvar os arquivos
			acbrNFSe.Configuracoes.Geral.Salvar = true;
			acbrNFSe.Configuracoes.Geral.AtualizarXmlCancelado = true;
			acbrNFSe.Configuracoes.Arquivos.Salvar = true;

			//webservices
			//Configure os dados da cidade e do Certificado aqui
			acbrNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
			acbrNFSe.Configuracoes.WebServices.CodigoMunicipio = 3543402;

			acbrNFSe.Configuracoes.Certificados.Certificado = "4E009FA5F9CABB8F";
			acbrNFSe.Configuracoes.Certificados.Senha = "";

			acbrNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "03514896000115";
			acbrNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = "85841";

			return acbrNFSe;
		}

		#endregion Setup
	}
}