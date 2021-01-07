using System.ComponentModel;
using ACBr.Net.Core.Generics;

namespace ACBr.Net.NFSe.Nota
{
    public sealed class DadosToken : GenericClone<DadosToken>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DadosToken"/> class.
        /// </summary>
        internal DadosToken()
        {
            CodigoToken = string.Empty;
            CodigoContribuinte = string.Empty;
        }

        #endregion Constructor

        #region Propriedades

        /// <summary>
        /// Gets or sets the CodigoToken.
        /// </summary>
        /// <value>The CodigoToken.</value>
        public string CodigoToken { get; set; }

        /// <summary>
        /// Gets or sets the CodigoContribuinte.
        /// </summary>
        /// <value>The CodigoContribuinte.</value>
        public string CodigoContribuinte { get; set; }

        #endregion Propriedades
    }
}