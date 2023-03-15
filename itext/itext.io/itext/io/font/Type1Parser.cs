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
using System.IO;
using System.Text;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font {
    internal class Type1Parser {
        private const String AFM_HEADER = "StartFontMetrics";

        private String afmPath;

        private String pfbPath;

        private byte[] pfbData;

        private byte[] afmData;

        private bool isBuiltInFont;

        private RandomAccessSourceFactory sourceFactory = new RandomAccessSourceFactory();

        /// <summary>Creates a new Type1 font file.</summary>
        /// <param name="afm">the AFM file if the input is made with a <c>byte</c> array</param>
        /// <param name="pfb">the PFB file if the input is made with a <c>byte</c> array</param>
        /// <param name="metricsPath">the name of one of the 14 built-in fonts or the location of an AFM file. The file must end in '.afm'
        ///     </param>
        public Type1Parser(String metricsPath, String binaryPath, byte[] afm, byte[] pfb) {
            this.afmData = afm;
            this.pfbData = pfb;
            this.afmPath = metricsPath;
            this.pfbPath = binaryPath;
        }

        public virtual RandomAccessFileOrArray GetMetricsFile() {
            isBuiltInFont = false;
            if (StandardFonts.IsStandardFont(afmPath)) {
                isBuiltInFont = true;
                byte[] buf = new byte[1024];
                Stream resource = null;
                try {
                    String resourcePath = FontResources.AFMS + afmPath + ".afm";
                    resource = ResourceUtil.GetResourceStream(resourcePath);
                    if (resource == null) {
                        throw new iText.IO.Exceptions.IOException("{0} was not found as resource.").SetMessageParams(resourcePath);
                    }
                    MemoryStream stream = new MemoryStream();
                    int read;
                    while ((read = resource.Read(buf)) >= 0) {
                        stream.Write(buf, 0, read);
                    }
                    buf = stream.ToArray();
                }
                finally {
                    if (resource != null) {
                        try {
                            resource.Dispose();
                        }
                        catch (Exception) {
                        }
                    }
                }
                return new RandomAccessFileOrArray(sourceFactory.CreateSource(buf));
            }
            else {
                if (afmPath != null) {
                    if (afmPath.ToLowerInvariant().EndsWith(".afm")) {
                        return new RandomAccessFileOrArray(sourceFactory.CreateBestSource(afmPath));
                    }
                    else {
                        if (afmPath.ToLowerInvariant().EndsWith(".pfm")) {
                            MemoryStream ba = new MemoryStream();
                            RandomAccessFileOrArray rf = new RandomAccessFileOrArray(sourceFactory.CreateBestSource(afmPath));
                            Pfm2afm.Convert(rf, ba);
                            rf.Close();
                            return new RandomAccessFileOrArray(sourceFactory.CreateSource(ba.ToArray()));
                        }
                        else {
                            throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException._1IsNotAnAfmOrPfmFontFile).SetMessageParams
                                (afmPath);
                        }
                    }
                }
                else {
                    if (afmData != null) {
                        RandomAccessFileOrArray rf = new RandomAccessFileOrArray(sourceFactory.CreateSource(afmData));
                        if (IsAfmFile(rf)) {
                            return rf;
                        }
                        else {
                            MemoryStream ba = new MemoryStream();
                            try {
                                Pfm2afm.Convert(rf, ba);
                            }
                            catch (Exception) {
                                throw new iText.IO.Exceptions.IOException("Invalid afm or pfm font file.");
                            }
                            finally {
                                rf.Close();
                            }
                            return new RandomAccessFileOrArray(sourceFactory.CreateSource(ba.ToArray()));
                        }
                    }
                    else {
                        throw new iText.IO.Exceptions.IOException("Invalid afm or pfm font file.");
                    }
                }
            }
        }

        public virtual RandomAccessFileOrArray GetPostscriptBinary() {
            if (pfbData != null) {
                return new RandomAccessFileOrArray(sourceFactory.CreateSource(pfbData));
            }
            else {
                if (pfbPath != null && pfbPath.ToLowerInvariant().EndsWith(".pfb")) {
                    return new RandomAccessFileOrArray(sourceFactory.CreateBestSource(pfbPath));
                }
                else {
                    pfbPath = afmPath.JSubstring(0, afmPath.Length - 3) + "pfb";
                    return new RandomAccessFileOrArray(sourceFactory.CreateBestSource(pfbPath));
                }
            }
        }

        public virtual bool IsBuiltInFont() {
            return isBuiltInFont;
        }

        public virtual String GetAfmPath() {
            return afmPath;
        }

        private bool IsAfmFile(RandomAccessFileOrArray raf) {
            StringBuilder builder = new StringBuilder(AFM_HEADER.Length);
            for (int i = 0; i < AFM_HEADER.Length; i++) {
                try {
                    builder.Append((char)raf.ReadByte());
                }
                catch (EndOfStreamException) {
                    raf.Seek(0);
                    return false;
                }
            }
            raf.Seek(0);
            return AFM_HEADER.Equals(builder.ToString());
        }
    }
}
