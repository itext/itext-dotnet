/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to flatten annotations.</summary>
    /// <remarks>
    /// This class is used to flatten annotations.
    /// The default implementation first tries to draw the normal appearance stream of the annotation.
    /// If the normal appearance stream is not present, then it tries to draw the annotation using the fallback
    /// implementation.
    /// </remarks>
    public class DefaultAnnotationFlattener : IAnnotationFlattener {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.Annotationsflattening.DefaultAnnotationFlattener
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="DefaultAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public DefaultAnnotationFlattener() {
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
            PdfArray pdfArrayRectangle = annotation.GetRectangle();
            if (pdfArrayRectangle == null) {
                return false;
            }
            PdfObject normalAppearance = annotation.GetNormalAppearanceObject();
            if (normalAppearance is PdfStream) {
                Rectangle area = annotation.GetRectangle().ToRectangle();
                PdfCanvas under = this.CreateCanvas(page);
                PdfStream annotationNormalAppearanceStream = (PdfStream)normalAppearance;
                under.AddXObjectFittedIntoRectangle(new PdfFormXObject(annotationNormalAppearanceStream), area);
                page.RemoveAnnotation(annotation);
                return true;
            }
            bool drawn = Draw(annotation, page);
            if (drawn) {
                page.RemoveAnnotation(annotation);
                return true;
            }
            String message = MessageFormatUtil.Format(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED, annotation
                .GetSubtype());
            LOGGER.LogWarning(message);
            return false;
        }

        /// <summary>Creates a canvas.</summary>
        /// <remarks>Creates a canvas. It will draw above the other items on the canvas.</remarks>
        /// <param name="page">the page to draw the annotation on</param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// the annotation will be drawn upon.
        /// </returns>
        protected internal virtual PdfCanvas CreateCanvas(PdfPage page) {
            return new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), page.GetDocument());
        }

        /// <summary>Draws annotation.</summary>
        /// <remarks>
        /// Draws annotation.
        /// This method is called if the normal appearance stream of the annotation is not present.
        /// The default implementation returns false.
        /// </remarks>
        /// <param name="annotation">annotation to draw</param>
        /// <param name="page">page to draw annotation on</param>
        /// <returns>true if annotation was drawn, false otherwise</returns>
        protected internal virtual bool Draw(PdfAnnotation annotation, PdfPage page) {
            return false;
        }
    }
}
