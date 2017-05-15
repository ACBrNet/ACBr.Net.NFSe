using System;
using System.Security.Cryptography.X509Certificates;
using ACBr.Net.DFe.Core.Service;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    internal sealed class SaoPauloServiceClient : DFeServiceClientBase<ISaoPauloServiceClient>, ISaoPauloServiceClient

    {
        #region Constructors

        public SaoPauloServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelamentoNFe(string request)
        {
            var loteRequest = new CancelamentoNFeRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).CancelamentoNFe(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaCNPJ(string request)
        {
            var loteRequest = new ConsultaCNPJRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaCNPJ(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaInformacoesLote(string request)
        {
            var loteRequest = new ConsultaInformacoesLoteRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaInformacoesLote(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaLote(string request)
        {
            var loteRequest = new ConsultaLoteRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaLote(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaNFe(string request)
        {
            var loteRequest = new ConsultaNFeRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaNFe(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaNFeEmitidas(string request)
        {
            var loteRequest = new ConsultaNFeEmitidasRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaNFeEmitidas(loteRequest);
            return ret.RetornoXML;
        }

        public string ConsultaNFeRecebidas(string request)
        {
            var loteRequest = new ConsultaNFeRecebidasRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).ConsultaNFeRecebidas(loteRequest);
            return ret.RetornoXML;
        }

        public string EnvioLoteRPS(string request)
        {
            var loteRequest = new EnvioLoteRPSRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).EnvioLoteRPS(loteRequest);
            return ret.RetornoXML;
        }

        public string EnvioRPS(string request)
        {
            var loteRequest = new EnvioRPSRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).EnvioRPS(loteRequest);
            return ret.RetornoXML;
        }

        public string TesteEnvioLoteRPS(string request)
        {
            var loteRequest = new TesteEnvioLoteRPSRequest(1, request);
            var ret = ((ISaoPauloServiceClient)this).TesteEnvioLoteRPS(loteRequest);
            return ret.RetornoXML;
        }

        #region Interface Methods

        CancelamentoNFeResponse ISaoPauloServiceClient.CancelamentoNFe(CancelamentoNFeRequest request)
        {
            return Channel.CancelamentoNFe(request);
        }

        ConsultaCNPJResponse ISaoPauloServiceClient.ConsultaCNPJ(ConsultaCNPJRequest request)
        {
            return Channel.ConsultaCNPJ(request);
        }

        ConsultaInformacoesLoteResponse ISaoPauloServiceClient.ConsultaInformacoesLote(ConsultaInformacoesLoteRequest request)
        {
            return Channel.ConsultaInformacoesLote(request);
        }

        ConsultaLoteResponse ISaoPauloServiceClient.ConsultaLote(ConsultaLoteRequest request)
        {
            return Channel.ConsultaLote(request);
        }

        ConsultaNFeResponse ISaoPauloServiceClient.ConsultaNFe(ConsultaNFeRequest request)
        {
            return Channel.ConsultaNFe(request);
        }

        ConsultaNFeEmitidasResponse ISaoPauloServiceClient.ConsultaNFeEmitidas(ConsultaNFeEmitidasRequest request)
        {
            return Channel.ConsultaNFeEmitidas(request);
        }

        ConsultaNFeRecebidasResponse ISaoPauloServiceClient.ConsultaNFeRecebidas(ConsultaNFeRecebidasRequest request)
        {
            return Channel.ConsultaNFeRecebidas(request);
        }

        EnvioLoteRPSResponse ISaoPauloServiceClient.EnvioLoteRPS(EnvioLoteRPSRequest request)
        {
            return Channel.EnvioLoteRPS(request);
        }

        EnvioRPSResponse ISaoPauloServiceClient.EnvioRPS(EnvioRPSRequest request)
        {
            return Channel.EnvioRPS(request);
        }

        TesteEnvioLoteRPSResponse ISaoPauloServiceClient.TesteEnvioLoteRPS(TesteEnvioLoteRPSRequest request)
        {
            return Channel.TesteEnvioLoteRPS(request);
        }

        #endregion Interface Methods

        #endregion Methods

    }
}