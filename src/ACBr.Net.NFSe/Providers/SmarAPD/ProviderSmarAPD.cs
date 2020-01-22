using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers.SmarAPD
{
    internal sealed class ProviderSmarAPD : ProviderABRASF204
    {
        #region Constructors

        public ProviderSmarAPD(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "SmarAPD";
        }

        #endregion Constructors

        #region Methods

        protected override IABRASF2Client GetClient(TipoUrl tipo)
        {
            return new SmarAPDServiceClient(this, tipo);
        }

        #endregion Methods
    }
}
