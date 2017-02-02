using iText.Layout.Minmaxwidth;

namespace iText.Layout.Renderer {
    internal abstract class AbstractWidthHandler {
        internal MinMaxWidth minMaxWidth;

        public AbstractWidthHandler(MinMaxWidth minMaxWidth) {
            this.minMaxWidth = minMaxWidth;
        }

        public abstract void UpdateMinChildWidth(float childMinWidth);

        public abstract void UpdateMaxChildWidth(float childMaxWidth);

        public virtual void UpdateMinMaxWidth(MinMaxWidth minMaxWidth) {
            if (minMaxWidth != null) {
                UpdateMaxChildWidth(minMaxWidth.GetMaxWidth());
                UpdateMinChildWidth(minMaxWidth.GetMinWidth());
            }
        }
    }
}
