using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers
{
    [ServiceContract(Namespace = "http://homologacao.ginfes.com.br")]
    internal interface IGinfesHomService
    {
        [OperationContract(Action = "", ReplyAction = "*")]
        [DataContractFormat(Style = OperationFormatStyle.Rpc)]
        [return: MessageParameter(Name = "return")]
        string CancelarNfseV3([MessageParameter(Name = "arg0")]string cabecalho, [MessageParameter(Name = "arg1")]string dados);

        [OperationContract(Action = "", ReplyAction = "*")]
        [DataContractFormat(Style = OperationFormatStyle.Rpc)]
        [return: MessageParameter(Name = "return")]
        string ConsultarLoteRpsV3([MessageParameter(Name = "arg0")]string cabecalho, [MessageParameter(Name = "arg1")]string dados);

        [OperationContract(Action = "", ReplyAction = "*")]
        [DataContractFormat(Style = OperationFormatStyle.Rpc)]
        [return: MessageParameter(Name = "return")]
        string ConsultarNfseV3([MessageParameter(Name = "arg0")]string cabecalho, [MessageParameter(Name = "arg1")]string dados);

        [OperationContract(Action = "", ReplyAction = "*")]
        [DataContractFormat(Style = OperationFormatStyle.Rpc)]
        [return: MessageParameter(Name = "return")]
        string ConsultarNfsePorRpsV3([MessageParameter(Name = "arg0")]string cabecalho, [MessageParameter(Name = "arg1")]string dados);

        [OperationContract(Action = "", ReplyAction = "*")]
        [DataContractFormat(Style = OperationFormatStyle.Rpc)]
        [return: MessageParameter(Name = "return")]
        string ConsultarSituacaoLoteRpsV3([MessageParameter(Name = "arg0")]string cabecalho, [MessageParameter(Name = "arg1")]string dados);

        [OperationContract(Action = "", ReplyAction = "*")]
        [DataContractFormat(Style = OperationFormatStyle.Rpc)]
        [return: MessageParameter(Name = "return")]
        string RecepcionarLoteRpsV3([MessageParameter(Name = "arg0")]string cabecalho, [MessageParameter(Name = "arg1")]string arg1);
    }
}