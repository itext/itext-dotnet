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
    internal abstract class FlexColumnItemMainDirector : IFlexItemMainDirector {
//\cond DO_NOT_DOCUMENT
        internal FlexColumnItemMainDirector() {
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
        public abstract void ApplyDirectionForLine<T>(IList<T> renderers);

        public abstract void ApplyJustifyContent(IList<FlexUtil.FlexItemCalculationInfo> arg1, JustifyContent arg2
            , float arg3);
    }
//\endcond
}
