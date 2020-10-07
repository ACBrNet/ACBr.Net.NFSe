using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe;
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
                acbrNFSe.Configuracoes.WebServices.Senha = "123456789";//SENHA

                //adicionado rps

                var nota = acbrNFSe.NotasServico.AddNew();
                //ccm = Configuracoes.WebServices.Usuario;
                //cnpj = Configuracoes.PrestadorPadrao.CpfCnpj

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
