/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    public class PdfSoundAnnotation : PdfMarkupAnnotation {
        public PdfSoundAnnotation(Rectangle rect, PdfStream sound)
            : base(rect) {
            /*
            There is a problem playing *.wav files via internal player in Acrobat.
            The first byte of the audio stream data should be deleted, then wav file will be played correctly.
            Otherwise it will be broken. Other supporting file types don't have such problem.
            */
            Put(PdfName.Sound, sound);
        }

        /// <param name="pdfObject">object representing this annotation</param>
        [System.ObsoleteAttribute(@"Use PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject) instead. Will be made protected in 7.1"
            )]
        public PdfSoundAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <exception cref="System.IO.IOException"/>
        public PdfSoundAnnotation(PdfDocument document, Rectangle rect, Stream soundStream, float sampleRate, PdfName
             encoding, int channels, int sampleSizeInBits)
            : base(rect) {
            PdfStream sound = new PdfStream(document, JavaUtil.CorrectWavFile(soundStream));
            sound.Put(PdfName.R, new PdfNumber(sampleRate));
            sound.Put(PdfName.E, encoding);
            sound.Put(PdfName.B, new PdfNumber(sampleSizeInBits));
            sound.Put(PdfName.C, new PdfNumber(channels));
            Put(PdfName.Sound, sound);
        }

        public override PdfName GetSubtype() {
            return PdfName.Sound;
        }

        public virtual PdfStream GetSound() {
            return GetPdfObject().GetAsStream(PdfName.Sound);
        }

        /// <summary>The name of an icon that is used in displaying the annotation.</summary>
        /// <remarks>
        /// The name of an icon that is used in displaying the annotation. Possible values are different for different
        /// annotation types. See
        /// <see cref="SetIconName(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies the icon for displaying annotation, or null if icon name is not specified.
        /// </returns>
        public override PdfName GetIconName() {
            return GetPdfObject().GetAsName(PdfName.Name);
        }

        /// <summary>The name of an icon that is used in displaying the annotation.</summary>
        /// <param name="name">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies the icon for displaying annotation. Possible values are different
        /// for different annotation types:
        /// <ul>
        /// <li>Speaker</li>
        /// <li>Mic</li>
        /// </ul>
        /// Additional names may be supported as well.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfSoundAnnotation"/>
        /// instance.
        /// </returns>
        public override PdfMarkupAnnotation SetIconName(PdfName name) {
            return (iText.Kernel.Pdf.Annot.PdfSoundAnnotation)Put(PdfName.Name, name);
        }
    }
}
