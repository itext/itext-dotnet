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
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class is responsible for left to right placement of flex items.</summary>
    internal class LtrFlexItemMainDirector : IFlexItemMainDirector {
//\cond DO_NOT_DOCUMENT
        internal LtrFlexItemMainDirector() {
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

        public virtual void ApplyDirectionForLine<T>(IList<T> renderers) {
        }

        // Do nothing
        /// <summary><inheritDoc/></summary>
        public virtual void ApplyJustifyContent(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent
            , float freeSpace) {
            if (freeSpace < 0 && (JustifyContent.SPACE_AROUND == justifyContent || JustifyContent.SPACE_BETWEEN == justifyContent
                 || JustifyContent.SPACE_EVENLY == justifyContent)) {
                return;
            }
            float space;
            switch (justifyContent) {
                case JustifyContent.RIGHT:
                case JustifyContent.END:
                case JustifyContent.FLEX_END: {
                    line[0].xShift = freeSpace;
                    break;
                }

                case JustifyContent.CENTER: {
                    line[0].xShift = freeSpace / 2;
                    break;
                }

                case JustifyContent.SPACE_BETWEEN: {
                    space = freeSpace / (line.Count - 1);
                    for (int i = 1; i < line.Count; i++) {
                        FlexUtil.FlexItemCalculationInfo item = line[i];
                        item.xShift = space;
                    }
                    break;
                }

                case JustifyContent.SPACE_AROUND: {
                    space = freeSpace / (line.Count * 2);
                    for (int i = 0; i < line.Count; i++) {
                        FlexUtil.FlexItemCalculationInfo item = line[i];
                        item.xShift = i == 0 ? space : space * 2;
                    }
                    break;
                }

                case JustifyContent.SPACE_EVENLY: {
                    space = freeSpace / (line.Count + 1);
                    foreach (FlexUtil.FlexItemCalculationInfo item in line) {
                        item.xShift = space;
                    }
                    break;
                }

                case JustifyContent.FLEX_START:
                case JustifyContent.STRETCH:
                case JustifyContent.NORMAL:
                case JustifyContent.START:
                case JustifyContent.LEFT:
                default: {
                    break;
                }
            }
        }
        // stretch in flexbox behaves as flex-start, see https://drafts.csswg.org/css-align/#distribution-flex
        // We don't need to do anything in these cases
    }
//\endcond
}
