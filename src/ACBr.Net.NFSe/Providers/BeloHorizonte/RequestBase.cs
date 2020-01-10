using System;
using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [Serializable]
    [MessageContract]
    internal abstract class RequestBase
    {
        #region Properties

        [MessageBodyMember(Namespace = "", Order = 0)]
        public string nfseCabecMsg;

        [MessageBodyMember(Namespace = "", Order = 1)]
        public string nfseDadosMsg;

        #endregion Properties
    }
}