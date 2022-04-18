using Microsoft.Extensions.Logging;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    internal sealed class IndirectFilterUtils {
        private IndirectFilterUtils() {
        }

        internal static void ThrowFlushedFilterException(PdfStream stream) {
            throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.FLUSHED_STREAM_FILTER_EXCEPTION
                , stream.GetIndirectReference().GetObjNumber(), stream.GetIndirectReference().GetGenNumber()));
        }

        internal static void LogFilterWasAlreadyFlushed(ILogger logger, PdfStream stream) {
            logger.LogInformation(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED
                , stream.GetIndirectReference().GetObjNumber(), stream.GetIndirectReference().GetGenNumber()));
        }
    }
}
