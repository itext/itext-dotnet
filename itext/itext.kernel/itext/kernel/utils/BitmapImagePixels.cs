/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Utils {
    /// <summary>
    /// Class allows to process pixels of the bitmap image stored as byte array according to PDF
    /// specification.
    /// </summary>
    public class BitmapImagePixels {
        private const int BITS_IN_BYTE = 8;

        private const int DEFAULT_BITS_PER_COMPONENT = 8;

        private const int BYTE_WITH_LEADING_BIT = 0b10000000;

        // (x / 8) == (x >>> 3)
        private const int BITS_IN_BYTE_LOG = 3;

        // (x % 8) == (x & 0b00000111)
        private const int BIT_MASK = 0b00000111;

        private readonly int width;

        // Pdf spec: each row of sample data shall begin on a byte boundary. If the number of data bits
        // per row is not a multiple of 8, the end of the row is padded with extra bits to fill out the
        // last byte. A conforming reader shall ignore these padding bits.
        private readonly int bitsInRow;

        private readonly int height;

        private readonly int bitsPerComponent;

        private readonly int maxComponentValue;

        private readonly int numberOfComponents;

        private readonly byte[] data;

        /// <summary>Creates a representation of empty image.</summary>
        /// <param name="width">is a width of the image</param>
        /// <param name="height">is a height of the image</param>
        /// <param name="bitsPerComponent">is an amount of bits representing each color component of a pixel</param>
        /// <param name="numberOfComponents">is a number of components representing a pixel</param>
        public BitmapImagePixels(int width, int height, int bitsPerComponent, int numberOfComponents)
            : this(width, height, bitsPerComponent, numberOfComponents, null) {
        }

        /// <summary>
        /// Creates a representation of an image presented as
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>.
        /// </summary>
        /// <param name="image">
        /// is an image as
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// </param>
        public BitmapImagePixels(PdfImageXObject image)
            : this((int)MathematicUtil.Round(image.GetWidth()), (int)MathematicUtil.Round(image.GetHeight()), ObtainBitsPerComponent
                (image), ObtainNumberOfComponents(image), image.GetPdfObject().GetBytes()) {
        }

        /// <summary>Creates a representation of an image presented as bytes array.</summary>
        /// <param name="width">is a width of the image</param>
        /// <param name="height">is a height of the image</param>
        /// <param name="bitsPerComponent">is an amount of bits representing each color component of a pixel</param>
        /// <param name="numberOfComponents">is a number of components representing a pixel</param>
        /// <param name="data">is an image data</param>
        public BitmapImagePixels(int width, int height, int bitsPerComponent, int numberOfComponents, byte[] data) {
            this.width = width;
            this.height = height;
            this.bitsPerComponent = bitsPerComponent;
            this.maxComponentValue = (1 << this.bitsPerComponent) - 1;
            this.numberOfComponents = numberOfComponents;
            int rowLength = width * bitsPerComponent * numberOfComponents;
            if (rowLength % BITS_IN_BYTE != 0) {
                rowLength += BITS_IN_BYTE - (rowLength & BIT_MASK);
            }
            bitsInRow = rowLength;
            if (data == null) {
                this.data = new byte[(int)(((uint)(bitsInRow * height)) >> BITS_IN_BYTE_LOG)];
            }
            else {
                int expectedLength = bitsInRow * height;
                int actualLength = data.Length * BITS_IN_BYTE;
                if (expectedLength != actualLength) {
                    throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_DATA_LENGTH, expectedLength
                        , actualLength));
                }
                this.data = JavaUtil.ArraysCopyOf(data, data.Length);
            }
        }

        /// <summary>Gets pixel of the image.</summary>
        /// <param name="x">is an x-coordinate of a pixel to update</param>
        /// <param name="y">is a y-coordinate of a pixel to update</param>
        /// <returns>an array representing pixel color according to used color space</returns>
        public virtual double[] GetPixel(int x, int y) {
            long[] longArray = GetPixelAsLongs(x, y);
            double[] pixelArray = new double[longArray.Length];
            for (int i = 0; i < pixelArray.Length; i++) {
                pixelArray[i] = (double)longArray[i] / maxComponentValue;
            }
            return pixelArray;
        }

        /// <summary>Gets pixel of the image presented as long values.</summary>
        /// <param name="x">is an x-coordinate of a pixel to update</param>
        /// <param name="y">is a y-coordinate of a pixel to update</param>
        /// <returns>an array representing pixel color according to used color space</returns>
        public virtual long[] GetPixelAsLongs(int x, int y) {
            CheckCoordinates(x, y);
            long[] pixelArray = new long[numberOfComponents];
            for (int i = 0; i < pixelArray.Length; i++) {
                pixelArray[i] = ReadNumber(
                                // skip y rows from 0 to y-1
                                y * bitsInRow + 
                                // skip x pixels from 0 to (x-1)
                                x * bitsPerComponent * numberOfComponents + 
                                // skip i components of the current pixel from 0 to (i-1)
                                i * bitsPerComponent);
            }
            return pixelArray;
        }

        /// <summary>Updates a pixel of the image.</summary>
        /// <param name="x">is an x-coordinate of a pixel to update</param>
        /// <param name="y">is a y-coordinate of a pixel to update</param>
        /// <param name="value">
        /// is a pixel color. Pixel should be presented as double array according to used
        /// color space. Each value should be in range [0., 1.] (otherwise negative value
        /// will be replaced with 0. and large numbers are replaced with 1.)
        /// </param>
        public virtual void SetPixel(int x, int y, double[] value) {
            long[] longArray = new long[value.Length];
            for (int i = 0; i < value.Length; i++) {
                longArray[i] = (long)MathematicUtil.Round(value[i] * maxComponentValue);
            }
            SetPixel(x, y, longArray);
        }

        /// <summary>Updates a pixel of the image.</summary>
        /// <param name="x">is an x-coordinate of a pixel to update</param>
        /// <param name="y">is a y-coordinate of a pixel to update</param>
        /// <param name="value">
        /// is a pixel color. Pixel should be presented as long array according to used
        /// color space. Each value should be in range
        /// [0, <c>2 ^ bitsPerComponent</c> - 1] (otherwise negative value
        /// will be replaced with 0. and large numbers are replaced with maximum allowed
        /// value.)
        /// </param>
        public virtual void SetPixel(int x, int y, long[] value) {
            CheckCoordinates(x, y);
            CheckPixel(value);
            for (int i = 0; i < value.Length; i++) {
                WriteNumber(value[i], 
                                // skip y rows from 0 to y-1
                                y * bitsInRow + 
                                // skip x pixels from 0 to (x-1)
                                x * bitsPerComponent * numberOfComponents + 
                                // skip i components of the current pixel from 0 to (i-1)
                                i * bitsPerComponent);
            }
        }

        /// <summary>Getter for a width of the image.</summary>
        /// <returns>width of the image</returns>
        public virtual int GetWidth() {
            return width;
        }

        /// <summary>Getter for a height of the image.</summary>
        /// <returns>height of the image</returns>
        public virtual int GetHeight() {
            return height;
        }

        /// <summary>Getter for bits per component parameter of the image.</summary>
        /// <returns>bits per component parameter of the image</returns>
        public virtual int GetBitsPerComponent() {
            return bitsPerComponent;
        }

        /// <summary>Getter for number of components parameter of the image.</summary>
        /// <returns>number of components of the image</returns>
        public virtual int GetNumberOfComponents() {
            return numberOfComponents;
        }

        /// <summary>Getter for byte representation of the image.</summary>
        /// <returns>image data as byte array</returns>
        public virtual byte[] GetData() {
            return data;
        }

        /// <summary>Gets the maximum value for the component.</summary>
        /// <returns>maximum value of the component</returns>
        public virtual int GetMaxComponentValue() {
            return maxComponentValue;
        }

        private long ReadNumber(int index) {
            long result = 0;
            for (int i = 0; i < bitsPerComponent; i++) {
                result = (result << 1) + BooleanToInt(GetBit(index + i));
            }
            return result;
        }

        private void WriteNumber(long number, int index) {
            for (int bitNumber = 0; bitNumber < bitsPerComponent; bitNumber++) {
                int actualBitMask = 1 << (bitsPerComponent - bitNumber - 1);
                SetBit(index + bitNumber, (number & actualBitMask) != 0);
            }
        }

        private bool GetBit(int index) {
            return (data[(int)(((uint)index) >> BITS_IN_BYTE_LOG)] & 0xff & ((int)(((uint)BYTE_WITH_LEADING_BIT) >> (index
                 & BIT_MASK)))) != 0;
        }

        private void SetBit(int index, bool value) {
            if (value) {
                data[(int)(((uint)index) >> BITS_IN_BYTE_LOG)] |= (byte)((int)(((uint)BYTE_WITH_LEADING_BIT) >> (index & BIT_MASK
                    )));
            }
            else {
                data[(int)(((uint)index) >> BITS_IN_BYTE_LOG)] &= (byte)~((int)(((uint)BYTE_WITH_LEADING_BIT) >> (index & 
                    BIT_MASK)));
            }
        }

        private void CheckCoordinates(int x, int y) {
            if (x < 0 || x >= width || y < 0 || y > height) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS, 
                    x, y, width, height));
            }
        }

        private void CheckPixel(long[] pixel) {
            if (pixel.Length != numberOfComponents) {
                throw new ArgumentException(MessageFormatUtil.Format(KernelExceptionMessageConstant.LENGTH_OF_ARRAY_SHOULD_MATCH_NUMBER_OF_COMPONENTS
                    , pixel.Length, numberOfComponents));
            }
            for (int i = 0; i < pixel.Length; i++) {
                if (pixel[i] < 0) {
                    pixel[i] = 0;
                }
                if (pixel[i] > maxComponentValue) {
                    pixel[i] = maxComponentValue;
                }
            }
        }

        private static int ObtainBitsPerComponent(PdfImageXObject objectToProcess) {
            PdfStream imageStream = objectToProcess.GetPdfObject();
            PdfNumber bpc = imageStream.GetAsNumber(PdfName.BitsPerComponent);
            if (bpc == null) {
                return DEFAULT_BITS_PER_COMPONENT;
            }
            else {
                return bpc.IntValue();
            }
        }

        private static int ObtainNumberOfComponents(PdfImageXObject objectToProcess) {
            return PdfColorSpace.MakeColorSpace(objectToProcess.GetPdfObject().Get(PdfName.ColorSpace)).GetNumberOfComponents
                ();
        }

        private static int BooleanToInt(bool value) {
            return value ? 1 : 0;
        }
    }
}
