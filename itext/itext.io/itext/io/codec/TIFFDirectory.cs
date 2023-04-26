/*
* Copyright 2003-2012 by Paulo Soares.
*
* This code was originally released in 2001 by SUN (see class
* com.sun.media.imageio.plugins.tiff.TIFFDirectory.java)
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
using System.Collections.Generic;
using System.IO;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Codec {
    /// <summary>
    /// A class representing an Image File Directory (IFD) from a TIFF 6.0
    /// stream.
    /// </summary>
    /// <remarks>
    /// A class representing an Image File Directory (IFD) from a TIFF 6.0
    /// stream.  The TIFF file format is described in more detail in the
    /// comments for the TIFFDescriptor class.
    /// <br />
    /// <para /> A TIFF IFD consists of a set of TIFFField tags.  Methods are
    /// provided to query the set of tags and to obtain the raw field
    /// array.  In addition, convenience methods are provided for acquiring
    /// the values of tags that contain a single value that fits into a
    /// byte, int, long, float, or double.
    /// <br />
    /// <para /> Every TIFF file is made up of one or more public IFDs that are
    /// joined in a linked list, rooted in the file header.  A file may
    /// also contain so-called private IFDs that are referenced from
    /// tag data and do not appear in the main list.
    /// <br />
    /// <para /><b> This class is not a committed part of the JAI API.  It may
    /// be removed or changed in future releases of JAI.</b>
    /// </remarks>
    /// <seealso cref="TIFFField"/>
    public class TIFFDirectory {
        /// <summary>A boolean storing the endianness of the stream.</summary>
        internal bool isBigEndian;

        /// <summary>The number of entries in the IFD.</summary>
        internal int numEntries;

        /// <summary>An array of TIFFFields.</summary>
        internal TIFFField[] fields;

        /// <summary>A Hashtable indexing the fields by tag number.</summary>
        internal IDictionary<int, int?> fieldIndex = new Dictionary<int, int?>();

        /// <summary>The offset of this IFD.</summary>
        internal long IFDOffset = 8;

        /// <summary>The offset of the next IFD.</summary>
        internal long nextIFDOffset = 0;

        /// <summary>The default constructor.</summary>
        internal TIFFDirectory() {
        }

        private static bool IsValidEndianTag(int endian) {
            return endian == 0x4949 || endian == 0x4d4d;
        }

        /// <summary>Constructs a TIFFDirectory from a SeekableStream.</summary>
        /// <remarks>
        /// Constructs a TIFFDirectory from a SeekableStream.
        /// The directory parameter specifies which directory to read from
        /// the linked list present in the stream; directory 0 is normally
        /// read but it is possible to store multiple images in a single
        /// TIFF file by maintaining multiple directories.
        /// </remarks>
        /// <param name="stream">a SeekableStream to read from.</param>
        /// <param name="directory">the index of the directory to read.</param>
        public TIFFDirectory(RandomAccessFileOrArray stream, int directory) {
            long global_save_offset = stream.GetPosition();
            long ifd_offset;
            // Read the TIFF header
            stream.Seek(0L);
            int endian = stream.ReadUnsignedShort();
            if (!IsValidEndianTag(endian)) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BAD_ENDIANNESS_TAG_0X4949_OR_0X4D4D);
            }
            isBigEndian = endian == 0x4d4d;
            int magic = ReadUnsignedShort(stream);
            if (magic != 42) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BAD_MAGIC_NUMBER_SHOULD_BE_42);
            }
            // Get the initial ifd offset as an unsigned int (using a long)
            ifd_offset = ReadUnsignedInt(stream);
            for (int i = 0; i < directory; i++) {
                if (ifd_offset == 0L) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.DIRECTORY_NUMBER_IS_TOO_LARGE);
                }
                stream.Seek(ifd_offset);
                int entries = ReadUnsignedShort(stream);
                stream.Skip(12 * entries);
                ifd_offset = ReadUnsignedInt(stream);
            }
            stream.Seek(ifd_offset);
            Initialize(stream);
            stream.Seek(global_save_offset);
        }

        /// <summary>Constructs a TIFFDirectory by reading a SeekableStream.</summary>
        /// <remarks>
        /// Constructs a TIFFDirectory by reading a SeekableStream.
        /// The ifd_offset parameter specifies the stream offset from which
        /// to begin reading; this mechanism is sometimes used to store
        /// private IFDs within a TIFF file that are not part of the normal
        /// sequence of IFDs.
        /// </remarks>
        /// <param name="stream">a SeekableStream to read from.</param>
        /// <param name="ifd_offset">the long byte offset of the directory.</param>
        /// <param name="directory">
        /// the index of the directory to read beyond the
        /// one at the current stream offset; zero indicates the IFD
        /// at the current offset.
        /// </param>
        public TIFFDirectory(RandomAccessFileOrArray stream, long ifd_offset, int directory) {
            long global_save_offset = stream.GetPosition();
            stream.Seek(0L);
            int endian = stream.ReadUnsignedShort();
            if (!IsValidEndianTag(endian)) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BAD_ENDIANNESS_TAG_0X4949_OR_0X4D4D);
            }
            isBigEndian = endian == 0x4d4d;
            // Seek to the first IFD.
            stream.Seek(ifd_offset);
            // Seek to desired IFD if necessary.
            int dirNum = 0;
            while (dirNum < directory) {
                // Get the number of fields in the current IFD.
                int numEntries = ReadUnsignedShort(stream);
                // Skip to the next IFD offset value field.
                stream.Seek(ifd_offset + 12 * numEntries);
                // Read the offset to the next IFD beyond this one.
                ifd_offset = ReadUnsignedInt(stream);
                // Seek to the next IFD.
                stream.Seek(ifd_offset);
                // Increment the directory.
                dirNum++;
            }
            Initialize(stream);
            stream.Seek(global_save_offset);
        }

        private static readonly int[] sizeOfType = new int[] { 
                //  0 = n/a
                0, 
                //  1 = byte
                1, 
                //  2 = ascii
                1, 
                //  3 = short
                2, 
                //  4 = long
                4, 
                //  5 = rational
                8, 
                //  6 = sbyte
                1, 
                //  7 = undefined
                1, 
                //  8 = sshort
                2, 
                //  9 = slong
                4, 
                // 10 = srational
                8, 
                // 11 = float
                4, 
                // 12 = double
                8 };

        private void Initialize(RandomAccessFileOrArray stream) {
            long nextTagOffset = 0L;
            long maxOffset = stream.Length();
            int i;
            int j;
            IFDOffset = stream.GetPosition();
            numEntries = ReadUnsignedShort(stream);
            fields = new TIFFField[numEntries];
            for (i = 0; i < numEntries && nextTagOffset < maxOffset; i++) {
                int tag = ReadUnsignedShort(stream);
                int type = ReadUnsignedShort(stream);
                int count = (int)ReadUnsignedInt(stream);
                bool processTag = true;
                // The place to return to to read the next tag
                nextTagOffset = stream.GetPosition() + 4;
                try {
                    // If the tag data can't fit in 4 bytes, the next 4 bytes
                    // contain the starting offset of the data
                    if (count * sizeOfType[type] > 4) {
                        long valueOffset = ReadUnsignedInt(stream);
                        // bounds check offset for EOF
                        if (valueOffset < maxOffset) {
                            stream.Seek(valueOffset);
                        }
                        else {
                            // bad offset pointer .. skip tag
                            processTag = false;
                        }
                    }
                }
                catch (IndexOutOfRangeException) {
                    // if the data type is unknown we should skip this TIFF Field
                    processTag = false;
                }
                if (processTag) {
                    fieldIndex.Put(tag, i);
                    Object obj = null;
                    switch (type) {
                        case TIFFField.TIFF_BYTE:
                        case TIFFField.TIFF_SBYTE:
                        case TIFFField.TIFF_UNDEFINED:
                        case TIFFField.TIFF_ASCII: {
                            byte[] bvalues = new byte[count];
                            stream.ReadFully(bvalues, 0, count);
                            if (type == TIFFField.TIFF_ASCII) {
                                // Can be multiple strings
                                int index = 0;
                                int prevIndex = 0;
                                IList<String> v = new List<String>();
                                while (index < count) {
                                    while (index < count && bvalues[index++] != 0) {
                                    }
                                    // When we encountered zero, means one string has ended
                                    v.Add(iText.Commons.Utils.JavaUtil.GetStringForBytes(bvalues, prevIndex, (index - prevIndex)));
                                    prevIndex = index;
                                }
                                count = v.Count;
                                String[] strings = new String[count];
                                for (int c = 0; c < count; c++) {
                                    strings[c] = v[c];
                                }
                                obj = strings;
                            }
                            else {
                                obj = bvalues;
                            }
                            break;
                        }

                        case TIFFField.TIFF_SHORT: {
                            char[] cvalues = new char[count];
                            for (j = 0; j < count; j++) {
                                cvalues[j] = (char)ReadUnsignedShort(stream);
                            }
                            obj = cvalues;
                            break;
                        }

                        case TIFFField.TIFF_LONG: {
                            long[] lvalues = new long[count];
                            for (j = 0; j < count; j++) {
                                lvalues[j] = ReadUnsignedInt(stream);
                            }
                            obj = lvalues;
                            break;
                        }

                        case TIFFField.TIFF_RATIONAL: {
                            long[][] llvalues = new long[count][];
                            for (j = 0; j < count; j++) {
                                llvalues[j] = new long[2];
                                llvalues[j][0] = ReadUnsignedInt(stream);
                                llvalues[j][1] = ReadUnsignedInt(stream);
                            }
                            obj = llvalues;
                            break;
                        }

                        case TIFFField.TIFF_SSHORT: {
                            short[] svalues = new short[count];
                            for (j = 0; j < count; j++) {
                                svalues[j] = ReadShort(stream);
                            }
                            obj = svalues;
                            break;
                        }

                        case TIFFField.TIFF_SLONG: {
                            int[] ivalues = new int[count];
                            for (j = 0; j < count; j++) {
                                ivalues[j] = ReadInt(stream);
                            }
                            obj = ivalues;
                            break;
                        }

                        case TIFFField.TIFF_SRATIONAL: {
                            int[][] iivalues = new int[count][];
                            for (j = 0; j < count; j++) {
                                iivalues[j] = new int[2];
                                iivalues[j][0] = ReadInt(stream);
                                iivalues[j][1] = ReadInt(stream);
                            }
                            obj = iivalues;
                            break;
                        }

                        case TIFFField.TIFF_FLOAT: {
                            float[] fvalues = new float[count];
                            for (j = 0; j < count; j++) {
                                fvalues[j] = ReadFloat(stream);
                            }
                            obj = fvalues;
                            break;
                        }

                        case TIFFField.TIFF_DOUBLE: {
                            double[] dvalues = new double[count];
                            for (j = 0; j < count; j++) {
                                dvalues[j] = ReadDouble(stream);
                            }
                            obj = dvalues;
                            break;
                        }

                        default: {
                            break;
                        }
                    }
                    fields[i] = new TIFFField(tag, type, count, obj);
                }
                stream.Seek(nextTagOffset);
            }
            // Read the offset of the next IFD.
            try {
                nextIFDOffset = ReadUnsignedInt(stream);
            }
            catch (Exception) {
                // broken tiffs may not have this pointer
                nextIFDOffset = 0;
            }
        }

        /// <summary>Returns the number of directory entries.</summary>
        /// <returns>The number of directory entries</returns>
        public virtual int GetNumEntries() {
            return numEntries;
        }

        /// <summary>
        /// Returns the value of a given tag as a TIFFField,
        /// or null if the tag is not present.
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The value of the given tag as a TIFFField or null</returns>
        public virtual TIFFField GetField(int tag) {
            int? i = fieldIndex.Get(tag);
            if (i == null) {
                return null;
            }
            else {
                return fields[(int)i];
            }
        }

        /// <summary>Returns true if a tag appears in the directory.</summary>
        /// <param name="tag">The tag</param>
        /// <returns>True if the tag appears in the directory, false otherwise</returns>
        public virtual bool IsTagPresent(int tag) {
            return fieldIndex.ContainsKey(tag);
        }

        /// <summary>
        /// Returns an ordered array of integers indicating the tags
        /// values.
        /// </summary>
        /// <returns>an ordered array of integers indicating the tags</returns>
        public virtual int[] GetTags() {
            int[] tags = new int[fieldIndex.Count];
            int i = 0;
            foreach (int? integer in fieldIndex.Keys) {
                tags[i++] = (int)integer;
            }
            return tags;
        }

        /// <summary>
        /// Returns an array of TIFFFields containing all the fields
        /// in this directory.
        /// </summary>
        /// <returns>an array of TIFFFields containing all the fields in this directory</returns>
        public virtual TIFFField[] GetFields() {
            return fields;
        }

        /// <summary>
        /// Returns the value of a particular index of a given tag as a
        /// byte.
        /// </summary>
        /// <remarks>
        /// Returns the value of a particular index of a given tag as a
        /// byte.  The caller is responsible for ensuring that the tag is
        /// present and has type TIFFField.TIFF_SBYTE, TIFF_BYTE, or
        /// TIFF_UNDEFINED.
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <param name="index">The index</param>
        /// <returns>the value of a particular index of a given tag as a byte</returns>
        public virtual byte GetFieldAsByte(int tag, int index) {
            int? i = fieldIndex.Get(tag);
            byte[] b = fields[(int)i].GetAsBytes();
            return b[index];
        }

        /// <summary>
        /// Returns the value of index 0 of a given tag as a
        /// byte.
        /// </summary>
        /// <remarks>
        /// Returns the value of index 0 of a given tag as a
        /// byte.  The caller is responsible for ensuring that the tag is
        /// present and has  type TIFFField.TIFF_SBYTE, TIFF_BYTE, or
        /// TIFF_UNDEFINED.
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <returns>The value of index 0 of the given tag as a byte</returns>
        public virtual byte GetFieldAsByte(int tag) {
            return GetFieldAsByte(tag, 0);
        }

        /// <summary>
        /// Returns the value of a particular index of a given tag as a
        /// long.
        /// </summary>
        /// <remarks>
        /// Returns the value of a particular index of a given tag as a
        /// long.  The caller is responsible for ensuring that the tag is
        /// present and has type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED,
        /// TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG or TIFF_LONG.
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <param name="index">The index</param>
        /// <returns>The value of the given index of the given tag as a long</returns>
        public virtual long GetFieldAsLong(int tag, int index) {
            int? i = fieldIndex.Get(tag);
            return fields[(int)i].GetAsLong(index);
        }

        /// <summary>
        /// Returns the value of index 0 of a given tag as a
        /// long.
        /// </summary>
        /// <remarks>
        /// Returns the value of index 0 of a given tag as a
        /// long.  The caller is responsible for ensuring that the tag is
        /// present and has type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED,
        /// TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG or TIFF_LONG.
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <returns>The value of index 0 of the given tag as a long</returns>
        public virtual long GetFieldAsLong(int tag) {
            return GetFieldAsLong(tag, 0);
        }

        /// <summary>
        /// Returns the value of a particular index of a given tag as a
        /// float.
        /// </summary>
        /// <remarks>
        /// Returns the value of a particular index of a given tag as a
        /// float.  The caller is responsible for ensuring that the tag is
        /// present and has numeric type (all but TIFF_UNDEFINED and
        /// TIFF_ASCII).
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <param name="index">The index</param>
        /// <returns>The value of the given index of the given tag as a float</returns>
        public virtual float GetFieldAsFloat(int tag, int index) {
            int? i = fieldIndex.Get(tag);
            return fields[(int)i].GetAsFloat(index);
        }

        /// <summary>Returns the value of index 0 of a given tag as a float.</summary>
        /// <remarks>
        /// Returns the value of index 0 of a given tag as a float.  The
        /// caller is responsible for ensuring that the tag is present and
        /// has numeric type (all but TIFF_UNDEFINED and TIFF_ASCII).
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <returns>The value of index 0 of the given tag as a float</returns>
        public virtual float GetFieldAsFloat(int tag) {
            return GetFieldAsFloat(tag, 0);
        }

        /// <summary>
        /// Returns the value of a particular index of a given tag as a
        /// double.
        /// </summary>
        /// <remarks>
        /// Returns the value of a particular index of a given tag as a
        /// double.  The caller is responsible for ensuring that the tag is
        /// present and has numeric type (all but TIFF_UNDEFINED and
        /// TIFF_ASCII).
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <param name="index">The index</param>
        /// <returns>The value of the given index of the given tag as a double</returns>
        public virtual double GetFieldAsDouble(int tag, int index) {
            int? i = fieldIndex.Get(tag);
            return fields[(int)i].GetAsDouble(index);
        }

        /// <summary>Returns the value of index 0 of a given tag as a double.</summary>
        /// <remarks>
        /// Returns the value of index 0 of a given tag as a double.  The
        /// caller is responsible for ensuring that the tag is present and
        /// has numeric type (all but TIFF_UNDEFINED and TIFF_ASCII).
        /// </remarks>
        /// <param name="tag">The tag</param>
        /// <returns>The value of index 0 of the given tag as a double</returns>
        public virtual double GetFieldAsDouble(int tag) {
            return GetFieldAsDouble(tag, 0);
        }

        // Methods to read primitive data types from the stream
        private short ReadShort(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadShort();
            }
            else {
                return stream.ReadShortLE();
            }
        }

        private int ReadUnsignedShort(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadUnsignedShort();
            }
            else {
                return stream.ReadUnsignedShortLE();
            }
        }

        private int ReadInt(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadInt();
            }
            else {
                return stream.ReadIntLE();
            }
        }

        private long ReadUnsignedInt(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadUnsignedInt();
            }
            else {
                return stream.ReadUnsignedIntLE();
            }
        }

        private long ReadLong(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadLong();
            }
            else {
                return stream.ReadLongLE();
            }
        }

        private float ReadFloat(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadFloat();
            }
            else {
                return stream.ReadFloatLE();
            }
        }

        private double ReadDouble(RandomAccessFileOrArray stream) {
            if (isBigEndian) {
                return stream.ReadDouble();
            }
            else {
                return stream.ReadDoubleLE();
            }
        }

        private static int ReadUnsignedShort(RandomAccessFileOrArray stream, bool isBigEndian) {
            if (isBigEndian) {
                return stream.ReadUnsignedShort();
            }
            else {
                return stream.ReadUnsignedShortLE();
            }
        }

        private static long ReadUnsignedInt(RandomAccessFileOrArray stream, bool isBigEndian) {
            if (isBigEndian) {
                return stream.ReadUnsignedInt();
            }
            else {
                return stream.ReadUnsignedIntLE();
            }
        }

        // Utilities
        /// <summary>
        /// Returns the number of image directories (subimages) stored in a
        /// given TIFF file, represented by a <c>SeekableStream</c>.
        /// </summary>
        /// <param name="stream">RandomAccessFileOrArray</param>
        /// <returns>
        /// The number of image directories (subimages) stored
        /// in a given TIFF file
        /// </returns>
        public static int GetNumDirectories(RandomAccessFileOrArray stream) {
            // Save stream pointer
            long pointer = stream.GetPosition();
            stream.Seek(0L);
            int endian = stream.ReadUnsignedShort();
            if (!IsValidEndianTag(endian)) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BAD_ENDIANNESS_TAG_0X4949_OR_0X4D4D);
            }
            bool isBigEndian = endian == 0x4d4d;
            int magic = ReadUnsignedShort(stream, isBigEndian);
            if (magic != 42) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.BAD_MAGIC_NUMBER_SHOULD_BE_42);
            }
            stream.Seek(4L);
            long offset = ReadUnsignedInt(stream, isBigEndian);
            int numDirectories = 0;
            while (offset != 0L) {
                ++numDirectories;
                // EOFException means IFD was probably not properly terminated.
                try {
                    stream.Seek(offset);
                    int entries = ReadUnsignedShort(stream, isBigEndian);
                    stream.Skip(12 * entries);
                    offset = ReadUnsignedInt(stream, isBigEndian);
                }
                catch (EndOfStreamException) {
                    numDirectories--;
                    break;
                }
            }
            // Reset stream pointer
            stream.Seek(pointer);
            return numDirectories;
        }

        /// <summary>
        /// Returns a boolean indicating whether the byte order used in the
        /// TIFF file is big-endian (i.e. whether the byte order is from
        /// the most significant to the least significant)
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the byte order used in the TIFF file is big-endian
        /// </returns>
        public virtual bool IsBigEndian() {
            return isBigEndian;
        }

        /// <summary>Returns the offset of the IFD corresponding to this <c>TIFFDirectory</c>.</summary>
        /// <returns>the offset of the IFD corresponding to this <c>TIFFDirectory</c>.</returns>
        public virtual long GetIFDOffset() {
            return IFDOffset;
        }

        /// <summary>
        /// Returns the offset of the next IFD after the IFD corresponding to this
        /// <c>TIFFDirectory</c>.
        /// </summary>
        /// <returns>
        /// the offset of the next IFD after the IFD corresponding to this
        /// <c>TIFFDirectory</c>.
        /// </returns>
        public virtual long GetNextIFDOffset() {
            return nextIFDOffset;
        }
    }
}
