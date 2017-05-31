using System;
using iText.Kernel.Geom;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class acts as a default implementation of IPdfTextLocation</summary>
    public class DefaultPdfTextLocation : IPdfTextLocation {
        private int pageNr;

        private Rectangle rectangle;

        private String text;

        public DefaultPdfTextLocation(int pageNr, Rectangle rect, String text) {
            this.pageNr = pageNr;
            this.rectangle = rect;
            this.text = text;
        }

        public virtual Rectangle GetRectangle() {
            return rectangle;
        }

        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetRectangle(Rectangle rectangle
            ) {
            this.rectangle = rectangle;
            return this;
        }

        public virtual String GetText() {
            return text;
        }

        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetText(String text) {
            this.text = text;
            return this;
        }

        public virtual int GetPageNumber() {
            return pageNr;
        }

        public virtual iText.Kernel.Pdf.Canvas.Parser.Listener.DefaultPdfTextLocation SetPageNr(int pageNr) {
            this.pageNr = pageNr;
            return this;
        }
    }
}
