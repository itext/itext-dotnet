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
    internal class BottomToTopFlexItemMainDirector : FlexColumnItemMainDirector {
//\cond DO_NOT_DOCUMENT
        internal BottomToTopFlexItemMainDirector() {
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public override void ApplyDirectionForLine<T>(IList<T> renderers) {
            JavaCollectionsUtil.Reverse(renderers);
        }

        public override void ApplyJustifyContent(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent
            , float freeSpace) {
            if (freeSpace < 0 && (JustifyContent.SPACE_AROUND == justifyContent || JustifyContent.SPACE_BETWEEN == justifyContent
                 || JustifyContent.SPACE_EVENLY == justifyContent)) {
                return;
            }
            float space;
            switch (justifyContent) {
                case JustifyContent.END:
                case JustifyContent.START:
                case JustifyContent.STRETCH:
                case JustifyContent.NORMAL:
                case JustifyContent.FLEX_START: {
                    // stretch in flexbox behaves as flex-start, see https://drafts.csswg.org/css-align/#distribution-flex
                    line[line.Count - 1].yShift = freeSpace;
                    break;
                }

                case JustifyContent.CENTER: {
                    line[line.Count - 1].yShift = freeSpace / 2;
                    break;
                }

                case JustifyContent.SPACE_BETWEEN: {
                    if (line.Count == 1) {
                        line[0].yShift = freeSpace;
                    }
                    else {
                        space = freeSpace / (line.Count - 1);
                        for (int i = 0; i < line.Count - 1; i++) {
                            FlexUtil.FlexItemCalculationInfo item = line[i];
                            item.yShift = space;
                        }
                    }
                    break;
                }

                case JustifyContent.SPACE_AROUND: {
                    space = freeSpace / (line.Count * 2);
                    for (int i = 0; i < line.Count; i++) {
                        FlexUtil.FlexItemCalculationInfo item = line[i];
                        item.yShift = i == (line.Count - 1) ? space : space * 2;
                    }
                    break;
                }

                case JustifyContent.SPACE_EVENLY: {
                    space = freeSpace / (line.Count + 1);
                    foreach (FlexUtil.FlexItemCalculationInfo item in line) {
                        item.yShift = space;
                    }
                    break;
                }

                case JustifyContent.FLEX_END:
                case JustifyContent.LEFT:
                case JustifyContent.RIGHT:
                default: {
                    break;
                }
            }
        }
        // We don't need to do anything in these cases
    }
//\endcond
}
