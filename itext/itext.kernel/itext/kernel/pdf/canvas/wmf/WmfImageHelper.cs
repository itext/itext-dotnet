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
using System.IO;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>Helper class for the WmfImage implementation.</summary>
    /// <remarks>
    /// Helper class for the WmfImage implementation. Assists in the creation of a
    /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>.
    /// </remarks>
    public class WmfImageHelper {
        /// <summary>Scales the WMF font size.</summary>
        /// <remarks>Scales the WMF font size. The default value is 0.86.</remarks>
        public static float wmfFontCorrection = 0.86f;

        private WmfImageData wmf;

        private float plainWidth;

        private float plainHeight;

        /// <summary>Creates a helper instance.</summary>
        /// <param name="wmf">
        /// the
        /// <see cref="WmfImageData"/>
        /// object
        /// </param>
        public WmfImageHelper(ImageData wmf) {
            if (wmf.GetOriginalType() != ImageType.WMF) {
                throw new ArgumentException("WMF image expected");
            }
            this.wmf = (WmfImageData)wmf;
            ProcessParameters();
        }

        /// <summary>This method checks if the image is a valid WMF and processes some parameters.</summary>
        private void ProcessParameters() {
            Stream @is = null;
            try {
                String errorID;
                if (wmf.GetData() == null) {
                    @is = UrlUtil.OpenStream(wmf.GetUrl());
                    errorID = wmf.GetUrl().ToString();
                }
                else {
                    @is = new MemoryStream(wmf.GetData());
                    errorID = "Byte array";
                }
                InputMeta @in = new InputMeta(@is);
                if (@in.ReadInt() != unchecked((int)(0x9AC6CDD7))) {
                    throw new PdfException(KernelExceptionMessageConstant.NOT_A_VALID_PLACEABLE_WINDOWS_METAFILE, errorID);
                }
                @in.ReadWord();
                int left = @in.ReadShort();
                int top = @in.ReadShort();
                int right = @in.ReadShort();
                int bottom = @in.ReadShort();
                int inch = @in.ReadWord();
                wmf.SetDpi(72, 72);
                wmf.SetHeight((float)(bottom - top) / inch * 72f);
                wmf.SetWidth((float)(right - left) / inch * 72f);
            }
            catch (System.IO.IOException) {
                throw new PdfException(KernelExceptionMessageConstant.WMF_IMAGE_EXCEPTION);
            }
            finally {
                if (@is != null) {
                    try {
                        @is.Dispose();
                    }
                    catch (System.IO.IOException) {
                    }
                }
            }
        }

        /// <summary>Create a PdfXObject based on the WMF image.</summary>
        /// <remarks>
        /// Create a PdfXObject based on the WMF image. The PdfXObject will have the dimensions of the
        /// WMF image.
        /// </remarks>
        /// <param name="document">PdfDocument to add the PdfXObject to</param>
        /// <returns>PdfXObject based on the WMF image</returns>
        public virtual PdfXObject CreateFormXObject(PdfDocument document) {
            PdfFormXObject pdfForm = new PdfFormXObject(new Rectangle(0, 0, wmf.GetWidth(), wmf.GetHeight()));
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            Stream @is = null;
            try {
                if (wmf.GetData() == null) {
                    @is = UrlUtil.OpenStream(wmf.GetUrl());
                }
                else {
                    @is = new MemoryStream(wmf.GetData());
                }
                MetaDo meta = new MetaDo(@is, canvas);
                meta.ReadAll();
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.WMF_IMAGE_EXCEPTION, e);
            }
            finally {
                if (@is != null) {
                    try {
                        @is.Dispose();
                    }
                    catch (System.IO.IOException) {
                    }
                }
            }
            return pdfForm;
        }
    }
}
