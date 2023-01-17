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
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Function {
    /// <summary>The class that represents the Pdf Function.</summary>
    [System.ObsoleteAttribute(@"Will be removed is future releases, use AbstractPdfFunction{T} instead")]
    public class PdfFunction : PdfObjectWrapper<PdfObject> {
        public PdfFunction(PdfObject pdfObject)
            : base(pdfObject) {
        }

        public virtual int GetFunctionType() {
            return (int)((PdfDictionary)GetPdfObject()).GetAsInt(PdfName.FunctionType);
        }

        public virtual bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
            return true;
        }

        public virtual int GetInputSize() {
            return ((PdfDictionary)GetPdfObject()).GetAsArray(PdfName.Domain).Size() / 2;
        }

        public virtual int GetOutputSize() {
            PdfArray range = ((PdfDictionary)GetPdfObject()).GetAsArray(PdfName.Range);
            return range == null ? 0 : (range.Size() / 2);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>Represents a type 0 pdf function.</summary>
        [System.ObsoleteAttribute(@"Will be removed is future releases, use PdfType0Function instead")]
        public class Type0 : PdfFunction {
            public Type0(PdfStream pdfObject)
                : base(pdfObject) {
            }

            public Type0(PdfArray domain, PdfArray range, PdfArray size, PdfNumber bitsPerSample, byte[] samples)
                : this(domain, range, size, bitsPerSample, null, null, null, samples) {
            }

            public Type0(PdfArray domain, PdfArray range, PdfArray size, PdfNumber bitsPerSample, PdfNumber order, PdfArray
                 encode, PdfArray decode, byte[] samples)
                : this(MakeType0(domain, range, size, bitsPerSample, order, encode, decode, samples)) {
            }

            public override bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
                return GetInputSize() == 1 && GetOutputSize() == alternateSpace.GetNumberOfComponents();
            }

            private static PdfStream MakeType0(PdfArray domain, PdfArray range, PdfArray size, PdfNumber bitsPerSample
                , PdfNumber order, PdfArray encode, PdfArray decode, byte[] samples) {
                PdfStream stream = new PdfStream(samples);
                stream.Put(PdfName.FunctionType, new PdfNumber(0));
                stream.Put(PdfName.Domain, domain);
                stream.Put(PdfName.Range, range);
                stream.Put(PdfName.Size, size);
                stream.Put(PdfName.BitsPerSample, bitsPerSample);
                if (order != null) {
                    stream.Put(PdfName.Order, order);
                }
                if (encode != null) {
                    stream.Put(PdfName.Encode, encode);
                }
                if (decode != null) {
                    stream.Put(PdfName.Decode, decode);
                }
                return stream;
            }
        }

        /// <summary>Represents a type 2 pdf function.</summary>
        [System.ObsoleteAttribute(@"Will be removed is future releases, use PdfType2Function instead")]
        public class Type2 : PdfFunction {
            public Type2(PdfDictionary pdfObject)
                : base(pdfObject) {
            }

            public Type2(PdfArray domain, PdfArray range, PdfNumber n)
                : this(domain, range, null, null, n) {
            }

            public Type2(PdfArray domain, PdfArray range, PdfArray c0, PdfArray c1, PdfNumber n)
                : this(MakeType2(domain, range, c0, c1, n)) {
            }

            public override int GetOutputSize() {
                PdfArray range = ((PdfDictionary)GetPdfObject()).GetAsArray(PdfName.C1);
                return range == null ? 0 : range.Size();
            }

            private static PdfDictionary MakeType2(PdfArray domain, PdfArray range, PdfArray c0, PdfArray c1, PdfNumber
                 n) {
                PdfDictionary dictionary = new PdfDictionary();
                dictionary.Put(PdfName.FunctionType, new PdfNumber(2));
                dictionary.Put(PdfName.Domain, domain);
                if (range != null) {
                    dictionary.Put(PdfName.Range, range);
                }
                if (c0 != null) {
                    dictionary.Put(PdfName.C0, c0);
                }
                if (c1 != null) {
                    dictionary.Put(PdfName.C1, c1);
                }
                dictionary.Put(PdfName.N, n);
                return dictionary;
            }
        }

        /// <summary>Represents a type 3 pdf function.</summary>
        [System.ObsoleteAttribute(@"Will be removed is future releases, use PdfType3Function instead")]
        public class Type3 : PdfFunction {
            public Type3(PdfDictionary pdfObject)
                : base(pdfObject) {
            }

            public Type3(PdfArray domain, PdfArray range, PdfArray functions, PdfArray bounds, PdfArray encode)
                : this(MakeType3(domain, range, functions, bounds, encode)) {
            }

            public Type3(PdfArray domain, PdfArray range, IList<PdfFunction> functions, PdfArray bounds, PdfArray encode
                )
                : this(domain, range, GetFunctionsArray(functions), bounds, encode) {
            }

            private static PdfDictionary MakeType3(PdfArray domain, PdfArray range, PdfArray functions, PdfArray bounds
                , PdfArray encode) {
                PdfDictionary dictionary = new PdfDictionary();
                dictionary.Put(PdfName.FunctionType, new PdfNumber(3));
                dictionary.Put(PdfName.Domain, domain);
                if (range != null) {
                    dictionary.Put(PdfName.Range, range);
                }
                dictionary.Put(PdfName.Functions, functions);
                dictionary.Put(PdfName.Bounds, bounds);
                dictionary.Put(PdfName.Encode, encode);
                return dictionary;
            }

            private static PdfArray GetFunctionsArray(IList<PdfFunction> functions) {
                PdfArray array = new PdfArray();
                foreach (PdfFunction function in functions) {
                    array.Add(function.GetPdfObject());
                }
                return array;
            }
        }

        /// <summary>Represents a type 4 pdf function.</summary>
        [System.ObsoleteAttribute(@"Will be removed is future releases, use PdfType4Function instead")]
        public class Type4 : PdfFunction {
            public Type4(PdfStream pdfObject)
                : base(pdfObject) {
            }

            public Type4(PdfArray domain, PdfArray range, byte[] ps)
                : this(MakeType4(domain, range, ps)) {
            }

            public override bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
                return GetInputSize() == 1 && GetOutputSize() == alternateSpace.GetNumberOfComponents();
            }

            private static PdfStream MakeType4(PdfArray domain, PdfArray range, byte[] ps) {
                PdfStream stream = new PdfStream(ps);
                stream.Put(PdfName.FunctionType, new PdfNumber(4));
                stream.Put(PdfName.Domain, domain);
                stream.Put(PdfName.Range, range);
                return stream;
            }
        }

        public static PdfFunction MakeFunction(PdfDictionary pdfObject) {
            switch (pdfObject.GetAsNumber(PdfName.FunctionType).IntValue()) {
                case 0: {
                    return new PdfFunction.Type0((PdfStream)pdfObject);
                }

                case 2: {
                    return new PdfFunction.Type2(pdfObject);
                }

                case 3: {
                    return new PdfFunction.Type3(pdfObject);
                }

                case 4: {
                    return new PdfFunction.Type4((PdfStream)pdfObject);
                }

                default: {
                    return null;
                }
            }
        }
    }
}
