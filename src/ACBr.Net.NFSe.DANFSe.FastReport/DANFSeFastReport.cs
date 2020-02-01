using System;
using System.IO;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Common;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Pdf;

namespace ACBr.Net.NFSe.DANFSe.FastReport
{
    public sealed class DANFSeFastReport : ACBrDANFSeBase
    {
        #region Fields

        private Report internalReport;

        #endregion Fields

        #region Events

        public event EventHandler<DANFSeEventArgs> OnGetReport;

        #endregion Events

        #region Propriedades

        public bool ShowDesign { get; set; }

        #endregion Propriedades

        #region Methods

        public override void Imprimir()
        {
            ImprimirInterno();
        }

        public override void ImprimirPDF()
        {
            Filtro = FiltroDFeReport.PDF;
            ImprimirInterno();
        }

        private void ImprimirInterno()
        {
            PrepararImpressao();
            internalReport.RegisterData(Parent.NotasFiscais.ToArray(), "NotaFiscal");

            internalReport.Prepare();

            if (ShowDesign)
            {
                internalReport.Design();
            }
            else
            {
                switch (Filtro)
                {
                    case FiltroDFeReport.Nenhum:
                        if (MostrarPreview)
                            internalReport.Show();
                        else
                            internalReport.Print();
                        break;

                    case FiltroDFeReport.PDF:
                        var pdfExport = new PDFExport
                        {
                            EmbeddingFonts = true,
                            ShowProgress = MostrarSetup,
                            OpenAfterExport = MostrarPreview
                        };

                        internalReport.Export(pdfExport, NomeArquivo);
                        break;

                    case FiltroDFeReport.HTML:
                        var htmlExport = new HTMLExport
                        {
                            Format = HTMLExportFormat.MessageHTML,
                            EmbedPictures = true,
                            Preview = MostrarPreview,
                            ShowProgress = MostrarSetup
                        };

                        internalReport.Export(htmlExport, NomeArquivo);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            internalReport.Dispose();
            internalReport = null;
        }

        private void PrepararImpressao()
        {
            internalReport = new Report();

            var e = new DANFSeEventArgs(Layout);
            OnGetReport.Raise(this, e);
            if (e.FilePath.IsEmpty() || !File.Exists(e.FilePath))
            {
                MemoryStream ms;
                switch (Layout)
                {
                    case LayoutImpressao.ABRASF2:
                        ms = new MemoryStream(Properties.Resources.DANFSe);
                        break;

                    case LayoutImpressao.DSF:
                        ms = new MemoryStream(Properties.Resources.DANFSe);
                        break;

                    case LayoutImpressao.Ginfes:
                        ms = new MemoryStream(Properties.Resources.DANFSe);
                        break;

                    case LayoutImpressao.ABRASF:
                        ms = new MemoryStream(Properties.Resources.DANFSe);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                internalReport.Load(ms);
            }
            else
            {
                internalReport.Load(e.FilePath);
            }

            internalReport.SetParameterValue("Logo", Logo.ToByteArray());
            internalReport.SetParameterValue("LogoPrefeitura", LogoPrefeitura.ToByteArray());
            internalReport.SetParameterValue("MunicipioPrestador", Parent.Configuracoes.WebServices.Municipio);
            internalReport.SetParameterValue("Ambiente", (int)Parent.Configuracoes.WebServices.Ambiente);
            internalReport.SetParameterValue("SoftwareHouse", SoftwareHouse);
            internalReport.SetParameterValue("Site", Site);

            internalReport.PrintSettings.Copies = NumeroCopias;
            internalReport.PrintSettings.Printer = Impressora;
            internalReport.PrintSettings.ShowDialog = MostrarSetup;
        }

        #endregion Methods

        #region Overrides

        protected override void OnInitialize()
        {
            //
        }

        protected override void OnDisposing()
        {
            //
        }

        #endregion Overrides
    }
}