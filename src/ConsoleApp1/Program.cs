using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe;
using ACBr.Net.NFSe.Nota;
using System;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                ACBrNFSe acbrNFSe = new ACBrNFSe();

                //Salvar os arquivos
                acbrNFSe.Configuracoes.Geral.Salvar = false;
                acbrNFSe.Configuracoes.Arquivos.Salvar = false;

                //webservices
                //Configure os dados da cidade e do Certificado aqui
                acbrNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Producao;
                acbrNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;

                acbrNFSe.Configuracoes.WebServices.Usuario = "888888";//USUARIO
                acbrNFSe.Configuracoes.WebServices.Senha = "123456";//SENHA

                //adicionado rps

                var nota = acbrNFSe.NotasServico.AddNew();
                nota.Prestador.CpfCnpj = "37761587000161";
                nota.RegimeEspecialTributacao = ACBr.Net.NFSe.Nota.RegimeEspecialTributacao.SimplesNacional;
                nota.Servico.Valores.Aliquota = 2;
                nota.Servico.CodigoTributacaoMunicipio = "802";
                nota.NaturezaOperacao = NaturezaOperacao.Sigiss.TributadaNoPrestador;
                nota.Servico.Valores.ValorServicos = 29.91M;
                nota.Servico.Valores.BaseCalculo = 29.91M;
                nota.Servico.Descricao = "serviço teste";
                nota.Tomador.Tipo = TipoTomador.Sigiss.PFNI;
                nota.Tomador.DadosContato.Email = "danilo@bredas.com.br";

                //enviando
                var retorno = acbrNFSe.Enviar(1);

                //erros:
                //retorno.Erros
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
