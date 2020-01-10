using ACBr.Net.NFSe.Providers;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    internal sealed class BeloHorizonteServiceClient : NFSeServiceClient<IBeloHorizonteServiceClient>, IABRASFClient
    {
        #region Constructors

        public BeloHorizonteServiceClient(ProviderBeloHorizonte provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new CancelarNfseRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.CancelarNfse(inValue);
            return retVal.outputXML;
        }

        public string ConsultarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarLoteRps(inValue);
            return retVal.outputXML;
        }

        public string ConsultarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarNfseRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarNfse(inValue);
            return retVal.outputXML;
        }

        public string ConsultarNfsePorFaixa(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarNfsePorFaixaRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarNfsePorFaixa(inValue);
            return retVal.outputXML;
        }

        public string ConsultarNFSePorRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarNfsePorRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarNfsePorRps(inValue);
            return retVal.outputXML;
        }

        public string ConsultarSituacaoLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new ConsultarSituacaoLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.ConsultarSituacaoLoteRps(inValue);
            return retVal.outputXML;
        }

        public string RecepcionarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new RecepcionarLoteRpsRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.RecepcionarLoteRps(inValue);
            return retVal.outputXML;
        }

        public string GerarNfse(string nfseCabecMsg, string nfseDadosMsg)
        {
            var inValue = new GerarNfseRequest(nfseCabecMsg, nfseDadosMsg);
            var retVal = Channel.GerarNfse(inValue);
            return retVal.outputXML;
        }

        #endregion Methods
    }
}