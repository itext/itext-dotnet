namespace iText.Kernel.Counter.Data {
    public sealed class WaitTime {
        private readonly long time;

        private readonly long initial;

        private readonly long maximum;

        public WaitTime(long initial, long maximum)
            : this(initial, maximum, initial) {
        }

        public WaitTime(long initial, long maximum, long time) {
            this.initial = initial;
            this.maximum = maximum;
            this.time = time;
        }

        public long GetInitial() {
            return initial;
        }

        public long GetMaximum() {
            return maximum;
        }

        public long GetTime() {
            return time;
        }
    }
}
