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
