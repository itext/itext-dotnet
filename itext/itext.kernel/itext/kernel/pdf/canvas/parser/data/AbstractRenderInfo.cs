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
using System;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    public class AbstractRenderInfo : IEventData {
        protected internal CanvasGraphicsState gs;

        private bool graphicsStateIsPreserved;

        public AbstractRenderInfo(CanvasGraphicsState gs) {
            this.gs = gs;
        }

        public virtual CanvasGraphicsState GetGraphicsState() {
            CheckGraphicsState();
            return graphicsStateIsPreserved ? gs : new CanvasGraphicsState(gs);
        }

        public virtual bool IsGraphicsStatePreserved() {
            return graphicsStateIsPreserved;
        }

        public virtual void PreserveGraphicsState() {
            CheckGraphicsState();
            this.graphicsStateIsPreserved = true;
            gs = new CanvasGraphicsState(gs);
        }

        public virtual void ReleaseGraphicsState() {
            if (!graphicsStateIsPreserved) {
                gs = null;
            }
        }

        // check if graphics state was released
        protected internal virtual void CheckGraphicsState() {
            if (null == gs) {
                throw new InvalidOperationException(iText.IO.Logs.IoLogMessageConstant.GRAPHICS_STATE_WAS_DELETED);
            }
        }
    }
}
