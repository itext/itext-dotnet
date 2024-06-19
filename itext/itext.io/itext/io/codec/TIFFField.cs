/*
* Copyright 2003-2012 by Paulo Soares.
*
* This code was originally released in 2001 by SUN (see class
* com.sun.media.imageio.plugins.tiff.TIFFField.java)
* using the BSD license in a specific wording. In a mail dating from
* January 23, 2008, Brian Burkhalter (@sun.com) gave us permission
* to use the code under the following version of the BSD license:
*
* Copyright (c) 2006 Sun Microsystems, Inc. All  Rights Reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions
* are met:
*
* - Redistribution of source code must retain the above copyright
*   notice, this  list of conditions and the following disclaimer.
*
* - Redistribution in binary form must reproduce the above copyright
*   notice, this list of conditions and the following disclaimer in
*   the documentation and/or other materials provided with the
*   distribution.
*
* Neither the name of Sun Microsystems, Inc. or the names of
* contributors may be used to endorse or promote products derived
* from this software without specific prior written permission.
*
* This software is provided "AS IS," without a warranty of any
* kind. ALL EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND
* WARRANTIES, INCLUDING ANY IMPLIED WARRANTY OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE OR NON-INFRINGEMENT, ARE HEREBY
* EXCLUDED. SUN MIDROSYSTEMS, INC. ("SUN") AND ITS LICENSORS SHALL
* NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF
* USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
* DERIVATIVES. IN NO EVENT WILL SUN OR ITS LICENSORS BE LIABLE FOR
* ANY LOST REVENUE, PROFIT OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL,
* CONSEQUENTIAL, INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND
* REGARDLESS OF THE THEORY OF LIABILITY, ARISING OUT OF THE USE OF OR
* INABILITY TO USE THIS SOFTWARE, EVEN IF SUN HAS BEEN ADVISED OF THE
* POSSIBILITY OF SUCH DAMAGES.
*
* You acknowledge that this software is not designed or intended for
* use in the design, construction, operation or maintenance of any
* nuclear facility.
*/
using System;

namespace iText.IO.Codec {
    /// <summary>A class representing a field in a TIFF 6.0 Image File Directory.</summary>
    /// <remarks>
    /// A class representing a field in a TIFF 6.0 Image File Directory.
    /// <para /> The TIFF file format is described in more detail in the
    /// comments for the TIFFDescriptor class.
    /// <para /> A field in a TIFF Image File Directory (IFD).  A field is defined
    /// as a sequence of values of identical data type.  TIFF 6.0 defines
    /// 12 data types, which are mapped internally onto the Java data types
    /// byte, int, long, float, and double.
    /// <para /><b> This class is not a committed part of the JAI API.  It may
    /// be removed or changed in future releases of JAI.</b>
    /// </remarks>
    /// <seealso cref="TIFFDirectory"/>
    public class TIFFField : IComparable<iText.IO.Codec.TIFFField> {
        /// <summary>Flag for 8 bit unsigned integers.</summary>
        public const int TIFF_BYTE = 1;

        /// <summary>Flag for null-terminated ASCII strings.</summary>
        public const int TIFF_ASCII = 2;

        /// <summary>Flag for 16 bit unsigned integers.</summary>
        public const int TIFF_SHORT = 3;

        /// <summary>Flag for 32 bit unsigned integers.</summary>
        public const int TIFF_LONG = 4;

        /// <summary>Flag for pairs of 32 bit unsigned integers.</summary>
        public const int TIFF_RATIONAL = 5;

        /// <summary>Flag for 8 bit signed integers.</summary>
        public const int TIFF_SBYTE = 6;

        /// <summary>Flag for 8 bit uninterpreted bytes.</summary>
        public const int TIFF_UNDEFINED = 7;

        /// <summary>Flag for 16 bit signed integers.</summary>
        public const int TIFF_SSHORT = 8;

        /// <summary>Flag for 32 bit signed integers.</summary>
        public const int TIFF_SLONG = 9;

        /// <summary>Flag for pairs of 32 bit signed integers.</summary>
        public const int TIFF_SRATIONAL = 10;

        /// <summary>Flag for 32 bit IEEE floats.</summary>
        public const int TIFF_FLOAT = 11;

        /// <summary>Flag for 64 bit IEEE doubles.</summary>
        public const int TIFF_DOUBLE = 12;

//\cond DO_NOT_DOCUMENT
        /// <summary>The tag number.</summary>
        internal int tag;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The tag type.</summary>
        internal int type;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The number of data items present in the field.</summary>
        internal int count;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The field data.</summary>
        internal Object data;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The default constructor.</summary>
        internal TIFFField() {
        }
//\endcond

        /// <summary>Constructs a TIFFField with arbitrary data.</summary>
        /// <remarks>
        /// Constructs a TIFFField with arbitrary data.  The data
        /// parameter must be an array of a Java type appropriate for the
        /// type of the TIFF field.  Since there is no available 32-bit
        /// unsigned data type, long is used. The mapping between types is
        /// as follows:
        /// <table border="1" summary="tifffield data">
        /// <tr>
        /// <th> TIFF type </th> <th> Java type </th>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_BYTE</tt></td>      <td><tt>byte</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_ASCII</tt></td>     <td><tt>String</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_SHORT</tt></td>     <td><tt>char</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_LONG</tt></td>      <td><tt>long</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_RATIONAL</tt></td>  <td><tt>long[2]</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_SBYTE</tt></td>     <td><tt>byte</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_UNDEFINED</tt></td> <td><tt>byte</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_SSHORT</tt></td>    <td><tt>short</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_SLONG</tt></td>     <td><tt>int</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_SRATIONAL</tt></td> <td><tt>int[2]</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_FLOAT</tt></td>     <td><tt>float</tt></td>
        /// </tr>
        /// <tr>
        /// <td><tt>TIFF_DOUBLE</tt></td>    <td><tt>double</tt></td>
        /// </tr>
        /// </table>
        /// </remarks>
        /// <param name="tag">the tag number</param>
        /// <param name="type">the tag type</param>
        /// <param name="count">the number of data items present in the field</param>
        /// <param name="data">the field data</param>
        public TIFFField(int tag, int type, int count, Object data) {
            this.tag = tag;
            this.type = type;
            this.count = count;
            this.data = data;
        }

        /// <summary>Returns the tag number</summary>
        /// <returns>the tag number, between 0 and 65535.</returns>
        public virtual int GetTag() {
            return tag;
        }

        /// <summary>Returns the type of the data stored in the IFD.</summary>
        /// <remarks>
        /// Returns the type of the data stored in the IFD.
        /// For a TIFF6.0 file, the value will equal one of the
        /// TIFF_ constants defined in this class.  For future
        /// revisions of TIFF, higher values are possible.
        /// </remarks>
        /// <returns>The type of the data stored in the IFD</returns>
        public virtual int GetFieldType() {
            return type;
        }

        /// <summary>Returns the number of elements in the IFD.</summary>
        /// <returns>The number of elements in the IFD</returns>
        public virtual int GetCount() {
            return count;
        }

        /// <summary>Returns the data as an uninterpreted array of bytes.</summary>
        /// <remarks>
        /// Returns the data as an uninterpreted array of bytes.
        /// The type of the field must be one of TIFF_BYTE, TIFF_SBYTE,
        /// or TIFF_UNDEFINED;
        /// <para /> For data in TIFF_BYTE format, the application must take
        /// care when promoting the data to longer integral types
        /// to avoid sign extension.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_BYTE, TIFF_SBYTE, or TIFF_UNDEFINED.
        /// </remarks>
        /// <returns>the data as an uninterpreted array of bytes</returns>
        public virtual byte[] GetAsBytes() {
            return (byte[])data;
        }

        /// <summary>
        /// Returns TIFF_SHORT data as an array of chars (unsigned 16-bit
        /// integers).
        /// </summary>
        /// <remarks>
        /// Returns TIFF_SHORT data as an array of chars (unsigned 16-bit
        /// integers).
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_SHORT.
        /// </remarks>
        /// <returns>TIFF_SHORT data as an array of chars</returns>
        public virtual char[] GetAsChars() {
            return (char[])data;
        }

        /// <summary>
        /// Returns TIFF_SSHORT data as an array of shorts (signed 16-bit
        /// integers).
        /// </summary>
        /// <remarks>
        /// Returns TIFF_SSHORT data as an array of shorts (signed 16-bit
        /// integers).
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_SSHORT.
        /// </remarks>
        /// <returns>TIFF_SSHORT data as an array of shorts (signed 16-bit integers).</returns>
        public virtual short[] GetAsShorts() {
            return (short[])data;
        }

        /// <summary>
        /// Returns TIFF_SLONG data as an array of ints (signed 32-bit
        /// integers).
        /// </summary>
        /// <remarks>
        /// Returns TIFF_SLONG data as an array of ints (signed 32-bit
        /// integers).
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_SLONG.
        /// </remarks>
        /// <returns>TIFF_SLONG data as an array of ints (signed 32-bit integers).</returns>
        public virtual int[] GetAsInts() {
            return (int[])data;
        }

        /// <summary>
        /// Returns TIFF_LONG data as an array of longs (signed 64-bit
        /// integers).
        /// </summary>
        /// <remarks>
        /// Returns TIFF_LONG data as an array of longs (signed 64-bit
        /// integers).
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_LONG.
        /// </remarks>
        /// <returns>TIFF_LONG data as an array of longs (signed 64-bit integers).</returns>
        public virtual long[] GetAsLongs() {
            return (long[])data;
        }

        /// <summary>Returns TIFF_FLOAT data as an array of floats.</summary>
        /// <remarks>
        /// Returns TIFF_FLOAT data as an array of floats.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_FLOAT.
        /// </remarks>
        /// <returns>TIFF_FLOAT data as an array of floats.</returns>
        public virtual float[] GetAsFloats() {
            return (float[])data;
        }

        /// <summary>Returns TIFF_DOUBLE data as an array of doubles.</summary>
        /// <remarks>
        /// Returns TIFF_DOUBLE data as an array of doubles.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_DOUBLE.
        /// </remarks>
        /// <returns>TIFF_DOUBLE data as an array of doubles.</returns>
        public virtual double[] GetAsDoubles() {
            return (double[])data;
        }

        /// <summary>Returns TIFF_ASCII data as an array of strings.</summary>
        /// <remarks>
        /// Returns TIFF_ASCII data as an array of strings.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_ASCII.
        /// </remarks>
        /// <returns>TIFF_ASCII data as an array of strings.</returns>
        public virtual String[] GetAsStrings() {
            return (String[])data;
        }

        /// <summary>Returns TIFF_SRATIONAL data as an array of 2-element arrays of ints.</summary>
        /// <remarks>
        /// Returns TIFF_SRATIONAL data as an array of 2-element arrays of ints.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_SRATIONAL.
        /// </remarks>
        /// <returns>TIFF_SRATIONAL data as an array of 2-element arrays of ints.</returns>
        public virtual int[][] GetAsSRationals() {
            return (int[][])data;
        }

        /// <summary>Returns TIFF_RATIONAL data as an array of 2-element arrays of longs.</summary>
        /// <remarks>
        /// Returns TIFF_RATIONAL data as an array of 2-element arrays of longs.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_RATTIONAL.
        /// </remarks>
        /// <returns>TIFF_RATIONAL data as an array of 2-element arrays of longs.</returns>
        public virtual long[][] GetAsRationals() {
            return (long[][])data;
        }

        /// <summary>
        /// Returns data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
        /// TIFF_SSHORT, or TIFF_SLONG format as an int.
        /// </summary>
        /// <remarks>
        /// Returns data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
        /// TIFF_SSHORT, or TIFF_SLONG format as an int.
        /// <para /> TIFF_BYTE and TIFF_UNDEFINED data are treated as unsigned;
        /// that is, no sign extension will take place and the returned
        /// value will be in the range [0, 255].  TIFF_SBYTE data will
        /// be returned in the range [-128, 127].
        /// <para /> A ClassCastException will be thrown if the field is not of
        /// type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
        /// TIFF_SSHORT, or TIFF_SLONG.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>
        /// data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT, TIFF_SSHORT,
        /// or TIFF_SLONG format as an int.
        /// </returns>
        public virtual int GetAsInt(int index) {
            switch (type) {
                case TIFF_BYTE:
                case TIFF_UNDEFINED: {
                    return ((byte[])data)[index] & 0xff;
                }

                case TIFF_SBYTE: {
                    return ((byte[])data)[index];
                }

                case TIFF_SHORT: {
                    return ((char[])data)[index] & 0xffff;
                }

                case TIFF_SSHORT: {
                    return ((short[])data)[index];
                }

                case TIFF_SLONG: {
                    return ((int[])data)[index];
                }

                default: {
                    throw new InvalidCastException();
                }
            }
        }

        /// <summary>
        /// Returns data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
        /// TIFF_SSHORT, TIFF_SLONG, or TIFF_LONG format as a long.
        /// </summary>
        /// <remarks>
        /// Returns data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
        /// TIFF_SSHORT, TIFF_SLONG, or TIFF_LONG format as a long.
        /// <para /> TIFF_BYTE and TIFF_UNDEFINED data are treated as unsigned;
        /// that is, no sign extension will take place and the returned
        /// value will be in the range [0, 255].  TIFF_SBYTE data will
        /// be returned in the range [-128, 127].
        /// <para /> A ClassCastException will be thrown if the field is not of
        /// type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
        /// TIFF_SSHORT, TIFF_SLONG, or TIFF_LONG.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>
        /// data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG,
        /// or TIFF_LONG format as a long.
        /// </returns>
        public virtual long GetAsLong(int index) {
            switch (type) {
                case TIFF_BYTE:
                case TIFF_UNDEFINED: {
                    return ((byte[])data)[index] & 0xff;
                }

                case TIFF_SBYTE: {
                    return ((byte[])data)[index];
                }

                case TIFF_SHORT: {
                    return ((char[])data)[index] & 0xffff;
                }

                case TIFF_SSHORT: {
                    return ((short[])data)[index];
                }

                case TIFF_SLONG: {
                    return ((int[])data)[index];
                }

                case TIFF_LONG: {
                    return ((long[])data)[index];
                }

                default: {
                    throw new InvalidCastException();
                }
            }
        }

        /// <summary>Returns data in any numerical format as a float.</summary>
        /// <remarks>
        /// Returns data in any numerical format as a float. Data in
        /// TIFF_SRATIONAL or TIFF_RATIONAL format are evaluated by
        /// dividing the numerator into the denominator using
        /// double-precision arithmetic and then truncating to single
        /// precision. Data in TIFF_SLONG, TIFF_LONG, or TIFF_DOUBLE
        /// format may suffer from truncation.
        /// <para /> A ClassCastException will be thrown if the field is
        /// of type TIFF_UNDEFINED or TIFF_ASCII.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>data in any numerical format as a float.</returns>
        public virtual float GetAsFloat(int index) {
            switch (type) {
                case TIFF_BYTE: {
                    return ((byte[])data)[index] & 0xff;
                }

                case TIFF_SBYTE: {
                    return ((byte[])data)[index];
                }

                case TIFF_SHORT: {
                    return ((char[])data)[index] & 0xffff;
                }

                case TIFF_SSHORT: {
                    return ((short[])data)[index];
                }

                case TIFF_SLONG: {
                    return ((int[])data)[index];
                }

                case TIFF_LONG: {
                    return ((long[])data)[index];
                }

                case TIFF_FLOAT: {
                    return ((float[])data)[index];
                }

                case TIFF_DOUBLE: {
                    return (float)((double[])data)[index];
                }

                case TIFF_SRATIONAL: {
                    int[] ivalue = GetAsSRational(index);
                    return (float)((double)ivalue[0] / ivalue[1]);
                }

                case TIFF_RATIONAL: {
                    long[] lvalue = GetAsRational(index);
                    return (float)((double)lvalue[0] / lvalue[1]);
                }

                default: {
                    throw new InvalidCastException();
                }
            }
        }

        /// <summary>Returns data in any numerical format as a double.</summary>
        /// <remarks>
        /// Returns data in any numerical format as a double. Data in
        /// TIFF_SRATIONAL or TIFF_RATIONAL format are evaluated by
        /// dividing the numerator into the denominator using
        /// double-precision arithmetic.
        /// <para /> A ClassCastException will be thrown if the field is of
        /// type TIFF_UNDEFINED or TIFF_ASCII.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>data in any numerical format as a double.</returns>
        public virtual double GetAsDouble(int index) {
            switch (type) {
                case TIFF_BYTE: {
                    return ((byte[])data)[index] & 0xff;
                }

                case TIFF_SBYTE: {
                    return ((byte[])data)[index];
                }

                case TIFF_SHORT: {
                    return ((char[])data)[index] & 0xffff;
                }

                case TIFF_SSHORT: {
                    return ((short[])data)[index];
                }

                case TIFF_SLONG: {
                    return ((int[])data)[index];
                }

                case TIFF_LONG: {
                    return ((long[])data)[index];
                }

                case TIFF_FLOAT: {
                    return ((float[])data)[index];
                }

                case TIFF_DOUBLE: {
                    return ((double[])data)[index];
                }

                case TIFF_SRATIONAL: {
                    int[] ivalue = GetAsSRational(index);
                    return (double)ivalue[0] / ivalue[1];
                }

                case TIFF_RATIONAL: {
                    long[] lvalue = GetAsRational(index);
                    return (double)lvalue[0] / lvalue[1];
                }

                default: {
                    throw new InvalidCastException();
                }
            }
        }

        /// <summary>Returns a TIFF_ASCII data item as a String.</summary>
        /// <remarks>
        /// Returns a TIFF_ASCII data item as a String.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_ASCII.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>a TIFF_ASCII data item as a String.</returns>
        public virtual String GetAsString(int index) {
            return ((String[])data)[index];
        }

        /// <summary>
        /// Returns a TIFF_SRATIONAL data item as a two-element array
        /// of ints.
        /// </summary>
        /// <remarks>
        /// Returns a TIFF_SRATIONAL data item as a two-element array
        /// of ints.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_SRATIONAL.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>a TIFF_SRATIONAL data item as a two-element array of ints.</returns>
        public virtual int[] GetAsSRational(int index) {
            return ((int[][])data)[index];
        }

        /// <summary>
        /// Returns a TIFF_RATIONAL data item as a two-element array
        /// of ints.
        /// </summary>
        /// <remarks>
        /// Returns a TIFF_RATIONAL data item as a two-element array
        /// of ints.
        /// <para /> A ClassCastException will be thrown if the field is not
        /// of type TIFF_RATIONAL.
        /// </remarks>
        /// <param name="index">The index</param>
        /// <returns>a TIFF_RATIONAL data item as a two-element array of ints</returns>
        public virtual long[] GetAsRational(int index) {
            if (type == TIFF_LONG) {
                return GetAsLongs();
            }
            return ((long[][])data)[index];
        }

        /// <summary>
        /// Compares this <c>TIFFField</c> with another
        /// <c>TIFFField</c> by comparing the tags.
        /// </summary>
        /// <remarks>
        /// Compares this <c>TIFFField</c> with another
        /// <c>TIFFField</c> by comparing the tags.
        /// <para /><b>Note: this class has a natural ordering that is inconsistent
        /// with <c>equals()</c>.</b>
        /// </remarks>
        public virtual int CompareTo(iText.IO.Codec.TIFFField o) {
            if (o == null) {
                throw new ArgumentException();
            }
            int oTag = o.GetTag();
            if (tag < oTag) {
                return -1;
            }
            else {
                if (tag > oTag) {
                    return 1;
                }
                else {
                    return 0;
                }
            }
        }
    }
}
