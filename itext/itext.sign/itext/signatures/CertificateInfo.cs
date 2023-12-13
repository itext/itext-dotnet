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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using iText.Kernel;

namespace iText.Signatures {
    /// <summary>
    /// Class containing static methods that allow you to get information from
    /// an X509 Certificate: the issuer and the subject.
    /// </summary>
    public class CertificateInfo {
        // Inner classes
        /// <summary>Class that holds an X509 name.</summary>
        public class X500Name {
            /// <summary>Country code - StringType(SIZE(2)).</summary>
            public static readonly DerObjectIdentifier C = new DerObjectIdentifier("2.5.4.6");

            /// <summary>Organization - StringType(SIZE(1..64)).</summary>
            public static readonly DerObjectIdentifier O = new DerObjectIdentifier("2.5.4.10");

            /// <summary>Organizational unit name - StringType(SIZE(1..64)).</summary>
            public static readonly DerObjectIdentifier OU = new DerObjectIdentifier("2.5.4.11");

            /// <summary>Title.</summary>
            public static readonly DerObjectIdentifier T = new DerObjectIdentifier("2.5.4.12");

            /// <summary>Common name - StringType(SIZE(1..64)).</summary>
            public static readonly DerObjectIdentifier CN = new DerObjectIdentifier("2.5.4.3");

            /// <summary>Device serial number name - StringType(SIZE(1..64)).</summary>
            public static readonly DerObjectIdentifier SN = new DerObjectIdentifier("2.5.4.5");

            /// <summary>Locality name - StringType(SIZE(1..64)).</summary>
            public static readonly DerObjectIdentifier L = new DerObjectIdentifier("2.5.4.7");

            /// <summary>State, or province name - StringType(SIZE(1..64)).</summary>
            public static readonly DerObjectIdentifier ST = new DerObjectIdentifier("2.5.4.8");

            /// <summary>Naming attribute of type X520name.</summary>
            public static readonly DerObjectIdentifier SURNAME = new DerObjectIdentifier("2.5.4.4");

            /// <summary>Naming attribute of type X520name.</summary>
            public static readonly DerObjectIdentifier GIVENNAME = new DerObjectIdentifier("2.5.4.42");

            /// <summary>Naming attribute of type X520name.</summary>
            public static readonly DerObjectIdentifier INITIALS = new DerObjectIdentifier("2.5.4.43");

            /// <summary>Naming attribute of type X520name.</summary>
            public static readonly DerObjectIdentifier GENERATION = new DerObjectIdentifier("2.5.4.44");

            /// <summary>Naming attribute of type X520name.</summary>
            public static readonly DerObjectIdentifier UNIQUE_IDENTIFIER = new DerObjectIdentifier("2.5.4.45");

            /// <summary>Email address (RSA PKCS#9 extension) - IA5String.</summary>
            /// <remarks>
            /// Email address (RSA PKCS#9 extension) - IA5String.
            /// <para />
            /// Note: if you're trying to be ultra orthodox, don't use this! It shouldn't be in here.
            /// </remarks>
            public static readonly DerObjectIdentifier EmailAddress = new DerObjectIdentifier("1.2.840.113549.1.9.1");

            /// <summary>Email address in Verisign certificates.</summary>
            public static readonly DerObjectIdentifier E = EmailAddress;

            /// <summary>Object identifier.</summary>
            public static readonly DerObjectIdentifier DC = new DerObjectIdentifier("0.9.2342.19200300.100.1.25");

            /// <summary>LDAP User id.</summary>
            public static readonly DerObjectIdentifier UID = new DerObjectIdentifier("0.9.2342.19200300.100.1.1");

            /// <summary>A Map with default symbols.</summary>
            public static readonly IDictionary<DerObjectIdentifier, String> DefaultSymbols = new Dictionary<DerObjectIdentifier
                , String>();

            static X500Name() {
                DefaultSymbols.Put(C, "C");
                DefaultSymbols.Put(O, "O");
                DefaultSymbols.Put(T, "T");
                DefaultSymbols.Put(OU, "OU");
                DefaultSymbols.Put(CN, "CN");
                DefaultSymbols.Put(L, "L");
                DefaultSymbols.Put(ST, "ST");
                DefaultSymbols.Put(SN, "SN");
                DefaultSymbols.Put(EmailAddress, "E");
                DefaultSymbols.Put(DC, "DC");
                DefaultSymbols.Put(UID, "UID");
                DefaultSymbols.Put(SURNAME, "SURNAME");
                DefaultSymbols.Put(GIVENNAME, "GIVENNAME");
                DefaultSymbols.Put(INITIALS, "INITIALS");
                DefaultSymbols.Put(GENERATION, "GENERATION");
            }

            /// <summary>A Map with values.</summary>
            public IDictionary<String, IList<String>> values = new Dictionary<String, IList<String>>();

            /// <summary>Constructs an X509 name.</summary>
            /// <param name="seq">an ASN1 Sequence</param>
            public X500Name(Asn1Sequence seq) {
                IEnumerator e = seq.GetEnumerator();
                while (e.MoveNext()) {
                    Asn1Set set = (Asn1Set)e.Current;
                    for (int i = 0; i < set.Count; i++) {
                        Asn1Sequence s = (Asn1Sequence)set[i];
                        String id = DefaultSymbols.Get((DerObjectIdentifier)s[0]);
                        if (id == null) {
                            continue;
                        }
                        IList<String> vs = values.Get(id);
                        if (vs == null) {
                            vs = new List<String>();
                            values.Put(id, vs);
                        }
                        vs.Add(((DerStringBase)s[1]).GetString());
                    }
                }
            }

            /// <summary>Constructs an X509 name.</summary>
            /// <param name="dirName">a directory name</param>
            public X500Name(String dirName) {
                CertificateInfo.X509NameTokenizer nTok = new CertificateInfo.X509NameTokenizer(dirName);
                while (nTok.HasMoreTokens()) {
                    String token = nTok.NextToken();
                    int index = token.IndexOf('=');
                    if (index == -1) {
                        throw new ArgumentException();
                    }
                    /*MessageLocalization.getComposedMessage("badly.formated.directory.string")*/
                    String id = token.JSubstring(0, index).ToUpperInvariant();
                    String value = token.Substring(index + 1);
                    IList<String> vs = values.Get(id);
                    if (vs == null) {
                        vs = new List<String>();
                        values.Put(id, vs);
                    }
                    vs.Add(value);
                }
            }

            /// <summary>Gets the first entry from the field array retrieved from the values Map.</summary>
            /// <param name="name">the field name</param>
            /// <returns>the (first) field value</returns>
            public virtual String GetField(String name) {
                IList<String> vs = values.Get(name);
                return vs == null ? null : (String)vs[0];
            }

            /// <summary>Gets a field array from the values Map.</summary>
            /// <param name="name">The field name</param>
            /// <returns>List</returns>
            public virtual IList<String> GetFieldArray(String name) {
                return values.Get(name);
            }

            /// <summary>Getter for values.</summary>
            /// <returns>Map with the fields of the X509 name</returns>
            public virtual IDictionary<String, IList<String>> GetFields() {
                return values;
            }

            public override String ToString() {
                return values.ToString();
            }
        }

        /// <summary>
        /// Class for breaking up an X500 Name into it's component tokens, similar to
        /// <see cref="iText.IO.Util.StringTokenizer"/>.
        /// </summary>
        /// <remarks>
        /// Class for breaking up an X500 Name into it's component tokens, similar to
        /// <see cref="iText.IO.Util.StringTokenizer"/>.
        /// We need this class as some of the lightweight Java environments don't support classes such as StringTokenizer.
        /// </remarks>
        public class X509NameTokenizer {
            private String oid;

            private int index;

            private StringBuilder buf = new StringBuilder();

            /// <summary>Creates an X509NameTokenizer.</summary>
            /// <param name="oid">the oid that needs to be parsed</param>
            public X509NameTokenizer(String oid) {
                this.oid = oid;
                this.index = -1;
            }

            /// <summary>Checks if the tokenizer has any tokens left.</summary>
            /// <returns>true if there are any tokens left, false if there aren't</returns>
            public virtual bool HasMoreTokens() {
                return index != oid.Length;
            }

            /// <summary>Returns the next token.</summary>
            /// <returns>the next token</returns>
            public virtual String NextToken() {
                if (index == oid.Length) {
                    return null;
                }
                int end = index + 1;
                bool quoted = false;
                bool escaped = false;
                buf.Length = 0;
                while (end != oid.Length) {
                    char c = oid[end];
                    if (c == '"') {
                        if (!escaped) {
                            quoted = !quoted;
                        }
                        else {
                            buf.Append(c);
                        }
                        escaped = false;
                    }
                    else {
                        if (escaped || quoted) {
                            buf.Append(c);
                            escaped = false;
                        }
                        else {
                            if (c == '\\') {
                                escaped = true;
                            }
                            else {
                                if (c == ',') {
                                    break;
                                }
                                else {
                                    buf.Append(c);
                                }
                            }
                        }
                    }
                    end++;
                }
                index = end;
                return buf.ToString().Trim();
            }
        }

        // Certificate issuer
        /// <summary>Get the issuer fields from an X509 Certificate.</summary>
        /// <param name="cert">an X509Certificate</param>
        /// <returns>an X500Name</returns>
        public static CertificateInfo.X500Name GetIssuerFields(X509Certificate cert) {
            try {
                return new CertificateInfo.X500Name((Asn1Sequence)CertificateInfo.GetIssuer(cert.GetTbsCertificate()));
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>Get the "issuer" from the TBSCertificate bytes that are passed in.</summary>
        /// <param name="enc">a TBSCertificate in a byte array</param>
        /// <returns>an ASN1Primitive</returns>
        public static Asn1Object GetIssuer(byte[] enc) {
            try {
                Asn1InputStream @in = new Asn1InputStream(new MemoryStream(enc));
                Asn1Sequence seq = (Asn1Sequence)@in.ReadObject();
                return (Asn1Object)seq[seq[0] is Asn1TaggedObject ? 3 : 2];
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }

        // Certificate Subject
        /// <summary>Get the subject fields from an X509 Certificate.</summary>
        /// <param name="cert">an X509Certificate</param>
        /// <returns>an X500Name</returns>
        public static CertificateInfo.X500Name GetSubjectFields(X509Certificate cert) {
            try {
                if (cert != null) {
                    return new CertificateInfo.X500Name((Asn1Sequence)CertificateInfo.GetSubject(cert.GetTbsCertificate()));
                }
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            return null;
        }

        /// <summary>Get the "subject" from the TBSCertificate bytes that are passed in.</summary>
        /// <param name="enc">A TBSCertificate in a byte array</param>
        /// <returns>a ASN1Primitive</returns>
        public static Asn1Object GetSubject(byte[] enc) {
            try {
                Asn1InputStream @in = new Asn1InputStream(new MemoryStream(enc));
                Asn1Sequence seq = (Asn1Sequence)@in.ReadObject();
                return (Asn1Object)seq[seq[0] is Asn1TaggedObject ? 5 : 4];
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }
    }
}
