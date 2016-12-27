using iText.Kernel.Pdf.Xobject;

namespace iText.Layout.Properties {
    public class BackgroundImage {
        protected internal PdfImageXObject image;

        protected internal bool repeatX;

        protected internal bool repeatY;

        public BackgroundImage(PdfImageXObject image)
            : this(image, true, true) {
        }

        public BackgroundImage(PdfImageXObject image, bool repeatX, bool repeatY) {
            this.image = image;
            this.repeatX = repeatX;
            this.repeatY = repeatY;
        }

        public virtual PdfImageXObject GetImage() {
            return image;
        }

        public virtual bool IsRepeatX() {
            return repeatX;
        }

        public virtual bool IsRepeatY() {
            return repeatY;
        }
    }
}
