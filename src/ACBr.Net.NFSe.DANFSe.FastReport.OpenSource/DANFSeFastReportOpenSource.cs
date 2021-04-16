using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using ACBr.Net.Core;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Common;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.PdfSimple;

namespace ACBr.Net.NFSe.DANFSe.FastReport.OpenSource
{
    [TypeConverter(typeof(ACBrExpandableObjectConverter))]
    public sealed class DANFSeFastReportOpenSource : ACBrDANFSeBase
    {
        #region Fields

        private Report internalReport;
        private PrinterSettings settings;

        #endregion Fields

        #region Events

        public event EventHandler<DANFSeEventArgs> OnGetReport;

        public event EventHandler<DANFSeExportEventArgs> OnExport;

        #endregion Events

        #region Methods

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
            try
            {
                using (internalReport = new Report())
                {
                    PrepararImpressao();

                    internalReport.RegisterData(Parent.NotasServico.ToArray(), "NotaServico");
                    internalReport.Prepare();

                    switch (Filtro)
                    {
                        case FiltroDFeReport.Nenhum:
#if NETFRAMEWORK
                            if (MostrarPreview)
                                internalReport.Show();
                            else if (MostrarSetup)
                                internalReport.PrintWithDialog();
                            else
                                internalReport.Print(settings);
#else
                            throw new ACBrDFeException("Metodo não suportado nesta plataforma.");
#endif
                            break;

                        case FiltroDFeReport.PDF:
                            var evtPdf = new DANFSeExportEventArgs();
                            evtPdf.Export = new PDFSimpleExport()
                            {
                                ImageDpi = 600,
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
            }
            finally
            {
                internalReport = null;
                settings = null;
            }
        }

        private void PrepararImpressao()
        {
            var e = new DANFSeEventArgs(Layout);
            OnGetReport.Raise(this, e);
            if (e.FilePath.IsEmpty() || !File.Exists(e.FilePath))
            {
                //ToDo: Adicionar os layouts de acordo com o provedor
                var assembly = Assembly.GetExecutingAssembly();

                Stream ms;
                switch (Layout)
                {
                    case LayoutImpressao.ABRASF2:
                        ms = assembly.GetManifestResourceStream("ACBr.Net.NFSe.DANFSe.FastReport.OpenSource.Report.DANFSe.frx");
                        break;

                    case LayoutImpressao.DSF:
                        ms = assembly.GetManifestResourceStream("ACBr.Net.NFSe.DANFSe.FastReport.OpenSource.Report.DANFSe.frx");
                        break;

                    case LayoutImpressao.Ginfes:
                        ms = assembly.GetManifestResourceStream("ACBr.Net.NFSe.DANFSe.FastReport.OpenSource.Report.DANFSe.frx");
                        break;

                    case LayoutImpressao.ABRASF:
                        ms = assembly.GetManifestResourceStream("ACBr.Net.NFSe.DANFSe.FastReport.OpenSource.Report.DANFSe.frx");
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

#if NETFRAMEWORK
            internalReport.SetParameterValue("Logo", Logo.ToByteArray());
            internalReport.SetParameterValue("LogoPrefeitura", LogoPrefeitura.ToByteArray());
#else
            internalReport.SetParameterValue("Logo", Logo);
            internalReport.SetParameterValue("LogoPrefeitura", LogoPrefeitura);
#endif
            internalReport.SetParameterValue("MunicipioPrestador", Parent.Configuracoes.WebServices.Municipio);
            internalReport.SetParameterValue("Ambiente", (int)Parent.Configuracoes.WebServices.Ambiente);
            internalReport.SetParameterValue("SoftwareHouse", SoftwareHouse);
            internalReport.SetParameterValue("Site", Site);

            settings = new PrinterSettings { Copies = (short)NumeroCopias, PrinterName = Impressora };
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