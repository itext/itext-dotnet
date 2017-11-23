using iText.Kernel.Geom;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    public interface ITextChunkLocation {
        float DistParallelEnd();

        float DistParallelStart();

        int DistPerpendicular();

        float GetCharSpaceWidth();

        Vector GetEndLocation();

        Vector GetStartLocation();

        int OrientationMagnitude();

        bool SameLine(ITextChunkLocation @as);

        float DistanceFromEndOf(ITextChunkLocation other);

        bool IsAtWordBoundary(ITextChunkLocation previous);
    }
}
