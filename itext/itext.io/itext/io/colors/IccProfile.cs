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
using System.IO;
using iText.IO.Exceptions;
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE);
            }
            iText.IO.Colors.IccProfile icc = new iText.IO.Colors.IccProfile();
            icc.data = data;
            int? cs;
            cs = GetIccNumberOfComponents(data);
            int nc = cs == null ? 0 : (int)cs;
            icc.numComponents = nc;
            // invalid ICC
            if (nc != numComponents) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.ICC_PROFILE_CONTAINS_COMPONENTS_WHILE_THE_IMAGE_DATA_CONTAINS_COMPONENTS
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
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE);
                    }
                    remain -= n;
                    ptr += n;
                }
                if (head[36] != 0x61 || head[37] != 0x63 || head[38] != 0x73 || head[39] != 0x70) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE);
                }
                remain = (head[0] & 0xff) << 24 | (head[1] & 0xff) << 16 | (head[2] & 0xff) << 8 | head[3] & 0xff;
                byte[] icc = new byte[remain];
                Array.Copy(head, 0, icc, 0, head.Length);
                remain -= head.Length;
                ptr = head.Length;
                while (remain > 0) {
                    int n = file.Read(icc, ptr, remain);
                    if (n < 0) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE);
                    }
                    remain -= n;
                    ptr += n;
                }
                return GetInstance(icc);
            }
            catch (Exception ex) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE, ex);
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE, e);
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE, e);
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE, e);
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
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INVALID_ICC_PROFILE, e);
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
