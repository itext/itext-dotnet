/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class is responsible for right to left placement of flex items.</summary>
    internal class RtlFlexItemMainDirector : IFlexItemMainDirector {
//\cond DO_NOT_DOCUMENT
        internal RtlFlexItemMainDirector() {
        }
//\endcond

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
        public virtual void ApplyJustifyContent(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent
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
//\endcond
}
