using ACBr.Net.Core.Extensions;
using System.IO;
using System.Xml.Linq;
using Xunit;

namespace ACBr.Net.NFSe.Test
{
	public class TestProviderGinfes
	{
		[Fact]
		public void TestarGeracaoRps()
		{
			var acbrNFSe = SetupACBrNFSe.Instance;

			acbrNFSe.NotasFiscais.Clear();

			var dados = new MemoryStream(Properties.Resources.Exemplo_Rps_Ginfes);
			acbrNFSe.NotasFiscais.Load(dados);

			Assert.True(acbrNFSe.NotasFiscais.Count == 1, "Erro ao carregar a Rps");

			var rpsGerada = acbrNFSe.NotasFiscais.GetXml(acbrNFSe.NotasFiscais[0]);

			dados.Position = 0;
			var xml = XDocument.Load(dados);
			var rpsOriginal = xml.AsString(true);

			Assert.True(rpsGerada == rpsOriginal, "Erro na Geração do Xml da Rps");
		}
	}
}