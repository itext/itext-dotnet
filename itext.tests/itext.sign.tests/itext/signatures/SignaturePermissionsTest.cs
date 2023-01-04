/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class SignaturePermissionsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultValuesTest() {
            SignaturePermissions permissions = new SignaturePermissions(new PdfDictionary(), null);
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void TransformedMethodDocMDPIsPresentedTest() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithDocMDP = new PdfDictionary();
            dictWithDocMDP.Put(PdfName.TransformMethod, PdfName.DocMDP);
            dictWithDocMDP.Put(PdfName.TransformParams, new PdfDictionary());
            references.Add(dictWithDocMDP);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.IsTrue(permissions.IsCertification());
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void ActionIsPresentedTest() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            PdfName action = new PdfName("Name");
            PdfArray fields = new PdfArray();
            fields.Add(new PdfString("Value1"));
            fields.Add(new PdfString("Value2"));
            @params.Put(PdfName.Action, action);
            @params.Put(PdfName.Fields, fields);
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.AreEqual(1, permissions.GetFieldLocks().Count);
            SignaturePermissions.FieldLock fieldLock = permissions.GetFieldLocks()[0];
            NUnit.Framework.Assert.AreEqual(action, fieldLock.GetAction());
            NUnit.Framework.Assert.AreEqual(fields, fieldLock.GetFields());
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleActionsArePresentedTest() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            PdfName action = new PdfName("Name");
            PdfArray fields = new PdfArray();
            fields.Add(new PdfString("Value1"));
            fields.Add(new PdfString("Value2"));
            @params.Put(PdfName.Action, action);
            @params.Put(PdfName.Fields, fields);
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            references.Add(dictWithAction);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.AreEqual(3, permissions.GetFieldLocks().Count);
            foreach (SignaturePermissions.FieldLock fieldLock in permissions.GetFieldLocks()) {
                NUnit.Framework.Assert.AreEqual(action, fieldLock.GetAction());
                NUnit.Framework.Assert.AreEqual(fields, fieldLock.GetFields());
            }
            NUnit.Framework.Assert.IsTrue(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
        }

        [NUnit.Framework.Test]
        public virtual void PParamEqualsTo1Test() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            @params.Put(PdfName.P, new PdfNumber(1));
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.IsFalse(permissions.IsFillInAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
        }

        [NUnit.Framework.Test]
        public virtual void PParamEqualsTo2Test() {
            PdfDictionary dict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            @params.Put(PdfName.P, new PdfNumber(2));
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            dict.Put(PdfName.Reference, references);
            SignaturePermissions permissions = new SignaturePermissions(dict, null);
            NUnit.Framework.Assert.IsFalse(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.AreEqual(new List<Object>(), permissions.GetFieldLocks());
            NUnit.Framework.Assert.IsTrue(permissions.IsFillInAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
        }

        [NUnit.Framework.Test]
        public virtual void PreviousIsSetTest() {
            PdfDictionary previousDict = new PdfDictionary();
            PdfArray references = new PdfArray();
            PdfDictionary dictWithAction = new PdfDictionary();
            PdfDictionary @params = new PdfDictionary();
            @params.Put(PdfName.P, new PdfNumber(1));
            PdfName action = new PdfName("Name");
            PdfArray fields = new PdfArray();
            fields.Add(new PdfString("Value1"));
            fields.Add(new PdfString("Value2"));
            @params.Put(PdfName.Action, action);
            @params.Put(PdfName.Fields, fields);
            dictWithAction.Put(PdfName.TransformParams, @params);
            references.Add(dictWithAction);
            previousDict.Put(PdfName.Reference, references);
            SignaturePermissions previousPermissions = new SignaturePermissions(previousDict, null);
            SignaturePermissions permissions = new SignaturePermissions(new PdfDictionary(), previousPermissions);
            NUnit.Framework.Assert.AreEqual(1, permissions.GetFieldLocks().Count);
            SignaturePermissions.FieldLock fieldLock = permissions.GetFieldLocks()[0];
            NUnit.Framework.Assert.AreEqual(action, fieldLock.GetAction());
            NUnit.Framework.Assert.AreEqual(fields, fieldLock.GetFields());
            NUnit.Framework.Assert.IsFalse(permissions.IsAnnotationsAllowed());
            NUnit.Framework.Assert.IsFalse(permissions.IsCertification());
            NUnit.Framework.Assert.IsFalse(permissions.IsFillInAllowed());
        }
    }
}
