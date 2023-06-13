using System.Collections.Generic;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal class TopToBottomFlexItemMainDirector : IFlexItemMainDirector {
        internal TopToBottomFlexItemMainDirector() {
        }

        public virtual IList<IRenderer> ApplyDirection(IList<IList<FlexItemInfo>> lines) {
            // TODO DEVSIX-7595 Shall be implemented in the scope of this ticket
            return null;
        }

        public virtual void ApplyDirectionForLine<T>(IList<T> renderers) {
        }

        // TODO DEVSIX-7595 Shall be implemented in the scope of this ticket
        public virtual void ApplyJustifyContent(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent
            , float freeSpace) {
            switch (justifyContent) {
                case JustifyContent.END:
                case JustifyContent.SELF_END:
                case JustifyContent.FLEX_END: {
                    line[0].yShift = freeSpace;
                    break;
                }

                case JustifyContent.CENTER: {
                    line[0].yShift = freeSpace / 2;
                    break;
                }

                case JustifyContent.FLEX_START:
                case JustifyContent.NORMAL:
                case JustifyContent.STRETCH:
                case JustifyContent.START:
                case JustifyContent.LEFT:
                case JustifyContent.RIGHT:
                case JustifyContent.SELF_START:
                default: {
                    break;
                }
            }
        }
        // We don't need to do anything in these cases
    }
}
