namespace iText.IO.Util
{
    public sealed class AtomicLong
    {
        private long num;

        public long IncrementAndGet()
        {
            return System.Threading.Interlocked.Increment(ref num);
        }
    }
}
