using System;
using iText.Commons.Exceptions;

namespace iText.Bouncycastlefips
{
    public class UnsupportedEncryptionFeatureException : ITextException
    {
        public const String ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS =
            "Encryption with certificated is currently not supported in Bouncy Castle FIPS mode.";
        
        public UnsupportedEncryptionFeatureException(string msg): base(msg)
        {
        }
    }
}