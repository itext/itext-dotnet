using System;

namespace Org.BouncyCastle.Crypto.Tls
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT)
    [Serializable]
#endif
    public class TlsException : Exception
	{
		public TlsException() : base() { }
		public TlsException(string message) : base(message) { }
		public TlsException(string message, Exception exception) : base(message, exception) { }
	}
}
