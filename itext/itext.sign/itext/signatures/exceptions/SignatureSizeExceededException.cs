using System;
using System.Linq;
using System.Collections.Generic;

namespace iText.Signatures.Exceptions {
    public sealed class SignatureSizeExceededException : Exception {
        public byte[] EncodedSignature { get; private set; }

        public ICollection<byte[]> OcspList { get; private set; }

        public ICollection<byte[]> CrlBytes { get; private set; }

        internal SignatureSizeExceededException(string message, byte[] encodedSig, ICollection<byte[]> ocsp, ICollection<byte[]> crl) : base(message)
        {
            EncodedSignature = encodedSig != null ? encodedSig : new byte[0];
            OcspList = ocsp != null ? ocsp : new List<byte[]>();
            CrlBytes = crl != null ? crl : new List<byte[]>();
        }
    }
}
