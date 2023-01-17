/*
*
* This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
* Authors: Bruno Lowagie, Paulo Soares, et al.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License version 3
* as published by the Free Software Foundation with the addition of the
* following permission added to Section 15 as permitted in Section 7(a):
* FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
* ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
* OF THIRD PARTY RIGHTS
*
* This program is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
* or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU Affero General Public License for more details.
* You should have received a copy of the GNU Affero General Public License
* along with this program; if not, see http://www.gnu.org/licenses or write to
* the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
* Boston, MA, 02110-1301 USA, or download the license from the following URL:
* http://itextpdf.com/terms-of-use/
*
* The interactive user interfaces in modified source and object code versions
* of this program must display Appropriate Legal Notices, as required under
* Section 5 of the GNU Affero General Public License.
*
* In accordance with Section 7(b) of the GNU Affero General Public License,
* a covered work must retain the producer line in every PDF that is created
* or manipulated using iText.
*
* You can be released from the requirements of the license by purchasing
* a commercial license. Buying such a license is mandatory as soon as you
* develop commercial activities involving the iText software without
* disclosing the source code of your own applications.
* These activities include: offering paid services to customers as an ASP,
* serving PDFs on the fly in a web application, shipping iText with a closed
* source product.
*
* For more information, please contact iText Software Corp. at this
* address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;

namespace iText.IO.Colors {
    /// <summary>Class used to represented the International Color Consortium profile</summary>
    public class IccProfile {
        protected internal byte[] data;

        protected internal int numComponents;

        private static IDictionary<String, int?> cstags = new Dictionary<String, int?>();

        protected internal IccProfile() {
        }

        /// <summary>Construct an icc profile from the passed byte[], using the passed number of components.</summary>
        /// <param name="data">byte[] containing the raw icc profile data</param>
        /// <param name="numComponents">number of components the profile contains</param>
        /// <returns>IccProfile constructed from the data</returns>
        public static iText.IO.Colors.IccProfile GetInstance(byte[] data, int numComponents) {
            if (data.Length < 128 || data[36] != 0x61 || data[37] != 0x63 || data[38] != 0x73 || data[39] != 0x70) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile);
            }
            iText.IO.Colors.IccProfile icc = new iText.IO.Colors.IccProfile();
            icc.data = data;
            int? cs;
            cs = GetIccNumberOfComponents(data);
            int nc = cs == null ? 0 : (int)cs;
            icc.numComponents = nc;
            // invalid ICC
            if (nc != numComponents) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.IccProfileContains0ComponentsWhileImageDataContains1Components
                    ).SetMessageParams(nc, numComponents);
            }
            return icc;
        }

        /// <summary>Construct an icc profile from the passed byte[], using the passed number of components.</summary>
        /// <param name="data">byte[] containing the raw icc profile data</param>
        /// <returns>IccProfile constructed from the data</returns>
        public static iText.IO.Colors.IccProfile GetInstance(byte[] data) {
            int? cs;
            cs = GetIccNumberOfComponents(data);
            int numComponents = cs == null ? 0 : (int)cs;
            return GetInstance(data, numComponents);
        }

        /// <summary>Construct an icc profile from the passed random-access file or array.</summary>
        /// <param name="file">random-access file or array containing the profile</param>
        /// <returns>IccProfile constructed from the data</returns>
        public static iText.IO.Colors.IccProfile GetInstance(RandomAccessFileOrArray file) {
            try {
                byte[] head = new byte[128];
                int remain = head.Length;
                int ptr = 0;
                while (remain > 0) {
                    int n = file.Read(head, ptr, remain);
                    if (n < 0) {
                        throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile);
                    }
                    remain -= n;
                    ptr += n;
                }
                if (head[36] != 0x61 || head[37] != 0x63 || head[38] != 0x73 || head[39] != 0x70) {
                    throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile);
                }
                remain = (head[0] & 0xff) << 24 | (head[1] & 0xff) << 16 | (head[2] & 0xff) << 8 | head[3] & 0xff;
                byte[] icc = new byte[remain];
                Array.Copy(head, 0, icc, 0, head.Length);
                remain -= head.Length;
                ptr = head.Length;
                while (remain > 0) {
                    int n = file.Read(icc, ptr, remain);
                    if (n < 0) {
                        throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile);
                    }
                    remain -= n;
                    ptr += n;
                }
                return GetInstance(icc);
            }
            catch (Exception ex) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile, ex);
            }
        }

        /// <summary>Construct an icc profile from the passed InputStream.</summary>
        /// <param name="stream">inputstream containing the profile</param>
        /// <returns>IccProfile constructed from the data</returns>
        public static iText.IO.Colors.IccProfile GetInstance(Stream stream) {
            RandomAccessFileOrArray raf;
            try {
                raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(stream));
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile, e);
            }
            return GetInstance(raf);
        }

        /// <summary>Construct an icc profile from the file found at the passed path</summary>
        /// <param name="filename">path to the file contaning the profile</param>
        /// <returns>IccProfile constructed from the data</returns>
        public static iText.IO.Colors.IccProfile GetInstance(String filename) {
            RandomAccessFileOrArray raf;
            try {
                raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(filename));
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile, e);
            }
            return GetInstance(raf);
        }

        /// <summary>Get the Color space name of the icc profile found in the data.</summary>
        /// <param name="data">byte[] containing the icc profile</param>
        /// <returns>String containing the color space of the profile</returns>
        public static String GetIccColorSpaceName(byte[] data) {
            String colorSpace;
            try {
                colorSpace = iText.Commons.Utils.JavaUtil.GetStringForBytes(data, 16, 4, "US-ASCII");
            }
            catch (ArgumentException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile, e);
            }
            return colorSpace;
        }

        /// <summary>Get the device class of the icc profile found in the data.</summary>
        /// <param name="data">byte[] containing the icc profile</param>
        /// <returns>String containing the device class of the profile</returns>
        public static String GetIccDeviceClass(byte[] data) {
            String deviceClass;
            try {
                deviceClass = iText.Commons.Utils.JavaUtil.GetStringForBytes(data, 12, 4, "US-ASCII");
            }
            catch (ArgumentException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.InvalidIccProfile, e);
            }
            return deviceClass;
        }

        /// <summary>Get the number of color components of the icc profile found in the data.</summary>
        /// <param name="data">byte[] containing the icc profile</param>
        /// <returns>Number of color components</returns>
        public static int? GetIccNumberOfComponents(byte[] data) {
            return cstags.Get(GetIccColorSpaceName(data));
        }

        /// <summary>Get the icc color profile data.</summary>
        /// <returns>byte[] containing the data</returns>
        public virtual byte[] GetData() {
            return data;
        }

        /// <summary>Get the number of color components in the profile.</summary>
        /// <returns>number of components</returns>
        public virtual int GetNumComponents() {
            return numComponents;
        }

        static IccProfile() {
            cstags.Put("XYZ ", 3);
            cstags.Put("Lab ", 3);
            cstags.Put("Luv ", 3);
            cstags.Put("YCbr", 3);
            cstags.Put("Yxy ", 3);
            cstags.Put("RGB ", 3);
            cstags.Put("GRAY", 1);
            cstags.Put("HSV ", 3);
            cstags.Put("HLS ", 3);
            cstags.Put("CMYK", 4);
            cstags.Put("CMY ", 3);
            cstags.Put("2CLR", 2);
            cstags.Put("3CLR", 3);
            cstags.Put("4CLR", 4);
            cstags.Put("5CLR", 5);
            cstags.Put("6CLR", 6);
            cstags.Put("7CLR", 7);
            cstags.Put("8CLR", 8);
            cstags.Put("9CLR", 9);
            cstags.Put("ACLR", 10);
            cstags.Put("BCLR", 11);
            cstags.Put("CCLR", 12);
            cstags.Put("DCLR", 13);
            cstags.Put("ECLR", 14);
            cstags.Put("FCLR", 15);
        }
    }
}
