using ACBr.Net.DFe.Core.Common;
using System.IO;
using Xunit;

namespace ACBr.Net.NFSe.Test
{
	public class TestProviderGinfes
	{
		#region Setup

		private ACBrNFSe GetACBrNFSe()
		{
			var acbrNFSe = new ACBrNFSe();

			//Salvar os arquivos
			acbrNFSe.Configuracoes.Geral.Salvar = true;
			acbrNFSe.Configuracoes.Geral.AtualizarXmlCancelado = true;
			acbrNFSe.Configuracoes.Arquivos.Salvar = true;

			//webservices
			//Configure os dados da cidade e do Certificado aqui
			acbrNFSe.Configuracoes.WebServices.Ambiente = TipoAmbiente.Homologacao;
			acbrNFSe.Configuracoes.WebServices.CodMunicipio = 3543402;

			acbrNFSe.Configuracoes.Certificados.Certificado = "4E009FA5F9CABB8F";
			acbrNFSe.Configuracoes.Certificados.Senha = "";

			acbrNFSe.Configuracoes.PrestadorPadrao.CPFCNPJ = "03514896000115";
			acbrNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = "85841";

			return acbrNFSe;
		}

		#endregion Setup

		[Fact]
		public void TestarGeracaoRPS()
		{
			var acbrNFSe = GetACBrNFSe();

			var dados = new MemoryStream(Properties.Resources.Ginfes);
			acbrNFSe.NotasFiscais.Load(dados);
			var ret = acbrNFSe.Enviar(10, false);

			Assert.True(ret.Sucesso, "Erro nos dados de configuração do NFSe");
		}
	}
}