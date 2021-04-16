using System;
using ACBr.Net.DFe.Core.Common;
using FastReport.Export;

namespace ACBr.Net.NFSe.DANFSe.FastReport.OpenSource
{
    public sealed class DANFSeExportEventArgs : EventArgs
    {
        #region Constructors

        internal DANFSeExportEventArgs()
        {
        }

        #endregion Constructors

        #region Properties

        public FiltroDFeReport Filtro { get; internal set; }

        public ExportBase Export { get; internal set; }

        #endregion Properties
    }
}