using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var acbrNFSe = new ACBrNFSe();

            //Salvar os arquivos
            acbrNFSe.Configuracoes.Geral.Salvar = false;
            acbrNFSe.Configuracoes.Arquivos.Salvar = false;

            //webservices
            //Configure os dados da cidade e do Certificado aqui
            acbrNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Homologacao;
            acbrNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;

            acbrNFSe.Configuracoes.WebServices.Usuario = "888888";//USUARIO
            acbrNFSe.Configuracoes.WebServices.Senha = "123456789";//SENHA

            acbrNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "03514896000115";
            acbrNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = "85841";
        }
    }
}
