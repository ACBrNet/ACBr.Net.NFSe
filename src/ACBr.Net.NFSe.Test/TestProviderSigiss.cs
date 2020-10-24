using ACBr.Net.NFSe.Nota;
using Xunit;

namespace ACBr.Net.NFSe.Test
{
    public class TestProviderSigiss
    {
        [Fact]
        public void EmissaoNota()
        {
            var acbrNFSe = SetupACBrNFSe.Sigiss;

            //adicionado rps
            var nota = acbrNFSe.NotasServico.AddNew();
            nota.Prestador.CpfCnpj = "37761587000161";
            nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            nota.Servico.Valores.Aliquota = 2;
            nota.Servico.CodigoTributacaoMunicipio = "802";
            nota.NaturezaOperacao = NaturezaOperacao.Sigiss.TributadaNoPrestador;
            nota.Servico.Valores.ValorServicos = 29.91M;
            nota.Servico.Valores.BaseCalculo = 29.91M;
            nota.Servico.Descricao = "serviço teste";
            nota.Tomador.Tipo = TipoTomador.Sigiss.PFNI;
            nota.Tomador.DadosContato.Email = "a@a.com";

            //enviando
            var retorno = acbrNFSe.Enviar(0);

            Assert.True(retorno.Sucesso);
        }

        [Fact]
        public void CancelarNota()
        {
            var acbrNFSe = SetupACBrNFSe.Sigiss;

            //enviando requisicao de cancelamento
            var retorno = acbrNFSe.CancelarNFSe("a@a.com", "7125", "motivo teste testetestetesteteste");
            Assert.True(retorno.Sucesso);
        }
    }
}