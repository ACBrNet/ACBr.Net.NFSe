using System;
using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [Serializable]
    [MessageContract]
    internal abstract class ResponseBase
    {
        #region Properties

        [MessageBodyMember(Namespace = "", Order = 0)]
        public string outputXML;

        #endregion Properties
    }
}