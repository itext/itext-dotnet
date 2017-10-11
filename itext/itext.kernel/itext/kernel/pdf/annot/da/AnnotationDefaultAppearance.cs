using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot.DA {
    /// <summary>Helper class for setting</summary>
    public class AnnotationDefaultAppearance {
        private static readonly IDictionary<StandardAnnotationFont, String> stdAnnotFontNames = new Dictionary<StandardAnnotationFont
            , String>();

        private static readonly IDictionary<ExtendedAnnotationFont, String> extAnnotFontNames = new Dictionary<ExtendedAnnotationFont
            , String>();

        static AnnotationDefaultAppearance() {
            stdAnnotFontNames.Put(StandardAnnotationFont.CourierBoldOblique, "/" + FontConstants.COURIER_BOLDOBLIQUE);
            stdAnnotFontNames.Put(StandardAnnotationFont.CourierBold, "/" + FontConstants.COURIER_BOLD);
            stdAnnotFontNames.Put(StandardAnnotationFont.CourierOblique, "/" + FontConstants.COURIER_OBLIQUE);
            stdAnnotFontNames.Put(StandardAnnotationFont.Courier, "/" + FontConstants.COURIER);
            stdAnnotFontNames.Put(StandardAnnotationFont.HelveticaBoldOblique, "/" + FontConstants.HELVETICA_BOLDOBLIQUE
                );
            stdAnnotFontNames.Put(StandardAnnotationFont.HelveticaBold, "/" + FontConstants.HELVETICA_BOLD);
            stdAnnotFontNames.Put(StandardAnnotationFont.HelveticaOblique, "/" + FontConstants.COURIER_OBLIQUE);
            stdAnnotFontNames.Put(StandardAnnotationFont.Helvetica, "/" + FontConstants.HELVETICA);
            stdAnnotFontNames.Put(StandardAnnotationFont.Symbol, "/" + FontConstants.SYMBOL);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesBoldItalic, "/" + FontConstants.TIMES_BOLDITALIC);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesBold, "/" + FontConstants.TIMES_BOLD);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesItalic, "/" + FontConstants.TIMES_ITALIC);
            stdAnnotFontNames.Put(StandardAnnotationFont.TimesRoman, "/" + FontConstants.TIMES_ROMAN);
            stdAnnotFontNames.Put(StandardAnnotationFont.ZapfDingbats, "/" + FontConstants.ZAPFDINGBATS);
            extAnnotFontNames.Put(ExtendedAnnotationFont.HYSMyeongJoMedium, "/HySm");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HYGoThicMedium, "/HyGo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HeiseiKakuGoW5, "/KaGo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HeiseiMinW3, "/KaMi");
            extAnnotFontNames.Put(ExtendedAnnotationFont.MHeiMedium, "/MHei");
            extAnnotFontNames.Put(ExtendedAnnotationFont.MSungLight, "/MSun");
            extAnnotFontNames.Put(ExtendedAnnotationFont.STSongLight, "/STSo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.MSungStdLight, "/MSun");
            extAnnotFontNames.Put(ExtendedAnnotationFont.STSongStdLight, "/STSo");
            extAnnotFontNames.Put(ExtendedAnnotationFont.HYSMyeongJoStdMedium, "/HySm");
            extAnnotFontNames.Put(ExtendedAnnotationFont.KozMinProRegular, "/KaMi");
        }

        private String colorOperand = "0 g";

        private String rawFontName = "/Helv";

        private float fontSize = 0;

        public AnnotationDefaultAppearance() {
            SetFont(StandardAnnotationFont.Helvetica);
            SetFontSize(12);
        }

        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetFont(StandardAnnotationFont font) {
            SetRawFontName(stdAnnotFontNames.Get(font));
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetFont(ExtendedAnnotationFont font) {
            SetRawFontName(extAnnotFontNames.Get(font));
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetFontSize(float fontSize) {
            this.fontSize = fontSize;
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetColor(DeviceRgb rgbColor) {
            SetColorOperand(rgbColor.GetColorValue(), "rg");
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetColor(DeviceCmyk cmykColor) {
            SetColorOperand(cmykColor.GetColorValue(), "k");
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.DA.AnnotationDefaultAppearance SetColor(DeviceGray grayColor) {
            SetColorOperand(grayColor.GetColorValue(), "g");
            return this;
        }

        public virtual PdfString ToPdfString() {
            return new PdfString(rawFontName + " " + fontSize + " Tf " + colorOperand);
        }

        private void SetColorOperand(float[] colorValues, String operand) {
            StringBuilder builder = new StringBuilder();
            foreach (float value in colorValues) {
                builder.Append(value);
                builder.Append(' ');
            }
            builder.Append(operand);
            this.colorOperand = builder.ToString();
        }

        private void SetRawFontName(String rawFontName) {
            if (rawFontName == null) {
                throw new ArgumentNullException();
            }
            this.rawFontName = rawFontName;
        }
    }
}
