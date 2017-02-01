using iText.Layout.Minmaxwidth;

namespace iText.Layout.Minmaxwidth.Handler {
    public abstract class AbstractWidthHandler {
        internal MinMaxWidth minMaxWidth;

        public AbstractWidthHandler(MinMaxWidth minMaxWidth) {
            this.minMaxWidth = minMaxWidth;
        }

        public abstract void UpdateMinChildWidth(float childMinWidth);

        public abstract void UpdateMaxChildWidth(float childMaxWidth);
    }
}
