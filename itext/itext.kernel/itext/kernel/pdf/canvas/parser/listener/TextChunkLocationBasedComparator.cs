using System.Collections.Generic;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    internal class TextChunkLocationBasedComparator : IComparer<TextChunk> {
        private IComparer<ITextChunkLocation> locationComparator;

        public TextChunkLocationBasedComparator(IComparer<ITextChunkLocation> locationComparator) {
            this.locationComparator = locationComparator;
        }

        public virtual int Compare(TextChunk o1, TextChunk o2) {
            return locationComparator.Compare(o1.location, o2.location);
        }
    }
}
