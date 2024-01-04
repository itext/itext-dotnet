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
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to warn user that annotation will not be flattened.</summary>
    public class WarnFormfieldFlattener : IAnnotationFlattener {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.Annotationsflattening.WarnFormfieldFlattener
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="WarnFormfieldFlattener"/>
        /// instance.
        /// </summary>
        public WarnFormfieldFlattener() {
        }

        //empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual bool Flatten(PdfAnnotation annotation, PdfPage page) {
            if (annotation == null) {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL, "annotation"
                    ));
            }
            if (page == null) {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL, "page"
                    ));
            }
            LOGGER.LogWarning(KernelLogMessageConstant.FORMFIELD_ANNOTATION_WILL_NOT_BE_FLATTENED);
            return false;
        }
    }
}
