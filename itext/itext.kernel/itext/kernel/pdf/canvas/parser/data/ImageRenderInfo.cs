/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf.Canvas.Parser.Data {
    /// <summary>Represents image data from a PDF.</summary>
    public class ImageRenderInfo : AbstractRenderInfo {
        /// <summary>The coordinate transformation matrix that was in effect when the image was rendered.</summary>
        private readonly Matrix ctm;

        private readonly PdfImageXObject image;

        /// <summary>The color space dictionary from resources which are associated with the image.</summary>
        private readonly PdfDictionary colorSpaceDictionary;

        /// <summary>Defines if the encountered image was inline.</summary>
        private readonly bool isInline;

        private readonly PdfName resourceName;

        /// <summary>Hierarchy of nested canvas tags for the text from the most inner (nearest to text) tag to the most outer.
        ///     </summary>
        private readonly IList<CanvasTag> canvasTagHierarchy;

        /// <summary>Creates an ImageRenderInfo.</summary>
        /// <param name="canvasTagHierarchy">
        /// the hierarchy of nested canvas tags for the text from the most
        /// inner (nearest to text) tag to the most outer
        /// </param>
        /// <param name="gs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.CanvasGraphicsState">canvas graphics state</see>
        /// </param>
        /// <param name="ctm">the coordinate transformation matrix at the time the image is rendered</param>
        /// <param name="imageStream">the image stream object</param>
        /// <param name="resourceName">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfName">name</see>
        /// of the image resource
        /// </param>
        /// <param name="colorSpaceDictionary">the color space dictionary from resources which are associated with the image
        ///     </param>
        /// <param name="isInline">defines if the encountered image was inline</param>
        public ImageRenderInfo(Stack<CanvasTag> canvasTagHierarchy, CanvasGraphicsState gs, Matrix ctm, PdfStream 
            imageStream, PdfName resourceName, PdfDictionary colorSpaceDictionary, bool isInline)
            : base(gs) {
            this.canvasTagHierarchy = JavaCollectionsUtil.UnmodifiableList<CanvasTag>(new List<CanvasTag>(canvasTagHierarchy
                ));
            this.resourceName = resourceName;
            this.ctm = ctm;
            this.image = new PdfImageXObject(imageStream);
            this.colorSpaceDictionary = colorSpaceDictionary;
            this.isInline = isInline;
        }

        /// <summary>Gets the image wrapped in ImageXObject.</summary>
        /// <remarks>
        /// Gets the image wrapped in ImageXObject.
        /// You can:
        /// <list type="bullet">
        /// <item><description>get image bytes with
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject.GetImageBytes(bool)"/>
        /// , these image bytes
        /// represent native image, i.e you can write these bytes to disk and get just an usual image;
        /// </description></item>
        /// <item><description>obtain PdfStream object which contains image dictionary with
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.GetPdfObject()"/>
        /// method;
        /// </description></item>
        /// <item><description>convert image to
        /// <see cref="System.Drawing.Bitmap"/>
        /// with
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject.GetBufferedImage()"/>
        /// ;
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject">image</see>
        /// </returns>
        public virtual PdfImageXObject GetImage() {
            return image;
        }

        /// <summary>Gets the name of the image resource.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfName">name</see>
        /// of the image resource
        /// </returns>
        public virtual PdfName GetImageResourceName() {
            return resourceName;
        }

        /// <summary>Gets the vector in User space representing the start point of the image.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Vector">vector</see>
        /// in User space representing the start point of the image
        /// </returns>
        public virtual Vector GetStartPoint() {
            return new Vector(0, 0, 1).Cross(ctm);
        }

        /// <summary>Gets the coordinate transformation matrix in User space which was active when this image was rendered.
        ///     </summary>
        /// <returns>
        /// the coordinate transformation matrix in User space which was active when this image
        /// was rendered
        /// </returns>
        public virtual Matrix GetImageCtm() {
            return ctm;
        }

        /// <summary>Gets the size of the image in User space units.</summary>
        /// <returns>the size of the image, in User space units</returns>
        public virtual float GetArea() {
            // the image space area is 1, so we multiply that by the determinant of the CTM to get the transformed area
            return ctm.GetDeterminant();
        }

        /// <summary>Gets the inline flag.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if image was inlined in original stream
        /// </returns>
        public virtual bool IsInline() {
            return isInline;
        }

        /// <summary>Gets the color space dictionary of the image.</summary>
        /// <returns>the color space dictionary from resources which are associated with the image</returns>
        public virtual PdfDictionary GetColorSpaceDictionary() {
            return colorSpaceDictionary;
        }

        /// <summary>Gets hierarchy of the canvas tags that wraps given text.</summary>
        /// <returns>list of the wrapping canvas tags. The first tag is the innermost (nearest to the text).</returns>
        public virtual IList<CanvasTag> GetCanvasTagHierarchy() {
            return canvasTagHierarchy;
        }

        /// <summary>
        /// Gets the marked-content identifier associated with this
        /// <see cref="ImageRenderInfo"/>
        /// instance
        /// </summary>
        /// <returns>associated marked-content identifier or -1 in case content is unmarked</returns>
        public virtual int GetMcid() {
            foreach (CanvasTag tag in canvasTagHierarchy) {
                if (tag.HasMcid()) {
                    return tag.GetMcid();
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if the text belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <returns>true if the text is marked with this id</returns>
        public virtual bool HasMcid(int mcid) {
            return HasMcid(mcid, false);
        }

        /// <summary>
        /// Checks if the text belongs to a marked content sequence
        /// with a given mcid.
        /// </summary>
        /// <param name="mcid">a marked content id</param>
        /// <param name="checkTheTopmostLevelOnly">indicates whether to check the topmost level of marked content stack only
        ///     </param>
        /// <returns>true if the text is marked with this id</returns>
        public virtual bool HasMcid(int mcid, bool checkTheTopmostLevelOnly) {
            if (checkTheTopmostLevelOnly) {
                if (canvasTagHierarchy != null) {
                    int infoMcid = GetMcid();
                    return infoMcid != -1 && infoMcid == mcid;
                }
            }
            else {
                foreach (CanvasTag tag in canvasTagHierarchy) {
                    if (tag.HasMcid() && (tag.GetMcid() == mcid)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
