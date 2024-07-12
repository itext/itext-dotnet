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
using Microsoft.Extensions.Logging;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
//\cond DO_NOT_DOCUMENT
    internal sealed class IndirectFilterUtils {
        private IndirectFilterUtils() {
        }

//\cond DO_NOT_DOCUMENT
        internal static void ThrowFlushedFilterException(PdfStream stream) {
            throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.FLUSHED_STREAM_FILTER_EXCEPTION
                , stream.GetIndirectReference().GetObjNumber(), stream.GetIndirectReference().GetGenNumber()));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void LogFilterWasAlreadyFlushed(ILogger logger, PdfStream stream) {
            logger.LogInformation(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.FILTER_WAS_ALREADY_FLUSHED
                , stream.GetIndirectReference().GetObjNumber(), stream.GetIndirectReference().GetGenNumber()));
        }
//\endcond
    }
//\endcond
}
