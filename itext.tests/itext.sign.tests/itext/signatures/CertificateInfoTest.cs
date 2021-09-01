using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel;
using iText.Test;

namespace iText.Signatures {
    public class CertificateInfoTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void X500InvalidDirectoryConstructorTest() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CertificateInfo.X500Name("some_dir"));
        }

        [NUnit.Framework.Test]
        public virtual void X500ValidDirectoryConstructorTest() {
            CertificateInfo.X500Name name = new CertificateInfo.X500Name("some=dir,another=dir,some=value");
            IDictionary<String, IList<String>> values = name.GetFields();
            NUnit.Framework.Assert.AreEqual(2, values.Count);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("dir", "value"), values.Get("SOME"));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList("dir"), values.Get("ANOTHER"));
        }

        [NUnit.Framework.Test]
        public virtual void X500GetFieldTest() {
            CertificateInfo.X500Name name = new CertificateInfo.X500Name("some=value,another=dir,some=dir");
            NUnit.Framework.Assert.AreEqual("value", name.GetField("SOME"));
            NUnit.Framework.Assert.AreEqual("dir", name.GetField("ANOTHER"));
        }

        [NUnit.Framework.Test]
        public virtual void X500GetFieldArrayTest() {
            CertificateInfo.X500Name name = new CertificateInfo.X500Name("some=value,another=dir,some=dir");
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value", "dir"), name.GetFieldArray("SOME"));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList("dir"), name.GetFieldArray("ANOTHER"));
        }

        [NUnit.Framework.Test]
        public virtual void X509NameTokenizerNextTokenComplicatedTest() {
            CertificateInfo.X509NameTokenizer tokenizer = new CertificateInfo.X509NameTokenizer("quoted\",\"comma=escaped\\,comma_escaped\\\"quote"
                );
            String token = tokenizer.NextToken();
            NUnit.Framework.Assert.AreEqual("quoted,comma=escaped,comma_escaped\"quote", token);
            NUnit.Framework.Assert.IsNull(tokenizer.NextToken());
        }

        [NUnit.Framework.Test]
        public virtual void GetIssuerFieldsExceptionTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => CertificateInfo.GetIssuer(new 
                byte[] { 4, 8, 15, 16, 23, 42 }));
            NUnit.Framework.Assert.AreEqual("corrupted stream - out of bounds length found: 8 >= 6", exception.InnerException
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetSubjectExceptionTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => CertificateInfo.GetSubject(
                new byte[] { 4, 8, 15, 16, 23, 42 }));
            NUnit.Framework.Assert.AreEqual("corrupted stream - out of bounds length found: 8 >= 6", exception.InnerException
                .Message);
        }
    }
}
