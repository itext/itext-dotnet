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
using Microsoft.Extensions.Logging;
using iText.Commons;

namespace iText.IO.Source {
    /// <summary>
    /// A RandomAccessSource that is based on a set of underlying sources,
    /// treating the sources as if they were a contiguous block of data.
    /// </summary>
    internal class GroupedRandomAccessSource : IRandomAccessSource {
        /// <summary>The underlying sources (along with some meta data to quickly determine where each source begins and ends)
        ///     </summary>
        private readonly GroupedRandomAccessSource.SourceEntry[] sources;

        /// <summary>Cached value to make multiple reads from the same underlying source more efficient</summary>
        private GroupedRandomAccessSource.SourceEntry currentSourceEntry;

        /// <summary>Cached size of the underlying channel</summary>
        private readonly long size;

        /// <summary>
        /// Constructs a new
        /// <see cref="GroupedRandomAccessSource"/>
        /// based on the specified set of sources
        /// </summary>
        /// <param name="sources">the sources used to build this group</param>
        public GroupedRandomAccessSource(IRandomAccessSource[] sources) {
            this.sources = new GroupedRandomAccessSource.SourceEntry[sources.Length];
            long totalSize = 0;
            for (int i = 0; i < sources.Length; i++) {
                this.sources[i] = new GroupedRandomAccessSource.SourceEntry(i, sources[i], totalSize);
                totalSize += sources[i].Length();
            }
            size = totalSize;
            currentSourceEntry = this.sources[sources.Length - 1];
            SourceInUse(currentSourceEntry.source);
        }

        /// <summary>For a given offset, return the index of the source that contains the specified offset.</summary>
        /// <remarks>
        /// For a given offset, return the index of the source that contains the specified offset.
        /// This is an optimization feature to help optimize the access of the correct source without having to iterate
        /// through every single source each time.  It is safe to always return 0, in which case the full set of sources
        /// will be searched.
        /// Subclasses should override this method if they are able to compute the source index more efficiently
        /// (for example
        /// <see cref="FileChannelRandomAccessSource"/>
        /// takes advantage of fixed size page buffers to compute the index)
        /// </remarks>
        /// <param name="offset">the offset</param>
        /// <returns>the index of the input source that contains the specified offset, or 0 if unknown</returns>
        protected internal virtual int GetStartingSourceIndex(long offset) {
            if (offset >= currentSourceEntry.firstByte) {
                return currentSourceEntry.index;
            }
            return 0;
        }

        /// <summary>
        /// Returns the SourceEntry that contains the byte at the specified offset
        /// sourceReleased is called as a notification callback so subclasses can take care of cleanup
        /// when the source is no longer the active source
        /// </summary>
        /// <param name="offset">the offset of the byte to look for</param>
        /// <returns>the SourceEntry that contains the byte at the specified offset</returns>
        private GroupedRandomAccessSource.SourceEntry GetSourceEntryForOffset(long offset) {
            if (offset >= size) {
                return null;
            }
            if (offset >= currentSourceEntry.firstByte && offset <= currentSourceEntry.lastByte) {
                return currentSourceEntry;
            }
            // hook to allow subclasses to release resources if necessary
            SourceReleased(currentSourceEntry.source);
            int startAt = GetStartingSourceIndex(offset);
            for (int i = startAt; i < sources.Length; i++) {
                if (offset >= sources[i].firstByte && offset <= sources[i].lastByte) {
                    currentSourceEntry = sources[i];
                    SourceInUse(currentSourceEntry.source);
                    return currentSourceEntry;
                }
            }
            return null;
        }

        /// <summary>Called when a given source is no longer the active source.</summary>
        /// <remarks>Called when a given source is no longer the active source.  This gives subclasses the abilty to release resources, if appropriate.
        ///     </remarks>
        /// <param name="source">the source that is no longer the active source</param>
        protected internal virtual void SourceReleased(IRandomAccessSource source) {
        }

        // by default, do nothing
        /// <summary>Called when a given source is about to become the active source.</summary>
        /// <remarks>Called when a given source is about to become the active source.  This gives subclasses the abilty to retrieve resources, if appropriate.
        ///     </remarks>
        /// <param name="source">the source that is about to become the active source</param>
        protected internal virtual void SourceInUse(IRandomAccessSource source) {
        }

        // by default, do nothing
        /// <summary>
        /// <inheritDoc/>
        /// The source that contains the byte at position is retrieved, the correct offset into that source computed, then the value
        /// from that offset in the underlying source is returned.
        /// </summary>
        public virtual int Get(long position) {
            GroupedRandomAccessSource.SourceEntry entry = GetSourceEntryForOffset(position);
            // if true, we have run out of data to read from
            if (entry == null) {
                return -1;
            }
            return entry.source.Get(entry.OffsetN(position));
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            GroupedRandomAccessSource.SourceEntry entry = GetSourceEntryForOffset(position);
            // if true, we have run out of data to read from
            if (entry == null) {
                return -1;
            }
            long offN = entry.OffsetN(position);
            int remaining = len;
            while (remaining > 0) {
                // if true, we have run out of data to read from
                if (entry == null) {
                    break;
                }
                if (offN > entry.source.Length()) {
                    break;
                }
                int count = entry.source.Get(offN, bytes, off, remaining);
                if (count == -1) {
                    break;
                }
                off += count;
                position += count;
                remaining -= count;
                offN = 0;
                entry = GetSourceEntryForOffset(position);
            }
            return remaining == len ? -1 : len - remaining;
        }

        /// <summary><inheritDoc/></summary>
        public virtual long Length() {
            return size;
        }

        /// <summary>
        /// <inheritDoc/>
        /// <br/>
        /// Closes all of the underlying sources.
        /// </summary>
        public virtual void Close() {
            System.IO.IOException firstThrownIOExc = null;
            foreach (GroupedRandomAccessSource.SourceEntry entry in sources) {
                try {
                    entry.source.Close();
                }
                catch (System.IO.IOException ex) {
                    if (firstThrownIOExc == null) {
                        firstThrownIOExc = ex;
                    }
                    else {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Source.GroupedRandomAccessSource));
                        logger.LogError(ex, iText.IO.Logs.IoLogMessageConstant.ONE_OF_GROUPED_SOURCES_CLOSING_FAILED);
                    }
                }
                catch (Exception ex) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Source.GroupedRandomAccessSource));
                    logger.LogError(ex, iText.IO.Logs.IoLogMessageConstant.ONE_OF_GROUPED_SOURCES_CLOSING_FAILED);
                }
            }
            if (firstThrownIOExc != null) {
                throw firstThrownIOExc;
            }
        }

        /// <summary>Used to track each source, along with useful meta data</summary>
        private class SourceEntry {
            /// <summary>The underlying source</summary>
            internal readonly IRandomAccessSource source;

            /// <summary>The first byte (in the coordinates of the GroupedRandomAccessSource) that this source contains</summary>
            internal readonly long firstByte;

            /// <summary>The last byte (in the coordinates of the GroupedRandomAccessSource) that this source contains</summary>
            internal readonly long lastByte;

            /// <summary>The index of this source in the GroupedRandomAccessSource</summary>
            internal readonly int index;

            /// <summary>Standard constructor</summary>
            /// <param name="index">the index</param>
            /// <param name="source">the source</param>
            /// <param name="offset">the offset of the source in the GroupedRandomAccessSource</param>
            public SourceEntry(int index, IRandomAccessSource source, long offset) {
                this.index = index;
                this.source = source;
                this.firstByte = offset;
                this.lastByte = offset + source.Length() - 1;
            }

            /// <summary>Given an absolute offset (in the GroupedRandomAccessSource coordinates), calculate the effective offset in the underlying source
            ///     </summary>
            /// <param name="absoluteOffset">the offset in the parent GroupedRandomAccessSource</param>
            /// <returns>the effective offset in the underlying source</returns>
            public virtual long OffsetN(long absoluteOffset) {
                return absoluteOffset - firstByte;
            }
        }
    }
}
