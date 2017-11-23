using System.Collections.Generic;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    internal class DefaultTextChunkLocationComparator : IComparer<ITextChunkLocation> {
        private bool leftToRight = true;

        public DefaultTextChunkLocationComparator()
            : this(true) {
        }

        public DefaultTextChunkLocationComparator(bool leftToRight) {
            this.leftToRight = leftToRight;
        }

        public virtual int Compare(ITextChunkLocation first, ITextChunkLocation second) {
            if (first == second) {
                return 0;
            }
            // not really needed, but just in case
            int result;
            result = iText.IO.Util.JavaUtil.IntegerCompare(first.OrientationMagnitude(), second.OrientationMagnitude()
                );
            if (result != 0) {
                return result;
            }
            int distPerpendicularDiff = first.DistPerpendicular() - second.DistPerpendicular();
            if (distPerpendicularDiff != 0) {
                return distPerpendicularDiff;
            }
            return leftToRight ? iText.IO.Util.JavaUtil.FloatCompare(first.DistParallelStart(), second.DistParallelStart
                ()) : -iText.IO.Util.JavaUtil.FloatCompare(first.DistParallelEnd(), second.DistParallelEnd());
        }
    }
}
