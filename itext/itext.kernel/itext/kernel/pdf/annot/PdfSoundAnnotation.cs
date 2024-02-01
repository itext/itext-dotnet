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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    public class PdfSoundAnnotation : PdfMarkupAnnotation {
        /// <summary>Creates a new Sound annotation.</summary>
        /// <remarks>
        /// Creates a new Sound annotation.
        /// There is a problem playing *.wav files via internal player in Acrobat.
        /// The first byte of the audio stream data should be deleted, then wav file will be played correctly.
        /// Otherwise it will be broken. Other supporting file types don't have such problem.
        /// Sound annotations are deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="rect">the rectangle that specifies annotation position and bounds on page</param>
        /// <param name="sound">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// with sound
        /// </param>
        public PdfSoundAnnotation(Rectangle rect, PdfStream sound)
            : base(rect) {
            Put(PdfName.Sound, sound);
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfSoundAnnotation"/>
        /// instance based on
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance, that represents existing annotation object in the document.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing annotation object
        /// </param>
        /// <seealso cref="PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject)"/>
        protected internal PdfSoundAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Creates a sound annotation.</summary>
        /// <remarks>Creates a sound annotation. Sound annotations are deprecated in PDF 2.0.</remarks>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to which annotation will be added
        /// </param>
        /// <param name="rect">the rectangle that specifies annotation position and bounds on page</param>
        /// <param name="soundStream">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// with sound
        /// </param>
        /// <param name="sampleRate">the sampling rate, in samples per second</param>
        /// <param name="encoding">the encoding format for the sample data</param>
        /// <param name="channels">the number of sound channels</param>
        /// <param name="sampleSizeInBits">the number of bits per sample value per channel</param>
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
        /// <see cref="SetIconName(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies the icon for displaying annotation, or null if icon name is not specified.
        /// </returns>
        public virtual PdfName GetIconName() {
            return GetPdfObject().GetAsName(PdfName.Name);
        }

        /// <summary>The name of an icon that is used in displaying the annotation.</summary>
        /// <param name="name">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies the icon for displaying annotation. Possible values are different
        /// for different annotation types:
        /// <list type="bullet">
        /// <item><description>Speaker
        /// </description></item>
        /// <item><description>Mic
        /// </description></item>
        /// </list>
        /// Additional names may be supported as well.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfSoundAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfSoundAnnotation SetIconName(PdfName name) {
            return (iText.Kernel.Pdf.Annot.PdfSoundAnnotation)Put(PdfName.Name, name);
        }
    }
}
