using ACBr.Net.DFe.Core.Common;

namespace ACBr.Net.NFSe.Test
{
    public class SetupACBrNFSe
    {
        #region Fields

        private static ACBrNFSe ginfes;
        private static ACBrNFSe sigiss;

        #endregion Fields

        #region Properties

        public static ACBrNFSe Ginfes => ginfes ?? (ginfes = GetGinfes());

        public static ACBrNFSe Sigiss => sigiss ?? (sigiss = GetSigiss());

        #endregion Properties

        #region Setup

        private static ACBrNFSe GetGinfes()
        {
            var acbrNFSe = new ACBrNFSe();

            //Salvar os arquivos
            acbrNFSe.Configuracoes.Geral.Salvar = true;
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

        private static ACBrNFSe GetSigiss()
        {
            var acbrNFSe = new ACBrNFSe();

            //Salvar os arquivos
            acbrNFSe.Configuracoes.Geral.Salvar = false;
            acbrNFSe.Configuracoes.Arquivos.Salvar = false;

            //prestador
            acbrNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = "37761587000161";

            //webservices
            //Configure os dados da cidade e do Certificado aqui
            acbrNFSe.Configuracoes.WebServices.Ambiente = DFeTipoAmbiente.Producao;
            acbrNFSe.Configuracoes.WebServices.CodigoMunicipio = 3529005;

            acbrNFSe.Configuracoes.WebServices.Usuario = "888888";//USUARIO
            acbrNFSe.Configuracoes.WebServices.Senha = "123456";//SENHA

            return acbrNFSe;
        }

        #endregion Setup
    }
}