using iText.Commons.Utils;

namespace iText.Kernel.Pdf.Function {
    [FunctionalInterfaceAttribute]
    public interface IOutputConversionFunction {
        byte[] Convert(double[] input);
    }
}
