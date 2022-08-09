using iText.Commons.Utils;

namespace iText.Kernel.Pdf.Function {
    [FunctionalInterfaceAttribute]
    public interface IInputConversionFunction {
        double[] Convert(byte[] input);
    }
}
