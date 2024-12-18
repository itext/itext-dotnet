using System;
using System.IO;
using iText.StyledXmlParser.Resolver.Resource;

namespace iText.Signatures.Validation.Mocks {
    public class MockResourceRetriever : IResourceRetriever {
        private Func<Uri, byte[]> getByteArrayByUrlHandler = (u) => null;

        private Func<Uri, Stream> getInputStreamByUrlHandler = (u) => null;

        public virtual Stream GetInputStreamByUrl(Uri url) {
            return getInputStreamByUrlHandler.Invoke(url);
        }

        public virtual byte[] GetByteArrayByUrl(Uri url) {
            return getByteArrayByUrlHandler.Invoke(url);
        }

        public virtual MockResourceRetriever OnGetInputStreamByUrl(Func<Uri, Stream> handler) {
            getInputStreamByUrlHandler = handler;
            return this;
        }

        public virtual MockResourceRetriever OnGetByteArrayByUrl(Func<Uri, byte[]> handler) {
            getByteArrayByUrlHandler = handler;
            return this;
        }
    }
}
