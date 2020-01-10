using System;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
    internal interface IGinfesServiceClient : IDisposable
    {
        string CancelarNfse(string cabecalho, string dados);

        string ConsultarLoteRps(string cabecalho, string dados);

        string ConsultarNfse(string cabecalho, string dados);

        string ConsultarNfsePorRps(string cabecalho, string dados);

        string ConsultarSituacao(string cabecalho, string dados);

        string RecepcionarLoteRps(string cabecalho, string dados);
    }
}