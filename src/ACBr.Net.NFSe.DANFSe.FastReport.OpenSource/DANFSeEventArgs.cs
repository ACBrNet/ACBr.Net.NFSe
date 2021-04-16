using System;

namespace ACBr.Net.NFSe.DANFSe.FastReport.OpenSource
{
    public class DANFSeEventArgs : EventArgs
    {
        #region Constructors

        public DANFSeEventArgs(LayoutImpressao layout)
        {
            Layout = layout;
            FilePath = string.Empty;
        }

        #endregion Constructors

        #region Propriedades

        /// <summary>
        /// Retorna o tipo de arquivo necessario.
        /// </summary>
        /// <value>The tipo.</value>
        public LayoutImpressao Layout { get; internal set; }

        /// <summary>
        /// Define ou retorna o caminho para o arquivo do FastReport.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        #endregion Propriedades
    }
}