/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to warn that annotation flattening is not supported for the given annotation.</summary>
    public class NotSupportedFlattener : IAnnotationFlattener {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.Annotationsflattening.NotSupportedFlattener
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="NotSupportedFlattener"/>
        /// instance.
        /// </summary>
        public NotSupportedFlattener() {
        }

        //empty constructor
        /// <summary>Logs a warning that annotation flattening is not supported for the given annotation.</summary>
        /// <param name="annotation">annotation to flatten</param>
        /// <param name="page">page to flatten annotation on</param>
        /// <returns>true if annotation was flattened, false otherwise</returns>
        public virtual bool Flatten(PdfAnnotation annotation, PdfPage page) {
            String message = MessageFormatUtil.Format(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED, annotation
                .GetSubtype());
            LOGGER.LogWarning(message);
            return false;
        }
    }
}
