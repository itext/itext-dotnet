using System;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf.Function.Utils {
    public abstract class SampleExtractor {
        public abstract long Extract(byte[] samples, int pos);

        public static SampleExtractor CreateExtractor(int bitsPerSample) {
            switch (bitsPerSample) {
                case 1:
                case 2:
                case 4: {
                    return new SampleExtractor.SampleBitsExtractor(bitsPerSample);
                }

                case 8:
                case 16:
                case 24:
                case 32: {
                    return new SampleExtractor.SampleBytesExtractor(bitsPerSample);
                }

                case 12: {
                    return new SampleExtractor.Sample12BitsExtractor();
                }

                default: {
                    throw new ArgumentException(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_BITS_PER_SAMPLE_INVALID_VALUE
                        );
                }
            }
        }

        private sealed class SampleBitsExtractor : SampleExtractor {
            private readonly int bitsPerSample;

            private readonly byte mask;

            public SampleBitsExtractor(int bitsPerSample) {
                this.bitsPerSample = bitsPerSample;
                this.mask = (byte)((1 << bitsPerSample) - 1);
            }

            public override long Extract(byte[] samples, int position) {
                int bitPos = position * bitsPerSample;
                int bytePos = bitPos >> 3;
                int shift = 8 - (bitPos & 7) - bitsPerSample;
                return (samples[bytePos] >> shift) & mask;
            }
        }

        private sealed class SampleBytesExtractor : SampleExtractor {
            private readonly int bytesPerSample;

            public SampleBytesExtractor(int bitsPerSample) {
                bytesPerSample = bitsPerSample >> 3;
            }

            public override long Extract(byte[] samples, int position) {
                int bytePos = position * bytesPerSample;
                long result = 0xff & samples[bytePos++];
                for (int i = 1; i < bytesPerSample; ++i) {
                    result = (result << 8) | (0xff & samples[bytePos++]);
                }
                return result;
            }
        }

        private sealed class Sample12BitsExtractor : SampleExtractor {
            public override long Extract(byte[] samples, int position) {
                int bitPos = position * 12;
                int bytePos = bitPos >> 3;
                if ((bitPos & 4) == 0) {
                    return ((0xff & samples[bytePos]) << 4) | ((0xf0 & samples[bytePos + 1]) >> 4);
                }
                else {
                    return ((0x0f & samples[bytePos]) << 8) | (0xff & samples[bytePos + 1]);
                }
            }
        }
    }
}
