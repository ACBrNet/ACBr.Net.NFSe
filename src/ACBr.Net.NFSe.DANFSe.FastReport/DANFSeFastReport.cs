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
        private bool isDesign;

        #endregion Fields

        #region Events

        public event EventHandler<DANFSeEventArgs> OnGetReport;

        public event EventHandler<DANFSeExportEventArgs> OnExport;

        #endregion Events

        #region Methods

        public void ShowDesign()
        {
            isDesign = true;

            try
            {
                Imprimir();
            }
            finally
            {
                isDesign = false;
            }
        }

        public override void ImprimirPDF()
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.PDF;
                Imprimir();
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        public override void Imprimir()
        {
            using (internalReport = new Report())
            {
                PrepararImpressao();

                internalReport.RegisterData(Parent.NotasServico.ToArray(), "NotaServico");
                internalReport.Prepare();

                if (isDesign)
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
                            var evtPdf = new DANFSeExportEventArgs();
                            evtPdf.Export = new PDFExport
                            {
                                EmbeddingFonts = true,
                                ShowProgress = MostrarSetup,
                                OpenAfterExport = MostrarPreview
                            };

                            OnExport.Raise(this, evtPdf);
                            internalReport.Export(evtPdf.Export, NomeArquivo);
                            break;

                        case FiltroDFeReport.HTML:
                            var evtHtml = new DANFSeExportEventArgs();
                            evtHtml.Export = new HTMLExport()
                            {
                                Format = HTMLExportFormat.MessageHTML,
                                EmbedPictures = true,
                                Preview = MostrarPreview,
                                ShowProgress = MostrarSetup
                            };

                            OnExport.Raise(this, evtHtml);
                            internalReport.Export(evtHtml.Export, NomeArquivo);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                internalReport.Dispose();
            }

            internalReport = null;
        }

        private void PrepararImpressao()
        {
            var e = new DANFSeEventArgs(Layout);
            OnGetReport.Raise(this, e);
            if (e.FilePath.IsEmpty() || !File.Exists(e.FilePath))
            {
                MemoryStream ms;

                //ToDo: Adicionar os layouts de acordo com o provedor
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