using System;

namespace iText.IO.Source {
    public class ThreadSafeRandomAccessSource : IRandomAccessSource {
        private readonly IRandomAccessSource source;

        private readonly Object lockObj = new Object();

        public ThreadSafeRandomAccessSource(IRandomAccessSource source) {
            this.source = source;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int Get(long position) {
            lock (lockObj) {
                return source.Get(position);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            lock (lockObj) {
                return source.Get(position, bytes, off, len);
            }
        }

        public virtual long Length() {
            lock (lockObj) {
                return source.Length();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void Close() {
            lock (lockObj) {
                source.Close();
            }
        }
    }
}
