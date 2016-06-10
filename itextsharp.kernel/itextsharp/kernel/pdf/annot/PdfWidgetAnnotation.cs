/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System.Collections.Generic;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Annot
{
    public class PdfWidgetAnnotation : PdfAnnotation
    {
        public PdfWidgetAnnotation(Rectangle rect)
            : base(rect)
        {
            {
                widgetEntries.Add(PdfName.Subtype);
                widgetEntries.Add(PdfName.Type);
                widgetEntries.Add(PdfName.Rect);
                widgetEntries.Add(PdfName.Contents);
                widgetEntries.Add(PdfName.P);
                widgetEntries.Add(PdfName.NM);
                widgetEntries.Add(PdfName.M);
                widgetEntries.Add(PdfName.F);
                widgetEntries.Add(PdfName.AP);
                widgetEntries.Add(PdfName.AS);
                widgetEntries.Add(PdfName.Border);
                widgetEntries.Add(PdfName.C);
                widgetEntries.Add(PdfName.StructParent);
                widgetEntries.Add(PdfName.OC);
                widgetEntries.Add(PdfName.H);
                widgetEntries.Add(PdfName.MK);
                widgetEntries.Add(PdfName.A);
                widgetEntries.Add(PdfName.AA);
                widgetEntries.Add(PdfName.BS);
            }
        }

        public PdfWidgetAnnotation(PdfDictionary pdfObject)
            : base(pdfObject)
        {
            {
                widgetEntries.Add(PdfName.Subtype);
                widgetEntries.Add(PdfName.Type);
                widgetEntries.Add(PdfName.Rect);
                widgetEntries.Add(PdfName.Contents);
                widgetEntries.Add(PdfName.P);
                widgetEntries.Add(PdfName.NM);
                widgetEntries.Add(PdfName.M);
                widgetEntries.Add(PdfName.F);
                widgetEntries.Add(PdfName.AP);
                widgetEntries.Add(PdfName.AS);
                widgetEntries.Add(PdfName.Border);
                widgetEntries.Add(PdfName.C);
                widgetEntries.Add(PdfName.StructParent);
                widgetEntries.Add(PdfName.OC);
                widgetEntries.Add(PdfName.H);
                widgetEntries.Add(PdfName.MK);
                widgetEntries.Add(PdfName.A);
                widgetEntries.Add(PdfName.AA);
                widgetEntries.Add(PdfName.BS);
            }
        }

        private HashSet<PdfName> widgetEntries = new HashSet<PdfName>();

        public override PdfName GetSubtype()
        {
            return PdfName.Widget;
        }

        public virtual iTextSharp.Kernel.Pdf.Annot.PdfWidgetAnnotation SetParent(PdfObject parent)
        {
            return (iTextSharp.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.Parent, parent);
        }

        /// <summary>Setter for the annotation's highlighting mode.</summary>
        /// <remarks>
        /// Setter for the annotation's highlighting mode. Possible values are
        /// <ul>
        /// <li>
        /// <see cref="PdfAnnotation.HIGHLIGHT_NONE"/>
        /// - No highlighting.</li>
        /// <li>
        /// <see cref="PdfAnnotation.HIGHLIGHT_INVERT"/>
        /// - Invert the contents of the annotation rectangle.</li>
        /// <li>
        /// <see cref="PdfAnnotation.HIGHLIGHT_OUTLINE"/>
        /// - Invert the annotation's border.</li>
        /// <li>
        /// <see cref="PdfAnnotation.HIGHLIGHT_PUSH"/>
        /// - Display the annotation?s down appearance, if any.</li>
        /// <li>
        /// <see cref="PdfAnnotation.HIGHLIGHT_TOGGLE"/>
        /// - Same as P.</li>
        /// </ul>
        /// </remarks>
        /// <param name="mode">The new value for the annotation's highlighting mode.</param>
        /// <returns>The widget annotation which this method was called on.</returns>
        public virtual iTextSharp.Kernel.Pdf.Annot.PdfWidgetAnnotation SetHighlightMode(PdfName mode)
        {
            return (iTextSharp.Kernel.Pdf.Annot.PdfWidgetAnnotation)Put(PdfName.H, mode);
        }

        /// <summary>Getter for the annotation's highlighting mode.</summary>
        /// <returns>Current value of the annotation's highlighting mode.</returns>
        public virtual PdfName GetHighlightMode()
        {
            return GetPdfObject().GetAsName(PdfName.H);
        }

        /// <summary>This method removes all widget annotation entries from the form field  the given annotation merged with.
        ///     </summary>
        public virtual void ReleaseFormFieldFromWidgetAnnotation()
        {
            PdfDictionary annotDict = GetPdfObject();
            foreach (PdfName entry in widgetEntries)
            {
                annotDict.Remove(entry);
            }
            PdfDictionary parent = annotDict.GetAsDictionary(PdfName.Parent);
            if (parent != null && annotDict.Size() == 1)
            {
                PdfArray kids = parent.GetAsArray(PdfName.Kids);
                kids.Remove(annotDict.GetIndirectReference());
                if (kids.Size() == 0)
                {
                    parent.Remove(PdfName.Kids);
                }
            }
        }
    }
}
