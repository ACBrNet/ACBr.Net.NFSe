using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [ServiceContract(Namespace = "http://ws.bhiss.pbh.gov.br")]
    internal interface IBeloHorizonteServiceClient
    {
        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/CancelarNfse", ReplyAction = "*")]
        CancelarNfseResponse CancelarNfse(CancelarNfseRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarLoteRps", ReplyAction = "*")]
        ConsultarLoteRpsResponse ConsultarLoteRps(ConsultarLoteRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarNfse", ReplyAction = "*")]
        ConsultarNfseResponse ConsultarNfse(ConsultarNfseRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarNfsePorFaixa", ReplyAction = "*")]
        ConsultarNfsePorFaixaResponse ConsultarNfsePorFaixa(ConsultarNfsePorFaixaRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarNfsePorRps", ReplyAction = "*")]
        ConsultarNfsePorRpsResponse ConsultarNfsePorRps(ConsultarNfsePorRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/ConsultarSituacaoLoteRps", ReplyAction = "*")]
        ConsultarSituacaoLoteRpsResponse ConsultarSituacaoLoteRps(ConsultarSituacaoLoteRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/RecepcionarLoteRps", ReplyAction = "*")]
        RecepcionarLoteRpsResponse RecepcionarLoteRps(RecepcionarLoteRpsRequest request);

        [OperationContract(Action = "http://ws.bhiss.pbh.gov.br/GerarNfse", ReplyAction = "*")]
        GerarNfseResponse GerarNfse(GerarNfseRequest request);
    }
}