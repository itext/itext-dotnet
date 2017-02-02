using System;
using iText.Layout.Minmaxwidth;

namespace iText.Layout.Renderer {
    internal class MaxMaxWidthHandler : AbstractWidthHandler {
        public MaxMaxWidthHandler(MinMaxWidth minMaxWidth)
            : base(minMaxWidth) {
        }

        public override void UpdateMinChildWidth(float childMinWidth) {
            minMaxWidth.SetChildrenMinWidth(Math.Max(minMaxWidth.GetChildrenMinWidth(), childMinWidth));
        }

        public override void UpdateMaxChildWidth(float childMaxWidth) {
            minMaxWidth.SetChildrenMaxWidth(Math.Max(minMaxWidth.GetChildrenMaxWidth(), childMaxWidth));
        }
    }
}
