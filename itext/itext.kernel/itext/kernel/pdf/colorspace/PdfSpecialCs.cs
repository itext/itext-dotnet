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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace {
    public abstract class PdfSpecialCs : PdfColorSpace {
        protected internal PdfSpecialCs(PdfArray pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        public class Indexed : PdfSpecialCs {
            public Indexed(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public Indexed(PdfObject @base, int hival, PdfString lookup)
                : this(GetIndexedCsArray(@base, hival, lookup)) {
            }

            public override int GetNumberOfComponents() {
                return 1;
            }

            public virtual PdfColorSpace GetBaseCs() {
                return MakeColorSpace(((PdfArray)GetPdfObject()).Get(1));
            }

            private static PdfArray GetIndexedCsArray(PdfObject @base, int hival, PdfString lookup) {
                PdfArray indexed = new PdfArray();
                indexed.Add(PdfName.Indexed);
                indexed.Add(@base);
                indexed.Add(new PdfNumber(hival));
                indexed.Add(lookup.SetHexWriting(true));
                return indexed;
            }
        }

        public class Separation : PdfSpecialCs {
            public Separation(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public Separation(PdfName name, PdfObject alternateSpace, PdfObject tintTransform)
                : this(GetSeparationCsArray(name, alternateSpace, tintTransform)) {
            }

            /// <summary>Creates a new separation color space.</summary>
            /// <param name="name">The name for the separation color</param>
            /// <param name="alternateSpace">The alternate colorspace</param>
            /// <param name="tintTransform">
            /// The function how the transform colors in the separation color space
            /// to the alternate color space
            /// </param>
            public Separation(String name, PdfColorSpace alternateSpace, IPdfFunction tintTransform)
                : this(new PdfName(name), alternateSpace.GetPdfObject(), tintTransform.GetAsPdfObject()) {
                if (!tintTransform.CheckCompatibilityWithColorSpace(alternateSpace)) {
                    throw new PdfException(KernelExceptionMessageConstant.FUNCTION_IS_NOT_COMPATIBLE_WITH_COLOR_SPACE, this);
                }
            }

            public override int GetNumberOfComponents() {
                return 1;
            }

            public virtual PdfColorSpace GetBaseCs() {
                return MakeColorSpace(((PdfArray)GetPdfObject()).Get(2));
            }

            public virtual PdfName GetName() {
                return ((PdfArray)GetPdfObject()).GetAsName(1);
            }

            /// <summary>Gets the function to calulate a separation color value to an alternative colorspace.</summary>
            /// <returns>
            /// a
            /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
            /// to perform the calculation
            /// </returns>
            public virtual IPdfFunction GetTintTransformation() {
                return PdfFunctionFactory.Create(((PdfArray)GetPdfObject()).Get(3));
            }

            private static PdfArray GetSeparationCsArray(PdfName name, PdfObject alternateSpace, PdfObject tintTransform
                ) {
                PdfArray separation = new PdfArray();
                separation.Add(PdfName.Separation);
                separation.Add(name);
                separation.Add(alternateSpace);
                separation.Add(tintTransform);
                return separation;
            }
        }

        public class DeviceN : PdfSpecialCs {
            protected internal int numOfComponents = 0;

            public DeviceN(PdfArray pdfObject)
                : base(pdfObject) {
                numOfComponents = pdfObject.GetAsArray(1).Size();
            }

            public DeviceN(PdfArray names, PdfObject alternateSpace, PdfObject tintTransform)
                : this(GetDeviceNCsArray(names, alternateSpace, tintTransform)) {
            }

            /// <summary>Creates a new DiviceN colorspace.</summary>
            /// <param name="names">the names of the components</param>
            /// <param name="alternateSpace">the alternate colorspace</param>
            /// <param name="tintTransform">the function to transform colors to the alternate colorspace</param>
            public DeviceN(IList<String> names, PdfColorSpace alternateSpace, IPdfFunction tintTransform)
                : this(new PdfArray(names, true), alternateSpace.GetPdfObject(), tintTransform.GetAsPdfObject()) {
                if (tintTransform.GetInputSize() != numOfComponents || tintTransform.GetOutputSize() != alternateSpace.GetNumberOfComponents
                    ()) {
                    throw new PdfException(KernelExceptionMessageConstant.FUNCTION_IS_NOT_COMPATIBLE_WITH_COLOR_SPACE, this);
                }
            }

            public override int GetNumberOfComponents() {
                return numOfComponents;
            }

            public virtual PdfColorSpace GetBaseCs() {
                return MakeColorSpace(((PdfArray)GetPdfObject()).Get(2));
            }

            public virtual PdfArray GetNames() {
                return ((PdfArray)GetPdfObject()).GetAsArray(1);
            }

            protected internal static PdfArray GetDeviceNCsArray(PdfArray names, PdfObject alternateSpace, PdfObject tintTransform
                ) {
                PdfArray deviceN = new PdfArray();
                deviceN.Add(PdfName.DeviceN);
                deviceN.Add(names);
                deviceN.Add(alternateSpace);
                deviceN.Add(tintTransform);
                return deviceN;
            }
        }

        public class NChannel : PdfSpecialCs.DeviceN {
            public NChannel(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public NChannel(PdfArray names, PdfObject alternateSpace, PdfObject tintTransform, PdfDictionary attributes
                )
                : this(GetNChannelCsArray(names, alternateSpace, tintTransform, attributes)) {
            }

            /// <summary>Creates a new NChannel colorspace.</summary>
            /// <param name="names">the names for the components</param>
            /// <param name="alternateSpace">the alternative colorspace</param>
            /// <param name="tintTransform">the function to transform colors to the alternate color space</param>
            /// <param name="attributes">NChannel specific attributes</param>
            public NChannel(IList<String> names, PdfColorSpace alternateSpace, IPdfFunction tintTransform, PdfDictionary
                 attributes)
                : this(new PdfArray(names, true), alternateSpace.GetPdfObject(), tintTransform.GetAsPdfObject(), attributes
                    ) {
                if (tintTransform.GetInputSize() != 1 || tintTransform.GetOutputSize() != alternateSpace.GetNumberOfComponents
                    ()) {
                    throw new PdfException(KernelExceptionMessageConstant.FUNCTION_IS_NOT_COMPATIBLE_WITH_COLOR_SPACE, this);
                }
            }

            protected internal static PdfArray GetNChannelCsArray(PdfArray names, PdfObject alternateSpace, PdfObject 
                tintTransform, PdfDictionary attributes) {
                PdfArray nChannel = GetDeviceNCsArray(names, alternateSpace, tintTransform);
                nChannel.Add(attributes);
                return nChannel;
            }
        }

        public class Pattern : PdfColorSpace {
            public Pattern()
                : base(PdfName.Pattern) {
            }

            protected internal Pattern(PdfObject pdfObj)
                : base(pdfObj) {
            }

            public override int GetNumberOfComponents() {
                return 0;
            }

            protected internal override bool IsWrappedObjectMustBeIndirect() {
                return false;
            }
        }

        public class UncoloredTilingPattern : PdfSpecialCs.Pattern {
            public UncoloredTilingPattern(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public UncoloredTilingPattern(PdfColorSpace underlyingColorSpace)
                : base(new PdfArray(JavaUtil.ArraysAsList(PdfName.Pattern, underlyingColorSpace.GetPdfObject()))) {
            }

            /// <summary>
            /// To manually flush a
            /// <c>PdfObject</c>
            /// behind this wrapper, you have to ensure
            /// that this object is added to the document, i.e. it has an indirect reference.
            /// </summary>
            /// <remarks>
            /// To manually flush a
            /// <c>PdfObject</c>
            /// behind this wrapper, you have to ensure
            /// that this object is added to the document, i.e. it has an indirect reference.
            /// Basically this means that before flushing you need to explicitly call
            /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
            /// For example: wrapperInstance.makeIndirect(document).flush();
            /// Note that not every wrapper require this, only those that have such warning in documentation.
            /// </remarks>
            public override void Flush() {
                base.Flush();
            }

            public virtual PdfColorSpace GetUnderlyingColorSpace() {
                return PdfColorSpace.MakeColorSpace(((PdfArray)GetPdfObject()).Get(1));
            }

            public override int GetNumberOfComponents() {
                return PdfColorSpace.MakeColorSpace(((PdfArray)GetPdfObject()).Get(1)).GetNumberOfComponents();
            }

            protected internal override bool IsWrappedObjectMustBeIndirect() {
                return true;
            }
        }
    }
}
