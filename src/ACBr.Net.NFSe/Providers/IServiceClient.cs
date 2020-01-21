using System;

namespace ACBr.Net.NFSe.Providers
{
    public interface IServiceClient : IDisposable
    {
        #region Properties

        string EnvelopeEnvio { get; }

        string EnvelopeRetorno { get; }

        #endregion Properties
    }
}