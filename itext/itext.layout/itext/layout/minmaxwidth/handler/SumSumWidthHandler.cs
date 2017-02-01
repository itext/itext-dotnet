using iText.Layout.Minmaxwidth;

namespace iText.Layout.Minmaxwidth.Handler {
    public class SumSumWidthHandler : AbstractWidthHandler {
        public SumSumWidthHandler(MinMaxWidth minMaxWidth)
            : base(minMaxWidth) {
        }

        public override void UpdateMinChildWidth(float childMinWidth) {
            minMaxWidth.SetChildrenMinWidth(minMaxWidth.GetChildrenMinWidth() + childMinWidth);
        }

        public override void UpdateMaxChildWidth(float childMaxWidth) {
            minMaxWidth.SetChildrenMaxWidth(minMaxWidth.GetChildrenMaxWidth() + childMaxWidth);
        }
    }
}
