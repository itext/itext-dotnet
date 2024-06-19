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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
//\cond DO_NOT_DOCUMENT
    /// <summary>PdfWriter implementation which allows to create documents in memory and dump them on disk on purpose.
    ///     </summary>
    /// <remarks>
    /// PdfWriter implementation which allows to create documents in memory and dump them on disk on purpose.
    /// Currently it's private and used in automated tests only.
    /// </remarks>
    internal class MemoryFirstPdfWriter : PdfWriter {
        private const int MAX_ALLOWED_STREAMS = 100;

        private static IDictionary<String, iText.Kernel.Utils.MemoryFirstPdfWriter> waitingStreams = new ConcurrentDictionary
            <String, iText.Kernel.Utils.MemoryFirstPdfWriter>();

        private String filePath;

        private MemoryStream outStream;

//\cond DO_NOT_DOCUMENT
        internal MemoryFirstPdfWriter(String filename, WriterProperties properties)
            : this(filename, iText.Kernel.Utils.MemoryFirstPdfWriter.CreateBAOutputStream(), properties) {
        }
//\endcond

        private MemoryFirstPdfWriter(String filename, MemoryStream outputStream, WriterProperties properties)
            : base(outputStream, properties) {
            SetCloseStream(false);
            filePath = filename;
            outStream = outputStream;
            if (iText.Kernel.Utils.MemoryFirstPdfWriter.waitingStreams.Count >= MAX_ALLOWED_STREAMS) {
                throw new Exception("Too many PdfWriter's have been created. Verify that you call" + " CompareTool.cleanup where necessary"
                    );
            }
            iText.Kernel.Utils.MemoryFirstPdfWriter.waitingStreams.Put(filename, this);
        }

//\cond DO_NOT_DOCUMENT
        internal static iText.Kernel.Utils.MemoryFirstPdfWriter Get(String filename) {
            return iText.Kernel.Utils.MemoryFirstPdfWriter.waitingStreams.Get(filename);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void Cleanup(String path) {
            if (path == null) {
                throw new ArgumentException("Provided path is null");
            }
            foreach (String filePath in iText.Kernel.Utils.MemoryFirstPdfWriter.waitingStreams.Keys) {
                if (filePath.StartsWith(path)) {
                    iText.Kernel.Utils.MemoryFirstPdfWriter.waitingStreams.JRemove(filePath);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Dump() {
            Stream fos = FileUtil.GetFileOutputStream(filePath);
            outStream.WriteTo(fos);
            fos.Dispose();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual MemoryStream GetBAOutputStream() {
            return outStream;
        }
//\endcond

        private static MemoryStream CreateBAOutputStream() {
            return new MemoryStream();
        }
    }
//\endcond
}
