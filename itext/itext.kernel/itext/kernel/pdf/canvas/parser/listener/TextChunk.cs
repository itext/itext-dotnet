/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>Represents a chunk of text, it's orientation, and location relative to the orientation vector</summary>
    public class TextChunk {
        /// <summary>the text of the chunk</summary>
        protected internal readonly String text;

        protected internal readonly ITextChunkLocation location;

        public TextChunk(String @string, ITextChunkLocation loc) {
            this.text = @string;
            this.location = loc;
        }

        /// <returns>the text captured by this chunk</returns>
        public virtual String GetText() {
            return text;
        }

        public virtual ITextChunkLocation GetLocation() {
            return location;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void PrintDiagnostics() {
            System.Console.Out.WriteLine("Text (@" + location.GetStartLocation() + " -> " + location.GetEndLocation() 
                + "): " + text);
            System.Console.Out.WriteLine("orientationMagnitude: " + location.OrientationMagnitude());
            System.Console.Out.WriteLine("distPerpendicular: " + location.DistPerpendicular());
            System.Console.Out.WriteLine("distParallel: " + location.DistParallelStart());
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool SameLine(iText.Kernel.Pdf.Canvas.Parser.Listener.TextChunk lastChunk) {
            return GetLocation().SameLine(lastChunk.GetLocation());
        }
//\endcond
    }
}
