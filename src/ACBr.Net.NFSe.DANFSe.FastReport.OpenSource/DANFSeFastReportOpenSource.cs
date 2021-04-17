// ***********************************************************************
// Assembly         : ACBr.Net.NFSe.DANFSe.FastReport.OpenSource
// Author           : Rafael Dias
// Created          : 01-31-2016
//
// Last Modified By : Rafael Dias
// Last Modified On : 07-05-2018
// ***********************************************************************
// <copyright file="DANFSeFastReportOpenSource.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using ACBr.Net.Core;
using ACBr.Net.Core.Extensions;
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

        /// <inheritdoc />
        public override void Imprimir()
        {
            Imprimir(null);
        }

        /// <inheritdoc />
        public override void ImprimirPDF()
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.PDF;
                Imprimir(null);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        /// <inheritdoc />
        public override void ImprimirPDF(Stream stream)
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.PDF;
                Imprimir(stream);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        /// <inheritdoc />
        public override void ImprimirHTML()
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.HTML;
                Imprimir(null);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        /// <inheritdoc />
        public override void ImprimirHTML(Stream stream)
        {
            var oldFiltro = Filtro;

            try
            {
                Filtro = FiltroDFeReport.HTML;
                Imprimir(stream);
            }
            finally
            {
                Filtro = oldFiltro;
            }
        }

        private void Imprimir(Stream stream)
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
                            if (MostrarPreview)
                                internalReport.Show();
                            else if (MostrarSetup)
                                internalReport.PrintWithDialog();
                            else
                                internalReport.Print(settings);
                            break;

                        case FiltroDFeReport.PDF:
                            var evtPdf = new DANFSeExportEventArgs();
                            evtPdf.Filtro = Filtro;
                            evtPdf.Export = new PDFSimpleExport()
                            {
                                ImageDpi = 600,
                                ShowProgress = MostrarSetup,
                                OpenAfterExport = MostrarPreview
                            };

                            OnExport.Raise(this, evtPdf);
                            if (stream.IsNull())
                                internalReport.Export(evtPdf.Export, NomeArquivo);
                            else
                                internalReport.Export(evtPdf.Export, stream);
                            break;

                        case FiltroDFeReport.HTML:
                            var evtHtml = new DANFSeExportEventArgs();
                            evtHtml.Filtro = Filtro;
                            evtHtml.Export = new HTMLExport()
                            {
                                Format = HTMLExportFormat.HTML,
                                EmbedPictures = true,
                                Preview = MostrarPreview,
                                ShowProgress = MostrarSetup
                            };

                            OnExport.Raise(this, evtHtml);
                            if (stream.IsNull())
                                internalReport.Export(evtHtml.Export, NomeArquivo);
                            else
                                internalReport.Export(evtHtml.Export, stream);
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

#if NETFULL
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