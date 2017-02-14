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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    /// <summary>Represents image data from a PDF</summary>
    public class ImageRenderInfo : IEventData {
        /// <summary>The coordinate transformation matrix that was in effect when the image was rendered</summary>
        private Matrix ctm;

        private PdfImageXObject image;

        /// <summary>the color space dictionary from resources which are associated with the image</summary>
        private PdfDictionary colorSpaceDictionary;

        /// <summary>defines if the encountered image was inline</summary>
        private bool isInline;

        /// <summary>Create an ImageRenderInfo</summary>
        /// <param name="ctm">the coordinate transformation matrix at the time the image is rendered</param>
        /// <param name="stream">image stream object</param>
        /// <param name="colorSpaceDictionary">the color space dictionary from resources which are associated with the image
        ///     </param>
        /// <param name="isInline">defines if the encountered image was inline</param>
        public ImageRenderInfo(Matrix ctm, PdfStream stream, PdfDictionary colorSpaceDictionary, bool isInline) {
            this.ctm = ctm;
            this.image = new PdfImageXObject(stream);
            this.colorSpaceDictionary = colorSpaceDictionary;
            this.isInline = isInline;
        }

        /// <summary>Gets an image wrapped in ImageXObject.</summary>
        /// <remarks>
        /// Gets an image wrapped in ImageXObject.
        /// You can:
        /// <ul>
        /// <li>get image bytes with
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject.GetImageBytes(bool)"/>
        /// , these image bytes
        /// represent native image, i.e you can write these bytes to disk and get just an usual image;</li>
        /// <li>obtain PdfStream object which contains image dictionary with
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.GetPdfObject()"/>
        /// method;</li>
        /// <li>convert image to
        /// <see cref="Java.Awt.Image.BufferedImage"/>
        /// with
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject.GetBufferedImage()"/>
        /// ;</li>
        /// </ul>
        /// </remarks>
        public virtual PdfImageXObject GetImage() {
            return image;
        }

        /// <returns>a vector in User space representing the start point of the image</returns>
        public virtual Vector GetStartPoint() {
            return new Vector(0, 0, 1).Cross(ctm);
        }

        /// <returns>The coordinate transformation matrix which was active when this image was rendered. Coordinates are in User space.
        ///     </returns>
        public virtual Matrix GetImageCtm() {
            return ctm;
        }

        /// <returns>the size of the image, in User space units</returns>
        public virtual float GetArea() {
            // the image space area is 1, so we multiply that by the determinant of the CTM to get the transformed area
            return ctm.GetDeterminant();
        }

        /// <returns>true if image was inlined in original stream.</returns>
        public virtual bool IsInline() {
            return isInline;
        }

        /// <returns>the color space dictionary from resources which are associated with the image</returns>
        public virtual PdfDictionary GetColorSpaceDictionary() {
            return colorSpaceDictionary;
        }
    }
}
