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
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Source;

namespace iText.IO.Codec {
    //TODO DEVSIX-6406: add support for indeterminate-segment-size value of dataLength
    /// <summary>
    /// Class to read a JBIG2 file at a basic level: understand all the segments,
    /// understand what segments belong to which pages, how many pages there are,
    /// what the width and height of each page is, and global segments if there
    /// are any.
    /// </summary>
    /// <remarks>
    /// Class to read a JBIG2 file at a basic level: understand all the segments,
    /// understand what segments belong to which pages, how many pages there are,
    /// what the width and height of each page is, and global segments if there
    /// are any.  Or: the minimum required to be able to take a normal sequential
    /// or random-access organized file, and be able to embed JBIG2 pages as images
    /// in a PDF.
    /// </remarks>
    public class Jbig2SegmentReader {
        //see 7.4.2.
        public const int SYMBOL_DICTIONARY = 0;

        //see 7.4.3.
        public const int INTERMEDIATE_TEXT_REGION = 4;

        //see 7.4.3.//see 7.4.3.
        public const int IMMEDIATE_TEXT_REGION = 6;

        //see 7.4.3.
        public const int IMMEDIATE_LOSSLESS_TEXT_REGION = 7;

        //see 7.4.4.
        public const int PATTERN_DICTIONARY = 16;

        //see 7.4.5.
        public const int INTERMEDIATE_HALFTONE_REGION = 20;

        //see 7.4.5.
        public const int IMMEDIATE_HALFTONE_REGION = 22;

        //see 7.4.5.
        public const int IMMEDIATE_LOSSLESS_HALFTONE_REGION = 23;

        //see 7.4.6.
        public const int INTERMEDIATE_GENERIC_REGION = 36;

        //see 7.4.6.
        public const int IMMEDIATE_GENERIC_REGION = 38;

        //see 7.4.6.
        public const int IMMEDIATE_LOSSLESS_GENERIC_REGION = 39;

        //see 7.4.7.
        public const int INTERMEDIATE_GENERIC_REFINEMENT_REGION = 40;

        //see 7.4.7.
        public const int IMMEDIATE_GENERIC_REFINEMENT_REGION = 42;

        //see 7.4.7.
        public const int IMMEDIATE_LOSSLESS_GENERIC_REFINEMENT_REGION = 43;

        //see 7.4.8.
        public const int PAGE_INFORMATION = 48;

        //see 7.4.9.
        public const int END_OF_PAGE = 49;

        //see 7.4.10.
        public const int END_OF_STRIPE = 50;

        //see 7.4.11.
        public const int END_OF_FILE = 51;

        //see 7.4.12.
        public const int PROFILES = 52;

        //see 7.4.13.
        public const int TABLES = 53;

        //see 7.4.14.
        public const int EXTENSION = 62;

        private readonly IDictionary<int, Jbig2SegmentReader.Jbig2Segment> segments = new SortedDictionary<int, Jbig2SegmentReader.Jbig2Segment
            >();

        private readonly IDictionary<int, Jbig2SegmentReader.Jbig2Page> pages = new SortedDictionary<int, Jbig2SegmentReader.Jbig2Page
            >();

        private readonly ICollection<Jbig2SegmentReader.Jbig2Segment> globals = new SortedSet<Jbig2SegmentReader.Jbig2Segment
            >();

        private RandomAccessFileOrArray ra;

        private bool sequential;

        private bool number_of_pages_known;

        private int number_of_pages = -1;

        private bool read = false;

        /// <summary>Inner class that holds information about a JBIG2 segment.</summary>
        public class Jbig2Segment : IComparable<Jbig2SegmentReader.Jbig2Segment> {
            public readonly int segmentNumber;

            public long dataLength = -1;

            public int page = -1;

            public int[] referredToSegmentNumbers = null;

            public bool[] segmentRetentionFlags = null;

            public int type = -1;

            public bool deferredNonRetain = false;

            public int countOfReferredToSegments = -1;

            public byte[] data = null;

            public byte[] headerData = null;

            public bool page_association_size = false;

            public int page_association_offset = -1;

            public Jbig2Segment(int segment_number) {
                this.segmentNumber = segment_number;
            }

            public virtual int CompareTo(Jbig2SegmentReader.Jbig2Segment s) {
                return this.segmentNumber - s.segmentNumber;
            }
        }

        /// <summary>Inner class that holds information about a JBIG2 page.</summary>
        public class Jbig2Page {
            public readonly int page;

            private readonly Jbig2SegmentReader sr;

            private readonly IDictionary<int, Jbig2SegmentReader.Jbig2Segment> segs = new SortedDictionary<int, Jbig2SegmentReader.Jbig2Segment
                >();

            public int pageBitmapWidth = -1;

            public int pageBitmapHeight = -1;

            public Jbig2Page(int page, Jbig2SegmentReader sr) {
                this.page = page;
                this.sr = sr;
            }

            /// <summary>
            /// return as a single byte array the header-data for each segment in segment number
            /// order, EMBEDDED organization, but I am putting the needed segments in SEQUENTIAL organization.
            /// </summary>
            /// <remarks>
            /// return as a single byte array the header-data for each segment in segment number
            /// order, EMBEDDED organization, but I am putting the needed segments in SEQUENTIAL organization.
            /// if for_embedding, skip the segment types that are known to be not for acrobat.
            /// </remarks>
            /// <param name="for_embedding">True if the bytes represents embedded data, false otherwise</param>
            /// <returns>a byte array</returns>
            public virtual byte[] GetData(bool for_embedding) {
                MemoryStream os = new MemoryStream();
                foreach (int sn in segs.Keys) {
                    Jbig2SegmentReader.Jbig2Segment s = segs.Get(sn);
                    // pdf reference 1.4, section 3.3.6 Jbig2Decode Filter
                    // D.3 Embedded organisation
                    if (for_embedding && (s.type == END_OF_FILE || s.type == END_OF_PAGE)) {
                        continue;
                    }
                    if (for_embedding) {
                        // change the page association to page 1
                        byte[] headerData_emb = CopyByteArray(s.headerData);
                        if (s.page_association_size) {
                            headerData_emb[s.page_association_offset] = 0x0;
                            headerData_emb[s.page_association_offset + 1] = 0x0;
                            headerData_emb[s.page_association_offset + 2] = 0x0;
                            headerData_emb[s.page_association_offset + 3] = 0x1;
                        }
                        else {
                            headerData_emb[s.page_association_offset] = 0x1;
                        }
                        os.Write(headerData_emb);
                    }
                    else {
                        os.Write(s.headerData);
                    }
                    os.Write(s.data);
                }
                os.Dispose();
                return os.ToArray();
            }

            public virtual void AddSegment(Jbig2SegmentReader.Jbig2Segment s) {
                segs.Put(s.segmentNumber, s);
            }
        }

        public Jbig2SegmentReader(RandomAccessFileOrArray ra) {
            this.ra = ra;
        }

        public static byte[] CopyByteArray(byte[] b) {
            byte[] bc = new byte[b.Length];
            Array.Copy(b, 0, bc, 0, b.Length);
            return bc;
        }

        public virtual void Read() {
            if (this.read) {
                throw new InvalidOperationException("already.attempted.a.read.on.this.jbig2.file");
            }
            this.read = true;
            ReadFileHeader();
            // Annex D
            if (this.sequential) {
                // D.1
                do {
                    Jbig2SegmentReader.Jbig2Segment tmp = ReadHeader();
                    ReadSegment(tmp);
                    segments.Put(tmp.segmentNumber, tmp);
                }
                while (this.ra.GetPosition() < this.ra.Length());
            }
            else {
                // D.2
                Jbig2SegmentReader.Jbig2Segment tmp;
                do {
                    tmp = ReadHeader();
                    segments.Put(tmp.segmentNumber, tmp);
                }
                while (tmp.type != END_OF_FILE);
                foreach (int integer in segments.Keys) {
                    ReadSegment(segments.Get(integer));
                }
            }
        }

        internal virtual void ReadSegment(Jbig2SegmentReader.Jbig2Segment s) {
            int ptr = (int)ra.GetPosition();
            //TODO DEVSIX-6406 7.2.7 not supported
            if (s.dataLength == unchecked((long)(0xffffffffl))) {
                return;
            }
            byte[] data = new byte[(int)s.dataLength];
            ra.Read(data);
            s.data = data;
            if (s.type == PAGE_INFORMATION) {
                int last = (int)ra.GetPosition();
                ra.Seek(ptr);
                int page_bitmap_width = ra.ReadInt();
                int page_bitmap_height = ra.ReadInt();
                ra.Seek(last);
                Jbig2SegmentReader.Jbig2Page p = pages.Get(s.page);
                if (p == null) {
                    throw new iText.IO.Exceptions.IOException("Referring to widht or height of a page we haven't seen yet: {0}"
                        ).SetMessageParams(s.page);
                }
                p.pageBitmapWidth = page_bitmap_width;
                p.pageBitmapHeight = page_bitmap_height;
            }
        }

        internal virtual Jbig2SegmentReader.Jbig2Segment ReadHeader() {
            int ptr = (int)ra.GetPosition();
            // 7.2.1
            int segment_number = ra.ReadInt();
            Jbig2SegmentReader.Jbig2Segment s = new Jbig2SegmentReader.Jbig2Segment(segment_number);
            // 7.2.3
            int segment_header_flags = ra.Read();
            bool deferred_non_retain = (segment_header_flags & 0x80) == 0x80;
            s.deferredNonRetain = deferred_non_retain;
            bool page_association_size = (segment_header_flags & 0x40) == 0x40;
            int segment_type = segment_header_flags & 0x3f;
            s.type = segment_type;
            //7.2.4
            int referred_to_byte0 = ra.Read();
            int count_of_referred_to_segments = (referred_to_byte0 & 0xE0) >> 5;
            int[] referred_to_segment_numbers = null;
            bool[] segment_retention_flags = null;
            if (count_of_referred_to_segments == 7) {
                // at least five bytes
                ra.Seek(ra.GetPosition() - 1);
                count_of_referred_to_segments = ra.ReadInt() & 0x1fffffff;
                segment_retention_flags = new bool[count_of_referred_to_segments + 1];
                int i = 0;
                int referred_to_current_byte = 0;
                do {
                    int j = i % 8;
                    if (j == 0) {
                        referred_to_current_byte = ra.Read();
                    }
                    segment_retention_flags[i] = (0x1 << j & referred_to_current_byte) >> j == 0x1;
                    i++;
                }
                while (i <= count_of_referred_to_segments);
            }
            else {
                if (count_of_referred_to_segments <= 4) {
                    // only one byte
                    segment_retention_flags = new bool[count_of_referred_to_segments + 1];
                    referred_to_byte0 &= 0x1f;
                    for (int i = 0; i <= count_of_referred_to_segments; i++) {
                        segment_retention_flags[i] = (0x1 << i & referred_to_byte0) >> i == 0x1;
                    }
                }
                else {
                    if (count_of_referred_to_segments == 5 || count_of_referred_to_segments == 6) {
                        throw new iText.IO.Exceptions.IOException("Count of referred-to segments has forbidden value in the header for segment {0} starting at {1}"
                            ).SetMessageParams(segment_number, ptr);
                    }
                }
            }
            s.segmentRetentionFlags = segment_retention_flags;
            s.countOfReferredToSegments = count_of_referred_to_segments;
            // 7.2.5
            referred_to_segment_numbers = new int[count_of_referred_to_segments + 1];
            for (int i = 1; i <= count_of_referred_to_segments; i++) {
                if (segment_number <= 256) {
                    referred_to_segment_numbers[i] = ra.Read();
                }
                else {
                    if (segment_number <= 65536) {
                        referred_to_segment_numbers[i] = ra.ReadUnsignedShort();
                    }
                    else {
                        referred_to_segment_numbers[i] = (int)ra.ReadUnsignedInt();
                    }
                }
            }
            s.referredToSegmentNumbers = referred_to_segment_numbers;
            // 7.2.6
            int segment_page_association;
            int page_association_offset = (int)ra.GetPosition() - ptr;
            if (page_association_size) {
                segment_page_association = ra.ReadInt();
            }
            else {
                segment_page_association = ra.Read();
            }
            if (segment_page_association < 0) {
                throw new iText.IO.Exceptions.IOException("Page {0} is invalid for segment {1} starting at {2}").SetMessageParams
                    (segment_page_association, segment_number, ptr);
            }
            s.page = segment_page_association;
            // so we can change the page association at embedding time.
            s.page_association_size = page_association_size;
            s.page_association_offset = page_association_offset;
            if (segment_page_association > 0 && !pages.ContainsKey(segment_page_association)) {
                pages.Put(segment_page_association, new Jbig2SegmentReader.Jbig2Page(segment_page_association, this));
            }
            if (segment_page_association > 0) {
                pages.Get(segment_page_association).AddSegment(s);
            }
            else {
                globals.Add(s);
            }
            // 7.2.7
            long segment_data_length = ra.ReadUnsignedInt();
            //TODO DEVSIX-6406 the 0xffffffff value that might be here, and how to understand those afflicted segments
            s.dataLength = segment_data_length;
            int end_ptr = (int)ra.GetPosition();
            ra.Seek(ptr);
            byte[] header_data = new byte[end_ptr - ptr];
            ra.Read(header_data);
            s.headerData = header_data;
            return s;
        }

        internal virtual void ReadFileHeader() {
            ra.Seek(0);
            byte[] idstring = new byte[8];
            ra.Read(idstring);
            byte[] refidstring = new byte[] { (byte)0x97, 0x4A, 0x42, 0x32, 0x0D, 0x0A, 0x1A, 0x0A };
            for (int i = 0; i < idstring.Length; i++) {
                if (idstring[i] != refidstring[i]) {
                    throw new iText.IO.Exceptions.IOException("File header idstring is not good at byte {0}").SetMessageParams
                        (i);
                }
            }
            int fileheaderflags = ra.Read();
            this.sequential = (fileheaderflags & 0x1) == 0x1;
            this.number_of_pages_known = (fileheaderflags & 0x2) == 0x0;
            if ((fileheaderflags & 0xfc) != 0x0) {
                throw new iText.IO.Exceptions.IOException("File header flags bits from 2 to 7 should be 0, some not");
            }
            if (this.number_of_pages_known) {
                this.number_of_pages = ra.ReadInt();
            }
        }

        public virtual int NumberOfPages() {
            return pages.Count;
        }

        public virtual int GetPageHeight(int i) {
            return pages.Get(i).pageBitmapHeight;
        }

        public virtual int GetPageWidth(int i) {
            return pages.Get(i).pageBitmapWidth;
        }

        public virtual Jbig2SegmentReader.Jbig2Page GetPage(int page) {
            return pages.Get(page);
        }

        public virtual byte[] GetGlobal(bool for_embedding) {
            MemoryStream os = new MemoryStream();
            byte[] streamBytes = null;
            try {
                foreach (Object element in globals) {
                    Jbig2SegmentReader.Jbig2Segment s = (Jbig2SegmentReader.Jbig2Segment)element;
                    if (for_embedding && (s.type == END_OF_FILE || s.type == END_OF_PAGE)) {
                        continue;
                    }
                    os.Write(s.headerData);
                    os.Write(s.data);
                }
                if (os.Length > 0) {
                    streamBytes = os.ToArray();
                }
                os.Dispose();
            }
            catch (System.IO.IOException e) {
                ILogger logger = ITextLogManager.GetLogger(typeof(Jbig2SegmentReader));
                logger.LogDebug(e.Message);
            }
            return streamBytes;
        }

        public override String ToString() {
            if (this.read) {
                return "Jbig2SegmentReader: number of pages: " + this.NumberOfPages();
            }
            else {
                return "Jbig2SegmentReader in indeterminate state.";
            }
        }
    }
}
