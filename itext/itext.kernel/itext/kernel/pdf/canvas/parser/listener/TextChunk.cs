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

        internal virtual void PrintDiagnostics() {
            System.Console.Out.WriteLine("Text (@" + location.GetStartLocation() + " -> " + location.GetEndLocation() 
                + "): " + text);
            System.Console.Out.WriteLine("orientationMagnitude: " + location.OrientationMagnitude());
            System.Console.Out.WriteLine("distPerpendicular: " + location.DistPerpendicular());
            System.Console.Out.WriteLine("distParallel: " + location.DistParallelStart());
        }

        internal virtual bool SameLine(iText.Kernel.Pdf.Canvas.Parser.Listener.TextChunk lastChunk) {
            return GetLocation().SameLine(lastChunk.GetLocation());
        }
    }
}
