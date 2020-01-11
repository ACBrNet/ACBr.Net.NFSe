using ACBr.Net.NFSe.Configuracao;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    internal sealed class ProviderBeloHorizonte : ProviderABRASF
    {
        #region Constructors

        public ProviderBeloHorizonte(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Belo Horizonte";
        }

        #endregion Constructors

        #region Methods

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            return new BeloHorizonteServiceClient(this, tipo);
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.abrasf.org.br/nfse.xsd\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            return "nfse_v20_08_2015.xsd";
        }

        #endregion Methods
    }
}