using System;
using iText.Layout.Minmaxwidth;

namespace iText.Layout.Renderer {
    internal class MaxSumWidthHandler : AbstractWidthHandler {
        public MaxSumWidthHandler(MinMaxWidth minMaxWidth)
            : base(minMaxWidth) {
        }

        public override void UpdateMinChildWidth(float childMinWidth) {
            minMaxWidth.SetChildrenMinWidth(Math.Max(minMaxWidth.GetChildrenMinWidth(), childMinWidth));
        }

        public override void UpdateMaxChildWidth(float childMaxWidth) {
            minMaxWidth.SetChildrenMaxWidth(minMaxWidth.GetChildrenMaxWidth() + childMaxWidth);
        }
    }
}
