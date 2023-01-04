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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf.Function {
    public sealed class BaseInputOutPutConvertors {
        private BaseInputOutPutConvertors() {
        }

        public static BaseInputOutPutConvertors.IInputConversionFunction GetInputConvertor(int wordSize, double scaleFactor
            ) {
            return GetByteBasedInputConvertor(wordSize, scaleFactor * (1L << (wordSize * 8)) - 1);
        }

        public static BaseInputOutPutConvertors.IOutputConversionFunction GetOutputConvertor(int wordSize, double 
            scaleFactor) {
            return GetByteBasedOutputConvertor(wordSize, scaleFactor * (1L << (wordSize * 8)) - 1);
        }

        private static BaseInputOutPutConvertors.IInputConversionFunction GetByteBasedInputConvertor(int wordSize, 
            double scale) {
            return (input, o, l) => {
                if (o + l > input.Length) {
                    throw new ArgumentException(KernelExceptionMessageConstant.INVALID_LENGTH);
                }
                if (l % wordSize != 0) {
                    throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_LENGTH_FOR_WORDSIZE
                        , wordSize));
                }
                double[] @out = new double[l / wordSize];
                int inIndex = o;
                int outIndex = 0;
                while (inIndex < (l + o)) {
                    int val = 0;
                    for (int wordIndex = 0; wordIndex < wordSize; wordIndex++) {
                        val = ((val << 8) + (input[inIndex + wordIndex] & 0xff));
                        inIndex++;
                    }
                    @out[outIndex] = val / scale;
                    outIndex++;
                }
                return @out;
            }
            ;
        }

        private static BaseInputOutPutConvertors.IOutputConversionFunction GetByteBasedOutputConvertor(int wordSize
            , double scale) {
            return (input) => {
                byte[] @out = new byte[input.Length * wordSize];
                int inIndex = 0;
                int outIndex = 0;
                while (inIndex < input.Length && outIndex < @out.Length) {
                    int val = (int)(input[inIndex] * scale);
                    for (int wordIndex = 0; wordIndex < wordSize; wordIndex++) {
                        @out[outIndex++] = (byte)((int)(((uint)val) >> (wordIndex * 8)));
                    }
                    inIndex++;
                }
                return @out;
            }
            ;
        }

        public delegate double[] IInputConversionFunction(byte[] input, int offset, int length);

        public delegate byte[] IOutputConversionFunction(double[] input);
    }
}
