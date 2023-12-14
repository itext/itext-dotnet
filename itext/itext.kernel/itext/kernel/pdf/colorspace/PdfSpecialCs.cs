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
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel;
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

            public Separation(String name, PdfColorSpace alternateSpace, PdfFunction tintTransform)
                : this(new PdfName(name), alternateSpace.GetPdfObject(), tintTransform.GetPdfObject()) {
                if (!tintTransform.CheckCompatibilityWithColorSpace(alternateSpace)) {
                    throw new PdfException(PdfException.FunctionIsNotCompatibleWitColorSpace, this);
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

            public DeviceN(IList<String> names, PdfColorSpace alternateSpace, PdfFunction tintTransform)
                : this(new PdfArray(names, true), alternateSpace.GetPdfObject(), tintTransform.GetPdfObject()) {
                if (tintTransform.GetInputSize() != GetNumberOfComponents() || tintTransform.GetOutputSize() != alternateSpace
                    .GetNumberOfComponents()) {
                    throw new PdfException(PdfException.FunctionIsNotCompatibleWitColorSpace, this);
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

            public NChannel(IList<String> names, PdfColorSpace alternateSpace, PdfFunction tintTransform, PdfDictionary
                 attributes)
                : this(new PdfArray(names, true), alternateSpace.GetPdfObject(), tintTransform.GetPdfObject(), attributes) {
                if (tintTransform.GetInputSize() != 1 || tintTransform.GetOutputSize() != alternateSpace.GetNumberOfComponents
                    ()) {
                    throw new PdfException(PdfException.FunctionIsNotCompatibleWitColorSpace, this);
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
            protected internal override bool IsWrappedObjectMustBeIndirect() {
                return false;
            }

            public Pattern()
                : base(PdfName.Pattern) {
            }

            protected internal Pattern(PdfObject pdfObj)
                : base(pdfObj) {
            }

            public override int GetNumberOfComponents() {
                return 0;
            }
        }

        public class UncoloredTilingPattern : PdfSpecialCs.Pattern {
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

            public UncoloredTilingPattern(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public UncoloredTilingPattern(PdfColorSpace underlyingColorSpace)
                : base(new PdfArray(JavaUtil.ArraysAsList(PdfName.Pattern, underlyingColorSpace.GetPdfObject()))) {
            }

            public override int GetNumberOfComponents() {
                return PdfColorSpace.MakeColorSpace(((PdfArray)GetPdfObject()).Get(1)).GetNumberOfComponents();
            }

            public virtual PdfColorSpace GetUnderlyingColorSpace() {
                return PdfColorSpace.MakeColorSpace(((PdfArray)GetPdfObject()).Get(1));
            }
        }
    }
}
