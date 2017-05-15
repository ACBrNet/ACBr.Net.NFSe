using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{

    [ServiceContractAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", ConfigurationName = "Providers.SaoPaulo.LoteNFeSoap")]
    public interface ISaoPauloServiceClient
    {

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS", ReplyAction = "*")]
        EnvioRPSResponse EnvioRPS(EnvioRPSRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS", ReplyAction = "*")]
        EnvioLoteRPSResponse EnvioLoteRPS(EnvioLoteRPSRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/testeenvio", ReplyAction = "*")]
        TesteEnvioLoteRPSResponse TesteEnvioLoteRPS(TesteEnvioLoteRPSRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe", ReplyAction = "*")]
        CancelamentoNFeResponse CancelamentoNFe(CancelamentoNFeRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe", ReplyAction = "*")]
        ConsultaNFeResponse ConsultaNFe(ConsultaNFeRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeRecebidas", ReplyAction = "*")]
        ConsultaNFeRecebidasResponse ConsultaNFeRecebidas(ConsultaNFeRecebidasRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeEmitidas", ReplyAction = "*")]
        ConsultaNFeEmitidasResponse ConsultaNFeEmitidas(ConsultaNFeEmitidasRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote", ReplyAction = "*")]
        ConsultaLoteResponse ConsultaLote(ConsultaLoteRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote", ReplyAction = "*")]
        ConsultaInformacoesLoteResponse ConsultaInformacoesLote(ConsultaInformacoesLoteRequest request);

        [OperationContractAttribute(Action = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaCNPJ", ReplyAction = "*")]
        ConsultaCNPJResponse ConsultaCNPJ(ConsultaCNPJRequest request);
    }

}
