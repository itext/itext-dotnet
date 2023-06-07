using System.Collections.Generic;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>This class is responsible for left to right placement of flex items.</summary>
    internal class LtrFlexItemMainDirector : IFlexItemMainDirector {
        internal LtrFlexItemMainDirector() {
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<IRenderer> ApplyDirection(IList<IList<FlexItemInfo>> lines) {
            IList<IRenderer> renderers = new List<IRenderer>();
            foreach (IList<FlexItemInfo> line in lines) {
                foreach (FlexItemInfo itemInfo in line) {
                    renderers.Add(itemInfo.GetRenderer());
                }
            }
            return renderers;
        }

        public virtual void ApplyDirectionForLine<T>(IList<T> renderers) {
        }

        // Do nothing
        /// <summary><inheritDoc/></summary>
        public virtual void ApplyAlignment(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent
            , float freeSpace) {
            switch (justifyContent) {
                case JustifyContent.RIGHT:
                case JustifyContent.END:
                case JustifyContent.SELF_END:
                case JustifyContent.FLEX_END: {
                    line[0].xShift = freeSpace;
                    break;
                }

                case JustifyContent.CENTER: {
                    line[0].xShift = freeSpace / 2;
                    break;
                }

                case JustifyContent.FLEX_START:
                case JustifyContent.NORMAL:
                case JustifyContent.STRETCH:
                case JustifyContent.START:
                case JustifyContent.LEFT:
                case JustifyContent.SELF_START:
                default: {
                    break;
                }
            }
        }
        // We don't need to do anything in these cases
    }
}
