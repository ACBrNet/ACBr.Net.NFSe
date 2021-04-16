using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using FastReport;
using FastReport.Export;
using FastReport.Export.Image;
using FastReport.Utils;

namespace ACBr.Net.NFSe.DANFSe.FastReport.OpenSource
{
    public static class Extensions
    {
        #region Fields

        private const float scaleFactor = 300 / 96f;

        #endregion Fields

        #region Methods

        public static void PrintWithDialog(this Report report)
        {
            using (var dlg = new PrintDialog())
            {
                dlg.AllowSomePages = true;
                dlg.AllowSelection = true;
                dlg.UseEXDialog = true;

                if (dlg.ShowDialog() != DialogResult.OK) return;

                report.Print(dlg.PrinterSettings);
            }
        }

        public static void Print(this Report report, PrinterSettings settings = null)
        {
            var doc = report.PrepareDoc(settings);
            if (doc == null) return;

            doc.Print();
            doc.Dispose();
        }

        public static void Show(this Report report, PrinterSettings settings = null)
        {
            var doc = report.PrepareDoc(settings);
            if (doc == null) return;

            using (var preview = new PrintPreviewDialog()
            {
                Document = doc,
                StartPosition = FormStartPosition.CenterScreen,
                WindowState = FormWindowState.Maximized
            })
                preview.ShowDialog();

            doc.Dispose();
        }

        private static PrintDocument PrepareDoc(this Report report, PrinterSettings settings = null)
        {
            if (report.PreparedPages.Count < 1)
            {
                report.Prepare();
                if (report.PreparedPages.Count < 1) return null;
            }

            var page = 0;
            var exp = new ImageExport { ImageFormat = ImageExportFormat.Png, Resolution = 600 };

            var doc = new PrintDocument { DocumentName = report.Name };
            if (settings != null)
                doc.PrinterSettings = settings;

            // Ajustando o tamanho da pagina
            doc.QueryPageSettings += (sender, args) =>
            {
                var rPage = report.PreparedPages.GetPage(page);
                args.PageSettings.Landscape = rPage.Landscape;
                args.PageSettings.Margins = new Margins((int)(scaleFactor * rPage.LeftMargin * Units.HundrethsOfInch),
                    (int)(scaleFactor * rPage.RightMargin * Units.HundrethsOfInch),
                    (int)(scaleFactor * rPage.TopMargin * Units.HundrethsOfInch),
                    (int)(scaleFactor * rPage.BottomMargin * Units.HundrethsOfInch));

                args.PageSettings.PaperSize = new PaperSize("Custom", (int)(ExportUtils.GetPageWidth(rPage) * scaleFactor * Units.HundrethsOfInch),
                    (int)(ExportUtils.GetPageHeight(rPage) * scaleFactor * Units.HundrethsOfInch));
            };

            doc.PrintPage += (sender, args) =>
            {
                using (var ms = new MemoryStream())
                {
                    exp.PageRange = PageRange.PageNumbers;
                    exp.PageNumbers = $"{page + 1}";
                    exp.Export(report, ms);

                    args.Graphics.DrawImage(Image.FromStream(ms), args.PageBounds);
                }

                page++;

                args.HasMorePages = page < report.PreparedPages.Count;
            };

            doc.EndPrint += (sender, args) => page = 0;
            doc.Disposed += (sender, args) => exp?.Dispose();

            return doc;
        }

        #endregion Methods
    }
}