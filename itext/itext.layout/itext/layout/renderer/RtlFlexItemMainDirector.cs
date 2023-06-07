using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>This class is responsible for right to left placement of flex items.</summary>
    internal class RtlFlexItemMainDirector : IFlexItemMainDirector {
        internal RtlFlexItemMainDirector() {
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<IRenderer> ApplyDirection(IList<IList<FlexItemInfo>> lines) {
            IList<IRenderer> renderers = new List<IRenderer>();
            foreach (IList<FlexItemInfo> line in lines) {
                ApplyDirectionForLine(line);
                foreach (FlexItemInfo itemInfo in line) {
                    renderers.Add(itemInfo.GetRenderer());
                }
            }
            return renderers;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ApplyDirectionForLine<T>(IList<T> renderers) {
            JavaCollectionsUtil.Reverse(renderers);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ApplyAlignment(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent
            , float freeSpace) {
            switch (justifyContent) {
                case JustifyContent.RIGHT:
                case JustifyContent.END:
                case JustifyContent.SELF_END:
                case JustifyContent.FLEX_START: {
                    line[line.Count - 1].xShift = freeSpace;
                    break;
                }

                case JustifyContent.CENTER: {
                    line[line.Count - 1].xShift = freeSpace / 2;
                    break;
                }

                case JustifyContent.FLEX_END:
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
