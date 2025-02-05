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
            private readonly int segmentNumber;

            private long dataLength = -1;

            private int page = -1;

            private int[] referredToSegmentNumbers = null;

            private bool[] segmentRetentionFlags = null;

            private int type = -1;

            private bool deferredNonRetain = false;

            private int countOfReferredToSegments = -1;

            private byte[] data = null;

            private byte[] headerData = null;

            private bool pageAssociationSize = false;

            private int pageAssociationOffset = -1;

            public Jbig2Segment(int segment_number) {
                this.segmentNumber = segment_number;
            }

            public virtual int CompareTo(Jbig2SegmentReader.Jbig2Segment s) {
                return this.segmentNumber - s.segmentNumber;
            }

            /// <summary>Retrieves the data length of a JBig2Segment object.</summary>
            /// <returns>data length value</returns>
            public virtual long GetDataLength() {
                return dataLength;
            }

            /// <summary>Sets the data length of a JBig2Segment object.</summary>
            /// <param name="dataLength">data length value</param>
            public virtual void SetDataLength(long dataLength) {
                this.dataLength = dataLength;
            }

            /// <summary>Retrieves the page number of a JBig2Segment object.</summary>
            /// <returns>page number</returns>
            public virtual int GetPage() {
                return page;
            }

            /// <summary>Sets the page number of a JBig2Segment object.</summary>
            /// <param name="page">page number</param>
            public virtual void SetPage(int page) {
                this.page = page;
            }

            /// <summary>Retrieves the referred-to segment numbers of a JBig2Segment object.</summary>
            /// <returns>Every referred-to segment number</returns>
            public virtual int[] GetReferredToSegmentNumbers() {
                return referredToSegmentNumbers;
            }

            /// <summary>Sets the referred-to segment numbers of a JBig2Segment object.</summary>
            /// <param name="referredToSegmentNumbers">Referred-to segment numbers</param>
            public virtual void SetReferredToSegmentNumbers(int[] referredToSegmentNumbers) {
                this.referredToSegmentNumbers = referredToSegmentNumbers;
            }

            /// <summary>Retrieves segment retention flags of a JBig2Segment object.</summary>
            /// <returns>Every segment retention flag value</returns>
            public virtual bool[] GetSegmentRetentionFlags() {
                return segmentRetentionFlags;
            }

            /// <summary>Sets segment retention flags of a JBig2Segment object.</summary>
            /// <param name="segmentRetentionFlags">Segment retention flag values</param>
            public virtual void SetSegmentRetentionFlags(bool[] segmentRetentionFlags) {
                this.segmentRetentionFlags = segmentRetentionFlags;
            }

            /// <summary>Retrieves type of the JBig2Segment object.</summary>
            /// <returns>Type value</returns>
            public virtual int GetType() {
                return type;
            }

            /// <summary>Sets type of the JBig2Segment object.</summary>
            /// <param name="type">Type value</param>
            public virtual void SetType(int type) {
                this.type = type;
            }

            /// <summary>Retrieves whether the object is deferred without retention.</summary>
            /// <remarks>
            /// Retrieves whether the object is deferred without retention.
            /// Default value is false.
            /// </remarks>
            /// <returns>true if deferred without retention, false otherwise</returns>
            public virtual bool IsDeferredNonRetain() {
                return deferredNonRetain;
            }

            /// <summary>Sets whether the JBig2Segments object is deferred without retention.</summary>
            /// <param name="deferredNonRetain">true for deferred without retention, false otherwise</param>
            public virtual void SetDeferredNonRetain(bool deferredNonRetain) {
                this.deferredNonRetain = deferredNonRetain;
            }

            /// <summary>Retrieves the count of the referred-to segments.</summary>
            /// <returns>count of referred-to segments</returns>
            public virtual int GetCountOfReferredToSegments() {
                return countOfReferredToSegments;
            }

            /// <summary>Sets the count of referred-to segments of the JBig2Segment object.</summary>
            /// <param name="countOfReferredToSegments">count of referred segments</param>
            public virtual void SetCountOfReferredToSegments(int countOfReferredToSegments) {
                this.countOfReferredToSegments = countOfReferredToSegments;
            }

            /// <summary>Retrieves data of the JBig2Segment object.</summary>
            /// <returns>data bytes</returns>
            public virtual byte[] GetData() {
                return data;
            }

            /// <summary>Sets data of the JBig2Segment object.</summary>
            /// <param name="data">data bytes</param>
            public virtual void SetData(byte[] data) {
                this.data = data;
            }

            /// <summary>Retrieves header data of the JBig2Segment object.</summary>
            /// <returns>header data bytes</returns>
            public virtual byte[] GetHeaderData() {
                return headerData;
            }

            /// <summary>Sets header data of the JBig2Segment object.</summary>
            /// <param name="headerData">header date bytes</param>
            public virtual void SetHeaderData(byte[] headerData) {
                this.headerData = headerData;
            }

            /// <summary>Retrieves page association size of the JBig2Segment object.</summary>
            /// <returns>page association size value</returns>
            public virtual bool IsPageAssociationSize() {
                return pageAssociationSize;
            }

            /// <summary>Sets page association size of the JBig2Segment object.</summary>
            /// <param name="pageAssociationSize">page association size</param>
            public virtual void SetPageAssociationSize(bool pageAssociationSize) {
                this.pageAssociationSize = pageAssociationSize;
            }

            /// <summary>Retrieves the page association offset of the JBig2Segment object.</summary>
            /// <returns>page association offset value</returns>
            public virtual int GetPageAssociationOffset() {
                return pageAssociationOffset;
            }

            /// <summary>Sets page association offset of the JBig2Segment object.</summary>
            /// <param name="pageAssociationOffset">page association offset</param>
            public virtual void SetPageAssociationOffset(int pageAssociationOffset) {
                this.pageAssociationOffset = pageAssociationOffset;
            }

            /// <summary>Retrieves the segment number of the JBig2Segment object.</summary>
            /// <returns>segment number</returns>
            public virtual int GetSegmentNumber() {
                return segmentNumber;
            }
        }

        /// <summary>Inner class that holds information about a JBIG2 page.</summary>
        public class Jbig2Page {
            private readonly int page;

            private readonly Jbig2SegmentReader sr;

            private readonly IDictionary<int, Jbig2SegmentReader.Jbig2Segment> segs = new SortedDictionary<int, Jbig2SegmentReader.Jbig2Segment
                >();

            private int pageBitmapWidth = -1;

            private int pageBitmapHeight = -1;

            public Jbig2Page(int page, Jbig2SegmentReader sr) {
                this.page = page;
                this.sr = sr;
            }

            /// <summary>Retrieves the page number of the Jbig2Page object.</summary>
            /// <returns>page number</returns>
            public virtual int GetPage() {
                return page;
            }

            /// <summary>Retrieves page bitmap width of the Jbig2Page object.</summary>
            /// <returns>width of page bitmap</returns>
            public virtual int GetPageBitmapWidth() {
                return pageBitmapWidth;
            }

            /// <summary>Sets page bitmap width of the JBig2Page object.</summary>
            /// <param name="pageBitmapWidth">page bitmap width</param>
            public virtual void SetPageBitmapWidth(int pageBitmapWidth) {
                this.pageBitmapWidth = pageBitmapWidth;
            }

            /// <summary>Retrieves page bitmap height of the JBig2Page object.</summary>
            /// <returns>height of the page bitmap</returns>
            public virtual int GetPageBitmapHeight() {
                return pageBitmapHeight;
            }

            /// <summary>Sets the height of the page bitmap of a Jbig2Page object.</summary>
            /// <param name="pageBitmapHeight">height of the page bitmap</param>
            public virtual void SetPageBitmapHeight(int pageBitmapHeight) {
                this.pageBitmapHeight = pageBitmapHeight;
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
                    if (for_embedding && (s.GetType() == END_OF_FILE || s.GetType() == END_OF_PAGE)) {
                        continue;
                    }
                    if (for_embedding) {
                        // change the page association to page 1
                        byte[] headerDataEmb = CopyByteArray(s.GetHeaderData());
                        if (s.IsPageAssociationSize()) {
                            headerDataEmb[s.GetPageAssociationOffset()] = 0x0;
                            headerDataEmb[s.GetPageAssociationOffset() + 1] = 0x0;
                            headerDataEmb[s.GetPageAssociationOffset() + 2] = 0x0;
                            headerDataEmb[s.GetPageAssociationOffset() + 3] = 0x1;
                        }
                        else {
                            headerDataEmb[s.GetPageAssociationOffset()] = 0x1;
                        }
                        os.Write(headerDataEmb);
                    }
                    else {
                        os.Write(s.GetHeaderData());
                    }
                    os.Write(s.GetData());
                }
                os.Dispose();
                return os.ToArray();
            }

            public virtual void AddSegment(Jbig2SegmentReader.Jbig2Segment s) {
                segs.Put(s.GetSegmentNumber(), s);
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
                    segments.Put(tmp.GetSegmentNumber(), tmp);
                }
                while (this.ra.GetPosition() < this.ra.Length());
            }
            else {
                // D.2
                Jbig2SegmentReader.Jbig2Segment tmp;
                do {
                    tmp = ReadHeader();
                    segments.Put(tmp.GetSegmentNumber(), tmp);
                }
                while (tmp.GetType() != END_OF_FILE);
                foreach (int integer in segments.Keys) {
                    ReadSegment(segments.Get(integer));
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void ReadSegment(Jbig2SegmentReader.Jbig2Segment s) {
            int ptr = (int)ra.GetPosition();
            //TODO DEVSIX-6406 7.2.7 not supported
            if (s.GetDataLength() == unchecked((long)(0xffffffffl))) {
                return;
            }
            byte[] data = new byte[(int)s.GetDataLength()];
            ra.Read(data);
            s.SetData(data);
            if (s.GetType() == PAGE_INFORMATION) {
                int last = (int)ra.GetPosition();
                ra.Seek(ptr);
                int page_bitmap_width = ra.ReadInt();
                int page_bitmap_height = ra.ReadInt();
                ra.Seek(last);
                Jbig2SegmentReader.Jbig2Page p = pages.Get(s.GetPage());
                if (p == null) {
                    throw new iText.IO.Exceptions.IOException("Referring to widht or height of a page we haven't seen yet: {0}"
                        ).SetMessageParams(s.GetPage());
                }
                p.SetPageBitmapWidth(page_bitmap_width);
                p.SetPageBitmapHeight(page_bitmap_height);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual Jbig2SegmentReader.Jbig2Segment ReadHeader() {
            int ptr = (int)ra.GetPosition();
            // 7.2.1
            int segment_number = ra.ReadInt();
            Jbig2SegmentReader.Jbig2Segment s = new Jbig2SegmentReader.Jbig2Segment(segment_number);
            // 7.2.3
            int segment_header_flags = ra.Read();
            bool deferred_non_retain = (segment_header_flags & 0x80) == 0x80;
            s.SetDeferredNonRetain(deferred_non_retain);
            bool page_association_size = (segment_header_flags & 0x40) == 0x40;
            int segment_type = segment_header_flags & 0x3f;
            s.SetType(segment_type);
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
            s.SetSegmentRetentionFlags(segment_retention_flags);
            s.SetCountOfReferredToSegments(count_of_referred_to_segments);
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
            s.SetReferredToSegmentNumbers(referred_to_segment_numbers);
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
            s.SetPage(segment_page_association);
            // so we can change the page association at embedding time.
            s.SetPageAssociationSize(page_association_size);
            s.SetPageAssociationOffset(page_association_offset);
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
            s.SetDataLength(segment_data_length);
            int end_ptr = (int)ra.GetPosition();
            ra.Seek(ptr);
            byte[] header_data = new byte[end_ptr - ptr];
            ra.Read(header_data);
            s.SetHeaderData(header_data);
            return s;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

        public virtual int NumberOfPages() {
            return pages.Count;
        }

        public virtual int GetPageHeight(int i) {
            return pages.Get(i).GetPageBitmapHeight();
        }

        public virtual int GetPageWidth(int i) {
            return pages.Get(i).GetPageBitmapWidth();
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
                    if (for_embedding && (s.GetType() == END_OF_FILE || s.GetType() == END_OF_PAGE)) {
                        continue;
                    }
                    os.Write(s.GetHeaderData());
                    os.Write(s.GetData());
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
